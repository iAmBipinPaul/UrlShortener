
using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;


class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.LogoutFromDockerHub);
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    [GitRepository] readonly GitRepository GitRepository;
    [Parameter("Docker Registry Password")] readonly string DockerRegistryPassword;
    const string User = "iambipinpaul";
    string Tag = "";
    bool IsMaster;
    AbsolutePath ProjectPath => RootDirectory / "src"/"API"/"UrlShortener.Server"/ "UrlShortener.Server.csproj";
    
    Target CheckDockerVersion => _ => _
        .DependsOn(CheckBranch)
        .Executes(() =>
        {
            DockerTasks.DockerVersion();
        });

    Target BuildAndPublishDockerImage => _ => _
        .DependsOn(LoginIntoDockerHub)
        .DependsOn(DetermineTag)
        .Executes(() =>
        {
           
           DotNetTasks.DotNetPublish(s => s
               .SetProject(ProjectPath)
               .SetConfiguration(Configuration)
               .SetProperty("PublishProfile", "DefaultContainer")
               .SetProperty("ContainerImageTag", Tag));
           
        });

    Target LoginIntoDockerHub => _ => _
        .DependsOn(CheckDockerVersion)
        .Executes(() =>
        {
            DockerTasks.DockerLogin(l => l
                .SetServer("ghcr.io")
                .SetUsername(User)
                .SetPassword(DockerRegistryPassword)
            );
        });


    Target DetermineTag => _ => _
        .Executes(() =>
        {
            Tag = "dev";
            if (GitRepository.Branch != null)
            {
                string branch = GitRepository.Branch.Split("/").Last();
                IsMaster = branch == "main";
                if (IsMaster)
                {
                    Tag = "latest";
                }
                else
                {
                    long buildNumber = 0;
                    if (IsServerBuild)
                    {
                        buildNumber = GitHubActions.Instance.RunId;
                    }
                    Tag = $"{branch}-{buildNumber}";
                }
            }
        });
    
    Target LogoutFromDockerHub => _ => _
        .DependsOn(BuildAndPublishDockerImage)
        .Executes(() =>
        {
            DockerTasks.DockerLogout();
        });

    Target CheckBranch => _ => _
        .Executes(() =>
        {
            Console.WriteLine(GitRepository.Branch);
        });
}