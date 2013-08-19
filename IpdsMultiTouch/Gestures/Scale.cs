using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IpdsMultiTouch.Gestures
{
    public class Scale : aGesture
    {
        public Scale()
        { }
        public Scale(TouchManipulator manipulator)
            : base(manipulator)
        { }

        public double MaxScale { get; set; }
        private Matrix lastPositionInside;
        public override void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref System.Windows.Media.Matrix matrix)
        {
            base.Manipulate(args, ref matrix);

            ManipulationDelta delta = args.DeltaManipulation;
            Point center = args.ManipulationOrigin;


            Vector scale = new Vector(matrix.M11, matrix.M22);
            if (MaxScale == 0.0 || MaxScale > scale.Length)
            {
                //matrix.ScaleAtPrepend(delta.Scale.X, delta.Scale.Y, center.X, center.Y);
                System.Windows.Media.Matrix m = new System.Windows.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                //m.ScalePrepend(delta.Scale.X, delta.Scale.Y);

                matrix.ScalePrepend(delta.Scale.X, delta.Scale.Y);

                if (!this.ElementInsideContainer() && (delta.Scale.X > 1 || delta.Scale.Y > 1))
                {
                    if (lastPositionInside != null)
                    {
                        matrix = m;
                        //matrix = new System.Windows.Media.Matrix(lastPositionInside.M11, lastPositionInside.M12, lastPositionInside.M21, lastPositionInside.M22, lastPositionInside.OffsetX, lastPositionInside.OffsetY);
                    }
                    args.Handled = true;
                }
                else
                {
                    lastPositionInside = new System.Windows.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
                }
            }
        }
    }
}
