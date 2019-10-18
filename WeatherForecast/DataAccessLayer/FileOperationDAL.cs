using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.DataAccessLayer
{
    class FileOperationDAL : IFileOps
    {
        public void cityBasedFileCreation(string cityName, DateTimeOffset specificdate, Weather weatherInfo, string dateTimeGMT, string uriFolder)
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
            catch (Exception e)
            {
                Console.WriteLine("Error in cityBasedFileCreation" + e.ToString());
            }
        }

        public string createFolderInDocuments()
        {
            string locationToCreateFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); //check if this is not accessible
            string folderName = "Prudential_WeatherData";
            string str = String.Format("{0}" + "\\" + "{1}", locationToCreateFolder, folderName);
            if (!Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
            return str;
        }

        public Hashtable moveDataToProperFormat(List<string> logFile)
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
