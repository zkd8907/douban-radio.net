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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace Douban.LayerCollection
{
    /// <summary>
    /// Interaction logic for ControlPanel.xaml
    /// </summary>
    public partial class ControlPanel : UserControl
    {
        public static readonly RoutedEvent CloseWindowEvent;
        public static readonly RoutedEvent MinimizeWindowEvent;
        public static readonly RoutedEvent LoginEvent;
        public static readonly RoutedEvent ChangeStyleEvent;
        public static readonly RoutedEvent RestoreStyleEvent;
        static ControlPanel()
        {
            CloseWindowEvent = EventManager.RegisterRoutedEvent(
                "Close",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ControlPanel));

            MinimizeWindowEvent = EventManager.RegisterRoutedEvent(
                "Minimize",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ControlPanel));

            LoginEvent = EventManager.RegisterRoutedEvent(
                "Login",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ControlPanel));

            ChangeStyleEvent = EventManager.RegisterRoutedEvent(
                "ChangeStyle",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ControlPanel));

            RestoreStyleEvent = EventManager.RegisterRoutedEvent(
                "RestoreStyle",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ControlPanel));
        }

        public event RoutedEventHandler CloseWindow
        {
            add { AddHandler(CloseWindowEvent, value); }
            remove { RemoveHandler(CloseWindowEvent, value); }
        }

        public event RoutedEventHandler MinimizeWindow
        {
            add { AddHandler(MinimizeWindowEvent, value); }
            remove { RemoveHandler(MinimizeWindowEvent, value); }
        }

        public event RoutedEventHandler Login
        {
            add { AddHandler(LoginEvent, value); }
            remove { RemoveHandler(LoginEvent, value); }
        }

        public event RoutedEventHandler ChangeStyle
        {
            add { AddHandler(ChangeStyleEvent, value); }
            remove { RemoveHandler(ChangeStyleEvent, value); }
        }

        public event RoutedEventHandler RestoreStyle
        {
            add { AddHandler(RestoreStyleEvent, value); }
            remove { RemoveHandler(RestoreStyleEvent, value); }
        }
#region Properties
        public bool IsShowChangeStyleButton
        {
            set
            {
                if (value)
                    btnChangeStyle.Visibility = Visibility.Visible;
                else
                    btnChangeStyle.Visibility = Visibility.Collapsed;
            }

            get
            {
                return (btnChangeStyle.Visibility == Visibility.Visible);
            }
        }

        public bool IsShowMinimizeButton
        {
            set
            {
                if (value)
                    btnMinus.Visibility = Visibility.Visible;
                else
                    btnMinus.Visibility = Visibility.Collapsed;
            }

            get
            {
                return (btnMinus.Visibility == Visibility.Visible);
            }
        }

        public string Title
        {
            set { rTitle.Text = value; }
            get { return rTitle.Text; }
        }

        public string UserName
        {
            set { rLogin.Text = value; }
            get { return rLogin.Text; }
        }

        public bool EnableChangeStyleButton
        {
            get { return btnChangeStyle.IsEnabled; }
            set { btnChangeStyle.IsEnabled = value; }
        }
#endregion


        public ControlPanel()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ControlPanel.CloseWindowEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ControlPanel.MinimizeWindowEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        private void lLogo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Diagnostics.Process.Start("http://douban.fm");
            }
        }

        private void rLogin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs();
            args.RoutedEvent = ControlPanel.LoginEvent;
            args.Source = this;
            RaiseEvent(args);
        }

        private void btnChangeStyle_Click(object sender, RoutedEventArgs e)
        {
            if(btnChangeStyle.Tag.ToString() == @"Image\ChangeStyle.png")
            {
                btnChangeStyle.Tag = @"Image\RestoreStyle.png";
                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = ControlPanel.ChangeStyleEvent;
                args.Source = this;
                RaiseEvent(args);

                Storyboard sb = ((Storyboard)Resources["ChangeStyle"]).Clone();
                sb.Completed += delegate
                {
                    lLogo.Visibility = Visibility.Collapsed;
                    image.Visibility = Visibility.Collapsed;
                    textBlock.Visibility = Visibility.Collapsed;
                };
                if (Resources.Contains(textBlock))
                {
                    Resources[textBlock] = sb;
                }
                else
                {
                    Resources.Add(textBlock, sb);
                }
                sb.Begin();
            }
            else
            {
                btnChangeStyle.Tag = @"Image\ChangeStyle.png";
                RoutedEventArgs args = new RoutedEventArgs();
                args.RoutedEvent = ControlPanel.RestoreStyleEvent;
                args.Source = this;
                RaiseEvent(args);


                Storyboard sb = ((Storyboard)Resources["RestoreStyle"]).Clone();
                sb.Completed += delegate
                {
                    lLogo.Visibility = Visibility.Visible;
                    image.Visibility = Visibility.Visible;
                    textBlock.Visibility = Visibility.Visible;
                };
                if (Resources.Contains(textBlock))
                {
                    Resources[textBlock] = sb;
                }
                else
                {
                    Resources.Add(textBlock, sb);
                }
                sb.Begin();
            }
        }

    }
}
