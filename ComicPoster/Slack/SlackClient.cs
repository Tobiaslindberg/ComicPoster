using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
namespace ComicPoster.Slack
{
    public class SlackClient
    {
        private const string SuccessMessage = "ok";

        public bool Post(Message message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var serializeMessage = JsonConvert.SerializeObject(message, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var data = Encoding.UTF8.GetBytes(serializeMessage);

            var webRequest = WebRequest.Create(SettingsHelper.SlackUrl);
            webRequest.Method = "POST";
            webRequest.ContentLength = data.Length;

            using (var requestStream = webRequest.GetRequestStream())
            {
                requestStream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)webRequest.GetResponse();

            var responseMessage = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return SuccessMessage.Equals(responseMessage);
        }
    }
}