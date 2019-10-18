using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.DataAccessLayer
{
    class GetDataFromRestDAL : IGetData
    {
        public RootObject getDataFromRestAPI(string uri)
        {
            RootObject data;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                var apiResponse = reader.ReadToEnd();
                data = JsonConvert.DeserializeObject<RootObject>(apiResponse); //deserializing the API data to RootObject
            }
            return data;
        }
    }
}
