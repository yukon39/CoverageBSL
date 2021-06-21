#Use coveragebsl

Procedure RunVRunner(DebugUrl)
	
	DebugParamater = New Array();
	DebugParamater.Add("/Debug");
	DebugParamater.Add("-http");
	DebugParamater.Add("-attach");
	DebugParamater.Add("/DEBUGGERURL");
	DebugParamater.Add(DebugUrl);
	
	StrDebugParameter = StrConcat(DebugParamater, " ");
	
	CommandLine = New Array();
	CommandLine.Add("vrunner.bat");
	CommandLine.Add("run");
	CommandLine.Add("--additional");
	CommandLine.Add(StrTemplate("""%1""", StrDebugParameter));
	
	CommandLineStr = StrConcat(CommandLine, " ");
	
	Message("CommandLine: " + CommandLineStr);
	Process = CreateProcess(CommandLineStr);
	Process.Start();
	Process.WaitForExit();
	
EndProcedure

DebugUrl = "http://localhost:1550";
InfobaseAlias = "DefAlias"; // Default value
DebuggerPassword = "";

CoverageManager = New CoverageManager(DebugUrl);

CoverageManager.TestConnection();

APIVersion = CoverageManager.APIVersion();
Message("APIVersion: " + APIVersion);

Session = CoverageManager.NewCoverageSession("DefAlias");

Session.Attach(DebuggerPassword);

MeasureID = Session.StartCoverageCapture();
Message("MeasureID: " + MeasureID);

RunVRunner(DebugUrl);

CoverageData = Session.StopCoverageCapture();

Session.Detach();

Message("TotalDurability: " + CoverageData.TotalDurability);
Message("Data.Count: " + CoverageData.Data.Count());

Writer = New JSONWriter();
Writer.OpenFile("platformCoverage.json");
CoverageData.SerializeJSON(Writer);
Writer.Close();