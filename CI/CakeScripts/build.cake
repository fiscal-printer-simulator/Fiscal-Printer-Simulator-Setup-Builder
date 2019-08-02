#addin "Cake.Npm&version=0.17.0"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"




string rootPath = @"..\..\";
string PFS_ClientPath = rootPath +  @"SourceCodes\FiscalPrinterSimulatorClient";
string FPS_ServiceSolutionPath = rootPath + @"SourceCodes\FiscalPrinterSimulatorService\FiscalPrinterSimulator.sln";
string WIX_InstallerSolutionPath = @"..\WixInstaller\FiscalPrinterSimulatorInstaller.sln";
string targetPlatform = Argument ("targetPlatform","x86");
string configuration = Argument ("configuration","Release");
bool verbosity = HasArgument ("verbosity");
bool skipBuildFPS_Client = HasArgument ("skip-client-build");
string ReleaseVersion =  Argument("releaseVersion","1.0.0");
string buildNumber = Argument("buildNumber","0");

Task ("Init")
    .Does (() => {
        Information ("Start to build Fiscal Printer Simulator Instalator.");
        Information ("Platform: "+targetPlatform);
        Information ("Configuration: "+configuration);
        Information ("SkipBuildingClient: "+skipBuildFPS_Client);
    });

Task ("Build Fiscal Printer Simulator Client")
    .WithCriteria(!skipBuildFPS_Client)
    .IsDependentOn ("Init")
    .Does (() => {
        NpmInstall(new NpmInstallSettings () {
            Production = false,
            LogLevel = Cake.Npm.NpmLogLevel.Verbose,
            WorkingDirectory = PFS_ClientPath
        });
        NpmRunScript("test:cov",settings => settings.FromPath (PFS_ClientPath));
        NpmRunScript("electron-pack-"+ targetPlatform, settings => settings.FromPath (PFS_ClientPath));
    });

Task ("Build Fiscal Printer Simulator Service")
    .IsDependentOn ("Build Fiscal Printer Simulator Client")
    .Does (() => {
        MSBuild(FPS_ServiceSolutionPath, new MSBuildSettings {
                Restore = true,
                Verbosity = verbosity ? Verbosity.Verbose : Verbosity.Minimal,
                ToolVersion = MSBuildToolVersion.VS2019,
                Configuration =configuration,
                PlatformTarget = targetPlatform == "x64" ? PlatformTarget.x64 : PlatformTarget.x86
        });
      
        var testAssemblies = GetFiles(rootPath + @"Resources\TestBinaries\*.Tests.dll");
        NUnit3(testAssemblies);
    });

Task ("Build Fiscal Printer Simulator Installer")
    .IsDependentOn ("Build Fiscal Printer Simulator Service")
    .Does (() => {
        MSBuild(WIX_InstallerSolutionPath, new MSBuildSettings {
                Verbosity = verbosity ? Verbosity.Verbose : Verbosity.Minimal,
                ToolVersion = MSBuildToolVersion.VS2019,
                Configuration =configuration,
                PlatformTarget = targetPlatform == "x64" ? PlatformTarget.x64 : PlatformTarget.x86,
                 EnvironmentVariables = new Dictionary<string, string>{
                    {"AppVersion",ReleaseVersion },
                    {"BuildNumber", buildNumber }
                }
        });
    });

Task ("EndBuild")
    .IsDependentOn ("Build Fiscal Printer Simulator Installer")
    .Does (() => {
        Information ("Work on build Fiscal Printer Simulator Instalator done.");
    });
RunTarget ("EndBuild");