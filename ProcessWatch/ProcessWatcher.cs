using Serilog;
using System;
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

        /// <summary>
        /// Checks the status of the watched processes.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        public void CheckProcesses(object source, ElapsedEventArgs args)
        {
            Log.Logger.Information("Checking processes...");
            List<Process> processes = GetProcesses(_settings.ProcessNames);

            if (processes.Any() && !AreAllProcessesResponding(processes))
            {
                KillProcesses(processes);
                if (_settings.AutoRestartOnFailure)
                {
                    RestartProcessesUsingScript(_settings.RestartScriptFileLocation);

                    // Wait five and check status
                    System.Threading.Thread.Sleep(2000);
                    if (CheckProcessesRestartedSuccessfully(GetProcesses(_settings.ProcessNames)))
                    {
                        Log.Logger.Fatal("One or more of the watched processes were unresponsive. All watched processes have been killed and restarted successfully.");
                    }
                    else
                    {
                        Log.Logger.Fatal("One or more of the watched processes were unresponsive. Watched processes were not restarted properly.");
                    }
                }
                else
                {
                    Log.Logger.Fatal("One or more watched of the watched processes has failed. {ApplicationName} has been configured to not automatically restart watched processes.", _settings.ApplicationName);
                }
            } else
            {
                Log.Logger.Information("All processes found responsive. No action taken.");
            }
        }

        /// <summary>
        /// Runs the specified .bat script (which should be written to restart the processes
        /// being monitored.)
        /// </summary>
        /// <param name="batScript"></param>
        private void RestartProcessesUsingScript(string batScript)
        {
            Log.Logger.Information("Restarting processes.");
            ProcessStartInfo info = new ProcessStartInfo("cmd.exe", "/c "+ "\"" + @batScript +"\"")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            try
            {
                using (var process = Process.Start(info))
                {
                    process.WaitForExit();

                    string standardOutput = process.StandardOutput.ReadToEnd();
                    string errorOutput = process.StandardError.ReadToEnd();

                    Log.Logger.Information("Process exit code: {exitCode}. Standard out: {standardoutput}. Standard error: {standarderror}.", process.ExitCode, standardOutput, errorOutput);

                    if (!string.IsNullOrWhiteSpace(errorOutput))
                    {
                        Log.Logger.Fatal("{batScript} did not complete successfully. Error was: {error}", batScript, errorOutput);
                    }
                }
            } catch (Exception ex)
            {
                Log.Logger.Error(ex, "An exception occurred.");
                throw ex;
            }
        }

        /// <summary>
        /// Confirms all watched processes are in a Responding state
        /// </summary>
        /// <param name="processes"></param>
        /// <returns></returns>
        private bool CheckProcessesRestartedSuccessfully(List<Process> processes)
        {
            return AreAllProcessesResponding(processes);
        }

        /// <summary>
        /// Kills all processes being watched.
        /// </summary>
        /// <param name="processes"></param>
        private void KillProcesses(List<Process> processes)
        {
            Log.Logger.Information("Killing processes.");

            processes.ForEach((process) =>
            {
                process.Kill();
            });
        }

        /// <summary>
        /// Checks to see if all processes are responding. 
        /// Returns false and stops as soon as a not responding process is found.
        /// </summary>
        /// <param name="processes"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets processes by name.
        /// NOTE: The names do not include '.exe'
        /// </summary>
        /// <param name="processNames"></param>
        /// <returns></returns>
        private List<Process> GetProcesses(List<string> processNames)
        {
            List<Process> processes = new List<Process>();
            foreach (var name in processNames)
            {
                processes.AddRange(Process.GetProcessesByName(name));
            }
            return processes;
        }

        /// <summary>
        /// Checks if a process is responding.
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public bool IsProcessResponding(Process process) => false;// process.Responding;

    }
}
