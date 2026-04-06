using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Utils;

namespace MDR_YieldmaxTools.Tabs.DivPerDollar
{
    public class DivPerDollarHandler
    {
        public List<DivPerDollarDataItem> Rows;

        public DivPerDollarHandler()
        {
            Rows = GetRows();
        }

        public List<DivPerDollarDataItem> GetRows()
        {
            List<DivPerDollarDataItem> result = new List<DivPerDollarDataItem>();

            var tickDbDatas = new Dictionary<Symbols,HistoricalTickData1D[]>();
            var divDbDatas = new Dictionary<Symbols,HistoricalDividendData[]>();

            double h = -1;
            double l = Single.MaxValue;
            double o = -1;
            double c = 0;

            MovingAvg dpdMA = new MovingAvg(7);

            using (var db = new dbDataContext())
            {
                #region Pull data from DB into dictionaries.

                foreach (Symbols symbol in GlobalVars.AllSymbols)
                {
                    if (!tickDbDatas.ContainsKey(symbol))
                    {
                        tickDbDatas.Add(symbol, db.HistoricalTickData1Ds.Where(x => x.symbol == symbol.ToString()).OrderBy(y => y.timestamp).ToArray());
                    }

                    if (!divDbDatas.ContainsKey(symbol))
                    {
                        divDbDatas.Add(symbol, db.HistoricalDividendDatas.Where(x => x.symbol == symbol.ToString()).OrderBy(y => y.timestamp).ToArray());
                    }
                }

                #endregion

                #region Iterate through ticks until dividend.

                foreach (Symbols symbol in GlobalVars.AllSymbols)
                {
                    for (int i = 0; i < tickDbDatas[symbol].Length; i++)
                    {
                        var tick = tickDbDatas[symbol][i];
                        var tH = tick.high ?? 0;
                        var tL = tick.low ?? 0;
                        var tC = tick.close ?? 0;
                        var tO = tick.open ?? 0;

                        if (tH > h) h = tH;
                        if (tL < l) l = tL;
                        if (o == -1) o = tO;
                        c = tC;

                        if (divDbDatas[symbol].Any(x => x.timestamp.Date == tick.timestamp.Date))
                        {
                            var div = divDbDatas[symbol].Single(x => x.timestamp.Date == tick.timestamp.Date);
                            var typical = (h + l + c) / 3;
                            var divVal = div.dividend ?? 0;
                            var dpd = divVal / typical;
                            dpdMA.Add(dpd);
                            result.Add(new DivPerDollarDataItem
                            {
                                Symbol = symbol,
                                Timestamp = div.timestamp.Date,
                                LastClose = (decimal) Math.Round(c, 4),
                                TypicalPrice = (decimal) Math.Round(typical, 4),
                                Dividend = (decimal) Math.Round(divVal, 4),
                                DivPerDollar = (decimal) Math.Round(dpd, 4),
                                DivPerDol_4MA = (decimal) Math.Round(dpdMA.Average, 4),
                                DPD_Vs_4MA = (int) Math.Round(SussMath.GetPercentChange(dpdMA.Average, dpd))
                            });

                            h = -1;
                            l = Single.MaxValue;
                            o = -1;
                            c = 0;
                        }
                    }
                }

                #endregion
            }

            return result;
        }
    }
}