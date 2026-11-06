using System;
using System.Collections.Generic;
using System.Windows;
using HuntAndPeck.Models;
using HuntAndPeck.Services;
using Xunit;

namespace HuntAndPeck.Tests.Services
{
    public class HintOverlapTest
    {
        private class TestHint : Hint
        {
            public TestHint(Rect rect) : base(IntPtr.Zero, rect) { }
            public override void Invoke() { }
        }

        [Fact]
        public void IntersectionOverUnion_Disjoint_IsZero()
        {
            Assert.Equal(0, HintOverlap.IntersectionOverUnion(new Rect(0, 0, 10, 10), new Rect(20, 20, 10, 10)));
        }

        [Fact]
        public void IntersectionOverUnion_Identical_IsOne()
        {
            Assert.Equal(1, HintOverlap.IntersectionOverUnion(new Rect(0, 0, 10, 10), new Rect(0, 0, 10, 10)), 6);
        }

        [Fact]
        public void RemoveNearDuplicates_KeepsFirstOfNearlyIdenticalRects()
        {
            var first = new TestHint(new Rect(0, 0, 100, 20));
            var second = new TestHint(new Rect(1, 0, 100, 20));

            var result = HintOverlap.RemoveNearDuplicates(new List<Hint> { first, second });

            Assert.Single(result);
            Assert.Same(first, result[0]);
        }

        [Fact]
        public void RemoveNearDuplicates_KeepsSmallControlInsideLargeOne()
        {
            var container = new TestHint(new Rect(0, 0, 200, 200));
            var button = new TestHint(new Rect(10, 10, 20, 20));

            Assert.Equal(2, HintOverlap.RemoveNearDuplicates(new List<Hint> { container, button }).Count);
        }
    }
}
