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
        /// <summary>
        /// ApplicationName used to generate log files and emails
        /// </summary>
        public string ApplicationName => ConfigurationManager.AppSettings["ApplicationName"].ToString();

        /// <summary>
        /// Time in milliseconds between checks for process responsiveness
        /// </summary>
        public double StatusCheckInterval => double.Parse(ConfigurationManager.AppSettings["StatusCheckIntervalMillis"].ToString());

        /// <summary>
        /// Not implemented
        /// </summary>
        public bool AutoRestartOnFailure => bool.Parse(ConfigurationManager.AppSettings["AutoRestartOnFailure"].ToString());

        /// <summary>
        /// File path indicating directory to place log file in
        /// </summary>
        public string LogFileLocation => ConfigurationManager.AppSettings["LogFileLocation"].ToString();

        /// <summary>
        /// Number of log files to retain. Set to 0 to retain all logs.
        /// </summary>
        public int RetainedLogFileCount => int.Parse(ConfigurationManager.AppSettings["RetainedLogFileCount"].ToString());

        /// <summary>
        /// Location of script to be run in the event that an non-responsive process is found.
        /// </summary>
        public string RestartScriptFileLocation => ConfigurationManager.AppSettings["RestartScriptFileLocation"].ToString();

        /// <summary>
        /// Names of the processes to be monitored. These names should not include '.exe'
        /// </summary>
        public List<string> ProcessNames => ConfigurationManager.AppSettings["ProcessNames"].ToString().Split(',').ToList();

        #region Email config
        /// <summary>
        /// Enable Email logging/alerts
        /// </summary>
        public bool SendEmailAlerts => bool.Parse(ConfigurationManager.AppSettings["SendEmailAlerts"].ToString());

        /// <summary>
        /// Email Addresses that should receive alerts
        /// </summary>
        public string ToEmailAddresses => ConfigurationManager.AppSettings["ToEmailAddresses"].ToString();

        /// <summary>
        /// Email address alerts will come from
        /// </summary>
        public string FromEmailAddress => ConfigurationManager.AppSettings["FromEmailAddress"].ToString();

        /// <summary>
        /// Login name to authenticate mail server
        /// </summary>
        public string EmailLoginName => ConfigurationManager.AppSettings["EmailLoginName"].ToString();

        /// <summary>
        /// Password to authenticate mail server
        /// </summary>
        public string EmailLoginPassword => ConfigurationManager.AppSettings["EmailLoginPassword"].ToString();

        /// <summary>
        /// Enable SSL
        /// </summary>
        public bool EmailEnableSsl => bool.Parse(ConfigurationManager.AppSettings["EmailEnableSsl"].ToString());

        /// <summary>
        /// Mail server address eg. smtp.gmail.com
        /// </summary>
        public string MailServerAddress => ConfigurationManager.AppSettings["MailServerAddress"].ToString();

        /// <summary>
        /// Mail server port eg. 25
        /// </summary>
        public int MailServerPort => int.Parse(ConfigurationManager.AppSettings["MailServerPort"].ToString());

        /// <summary>
        /// Maximum number of log messages to queue before sending email
        /// </summary>
        public int EmailBatchPostingLimit => int.Parse(ConfigurationManager.AppSettings["EmailBatchPostingLimit"]);

        /// <summary>
        /// Frequency emails should be sent from service, when logs are available
        /// </summary>
        public int EmailSendInterval => int.Parse(ConfigurationManager.AppSettings["EmailSendIntervalMillis"]);

        #endregion
    }

}
