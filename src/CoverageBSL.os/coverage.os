	
AttachAddIn("src\CoverageBSL\bin\Debug\net48\CoverageBSL.dll");
CoverageManager = New CoverageManager();

Version = CoverageManager.Configure("http://localhost:8888");
Message("Version: " + Version);

Session = CoverageManager.NewCoverageSession("DefAlias");
Session.Attach("");

MeasureID = Session.StartPerformanceMeasure();
Message("MeasureID: " + MeasureID);

Sleep(50000);

CoverageData = Session.StopPerformanceMeasure();
Message("TotalDurability: " + CoverageData.TotalDurability);
Message("Data.Count: " + CoverageData.Data.Count());

Session.Detach();