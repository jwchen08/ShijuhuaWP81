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
using System.Windows.Media.Animation;
using System.Diagnostics;
using Coding4Fun.Toolkit.Controls;
using System.IO.IsolatedStorage;
using System.IO;
using WeiboSdk;
using WeiboSdk.PageViews;
using RenrenSDKLibrary;
using MicroMsg.sdk;

namespace Shijuhua
{
    public partial class FavoriteDetailPage : PhoneApplicationPage
    {
        private int currentIndex;
        RenrenAPI api = App.api;

        public FavoriteDetailPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            currentIndex = 0;
            var listener = GestureService.GetGestureListener(contentPanel);
            listener.Flick += new EventHandler<FlickGestureEventArgs>(listener_Flick);

            //微博开发者密钥
            SdkData.AppKey = "3403556419";
            SdkData.AppSecret = "e7ee5d14e43c4962049812da5d218417";
            // 您app设置的重定向页,必须一致
            SdkData.RedirectUri = "http://weibo.com/jwchen08";
        }

        void listener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Vertical)
                return;
            if (e.HorizontalVelocity < 0)
            {
                nextButton_Click(null, null);
            }
            else
            {
                beforeButton_Click(null, null);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var file = appStorage.OpenFile("FavoriteDetailPageCurrentIndex.txt", System.IO.FileMode.Create))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                    {
                        sw.WriteLine(currentIndex.ToString());
                    }
                }
            }
        }

        // 导航页面以将数据上下文设置为列表中的所选项时
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //如果是从主页面导航过来的
            if (NavigationContext.QueryString.ContainsKey("selectedItem"))
            {
                string selectedIndex = NavigationContext.QueryString["selectedItem"];
                int index = int.Parse(selectedIndex);
                RssItem item = App.favoriteItemList[index];
                TimeTextBlock.Text = item.PublishedDate;
                ContentTextBlock.Text = item.PlainSummary;
                App.wordsToShare = item.PlainSummary;

                currentIndex = index;
                NavigationContext.QueryString.Remove("selectedItem");
            }
            else//从其余页面返回回来的
            {
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStorage.FileExists("FavoriteDetailPageCurrentIndex.txt"))
                    {
                        using (var file = appStorage.OpenFile("FavoriteDetailPageCurrentIndex.txt", FileMode.Open))
                        {
                            using (StreamReader sr = new StreamReader(file))
                            {
                                currentIndex = int.Parse(sr.ReadLine());
                                RssItem item = App.favoriteItemList[currentIndex];
                                TimeTextBlock.Text = item.PublishedDate;
                                ContentTextBlock.Text = item.PlainSummary;

                            }
                        }
                    }
                }
            }
        }

        //上一条收藏
        private void beforeButton_Click(object sender, EventArgs e)
        {
            if (currentIndex == 0)//如果已经是第一个了
            {
                ToastPrompt toast = new ToastPrompt
                {
                    Message = "已经是第一条了！",
                };
                toast.Show();
            }
            else
            {
                ChangeDetail(() =>
                {
                    currentIndex--;
                    RssItem item = App.favoriteItemList[currentIndex];
                    TimeTextBlock.Text = item.PublishedDate;
                    ContentTextBlock.Text = item.PlainSummary;

                    App.wordsToShare = item.PlainSummary;
                }, false);
            }

        }

        //取消收藏
        private void deleteButton_Click(object sender, EventArgs e)
        {
            App.favoriteItemList.RemoveAt(currentIndex);
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        //下一条收藏
        private void nextButton_Click(object sender, EventArgs e)
        {
            if (currentIndex == App.favoriteItemList.Count - 1)//如果已经是最后一个了
            {
                ToastPrompt toast = new ToastPrompt
                {
                    Message = "已经是最后一条了！",
                };
                toast.Show();
            }
            else
            {
                ChangeDetail(() =>
                {
                    currentIndex++;
                    RssItem item = App.favoriteItemList[currentIndex];
                    TimeTextBlock.Text = item.PublishedDate;
                    ContentTextBlock.Text = item.PlainSummary;

                    App.wordsToShare = item.PlainSummary;
                }, true);
            }

        }

        //本地化 ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            ApplicationBarIconButton beforeAppBar = new ApplicationBarIconButton(new Uri("/icons/back.png", UriKind.Relative));
            beforeAppBar.Text = "上一条";
            beforeAppBar.Click += beforeButton_Click;
            ApplicationBarIconButton cancelAppBar = new ApplicationBarIconButton(new Uri("/icons/delete.png", UriKind.Relative));
            cancelAppBar.Text = "取消收藏";
            cancelAppBar.Click += deleteButton_Click;
            ApplicationBarIconButton nextAppBar = new ApplicationBarIconButton(new Uri("/icons/next.png", UriKind.Relative));
            nextAppBar.Text = "下一条";
            nextAppBar.Click += nextButton_Click;

            ApplicationBar.Buttons.Add(beforeAppBar);
            ApplicationBar.Buttons.Add(cancelAppBar);
            ApplicationBar.Buttons.Add(nextAppBar);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem("复制");
            appBarMenuItem1.Click += copyButton_Click;
            ApplicationBarMenuItem appBarMenuItem2 = new ApplicationBarMenuItem("微博分享");
            appBarMenuItem2.Click += weiboButton_Click;
            ApplicationBarMenuItem weixinButton = new ApplicationBarMenuItem("微信分享:-)");
            weixinButton.Click+=weixinButton_Click;
            ApplicationBarMenuItem appBarMenuItem3 = new ApplicationBarMenuItem("人人分享");
            appBarMenuItem3.Click += renrenButton_Click;
            ApplicationBarMenuItem appBarMenuItem4 = new ApplicationBarMenuItem("短信分享");
            appBarMenuItem4.Click += textButton_Click;
            ApplicationBarMenuItem appBarMenuItem5 = new ApplicationBarMenuItem("邮件分享");
            appBarMenuItem5.Click += emailButton_Click;

            ApplicationBar.MenuItems.Add(appBarMenuItem1);
            ApplicationBar.MenuItems.Add(appBarMenuItem2);
            ApplicationBar.MenuItems.Add(weixinButton);
            ApplicationBar.MenuItems.Add(appBarMenuItem3);
            ApplicationBar.MenuItems.Add(appBarMenuItem4);
            ApplicationBar.MenuItems.Add(appBarMenuItem5);
        }

        //微信分享
        void weixinButton_Click(object sender, EventArgs e)
        {
            try
            {
                int scene = SendMessageToWX.Req.WXSceneChooseByUser; //发给微信朋友
                WXTextMessage message = new WXTextMessage();
                message.Title = "10句话分享";
                message.Text = ContentTextBlock.Text;
                SendMessageToWX.Req req = new SendMessageToWX.Req(message, scene);
                IWXAPI api = WXAPIFactory.CreateWXAPI("wxa2a3bf2a146aacb5");
                api.SendReq(req);
            }
            catch (WXException ex)
            {
                MessageBox.Show("分享出错，请检查网络！");
            }
        }

        //复制到剪贴板
        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(ContentTextBlock.Text);
            ToastPrompt toast = new ToastPrompt
            {
                Message = "已复制到剪贴板！",
            };
            toast.Show();

        }

        //邮件分享
        private void emailButton_Click(object sender, EventArgs e)
        {
            EmailComposeTask ect = new EmailComposeTask();
            ect.Subject = "10句话分享";
            ect.Body = ContentTextBlock.Text;
            ect.Show();
        }

        //短信分享
        private void textButton_Click(object sender, EventArgs e)
        {
            SmsComposeTask sct = new SmsComposeTask();
            sct.Body = ContentTextBlock.Text;
            sct.Show();
        }

        //微博分享
        void weiboButton_Click(object sender, EventArgs e)
        {
            //如果不存在AppKey
            if (string.IsNullOrEmpty(SdkData.AppKey) || string.IsNullOrEmpty(SdkData.AppSecret) || string.IsNullOrEmpty(SdkData.RedirectUri))
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("请在构造函数中设置自己的appkey、appkeysecret、RedirectUri.");
                });
                return;
            }
            //如果不存在AccessToken，则进行授权
            if (string.IsNullOrEmpty(App.AccessToken))
            {
                AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
                AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);
                //其它通知事件...

                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml"
                        , UriKind.Relative));
                });
            }
            //如果存在AccessToken，则尝试发布
            else
            {
                SdkShare sdkShare = new SdkShare
                {
                    //设置OAuth2.0的access_token
                    AccessToken = App.AccessToken,
                    //AccessTokenSecret = App.AccessTokenSecret,
                    //PicturePath = "TempJPEG.jpg",
                    Message = ContentTextBlock.Text

                };

                sdkShare.Completed = new EventHandler<SendCompletedEventArgs>(ShareCompleted);
                sdkShare.Show();
            }
        }

        //微博授权完成后回调
        private void VerifyBack(bool isSucess, SdkAuthError errCode, SdkAuth2Res response)
        {
            if (errCode.errCode == SdkErrCode.SUCCESS)
            {
                if (null != response)
                {
                    App.AccessToken = response.accesssToken;
                    App.RefleshToken = response.refleshToken;
                }

                //授权成功
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    //NavigationService.Navigate(new Uri("/PageViews/SampleTimeline.xaml",UriKind.Relative));
                    SdkShare sdkShare = new SdkShare
                    {
                        //设置OAuth2.0的access_token
                        AccessToken = App.AccessToken,
                        //AccessTokenSecret = App.AccessTokenSecret,
                        //PicturePath = "TempJPEG.jpg",
                        Message = ContentTextBlock.Text

                    };

                    sdkShare.Completed = new EventHandler<SendCompletedEventArgs>(ShareCompleted);
                    sdkShare.Show();

                });
            }
            else if (errCode.errCode == SdkErrCode.NET_UNUSUAL)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("检查网络");
                });
            }
            else if (errCode.errCode == SdkErrCode.SERVER_ERR)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("服务器返回错误，错误代码:" + errCode.specificCode);
                });
            }
            else
                Debug.WriteLine("Other Err.");

        }

        void ShareCompleted(object sender, SendCompletedEventArgs e)
        {
            if (e.IsSendSuccess)
                MessageBox.Show("发送成功！");
            else
            {

                AuthenticationView.OAuth2VerifyCompleted = (e1, e2, e3) => VerifyBack(e1, e2, e3);
                AuthenticationView.OBrowserCancelled = new EventHandler(cancleEvent);
                NavigationService.Navigate(new Uri("/WeiboSdk;component/PageViews/AuthenticationView.xaml", UriKind.Relative));

            }
        }


        private void cancleEvent(object sender, EventArgs e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                NavigationService.GoBack();
            });
        }

        //人人分享
        void renrenButton_Click(object sender, EventArgs e)
        {
            List<string> scope = new List<string> { "status_update", "publish_blog" };
            if (api.IsAccessTokenValid())
            {
                NavigationService.Navigate(new Uri("/RenrenSharePage.xaml", UriKind.Relative));
            }
            else
            {
                api.Login(this, scope, renren_LoginCompletedHandler);
            }
        }

        //人人授权完成回调函数
        public void renren_LoginCompletedHandler(object sender, LoginCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                NavigationService.Navigate(new Uri("/RenrenSharePage.xaml", UriKind.Relative));
            }
            else
                MessageBox.Show(e.Error.Message);
        }



        #region 切换笔记时的动画效果
        private Storyboard _showNoteSb;
        private Storyboard _hideNoteSb;
        private Action _changeNoteAction;
        private void ChangeDetail(Action action, bool isToNext)
        {
            _changeNoteAction = action;
            if (_hideNoteSb == null)
            {
                _hideNoteSb = Resources["HideDetailStoryboard"] as Storyboard;
                if (_hideNoteSb == null)
                {
                    _changeNoteAction();
                    return;
                }
                _hideNoteSb.Completed += (sender, e) =>
                {
                    _changeNoteAction();
                    _showNoteSb.Begin();
                };
            }
            if (_showNoteSb == null)
                _showNoteSb = Resources["ShowDetailStoryboard"] as Storyboard;
            try
            {
                Debug.Assert(_showNoteSb != null);
                var frames = _showNoteSb.Children[2] as DoubleAnimationUsingKeyFrames;
                Debug.Assert(frames != null);
                frames.KeyFrames[0].Value = isToNext ? 50 : -50;
                frames = _showNoteSb.Children[3] as DoubleAnimationUsingKeyFrames;
                Debug.Assert(frames != null);
                frames.KeyFrames[0].Value = isToNext ? 100 : -100;

                frames = _hideNoteSb.Children[2] as DoubleAnimationUsingKeyFrames;
                Debug.Assert(frames != null);
                frames.KeyFrames[1].Value = isToNext ? -50 : 50;
                frames = _hideNoteSb.Children[3] as DoubleAnimationUsingKeyFrames;
                Debug.Assert(frames != null);
                frames.KeyFrames[1].Value = isToNext ? -100 : 100;
            }
            catch
            {
                action();
                return;
            }
            _hideNoteSb.Begin();
        }
        #endregion

    }
}