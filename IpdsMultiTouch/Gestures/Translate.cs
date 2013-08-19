using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace IpdsMultiTouch.Gestures
{
    public class Translate: aGesture
    {
        public Translate()
        {
        }
        public Translate(TouchManipulator manipulator)
            : base(manipulator)
        { }
        public override void Manipulate(System.Windows.Input.ManipulationDeltaEventArgs args, ref Matrix matrix)
        {
            base.Manipulate(args, ref matrix);
            Vector trans;
            trans = new Vector(args.DeltaManipulation.Translation.X, args.DeltaManipulation.Translation.Y);

            matrix.Translate(trans.X, trans.Y);
        }
    }
}
