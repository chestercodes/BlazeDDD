var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = Directory("./Web/bin") + Directory(configuration);
var publishDir = Directory("./docs");
var slnFile = "./BlazeDDD.sln";

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

Task("Publish-WebProject").IsDependentOn("Build").Does(() =>
{
    var settings = new DotNetCorePublishSettings
     {
         Framework = "netstandard2.0",
         Configuration = "Release",
         OutputDirectory = "./.publish/"
     };

    DotNetCorePublish("./Web/Web.csproj", settings);
});

Task("Move-Published-To-Docs").IsDependentOn("Publish-WebProject").Does(() =>
{
    CopyDirectory("./.publish/Web/dist", "./docs");
});

Task("Default").IsDependentOn("Move-Published-To-Docs");

RunTarget(target);