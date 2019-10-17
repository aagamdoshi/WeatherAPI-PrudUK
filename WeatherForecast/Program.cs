using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
            var dataFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = "TestData.txt";
            string readDataFileUri = String.Format("{0}" + "\\" + "{1}", dataFile, fileName);
            Console.WriteLine("Kindly put the proper formatted cities data in TestData.txt file in path.\n" + readDataFileUri + " \nPress \"Enter\" to continue file processing ");
            Console.ReadLine();
            
            //read the data in TestData.txt and move into proper data structure.
            List<string> logFile = File.ReadAllLines(readDataFileUri).ToList();
            listCity = moveDataToProperFormat(logFile);
            Console.WriteLine("Total number of cities found in the text file is " + logFile.Count() + " \nProcessing Data.....");
            ICollection keys = listCity.Keys;

            //create the  Prudential output folder for weather data
            string uriFolder = createFolderInDocuments();

            //Iterate through each data and create/update file based on timestamp
            foreach (string cityId in keys)
            {
                try
                {
                    string appId = ConfigurationManager.AppSettings["APPID"].ToString();
                    string appURL = ConfigurationManager.AppSettings["APPURL"].ToString();

                    //Get response from WeatherRestAPI
                    var uri = String.Format(appURL, cityId, appId);
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        var apiResponse = reader.ReadToEnd();
                        data = JsonConvert.DeserializeObject<RootObject>(apiResponse); //deserializing the API data to RootObject
                    }

                    //iterate and pass the date and city
                    string cityName = data.city.name;
                    foreach (ListData ld in data.list)
                    {
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ld.dt);
                        cityBasedFileCreation(cityName, dateTimeOffset, ld.weather[0], ld.dt_txt, uriFolder);
                    }
                   
                }
                catch(Exception e)
                {
                    Console.WriteLine("Error in Main method api call" + e.ToString());
                }
                
            }
            Console.WriteLine("\nFiles updated successfully.\n\nKindly check the following path(output folder) for detailed weather records\n" + readDataFileUri + "\\Prudential_WeatherData");
            Console.ReadLine();
        }

        //Function which creates/updates files based on Cityname and timestamp
        private static void cityBasedFileCreation(string cityName, DateTimeOffset specificdate, Weather weatherInfo, string dateTimeGMT, string uriFolder)
        {
            try
            {
                string filename = String.Format("{0} " + "-- {1}" + ".txt",
                        cityName, specificdate.Date.ToShortDateString());
                string path = String.Format(uriFolder + "\\{0}", filename);

                // This text is added only once to the file.
                if (!File.Exists(path))
                {
                    // Create a file to write to.
                    string createText = "Main: " + weatherInfo.main + Environment.NewLine +
                        "Description: " + weatherInfo.description + Environment.NewLine
                       + "Indian Standard Time Zone: " + specificdate.LocalDateTime.ToShortTimeString() + " ------  G.M.T: " + dateTimeGMT + Environment.NewLine;
                    File.WriteAllText(path, createText);
                }

                string appendText = Environment.NewLine + "Main: " + weatherInfo.main + Environment.NewLine
                    + "Description: " + weatherInfo.description + Environment.NewLine
                + "Indian Standard Time Zone: " + specificdate.LocalDateTime.ToShortTimeString() + " ------  G.M.T: " + dateTimeGMT + Environment.NewLine;
                File.AppendAllText(path, appendText);
            }
            catch(Exception e)
            {
                Console.WriteLine("Error in cityBasedFileCreation" + e.ToString());
            }
            
        }

        //This function creates folder which will store the weather data as per the API response
        private static string createFolderInDocuments()
        {
            string locationToCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //check if this is not accessible
            string folderName = "Prudential_WeatherData";
            string str = String.Format("{0}"+ "\\" +"{1}",locationToCreateFolder, folderName);
            if (!Directory.Exists(str)) {
                Directory.CreateDirectory(str);
            }
            return str;
        }

        //Formats the data from TestData.txt for proper Datastructure
        private static Hashtable moveDataToProperFormat(List<string> logFile)
        {
            Hashtable lst = new Hashtable();
            foreach (string line in logFile)
            {
                string[] temp = line.Split('=');
                if (temp.Length >= 2 && !lst.ContainsKey(temp[0]))
                {
                    lst.Add(temp[0], temp[1]);
                }
            }
            return lst;
        }
    }
}
