using System;
using System.Collections.Generic;
using System.Windows;

namespace HuntAndPeck.Models
{
    public class DebugHint : Hint
    {
        public DebugHint(IntPtr owningWindow, Rect boundingRectangle, IList<string> supportedPatterns)
            : base(owningWindow, boundingRectangle)
        {
            SupportedPatterns = supportedPatterns;
        }

        public IList<string> SupportedPatterns { get; set; }

        public override void Invoke()
        {

        }
    }
}
