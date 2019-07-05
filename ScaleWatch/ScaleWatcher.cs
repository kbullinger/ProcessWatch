using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace ScaleWatch
{
    public class ScaleWatcher
    {
        private static EventLog eventLog;
        private readonly IAppSettings _settings;

        public ScaleWatcher(EventLog evtLog, IAppSettings settings)
        {
            eventLog = evtLog;
            _settings = settings;
        }

        public void CheckProcesses(object source, ElapsedEventArgs args)
        {
            Log.Logger.Information("Checking processes.");
            List<Process> processes = GetProcesses(_settings.ProcessNames);

            if (!AreAllProcessesResponding(processes))
            {
                KillProcesses(processes);
                RestartProcessesUsingScript(_settings.RestartScriptFileLocation);
            }

        }

        private void RestartProcessesUsingScript(string batScript)
        {
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", $"/c {batScript}");
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardError = true;
            info.RedirectStandardOutput = true;

            using (var process = Process.Start(info))
            {
                process.WaitForExit();

                Log.Logger.Information(process.StandardOutput.ReadToEnd());
                Log.Logger.Error(process.StandardError.ReadToEnd());

                Log.Logger.Information("Process exit code: {exitCode}", process.ExitCode);
            }
        }

        private void KillProcesses(List<Process> processes)
        {
            Log.Logger.Information("Restarting processes.");
            processes.ForEach((process) =>
            {
                process.Kill();
            });
        }

        private bool AreAllProcessesResponding(List<Process> processes)
        {
            foreach (var process in processes)
            {
                if (!IsProcessResponding(process))
                {
                    Log.Logger.Error("Process {process} is not responding.", process);
                    return false;
                }
            }
            return true;
        }

        private List<Process> GetProcesses(List<string> processNames)
        {
            List<Process> processes = new List<Process>();
            foreach (var name in processNames)
            {
                processes.AddRange(Process.GetProcessesByName(name));
            }
            Log.Logger.Information("{process}", processes.First().Responding);
            return processes;
        }

        private bool IsProcessResponding(Process process) => process.Responding;

    }
}
