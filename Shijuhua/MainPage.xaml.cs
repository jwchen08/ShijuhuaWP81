using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Shijuhua.Resources;
using System.Collections.ObjectModel;
using Coding4Fun.Toolkit.Controls;
using HtmlAgilityPack;
using HtmlExtractor;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using System.IO;

namespace Shijuhua
{
    public partial class MainPage : PhoneApplicationPage
    {
        //记录是否加载完成，防止重复加载
        public static bool isContentLoaded = false;
        //主页10句话列表
        private ObservableCollection<RssItem> mainItemList=new ObservableCollection<RssItem>();
        //用来传出去的值
        public static List<RssItem> rssItemList;

        // 构造函数
        public MainPage()
        {
            InitializeComponent();

             //用于本地化 ApplicationBar 的示例代码
            BuildLocalizedApplicationBar();

            isContentLoaded = false;
            this.Loaded += MainPage_Loaded;
            RateAndCommend();
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!isContentLoaded)
            {
                progressBar1.Opacity = 100;
                progressBar1.IsIndeterminate = true;
                getHtmlContent(sender, e);
            }
            MainListBox.SelectionChanged+=MainListBox_SelectionChanged;
            //if ((Application.Current as App).IsTrial)
            //{
            //    MessageBox.Show("试用版！");
            //}
            //else
            //{
            //    MessageBox.Show("正式版！");
            //}
        }

        //动态磁贴
        private void UpdateLiveTiles()
        {
            ShellTile mainTile = ShellTile.ActiveTiles.First();
            FlipTileData TileData = new FlipTileData()
            {
                Title = "",
                BackTitle = "10句话",
                BackContent = mainItemList.First().PlainSummary,
                WideBackContent = mainItemList.First().PlainSummary,
                SmallBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileSmall.png", UriKind.Relative),
                BackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileMedium.png", UriKind.Relative),
                BackBackgroundImage = new Uri("/Assets/Tiles/Back.png", UriKind.Relative),
                WideBackgroundImage = new Uri("/Assets/Tiles/FlipCycleTileLarge.png", UriKind.Relative),
                WideBackBackgroundImage = new Uri("/Assets/Tiles/WideBack.png", UriKind.Relative),
            };
            mainTile.Update(TileData);
        }

        // 处理在 ListBox 中更改的选定内容
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选索引为 -1 (没有选定内容)，则不执行任何操作
            if (MainListBox.SelectedIndex == -1)
                return;

            // 导航到新页面
            NavigationService.Navigate(new Uri("/DetailPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // 将所选索引重置为 -1 (没有选定内容)
            MainListBox.SelectedIndex = -1;
        }

        //******全新获取内容方法********
        //获取网页内容
        private void getHtmlContent(object sender, EventArgs e)
        {
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            WebClient htmlString = new WebClient();
            //htmlString.Encoding = new Gb2312Encoding();
            
            htmlString.DownloadStringCompleted += new DownloadStringCompletedEventHandler(htmlString_DownloadStringCompleted);
            string uriStr = string.Format("http://www.10juhua.com");
            htmlString.DownloadStringAsync(new Uri(uriStr));
        }

        //解析网页内容
        void htmlString_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            isContentLoaded = true;
            progressBar1.IsIndeterminate = false;
            progressBar1.Opacity = 0;

            try
            {
                //string cleanStr = CleanInvalidXmlChars(e.Result);
                HtmlAgilityPack.HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(e.Result);
                

                //获取发表时间
                //var time = doc.DocumentNode.Descendants("div").First(t =>
                //{
                //    var attr = t.Attributes["id"];
                //    if (attr != null && attr.Value == "time")
                //        return true;
                //    return false;
                //}).InnerText;

                //获取主体内容
                var words = doc.DocumentNode.Descendants("div").Where(t =>
                {
                    var attr = t.Attributes["class"];
                    return attr != null && attr.Value == "sjh_ten_width sjh_left_border fl-l";
                }).Select(t =>
                {
                    var a = t.Descendants("a").First();
                    return a.InnerText;
                });
                var ws = words.ToArray();

                //获取分享次数
                var infos = doc.DocumentNode.Descendants("div").Where(t =>
                {
                    var attr = t.Attributes["class"];
                    return attr != null && attr.Value == "sjh_ten_width sjh_left_border fl-l";
                }).Select(t =>
                {
                    var info = t.Descendants("font").First();
                    return info.InnerText;
                });
                var ins = infos.ToArray();

                //先清除所有元素再增加
                mainItemList.Clear();
                for (int i = 0; i < ws.Count(); i++)
                {
                    //mainItemList.Add(new RssItem("第" + (i + 1).ToString() + "句：" , ws[i], DateTime.Now.ToString(), "url"));
                    mainItemList.Add(new RssItem("第" + (i + 1).ToString() + "句：" + ins[i], ws[i], DateTime.Now.ToString(), "url"));
                }

                //主页面列表显示
                MainListBox.ItemsSource = mainItemList;
                rssItemList = mainItemList.ToList();
                //更新动态磁贴
                UpdateLiveTiles();
                //MessageBox.Show(ws.Count().ToString());
            }
            catch (Exception ex)
            {
                ToastPrompt tp = new ToastPrompt { Title = "提示：", Message = "网络异常或文本解析错误，请检查！" };
                tp.Show();
            }

            //
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
        }

        public string CleanInvalidXmlChars(string text)
        {
            string re = @"[^\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF\u4e00-\u9fa5]";
            return System.Text.RegularExpressions.Regex.Replace(text, re, "");
        }


         //用于生成本地化 ApplicationBar 的示例代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            ApplicationBarIconButton refreshAppBar = new ApplicationBarIconButton(new Uri("/icons/refresh.png", UriKind.Relative));
            refreshAppBar.Text = "刷新";
            refreshAppBar.Click += refreshAppBar_Click;
            ApplicationBarIconButton favoriteAppBar = new ApplicationBarIconButton(new Uri("/icons/favorite.png", UriKind.Relative));
            favoriteAppBar.Text = "收藏夹";
            favoriteAppBar.Click += favoriteAppBar_Click;
            ApplicationBar.Buttons.Add(refreshAppBar);
            ApplicationBar.Buttons.Add(favoriteAppBar);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem("给个好评:-)");
            appBarMenuItem1.Click += appBarMenuItem1_Click;
            ApplicationBarMenuItem appBarMenuItem2 = new ApplicationBarMenuItem("应用推荐!");
            appBarMenuItem2.Click += appBarMenuItem2_Click;
            ApplicationBarMenuItem appBarMenuItem4 = new ApplicationBarMenuItem("关于?");
            appBarMenuItem4.Click += appBarMenuItem4_Click;
            ApplicationBar.MenuItems.Add(appBarMenuItem1);
            ApplicationBar.MenuItems.Add(appBarMenuItem2);
            ApplicationBar.MenuItems.Add(appBarMenuItem4);
        }


        //查看收藏夹
        void favoriteAppBar_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/FavoritePage.xaml", UriKind.RelativeOrAbsolute));
        }

        //刷新页面
        void refreshAppBar_Click(object sender, EventArgs e)
        {
            progressBar1.Opacity = 100;
            progressBar1.IsIndeterminate = true;
            getHtmlContent(sender, e);
        }

        //关于
        void appBarMenuItem4_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AboutPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //消息中心
        //void appBarMenuItem3_Click(object sender, EventArgs e)
        //{
        //    Unique.Service.AppService.ShowMessageCenter();
        //}

        //应用推荐
        void appBarMenuItem2_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/RecommendPage.xaml", UriKind.RelativeOrAbsolute));
        }

        //给个好评
        void appBarMenuItem1_Click(object sender, EventArgs e)
        {
            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var file = appStorage.OpenFile("RatedInfo.txt", System.IO.FileMode.OpenOrCreate))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                    {
                        sw.WriteLine("1");
                        sw.WriteLine("0");

                    }
                }
            }
            MarketplaceReviewTask mrt = new MarketplaceReviewTask();
            mrt.Show();
        }

        //启动5次后提示打分，点击确定后以后不再提示，否则每启动5次提示打分
        private void RateAndCommend()
        {
            int isRated = 0;
            int launchTimes = 0;
            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (appStorage.FileExists("RatedInfo.txt"))
                {
                    using (var file = appStorage.OpenFile("RatedInfo.txt", FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            //是否打过分
                            isRated = int.Parse(sr.ReadLine());
                            //启动次数
                            launchTimes = int.Parse(sr.ReadLine());

                        }
                    }
                }
            }
            //已经打过分了
            if (isRated == 1)
            {
                return;
            }
            //没打分，但是启动了五次
            else if (launchTimes == 5)
            {
                MessageBoxResult result = MessageBox.Show("支持一下开发者，给个好评吧！", "提醒", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var file = appStorage.OpenFile("RatedInfo.txt", System.IO.FileMode.OpenOrCreate))
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                            {
                                sw.WriteLine("1");
                                sw.WriteLine("0");

                            }
                        }
                    }
                    MarketplaceReviewTask mrt = new MarketplaceReviewTask();
                    mrt.Show();
                }

                else if (result == MessageBoxResult.Cancel)
                {
                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var file = appStorage.OpenFile("RatedInfo.txt", System.IO.FileMode.OpenOrCreate))
                        {
                            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                            {
                                sw.WriteLine("0");
                                sw.WriteLine("0");

                            }
                        }
                    }
                }
            }
            //没打分，启动次数不到五次
            else
            {
                launchTimes++;
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (var file = appStorage.OpenFile("RatedInfo.txt", System.IO.FileMode.OpenOrCreate))
                    {
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                        {
                            sw.WriteLine("0");
                            sw.WriteLine(launchTimes);

                        }
                    }
                }
            }

        }

        //第三方启动测试
        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    IDictionary<string, string> queryStrings = NavigationContext.QueryString;
        //    if (queryStrings.ContainsKey("Msg")) MessageBox.Show("来自URI关联调用方信息：\n" + queryStrings["Msg"]);
        //    base.OnNavigatedTo(e);
        //}


    }
}