using IpdsMultiTouch.Animation;
using IpdsMultiTouch.Gestures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IpdsMultiTouch
{
    #region Collision event stuff
    public delegate void CollisionHandler(CollisionEventArgs args);
    public class CollisionEventArgs : EventArgs
    {
        public enum Sites
        {
            Top, Bottom, Left, Right
        }
        public enum Direction
        {
            Inside, Outside
        }

        public Sites CollisionOnSite { get; set; }
        public Direction CollisionDirection { get; set; }

        public UIElement Element1 { get; set; }
        public UIElement Element2 { get; set; }
    }
    #endregion Collision event stuff

    public class TouchManipulator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element"></param>
        /// <param name="manipulationContainer"></param>
        public TouchManipulator(FrameworkElement element, FrameworkElement manipulationContainer)
        {
            this.Element = element;
            this.ManipulationContainer = manipulationContainer;
            this.ContainerControl = manipulationContainer;

            ManipulatorContainer.Instance.AddManipulator(this);
        }

        private GestureContainer gestures;
        /// <summary>
        /// List of gestures
        /// </summary>
        public GestureContainer Gestures
        {
            get
            {
                if (gestures == null)
                    gestures = new GestureContainer();
                return gestures;
            }
        }

        public event EventHandler WantToTop;
        private void OnWantToTop()
        {
            if (this.WantToTop != null)
            {
                this.WantToTop(this, null);
            }
            else
            {
                //set to top
                Utility.SetTop(this);
            }
        }

        private FrameworkElement _manipulationContainer;
        public FrameworkElement ManipulationContainer
        {
            get {
                return _manipulationContainer; }
            set { _manipulationContainer = value; }
        }


        private FrameworkElement element;
        public FrameworkElement Element
        {
            get
            {
                return element;
            }
            set
            {
                if (element != null)
                {
                    element.ManipulationStarting -= element_ManipulationStarting;
                    element.ManipulationInertiaStarting -= element_ManipulationInertiaStarting;
                    element.ManipulationDelta -= element_ManipulationDelta;
                    element.ManipulationCompleted -= element_ManipulationCompleted;
                }

                element = value;
                if (element != null)
                {
                    element.ManipulationStarting += element_ManipulationStarting;
                    element.ManipulationInertiaStarting += element_ManipulationInertiaStarting;
                    element.ManipulationDelta += element_ManipulationDelta;
                    element.ManipulationCompleted += element_ManipulationCompleted;

                    OriginalElementTransform = this.ElementTransform;
                }
            }
        }

        private Transform OriginalElementTransform;
        internal virtual Transform ElementTransform
        {
            get
            {
                return element.RenderTransform;
            }
            set
            {
                element.RenderTransform = value;
            }
        }

        /// <summary>
        /// The container control (area) where the Control moves
        /// </summary>
        public FrameworkElement ContainerControl { get; set; }
        
        /// <summary>
        /// Add gestures. Gesture manipulator will be setted
        /// </summary>
        /// <param name="gesture">manipulation gesture</param>
        /// <param name="replace">replace gesture if container already has gesture with this gesture (parameter) type</param>
        public void AddGesture(aGesture gesture, bool replace = false)
        {
            gesture.Manipulator = this;
            aGesture g;
            if ((g = Gestures[gesture.GetType()]) == null)
            {
                this.Gestures.Add(gesture);
            }
            else if (replace)
            {
                int i = Gestures.IndexOf(g);
                Gestures.Remove(g);
                Gestures.Insert(i, gesture);
            }
        }

        /// <summary>
        /// Reset element transform. Set actual transform to Origin transform
        /// </summary>
        public void ResetTransform()
        {
            MatrixTransform mt = Utility.GetMatrixTransformFromTransform(this.ElementTransform);
            
            Matrix oldMatrix = mt.Matrix;
            Matrix newMatrix = Utility.GetMatrixTransformFromTransform(OriginalElementTransform).Matrix;
            
            MatrixAnimation matrixAnimation = new MatrixAnimation(oldMatrix, newMatrix, new Duration(new TimeSpan(0, 0, 1)))
            {
                EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 2 }
            };

            MatrixTransform matrixTransform = new MatrixTransform(oldMatrix);
            this.ElementTransform = (MatrixTransform)matrixTransform;

            matrixTransform.BeginAnimation(MatrixTransform.MatrixProperty, matrixAnimation);
        }

        void element_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
        }

        /// <summary>
        /// Tell the gestures to make the manipulation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void element_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs args)
        {
            if (this.Element == null)
            {
                args.Cancel();
                return;
            }
            ApplyTransformForcedByGesture = null;
            Transform transform = Utility.GetMatrixTransformFromTransform(this.ElementTransform);
            
            //apply gestures
            if (transform is MatrixTransform)
            {
                Matrix matrix = (transform as MatrixTransform).Matrix;

                foreach (aGesture gest in Gestures)
                {
                    if (gest.IsEnabled)
                    {
                        if (args.Handled)
                            break;

                        gest.Manipulate(args, ref matrix);

                        // refresh matrix if gesture forced it. exept if it is the the last gesture of the list.
                        if (ApplyTransformForcedByGesture == gest && Gestures.Last() != gest)
                        {
                            transform = Utility.GetMatrixTransformFromTransform(this.ElementTransform);
                            matrix = (transform as MatrixTransform).Matrix;

                            ApplyTransformForcedByGesture = null;
                        }
                    }
                }

                // do not apply it twice
                if (ApplyTransformForcedByGesture == null)
                    ApplyTransform(matrix);
            }

            args.Handled = true;
        }

        /// <summary>
        /// gesture that called the applyTransform
        /// </summary>
        private aGesture ApplyTransformForcedByGesture;

        /// <summary>
        /// Apply transformation on Element
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="gesture"></param>
        public void ApplyTransform(Matrix matrix, aGesture gesture = null)
        {
            this.ElementTransform = (Transform)new MatrixTransform(matrix).GetAsFrozen();
            ApplyTransformForcedByGesture = gesture;
        }

        void element_ManipulationInertiaStarting(object sender, System.Windows.Input.ManipulationInertiaStartingEventArgs e)
        {
            if (this.Element == null)
            {
                e.Cancel();
                return;
            }

            //start inertia on all gesture
            Gestures.ForEach(a => a.ManipulationInertiaStarting(e));

            //Debug.WriteLine("OnManipulationInertiaStarting");
            double ms = 1000.0;
            

            //base.OnManipulationInertiaStarting(e);
            // Decrease the velocity of the Rectangle's movement by 
            // 10 inches per second every second.
            // (10 inches * 96 pixels per inch / 1000ms^2)
            //e.TranslationBehavior.DesiredDeceleration = 10 * 96.0 / (1000.0 * 1000.0);
            e.TranslationBehavior = new InertiaTranslationBehavior()
            {
                InitialVelocity = e.InitialVelocities.LinearVelocity,
                DesiredDeceleration = 10.0 * 96.0 / (ms * ms)
            };

            // Decrease the velocity of the Rectangle's resizing by 
            // 0.1 inches per second every second.
            // (0.1 inches * 96 pixels per inch / (1000ms^2)
            //e.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / 1000.0 * 1000.0;
            e.ExpansionBehavior = new InertiaExpansionBehavior()
            {
                InitialVelocity = e.InitialVelocities.ExpansionVelocity,
                DesiredDeceleration = 0.1 * 96 / ms * ms
            };

            // Decrease the velocity of the Rectangle's rotation rate by 
            // 2 rotations per second every second.
            // (2 * 360 degrees / (1000ms^2)
            //e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);
            e.RotationBehavior = new InertiaRotationBehavior()
            {
                InitialVelocity = e.InitialVelocities.AngularVelocity,
                DesiredDeceleration = 720 / (ms * ms)
            };


            e.Handled = true;
        }

        void element_ManipulationStarting(object sender, System.Windows.Input.ManipulationStartingEventArgs args)
        {
            if (this.Element == null)
            {
                args.Cancel();
                return;
            }
            //apply gestures
            Gestures.ForEach(a => a.ManipulationStarting(args));

            //get element to top
            this.OnWantToTop();

            //Debug.WriteLine("OnManipulationStarting");
            args.ManipulationContainer = this.ManipulationContainer;
            
            args.Handled = true;
            
        }

        /// <summary>
        /// move Element to parameter position with default animation
        /// If you want to use custom animation or no animation use SetPosition() method
        /// </summary>
        /// <param name="toElement"></param>
        public void MoveTowards(FrameworkElement toElement)
        {
            SetPosition(toElement, true);
        }
        
        /// <summary>
        /// Set element position with or whitout animation
        /// </summary>
        /// <param name="toElement">target element to move</param>
        /// <param name="DoAnimation">do or do not animation</param>
        /// <param name="animation">To and From value will be setted. If null a default animation will be used</param>
        public void SetPosition(FrameworkElement toElement, bool DoAnimation = false, MatrixAnimation animation = null)
        {
            this.Element.UpdateLayout();

            GeneralTransform trans_MovableInFrom_Offset_On_To = toElement.TransformToVisual(this.Element);
            
            Matrix reverseMatrix = (trans_MovableInFrom_Offset_On_To as MatrixTransform).Matrix;
            
            Transform transform = this.ElementTransform;

            MatrixTransform mt = Utility.GetMatrixTransformFromTransform(transform);
            Matrix newMatrix = mt.Matrix;
         
            newMatrix.Prepend(reverseMatrix);
         
            Matrix oldMatrix = mt.Matrix;
            MatrixTransform matrixTransform = new MatrixTransform(oldMatrix);

            if (DoAnimation)
            {
                if (animation != null)
                {
                    animation.From = oldMatrix;
                    animation.To = newMatrix;
                }
                else
                {
                    animation = new MatrixAnimation(oldMatrix, newMatrix, new Duration(new TimeSpan(0, 0, 1)))
                    {
                        EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 2 }
                    };
                }
                
                this.ElementTransform = (MatrixTransform)matrixTransform;

                if (!DoAnimation)
                {
                    animation.Duration = new Duration(new TimeSpan(0));
                }

                matrixTransform.BeginAnimation(MatrixTransform.MatrixProperty, animation);
            }
            else
            {
                this.ElementTransform = new MatrixTransform(newMatrix);
                
            }
        }


    }
}
