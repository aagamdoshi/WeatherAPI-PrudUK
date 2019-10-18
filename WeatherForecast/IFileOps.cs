using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast
{
    interface IFileOps
    {
        Hashtable moveDataToProperFormat(List<string> logFile);

        string createFolderInDocuments();

        void cityBasedFileCreation(string cityName, DateTimeOffset specificdate, Weather weatherInfo, string dateTimeGMT, string uriFolder);

    }
}
