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
    public partial class FavoritePage : PhoneApplicationPage
    {
        public FavoritePage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            FavoriteListBox.ItemsSource = App.favoriteItemList;
        }

        private void FavoriteListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选索引为 -1 (没有选定内容)，则不执行任何操作
            if (FavoriteListBox.SelectedIndex == -1)
                return;

            // 导航到新页面
            NavigationService.Navigate(new Uri("/FavoriteDetailPage.xaml?selectedItem=" + FavoriteListBox.SelectedIndex, UriKind.Relative));

            // 将所选索引重置为 -1 (没有选定内容)
            FavoriteListBox.SelectedIndex = -1;

        }

         //本地化 ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            ApplicationBarIconButton exportAppBar = new ApplicationBarIconButton(new Uri("/icons/Export.png", UriKind.Relative));
            ApplicationBarIconButton clearAppBar = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            exportAppBar.Text = "全部导出";
            clearAppBar.Text = "清空收藏";
            exportAppBar.Click += exportAppBar_Click;
            clearAppBar.Click += clearAppBar_Click;

            ApplicationBar.Buttons.Add(exportAppBar);
            ApplicationBar.Buttons.Add(clearAppBar);
        }

        //全部导出
        void exportAppBar_Click(object sender, EventArgs e)
        {
            string collections = "";
            for (int i = 0; i < App.favoriteItemList.Count; i++)
            {
                collections += App.favoriteItemList[i].PublishedDate+"\n";
                collections += App.favoriteItemList[i].PlainSummary+"\n";
            }
            EmailComposeTask ect = new EmailComposeTask();
            ect.Subject = "10句话收藏导出";
            ect.Body = collections;
            ect.Show();
        }

        //清空收藏
        void clearAppBar_Click(object sender, EventArgs e)
        {
            MessageBoxResult msgRst = MessageBox.Show("确认清空收藏？", "提示", MessageBoxButton.OKCancel);
            if (msgRst == MessageBoxResult.OK)
            {
                App.favoriteItemList.Clear();
            }
        }

    }
}