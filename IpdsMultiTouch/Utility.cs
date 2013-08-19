using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using System.Windows.Media;

namespace IpdsMultiTouch
{
    public class Utility
    {
        /// <summary>
        /// Gets the bounds inside a container
        /// </summary>
        /// <param name="of"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        public static Rect GetBounds(FrameworkElement of, FrameworkElement from)
        {
            // Might throw an exception if of and from are not in the same visual tree
            GeneralTransform transform = of.TransformToVisual(from);
            return transform.TransformBounds(new Rect(new Size(of.ActualWidth, of.ActualHeight)));
        }

        /// <summary>
        /// Get angle f rotation in a matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static double GetRotateAngleFromMatrix(Matrix matrix)
        {
            var x = new Vector(1, 0);
            Vector rotated = Vector.Multiply(x, matrix);
            double angleBetween = Vector.AngleBetween(x, rotated);

            return angleBetween;
        }

        /// <summary>
        /// Get MatrixTransformation from any kint of Transformation
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static MatrixTransform GetMatrixTransformFromTransform(GeneralTransform transform)
        {
            if (transform is MatrixTransform)
                return transform as MatrixTransform;
            
            Matrix matrix = Matrix.Identity;
            TransformGroup tg = null;
            
            if (!(transform is TransformGroup))
            {
                tg = new TransformGroup();
                tg.Children.Add(transform as Transform);
                transform = tg;
            }
            
            tg = transform as TransformGroup;
            if (tg.Children.Count > 0)
            {
                matrix = tg.Children[0].Value;

                for (int i = 1; i < tg.Children.Count; i++)
                {
                    matrix *= tg.Children[i].Value;
                }
            }
            

            return new MatrixTransform(matrix);
        }

        /// <summary>
        /// Reset transformation in matrix to default state
        /// Important: a sorrend fontos, ha rotate false, es scale true, helytelen eredmeny lesz
        /// </summary>
        /// <param name="m"></param>
        /// <param name="rotate">reset rotate transform</param>
        /// <param name="scale">reset scale transform</param>
        /// <param name="offset">reset offset transform</param>
        /// <returns></returns>
        public static Matrix ResetMatrix(Matrix m,  bool rotate = true, bool scale = true, bool offset = true)
        {
            Matrix res = m;
            
            //Point center = newMatrix.Transform(new Point(containerFrom.movableControl.ActualWidth / 2, containerFrom.movableControl.ActualHeight / 2));
            if (rotate)
            {
                double d = Utility.GetRotateAngleFromMatrix(res);
                res.Rotate(-d);
            }
            if (scale)
            {
                double ScaleX = 1 / res.M11;
                double ScaleY = 1 / res.M22;
                res.Scale(ScaleX, ScaleY);
            }
            if (offset)
            {
                res.OffsetX -= res.OffsetX;
                res.OffsetY -= res.OffsetY;
            }

            return res;
        }

        /// <summary>
        /// set manipulator element the higest z-index
        /// </summary>
        /// <param name="manipulator"></param>
        public static void SetTop(TouchManipulator manipulator)
        {
            int z = 0;
            IpdsMultiTouch.ManipulatorContainer.Instance.Manipulators.ForEach(a => z = Math.Max(z, System.Windows.Controls.Panel.GetZIndex(a.Element)));

            System.Windows.Controls.Panel.SetZIndex(manipulator.Element, z + 1);
            System.Windows.Controls.Canvas.SetZIndex(manipulator.Element, z + 1);
        }

        /// <summary>
        /// Calculate element overshot in container
        /// </summary>
        /// <param name="element"></param>
        /// <param name="container"></param>
        /// <param name="overshoot"></param>
        /// <returns></returns>
        public static bool CalculateOvershoot(UIElement element, IInputElement container, out Vector overshoot)
        {
            // Get axis aligned element bounds
            Rect elementBounds = Utility.GetBounds(element as FrameworkElement, container as FrameworkElement);

            //double extraX = 0.0, extraY = 0.0;
            overshoot = new Vector();

            FrameworkElement parent = container as FrameworkElement;
            if (parent == null)
            {
                return false;
            }

            // Calculate overshoot.  
            if (elementBounds.Left < 0)
                overshoot.X = elementBounds.Left;
            else if (elementBounds.Right > parent.ActualWidth)
                overshoot.X = elementBounds.Right - parent.ActualWidth;

            if (elementBounds.Top < 0)
                overshoot.Y = elementBounds.Top;
            else if (elementBounds.Bottom > parent.ActualHeight)
                overshoot.Y = elementBounds.Bottom - parent.ActualHeight;

            // Return false if Overshoot is empty; otherwsie, return true.
            return !Vector.Equals(overshoot, new Vector());
        }
    }

}
