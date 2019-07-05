using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ScaleWatch
{
    public interface IAppSettings
    {
        string StatusCheckInterval { get; }
        bool AutoRestartOnFailure { get; }
        bool SendEmailAlerts { get; }
        string LogFileLocation { get; }
        List<string> EmailAddresses { get; }
        string RestartScriptFileLocation { get; }
        List<string> ProcessNames { get; }
    }

    public class AppSettings : IAppSettings
    {
        public string StatusCheckInterval => ConfigurationManager.AppSettings["StatusCheckInterval"].ToString();

        public bool AutoRestartOnFailure => bool.Parse(ConfigurationManager.AppSettings["AutoRestartOnFailure"].ToString());

        public bool SendEmailAlerts => bool.Parse(ConfigurationManager.AppSettings["SendEmailAlerts"].ToString());

        public string LogFileLocation => ConfigurationManager.AppSettings["LogFileLocation"].ToString();

        public string RestartScriptFileLocation => ConfigurationManager.AppSettings["BatchFileScriptLocation"].ToString();

        public List<string> EmailAddresses => ConfigurationManager.AppSettings["EmailAddresses"].ToString().Split(',').ToList();

        public List<String> ProcessNames => ConfigurationManager.AppSettings["ProcessNames"].ToString().Split(',').ToList();
    }

}
