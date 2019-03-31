using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string username = user.Text;
            string password = pass.Text;
            if (register.IsChecked.Value)//registering?
            {
                if (password == confirm.Text)//If they equal
                {

                }
            }
            MainWindow main = new MainWindow();
            Application.Current.Windows[0].Close();
            main.ShowDialog();
        }
    }
}
