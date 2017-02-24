#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var projDir = "./src/demoapi/";
var binDir = String.Concat(projDir,"bin" ) ;
var projJson = String.Concat(projDir , "project.json");
var solutionFile = "";
var outputDir = Directory(binDir) + Directory(configuration);

var buildSettings = new DotNetCoreBuildSettings
     {
         Framework = "netcoreapp1.1",
         Configuration = "Release",
         OutputDirectory = outputDir
     };
 var packSettings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true
        };


//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    if (DirectoryExists(outputDir))
        {
            DeleteDirectory(outputDir, recursive:true);
        }
});

Task("Restore")
    .Does(() => {
        DotNetCoreRestore(projDir);
    });

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() => {
         if(IsRunningOnWindows())
    {
      // Use MSBuild
      //MSBuild(solutionFile , settings => settings.SetConfiguration(configuration));
       DotNetCoreBuild(projJson, buildSettings);

    }
    else
    {
      // Use XBuild
     DotNetCoreBuild(projJson, buildSettings);
     
    }
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
       
         DotNetCorePack(projJson, packSettings);

    });


//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);