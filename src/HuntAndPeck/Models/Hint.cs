using System;
using System.Windows;

namespace HuntAndPeck.Models
{

    public abstract class Hint
    {

        protected Hint(IntPtr owningWindow, Rect boundingRectangle)
        {
            OwningWindow = owningWindow;
            BoundingRectangle = boundingRectangle;
        }

        public Rect BoundingRectangle { get; private set; }

        public IntPtr OwningWindow { get; private set; }

        public abstract void Invoke();
    }
}
