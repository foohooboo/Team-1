using Shared.MarketStructures;
using System;
using System.Collections.Generic;

namespace StockServer.Data
{
    public sealed class CompanyDataRaw
    {
        public Stock MetaData;

        private DateTime _curretnDate = DateTime.MinValue;
        public DateTime CurrentDate
        {
            get
            {
                if (_curretnDate==DateTime.MinValue&&DailyValue.Count>0)
                {
                    _curretnDate = GetDate(DailyValue[0]);
                }
                return _curretnDate;
            }
            private set
            {
                _curretnDate = value;
            }
        }
        public List<string> DailyValue = new List<string>();

        private int Index;

        public string CurrentDayData()
        {
            return DailyValue[Index];
        }

        public string AdvanceDay()
        {
            ++Index;
            CurrentDate = GetDate(DailyValue[Index]);
            return DailyValue[Index];
        }

        public bool AdvanceTo(DateTime date)
        {
            var succeed = true;
            try
            {
                var currentDate = GetDate(CurrentDayData());
                while (currentDate < date)
                    currentDate = GetDate(AdvanceDay());
            }
            catch
            {
                succeed = false;
            }
            return succeed;
        }

        private static DateTime GetDate(string dayLine)
        {
            return (DateTime.Parse(dayLine.Substring(0, dayLine.IndexOf(','))));
        }
    }
}
