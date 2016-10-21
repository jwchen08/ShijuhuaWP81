using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Shijuhua
{
    public class RecommendItem
    {
        public string Name { get; set; }
        public string Icon { get; set; }
        public string Remark { get; set; }
        public string Id { get; set; }
    }

    public class RecommendList : ObservableCollection<RecommendItem>
    {
        private RecommendList() { }
        public static RecommendList GetRecommendList()
        {
            var list = new RecommendList();
            list.Add(new RecommendItem
            {
                Name = "10幅图",
                Icon = "/Recommend/10futu.png",
                Remark = "每天十幅图，gif动不停！",
                Id = "9f48bf62-0b12-48cb-bea5-9f0c8102f700"
            });
            list.Add(new RecommendItem
            {
                Name = "结构小助手",
                Icon = "/Recommend/jiegou.png",
                Remark = "掌上结构利器，土木工程师必备软件，亲切到令人泪奔！",
                Id = "6d3dd299-b0ee-4656-8214-6b4e6fb06412"
            });
            list.Add(new RecommendItem
            {
                Name = "X-Note",
                Icon = "/Recommend/xxjishi.png",
                Remark = "一款酷炫十足的记事本应用，亮瞎你的双眼，界面简洁，功能强大!",
                Id = "13eaa951-787b-4795-a037-45f5eb3087d0"
            });
            list.Add(new RecommendItem
            {
                Name = "语音备忘录",
                Icon = "/Recommend/yuyin.png",
                Remark = "非常实用的语音记事本，直接说话就能保存，方便快捷，随时查看！",
                Id = "bc08a447-6f76-45ab-8db8-6e957b3150f4"
            });
            list.Add(new RecommendItem
            {
                Name = "二维码Maker",
                Icon = "/Recommend/erweimaMaker.png",
                Remark = "离线快速生成各种信息的二维码，同时支持将二维码保存到图片库！",
                Id = "0a4230e3-971c-4e7e-b4ba-416eb4b5cb1b"
            });

            return list;
        }

    }
}
