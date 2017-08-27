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
        

        private int _id;//format 后得到
        private string _url;//circlely://story/9587569 后面数字为 Story 类的 ID
        private string _avatar;//相对（/img/app/default_story_thumbnail.png）←这个为知乎内容的图] 或绝对（https://pic3.zhimg.com/v2-38d2a9711e5b00beb5d4112c50bf5ab6.jpg）
        private string _title;//标题
        private int _views;//点击量
        public Cell(string url, string avatar, string title, int views, string relativePath = "https://news-at.zhihu.com")
        {
            _url = url;
            _avatar = avatar;
            _title = title;
            _views = views;
            format(relativePath);
        }
        /// <summary>
        /// avatar 相对转绝对 URL ，填充 id
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
