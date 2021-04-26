using searchfight.Entities;
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


namespace searchfight
{
    public class SearchFromEngine
    {

        /// <summary>
        /// Gets the Number of Total Results for a given text in Search engines.
        /// </summary>
        /// <param name="textToSearch">Text to be searched in engines</param>
        /// <param name="googleResult">Number of total results in google engine.</param>
        /// <param name="azureResult">Number of total results in Bing engine.</param>
        public static void GetInfoFromSearchEngines(string textToSearch, ref Int64 googleResult, ref Int64 azureResult)
        {
            try
            {
                googleResult = GetTotalCountFromGoogle(textToSearch);
                azureResult = GetTotalCountFromAzure(textToSearch);
                Console.WriteLine($"{textToSearch}: Google: {googleResult} MSN Search: {azureResult} ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error in GetInfoFromSearchEngines. Error Message: {ex.Message} ; StackTrace: {ex.StackTrace} ");
            }


        }

        /// <summary>
        /// Gets the Number of Total Results from Google.
        /// </summary>
        /// <param name="textToSearch">Text to be searched in engines</param>
        public static Int64 GetTotalCountFromGoogle(string textToSearch)
        {
            Int64 numberOfRows = 0;


            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(Constants.googleApiURL);
                NameValueCollection googleCollection = new NameValueCollection();
                googleCollection.Add("key", Constants.googleApiKey);
                googleCollection.Add("cx", Constants.googleSearchEngineId);
                googleCollection.Add("q", textToSearch);
                googleCollection.Add("alt", "json");
                googleCollection.Add("fields", "queries(request(totalResults))");

                string gooResource = Constants.googleResource + ToQueryString(googleCollection);
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

        /// <summary>
        /// Gets the Number of Total Results from Bing.
        /// </summary>
        /// <param name="textToSearch">Text to be searched in engines</param>
        public static Int64 GetTotalCountFromAzure(string textToSearch)
        {
            Int64 numberOfRows = 0;
            NameValueCollection azureCollection = new NameValueCollection();
            azureCollection.Add("q", textToSearch);

            string resource = Constants.azureResource + ToQueryString(azureCollection);

            using (var client = new System.Net.Http.HttpClient())
            {
                client.BaseAddress = new Uri(Constants.azureApiURL);
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constants.azureApiKey);
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
