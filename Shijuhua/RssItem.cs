using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Shijuhua
{
    /// <summary>    
    /// RSS对象类    
    /// </summary>
    public class RssItem
    {
        /// <summary>        
        /// 初始化一个RSS目录        
        /// </summary>        
        /// <param name="title">标题</param>        
        /// <param name="summary">内容</param>        
        /// <param name="publishedDate">发表时间</param>        
        /// <param name="url">文章地址</param>        

        public RssItem(string title, string summary, string publishedDate, string url)
        {
            Title = title;
            Summary = summary;
            PublishedDate = publishedDate;
            Url = url;
            //解析html            
            PlainSummary = HttpUtility.HtmlDecode(Regex.Replace(summary, "<[^>]+?>", ""));
        }
        //标题        
        public string Title { get; set; }
        //内容        
        public string Summary { get; set; }
        //发表时间        
        public string PublishedDate { get; set; }
        //文章地址        
        public string Url { get; set; }
        //解析的文本内容        
        public string PlainSummary { get; set; }
    }

}
