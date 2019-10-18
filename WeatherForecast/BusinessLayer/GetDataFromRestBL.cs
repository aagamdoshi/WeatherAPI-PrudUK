using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.DataAccessLayer;

namespace WeatherForecast.BusinessLayer
{
    class GetDataFromRestBL
    {
        private IGetData _objData;

        public GetDataFromRestBL(IGetData objData)
        {
            _objData = objData;
        }

        public RootObject getDataFromRestAPI(string uri)
        {
            return _objData.getDataFromRestAPI(uri);
        }

    }
}
