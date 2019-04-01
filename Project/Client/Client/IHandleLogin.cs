using Shared.PortfolioResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IHandleLogin
    {
        void LoginSuccess(Portfolio portfolio);
        void LoginFailure(string message);
    }
}
