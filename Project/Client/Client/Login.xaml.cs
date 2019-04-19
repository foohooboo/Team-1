using Client.Conversations.CreatePortfolio;
using Client.Conversations.GetPortfolio;
using Client.Models;
using Shared;
using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Client
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window, IHandleLogin
    {
        public Login()
        {
            InitializeComponent();
            ConversationManager.Start(ConversationBuilder.Builder);

            ComService.AddUdpClient(Config.DEFAULT_UDP_CLIENT, 0);

            TraderModel.Current = new TraderModel();


        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //registering
            if (register.IsChecked.Value)
            {
                var PRC = new CreatePortfolioRequestConversation(Config.GetClientProcessNumber());
                PRC.SetInitialState(new CreatePortfolioRequestState(user.Text, pass.Password, confirm.Password, this, PRC, null));
                ConversationManager.AddConversation(PRC);
            }

            //logging in
            else
            {
                var loginConv = new GetPortfolioRequestConversation(Config.GetClientProcessNumber());
                loginConv.SetInitialState(new GetPortfolioRequestState(user.Text, pass.Password, this, loginConv));
                ConversationManager.AddConversation(loginConv);
            }
        }

        private void Register_Checked(object sender, RoutedEventArgs e)
        {
            confirm.IsEnabled = !confirm.IsEnabled;
        }

        public void LoginSuccess(Portfolio portfolio)
        {
            TraderModel.Current.Portfolio = portfolio;

            Dispatcher.Invoke(() =>
            {
                MainWindow main = new MainWindow();
                Application.Current.Windows[0].Close();
                main.ShowDialog();
            });
        }

        public void LoginFailure(string message)
        {
#if DEBUG
            //Please keep this code in the DEBUG directive. It allows us to advance to MainWindow
            //without having a Broker process running. It should NOT be executed in production.
            //-Dsphar 4/8/2019

            if (string.IsNullOrEmpty(message))
            {

                if (TraderModel.Current.StockHistory.Count == 0 || TraderModel.Current.StockHistory[0].TradedCompanies.Count == 0)
                {
                    TraderModel.Current.StockHistory = ManagedData.makeupMarketSegment(10, 50);
                }

                Random rand = new Random();

                var assets = new Dictionary<string, Asset>
            {
                { "$", new Asset(new Stock() { Symbol = "$" }, rand.Next(10000, 500000)) }
            };

                for (int i = 1; i < 6; i++)
                {
                    var asset = new Asset(TraderModel.Current.StockHistory[0].TradedCompanies[i], rand.Next(1000));
                    assets.Add(asset.RelatedStock.Symbol, asset);
                }

                var dummyPortfolio = new Portfolio()
                {
                    Username = "DebugPortfolioName",
                    Password = "password",
                    Assets = assets
                };

                LoginSuccess(dummyPortfolio);
            }
            else
            {
                MessageBox.Show(message);
            }


#else
            MessageBox.Show(message);
#endif
        }
    }
}
