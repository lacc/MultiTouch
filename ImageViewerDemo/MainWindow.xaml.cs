using IpdsMultiTouch;
using IpdsMultiTouch.Gestures;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //this.WindowState = WindowState.Normal;
            //this.WindowStyle = WindowStyle.None;
            // this.WindowState = WindowState.Maximized;
            //this.Topmost = true;
            this.Top = 0;
            this.Left = 0;

            rect = new Rect(0, 0, System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
            LoadMedias();
            WindowStyle = WindowStyle.None;
            WindowState = WindowState.Maximized;

        }

        Random rand = new Random();
        Rect rect;

        private List<object> _touchableElement;
        public List<object> TouchableElements //= new List<FrameworkElement>();
        {
            get
            {
                if (_touchableElement == null)
                    _touchableElement = new List<object>();
                return _touchableElement;
            }
            set
            {
                _touchableElement = value;
            }
        }
        void BindImages()
        {
           // images.ItemsSource = TouchableElements;

            foreach (var e in TouchableElements)
            {
                TouchManipulator tm = new TouchManipulator(e as FrameworkElement, myCanvas);
                tm.AddGesture(new TranslateWithBounce());
                tm.AddGesture(new Rotate());
                tm.AddGesture(new Scale());

                tm.WantToTop += a_WantToTop;

                (e as FrameworkElement).IsManipulationEnabled = true;
                (e as FrameworkElement).RenderTransformOrigin = new Point(0.5, 0.5);

                // TODO check if it is neccessarry
                RenderOptions.SetCachingHint(e as FrameworkElement, CachingHint.Cache);
                RenderOptions.SetBitmapScalingMode(e as FrameworkElement, BitmapScalingMode.LowQuality);

                myCanvas.Children.Add(e as FrameworkElement);
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            //IpdsMultiTouch.ManipulatorContainer.Instance.Manipulators.ForEach(a => a.WantToTop += a_WantToTop);
            // IPDS.WPF.Touch.TouchManipulator.Utility.SetTop(m);
        }


        protected override void OnManipulationStarting(ManipulationStartingEventArgs e)
        {
            base.OnManipulationStarting(e);
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);
        }
        protected override void OnManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.OnManipulationInertiaStarting(e);
        }

        #region MediaLoad
        private void LoadMedias()
        {

            //string path = System.Configuration.ConfigurationManager.AppSettings.Get("imagesFolder");

            //string path = System.Environment.CurrentDirectory + @"\Images";
            string path = AppDomain.CurrentDomain.BaseDirectory +  Properties.Settings.Default.imageFolder;
            try
            {
                string[] files = System.IO.Directory.GetFiles(path);

                foreach (string file in files)
                {
                    LoadFile(file);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("IO wrong:" + e.ToString());
            }

            BindImages();
        }

        private void LoadFile(string file)
        {
            string fileExt = System.IO.Path.GetExtension(file).ToLower();

            FrameworkElement elem = null;

            if (fileExt == ".jpg" || fileExt == ".png" || fileExt == ".gif" || fileExt == ".bmp" || fileExt == ".tif" || fileExt == ".ico" || fileExt == ".jpeg" || fileExt == ".tiff")
            {
                //TouchImage image = new TouchImage(this);
                Image image = new Image();

                image.Source = new BitmapImage(new Uri(file));

                //image.Width = 400;
                //image.Height = 400;
                image.RenderTransform = new ScaleTransform(0.5, 0.5);
                image.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                
                elem = image;
                TouchableElements.Add(image);
            }
            else if (fileExt == ".avi" || fileExt == ".wmv" || fileExt == ".mpg" || fileExt == ".mpeg" || fileExt == ".mp4")
            {
                MediaElement me = new MediaElement();
                me.Source = new Uri(file);
                me.LoadedBehavior = MediaState.Manual;
                me.Play();
                me.MediaEnded += new RoutedEventHandler(me_MediaEnded);

                TouchableElements.Add(me);

            }
            else if (fileExt == ".xps")
            {
                //DocumentViewer dv = (DocumentViewer)this.Resources["myDocViewer"];
                //dv.Document = new XpsDocument(file, FileAccess.Read).GetFixedDocumentSequence();

                //dv.FitToMaxPagesAcross(1);

                //TouchableElements.Add(dv);

                //elem = dv;

                //DragScaleRotate mdsr = new DragScaleRotate(true, true, true, true, rect);
                //IpdsMultiTouch.EnableGesture(dv, mdsr, null);
            }
            
        }
        
        void a_WantToTop(object sender, EventArgs e)
        {
            int z = 0;
            IpdsMultiTouch.ManipulatorContainer.Instance.Manipulators.ForEach(a => z = Math.Max(z, Panel.GetZIndex(a.Element as UIElement)));
            FrameworkElement elem = ((sender as TouchManipulator).Element as FrameworkElement);
            Panel.SetZIndex(elem, z + 1);
            Canvas.SetZIndex(elem, z + 1);
            
            elem = ((sender as TouchManipulator).Element.Parent as FrameworkElement);
            Panel.SetZIndex(elem, z + 1);
            Canvas.SetZIndex(elem, z + 1);

        }

        private void me_MediaEnded(object sender, RoutedEventArgs e)
        {
            MediaElement me = sender as MediaElement;

            me.Position = TimeSpan.FromSeconds(0);
            me.Play();
        }
        #endregion MediaLoad
        void manipulator_Collise(CollisionEventArgs args)
        {
            Ellipse e = new Ellipse();
            Rect r = args.Element1.RenderTransform.TransformBounds(new Rect());

            e.Width = 5; e.Height = 5;

            e.Fill = new SolidColorBrush(Colors.Red);
            this.myCanvas.Children.Add(e);

            Canvas.SetTop(e, r.Y);
            Canvas.SetLeft(e, r.X);

        }
        protected override void OnTouchDown(TouchEventArgs e)
        {
            //if (e.Source.GetType() == typeof(TouchImage))
            //    return;
            //if (images.Count == 0)
            //    return;

            if (ManipulatorContainer.Instance.Manipulators.Count == 0)
                return;

            //foreach (TouchImage image in images)
            {
                //TouchImage image = (TouchImage)images[rand.Next(images.Count - 1)];
                TouchPoint tp = e.GetTouchPoint(this);
                TouchManipulator m = ManipulatorContainer.Instance.Manipulators[rand.Next(ManipulatorContainer.Instance.Manipulators.Count - 1)];
                //m.MoveTowards(tp.Position);
            }
        }

        private void tm_WantToTop(object sender, EventArgs e)
        {

            //((TouchManipulator)sender).Element
        }

    }
    
}
