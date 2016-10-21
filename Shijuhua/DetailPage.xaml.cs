using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Phone.Tasks;
using Coding4Fun.Toolkit.Controls;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
using CN.SmartMad.Ads.WindowsPhone.WPF;
using WeiboSdk;
using WeiboSdk.PageViews;
using RenrenSDKLibrary;
using MicroMsg.sdk;
using UmengSDK;
using System.Threading.Tasks;
using MSNADSDK.AD;

namespace Shijuhua
{
    public partial class DetailPage : PhoneApplicationPage
    {
        private int currentIndex;
        //记录是否点击了广告
        int isAdClicked;
        RenrenAPI api = App.api;

        public DetailPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();

            currentIndex = 0;
            isAdClicked = 0;

            var listerner = GestureService.GetGestureListener(contentPanel);
            listerner.Flick += new EventHandler<FlickGestureEventArgs>(listerner_Flick);

            //微博开发者密钥
            SdkData.AppKey = "3403556419";
            SdkData.AppSecret = "e7ee5d14e43c4962049812da5d218417";
            // 您app设置的重定向页,必须一致
            SdkData.RedirectUri = "http://weibo.com/jwchen08";

            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (appStorage.FileExists("AdClick.txt"))
                {
                    using (var file = appStorage.OpenFile("AdClick.txt", FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(file))
                        {
                            int year1 = int.Parse(sr.ReadLine());
                            int month1 = int.Parse(sr.ReadLine());
                            int day1 = int.Parse(sr.ReadLine());
                            int isAdClicked1 = int.Parse(sr.ReadLine());
                            DateTime clickedDate = new DateTime(year1, month1, day1);
                            DateTime temp = clickedDate.AddDays(2.0);
                            int n = DateTime.Now.CompareTo(temp);
                            if (n < 0)
                            {
                                isAdClicked = 1;
                                //SmartMad广告
                                //ad1.Visibility = Visibility.Collapsed;
                                
                                //OpenXLive广告
                                AdControl.IsCloseIconEnabled = true;
                            }
                        }
                    }
                }
            }
            
            //友盟在线参数
            //10句话509290d452701518ce0002b7
            //十句话50f7b8765270150d9200000b
            UmengAnalytics.UpdateOnlineParamCompleted += UmengAnalytics_UpdateOnlineParamCompleted;
            UmengAnalytics.UpdateOnlineParamAsync("509290d452701518ce0002b7");

            //requestMSNAD();
        }

        //MSN广告测试用
        //string defaultAppid = "100000";
        //string defaultAdId = "100000";
        //string defaultSerectKey = "1d89bd7887494e39ad5b0d606f1b7360";
        //10句话WP81
        string defaultAppid = "145556";
        string defaultAdId = "191472";
        string defaultSerectKey = "042dd690432144fea8a772b716de2948";





        //获取友盟在线参数，每次仅获取一个
        void UmengAnalytics_UpdateOnlineParamCompleted(int statusCode, OnlineParamEventArgs e)
        {
            //statusCode返回状态码，0 标示有更新 
            //e OnlineConfigEventArgs 实例，包涵在线参数信息. 
            //e.Result 返回Dictionary对应线上配置的K-V格式在线参数信息
            if (statusCode == 0 && e.Result.Count > 0)
            {
                string ReadedInfo = "";
                //首先读取已查看过的消息列表
                using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStorage.FileExists("UmengInfo.txt"))
                    {
                        using (var file = appStorage.OpenFile("UmengInfo.txt", FileMode.Open))
                        {
                            using (StreamReader sr = new StreamReader(file))
                            {
                                //读取全部信息
                                ReadedInfo = sr.ReadToEnd();

                            }
                        }
                    }
                }

                //判断服务器上的消息是否已读
                for (int i = 0; i < e.Result.Count; i++)
                {
                    var item = e.Result.ElementAt(i);
                    //存在未读消息
                    if (!ReadedInfo.Contains(item.Key + item.Value))
                    {
                        this.Dispatcher.BeginInvoke(delegate()
                        {
                            MessageBox.Show(item.Value, item.Key, MessageBoxButton.OK);

                        });
                        //将该消息加进已读列表
                        using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                        {
                            using (var file = appStorage.OpenFile("UmengInfo.txt", System.IO.FileMode.Append))
                            {
                                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                                {
                                    //WriteLine其实是在字符串后面加上\r\n
                                    sw.WriteLine(item.Key + item.Value);

                                }
                            }
                        }
                        //此次不再获取新消息
                        break;
                    }
                }
                //StringBuilder param = new StringBuilder();
                //foreach (var item in e.Result)
                //{
                //    param.AppendLine(string.Format("{0}:{1}", item.Key, item.Value));
                //}
                //string s = param.ToString().Split('\n')[0];
                //string s = param.ToString();

            }
            UmengAnalytics.UpdateOnlineParamCompleted -= UmengAnalytics_UpdateOnlineParamCompleted;
        }


        //点击了SmartMad广告
        //private void ad1_AdBannerClicked(object sender, EventArgs e)
        //{
        //    isAdClicked = 1;
        //    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
        //    {
        //        using (var file = appStorage.OpenFile("AdClick.txt", System.IO.FileMode.OpenOrCreate))
        //        {
        //            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
        //            {
        //                sw.WriteLine(DateTime.Now.Year);
        //                sw.WriteLine(DateTime.Now.Month);
        //                sw.WriteLine(DateTime.Now.Day);
        //                sw.WriteLine(isAdClicked);
        //            }
        //        }
        //    }
        //}

        //点击了OpenXLive广告
        private void AdControl_OnIsEngagedChanged(object sender, EventArgs e)
        {
            AdControl.IsCloseIconEnabled = true;
            isAdClicked = 1;
            using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (var file = appStorage.OpenFile("AdClick.txt", System.IO.FileMode.OpenOrCreate))
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(file))
                    {
                        sw.WriteLine(DateTime.Now.Year);
                        sw.WriteLine(DateTime.Now.Month);
                        sw.WriteLine(DateTime.Now.Day);
                        sw.WriteLine(isAdClicked);
                    }
                }
            }
        }

        void listerner_Flick(object sender, FlickGestureEventArgs e)
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
                using (var file = appStorage.OpenFile("DetailPageCurrentIndex.txt", System.IO.FileMode.Create))
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
                RssItem item = MainPage.rssItemList[index];
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
                    if (appStorage.FileExists("DetailPageCurrentIndex.txt"))
                    {
                        using (var file = appStorage.OpenFile("DetailPageCurrentIndex.txt", FileMode.Open))
                        {
                            using (StreamReader sr = new StreamReader(file))
                            {
                                currentIndex = int.Parse(sr.ReadLine());
                                RssItem item = MainPage.rssItemList[currentIndex];
                                TimeTextBlock.Text = item.PublishedDate;
                                ContentTextBlock.Text = item.PlainSummary;

                            }
                        }
                    }
                }
            }
        }


        private void beforeButton_Click(object sender, EventArgs e)
        {
            //已经点击了广告
            //if (isAdClicked == 1)
            //{
            //    //SmartMad广告
            //    if (ad1.Visibility == Visibility.Visible)
            //    {
            //        ad1.Visibility = Visibility.Collapsed;
            //    }
            //}

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
                    RssItem item = MainPage.rssItemList[currentIndex];
                    TimeTextBlock.Text = item.PublishedDate;
                    ContentTextBlock.Text = item.PlainSummary;

                    App.wordsToShare = item.PlainSummary;
                }, false);
            }
        }

        //添加到收藏
        private void favoriteButton_Click(object sender, EventArgs e)
        {
            //已经点击了广告
            //if (isAdClicked == 1)
            //{
            //    //SmartMad广告
            //    if (ad1.Visibility == Visibility.Visible)
            //    {
            //        ad1.Visibility = Visibility.Collapsed;
            //    }
            //}

            MainPage.rssItemList[currentIndex].Title = "每天十句话，句句都精彩";
            //App.favoriteItemList.Add(MainPage.rssItemList[currentIndex]);
            App.favoriteItemList.Insert(0, MainPage.rssItemList[currentIndex]);
            ToastPrompt toast = new ToastPrompt
            {
                Message = "收藏成功！",
            };
            toast.Show();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            //已经点击了广告
            //if (isAdClicked == 1)
            //{
            //    //SmartMad广告
            //    if (ad1.Visibility == Visibility.Visible)
            //    {
            //        ad1.Visibility = Visibility.Collapsed;
            //    }
            //}

            if (currentIndex == MainPage.rssItemList.Count - 1)//如果已经是最后一个了
            {
                ToastPrompt toast = new ToastPrompt
                {
                    Message = "今天的木有了，记得明天再来看哦！",
                };
                toast.Show();
            }
            else
            {
                ChangeDetail(() =>
                {
                    currentIndex++;
                    RssItem item = MainPage.rssItemList[currentIndex];
                    TimeTextBlock.Text = item.PublishedDate;
                    ContentTextBlock.Text = item.PlainSummary;

                    App.wordsToShare = item.PlainSummary;
                }, true);
            }
        }

        //请求MSN广告
        //private void requestMSNAD()
        //{
        //    AdView adView = new AdView();
        //    adView.Appid = defaultAppid;
        //    adView.SecretKey = defaultSerectKey;
        //    adView.SizeForAd = AdSize.Large;
        //    adView.Adid = defaultAdId;
        //    adView.TelCapability = true;
        //    adView.MouseLeftButtonUp += adView_MouseLeftButtonUp;
        //    adArea.Children.Clear();
        //    adArea.Children.Add(adView);
        //    adView.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        //}

        //点击了MSN广告
        //void adView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    adArea.Children.Clear();
        //}

        //复制到剪贴板
        private void copyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(MainPage.rssItemList[currentIndex].PlainSummary);
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



        //本地化 ApplicationBar
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            ApplicationBarIconButton beforeAppBar = new ApplicationBarIconButton(new Uri("/icons/back.png", UriKind.Relative));
            beforeAppBar.Text = "上一条";
            beforeAppBar.Click += beforeButton_Click;
            ApplicationBarIconButton addAppBar = new ApplicationBarIconButton(new Uri("/icons/addto.png", UriKind.Relative));
            addAppBar.Text = "收藏";
            addAppBar.Click += favoriteButton_Click;
            ApplicationBarIconButton nextAppBar = new ApplicationBarIconButton(new Uri("/icons/next.png", UriKind.Relative));
            nextAppBar.Text = "下一条";
            nextAppBar.Click += nextButton_Click;

            ApplicationBar.Buttons.Add(beforeAppBar);
            ApplicationBar.Buttons.Add(addAppBar);
            ApplicationBar.Buttons.Add(nextAppBar);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem1 = new ApplicationBarMenuItem("复制");
            appBarMenuItem1.Click += copyButton_Click;
            ApplicationBarMenuItem appBarMenuItem2 = new ApplicationBarMenuItem("微博分享");
            appBarMenuItem2.Click += weiboButton_Click;
            ApplicationBarMenuItem weixinButton = new ApplicationBarMenuItem("微信分享:-)");
            weixinButton.Click += weixinButton_Click;
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

        //微博分享
        void weiboButton_Click(object sender, EventArgs e)
        {
            //Unique.Share.ShareService.Share("10句话分享", ContentTextBlock.Text);
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

        private void ad1_AdBannerReceived(object sender, EventArgs e)
        {
            //ToastPrompt tp = new ToastPrompt { Message = "接收成功！" };
            //tp.Show();
        }

        private void ad1_AdBannerReceiveFailed(object sender, SMAdEventCode e)
        {
            //ToastPrompt tp = new ToastPrompt { Message = "接收失败！" };
            //tp.Show();
        }




    }
}