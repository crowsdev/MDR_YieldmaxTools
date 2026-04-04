using System;
using System.Collections.Generic;
using System.Linq;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools.Tabs.Correlation
{
    public class CorrelationVectorsFactory
    {
        public List<Symbols> SymbolsToCorrelate = new List<Symbols>();
        public double[][] VectorsForPearsonMatrix = Array.Empty<double[]>();

        public Dictionary<Symbols, List<HistoricalTickData1D>> SymbolDataMap = new Dictionary<Symbols, List<HistoricalTickData1D>>();

        public Dictionary<DateTime, Dictionary<Symbols, HistoricalTickData1D>> DateSymbolDataMap = new Dictionary<DateTime, Dictionary<Symbols, HistoricalTickData1D>>();

        public Symbols[] IndexedSymbolsArray;

        private DateTime maxMinDate = default;
        private DateTime minMaxDate = default;

        public CorrelationVectorsFactory(List<Symbols> _symbols)
        {
            this.SymbolsToCorrelate = _symbols;
            this.SymbolDataMap = GetTrimmedDbData(this.SymbolsToCorrelate);
            // this.DateSymbolDataMap = GetDateGroupedDbData(this.SymbolDataMap);
            this.VectorsForPearsonMatrix = GetVectorsFromDbData(this.SymbolDataMap);
        }

        public double[][] GetVectorsFromDbData(Dictionary<Symbols, List<HistoricalTickData1D>> _symbolData)
        {
            DateTime currDay = maxMinDate;
            int totalDays = (this.minMaxDate - this.maxMinDate).Days;
            int totalSymbols = this.SymbolsToCorrelate.Count;
            this.IndexedSymbolsArray = new Symbols[totalSymbols];
            int idx = 0;
            object[] result = new object[totalSymbols];

            foreach (Symbols sym in this.SymbolsToCorrelate)
            {
                double[] valueArray = new double[totalDays + 1];

                for (int i = 0; i <= totalDays; i++)
                {
                    currDay += TimeSpan.FromDays(i);
                    HistoricalTickData1D dataRow = new HistoricalTickData1D
                    {
                        symbol = sym.ToString(),
                        timestamp = currDay,
                        open = 0,
                        high = 0,
                        low = 0,
                        close = 0,
                        adjustedClose = 0,
                        volume = 0
                    };
                    var dataRows = _symbolData[sym];
                    if (dataRows.Any(x => x.timestamp == currDay))
                    {
                        dataRow = dataRows.Single(x => x.timestamp == currDay);
                    }

                    valueArray[i] = dataRow.adjustedClose ?? 0.00;
                }

                result[idx] = valueArray;
                this.IndexedSymbolsArray[idx] = sym;
                idx++;
            }

            return result.Cast<double[]>().ToArray();
        }



        public Dictionary<DateTime, Dictionary<Symbols, HistoricalTickData1D>> GetDateGroupedDbData(Dictionary<Symbols, List<HistoricalTickData1D>> _symbolData)
        {
            Dictionary<DateTime, Dictionary<Symbols, HistoricalTickData1D>> result = new Dictionary<DateTime, Dictionary<Symbols, HistoricalTickData1D>>();

            DateTime currDay = this.maxMinDate; // start date.

            int totalDays = (this.minMaxDate - this.maxMinDate).Days;

            for (int i = 0; i <= totalDays; i++)
            {
                currDay += TimeSpan.FromDays(i);

                if (currDay > minMaxDate)
                {
                    break;
                }

                if (!result.ContainsKey(currDay))
                {
                    result.Add(currDay, new Dictionary<Symbols, HistoricalTickData1D>());
                }

                foreach (Symbols sym in SymbolsToCorrelate)
                {
                    var symData = _symbolData[sym];

                    // check data exists for current date.
                    if (!symData.Any(x => x.timestamp == currDay))
                    {
                        Console.WriteLine($"Failed to find datarow for {sym} on {currDay}");
                        return null;
                    }

                    // var rowForDate = symData.Single(x => x.timestamp.Month == currDay.Month && x.timestamp.Day == currDay.Day);
                    var rowForDate = symData.Single(x => x.timestamp == currDay);

                    if (!result[currDay].ContainsKey(sym))
                    {
                        result[currDay].Add(sym, rowForDate);
                    }
                }
            }

            return result;
        }

        public Dictionary<Symbols, List<HistoricalTickData1D>> GetTrimmedDbData(List<Symbols> _symbols)
        {
            Dictionary<Symbols, List<HistoricalTickData1D>> result = new Dictionary<Symbols, List<HistoricalTickData1D>>();

            using (var db = new dbDataContext())
            {
                foreach (Symbols symbol in _symbols)
                {
                    if (!result.ContainsKey(symbol))
                    {
                        result.Add(symbol, new List<HistoricalTickData1D>());
                    }

                    string symbolTxt = symbol.ToString();

                    var dbData = db.HistoricalTickData1Ds.Where(x => x.symbol.Equals(symbolTxt)).ToList();

                    result[symbol].AddRange(dbData);
                }
            }
            maxMinDate = new DateTime(1991, 1, 1);
            minMaxDate = DateTime.Today;

            foreach (var kvp0 in result)
            {
                // Find the date range that all symbols have data for.
                DateTime symbolMinDate = kvp0.Value.Select(x => x.timestamp).Min();
                DateTime symbolMaxDate = kvp0.Value.Select(x => x.timestamp).Max();

                // common starting date
                if (symbolMinDate > maxMinDate)
                {
                    maxMinDate = symbolMinDate;
                }

                // common ending date
                if (symbolMaxDate < minMaxDate)
                {
                    minMaxDate = symbolMaxDate;
                }
            }

            // Trim rows so only common dates are left.
            foreach (var sym in _symbols)
            {
                result[sym].RemoveAll(x => x.timestamp < maxMinDate || x.timestamp > minMaxDate);
            }

            return result;
        }


    }
}