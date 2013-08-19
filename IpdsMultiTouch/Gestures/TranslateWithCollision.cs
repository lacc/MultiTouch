using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IpdsMultiTouch.Gestures
{
 
    public class TranslateWithCollision: TranslateWithBounce
    {
        public TranslateWithCollision()
        {
        }
        public TranslateWithCollision(TouchManipulator manipulator)
            : base(manipulator)
        { }


        /// <summary>
        /// in this group all Control will collide with each other
        /// </summary>
        public int CollisionGroup { get; set; }

        public override void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref System.Windows.Media.Matrix matrix)
        {
            //base.Manipulate(args, ref matrix);

            var manipulators = from tm in ManipulatorContainer.Instance.Manipulators
                               where
                                    tm != this.Manipulator &&
                                    tm.ContainerControl == this.Manipulator.ContainerControl &&
                                    (from gest in tm.Gestures
                                     where 
                                            gest.IsEnabled &&
                                            gest is TranslateWithCollision && 
                                            
                                            ((TranslateWithCollision)gest).CollisionGroup.Equals(this.CollisionGroup)
                                     select gest).Count() > 0 
                               select tm;

            foreach (TouchManipulator tm in manipulators)
            {
                Vector trans;
                bool contain = true;
                contain = Collide(this.Manipulator.Element, tm.Element, args.DeltaManipulation.Translation, out trans);

                if (contain)
                {
                    //manipulator2.Element.
                    
                    //Manipulation.SetManipulationParameter(tm.Element
                    //Manipulation.StartInertia(tm.Element);
                    //System.Windows.Input.ManipulationDeltaEventArgs a = new ManipulationDeltaEventArgs

                    //MatrixAnimation animation = new MatrixAnimation(oldMatrix, newMatrix, new Duration(new TimeSpan(0, 0, 1)))
                    //{
                    //    EasingFunction = new PowerEase { EasingMode = EasingMode.EaseInOut, Power = 2 }
                    //};

                   // Manipulation.
                }


              //  if (contain)
                    matrix.Translate(trans.X, trans.Y);
            }
        }

      
        public bool Collide(FrameworkElement obj1, FrameworkElement obj2, Vector Translation, out Vector elementOffset)
        {
            
            double X = Translation.X;
            double Y = Translation.Y;
           
            elementOffset = new Vector(X, Y);

            Rect obj1Bounds = Utility.GetBounds(obj1, obj1.Parent as FrameworkElement);
            Rect obj2Bounds = Utility.GetBounds(obj2, obj1.Parent as FrameworkElement);
            //obj1Bounds.Intersect(obj2Bounds);

            bool contain = obj1Bounds.IntersectsWith(obj2Bounds);
            Rect obj1Bounds_clone = new Rect(obj1Bounds.X, obj1Bounds.Y, obj1Bounds.Width, obj1Bounds.Height);
            obj1Bounds_clone.Intersect(obj2Bounds);
            //if (contain)
            {
               

                //Debug.WriteLine("");
                //Debug.WriteLine("==============================");
                //Debug.WriteLine("obj2 Bound: " + obj2Bounds.ToString());
                ////Debug.WriteLine("containing Bound: " + containingRect.ToString());


                //bool _ReverseX;
                //bool _ReverseY;
                //bool _waitX;
                //bool _waitY;

                //top
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopLeft, new Point(obj1Bounds.TopRight.X, obj1Bounds.TopRight.Y - 1))))
                {
                }
                //bottom
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.BottomLeft, new Point(obj1Bounds.BottomRight.X, obj1Bounds.BottomRight.Y - 1))))
                {
                }
                //left
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopLeft, new Point(obj1Bounds.BottomLeft.X + 1, obj1Bounds.BottomLeft.Y))))
                {
                }
                //right
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopRight, new Point(obj1Bounds.BottomRight.X - 1, obj1Bounds.BottomRight.Y))))
                {
                }

                #region
                CollisionEventArgs args = new CollisionEventArgs();
                args.Element1 = obj1;
                args.Element2 = obj2;
                args.CollisionDirection = CollisionEventArgs.Direction.Outside;

                //1-es jobbrol  utkozik 2-esnek
                //if (obj1Bounds.Left < obj2Bounds.Left &&
                //    obj1Bounds.Left > obj2Bounds.Right)
                //right
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopRight, new Point(obj1Bounds.BottomRight.X - 1, obj1Bounds.BottomRight.Y))))
                {
                    if (!ReverseX && !waitLeft)
                    {
                        ReverseX = true;
                        waitLeft = true;

                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Left;

                        Debug.WriteLine("Left: if !ReverseX && !waitX");
                    }
                    else if (ReverseX && !waitLeft)
                    {
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Left;
                        ReverseX = false;
                        waitLeft = true;
                        Debug.WriteLine("Left: else if (ReverseX && !waitX)");
                    }
                    // else if (ReverseX && waitX)


                }
                else
                {
                    //if (waitLeft)
                    //    Manipulation.StartInertia(this.Manipulator.Element);
                    waitLeft = false;
                    
                }
                //1-es balrol  utkozik 2-esnek
                //if (obj1Bounds.Left > obj2Bounds.Left &&
                //    obj1Bounds.Left < obj2Bounds.Right)
                //left
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopLeft, new Point(obj1Bounds.BottomLeft.X + 1, obj1Bounds.BottomLeft.Y))))
                {

                    if (!ReverseX && !waitRight)
                    {
                        ReverseX = true;
                        waitRight = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Right;
                        Debug.WriteLine("Right: if (!ReverseX && !waitX)");
                    }
                    else if (ReverseX && !waitRight)
                    {
                        ReverseX = false;
                        waitRight = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Right;
                        Debug.WriteLine("Right: else if (ReverseX && !waitX)");
                    }
                    //ReverseX = !ReverseX;

                }
                else
                {
                    //if (waitRight)
                    //    Manipulation.StartInertia(this.Manipulator.Element);
                    waitRight = false;
                }


                //if (obj1Bounds.Top > obj2Bounds.Top &&
                //    obj1Bounds.Top < obj2Bounds.Bottom)
                //top
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.TopLeft, new Point(obj1Bounds.TopRight.X, obj1Bounds.TopRight.Y - 1))))
                {
                    if (!ReverseY && !waitTop)
                    {
                        ReverseY = true;
                        waitTop = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Top;
                        Debug.WriteLine("Top: if !ReverseY && !waitY");
                    }
                    else if (ReverseY && !waitTop)
                    {
                        ReverseY = false;
                        waitTop = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Top;
                        Debug.WriteLine("Top: else if (ReverseY && !waitY)");
                    }
                    //ReverseY = !ReverseY;
                }
                else
                {
                    //if (waitTop)
                    //    Manipulation.StartInertia(this.Manipulator.Element);
                    waitTop = false;
                }

                //if (obj1Bounds.Top < obj2Bounds.Top && 
                //    obj1Bounds.Bottom > obj2Bounds.Top)
                    //bottom
                if (obj1Bounds_clone.IntersectsWith(new Rect(obj1Bounds.BottomLeft, new Point(obj1Bounds.BottomRight.X, obj1Bounds.BottomRight.Y - 1))))
                {
                    if (!ReverseY && !waitBottom)
                    {
                        ReverseY = true;
                        waitBottom = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Bottom;
                        Debug.WriteLine("Bottom: if (!ReverseY && !waitY)");
                    }
                    else if (ReverseY && !waitBottom)
                    {
                        ReverseY = false;
                        waitBottom = true;
                        args.CollisionOnSite = args.CollisionOnSite & CollisionEventArgs.Sites.Bottom;
                        Debug.WriteLine("Bottom: else if (ReverseY && !waitY)");
                    }
                    //ReverseY = !ReverseY;
                }
                else
                {
                    //if (waitBottom)
                    //    Manipulation.StartInertia(this.Manipulator.Element);
                    waitBottom = false;
                }



                if (waitRight)
                {
                    //ReverseX = !ReverseX;
                    // waitX = false;
                    //Debug.WriteLine("contain 'waitX'");
                }
                if (waitTop)
                {
                    //ReverseY = !ReverseY;
                    // waitY = false;
                    //Debug.WriteLine("contain 'waitY'");
                }

                if (!contain)
                {
                    // OnCollise(args);
                }
                else
                {

                }
                #endregion

                //else //if (args.IsInertial)

                

                Debug.WriteLine("--- Before reverse ---");
                Debug.WriteLine("X: " + X);
                Debug.WriteLine("Y: " + Y);

                //Point p = transToOffsetOnFrom3.Transform(new Point(delta.Translation.X, delta.Translation.Y));
                // Transform tt = transToOffsetOnFrom3.

                if (ReverseY)
                {

                    Y = -Y;
                }
                if (ReverseX)
                {
                    X = -X;
                }
                Debug.WriteLine("--- After reverse ---");
                Debug.WriteLine("X: " + X);
                Debug.WriteLine("Y: " + Y);
            }
            elementOffset.X = X;
            elementOffset.Y = Y;
            //elementOffset.X = p.X;
            //elementOffset.Y = p.Y;

          

            return contain;
        }

        private void draw(Rect r, System.Windows.Controls.Canvas parent)
        {
            //System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
            //r.StrokeThickness = 1;
            //r.Stroke = Brushes.Red;
            //r.Width = obj1Bounds.Width;
            //r.Height = obj1Bounds.Height;
            //((System.Windows.Controls.Canvas)Element.Parent).Children.Add(r);
            //System.Windows.Controls.Canvas.SetLeft(r, obj1Bounds.Left);
            //System.Windows.Controls.Canvas.SetTop(r, obj1Bounds.Top);


            //System.Windows.Shapes.Rectangle r2 = new System.Windows.Shapes.Rectangle();
            //r2.StrokeThickness = 1;
            //r2.Stroke = Brushes.Blue;
            //r2.Width = obj2Bounds.Width;
            //r2.Height = obj2Bounds.Height;
            //((System.Windows.Controls.Canvas)Element.Parent).Children.Add(r2);
            //System.Windows.Controls.Canvas.SetLeft(r2, obj2Bounds.Left);
            //System.Windows.Controls.Canvas.SetTop(r2, obj2Bounds.Top);

            //System.Windows.Shapes.Rectangle r3 = new System.Windows.Shapes.Rectangle();
            ////r3.StrokeThickness = 1;
            ////r3.Stroke = Brushes.Red;
            //r3.Fill = Brushes.Green;
            //r3.Width = obj1Bounds_clone.Width;
            //r3.Height = obj1Bounds_clone.Height;
            //((System.Windows.Controls.Canvas)Element.Parent).Children.Add(r3);
            //System.Windows.Controls.Canvas.SetLeft(r3, obj1Bounds_clone.Left);
            //System.Windows.Controls.Canvas.SetTop(r3, obj1Bounds_clone.Top);
        }
    }
}
