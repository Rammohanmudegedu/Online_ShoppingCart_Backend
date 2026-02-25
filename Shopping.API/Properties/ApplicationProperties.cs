using Shopping.Utilities.Common;

namespace Shopping.API.Properties
{
    public static class ApplicationProperties
    {
        public static async void Load(IConfiguration configuration)
        {
            #region Application Specific Configuration Settings

            string logPath = string.IsNullOrWhiteSpace(configuration["Application:LocalLog"]) ? (configuration["LOG:FilePath"] ?? string.Empty) : (configuration["Application:LocalLog"] ?? string.Empty);

            ResourceConstants.ApplicationName = configuration["Application:Name"];
            ResourceConstants.ApplicationEnvironment = configuration["Application:Environment"];
            ResourceConstants.SMTPHost = configuration["SMTP:Host"];
            ResourceConstants.SMTPPort = Convert.ToInt32(configuration["SMTP:Port"]);
            ResourceConstants.SMTPUserName = configuration["SMTP:Username"];
            ResourceConstants.SMTPEncryptedSecret = configuration["SMTP:EncryptedSecret"];
            ResourceConstants.SMTPFromAddress = configuration["SMTP:FromAddress"];
            ResourceConstants.SMTPFromDisplayName = configuration["SMTP:FromDisplayName"];
            ResourceConstants.SMTPUseDefaultCredentials = Convert.ToBoolean(configuration["SMTP:UseDefaultCredentials"]);
            ResourceConstants.SMTPDeliveryMethod = configuration["SMTP:DeliveryMethod"];
            ResourceConstants.SMTPTimeout = Convert.ToInt32(configuration["SMTP:Timeout"]);
            ResourceConstants.SMTPDivertEmail = Convert.ToBoolean(configuration["SMTP:DivertEmail"]);
            ResourceConstants.SMTPDivertEmailToAddress = configuration["SMTP:DivertEmailTo"];
            ResourceConstants.SMTPRetryCount = Convert.ToInt32(configuration["SMTP:SMTPRetryCount"]);
            ResourceConstants.ErrorHandlerEmailToAddress = configuration["SMTP:ErrorEmailTo"];
            ResourceConstants.ErrorHandlerEmailSubject = configuration["LOG:ErrorEmailSubject"];
            ResourceConstants.LogToDB = Convert.ToBoolean(configuration["LOG:LogToDB"]);
            ResourceConstants.LogToFile = Convert.ToBoolean(configuration["LOG:LogToFile"]);


            #endregion Application Specific Configuration Settings
        }
    }


}
