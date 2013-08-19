using IpdsMultiTouch.Gestures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace IpdsMultiTouch
{
    public class TouchManipulatorBehavior : Behavior<FrameworkElement>
    {

        public TouchManipulatorBehavior()
        {
            SetValue(GesturesProperty, new ObservableCollection<aGesture>());
        }

        private static readonly DependencyProperty GesturesProperty =
            DependencyProperty.Register(
                "Gestures", typeof(ObservableCollection<aGesture>), typeof(TouchManipulatorBehavior), 
                new FrameworkPropertyMetadata(new ObservableCollection<aGesture>()));

        public ObservableCollection<aGesture> Gestures
        {
            get { return (ObservableCollection<aGesture>)GetValue(GesturesProperty); }
            set { SetValue(GesturesProperty, value); }
        }

        public TouchManipulator TouchManipulator { get; set; }


        public static readonly DependencyProperty ContainerControlProperty =
            DependencyProperty.Register("ContainerControl", typeof(FrameworkElement),
            typeof(TouchManipulatorBehavior),
            new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(ContainerControlPropertyChanged)
                ));

        public FrameworkElement ContainerControl
        {
            get
            {
                return (FrameworkElement)GetValue(ContainerControlProperty);
            }
            set
            {
                SetValue(ContainerControlProperty, value);

            }
        }
        public static readonly DependencyProperty IsManipulationEnabledProperty =
            DependencyProperty.Register("IsManipulationEnabled", typeof(bool), 
            typeof(TouchManipulatorBehavior),
            new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(IsManipulationEnabledPropertyChanged)
                ));
        private static void IsManipulationEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TouchManipulatorBehavior behv = d as TouchManipulatorBehavior;
            if (behv.AssociatedObject != null)
                behv.AssociatedObject.IsManipulationEnabled = (bool)e.NewValue;
        }

        public bool IsManipulationEnabled
        {
            get
            {
                return (bool)GetValue(IsManipulationEnabledProperty);
            }
            set
            {
                SetValue(IsManipulationEnabledProperty, value);
                
            }
        }
        private static void ContainerControlPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TouchManipulatorBehavior behv = d as TouchManipulatorBehavior;
            if (behv != null && behv.TouchManipulator != null)
            {
                behv.TouchManipulator.ContainerControl = (FrameworkElement)e.NewValue;
            }

        }
  
        protected override void OnAttached()
        {
            base.OnAttached();
            
            this.AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
            
        }

        void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            //create touchmanipulator
            TouchManipulator = new TouchManipulator(AssociatedObject, VisualTreeHelper.GetParent(AssociatedObject) as FrameworkElement);
            
            //build gestures
            foreach (aGesture t in (ObservableCollection<aGesture>)GetValue(GesturesProperty))
            {
                t.Manipulator = TouchManipulator;
                TouchManipulator.Gestures.Add(t);
            }

            //set container
            TouchManipulator.ContainerControl = GetValue(ContainerControlProperty) as FrameworkElement;
            AssociatedObject.IsManipulationEnabled = (bool)GetValue(IsManipulationEnabledProperty);

            //bind top request to set z-index
            TouchManipulator.WantToTop += TouchManipulator_WantToTop;
        }

        void TouchManipulator_WantToTop(object sender, EventArgs e)
        {
            OnWantToTop();
        }

        private void OnWantToTop()
        {
            if (this.WantToTop != null)
                this.WantToTop(this.TouchManipulator, new EventArgs());
        }

        public event EventHandler WantToTop;
    }
}
