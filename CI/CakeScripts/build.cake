#addin "Cake.Npm&version=0.17.0"
#tool "nuget:?package=NUnit.ConsoleRunner&version=3.10.0"




string rootPath = @"..\..\";
string FPS_ClientPath = rootPath +  @"SourceCodes\FiscalPrinterSimulatorClient";
string FPS_ServiceSolutionPath = rootPath + @"SourceCodes\FiscalPrinterSimulatorService\FiscalPrinterSimulator.sln";
string WIX_InstallerSolutionPath = @"..\WixInstaller\FiscalPrinterSimulatorInstaller.sln";
string targetPlatform = Argument ("targetPlatform","x86");
string configuration = Argument ("configuration","Release");
bool verbosity = HasArgument ("verbosity");
bool skipBuildFPS_Client = HasArgument ("skip-client-build");
string ReleaseVersion =  Argument("releaseVersion","1.0.");
string buildNumber = Argument("buildNumber","0");

Task ("Init")
    .Does (() => {
        Information ("Start to build Fiscal Printer Simulator Instalator.");
        Information ("Platform: "+targetPlatform);
        Information ("Configuration: "+configuration);
        Information ("SkipBuildingClient: "+skipBuildFPS_Client);
        Information ("Release Version: "+ ReleaseVersion+ buildNumber);
        Information ("Build Number: "+ buildNumber);
    });

Task ("Update Fiscal Printer Simulator Version")
    .WithCriteria(!skipBuildFPS_Client)
    .IsDependentOn("Init")
    .Does(()=>{
        string text = System.IO.File.ReadAllText(FPS_ClientPath + @"\package.json");
        text = new System.Text.RegularExpressions.Regex("\"version\"\\:\\s?\"(.*)\"").Replace(text, $"\"version\": \"{ReleaseVersion}{buildNumber}\"");
        System.IO.File.WriteAllText(FPS_ClientPath + @"\package.json", text);
    });

Task ("Build Fiscal Printer Simulator Client")
    .WithCriteria(!skipBuildFPS_Client)
    .IsDependentOn ("Update Fiscal Printer Simulator Version")
    .Does (() => {
        NpmInstall(new NpmInstallSettings () {
            Production = false,
            LogLevel = Cake.Npm.NpmLogLevel.Verbose,
            WorkingDirectory = FPS_ClientPath
        });
        NpmRunScript("test:cov",settings => settings.FromPath (FPS_ClientPath));
        NpmRunScript("electron-pack-"+ targetPlatform, settings => settings.FromPath (FPS_ClientPath));
    });

Task ("Build Fiscal Printer Simulator Service")
    .IsDependentOn ("Build Fiscal Printer Simulator Client")
    .Does (() => {
        var dotnetBuildVerbosityType  = "n"; // " --verbosity " + verbosity ? "n" : "m" +
        var dotnetBuildCommand = $"build {FPS_ServiceSolutionPath} --configuration {configuration} --verbosity {dotnetBuildVerbosityType} /p:Platform={targetPlatform}";
                
        StartProcess("dotnet",dotnetBuildCommand);
        DotNetCoreTest(FPS_ServiceSolutionPath);
    });

Task ("Build Fiscal Printer Simulator Installer")
    .IsDependentOn ("Build Fiscal Printer Simulator Service")
    .Does (() => {              
            var msBuildTarget = targetPlatform == "x64" ? MSBuildPlatform.x64 : MSBuildPlatform.x86;
            var msBuildVerbosity = verbosity ? Verbosity.Verbose : Verbosity.Minimal;

        MSBuild(WIX_InstallerSolutionPath, configurator => 
                configurator
                .SetConfiguration(configuration)
                .SetVerbosity(msBuildVerbosity)
                .UseToolVersion(MSBuildToolVersion.VS2019)
                .SetMSBuildPlatform(msBuildTarget)
                .WithProperty("Version",ReleaseVersion+buildNumber)
                .WithProperty("BuildNumber",buildNumber)
                .WithProperty("Platform",targetPlatform)
            );

       
    });

Task ("EndBuild")
    .IsDependentOn ("Build Fiscal Printer Simulator Installer")
    .Does (() => {
        Information ("Work on build Fiscal Printer Simulator Instalator done.");
    });
RunTarget ("EndBuild");