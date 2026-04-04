using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using MDR_YieldmaxTools.Enums;
using MDR_YieldmaxTools.Tabs.Correlation;
using MDR_YieldmaxTools.Utils;
using Telerik.WinControls.UI;
using MathNet.Numerics.Statistics;
using MDR_YieldmaxTools.Models;

namespace MDR_YieldmaxTools.Tabs.CorrelationTrend
{
    /*
     * Need minimum of 3 datapoints for correlation but will produce an inaccurate result.
     * Approx 30 datapoints will suffice for identifying strong correlations ie: 0.5+
     * For weaker correlations ie: 0.5 - 0.2 need 100 - 200 datapoints.
     *
     * I will use 5 here to start with.
     */
    public class CorrelationTrendHandler
    {
        public string SymbolA;
        public string SymbolB;

        public MovingSeries MovingSeriesA = new MovingSeries(5);
        public MovingSeries MovingSeriesB = new MovingSeries(5);

        public WeeklyCorrelationData WeeklySeriesA = new WeeklyCorrelationData();
        public WeeklyCorrelationData WeeklySeriesB = new WeeklyCorrelationData();

        public BindingList<CorrelationCoefficient> DataSource;

        public CorrelationTrendHandler(string symbolA, string symbolB)
        {
            SymbolA = symbolA;
            SymbolB = symbolB;

            DataSource = GetDataSource();
        }

        public Dictionary<DateTime, CorrelationCoefficient> DailyCorrelationValuesMap = new Dictionary<DateTime, CorrelationCoefficient>();

        // public Dictionary<DateTime, CorrelationCoefficient> GetDailyCorrelationValuesMap()
        // {
        //     Dictionary<DateTime, (double[] aSeries, double[] bSeries)> dict0 = new Dictionary<DateTime, (double[] aSeries, double[] bSeries)>();
        //     Dictionary<DateTime, CorrelationCoefficient> result = new Dictionary<DateTime, CorrelationCoefficient>();
        // 
        //     using (var db = new dbDataContext())
        //     {
        //         DateTime maxMin = new DateTime(1991, 1, 1);
        //         DateTime minMax = DateTime.Now;
        // 
        //         // List<HistoricalTickData1WK> a1Ws = db.HistoricalTickData1WKs.Where(x => x.symbol == SymbolA.ToString()).ToList();
        //         // List<HistoricalTickData1WK> b1Ws = db.HistoricalTickData1WKs.Where(x => x.symbol == SymbolB.ToString()).ToList();
        //         List<HistoricalTickData1D> a1Ds = db.HistoricalTickData1Ds.Where(x => x.symbol.Equals(SymbolA.ToString())).ToList();
        //         List<HistoricalTickData1D> b1Ds = db.HistoricalTickData1Ds.Where(x => x.symbol.Equals(SymbolB.ToString())).ToList();
        // 
        //         // DateTime a1Wmin = a1Ws.Min(x => x.timestamp);
        //         // DateTime b1Wmin = b1Ws.Min(x => x.timestamp);
        //         // DateTime a1Wmax = a1Ws.Max(x => x.timestamp);
        //         // DateTime b1Wmax = b1Ws.Max(x => x.timestamp);
        // 
        //         // maxMin = DateTime.Compare(a1Wmin, b1Wmin) >= 0 ? a1Wmin : b1Wmin;
        //         // minMax = DateTime.Compare(a1Wmax, b1Wmax) < 0 ? a1Wmax : b1Wmax;
        // 
        //         // a1Ws = a1Ws.Where(x => x.timestamp >= maxMin && x.timestamp < minMax).OrderBy(y => y.timestamp).ToList();
        //         // b1Ws = b1Ws.Where(x => x.timestamp >= maxMin && x.timestamp < minMax).OrderBy(y => y.timestamp).ToList();
        // 
        //         DateTime a1Dmin = a1Ds.Min(x => x.timestamp);
        //         DateTime b1Dmin = b1Ds.Min(x => x.timestamp);
        //         DateTime a1Dmax = a1Ds.Max(x => x.timestamp);
        //         DateTime b1Dmax = b1Ds.Max(x => x.timestamp);
        // 
        //         maxMin = DateTime.Compare(a1Dmin, b1Dmin) >= 0 ? a1Dmin : b1Dmin;
        //         minMax = DateTime.Compare(a1Dmax, b1Dmax) < 0 ? a1Dmax : b1Dmax;
        // 
        //         a1Ds = db.HistoricalTickData1Ds.Where(x => x.timestamp >= maxMin && x.timestamp < minMax && x.symbol == SymbolA).OrderBy(y => y.timestamp).ToList();
        //         b1Ds = db.HistoricalTickData1Ds.Where(x => x.timestamp >= maxMin && x.timestamp < minMax && x.symbol == SymbolB).OrderBy(y => y.timestamp).ToList();
        // 
        //         double a1DClose = 0;
        //         double a1DLastClose = 0;
        // 
        //         double b1DClose = 0;
        //         double b1DLastClose = 0;
        // 
        //         for (int i = 0; i < a1Ds.Count; i++)
        //         {
        //             var a1D = a1Ds[i];
        //             if (!b1Ds.Any(x => x.timestamp.Date == a1D.timestamp.Date))
        //             {
        //                 MessageBox.Show("ERROR", $"b1DS missing date : {a1D.timestamp.Date}");
        //                 var exc = new NullReferenceException();
        //                 throw exc;
        //             }
        // 
        //             var b1D = b1Ds.Single(x => x.timestamp.Date == a1D.timestamp.Date);
        //             if (i > 0)
        //             {
        //                 a1DLastClose = a1Ds[i - 1].close ?? 0;
        //                 b1DLastClose = b1Ds[i - 1].close ?? 0;
        //                 a1DClose = a1Ds[i].close ?? 0;
        //                 b1DClose = b1Ds[i].close ?? 0;
        //             }
        //             else
        //             {
        //                 continue;
        //             }
        // 
        //             if (!dict0.ContainsKey(a1D.timestamp.Date))
        //             {
        //                 dict0.Add(a1D.timestamp.Date, (aSeries: Array.Empty<double>(), bSeries: Array.Empty<double>()));
        //             }
        // 
        //             dict0[a1D.timestamp.Date] = (aSeries: new double[] { a1DLastClose, a1DClose }, bSeries: new double[] { b1DLastClose, b1DClose });
        //         }
        // 
        //         foreach (var kvp0 in dict0)
        //         {
        //             if (!result.ContainsKey(kvp0.Key))
        //             {
        //                 result.Add(kvp0.Key, new CorrelationCoefficient(SymbolA.ToString(), SymbolB.ToString(), 0, kvp0.Key));
        //             }
        // 
        //             result[kvp0.Key].Value = MathNet.Numerics.Statistics.Correlation.Pearson(kvp0.Value.aSeries, kvp0.Value.bSeries);
        //         }
        //     }
        // 
        //     return result;
        // }

        // public Dictionary<DateTime, CorrelationCoefficient> GetCorrelationValuesMap(int _daysPerSeries)
        // {
        //     Dictionary<DateTime, CorrelationCoefficient> result = new Dictionary<DateTime, CorrelationCoefficient>();
        // 
        //     using (var db = new dbDataContext())
        //     {
        //         List<HistoricalTickData1D> dbDataA = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolA}").OrderBy(y => y.timestamp).ToList();
        //         List<HistoricalTickData1D> dbDataB = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolB}").OrderBy(y => y.timestamp).ToList();
        // 
        //         #region Identify the symbol that started earliest, set it as first to iterate.
        // 
        //         List<HistoricalTickData1D> firstToIterate = new List<HistoricalTickData1D>();
        //         List<HistoricalTickData1D> secondToIterate = new List<HistoricalTickData1D>();
        // 
        //         if (dbDataA.Min(x => x.timestamp) <= dbDataB.Min(x => x.timestamp))
        //         {
        //             firstToIterate = dbDataA;
        //             secondToIterate = dbDataB;
        //         }
        //         else
        //         {
        //             firstToIterate = dbDataB;
        //             secondToIterate = dbDataA;
        //         }
        // 
        //         #endregion
        // 
        //         #region Iterate through dbData, every 5th loop  calculate correlation for the 5 day sets and add to result.
        // 
        //         for (int i = 0; i < firstToIterate.Count; i++)
        //         {
        //             var currentDayDataA = firstToIterate[i];
        // 
        //             // start from first monday.
        //             if (i < 5)
        //             {
        //                 if (currentDayDataA.timestamp.DayOfWeek != DayOfWeek.Monday)
        //                 {
        //                     continue;
        //                 }
        //             }
        //             double currentCloseA = currentDayDataA.close ?? 0;
        //             if (currentCloseA == 0)
        //             {
        //                 MessageBox.Show("ERROR", $"{currentDayDataA.timestamp} {currentDayDataA.symbol} Series A close value is zero.");
        //                 return null;
        //             }
        // 
        //             if (!secondToIterate.Any(x => x.timestamp.Date.Equals(currentDayDataA.timestamp.Date)))
        //             {
        //                 continue;
        //             }
        // 
        //             var currentDayDataB = secondToIterate.Single(x => x.timestamp.Date.Equals(currentDayDataA.timestamp.Date));
        //             double currentCloseB = currentDayDataB.close ?? 0;
        //             if (currentCloseB == 0)
        //             {
        //                 MessageBox.Show("ERROR", $"{currentDayDataB.timestamp} {currentDayDataA.symbol} Series B close value is zero.");
        //                 return null;
        //             }
        // 
        //             if (this.MovingSeriesA.Add(currentCloseA, out var seriesA) && this.MovingSeriesB.Add(currentCloseB, out var seriesB))
        //             {
        //                 CorrelationCoefficient cc = new CorrelationCoefficient(currentDayDataA.symbol, currentDayDataB.symbol, MathNet.Numerics.Statistics.Correlation.Pearson(seriesA, seriesB), currentDayDataA.timestamp.Date);
        // 
        //                 if (!result.ContainsKey(currentDayDataA.timestamp.Date))
        //                 {
        //                     result.Add(currentDayDataA.timestamp.Date, null);
        //                 }
        // 
        //                 result[currentDayDataA.timestamp.Date] = cc;
        //             }
        //         }
        // 
        //         #endregion
        //     }
        // 
        //     return result;
        // }


        public Dictionary<DateTime, CorrelationCoefficient> GetCorrelationValuesMap() // Monday to Friday periods.
        {
            Dictionary<DateTime, CorrelationCoefficient> result = new Dictionary<DateTime, CorrelationCoefficient>();

            using (var db = new dbDataContext())
            {
                List<HistoricalTickData1D> dbDataA = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolA}").OrderBy(y => y.timestamp).ToList();
                List<HistoricalTickData1D> dbDataB = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolB}").OrderBy(y => y.timestamp).ToList();

                // Identify the symbol that started earliest, set it as second to iterate.
                #region Identify earliest starter.

                List<HistoricalTickData1D> firstToIterate = new List<HistoricalTickData1D>();
                List<HistoricalTickData1D> secondToIterate = new List<HistoricalTickData1D>();

                if (dbDataA.Min(x => x.timestamp) <= dbDataB.Min(x => x.timestamp))
                {
                    firstToIterate = dbDataB;
                    secondToIterate = dbDataA;
                }
                else
                {
                    firstToIterate = dbDataA;
                    secondToIterate = dbDataB;
                }

                #endregion

                #region Iterate through dbData, add monday - friday to WeeklyCorrelationVector and reset after calculating correlation for the 5 day set and add to result.

                bool mondayA = false;
                bool mondayB = false;

                for (int i = 0; i < firstToIterate.Count; i++)
                {
                    var currentDayDataA = firstToIterate[i];

                    // find first monday.
                    if (!mondayA)
                    {
                        if (currentDayDataA.timestamp.DayOfWeek != DayOfWeek.Monday)
                        {
                            continue;
                        }

                        mondayA = true;
                    }

                    double currentCloseA = currentDayDataA.close ?? 0;
                    if (currentCloseA == 0)
                    {
                        MessageBox.Show("ERROR", $"{currentDayDataA.timestamp} {currentDayDataA.symbol} Series A close value is zero.");
                        return null;
                    }

                    if (!secondToIterate.Any(x => x.timestamp.Date.Equals(currentDayDataA.timestamp.Date)))
                    {
                        continue;
                    }

                    var currentDayDataB = secondToIterate.Single(x => x.timestamp.Date.Equals(currentDayDataA.timestamp.Date));

                    //  start when find second monday.
                    if (!mondayB)
                    {
                        if (currentDayDataB.timestamp.DayOfWeek != DayOfWeek.Monday)
                        {
                            continue;
                        }

                        mondayB = true;
                    }

                    double currentCloseB = currentDayDataB.close ?? 0;
                    if (currentCloseB == 0)
                    {
                        MessageBox.Show("ERROR", $"{currentDayDataB.timestamp} {currentDayDataA.symbol} Series B close value is zero.");
                        return null;
                    }

                    (DateTime _startDateA, double[] _dailyValuesA) _timeSeriesA = new ValueTuple<DateTime, double[]>(default, Array.Empty<double>());
                    (DateTime _startDateB, double[] _dailyValuesB) _timeSeriesB = new ValueTuple<DateTime, double[]>(default, Array.Empty<double>());

                    bool endA = this.WeeklySeriesA.AddDay(currentDayDataA, out _timeSeriesA);
                    bool endB = this.WeeklySeriesB.AddDay(currentDayDataB, out _timeSeriesB);

                    if (endA && endB)
                    {
                        CorrelationCoefficient cc = new CorrelationCoefficient(currentDayDataA.symbol, currentDayDataB.symbol, MathNet.Numerics.Statistics.Correlation.Pearson(_timeSeriesA._dailyValuesA, _timeSeriesB._dailyValuesB), _timeSeriesA._startDateA.Date);

                        if (!result.ContainsKey(_timeSeriesA._startDateA.Date))
                        {
                            result.Add(_timeSeriesA._startDateA.Date, null);
                        }

                        result[_timeSeriesA._startDateA.Date] = cc;
                        this.WeeklySeriesA.Reset();
                        this.WeeklySeriesB.Reset();
                    }
                }

                #endregion
            }

            return result;
        }

        public Dictionary<DateTime, CorrelationCoefficient> GetMonthlyCorrelationValuesMap()
        {
            Dictionary<DateTime, CorrelationCoefficient> result = new Dictionary<DateTime, CorrelationCoefficient>();

            var x = new DateTime(2001, 1, 1);
            var m = x.Date.Month;

            List<HistoricalTickData1D> dbDataA = new List<HistoricalTickData1D>();
            List<HistoricalTickData1D> dbDataB = new List<HistoricalTickData1D>();

            using (var db = new dbDataContext())
            {
                dbDataA = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolA}").OrderBy(y => y.timestamp).ToList();
                dbDataB = db.HistoricalTickData1Ds.Where(x => x.symbol == $"{this.SymbolB}").OrderBy(y => y.timestamp).ToList();

                var groupedDataA = dbDataA.OrderBy(z => z.timestamp).GroupBy(x => new { x.timestamp.Date.Year, x.timestamp.Date.Month })
                    .Select(y => new { Year = y.Key.Year, Month = y.Key.Month, Items = y.ToList() });

                foreach (var groupA in groupedDataA)
                {
                    List<double> aSeries = new List<double>();
                    List<double> bSeries = new List<double>();

                    DateTime keyDate = new DateTime(groupA.Year, groupA.Month, 1);

                    foreach (HistoricalTickData1D tickDataA in groupA.Items)
                    {
                        if (!dbDataB.Any(x => x.timestamp.Date == tickDataA.timestamp.Date)) continue;
                        bSeries.Add(dbDataB.Single(x => x.timestamp.Date == tickDataA.timestamp.Date).close ?? 0);
                        aSeries.Add(tickDataA.close ?? 0);
                    }

                    result.Add(keyDate, new CorrelationCoefficient(SymbolA, SymbolB, MathNet.Numerics.Statistics.Correlation.Pearson(aSeries, bSeries), keyDate));
                }
            }

            return result;
        }

        public BindingList<CorrelationCoefficient> GetDataSource()
        {
            this.DailyCorrelationValuesMap = this.GetMonthlyCorrelationValuesMap();
            BindingList<CorrelationCoefficient> result = new BindingList<CorrelationCoefficient>(this.DailyCorrelationValuesMap.Values.OrderBy(x => x.Timestamp).ToList());
            return result;
        }
    }
}
