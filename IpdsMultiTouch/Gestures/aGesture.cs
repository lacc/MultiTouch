using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IpdsMultiTouch.Gestures
{
    public abstract class aGesture
    {
        public aGesture()
        { }
        public aGesture(TouchManipulator manipulator)
            : base()
        {
            this.Manipulator = manipulator;
        }
        
        private TouchManipulator manipulator;
        [Browsable(false)]
        public TouchManipulator Manipulator
        {
            get { return manipulator; }
            set { manipulator = value; }
        }

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        private List<Type> omitGestures;
        //azon gesture-ok, melyeket ki kell hagyni ha ez a gesture benne van a vegrehajtandoak kozott
        /// <summary>
        /// Gestures that should omit if "this" gesture is in the processed gesture list in TouchManipulator
        /// </summary>
        protected List<Type> OmitGestures
        {
            get
            {
                if (omitGestures == null)
                    omitGestures = new List<Type>();
                return omitGestures;
            }
            set { omitGestures = value; }
        }

        public virtual void ManipulationInertiaStarting(System.Windows.Input.ManipulationInertiaStartingEventArgs e) { }
        public virtual void ManipulationStarting(System.Windows.Input.ManipulationStartingEventArgs args) { }
        public virtual void ManipulationCompleted(System.Windows.Input.ManipulationCompletedEventArgs e) { }

        /// <summary>
        /// override this to make manipulation on the Matrix (rotate, translate, etc).
        /// Call "base.Manipulate(delta)" in any case the top of the method when override. it makes some filter and validation
        /// </summary>
        /// <param name="delta"></param>
        public virtual void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref Matrix matrix)
        {
            if (!this.IsEnabled)
                return;

            if (Manipulator == null)
                throw new Exception("Please set the Manipulator of the Gesture!");

            //find guestures witch has this gesture in there omit list and skip this gesture if founded
            if (ManipulatorContainer.Instance.Manipulators.FirstOrDefault(a =>
                        a.Gestures.FirstOrDefault(b => b.IsEnabled && b.OmitGestures.Contains(this.GetType()) ) != null) 
                        != null)
            {
                return;
            }
        }

        /// <summary>
        /// Check if Control is inside its Container Control
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="elementOffset"></param>
        /// <returns></returns>
        public bool ElementInsideContainer()
        {
            Vector v;
            bool contain = this.ElementInsideContainer(out v);

            return contain;
        }

        /// <summary>
        /// Check if Control is inside its Container Control
        /// </summary>
        /// <param name="delta"></param>
        /// <param name="elementOffset"></param>
        /// <returns></returns>
        public bool ElementInsideContainer(out Vector overshot)
        {
            FrameworkElement container = this.Manipulator.ContainerControl as FrameworkElement;
            FrameworkElement element = this.Manipulator.Element;

            bool contain = !Utility.CalculateOvershoot(element, container, out overshot);

            return contain;
        }
    }
}
