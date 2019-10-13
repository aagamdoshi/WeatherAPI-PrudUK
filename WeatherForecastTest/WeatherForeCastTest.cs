using NUnit.Framework;
using System.IO;
using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace WeatherForecastTest
{
    [TestFixture]
    public class WeatherForeCastTest
    {
        [Test, Sequential]
        public void RestAPITestFor2Cities([Values("2988507", "2964574")]string cityId)
        {
            var apiResponse = "";
            var uri = String.Format("http://api.openweathermap.org/data/2.5/forecast?id={0}&APPID=aa69195559bd4f88d79f9aadeb77a8f6", cityId);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                apiResponse = reader.ReadToEnd();
            }
            Assert.IsNotEmpty(apiResponse);
        }

        [Test]
        public void CheckAccessForEnvironmentforFolderCreation()
        {
            string accesstoEnvironment = "";
            accesstoEnvironment = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Assert.IsNotEmpty(accesstoEnvironment);
        }
        [Test]
        [TestCase(500, "Windy", "Rain is expected", "i20d")]
        public void checkForWeatherDeserialization(int id, string main, string description, string icon)
        {
            string json = @"{ 'id': 500, 'main': 'Clear','description': 'Windy','icon':'01d' }";
            JsonConvert.DeserializeObject<Weather>(json);
        }
    }
    public class ListData
    {
        public List<Weather> weather { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }
}
