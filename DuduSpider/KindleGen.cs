using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ludoux.DuduSpider
{
    class KindleGen
    {
        private List<Story.Manifestx> manifestList;
        private List<object[]> contents;//{string contentLabel, List<int> i}每一个 object[] 为一个目录， i 为在 list 中的位置，从 0 算起
        private string title;
        private string language;
        private string creator;
        private string publisher;
        private string subject;
        private string date;
        private string description;
        public KindleGen(List<Story.Manifestx> manifestList, string title, string language, string creator, string publisher, string subject, string date, string description, List<object[]> contents)
        {
            this.manifestList = manifestList;
            this.title = title;
            this.language = language;
            this.creator = creator;
            this.publisher = publisher;
            this.subject = subject;
            this.date = date;
            this.description = description;
            this.contents = contents;
        }
        public void MakePeriodical()
        {
            LogWriter.WriteLine("Start making .opf File!");
            makeOpfFile();
            LogWriter.WriteLine("Start making contents.html File!");
            makeContentsFile();
            LogWriter.WriteLine("Start making .ncx File!");
            makeNcxFile();
            LogWriter.WriteLine("Start making .mobi File!");
            if (makeMobiFile())
                LogWriter.WriteLine("Mobi file: " + Math.Ceiling(new System.IO.FileInfo(@"files\" + title + ".mobi").Length / 1048576.0) + " MByte.");
            else
                LogWriter.WriteLine("Error: Failed to make mobi file.");
        }
        private void makeOpfFile()
        {
            
            StringBuilder opf = new StringBuilder("<?xml version='1.0' encoding='utf-8'?>");
            opf.AppendFormat(Environment.NewLine + @"<package xmlns=""http://www.idpf.org/2007/opf"" version=""2.0"" unique-identifier=""{0}"">", HashTools.Hash_MD5_16(title + date));
            opf.AppendFormat(Environment.NewLine + 
@"<metadata>" + Environment.NewLine +
@"    <dc-metadata xmlns:dc=""http://purl.org/dc/elements/1.1/"" xmlns:opf=""http://www.idpf.org/2007/opf"">" + Environment.NewLine +
@"        <dc:title>{0}</dc:title>" + Environment.NewLine +
@"        <dc:language>{1}</dc:language>" + Environment.NewLine +
@"        <dc:Identifier id=""uid"">{2}</dc:Identifier>" + Environment.NewLine +
@"        <dc:creator>{3}</dc:creator>" + Environment.NewLine +
@"        <dc:publisher>{4}</dc:publisher>" + Environment.NewLine +
@"        <dc:subject>{5}</dc:subject>" + Environment.NewLine +
@"        <dc:date>{6}</dc:date>" + Environment.NewLine +
@"        <dc:description>{7}</dc:description>" + Environment.NewLine +
@"    </dc-metadata>", title, language, Guid.NewGuid(), creator, publisher, subject, date, description);
            opf.AppendFormat(Environment.NewLine + 
@"<x-metadata>" + Environment.NewLine +
@"        <output content-type=""application/x-mobipocket-subscription-magazine"" encoding =""utf-8""/>" + Environment.NewLine +
@"        <EmbeddedCover>./image/cover.jpg</EmbeddedCover>" + Environment.NewLine +
@"    </x-metadata>" + Environment.NewLine +
@"</metadata>");
            opf.Append(Environment.NewLine +  "<manifest>");
            int i = 0;
            foreach (Story.Manifestx m in manifestList)
            {
                opf.AppendFormat(Environment.NewLine + @"    <item href=""{0}"" media-type=""application/xhtml+xml"" id=""{1}""/>", (m[0])[0].Replace("/files", ""), string.Format("{0:D3}-html", ++i));//添加内部文章 html
            }
            i = 0;
            foreach (Story.Manifestx m in manifestList)
            {
                foreach(string ima in (List<string>)m[1])
                {
                    string extension = Regex.Match(ima, @".[^.]*?$").Value;
                    string type = "";
                    switch (extension)
                    {
                        case ".gif":
                            type = "image/gif"; break;//extension = ".gif"; break;
                        case ".jpg":
                            type = "image/jpeg"; break;
                        case ".png":
                            type = "image/png"; break;
                        default:
                            continue;//不添加这个图片
                    }
                    opf.AppendFormat(Environment.NewLine + @"    <item href=""{0}"" media-type=""{1}"" id=""{2}""/>", ima.Replace(@"/files", ""), type, string.Format("{0:D3}-image", ++i));//添加 html 对应的图片
                }
            }
            opf.AppendFormat(Environment.NewLine +
@"    <item href=""./html/contents.html"" media-type=""application/xhtml+xml"" id=""contents""/>" + Environment.NewLine +
@"    <item href=""./misc/nav-contents.ncx"" media-type=""application/x-dtbncx+xml"" id=""nav-contents""/>" + Environment.NewLine +
@"    <item href= ""./image/cover.jpg"" media-type= ""image/jpeg"" id= ""cover_img""/>" + Environment.NewLine +
@"</manifest>" + Environment.NewLine +
@"<spine toc=""nav-contents"">" + Environment.NewLine +
@"    <itemref idref=""contents""/>");
            for(i = 0; i<manifestList.Count; i++)
            {
                opf.AppendFormat(Environment.NewLine + @"    <itemref idref=""{0}""/>", string.Format("{0:D3}-html", i + 1));
            }
            opf.Append(Environment.NewLine +
@"</spine>" + Environment.NewLine + 
@"</package>");
            System.IO.File.WriteAllText(@"files\" + title + ".opf", opf.ToString(), Encoding.UTF8);
        }
        private void makeNcxFile()
        {
            StringBuilder ncx = new StringBuilder(
@"<?xml version='1.0' encoding='utf-8'?>" + Environment.NewLine +
@"<ncx xmlns:mbp=""http://mobipocket.com/ns/mbp"" xmlns =""http://www.daisy.org/z3986/2005/ncx/"" version =""2005-1"">");
            ncx.AppendFormat(Environment.NewLine +
@"    <head>" + Environment.NewLine +
@"        <meta content=""{0}"" name=""dtb:uid""/>" + Environment.NewLine +
@"        <meta content= ""2"" name=""dtb:depth""/>" + Environment.NewLine +
@"        <meta content= ""0"" name=""dtb:totalPageCount""/>" + Environment.NewLine +
@"        <meta content= ""0"" name=""dtb:maxPageNumber""/>" + Environment.NewLine +
@"    </head>", Guid.NewGuid());
            ncx.AppendFormat(Environment.NewLine +
@"    <docTitle>" + Environment.NewLine +
@"        <text>{0}</text>" + Environment.NewLine +
@"    </docTitle>" + Environment.NewLine +
@"    <docAuthor>" + Environment.NewLine +
@"        <text>{1}</text>" + Environment.NewLine +
@"	</docAuthor>", title, creator);
            ncx.AppendFormat(Environment.NewLine +
@"    <navMap>" + Environment.NewLine +
@"        <navPoint playOrder=""0"" class=""periodical"" id=""periodical"">" + Environment.NewLine +
@"            <mbp:meta-img src=""../image/masthead.jpg"" name =""mastheadImage""/>" + Environment.NewLine +
@"            <navLabel>" + Environment.NewLine +
@"                <text>目录</text>" + Environment.NewLine +
@"            </navLabel>" + Environment.NewLine +
@"            <content src=""../html/contents.html""/>");//masthead.jpg 为 网格模式头部 LOGO 图片，目前是写死路径和后缀名的！

            int playOrder = 1;//当跳目录和文章时都会续秒
            int articleOrder = 1;//只有跳文章时会续秒
            for (int i = 0; i < contents.Count; i++)
            {//i 为大的目录编号

                ncx.AppendFormat(Environment.NewLine +
@"            <navPoint playOrder=""{0}"" class=""section"" id=""{1}"">" + Environment.NewLine +
@"                <navLabel>" + Environment.NewLine +
@"                    <text>{2}</text>" + Environment.NewLine +
@"                </navLabel>" + Environment.NewLine +
@"                <content src=""{3}""/>", playOrder, string.Format("{0:D3}-nav", i + 1), contents[i][0], (manifestList[((List<int>)contents[i][1])[0]][0])[0].Replace(@"./files/html/", @"../html/"));//{3}为每个目录第一个文章文件路径（网格模式下），丧心病狂般的[]
                playOrder++;
                for (int j = 0; j < ((List<int>)contents[i][1]).Count; j++)
                {//j 为每个目录内部的文章编号
                    ncx.AppendFormat(Environment.NewLine +
@"                <navPoint playOrder=""{0}"" class=""article"" id=""{1}"">" + Environment.NewLine +
@"                    <navLabel>" + Environment.NewLine +
@"                        <text>{2}</text>" + Environment.NewLine +
@"                    </navLabel>" + Environment.NewLine +
@"                    <content src=""{3}""/>" + Environment.NewLine +
@"                </navPoint>", playOrder, string.Format("{0:D3}-article", articleOrder), (manifestList[((List<int>)contents[i][1])[j]][0])[1], (manifestList[((List<int>)contents[i][1])[j]][0])[0].Replace(@"./files/html/", @"../html/"));
                    playOrder++;
                    articleOrder++;
                }
                ncx.Append(Environment.NewLine +
@"            </navPoint>");
                
            }
            ncx.Append(Environment.NewLine +
@"        </navPoint>" + Environment.NewLine +
@"    </navMap>" + Environment.NewLine +
@"</ncx>");
            System.IO.File.WriteAllText(@"files\misc\nav-contents.ncx", ncx.ToString(), Encoding.UTF8);
        }
        private void makeContentsFile()
        {
            StringBuilder cons = new StringBuilder("<html>" + Environment.NewLine +
@"    <head>" + Environment.NewLine +
@"        <meta content=""text/html; charset=utf-8"" http -equiv=""Content-Type"" />" + Environment.NewLine +
@"        <title>目录</title>" + Environment.NewLine +
@"    </head>" + Environment.NewLine +
@"    <body>" + Environment.NewLine +
@"        <h1>目录</h1>");
            for(int i = 0; i < contents.Count; i++)
            {//i 为大的目录编号
                cons.AppendFormat(Environment.NewLine + 
@"        <h4 height=""1em"">{0}</h4>" + Environment.NewLine + 
@"        <ul>", (string)contents[i][0]);
                for(int j = 0; j< ((List<int>)contents[i][1]).Count; j++)
                {//j 为每个目录内部的文章编号
                    cons.AppendFormat(Environment.NewLine + 
@"            <li><a href=""{0}"" >{1}</a></li>", (manifestList[j][0])[0].Replace(@"./files/html/", @"./"), (manifestList[j][0])[1]);//Replace 硬替换路径
                }
                cons.Append(Environment.NewLine +  
@"        </ul>");
            }
            cons.Append(Environment.NewLine + 
@"    </body>" + Environment.NewLine + "</html>");
            System.IO.File.WriteAllText(@"files\html\contents.html", cons.ToString(), Encoding.UTF8);
        }
        private bool makeMobiFile()
        {
            
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("kindlegen.exe", "-dont_append_source " + Environment.CurrentDirectory + @"\files\" + title + ".opf -c2 -locale en");
            psi.StandardErrorEncoding = Encoding.UTF8;
            psi.StandardOutputEncoding = Encoding.UTF8;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.RedirectStandardInput = true;
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            System.Diagnostics.Process process;
            process = System.Diagnostics.Process.Start(psi);
            process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            
            if (!System.IO.File.Exists(@"files\" + title + ".mobi") || process.StandardOutput.ReadToEnd().Contains("Error"))
                return false;//未创建成功
            else
                return true;
        }
    }
}
