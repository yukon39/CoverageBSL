	
AttachAddIn("src\CoverageBSL\bin\Debug\net5.0\CoverageBSL.dll");
CoverageManager = New CoverageManager();

Version = CoverageManager.Configure("http://localhost:8888");
Message("Version: " + Version);

Result = CoverageManager.Attach("DefAlias", "");
Message("Result: " + Result);

Sleep(50000);

CoverageManager.Detach();