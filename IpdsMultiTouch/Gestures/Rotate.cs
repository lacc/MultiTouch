using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace IpdsMultiTouch.Gestures
{
    public class Rotate : aGesture
    {
        public Rotate()
        { }
        public Rotate(TouchManipulator manipulator)
            : base(manipulator)
        { }

        public override void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref System.Windows.Media.Matrix matrix)
        {
            base.Manipulate(args, ref matrix);
            ManipulationDelta delta = args.DeltaManipulation;
            Point center = args.ManipulationOrigin;
            Debug.WriteLine("==============================");
            Debug.WriteLine("0: " + center);
            
           System.Windows.Media.Matrix m = new System.Windows.Media.Matrix(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX, matrix.OffsetY);
          
            matrix.RotatePrepend(delta.Rotation);

            if (!this.ElementInsideContainer())
            {
                matrix = m;
            }

        }
    }
}
