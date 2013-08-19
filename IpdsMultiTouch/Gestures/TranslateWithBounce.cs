using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IpdsMultiTouch.Gestures
{
    public class TranslateWithBounce : aGesture
    {
        public TranslateWithBounce()
        {
            this.OmitGestures.Add(typeof(Translate));
        }
     
        public TranslateWithBounce(TouchManipulator manipulator)
            : base(manipulator)
        {
        }
        #region properties for bouncing

        

        /// <summary>
        /// Enable bouncing
        /// </summary>
        public bool DisableBounce { get; set; }

        /// <summary>
        /// Showing the X direction of the object when it is in inertia
        /// If it is true the direction will turn
        /// </summary>
        protected bool ReverseX { get; set; }
        /// <summary>
        /// Showing the Y direction of the object when it is in inertia
        /// If it is true the direction will turn
        /// </summary>
        protected bool ReverseY { get; set; }

        /// <summary>
        /// Waiting to the object to will be in the container completely during the inertia on X axis
        /// </summary>
        protected bool waitRight;
        protected bool waitLeft;
        /// <summary>
        /// Waiting to the object to will be in the container completely during the inertia on Y axis
        /// </summary>
        protected bool waitTop;
        protected bool waitBottom;
        #endregion for bouncing

        /// <summary>
        /// Resetting inertia, bouncing, etc.
        /// </summary>
        private void ResetInertia()
        {
            ReverseX = false;
            ReverseY = false;
            waitRight = false;
            waitLeft = false;
            waitTop = false;
            waitBottom = false;
        }

        public event CollisionHandler Collise;
        private void OnCollise(CollisionEventArgs args)
        {
            if (Collise != null)
                Collise(args);
        }

        public override void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref System.Windows.Media.Matrix matrix)
        {
            base.Manipulate(args, ref matrix);

            Vector trans;
            bool contain = true;
            if (DisableBounce)
            {
                matrix.Translate(args.DeltaManipulation.Translation.X, args.DeltaManipulation.Translation.Y);
            }
            else
            {
                    contain = Contains(this.Manipulator.ContainerControl as FrameworkElement, args.DeltaManipulation, out trans);
                    matrix.Translate(trans.X, trans.Y);
            }
        }

        public override void ManipulationInertiaStarting(ManipulationInertiaStartingEventArgs e)
        {
            base.ManipulationInertiaStarting(e);
            ResetInertia();
        }
        public override void ManipulationStarting(ManipulationStartingEventArgs args)
        {
            base.ManipulationStarting(args);
            ResetInertia();
        }
        public override void ManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.ManipulationCompleted(e);
            ResetInertia();
        }

        /// <summary>
        /// Get element offset in container by Translation of ManipulationDelta
        /// </summary>
        /// <param name="container">Where the element can move</param>
        /// <param name="element">the moving element</param>
        /// <param name="delta">ManipulationDelta</param>
        /// <param name="elementOffset">out parameter with the translation offset of element</param>
        /// <returns>return true if container isn't contain full of the element</returns>
        public bool Contains(FrameworkElement container, ManipulationDelta delta, out Vector elementOffset)
        {
            FrameworkElement Element = this.Manipulator.Element;

            elementOffset = new Vector(delta.Translation.X, delta.Translation.Y);

            Rect shapeBounds2 = Utility.GetBounds(Element, this.Manipulator.ContainerControl as FrameworkElement);
            Rect containerBounds = Utility.GetBounds(container, this.Manipulator.ContainerControl as FrameworkElement);
           
            bool contain = containerBounds.Contains(shapeBounds2);

            string d = "";

            CollisionEventArgs args = new CollisionEventArgs();
            args.Element1 = Element;
            args.Element2 = container;
            args.CollisionDirection = CollisionEventArgs.Direction.Inside;

            #region set reverse
            if (shapeBounds2.Left < containerBounds.Left)
            {
                d += "shapeBounds2.Left < containerBounds.Left";
                if (!ReverseX && !waitLeft)
                {
                    ReverseX = true;
                    waitLeft = true;

                    args.CollisionOnSite = CollisionEventArgs.Sites.Left;

                    //Debug.WriteLine("Left: if !ReverseX && !waitX");
                }
                else if (ReverseX && !waitLeft)
                {
                    args.CollisionOnSite = CollisionEventArgs.Sites.Left;
                    ReverseX = false;
                    waitLeft = true;
                    //Debug.WriteLine("Left: else if (ReverseX && !waitX)");
                }
            }
            else
                waitLeft = false;

            if (shapeBounds2.Right > containerBounds.Right)
            {
                d += " shapeBounds2.Right > containerBounds.Right";

                if (!ReverseX && !waitRight)
                {
                    ReverseX = true;
                    waitRight = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Right;
                    // Debug.WriteLine("Right: if (!ReverseX && !waitX)");
                }
                else if (ReverseX && !waitRight)
                {
                    ReverseX = false;
                    waitRight = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Right;
                    // Debug.WriteLine("Right: else if (ReverseX && !waitX)");
                }
            }
            else
                waitRight = false;

            if (shapeBounds2.Top < containerBounds.Top)
            {
                d += " shapeBounds2.Top < containerBounds.Top";
                if (!ReverseY && !waitTop)
                {
                    ReverseY = true;
                    waitTop = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Top;
                    // Debug.WriteLine("Top: if !ReverseY && !waitY");
                }
                else if (ReverseY && !waitTop)
                {
                    ReverseY = false;
                    waitTop = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Top;
                    //  Debug.WriteLine("Top: else if (ReverseY && !waitY)");
                }
            }
            else
                waitTop = false;

            if (shapeBounds2.Bottom > containerBounds.Bottom)
            {
                d += " shapeBounds2.Bottom > containerBounds.Bottom";

                if (!ReverseY && !waitBottom)
                {
                    ReverseY = true;
                    waitBottom = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Bottom;
                    // Debug.WriteLine("Bottom: if (!ReverseY && !waitY)");
                }
                else if (ReverseY && !waitBottom)
                {
                    ReverseY = false;
                    waitBottom = true;
                    args.CollisionOnSite = CollisionEventArgs.Sites.Bottom;
                    //  Debug.WriteLine("Bottom: else if (ReverseY && !waitY)");
                }
            }
            else
                waitBottom = false;
            #endregion set reverse

            if (!string.IsNullOrEmpty(d))
            {
                //Debug.WriteLine("sb T:" + shapeBounds2.Top + " | L: " + shapeBounds2.Left + " | W: " + shapeBounds2.Width + " | H: " + shapeBounds2.Height);
                //Debug.WriteLine("cb T:" + containerBounds.Top + " | L: " + containerBounds.Left + " | W: " + containerBounds.Width + " | H: " + containerBounds.Height);
                //Debug.WriteLine(d);
            }

            if (!contain)
            {
                OnCollise(args);
            }

            Vector trans = new Vector( delta.Translation.X, delta.Translation.Y);

            if (ReverseY)
            {
                trans.Y = -trans.Y;
            }
            if (ReverseX)
            {
                trans.X = -trans.X;
            }

            //if (ReverseX || ReverseY)
            //{
            //    Debug.WriteLine("--- After reverse ---");
            //    Debug.WriteLine("X: " + X);
            //    Debug.WriteLine("Y: " + Y);
            //}

            elementOffset = trans;

            return contain;
        }

     }
}


