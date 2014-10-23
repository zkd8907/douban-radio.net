// written by bobby. zkd8907@live.com All Rights Reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DoubanFM;
using Douban.LayerCollection;

namespace Douban
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        API api;
        public LoginWindow(API api)
        {
            InitializeComponent();
            this.api = api;
            api.ValidateCompleted += new API.ValidateCompletedEventHandler(api_ValidateCompleted);
        }

        private void api_ValidateCompleted(User CurrentUser)
        {
            tbUsername.IsEnabled = true;
            pbPassword.IsEnabled = true;
            btnLogin.IsEnabled = true;
            if (!CurrentUser.r)
            {
                Properties.Settings.Default.CurrentUser = CurrentUser;
                Properties.Settings.Default.Save();
                if (this.DialogResult != true)
                    this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("邮箱或密码错误。", "登录失败", MessageBoxButton.OK, MessageBoxImage.Information);
                tbUsername.Focus();
                return;
            }
        }

        private void pnlControl_MoveTitle(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }

        private void ControlPanel_CloseWindow(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void loginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (tbUsername.Text.Length == 0)
            {
                MessageBox.Show("邮箱不能为空。", "登录失败", MessageBoxButton.OK, MessageBoxImage.Information);
                tbUsername.Focus();
                return;
            }
            if (pbPassword.Password.Length == 0)
            {
                MessageBox.Show("密码不能为空。", "登录失败", MessageBoxButton.OK, MessageBoxImage.Information);
                pbPassword.Focus();
                return;
            }
            api.Validate(tbUsername.Text, pbPassword.Password);
            tbUsername.IsEnabled = false;
            pbPassword.IsEnabled = false;
            btnLogin.IsEnabled = false;
        }

        private void loginWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == this.GetType())
                this.DragMove();
        }
    }
}
