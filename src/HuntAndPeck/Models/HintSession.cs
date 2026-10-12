using System;
using System.Collections.Generic;
using System.Windows;

namespace HuntAndPeck.Models
{
    public class HintSession
    {

        public IList<Hint> Hints { get; set; }

        public IntPtr OwningWindow { get; set; }

        public Rect OwningWindowBounds { get; set; }
    }
}
