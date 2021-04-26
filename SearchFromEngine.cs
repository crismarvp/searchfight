using searchfight2.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace searchfight2
{
    public class SearchFromEngine
    {
        const string googleApiURL = "https://www.googleapis.com/";
        const string googleApiKey = "AIzaSyDgJYXi8EO20Twu6z56DxSl1DwO1nxOUXg";
        const string googleResource = "customsearch/v1";
        const string googleSearchEngineId = "dd983846ea5a371da";

        const string azureApiURL = "https://api.bing.microsoft.com/";
        const string azureResource = "v7.0/search";
        const string azureApiKey = "2295058b2a3b4be3bc69386049eeca02";

        public static void GetInfoFromEngine(string textToSearch, ref Int64 googleResult, ref Int64 azureResult)
        {

            googleResult = GetTotalCountFromGoogle(textToSearch);

            azureResult = GetTotalCountFromAzure(textToSearch);
            Console.WriteLine($"{textToSearch}: Google: {googleResult} MSN Search: {azureResult} ");

        }

        public static Int64 GetTotalCountFromGoogle(string textToSearch)
        {
            Int64 numberOfRows = 0;


            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(googleApiURL);
                NameValueCollection googleCollection = new NameValueCollection();
                googleCollection.Add("key", googleApiKey);
                googleCollection.Add("cx", googleSearchEngineId);
                googleCollection.Add("q", textToSearch);
                googleCollection.Add("alt", "json");
                googleCollection.Add("fields", "queries(request(totalResults))");

                string gooResource = googleResource + ToQueryString(googleCollection);
                var response = client.GetAsync(gooResource).Result;
                string res = "";
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    Task<string> result = content.ReadAsStringAsync();
                    res = result.Result;

                    string totalResults = "0";
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(res)))
                    {
                        // Deserialization from JSON  
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(GoogleData));
                        GoogleData myDeserializedClass = (GoogleData)deserializer.ReadObject(ms);

                        totalResults = myDeserializedClass.queries.request[0].totalResults;
                    }

                    numberOfRows = Convert.ToInt64(totalResults);
                }

            }

            return numberOfRows;
        }
        public static Int64 GetTotalCountFromAzure(string textToSearch)
        {
            Int64 numberOfRows = 0;
            NameValueCollection azureCollection = new NameValueCollection();
            azureCollection.Add("q", textToSearch);

            string resource = azureResource + ToQueryString(azureCollection);

            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(azureApiURL);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureApiKey);
                var response = client.GetAsync(resource).Result;
                string res = "";
                using (HttpContent content = response.Content)
                {
                    // ... Read the string.
                    Task<string> result = content.ReadAsStringAsync();
                    res = result.Result;
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(res)))
                    {
                        // Deserialization from JSON  
                        DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(BingCustomSearchResponse));
                        BingCustomSearchResponse myDeserializedClass = (BingCustomSearchResponse)deserializer.ReadObject(ms);

                        numberOfRows = myDeserializedClass.webPages.totalEstimatedMatches;
                    }
                }

            }

            return numberOfRows;
        }

        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (
                from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();
            return "?" + string.Join("&", array);
        }

    }
}
