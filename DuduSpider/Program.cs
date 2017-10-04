using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
namespace ludoux.DuduSpider
{
    
    class Program
    {
        
        static void Main(string[] args)
        {
            string authorization;
            List<Uri> allowedUrlHost = new List<Uri> { new Uri("http://mp.weixin.qq.com") };
            if (args.Length != 2)
            {
                Console.Write("Start successfully with WRONG ARGS.It should look like\r\n" + @"-f ""authorization string""" + "\r\nFirst arg can be -f(F) -u(U) or -l(L)\r\nRead manual to learn more.\r\n\r\nPress any key to exit.");
                Console.ReadKey();
                return;
            }
            authorization = args[1];
            Console.WriteLine("Method: {0}\r\nAuthorization: {1}\r\n", args[0].ToLower(), args[1]);
            if (args[0].ToLower() == "-f")
                firstTime(authorization, allowedUrlHost);
            else if (args[0].ToLower() == "-u")
                update(authorization, allowedUrlHost);
            else if (args[0].ToLower() == "-l")
                lastTime(authorization, allowedUrlHost);
        }
        private static void firstTime(string authorization, List<Uri> allowedUrlHost)
        {
            File.Delete(@"Manifest.txt");
            File.Delete(@"HomeTimeline_Cell.txt");
            File.Delete(@"HotStories_Cell.txt");
            if (Directory.Exists(@"files"))
                Directory.Delete(@"files", true);
            else
                Directory.CreateDirectory(@"files");
            Directory.CreateDirectory(@"files\image");
            Directory.CreateDirectory(@"files\css");
            Directory.CreateDirectory(@"files\html");
            Directory.CreateDirectory(@"files\misc");
            fetch(authorization, new List<Cell>(), new List<Cell>(), allowedUrlHost);
        }
        private static void update(string authorization, List<Uri> allowedUrlHost)
        {
            List<Cell> oldHomeTimeline = new List<Cell>();
            foreach(string s in File.ReadLines("HomeTimeline_Cell.txt"))
            {
                oldHomeTimeline.Add(new Cell(s));
            }
            List<Cell> oldHotStories = new List<Cell>();
            foreach (string s in File.ReadLines("HotStories_Cell.txt"))
            {
                oldHotStories.Add(new Cell(s));
            }
            fetch(authorization, oldHomeTimeline, oldHotStories, allowedUrlHost);

        }
        private static void lastTime(string authorization, List<Uri> allowedUrlHost)
        {
            update(authorization, allowedUrlHost);
            List<Story.Manifestx> m = new List<Story.Manifestx>();
            Regex r = new Regex(@"(?<=\[).*?(?=\])");
            /*["./files/html/CE31BF7096DB212C.html","采访于丹老师变成了试题"]
             * []
             * 或
             * ["./files/html/1EA55D04CF9F38FC.html","知乎用户：美国哪方面比中国强？"]
             * ["./files/image/D2FA331785D115F9.jpg","./files/image/7EA29F98F11C66CF.jpg","./files/image/B165128AEB8A23BB.jpg","./files/image/079EFCA10E54D451.jpg"]
            */
            bool isStoryLine = true;//单数行
            List<string> storyLine = new List<string>();
            foreach (string mt in File.ReadAllLines("Manifest.txt"))
            {
                if (isStoryLine)
                    storyLine = r.Match(mt).Value.Replace(@"""", "").Split(',').ToList();
                else if (isStoryLine == false && r.Match(mt).Value != @"")
                    m.Add(new Story.Manifestx(storyLine, r.Match(mt).Value.Replace(@"""", "").Split(',').ToList()));
                else if(isStoryLine == false && r.Match(mt).Value == @"")//没有图片资源
                    m.Add(new Story.Manifestx(storyLine, new List<String>()));

                isStoryLine = !isStoryLine;
            }
            int i = 0;
            List<int> home = new List<int>(), hot = new List<int>();
            foreach (Story.Manifestx mx in m)
            {
                if (mx._storyManifest[2] == "首页流")
                    home.Add(i);
                else if (mx._storyManifest[2] == "热门流")
                    hot.Add(i);
                i++;
            }
            int[] j = new int[m.Count];
            for (int k = 0; k < m.Count; k++)
                j[k] = k;
            List<object[]> contents = new List<object[]>();//{string contentLabel, List<int> i}每一个 object[] 为一个目录， i 为在 list 中的位置，从 0 算起
            contents.Add(new object[] { "首页流", home });
            contents.Add(new object[] { "热门流", hot });
            new KindleGen(m, "DuduSpider", "zh-cn", "ludoux", "ludoux", "Daily", DateTime.Now.Date.ToString("yyyy-MM-dd"), "Daily", contents).MakePeriodical();
        }
        private static void fetch(string authorization, List<Cell> oldHomeTimeline, List<Cell> oldHotStories, List<Uri> allowedUrlHost)
        {

            object[] o = HomeTimeline.FetchHomeTimeline(authorization, oldHomeTimeline, allowedUrlHost);
            List<string> line = new List<string>();
            List<Story.Manifestx> m = new List<Story.Manifestx>();//这个合并两次抓取，单独生成文件，后面制作电子书时候用
            
            foreach (Story s in (List<Story>)o[0])
            {
                m.Add(s.Manifest);
            }
            foreach (Cell c in (List<Cell>)o[1])
            {
                line.Add(c.ToString());
            }
            File.AppendAllLines("HomeTimeline_Cell.txt", line, Encoding.UTF8);
            line.Clear();
            o = HotStories.FetchHotStories(((List<Cell>)o[1]).Union(oldHotStories).Union(oldHomeTimeline).ToList(), allowedUrlHost);//将首页流（和之前抓的热门流和之前抓的首页流）的信息传递过去，那边会进行去重
            List<Story.Manifestx> tempM = new List<Story.Manifestx>();
            foreach (Story s in (List<Story>)o[0])
            {
                tempM.Add(s.Manifest);
            }
            
            foreach (Cell c in (List<Cell>)o[1])
            {
                line.Add(c.ToString());
            }
            File.AppendAllLines("HotStories_Cell.txt", line, Encoding.UTF8);
            line.Clear();
            
            m = m.Union(tempM).ToList();//有可能会重叠
            tempM = null;
            foreach (Story.Manifestx mt in m)
            {
                line.Add(JsonConvert.SerializeObject(mt._storyManifest));
                line.Add(JsonConvert.SerializeObject(mt._imageManifest));
            }
            File.AppendAllLines("Manifest.txt", line, Encoding.UTF8);
        }
    }
}