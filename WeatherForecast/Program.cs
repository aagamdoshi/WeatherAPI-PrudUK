using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace WeatherForecast
{
    class Program
    {
        public static void Main(string[] args)
        {
            Hashtable listCity;
            RootObject data;
            List<string> logFile = File.ReadAllLines(@"C:\D Drive\Weather Dump\TestData.txt").ToList(); //can automate this path as well
            listCity = moveDataToProperFormat(logFile);
            ICollection keys = listCity.Keys;
            createFolderInDocuments();
            foreach (string cityId in keys)
            {
                var uri = String.Format("http://api.openweathermap.org/data/2.5/forecast?id={0}&APPID=aa69195559bd4f88d79f9aadeb77a8f6", cityId);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    var apiResponse = reader.ReadToEnd();
                    data = JsonConvert.DeserializeObject<RootObject>(apiResponse);
                }
                //iterate and pass the date and city
                string cityName = data.city.name;
                foreach (ListData ld in data.list)
                {
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ld.dt);
                    cityBasedFileCreation(cityName, dateTimeOffset, ld.weather[0], ld.dt_txt);
                }
            }
        }

        private static void cityBasedFileCreation(string cityName, DateTimeOffset specificdate, Weather weatherInfo, string dateTimeGMT)
        {
            string filename = String.Format("{0} " + "-- {1}" + ".txt",
                    cityName, specificdate.Date.ToShortDateString());
            string path = String.Format("C:\\D Drive\\Weather Dump\\{0}",filename);

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                string createText = "Main: " + weatherInfo.main + Environment.NewLine + 
                    "Description: " + weatherInfo.description + Environment.NewLine
                   + "Indian Time Zone: " + specificdate.LocalDateTime.ToShortTimeString() + " ------  G.M.T: " + dateTimeGMT + Environment.NewLine;
                File.WriteAllText(path, createText);
            }

            string appendText = Environment.NewLine + "Main: " + weatherInfo.main + Environment.NewLine
                + "Description: " + weatherInfo.description + Environment.NewLine
            + "Indian Time Zone: " + specificdate.LocalDateTime.ToShortTimeString() + " ------  G.M.T: " + dateTimeGMT + Environment.NewLine;
            File.AppendAllText(path, appendText);
            
        }

        private static void createFolderInDocuments()
        {
            //string locationToCreateFolder = @"C:\D Drive\Weather Dump\";
            try
            {
                string locationToCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //check if this is not accessible
                string folderName = "-Prudential-WeatherData";
                //string date = DateTime.Now.ToString("ddd MM.dd.yyyy");
                if (!Directory.Exists(locationToCreateFolder + folderName))
                {
                    Directory.CreateDirectory(locationToCreateFolder + folderName);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        private static Hashtable moveDataToProperFormat(List<string> logFile)
        {
            Hashtable lst = new Hashtable();
            foreach (string line in logFile)
            {
                string[] temp = line.Split('=');
                if (temp.Length >= 2)
                {
                    lst.Add(temp[0], temp[1]);
                }
            }
            return lst;
        }
    }

    #region Class Creation
    public class Main
    {
        public double temp { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }
        public double pressure { get; set; }
        public double sea_level { get; set; }
        public double grnd_level { get; set; }
        public int humidity { get; set; }
        public double temp_kf { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Wind
    {
        public double speed { get; set; }
        public double deg { get; set; }
    }

    public class Rain
    {
    }

    public class Sys
    {
        public string pod { get; set; }
    }

    public class ListData
    {
        public int dt { get; set; }
        public Main main { get; set; }
        public List<Weather> weather { get; set; }
        public Clouds clouds { get; set; }
        public Wind wind { get; set; }
        public Rain rain { get; set; }
        public Sys sys { get; set; }
        public string dt_txt { get; set; }
    }

    public class Coord
    {
        public double lat { get; set; }
        public double lon { get; set; }
    }

    public class City
    {
        public int id { get; set; }
        public string name { get; set; }
        public Coord coord { get; set; }
        public string country { get; set; }
    }

    public class RootObject
    {
        public string cod { get; set; }
        public double message { get; set; }
        public int cnt { get; set; }
        public List<ListData> list { get; set; }
        public City city { get; set; }
    }
    #endregion
}
