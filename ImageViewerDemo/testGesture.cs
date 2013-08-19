using IpdsMultiTouch.Gestures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageViewer
{
    public class testGesture : aGesture
    {
        public testGesture()
            : base()
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
