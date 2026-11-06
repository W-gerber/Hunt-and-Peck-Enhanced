using System;
using System.Collections.Generic;
using System.Windows;
using HuntAndPeck.Models;

namespace HuntAndPeck.Services
{
    internal static class HintOverlap
    {
        // Intersection over union; 0 for disjoint or empty rects.
        public static double IntersectionOverUnion(Rect a, Rect b)
        {
            if (a.IsEmpty || b.IsEmpty)
            {
                return 0;
            }

            var intersection = Rect.Intersect(a, b);
            if (intersection.IsEmpty)
            {
                return 0;
            }

            var intersectionArea = intersection.Width * intersection.Height;
            var unionArea = (a.Width * a.Height) + (b.Width * b.Height) - intersectionArea;
            return unionArea <= 0 ? 0 : intersectionArea / unionArea;
        }

        // Keeps the first of any group of hints that almost completely cover each
        // other (a control and its inner wrapper, say); enumeration order is
        // tree order, so the outermost element wins.
        public static List<Hint> RemoveNearDuplicates(IList<Hint> hints, double threshold = 0.85)
        {
            var kept = new List<Hint>(hints.Count);
            foreach (var hint in hints)
            {
                var duplicate = false;
                for (var i = kept.Count - 1; i >= 0; --i)
                {
                    if (IntersectionOverUnion(kept[i].BoundingRectangle, hint.BoundingRectangle) > threshold)
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                {
                    kept.Add(hint);
                }
            }

            return kept;
        }
    }
}
