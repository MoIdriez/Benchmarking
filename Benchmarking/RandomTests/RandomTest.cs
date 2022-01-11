﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.RandomTests
{
    public class RandomTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RandomTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void PfTests()
        {
            var p1 = new Point(1, 1);
            var p2 = new Point(1, 2);
            _testOutputHelper.WriteLine(p1.DirectionTo(p2).ToString());
            _testOutputHelper.WriteLine(p1.DistanceTo(p2).ToString());
        }

        [Fact]
        public void IsStuckTester()
        {
            var steps = new List<Point>
            {
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
            };
            var groupBy = steps.TakeLast(20).GroupBy(g => g).ToList();
            var isStuck = groupBy.Any(g => g.Count() >= 8);
            Assert.True(isStuck);
        }
    }
}