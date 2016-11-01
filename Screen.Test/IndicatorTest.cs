using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Screen.Indicator;
using Screen.Parsers;
using Screen.Mixin;

namespace Screen.Test
{
    [TestClass]
    public class IndicatorTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var date = DateTime.Today;
            //var data = DataSeriesParser.Parse(@"D:\stockdata\SH600000.txt").Section(date);
            //data.Dump();
            //data.Bottom().Dump();
            //var ma = new MA(data.CloseTimeSeries(), 5);
            //ma.Dump();
            //var jax = new JAX(data, 20, 3);
            //jax.Dump();
            //var cci = new CCI(data, 5);
            //cci.Dump();
            //var macd = new MACD(data);
            //macd.Dump();
            //var jax = new JAX(data, 20, 3);
            //var ind = new PEAK(new PEAK(jax.A()));
            //ind.Dump();

            //var jax = new JAX(data, 20, 3);
            //var ind = new PEAK_BREAK(jax.A());
            //ind.Dump();
        }

        [TestMethod]
        public void PEAK_BREAK_Test()
        {
            var date = DateTime.Today;
            date = new DateTime(2014, 5, 21);

            var data = DataSeriesParser.ParseFile(@"D:\stockdata\SH600012.txt");
            var jax = new JAX(data.Data, 20, 3);
            var ind = new BREAK_BACK(jax.A());
            ind.Dump();
        }
    }
}
