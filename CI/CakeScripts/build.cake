#addin "Cake.Npm&version=0.17.0"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"

string rootPath = @"..\..\";
string PFS_ClientPath = rootPath +  @"SourceCodes\FiscalPrinterSimulatorClient";
string FPS_ServiceSolutionPath = rootPath + @"SourceCodes\FiscalPrinterSimulatorService\FiscalPrinterSimulator.sln";
string WIX_InstallerSolutionPath = @"..\WixInstaller\FiscalPrinterSimulatorInstaller.sln";
var targetPlatform = Argument ("targetPlatform","x64");
var verbosity = Argument ("verboseBuild","true");

var skipBuildFPS_Client = true;

Task ("Init")
    .Does (() => {
        Information ("Start to build Fiscal Printer Simulator Instalator.");
        Information ($"Building Instalator for platform: "+targetPlatform+".");
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
        NpmRunScript("electron-pack-"+ targetPlatform, settings => settings.FromPath (PFS_ClientPath));
    });

Task ("Build Fiscal Printer Simulator Service")
    .IsDependentOn ("Build Fiscal Printer Simulator Client")
    .Does (() => {
        MSBuild(FPS_ServiceSolutionPath, new MSBuildSettings {
                Restore = true,
                Verbosity = verbosity =="true" ? Verbosity.Verbose : Verbosity.Minimal,
                ToolVersion = MSBuildToolVersion.VS2019,
                Configuration ="Release",
                PlatformTarget = targetPlatform == "x64" ? PlatformTarget.x64 : PlatformTarget.x86
        });
      
        var testAssemblies = GetFiles(rootPath + @"Resources\TestBinaries\*.Tests.dll");
        NUnit3(testAssemblies);
    });

Task ("Build Fiscal Printer Simulator Installer")
    .IsDependentOn ("Build Fiscal Printer Simulator Service")
    .Does (() => {
        MSBuild(WIX_InstallerSolutionPath, new MSBuildSettings {
                Verbosity = verbosity =="true" ? Verbosity.Verbose : Verbosity.Minimal,
                ToolVersion = MSBuildToolVersion.VS2019,
                Configuration ="Release",
                PlatformTarget = targetPlatform == "x64" ? PlatformTarget.x64 : PlatformTarget.x86
        });
    });

Task ("EndBuild")
    .IsDependentOn ("Build Fiscal Printer Simulator Installer")
    .Does (() => {
        Information ("Work on build Fiscal Printer Simulator Instalator done.");
    });
RunTarget ("EndBuild");