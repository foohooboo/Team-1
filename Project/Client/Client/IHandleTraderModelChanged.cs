using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IHandleTraderModelChanged
    {
        void ProfileChanged();
        void LeaderboardChanged();
        void StockHistoryChanged();

    }
}
