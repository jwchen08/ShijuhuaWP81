using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using RenrenSDKLibrary;

namespace Shijuhua
{
    public partial class RenrenSharePage : PhoneApplicationPage
    {
        public RenrenSharePage()
        {
            InitializeComponent();

            renrenTextBox.Text = App.wordsToShare;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (NavigationContext.QueryString.ContainsKey("prepage") && ((NavigationContext.QueryString["prepage"] == "webcontrolpage")
                || (NavigationContext.QueryString["prepage"] == "passwordpage")))
            {
                NavigationService.RemoveBackEntry();
            }
        }

        //点击发布按钮
        private void publishButton_Click(object sender, EventArgs e)
        {
            if (renrenTextBox.Text == "")
                MessageBox.Show("内容不能为空！");
            else
            {
                List<APIParameter> param = new List<APIParameter>();
                param.Add(new APIParameter("method", "status.set"));
                param.Add(new APIParameter("status", renrenTextBox.Text));

                App.api.RequestAPIInterface(UpdateStatusCompletedCallBack, param);
            }
        }

        //发布结果
        private void UpdateStatusCompletedCallBack(object sender, APIRequestCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                MessageBox.Show("发布成功！");
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show(e.Error.Message);
                NavigationService.GoBack();
            }
        }

        //点击取消按钮
        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private void logOutButton_Click(object sender, EventArgs e)
        {
            App.api.LogOut();
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }


    }
}