using Serilog;
using Serilog.Sinks.Email;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Timers;

namespace ProcessWatch
{
    public partial class ProcessWatchService : ServiceBase
    {
        public static EventLog eventLog;
        private Timer _intervalTimer;
        private ProcessWatcher _watcher;
        public AppSettings settings;

        public ProcessWatchService()
        {
            InitializeComponent();
            _intervalTimer = new Timer();
            settings = new AppSettings();

            ConfigureEventLog();
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry($"Starting {Constants.ApplicationName}");

            try
            {
                ConfigureSerilog();

                _watcher = new ProcessWatcher(settings);

                _intervalTimer.Elapsed += new ElapsedEventHandler(_watcher.CheckProcesses);
                _intervalTimer.Interval = settings.StatusCheckInterval;
                _intervalTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                eventLog.WriteEntry($"Exception thrown: {ex.Message}");
            }
            Log.Logger.Information("End OnStart.");
        }

        /// <summary>
        /// Configure serilog for both log files and email.
        /// Note: Only Fatal events are written to email.
        /// </summary>
        private void ConfigureSerilog()
        {
            if (settings.SendEmailAlerts)
            {
                Log.Logger = new LoggerConfiguration()
                .WriteTo
                .RollingFile($"{settings.LogFileLocation}{settings.ApplicationName}.txt"
                , Serilog.Events.LogEventLevel.Verbose
                , retainedFileCountLimit: settings.RetainedLogFileCount
                , outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}")
                .WriteTo
                .Email(
                    new EmailConnectionInfo
                    {
                        EmailSubject = $"{settings.ApplicationName} Alert",
                        EnableSsl = settings.EmailEnableSsl,
                        FromEmail = settings.FromEmailAddress,
                        ToEmail = settings.ToEmailAddresses,
                        MailServer = settings.MailServerAddress,
                        NetworkCredentials = new NetworkCredential
                        {
                            UserName = settings.EmailLoginName,
                            Password = settings.EmailLoginPassword
                        },
                        Port = settings.MailServerPort,
                    }, batchPostingLimit: settings.EmailBatchPostingLimit,
                    period: new TimeSpan(settings.EmailSendInterval),
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} WARNING: {Message} {Exception}"
                )
                .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                .WriteTo
                .RollingFile($"{settings.LogFileLocation}{settings.ApplicationName}.txt"
                , Serilog.Events.LogEventLevel.Verbose
                , retainedFileCountLimit: settings.RetainedLogFileCount)
                .CreateLogger();

            }
            Log.Logger.Information("Logger created.");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry($"Stopping {settings.ApplicationName} service.");
            CleanUpResources();
        }

        /// <summary>
        /// Configure windows event log for basic logging on startup/shutdown.
        /// </summary>
        private void ConfigureEventLog()
        {
            try
            {
                eventLog = new EventLog("Application");
                eventLog.Source = settings.ApplicationName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            eventLog.WriteEntry($"{settings.ApplicationName} event log configured.");
        }

        private void CleanUpResources()
        {
            if (eventLog != null)
            {
                eventLog.Dispose();
            }
            if (_intervalTimer != null)
            {
                _intervalTimer.Dispose();
            }

        }

    }
}
