using System.Configuration;

namespace WebApplication.Models
{
    public static class Util
    {
        public static string GetServiceUri(string srv)
        {
            return Configuration.WidgetServiceURI + "api/" + srv;
        }
    }

    public static class Configuration
    {
        private static string _uri = null;

        public static string WidgetServiceURI
        {
            get
            {
                if (!string.IsNullOrEmpty(_uri))
                    return _uri;

                _uri = GetKeyVal("webServiceUri");
                if (string.IsNullOrEmpty(_uri))
                    return "http://localhost:7734/";
                else
                    return _uri;
            }
        }
        public static string GetKeyVal(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}