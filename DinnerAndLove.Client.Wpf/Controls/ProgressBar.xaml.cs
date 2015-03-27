using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DinnerAndLove.Client.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class LoadingProgressControl : UserControl
    {
        #region Dependency properties

        public static readonly DependencyProperty ProgressCircleDiameterProperty = DependencyProperty.Register(
            "ProgressCircleDiameter",
            typeof(double),
            typeof(LoadingProgressControl),
            new PropertyMetadata(20.0));

        public static readonly DependencyProperty CanvasSizeProperty = DependencyProperty.Register(
            "CanvasSize",
            typeof(double),
            typeof(LoadingProgressControl),
            new PropertyMetadata(120.0));

        public static readonly DependencyProperty ProgressCircleFillColorProperty = DependencyProperty.Register(
            "ProgressCircleFillColor",
            typeof(Brush),
            typeof(LoadingProgressControl),
            new PropertyMetadata(Brushes.Black));

        #endregion

        #region Members

        private readonly DispatcherTimer _animationTimer;

        #endregion

        #region Constructor

        public LoadingProgressControl()
        {
            InitializeComponent();

            _animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher)
            {
                Interval = new TimeSpan(0, 0, 0, 0, 100)
            };
        }

        #endregion

        #region Properties

        public double ProgressCircleDiameter
        {
            get
            {
                return (double)GetValue(ProgressCircleDiameterProperty);
            }
            set
            {
                SetValue(ProgressCircleDiameterProperty, value);
            }
        }

        public double CanvasSize
        {
            get
            {
                return (double)GetValue(CanvasSizeProperty);
            }
            set
            {
                SetValue(CanvasSizeProperty, value);
            }
        }

        public Brush ProgressCircleFillColor
        {
            get
            {
                return (Brush)GetValue(ProgressCircleFillColorProperty);
            }
            set
            {
                SetValue(ProgressCircleFillColorProperty, value);
            }
        }

        #endregion

        #region Private Methods

        private void Start()
        {
            _animationTimer.Tick += HandleAnimationTick;
            _animationTimer.Start();
        }

        private void Stop()
        {
            _animationTimer.Stop();
            _animationTimer.Tick -= HandleAnimationTick;
        }

        private void SetPosition(Ellipse ellipse, double offset,
            int position, double positionOffset, double step)
        {
            ellipse.SetValue(Canvas.LeftProperty, positionOffset
                + Math.Sin(offset + position * step) * positionOffset);

            ellipse.SetValue(Canvas.TopProperty, positionOffset
                + Math.Cos(offset + position * step) * positionOffset);
        }

        #endregion

        #region EventHandlers

        private void HandleAnimationTick(object sender, EventArgs e)
        {
            SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
        }

        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 10.0;
            var positionOffset = CanvasSize / 2.0 - ProgressCircleDiameter / 2.0;

            SetPosition(C0, offset, 0, positionOffset, step);
            SetPosition(C1, offset, 1, positionOffset, step);
            SetPosition(C2, offset, 2, positionOffset, step);
            SetPosition(C3, offset, 3, positionOffset, step);
            SetPosition(C4, offset, 4, positionOffset, step);
            SetPosition(C5, offset, 5, positionOffset, step);
            SetPosition(C6, offset, 6, positionOffset, step);
            SetPosition(C7, offset, 7, positionOffset, step);
            SetPosition(C8, offset, 8, positionOffset, step);
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void HandleVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var isVisible = (bool)e.NewValue;

            if (isVisible)
            {
                Start();
            }
            else
            {
                Stop();
            }
        }

        #endregion
    }
}
