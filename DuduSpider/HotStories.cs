using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    class HotStories
    {
        const int API = 7;
        
        public static List<Story> fetchHotStories()
        {
            List<Cell> cellList = new List<Cell>();
            List<Story> storyList = new List<Story>();
            string webSource = HttpRequest.DownloadString("https://news-at.zhihu.com/api/7/explore/stories/hot");
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
            foreach(Match m in collection)
            {
                cellList.Add(new Cell(m.Groups["url"].Value, m.Groups["avatar"].Value, m.Groups["title"].Value, Convert.ToInt32(m.Groups["views"].Value)));
            }
            int i = 0;
            foreach(Cell c in cellList)
            {
                Console.Write("\r\n" + DateTime.Now.TimeOfDay.ToString() + "[" + i++.ToString() + "]" + c.Title);
                storyList.Add(new Story(HttpRequest.DownloadString(string.Format(@"https://news-at.zhihu.com/api/{0}/story/{1}", API, c.Id))));
            }
            return storyList;
        }

    }
}
