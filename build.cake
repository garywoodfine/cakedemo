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
       DotNetCoreBuild(projJson);

    }
    else
    {
      // Use XBuild
     DotNetCoreBuild(projJson);
     
    }
    });

Task("Package")
    .IsDependentOn("Build")
    .Does(() => {
        var settings = new DotNetCorePackSettings
        {
            OutputDirectory = outputDir,
            NoBuild = true
        };

         DotNetCorePack(projJson, settings);

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