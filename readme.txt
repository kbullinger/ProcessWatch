Overview

This service is designed to watch a set of processes and act if one or more is found to be Not Responding. 
If configured, the service will execute a .bat script specified by the user to automatically restart the processes. 
If configured, an alert will be emailed in the following cases:
1) If not configured to autorestart, an alert will be triggered notifying that one or more of the watched processes is Not 		Responding.
2) If configured to autorestart, an alert will be triggered notifying that one or more of the watched processes was Not 		Responding and all processes have been killed and restarted.
3) If configured to autorestart but not successful, an alert will be triggered notifying that one or more the watched processes 	was Not Responding and were not restarted successfully.
		
All informational and error logs will be sent to flat files. Only fatal errors will be sent via email, if email is configured.

Environment Setup
1) If not already installed, install .NET Framework 4.0
	Available here: https://www.microsoft.com/en-US/Download/confirmation.aspx?id=17718

2) In Program Files, create a new folder. This folder will hold the files required for the service to run.
	Name the folder the same as you plan to name the service (eg. ScaleWatcher)
	
3) Copy the files from ProcessWatch/App to the folder in Program Files. Make note of the location of
	ProcessWatch.exe .

Service Setup
1) Open the command prompt as Administrator.

2) Run the following command -> sc create [serviceName] binpath= "[path to ProcessWatch.exe]
	Example: sc create ScaleWatcher binpath= "C:\Program Files\ScaleWatcher\ProcessWatch.exe"

3) Open Services and locate the newly created service

4) Right click on the service and click Properties

5) Navigate to the Log On tab

6) Check the box for "Allow service to interact with Desktop" (This is what allows the service to open processes in the user's desktop)

App Config Setup
1) In the folder containing the program files for the service, locate the ProcessWatch.exe.config file.

2) Edit the config file to fit your needs.

GENERAL SETTINGS (Required)
	ApplicationName: Used in naming log files and email alerts. Should be the same as what you have named the service.
	
	StatusCheckIntervalMillis: Determines how frequently processes are checked in milliseconds
	
	AutoRestartOnFailure: When set to true, will execute the .bat script as specified by RestartScriptFileLocation after
	finding a Not Responding process.
	
	RestartScriptFileLocation: File path of .bat script that restarts the processes being watched.
	
	LogFileLocation: Location where flat log files should be output. These are automatically named using the ApplicationName
	property and the current date. This path must end with a '\'.
	
	RetainedLogFileCount: Specifies how many log files should be kept. Setting this value to 0 keeps all log files
	indefinitely.
	
	ProcessNames: Names of the processes to be watched. These names should NOT contain '.exe' and should be comma separated
	with no spaces.
		Eg. "notepad,notepad++"
	
EMAIL ALERT SETTINGS (If SendEmailAlerts is set to true, ALL settings in this section are required.)
	SendEmailAlerts: When set to true, the service will attempt to configure and send email alerts when 
	watched processes enter a Not Responding state. 
	
	ToEmailAddresses: Specifies recipients for email alerts. These values should be comma separated with no spaces.
		Eg. "john@doe.com,jane@doe.com"
	
	FromEmailAddress: Specifies the sender for email alerts. 
	
	EmailLoginName: The username/email address used to authenticate FromEmailAddress. 
	
	EmailLoginPassword: The password used to authenticate FromEmailAddress.
	
	EmailEnableSsl: Set to true if the outgoing mail server requires SSL.
	
	MailServerAddress: Outgoing email server address. Eg. smtp.gmail.com
	
	MailServerPort: Output going email server port. Eg. 25
	
	EmailSendInterval: Frequency the service should check for and send alerts via email in milliseconds.
	
	EmailBatchPostingLimit: Specifies the maximum number of alerts that should be queued before triggering an email, 		regardless of EmailSendInterval.
	
Start Up
1) Open Services

2) Locate the newly created service (eg. ScaleWatcher)

3) Right click on the service and choose 'Start'

4) To help validate that the service has started successfully, open Windows Event Log and look for 
logs created by the service. If errors have occurred, they should show here.

5) Check the log file location specified to make sure the log file was created/updated at service startup.
If not, the service likely didn't start successfully.
