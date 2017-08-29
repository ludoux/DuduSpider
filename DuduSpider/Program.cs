﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Ludoux.DuduSpider
{
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            List<Story>  s = HotStories.fetchHotStories();//出来的是没有空文章的
            List<object[]> contents = new List<object[]>();//{string contentLabel, int[] i}每一个 object[] 为一个目录， i 为在 list 中的位置，从 0 算起
            int[] j = new int[s.Count];
            for (int k = 0; k < s.Count; k++)
                j[k] = k;

            contents.Add(new object[] { "热门文章", j });
            new KindleGen(s, "热门文章", "zh-cn", "Ludoux", "Ludoux", "Daily", DateTime.Now.Date.ToString("yyyy-MM-dd"), "Daily", contents).MakePeriodical();
        }
    }
}