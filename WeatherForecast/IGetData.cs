namespace WeatherForecast.DataAccessLayer
{
    interface IGetData
    {
        RootObject getDataFromRestAPI(string uri);
    }
}