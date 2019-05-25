using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotificationForToasts
{
    public class NewsModel
    {
        public string url { get; set; }
        public string hm { get; set; }
        public string datekey { get; set; }
        public string newcontent { get; set; }
        public string indexTitle { get; set; }
        public bool important { get; set; }
        public string id { get; set; }
        public bool hasimage { get; set; }
        public bool hasvideo { get; set; }
        public bool istop { get; set; }
        public bool hasrelated { get; set; }
        public bool hasvote { get; set; }
        public DateTime createTime { get; set; }
    }

    public class GetData
    {
        private string _getResult;
        private List<NewsModel> _getList;

        /// <summary>
        /// 获取最新的财经新闻
        /// </summary>
        /// <returns></returns>
        public List<NewsModel> UpdateNews()
        {
            EventArgs waiter;
            List<NewsModel> getnews = new List<NewsModel>();
            using (WebClient wc = new WebClient())
            {
                //type=0: 看全部， type=1 只看红标
                //https://www.yicai.com/api/ajax/getbrieflist?page=1&pagesize=20&type=0
                string url = "https://www.yicai.com/api/ajax/getbrieflist?page=1&pagesize=10&type=0" + Guid.NewGuid().ToString();
                //string url = "https://www.yicai.com/api/ajax/getbrieflist?page=1&pagesize=10&type=1&randomid=" + Guid.NewGuid().ToString();
                _getResult = Encoding.UTF8.GetString(wc.DownloadData(new Uri(url)));
                wc.Dispose();
            }

            getnews = JsonConvert.DeserializeObject<List<NewsModel>>(_getResult);
            foreach(NewsModel item in getnews)
            {
                item.createTime = DateTime.Parse(item.datekey.Replace(".", "-") + " " + item.hm);
            }
            _getList = getnews;
            return getnews;
        }


        /// <summary>
        /// 获取最新一条财经新闻(当前不更新原始数据，需要更新原始数据，调用UpdateNews方法)
        /// </summary>
        /// <returns></returns>
        public NewsModel GetTop1News()
        {
            if (_getList == null)
            {
                UpdateNews();
            }

            var resore = _getList.OrderByDescending(n => n.createTime).ToList<NewsModel>();
            if (resore != null && resore.Count > 0)
            {
                NewsModel item = resore[0];

                #region 固定置顶标题
                // 如果最新条目不是红标，就将红标的题目加上去。
                //if (!item.istop)
                //{
                //    var topitem = _getList.OrderByDescending(n => n.istop).ToList<NewsModel>();
                //    if (topitem != null && topitem[0].istop)
                //    {
                //        item.indexTitle = topitem[0].indexTitle;
                //    }
                //}
                #endregion

                //将内部部分包含标题去除。
                item.newcontent = item.newcontent.Replace(item.indexTitle, "");
                //去除HTML标识符
                Regex reg = new Regex(@"<\s*[^>]*>([\s\S]+?)/>", RegexOptions.IgnoreCase);
                item.indexTitle = reg.Replace(item.indexTitle, "");
                item.newcontent = reg.Replace(item.newcontent, "");

                return _getList != null && _getList.Count > 0 ? item : null;
            }
            else
            { 
                return null;
            }
        }
    }
}
