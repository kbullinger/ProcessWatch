using Serilog;
using Serilog.Sinks.Email;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Timers;

namespace ScaleWatch
{
    public partial class ScaleWatchService : ServiceBase
    {
        public static EventLog eventLog;
        private Timer intervalTimer;
        private ScaleWatcher watcher;

        public ScaleWatchService()
        {
            InitializeComponent();
            intervalTimer = new Timer();
            ConfigureEventLog();

        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry($"Starting {Constants.ApplicationName}");

            try
            {
                ConfigureSerilog();

                IAppSettings appSettings = new AppSettings();
                watcher = new ScaleWatcher(eventLog, appSettings);

                intervalTimer.Elapsed += new ElapsedEventHandler(watcher.CheckProcesses);
                intervalTimer.Interval = int.Parse(ConfigurationManager.AppSettings["StatusCheckIntervalMillis"].ToString());
                intervalTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                eventLog.WriteEntry($"Exception thrown: {ex.Message}");
            }
            Log.Logger.Information("End OnStart.");
        }

        private void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .RollingFile($"{ConfigurationManager.AppSettings["LogFileLocation"].ToString()}ScaleWatcher.txt"
                , Serilog.Events.LogEventLevel.Verbose
                , retainedFileCountLimit: 7)
                .CreateLogger();

            Log.Logger.Information("Logger created.");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry($"Stopping {Constants.ApplicationName} service.");
            CleanUpResources();
        }

        private void ConfigureEventLog()
        {
            try
            {
                eventLog = new EventLog("Application");
                eventLog.Source = Constants.ApplicationName;
            }
            catch (Exception ex)
            {

                //todo
                throw ex;
            }
            eventLog.WriteEntry("ScaleWatch event log configured.");
        }

        private void CleanUpResources()
        {
            if (eventLog != null)
            {
                eventLog.Dispose();
            }
            if (intervalTimer != null)
            {
                intervalTimer.Dispose();
            }
        }

    }
}
