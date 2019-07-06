using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ProcessWatch
{
    public interface IAppSettings
    {
        string ApplicationName { get; }
        double StatusCheckInterval { get; }
        bool AutoRestartOnFailure { get; }
        string LogFileLocation { get; }
        int RetainedLogFileCount { get; }
        string RestartScriptFileLocation { get; }
        List<string> ProcessNames { get; }

        // Email
        bool SendEmailAlerts { get; }
        string ToEmailAddresses { get; }
        string FromEmailAddress { get; }
        string EmailLoginName { get; }
        string EmailLoginPassword { get; }
        bool EmailEnableSsl { get; }
        string MailServerAddress { get; }
        int MailServerPort { get; }
        int EmailBatchPostingLimit { get; }
        int EmailSendInterval { get; }
    }

    public class AppSettings : IAppSettings
    {
        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationName"].ToString();

        public double StatusCheckInterval => double.Parse(ConfigurationManager.AppSettings["StatusCheckIntervalMillis"].ToString());

        public bool AutoRestartOnFailure => bool.Parse(ConfigurationManager.AppSettings["AutoRestartOnFailure"].ToString());

        public string LogFileLocation => ConfigurationManager.AppSettings["LogFileLocation"].ToString();

        public int RetainedLogFileCount => int.Parse(ConfigurationManager.AppSettings["RetainedLogFileCount"].ToString());

        public string RestartScriptFileLocation => ConfigurationManager.AppSettings["RestartScriptFileLocation"].ToString();

        public List<string> ProcessNames => ConfigurationManager.AppSettings["ProcessNames"].ToString().Split(',').ToList();

        #region Email config
        public bool SendEmailAlerts => bool.Parse(ConfigurationManager.AppSettings["SendEmailAlerts"].ToString());

        public string ToEmailAddresses => ConfigurationManager.AppSettings["ToEmailAddresses"].ToString();

        public string FromEmailAddress => ConfigurationManager.AppSettings["FromEmailAddress"].ToString();

        public string EmailLoginName => ConfigurationManager.AppSettings["EmailLoginName"].ToString();

        public string EmailLoginPassword => ConfigurationManager.AppSettings["EmailLoginPassword"].ToString();

        public bool EmailEnableSsl => bool.Parse(ConfigurationManager.AppSettings["EmailEnableSsl"].ToString());

        public string MailServerAddress => ConfigurationManager.AppSettings["MailServerAddress"].ToString();

        public int MailServerPort => int.Parse(ConfigurationManager.AppSettings["MailServerPort"].ToString());

        public int EmailBatchPostingLimit => int.Parse(ConfigurationManager.AppSettings["EmailBatchPostingLimit"]);

        public int EmailSendInterval => int.Parse(ConfigurationManager.AppSettings["EmailSendIntervalMillis"]);

        #endregion
    }

}
