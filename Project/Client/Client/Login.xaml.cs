using Client.Conversations;
using Client.Conversations.GetPortfolio;
using Shared;
using Shared.Comms.ComService;
using Shared.Conversations;
using Shared.PortfolioResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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

            TraderModel.Current = new TraderModel()
            {
                Portfolio = portfolio
            };

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
                Password = "password"
            };
            LoginSuccess(dummyPortfolio);
#else
            MessageBox.Show(message);
#endif
        }
    }
}
