using System.Configuration;

namespace PrDCOldApp.Web.Controllers
{
    internal class Configurations
    {
        internal static string UploadsFolder
        {
            get
            {
                string configValue = ConfigurationManager.AppSettings["uploadsFolder"];
                if (string.IsNullOrWhiteSpace(configValue))
                {
                    return "/Content/Uploads/";
                }
                else
                {
                    return configValue;
                }
            }
        }
    }
}