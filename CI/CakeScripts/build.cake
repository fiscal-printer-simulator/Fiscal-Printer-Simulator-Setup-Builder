#addin "Cake.Npm&version=0.17.0"
string PFS_ClientPath = "../Sources/FiscalPrinterSimulatorClient";
var targetPlatform = Argument("targetPlatform", "x64");



Task ("Init")
    .Does (() => {
        Information ("Start to build Fiscal Printer Simulator Instalator!");
    });

Task("Install FP Simulator Client NPM Dependencies")
.IsDependentOn("Init")
.Does(() => {
        var settings = new NpmInstallSettings();
        settings.Production = false;
        settings.LogLevel = NpmLogLevel.Verbose;
        settings.WorkingDirectory = PFS_ClientPath;
        NpmInstall(settings);
});

Task("Build Exe Client")
.IsDependentOn("Install FP Simulator Client NPM Dependencies")
.Does(()=>{
    NpmRunScript("electron-pack-"+targetPlatform, settings => settings.FromPath("../Sources/FiscalPrinterSimulatorClient"));
});






Task ("EndBuild")
    .IsDependentOn ("Build Exe Client")
    .Does (() => {
        Information ("Work on build Fiscal Printer Simulator Instalator done.");
     });
RunTarget ("EndBuild");