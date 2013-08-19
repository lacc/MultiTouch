using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IpdsMultiTouch.Gestures
{
    public class GestureContainer : List<aGesture>
    {
        public aGesture this[Type type]
        {
            get
            {
                return this.FirstOrDefault(a => a.GetType() == type);
            }
        }

        //public T GetGestureByType<T>()
        //{
        //    return this.FirstOrDefault(a => a is T);
        //}
    }
}
