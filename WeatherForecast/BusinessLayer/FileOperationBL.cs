using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.BusinessLayer
{
    class FileOperationBL
    {
        private IFileOps _objData;

        public FileOperationBL(IFileOps objData)
        {
            _objData = objData;
        }

        public Hashtable moveDataToProperFormat(List<string> logFile)
        {
            return _objData.moveDataToProperFormat(logFile);
        }

        public string createFolderInDocuments()
        {
            return _objData.createFolderInDocuments();
        }

        public void cityBasedFileCreation(string cityName, DateTimeOffset specificdate, Weather weatherInfo, string dateTimeGMT, string uriFolder)
        {
            _objData.cityBasedFileCreation(cityName, specificdate, weatherInfo, dateTimeGMT, uriFolder);
        }
    }
}
