using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ludoux.DuduSpider
{
    class Cell
    {
        CellJson cellJson = new CellJson();
        public int Id { get => cellJson._id; }//热门 format 后得到，首页直接填
        public string Url { get => cellJson._url; }//（仅热门！）circlely://story/9587569 后面数字为 Story 类的 ID
        public string Avatar { get => cellJson._avatar; }//（仅热门！）相对（/img/app/default_story_thumbnail.png）←这个为知乎内容的图] 或绝对（https://pic3.zhimg.com/v2-38d2a9711e5b00beb5d4112c50bf5ab6.jpg）
        public string Title { get => cellJson._title; }//标题
        public int Views { get => cellJson._views; }//（仅热门！）点击量
        //首页消息流只填 id（可以直接获得）和 time，title
        public int Time { get => cellJson._time; }//（仅首页！）Unix 时间戳
        internal class CellJson
        {
            [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
            internal int _id;
            [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
            internal string _url;
            [JsonProperty("avatar", NullValueHandling = NullValueHandling.Ignore)]
            internal string _avatar;
            [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
            internal string _title;
            [JsonProperty("views", NullValueHandling = NullValueHandling.Ignore)]
            internal int _views;
            [JsonProperty("time", NullValueHandling = NullValueHandling.Ignore)]
            internal int _time;
        }
        

        /// <summary>
        /// 首页流初始化
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        public Cell(int time, int id, string title)
        {
            cellJson._time = time;
            cellJson._id = id;
            cellJson._title = title;
        }
        /// <summary>
        /// 热门文章初始化
        /// </summary>
        /// <param name="url"></param>
        /// <param name="avatar"></param>
        /// <param name="title"></param>
        /// <param name="views"></param>
        /// <param name="relativePath"></param>
        public Cell(string url, string avatar, string title, int views, string relativePath = "https://news-at.zhihu.com")
        {
            cellJson._url = url;
            cellJson._avatar = avatar;
            cellJson._title = title;
            cellJson._views = views;
            format(relativePath);
        }
        /// <summary>
        /// 离线初始化
        /// </summary>
        /// <param name="json"></param>
        public Cell(string json)
        {
            cellJson = JsonConvert.DeserializeObject<CellJson>(json);
        }
        /// <summary>
        /// （仅热门！）avatar 相对转绝对 URL ，填充 id
        /// </summary>
        /// <param name="relativePath">"https://news-at.zhihu.com"或其他</param>
        private void format(string relativePath)
        {
            if (cellJson._avatar.StartsWith('/'))
                cellJson._avatar = relativePath + cellJson._avatar;//相对路径
            cellJson._id = Convert.ToInt32(Regex.Match(cellJson._url, @"(?<=story/)\d+$").Value);
        }
        public override bool Equals(object obj)
        {
            if (Id == ((Cell)obj).Id)
                return true;
            else
                return false;
        }
        public override int GetHashCode()
        {
            return Id;
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(cellJson);
        }
        
    }
}
