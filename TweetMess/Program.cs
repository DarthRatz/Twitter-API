using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tweetinvi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.Interfaces;


namespace TweetMess
{
    class Program
    {
        private static string accessToken = "45558579-whhXCzm29FqrXAF18QmWf1lVOPPbMFWG3UPVRpnKa";
        private static string accessTokenSecret = "kQHvxRmUS3swDoV6VeyfKgLxYaVYOvDdjB70DNcjg";
        private static string consumerKey = "NpK6abZKBKt5t5Q3YuZFA";
        private static string consumerSecret = "vWX2VZWKBxIVaeWf3oKkeJI4M2Qw8bFMUM6l4kAYEuY";
        private static string authInfo = "BQDsaX4taDa6op3D8WaTw_CHcfYTk_dESfj9FfTuJwd-H1U4idwqNGB2NVuwa7KEzyB3k2hy92HaASvfe3o2eQ_Nug9zs5ETiw3oVmnaKcEuqLahI3I9rC2sO8JVWr17fbymSqLS6paYG511rzax2b5VAG73sMcdI8Yc6OEr28FtXlPHwTjhclfzYW85Tj6SQ_rut3AU4Ty4aU-izSnYYoW4i01nrKF-K4Oitg";

        static void Main(string[] args)
        {
            String searchTerm = Console.ReadLine();
            JObject searchReturn = spotifySearch(searchTerm);

            //Console.WriteLine(searchReturn.ToString());
            String songTitle = searchReturn["tracks"]["items"][0]["name"].ToString();
            String artistName = searchReturn["tracks"]["items"][0]["artists"][0]["name"].ToString();
            String albumName = searchReturn["tracks"]["items"][0]["album"]["name"].ToString();
            String albumArtUrl = searchReturn["tracks"]["items"][0]["album"]["images"][0]["url"].ToString();

            Console.WriteLine(songTitle + " " + artistName + " " + albumName);
            Console.WriteLine(albumArtUrl);

            JObject playlist = addToPlaylist(searchReturn);
            Console.WriteLine(playlist.ToString());

            Console.WriteLine(SearchTweets("#lastfm"));
            
            Console.ReadLine();
        }

        private static JObject spotifySearch(String searchTerm)
        {
            String spotifySearch = ("https://api.spotify.com/v1/search?q=" + searchTerm.Replace(" ", "+") + "&type=track&market=IE&limit=1");

            WebRequest webRequest = WebRequest.Create(spotifySearch);
            WebResponse webResponse = webRequest.GetResponse();

            Encoding enc = Encoding.GetEncoding(1252);
            StreamReader configStream = new StreamReader(webResponse.GetResponseStream(), enc);

            string configuration = configStream.ReadToEnd();
            JObject o = JObject.Parse(configuration);

            return o;
        }
        private static JObject addToPlaylist(JObject searchReturn)
        {
            String songSpotifyUri = searchReturn["tracks"]["items"][0]["uri"].ToString();
            String spotifyurl = "https://api.spotify.com/v1/users/darthratz/playlists/7mQ1i7Sal2T67ftrLViGBm/tracks?uris=";
            spotifyurl += songSpotifyUri;

            //Console.WriteLine(spotifyurl);

            HttpWebRequest webRequest = WebRequest.Create(spotifyurl) as HttpWebRequest; ;
            webRequest.Headers["Authorization"] = "Bearer " + authInfo;
            webRequest.Method = "POST";

            try
            {
                WebResponse webResponse = webRequest.GetResponse();

                Encoding enc = Encoding.GetEncoding(1252);
                StreamReader configStream = new StreamReader(webResponse.GetResponseStream(), enc);

                string configuration = configStream.ReadToEnd();
                JObject o = JObject.Parse(configuration);

                JArray array = (JArray)o["items"];
                foreach (dynamic item in array)
                {
                    Console.WriteLine(item["track"]["name"].ToString());
                }
                return o;
            }
            catch { return new JObject();}
        }
        private static String SearchTweets(string query)
        {
            TwitterCredentials.SetCredentials(accessToken, accessTokenSecret, consumerKey, consumerSecret);

            IEnumerable<ITweet> tweets = new List<ITweet>();
            tweets = Search.SearchTweets(query);

            String tweetString ="";
            foreach (dynamic tweet in tweets)
            {
                Console.WriteLine(tweetString);

                /*JObject searchReturn = spotifySearch(tweetString);
                String songTitle = searchReturn["tracks"]["items"][0]["name"].ToString();
                String artistName = searchReturn["tracks"]["items"][0]["artists"][0]["name"].ToString();
                String albumName = searchReturn["tracks"]["items"][0]["album"]["name"].ToString();

                Console.WriteLine(songTitle + " " + artistName + " " + albumName);
                JObject playlist = addToPlaylist(searchReturn);
                */
                tweetString += tweet;
            }

            return tweetString;
        }
    }
}
