using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    class HomeTimeline
    {
        const int API = 7;
        public static List<Story> fetchHomeTimeline(string authorization)
        {
            List<Cell> cellList = new List<Cell>();
            List<Story> storyList = new List<Story>();
            string webSource = HttpRequest.DownloadString(string.Format(@"https://news-at.zhihu.com/api/{0}/home_timeline", API),authorization:authorization);
            Regex r = new Regex(@"""title"":""(?<title>.*?)"",.*?""time"":(?<time>\d*?),[^{]*?id"":(?<id>\d*?)(}|,[^{]*?)");
            MatchCollection collection = r.Matches(webSource);
            foreach (Match m in collection)
            {
                cellList.Add(new Cell(Convert.ToInt32(m.Groups["time"].Value), Convert.ToInt32(m.Groups["id"].Value), m.Groups["title"].Value));
            }

            int i = 0;
            foreach (Cell c in cellList)
            {
                Console.Write("\r\n" + DateTime.Now.TimeOfDay.ToString() + "[" + i++.ToString() + "]" + c.Title);
                storyList.Add(new Story(HttpRequest.DownloadString(string.Format(@"https://news-at.zhihu.com/api/{0}/story/{1}", API, c.Id))));
            }

            List<Story> notEmptyList = new List<Story>();
            foreach (Story s in storyList)
            {
                if (((string[])s.Manifest[0])[0] != "")//为空的就是前面获取时直接 return 的文章，不进入制作电子书的环节
                    notEmptyList.Add(s);
            }
            return notEmptyList;
        }
    }
}
