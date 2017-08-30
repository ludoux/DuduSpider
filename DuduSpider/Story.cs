using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    

    class Story
    {
        StoryJson storyJson = new StoryJson();
        /// <summary>
        /// 阅读模式的 html，可能存在<div style='height: 100vh; display: flex; align-items: center; justify-content: center;'>该文章暂不支持阅读模式</div>
        /// </summary>
        public string Body { get => storyJson._body ?? ""; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get => storyJson._title; }

        /// <summary>
        /// 分享时的 url
        /// </summary>
        public string Share_url { get => storyJson._share_url; }

        /// <summary>
        /// 供手机端 WebView 使用，可空
        /// </summary>
        public string[] Js { get => storyJson._js; }

        /// <summary>
        /// 供 Google Analytics 使用，本软件暂不使用
        /// </summary>
        public int Ga_prefix { get => storyJson._ga_prefix; }

        /// <summary>
        /// 通常仅为 1 个，可空
        /// </summary>
        public string[] Images { get => storyJson._images; }

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get => storyJson._id; }

        /// <summary>
        /// css 样式
        /// </summary>
        public string[] Css { get => storyJson._css; }

        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get => storyJson._type; }

        /// <summary>
        /// 好像当没有阅读模式时（不确定 type = 2），就要访问这个
        /// </summary>
        public string External_url { get => storyJson._external_url; }

        public Manifestx Manifest { get => _manifest; }
        private Manifestx _manifest = new Manifestx();//{下面两个}
        internal class StoryJson
        {
            [JsonProperty("body", NullValueHandling = NullValueHandling.Ignore)]
            internal string _body;

            [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
            internal string _title;

            [JsonProperty("share_url", NullValueHandling = NullValueHandling.Ignore)]
            internal string _share_url;

            [JsonProperty("js", NullValueHandling = NullValueHandling.Ignore)]
            internal string[] _js;

            [JsonProperty("ga_prefix", NullValueHandling = NullValueHandling.Ignore)]
            internal int _ga_prefix;

            [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore)]
            internal string[] _images;

            [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
            internal int _id;

            [JsonProperty("css", NullValueHandling = NullValueHandling.Ignore)]
            internal string[] _css;

            [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
            internal int _type;

            [JsonProperty("external_url", NullValueHandling = NullValueHandling.Ignore)]
            internal string _external_url;
        }
        internal class Manifestx
        {
            public Manifestx()
            {

            }
            public Manifestx(List<string> storyManifest, List<string> imageManifest)
            {
                this._storyManifest = storyManifest;
                this._imageManifest = imageManifest;
            }
            internal List<string> _storyManifest = new List<string>(2);//{本地地址, 标题} （在“根”目录）./files/html/6F489B596D7686A2.html, 美国核动力航母一直在下饺子，中国想追上非常不容易
            internal List<string> _imageManifest = new List<string>();//（在“根”目录）./files/image/981639B8C1BB31C5.jpg
            public override bool Equals(object obj)
            {
                return this._storyManifest[0] == ((Manifestx)obj)._storyManifest[0];
            }
            public override int GetHashCode()
            {
                return _storyManifest[0].GetHashCode();
            }
            public List<string> this[int index]
            {
                get
                {
                    if (index == 0)
                        return _storyManifest;
                    else if (index == 1)
                        return _imageManifest;
                    return null;
                }
                set
                {
                    if (index == 0)
                        _storyManifest = value;
                    else if (index == 1)
                        _imageManifest = value;
                }
            }
        }
        /// <summary>
        /// 初始化就会写 .html 文件啦！
        /// </summary>
        /// <param name="json"></param>
        public Story(string json)
        {
            StoryJson s = JsonConvert.DeserializeObject<StoryJson>(json);
            if(s != null)
            {
                storyJson = JsonConvert.DeserializeObject<StoryJson>(json);
                MakeHtmlFile();
            }
            
        }

        public void MakeHtmlFile()
        {
            StringBuilder sb = new StringBuilder();
            if(!Body.Contains("该文章暂不支持阅读模式"))
            {//从 Body 读入F
                Console.Write(" <内置");
                sb.Append("<head>");
                //为了更加简洁的 html 文件，不写入 css
                //for(int i = 0; i < Css.Length; i++)
                //{
                    //string fileName = HttpRequest.DownloadFile(Css[i], extension:".css", directory:@".\\files\css\");
                    //sb.AppendFormat(@"<link rel=""stylesheet"" type=""text/css"" href=""{1}"">", i, @"../css/" + fileName);
                //}
                sb.AppendFormat("</head><body>{0}</body>", cleanHtmlText(Body));

                Regex r = new Regex(@"(?<=<img.*?src="").*?(?="".*?>)");
                MatchCollection collection = r.Matches(sb.ToString());
                Console.Write(" <"+ collection.Count.ToString());
                if (collection.Count > 8)
                {
                    _manifest._storyManifest = new List<string>() { "", "" };
                    _manifest._imageManifest = new List<string>();
                    return;
                }
                foreach (Match m in collection)
                {
                    if (m.Value != "")
                    {
                        string fileName = HttpRequest.DownloadFile(m.Value, @"./files/image/");
                        if(fileName != "")
                        {
                            sb.Replace(m.Value, @"../image/" + fileName);
                            _manifest._imageManifest.Add(@"./files/image/" + fileName);
                        }
                        
                    }
                }
                File.WriteAllText(@"./files/html/" + HashTools.Hash_MD5_16(Title) + ".html", sb.ToString(), Encoding.UTF8);
                _manifest._storyManifest = new List<string>() { @"./files/html/" + HashTools.Hash_MD5_16(Title) + ".html", Title };
                
            }
            else
            {//从 External_url 获取
                if (!External_url.Contains("mp.weixin.qq.com"))//目前仅分析微信公众号
                {
                    Console.Write(" <外站");
                    _manifest._storyManifest = new List<string>() { "", "" };
                    _manifest._imageManifest = new List<string>();
                    return;
                }
                    
                Console.Write(" <微信");
                StringBuilder request = new StringBuilder(cleanHtmlText(HttpRequest.DownloadString(External_url, "")));
                
                Regex r = new Regex(@"(?<=<img.*?src="").*?(?="".*?>)", RegexOptions.Singleline);
                MatchCollection collection = r.Matches(request.ToString());
                Console.Write(" <" + collection.Count.ToString());
                if (collection.Count > 8)
                {
                    _manifest._storyManifest = new List<string>() { "", "" };
                    _manifest._imageManifest = new List<string>();
                    return;
                }
                foreach (Match m in collection)
                {
                    if (m.Value != "")
                    {
                        string fileName = HttpRequest.DownloadFile(m.Value, @"./files/image/","");
                        if (fileName != "")
                        {
                            sb.Replace(m.Value, @"../image/" + fileName);
                            _manifest._imageManifest.Add(@"./files/image/" + fileName);
                        }
                    }
                }
                request.Replace("data-src", "src");
                File.WriteAllText(@"./files/html/" + HashTools.Hash_MD5_16(Title) + ".html", request.ToString(), Encoding.UTF8);

                _manifest._storyManifest = new List<string> { @"./files/html/" + HashTools.Hash_MD5_16(Title) + ".html", Title };
            }
        }
        private string cleanHtmlText(string htmlSource)
        {
            Regex r = new Regex("<script.*?</script>", RegexOptions.Singleline);
            MatchCollection collection = r.Matches(htmlSource.ToString());
            foreach (Match m in collection)
            {
                htmlSource = htmlSource.Replace(m.Value, "");
            }

            r = new Regex("<style>.*?</style>", RegexOptions.Singleline);
            collection = r.Matches(htmlSource.ToString());
            foreach (Match m in collection)
            {
                htmlSource = htmlSource.Replace(m.Value, "");
            }
            r = new Regex("<span.*?>", RegexOptions.Singleline);
            collection = r.Matches(htmlSource.ToString());
            foreach (Match m in collection)
            {
                htmlSource = htmlSource.Replace(m.Value, "");
            }
            htmlSource = htmlSource.Replace("</span>", "");
            return htmlSource;
        }
        public override bool Equals(object obj)
        {
            if (Id == ((Story)obj).Id)
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
            return JsonConvert.SerializeObject(storyJson);
        }
    }
}
