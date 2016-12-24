using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trade.Mixin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace Trade.Mixin.Tests
{
    [TestClass()]
    public class DateTimeMixinTests
    {
        [TestMethod()]
        public void NearestKMinutesTest()
        {
            var sod = new DateTime(2015, 1, 1, 9, 30, 0);
            var noon = new DateTime(2015, 1, 1, 11, 30, 0);
            var afernoon = new DateTime(2015, 1, 1, 13, 0, 0);
            var eod = new DateTime(2015, 1, 1, 15, 00, 0);

            new DateTime(2015, 1, 1, 9, 31, 0).NearestKMinutes(sod, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 9, 35, 0));
            new DateTime(2015, 1, 1, 9, 35, 0).NearestKMinutes(sod, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 9, 35, 0));
            new DateTime(2015, 1, 1, 9, 35, 2).NearestKMinutes(sod, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 9, 40, 0));
            new DateTime(2015, 1, 1, 11, 30, 2).NearestKMinutes(sod, 5, noon)
                .Should()
                .Be(new DateTime(2015, 1, 1, 11, 30, 0));
            new DateTime(2015, 1, 1, 13, 00, 2).NearestKMinutes(afernoon, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 13, 5, 0));
            new DateTime(2015, 1, 1, 15, 00, 2).NearestKMinutes(afernoon, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 15, 0, 0));
            new DateTime(2015, 1, 1, 15, 20, 2).NearestKMinutes(afernoon, 5, eod)
                .Should()
                .Be(new DateTime(2015, 1, 1, 15, 0, 0));
        }
    }
}