using Client.Conversations.GetPortfolio;
using Shared;
using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.MarketStructures;
using Shared.PortfolioResources;
using System;
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

            ComService.AddClient(Config.DEFAULT_UDP_CLIENT, 0);

            TraderModel.Current = new TraderModel();


        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            //registering
            if (register.IsChecked.Value)
            {
                if (pass.Password == confirm.Password)//If they equal
                {
                    //TODO: initialize createPortfolio conversation
                }
            }

            //logging in
            else
            {
                var loginConv = new GetPortfolioRequestConversation(Config.GetInt(Config.CLIENT_PROCESS_NUM));
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
                MainWindow main = new MainWindow(TraderModel.Current);
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
            
            var dummyPortfolio = new Portfolio()
            {
                Username = "DebugPortfolioName",
                Password = "password",
            };

            if (TraderModel.Current.StockHistory.Count==0 || TraderModel.Current.StockHistory[0].TradedCompanies.Count == 0)
            {
                TraderModel.Current.StockHistory = ManagedData.makeupMarketSegment(10, 30);
            }

            Random rand = new Random();

            dummyPortfolio.ModifyAsset(new Asset(new Stock() { Symbol = "$" }, rand.Next(10000,500000)));

            for (int i = 0; i < 6; i++)
            {
                dummyPortfolio.ModifyAsset(new Asset(TraderModel.Current.StockHistory[0].TradedCompanies[i], rand.Next(1000)));
            }

            LoginSuccess(dummyPortfolio);
#else
            MessageBox.Show(message);
#endif
        }
    }
}
