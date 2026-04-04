using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MDR_YieldmaxTools.Tabs.CorrelationTrend;

namespace MDR_YieldmaxTools.Models
{
    public class WeeklyCorrelationData
    {
        public double[] DailyCloseValues = new double[5];

        public DateTime StartDate;

        public Dictionary<DayOfWeek, bool> DayHasBeenAdded = new Dictionary<DayOfWeek, bool>
        {
            { DayOfWeek.Monday, false },
            { DayOfWeek.Tuesday, false },
            { DayOfWeek.Wednesday, false },
            { DayOfWeek.Thursday, false },
            { DayOfWeek.Friday, false },
        };

        public bool AddDay(HistoricalTickData1D _dayTickData, out (DateTime _startDate, double[] _dailyValues) _timeSeries)
        {
            WriteLog($"\n{_dayTickData.symbol} || {_dayTickData.timestamp} || {_dayTickData.timestamp.DayOfWeek} || {_dayTickData.close ?? 0}");
            if (DayHasBeenAdded[_dayTickData.timestamp.DayOfWeek])
            {
                MessageBox.Show("ERROR", $"{_dayTickData.timestamp.DayOfWeek} has already been added. Cannot add twice.", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new ArgumentOutOfRangeException();
            }

            if (_dayTickData.timestamp.DayOfWeek == DayOfWeek.Monday)
            {
                StartDate = _dayTickData.timestamp;
            }

            int idx = ((int)_dayTickData.timestamp.DayOfWeek) - 1;
            DailyCloseValues[idx] = _dayTickData.close ?? 0;
            DayHasBeenAdded[_dayTickData.timestamp.DayOfWeek] = true;

            if (_dayTickData.timestamp.DayOfWeek == DayOfWeek.Friday)
            {
                if (DayHasBeenAdded.All(x => x.Value))
                {
                    _timeSeries._startDate = StartDate;
                    _timeSeries._dailyValues = DailyCloseValues;
                    return true;
                }
            }

            _timeSeries._startDate = default;
            _timeSeries._dailyValues = Array.Empty<double>();

            return false;
        }

        public void Reset()
        {
            DailyCloseValues = new double[5];
            DayHasBeenAdded[DayOfWeek.Monday] = false;
            DayHasBeenAdded[DayOfWeek.Tuesday] = false;
            DayHasBeenAdded[DayOfWeek.Wednesday] = false;
            DayHasBeenAdded[DayOfWeek.Thursday] = false;
            DayHasBeenAdded[DayOfWeek.Friday] = false;
            StartDate = default;
        }

        public void WriteLog(string _logLine)
        {
            File.AppendAllText("WeeklyCorrelationDataLog.txt", $"{DateTime.Now}     || {_logLine}");
        }
    }
}
