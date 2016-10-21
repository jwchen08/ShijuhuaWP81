using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;

namespace Shijuhua
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
        }

                //本地化 ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            ApplicationBarIconButton emailAppBar = new ApplicationBarIconButton(new Uri("/icons/email.png", UriKind.Relative));
            ApplicationBarIconButton copyAppbar = new ApplicationBarIconButton(new Uri("/icons/Copy.png", UriKind.Relative));
            emailAppBar.Text = "反馈";
            copyAppbar.Text = "启动协议";
            emailAppBar.Click += emailButton_Click;
            copyAppbar.Click += copyAppbar_Click;
            ApplicationBar.Buttons.Add(emailAppBar);
            ApplicationBar.Buttons.Add(copyAppbar);
        }

        void copyAppbar_Click(object sender, EventArgs e)
        {
            Clipboard.SetText("shijuhua:Home");
            MessageBox.Show("启动协议已经复制到剪贴板，请粘贴到桌面磁贴美化应用中以启动10句话！", "提示：", MessageBoxButton.OK);

        }

        private void emailButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask ect = new EmailComposeTask();
            ect.Subject = "10句话WP8.1版v2.6.6反馈";
            ect.To = "jwchen08@qq.com";
            ect.Show();
        }

    }
}