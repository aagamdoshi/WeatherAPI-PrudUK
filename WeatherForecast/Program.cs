using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using SimpleInjector;
using System.IO;
using System.Linq;
using WeatherForecast.BusinessLayer;
using WeatherForecast.DataAccessLayer;

namespace WeatherForecast
{
    class Program
    {
        public static void Main(string[] args)
        {
            Hashtable listCity;
            RootObject data;
            var dataFile = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var fileName = ConfigurationManager.AppSettings["FILENAME"].ToString();
            string readDataFileUri = String.Format("{0}" + "\\" + "{1}", dataFile, fileName);
            Console.WriteLine("Kindly put the proper formatted cities data in TestData.txt file in path.\n" + readDataFileUri + " \nPress \"Enter\" to continue file processing ");
            Console.ReadLine();
            
            //read the data in TestData.txt and move into proper data structure.
            List<string> logFile = File.ReadAllLines(readDataFileUri).ToList();
            
            var container = new Container();
            var ls = Lifestyle.Singleton;
            container.Register<IFileOps, FileOperationDAL>(ls);
            container.Register<IGetData, GetDataFromRestDAL>(ls);
            var bl = container.GetInstance<FileOperationBL>();
            var dataBl = container.GetInstance<GetDataFromRestBL>();
            
            listCity = bl.moveDataToProperFormat(logFile);
            Console.WriteLine("Total number of cities found in the text file is " + logFile.Count() + " \nProcessing Data.....");
            ICollection keys = listCity.Keys;

            //create the  Prudential output folder for weather data
            string uriFolder = bl.createFolderInDocuments();

            //Iterate through each data and create/update file based on timestamp
            foreach (string cityId in keys)
            {
                try
                {
                    string appId = ConfigurationManager.AppSettings["APPID"].ToString();
                    string appURL = ConfigurationManager.AppSettings["APPURL"].ToString();

                    //Get response from WeatherRestAPI
                    var uri = String.Format(appURL, cityId, appId);
                    data = dataBl.getDataFromRestAPI(uri);

                    //iterate and pass the date and city
                    string cityName = data.city.name;
                    foreach (ListData ld in data.list)
                    {
                        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ld.dt);
                        bl.cityBasedFileCreation(cityName, dateTimeOffset, ld.weather[0], ld.dt_txt, uriFolder);
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
    }
}
