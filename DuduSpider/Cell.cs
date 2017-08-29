using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    class Cell
    {
        public int Id { get => _id; }
        public string Url { get => _url; }
        public string Avatar { get => _avatar; }
        public string Title { get => _title; }
        public int Views { get => _views; }
        public int Time { get => _time; }

        private int _id;//热门 format 后得到，首页直接填
        private string _url;//（仅热门！）circlely://story/9587569 后面数字为 Story 类的 ID
        private string _avatar;//（仅热门！）相对（/img/app/default_story_thumbnail.png）←这个为知乎内容的图] 或绝对（https://pic3.zhimg.com/v2-38d2a9711e5b00beb5d4112c50bf5ab6.jpg）
        private string _title;//标题
        private int _views;//（仅热门！）点击量
        //首页消息流只填 id（可以直接获得）和 time，title
        private int _time;//（仅首页！）Unix 时间戳

        /// <summary>
        /// 首页流初始化
        /// </summary>
        /// <param name="time"></param>
        /// <param name="id"></param>
        public Cell(int time, int id, string title)
        {
            _time = time;
            _id = id;
            _title = title;
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
            _url = url;
            _avatar = avatar;
            _title = title;
            _views = views;
            format(relativePath);
        }
        /// <summary>
        /// （仅热门！）avatar 相对转绝对 URL ，填充 id
        /// </summary>
        /// <param name="relativePath">"https://news-at.zhihu.com"或其他</param>
        private void format(string relativePath)
        {
            if (_avatar.StartsWith('/'))
                _avatar = relativePath + _avatar;//相对路径
            _id = Convert.ToInt32(Regex.Match(_url, @"(?<=story/)\d+$").Value);
        }
    }
}
