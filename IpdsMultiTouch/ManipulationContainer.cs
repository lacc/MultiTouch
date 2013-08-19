using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IpdsMultiTouch
{
    public class ManipulatorContainer
    {
        //making skeleton
        private static ManipulatorContainer instance;
        public static ManipulatorContainer Instance
        {
            get
            {
                if (instance == null)
                    instance = new ManipulatorContainer();

                return instance;
            }
        }

        public TouchManipulator this[int i]
        {
            get
            {
                return this.Manipulators[i];
            }
        }
        public TouchManipulator this[UIElement element]
        {
            get
            {
                if (Instance.Manipulators == null || Instance.Manipulators.Count == 0)
                    return null;

                TouchManipulator res = Instance.Manipulators.FirstOrDefault(a => a.Element == element);
                return res;
            }
        }
        
        public static TouchManipulator GetManipulatorByElement(UIElement element)
        {
            return Instance[element]; ;
        }

        public static void RemoveManipulatorByElement(UIElement element)
        {
            TouchManipulator tm = Instance[element];
            if (tm != null)
            {
                Instance.Manipulators.Remove(tm);
                tm.Element = null;
            }
        }
        
        private List<TouchManipulator> manipulators = new List<TouchManipulator>();
        public List<TouchManipulator> Manipulators
        {
            get { return manipulators; }
            set { manipulators = value; }
        }

        public void AddManipulator(TouchManipulator manipulator)
        {
            this.Manipulators.Add(manipulator);
        }
        
    }

    public static class TouchManipulatorContaierHelper
    {
        public static TouchManipulator GetTouchManipulator(this UIElement element)
        {
            return ManipulatorContainer.GetManipulatorByElement(element);
        }
    }
}
