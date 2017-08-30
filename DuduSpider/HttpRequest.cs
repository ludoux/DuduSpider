using System;
using System.IO;
using System.Net;

namespace Ludoux.DuduSpider
{
    class HttpRequest
    {
        public static string GetContent(string sURL, string referer = "https://zhihu.com", string userAgent = "Mozilla/5.0 (iPad; CPU OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60",string cookie = "", string authorization = "")
        {

            string sContent = ""; //Content
            string sLine = "";
            try
            {
                HttpWebRequest wrGETURL = WebRequest.CreateHttp(sURL);
                
                wrGETURL.Referer = referer;
                wrGETURL.Headers.Set(HttpRequestHeader.UserAgent, userAgent);
                wrGETURL.Headers.Set(HttpRequestHeader.Cookie, cookie);
                wrGETURL.Headers.Set(HttpRequestHeader.Authorization, authorization);

                Stream objStream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader objReader = new StreamReader(objStream);
                while (sLine != null)
                {
                    sLine = objReader.ReadLine();
                    if (sLine != null)
                        sContent += sLine;
                }
            }
            catch (Exception e)
            {
                sContent = "ERR!" + e.ToString();
            }
            return sContent;
        }
        public static string DownloadString(string url, string referer = "https://zhihu.com", string userAgent = "Mozilla/5.0 (iPad; CPU OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60", string cookie = "", string authorization = "")
        {
            try
            {
                using (var web = new WebClient())
                {
                    web.Headers.Set(HttpRequestHeader.Referer, referer);
                    web.Headers.Set(HttpRequestHeader.UserAgent, userAgent);
                    web.Headers.Set(HttpRequestHeader.Cookie, cookie);
                    web.Headers.Set(HttpRequestHeader.Authorization, authorization);
                    return web.DownloadString(url);
                }
            }
            catch(Exception)
            {
                return "";
            }

        }

        public static string DownloadFile(string url, string directory, string referer = "https://zhihu.com", string userAgent = "Mozilla/5.0 (iPad; CPU OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60", string cookie = "", string extension = "A")
        {
            try
            {
                Directory.CreateDirectory(directory);
                using (var web = new WebClient())
                {
                    web.Headers.Set(HttpRequestHeader.Referer, referer);
                    web.Headers.Set(HttpRequestHeader.UserAgent, userAgent);
                    web.Headers.Set(HttpRequestHeader.Cookie, cookie);
                    if (url.StartsWith("//"))
                        url = "http:" + url;
                    if (url.StartsWith("/") && !url.StartsWith("//"))
                        url = "http://mp.qq.com" + url;
                    if (extension == "A")
                    {
                        WebRequest request = WebRequest.Create(url);
                        WebResponse response = request.GetResponse();
                        switch (response.ContentType)
                        {
                            case "image/gif":
                                return "";//extension = ".gif"; break;
                            case "image/jpeg":
                                extension = ".jpg"; break;
                            case "image/png":
                                extension = ".png"; break;
                            default:
                                break;
                        }
                    }
                    string fileName = HashTools.Hash_MD5_16(url) + extension;
                    if (!File.Exists(fileName))
                        web.DownloadFile(url, directory + fileName);
                    return fileName;
                }
            }
            catch (Exception)
            {
                return "";
            }
            
        }
    }
}
