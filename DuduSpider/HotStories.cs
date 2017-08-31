using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    class HotStories
    {
        const int API = 7;
        /// <summary>
        /// 抓取并保存（.html）最新的热门文章
        /// </summary>
        /// <param name="clist">之前抓取时保存的（首页流[去重]和热门流） List Cell（以不抓取这部分），若初始化则抓取保存全部</param>
        /// <returns>仅包括这次抓取的内容{ List Story, List Cell }</returns>
        public static object[] FetchHotStories(List<Cell> clist)
        {
            List<Story> newSList = new List<Story>();//这次需要抓取的
            List<Cell> newCList = fetchCellListOnly();
            newCList = newCList.Except(clist).ToList();//在抓取到的 cell 中删去抓取过的内容，余下的就是这次需要抓取的
            return new object[] { fetch(newCList), newCList };
        }
        /// <summary>
        /// 抓取并保存（.html）提供的 List 列表的消息，内部方法
        /// </summary>
        /// <param name="clist">抓取提供的 List</param>
        /// <returns></returns>
        private static List<Story> fetch(List<Cell> clist)
        {
            List<Cell> cellList = clist;
            List<Story> storyList = new List<Story>();
            
            int i = 0;
            foreach(Cell c in cellList)
            {
                Console.Write("\r\n" + DateTime.Now.TimeOfDay.ToString() + "[" + i++.ToString() + "]" + c.Title);
                storyList.Add(new Story(HttpRequest.DownloadString(string.Format(@"https://news-at.zhihu.com/api/{0}/story/{1}", API, c.Id))));
            }

            List<Story> notEmptyList = new List<Story>();
            foreach (Story s in storyList)
            {
                if (s.Manifest[0].Count > 0 && s.Manifest[0][0] != "")//为空的就是前面获取时直接 return 的文章，不进入制作电子书的环节
                    notEmptyList.Add(s);
            }

            return notEmptyList;
        }
        /// <summary>
        /// 仅抓取热门文章的 Cell List，内部方法
        /// </summary>
        /// <returns></returns>
        private static List<Cell> fetchCellListOnly()
        {
            List<Cell> cellList = new List<Cell>();
            string webSource = HttpRequest.DownloadString(string.Format(@"https://news-at.zhihu.com/api/{0}/explore/stories/hot", API));
            Regex r = new Regex(@"<a class=""article-cell"" href=""(?<url>.*?)"">\n\n<img class=""avatar"" src=""(?<avatar>.*?)"">\n<span class=""title"">(?<title>.*?)</span>\n<span class=""meta"">\n<i class=""icon""></i>\n<i>(?<views>\d*?)</i>\n</span>\n</a>", RegexOptions.CultureInvariant);
            /*<a class="article-cell" href="(?<url>.*?)">
             * 
             * <img class="avatar" src="(?<avatar>.*?)">
             * <span class="title">(?<title>.*?)</span>
             * <span class="meta">
             * <i class="icon"></i>
             * <i>(?<views>\d*?)</i>
             * </span>
             * </a>
             */
            MatchCollection collection = r.Matches(webSource);
            foreach (Match m in collection)
            {
                cellList.Add(new Cell(m.Groups["url"].Value, m.Groups["avatar"].Value, m.Groups["title"].Value, Convert.ToInt32(m.Groups["views"].Value)));
            }
            return cellList;
        }
    }
}
