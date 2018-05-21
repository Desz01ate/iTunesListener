using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace iTunesListener
{
    static class HTMLHelper
    {
        public static string Image_source
        {
            get
            {
                return "<img.+?src=[\"'](.+?)[\"'].+?>";
            }
        }
        public static string A_hyperlink_reference
        {
            get
            {
                return "<a.+?href=[\"'](.+?)[\"'].+?>";
            }
        }
        public static async Task<string> HTMLAgilityPackParser(string searchingKeyword, string XPath, params string[] searchingContent)
        {
            var url = $"https://www.google.co.th/search?q={searchingKeyword.Replace(' ', '+').Replace('&','-')}";
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(url);
                var input = System.Text.Encoding.UTF8.GetString(response);
                var doc = new HtmlDocument();
                byte[] byteArray = Encoding.ASCII.GetBytes(input);
                var ts = new MemoryStream(byteArray);
                doc.Load(ts);
                var root = doc.DocumentNode;
                var hrefs = root.SelectNodes(XPath).Select(p => p.GetAttributeValue("href", "not found"));
                foreach (var href in hrefs)
                {
                    var valid = true;
                    for (var i = 0; i < searchingContent.Length; i++)
                    {
                        if (!href.Contains(searchingContent[i]))
                        {
                            valid = false;
                            break;
                        }
                        //return href;
                    }
                    if (valid)
                    {
                        return href;
                        //return href;
                    }

                }
                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string ReFormat(string musicName, string href)
        {
            try
            {
                var baseString = @"https://itunes.apple.com/th";
                baseString += href.Substring(href.IndexOf(@"/album/"));
                return baseString;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static async Task<string> HTMLParser(string searchingKeyword, string regexPattern = "<img.+?src=[\"'](.+?)[\"'].+?>")
        {
            var url = $@"https://www.youtube.com/results?search_query={searchingKeyword.Replace(' ', '+')}";
            var baseToken = "https://www.youtube.com/watch?v=";
            var ResultList = new List<string>();
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(url);
                var source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                var result = Regex.Matches(source, regexPattern, RegexOptions.IgnoreCase);
                var token = result[0].ToString();
                token = token.Substring(token.IndexOf("data-context-item-id=\"") + "data-context-item-id=\"".Length, 11);
                baseToken += token;
            }
            catch
            {
                throw new Exception();
            }
            return baseToken;
        }
        public static async Task<string> GetYoutubeVideo(string searchingKeyword)
        {
            var url = $@"https://www.youtube.com/results?search_query={searchingKeyword.Replace(' ', '+')}";
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync(url);
                var source = Encoding.GetEncoding("utf-8").GetString(response, 0, response.Length - 1);
                if (source != string.Empty)
                    return url;
            }
            catch
            {
                Console.WriteLine("Keyword not exists in uncyclopedia, throwing 404.");
            }
            searchingKeyword = searchingKeyword.Replace(' ', '+');
            return $@"https://www.google.co.th/search?q={searchingKeyword}";
        }
        public static string GetMusicURL(string name, string album, string artist)
        {
            var url = HTMLAgilityPackParser($"{name} {album} {artist} itunes.apple.com/th", "//a[contains(@href,'album')]", "itunes").Result;
            url = UrlCleaning(url);
            if (url.Contains(@"/th/"))
                return url;
            else
                url = ReFormat(name, url);
            if (url == string.Empty)
                return HTMLParser($"{name} {artist}", "<div.+?data-context-item-id=[\"'](.+?)[\"'].+?>").Result;
            return url;
        }

        private static string UrlCleaning(string url)
        {
            var httpIndex = url.IndexOf("h");
            var slashIndex = Extension.GetIndexOfAt(url, '/', 7);
            var afterSlash = url.Substring(slashIndex + 1);
            var songDigit = Extension.GetOnlyDigit(afterSlash);
            url = url.Substring(httpIndex, slashIndex - url.Substring(0, httpIndex).Length + 1) + songDigit;
            return url;
        }
    }
}
