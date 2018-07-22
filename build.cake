var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = Directory("./Web/bin") + Directory(configuration);
var publishDir = Directory("./docs");
var slnFile = "./BlazeDDD.sln";
var repoName = "BlazeDDD";
var webProject = "./Web/Web.csproj";

Task("Clean").Does(() =>
{
    CleanDirectory(buildDir);
    CleanDirectory(publishDir);
});

Task("Restore-NuGet-Packages").IsDependentOn("Clean").Does(() =>
{
    DotNetCoreRestore(slnFile);
});

Task("Build").IsDependentOn("Restore-NuGet-Packages").Does(() =>
{
    DotNetCoreBuild(slnFile);
});

Task("Run_Process").IsDependentOn("Build").Does(() =>
{
    DotNetCoreRun("./Process", "./Process");
});

Task("Publish-WebProject").IsDependentOn("Run_Process").Does(() =>
{
    var settings = new DotNetCorePublishSettings
     {
         Framework = "netstandard2.0",
         Configuration = "Release",
         OutputDirectory = "./.publish/"
     };

    DotNetCorePublish(webProject, settings);
});

Task("Move-Published-To-Docs").IsDependentOn("Publish-WebProject").Does(() =>
{
    CopyDirectory("./.publish/Web/dist", "./docs");
    var indexFile = "./docs/index.html";
    var content = System.IO.File.ReadAllText(indexFile);
    content = content.Replace("<base href=\"/\" />", "<base href=\"/" + repoName + "/\" />");

    System.IO.File.WriteAllText(indexFile, content);

    System.IO.File.WriteAllText("./docs/.nojekyll", "");
});

Task("Default").IsDependentOn("Move-Published-To-Docs");

RunTarget(target);
