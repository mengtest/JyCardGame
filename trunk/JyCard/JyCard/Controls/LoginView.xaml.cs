using JyCard.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace JyCard.Controls
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        public void Show()
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.Account.Text = "";
            this.Password.Password = "";
        }

        private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            GameEngine.Instance.Login(Account.Text, Password.Password);
        }
    }
}
