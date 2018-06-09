using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;

namespace iTunesListener
{
    static class FacebookHelper
    {
        struct RenewalAccessToken
        {
            public string Access_token { get; set; }
            public string Token_type { get; set; }
            public string Expires_in { get; set; }
        }
        public static void RenewAccessToken()
        {
            var fb = new FacebookClient(Properties.Settings.Default.AccessToken);
            dynamic param = new ExpandoObject();
            param.client_id = Properties.Settings.Default.AppID;
            param.client_secret = Properties.Settings.Default.AppSecret;
            param.grant_type = "fb_exchange_token";
            param.fb_exchange_token = Properties.Settings.Default.AccessToken;
            var result = fb.Get("oauth/access_token", param);
            RenewalAccessToken jsonResult = JsonConvert.DeserializeObject<RenewalAccessToken>(Convert.ToString(result));
            Properties.Settings.Default.AccessToken = jsonResult.Access_token;
            Properties.Settings.Default.Save();
        }
        public static void DeletePreviousPost(ref FacebookClient fbClient, Action<Post> action)
        {
            try
            {
                List<Post> jsonArray = GetApplicationPost(ref fbClient);
                var previousPost = new Post();
                System.Threading.Tasks.Parallel.ForEach(jsonArray, action);
                //jsonArray.ForEach(action);
            }
            catch
            {
            }
        }

        public static List<Post> GetApplicationPost(ref FacebookClient fbClient)
        {
            dynamic param = new ExpandoObject();
            param.fields = "posts.since(1" + DateTime.Now.ToString("MMMMyyyy") + "){id,message,permalink_url}"; //posts.since(1monthyear){id,message}
            var result = fbClient.Get("me", param);
            var data = Convert.ToString(result.posts.data);
            return JsonConvert.DeserializeObject<List<Post>>(data);
        }
    }
}
