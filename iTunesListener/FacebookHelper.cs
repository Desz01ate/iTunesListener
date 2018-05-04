using Facebook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTunesListener
{
    static class FacebookHelper
    {
        struct RenewalAccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }
        public static void RenewAccessToken()
        {
            var fb = new Facebook.FacebookClient(Properties.Settings.Default.AccessToken);
            dynamic param = new ExpandoObject();
            param.client_id = Properties.Settings.Default.AppID;
            param.client_secret = Properties.Settings.Default.AppSecret;
            param.grant_type = "fb_exchange_token";
            param.fb_exchange_token = Properties.Settings.Default.AccessToken;
            var result = fb.Get("oauth/access_token", param);
            RenewalAccessToken jsonResult = JsonConvert.DeserializeObject<RenewalAccessToken>(Convert.ToString(result));
            Properties.Settings.Default.AccessToken = jsonResult.access_token;
            Properties.Settings.Default.Save();
        }
        public static void DeletePreviousPost(ref FacebookClient fbClient,Action<Post> action)
        {
            try
            {
                dynamic param = new ExpandoObject();
                param.fields = "posts.since(" + DateTime.Now.ToString("ddMMMMyyyy") + "){id,message}";
                var result = fbClient.Get("me", param);
                var data = Convert.ToString(result.posts.data);
                List<Post> jsonArray = JsonConvert.DeserializeObject<List<Post>>(data);
                var previousPost = new Post();
                jsonArray.ForEach(action);
            }
            catch
            {

            }
        }
    }
}
