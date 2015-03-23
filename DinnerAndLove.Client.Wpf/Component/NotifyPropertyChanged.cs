using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DinnerAndLove.Client.Wpf.Component
{
    public class NotifyPropertyChanged : INotifyPropertyChanged
    {
        #region Members

        static readonly Dictionary<Type, List<PropertyDependency>> _propertyDependencies;

        #endregion

        #region Static Constructor

        static NotifyPropertyChanged()
        {
            _propertyDependencies = new Dictionary<Type, List<PropertyDependency>>();
        }

        #endregion

        #region Static Methods

        protected static void ActivatePropertyDependencies(Type type)
        {
            if(_propertyDependencies.ContainsKey(type))
            {
                return;
            }

            var dependencyList = new List<PropertyDependency>();

            var typeProperties = type.GetProperties();

            foreach (var property in typeProperties)
            {
                var directDependencies = GetDependantProperties(typeProperties, property.Name);

                if(directDependencies.Any())
                {
                    var dependener = new PropertyDependency(property.Name)
                    {
                        DependantProperties = new HashSet<string>(directDependencies)
                    };

                    dependencyList.Add(dependener);
                }
            }

            foreach (var propertyDependency in dependencyList)
            {
                CollectDependencyHierarchy(dependencyList, propertyDependency);
            }

            _propertyDependencies.Add(type, dependencyList);
        }

        #endregion

        #region Protected Methods

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = ExtractPropertyName(propertyExpression);

            RaisePropertyChanged(propertyName);
        }

        protected string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression.", "propertyExpression");
            }

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property.", "propertyExpression");
            }

            if (!property.DeclaringType.IsInstanceOfType(this))
            {
                throw new ArgumentException("The referenced property belongs to a different type.", "propertyExpression");
            }

            var getMethod = property.GetGetMethod(true);
            if (getMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException("The referenced property does not have a get method.", "propertyExpression");
            }

            if (getMethod.IsStatic)
            {
                throw new ArgumentException("The referenced property is a static property.", "propertyExpression");
            }

            return memberExpression.Member.Name;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handlers = PropertyChanged;

            if (handlers != null)
            {
                handlers(this, new PropertyChangedEventArgs(propertyName));

                RaisePropertyChangedForDependencies(handlers, propertyName);
            }
        }

        protected void RaisePropertyChangedForDependencies(PropertyChangedEventHandler handlers, string propertyName)
        {
            var type = GetType();

            if (_propertyDependencies.ContainsKey(type))
            {
                foreach (var property in _propertyDependencies[type].Where(item => item.PropertyName == propertyName).SelectMany(item => item.DependantProperties))
                {
                    handlers(this, new PropertyChangedEventArgs(property));
                }
            }
        }

        #endregion

        #region Private Methods

        private static IEnumerable<string> GetDependantProperties(IEnumerable<PropertyInfo> properties, string inputName)
        {
            return from property in properties
                   where property.GetCustomAttributes(typeof(DependsUponAttribute), true).Cast<DependsUponAttribute>()
                         .Any(attribute => attribute.DependancyName == inputName)
                   select property.Name;
        }

        private static void CollectDependencyHierarchy(List<PropertyDependency> dependantProperties, PropertyDependency dependantProperty)
        {
            var previousCount = 0;

            while(previousCount != dependantProperty.DependantProperties.Count)
            {
                previousCount = dependantProperty.DependantProperties.Count;

                foreach (var property in dependantProperty.DependantProperties.ToList())
                {
                    foreach (var dependancy in dependantProperties.Where(item => item.PropertyName == property).SelectMany(item => item.DependantProperties))
                    {
                        if (!dependantProperty.DependantProperties.Contains(dependancy))
                        {
                            dependantProperty.DependantProperties.Add(dependancy);
                        }
                    }
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region DependsUponAttribute

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        protected class DependsUponAttribute : Attribute
        {
            public string DependancyName { get; private set; }

            public DependsUponAttribute(string propertyName)
            {
                DependancyName = propertyName;
            }
        }

        #region PropertyDependency

        private class PropertyDependency
        {
            public PropertyDependency(string propertyName)
            {
                PropertyName = propertyName;
            }

            public string PropertyName
            {
                get;
                private set;
            }

            public HashSet<string> DependantProperties
            {
                get;
                set;
            }

            public override int GetHashCode()
            {
                return PropertyName.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as PropertyDependency;

                if(other == null || PropertyName != other.PropertyName)
                {
                    return false;
                }

                return true;
            }
        }

        #endregion

        #endregion
    }
}
