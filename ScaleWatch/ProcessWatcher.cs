using Serilog;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace ProcessWatch
{
    public interface IScaleWatcher
    {
        void CheckProcesses(object source, ElapsedEventArgs args);
        bool IsProcessResponding(Process process);
    }

    public class ProcessWatcher : IScaleWatcher
    {
        private readonly IAppSettings _settings;

        public ProcessWatcher(IAppSettings settings)
        {
            _settings = settings;
        }

        public void CheckProcesses(object source, ElapsedEventArgs args)
        {
            Log.Logger.Information("Checking processes...");
            List<Process> processes = GetProcesses(_settings.ProcessNames);

            if (!AreAllProcessesResponding(processes))
            {
                KillProcesses(processes);
                RestartProcessesUsingScript(_settings.RestartScriptFileLocation);
            }

        }

        private void RestartProcessesUsingScript(string batScript)
        {
            Log.Logger.Information("Restarting processes.");
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", $"/c {batScript}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            using (var process = Process.Start(info))
            {
                process.WaitForExit();

                string standardOutput = process.StandardOutput.ReadToEnd();
                string errorOutput = process.StandardError.ReadToEnd();

                Log.Logger.Information("Process exit code: {exitCode}", process.ExitCode);

                if (string.IsNullOrWhiteSpace(errorOutput))
                {
                    Log.Logger.Fatal("{batScript} did not complete successfully. Error was: {error}", batScript, errorOutput);
                }
                else
                {
                    Log.Logger.Fatal("One or more of the watched processes were unresponsive. All watched processes have been killed and restarted.");
                }
            }
        }

        private void KillProcesses(List<Process> processes)
        {
            Log.Logger.Information("Killing processes.");

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
                    Log.Logger.Error("Process {process} is not responding.", process.ProcessName);
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

        public bool IsProcessResponding(Process process) => false;// process.Responding;

    }
}
