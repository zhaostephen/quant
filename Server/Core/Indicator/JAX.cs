using Screen.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Screen.Indicator
{
    public class JAX : JAXSeries
    {
        public JAX(DataSeries data, int N, int M)
        {
            var MA = new MA(data.CloseTimeSeries(), N);
            var MA3 = new MA(data.TimeSeries(d => (2 * d.Close + d.Low + d.High) / 4), M);
            var func = new Func<DataPoint, double, double>((d, salt) => d.Close/salt * (2 * d.Close + d.Low + d.High) / 4);
            var dmaPrevValue = 0d;

            for (int i = M - 1; i < data.Count; i++)
            {
                var curr = data[i];

                if (MA.WHICH(curr.Date) == 0d) continue;

                var AA = Math.Abs((2 * curr.Close + curr.High + curr.Low) / 4 - MA.WHICH(curr.Date)) / MA.WHICH(curr.Date);
                var dmaValue = (2 * curr.Close + curr.Low + curr.High) / 4;
                var jax = AA * dmaValue + (1 - AA) * dmaPrevValue;
                dmaPrevValue = jax;

                var MA1 = data.Skip(i + 1 - M).Take(M).Select(p => func(p, jax)).Average();
                var MAAA = ((MA1 - jax) / jax) / 3.0d;
                var TMP = MA1 - MAAA * MA1;
                var J = TMP <= jax ? jax : 0d;
                var A = TMP;
                var X = TMP <= jax ? TMP : 0d;

                this.Add(new JAXPoint { Date = curr.Date, JAX = Math.Round(jax, 2), J = Math.Round(J, 2), A = Math.Round(A, 2), X = Math.Round(X, 2) });
            }

            //AA:=ABS((2*CLOSE+HIGH+LOW)/4-MA(CLOSE,N))/MA(CLOSE,N);
            //JAX:DMA((2*CLOSE+LOW+HIGH)/4,AA),LINETHICK3,COLORMAGENTA;
            //CC:=(CLOSE/JAX);
            //MA1:=MA(CC*(2*CLOSE+HIGH+LOW)/4,3);
            //MAAA:=((MA1-JAX)/JAX)/3;
            //TMP:=MA1-MAAA*MA1;
            //J:IF(TMP<=JAX,JAX,DRAWNULL),LINETHICK3,COLORCYAN;
            //A:TMP,LINETHICK2,COLORYELLOW;
            //X:IF(TMP<=JAX,TMP,DRAWNULL),LINETHICK2,COLORGREEN;
        }
    }
}
