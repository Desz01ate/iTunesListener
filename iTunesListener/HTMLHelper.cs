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
        public static async Task<string> HTMLAgilityPackParser(string searchingKeyword,string XPath,string selectedAttribute)
        {
            var url = $@"https://www.youtube.com/results?search_query={searchingKeyword.Replace(' ', '+')}";
            var baseToken = "https://www.youtube.com/watch?v=";
            try
            {
                HttpClient http = new HttpClient();
                var response = await http.GetByteArrayAsync("https://www.youtube.com/results?search_query=adam+levine");
                var input = System.Text.Encoding.UTF8.GetString(response);
                var doc = new HtmlDocument();
                byte[] byteArray = Encoding.ASCII.GetBytes(input);
                var ts = new MemoryStream(byteArray);
                doc.Load(ts);
                var root = doc.DocumentNode;
                var tag = root.SelectNodes(XPath);
                var vidUrl = tag[0].Attributes[selectedAttribute];
                baseToken += vidUrl;
            }
            catch
            {
                return string.Empty;
            }
            return baseToken;
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
            var url = $@"https://www.youtube.com/results?search_query={searchingKeyword.Replace(' ','+')}";
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
    }
}
