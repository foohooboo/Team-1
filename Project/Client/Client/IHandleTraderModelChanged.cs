using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IHandleTraderModelChanged
    {
        void LeaderboardChanged();
        void StockHistoryChanged();
        void ReDrawPortfolioItems();
    }
}
