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
    public partial class RecommendPage : PhoneApplicationPage
    {
        public RecommendPage()
        {
            InitializeComponent();
            listBox.ItemsSource = RecommendList.GetRecommendList();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var appItem = listBox.SelectedItem as RecommendItem;
            if (appItem == null || string.IsNullOrEmpty(appItem.Id))
                return;
            var task = new MarketplaceDetailTask { ContentIdentifier = appItem.Id };
            task.Show();
        }



    }
}