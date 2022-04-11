


using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using System.Text.Json;
using tricep.cli.services;

namespace tricep.cli;

[Command("ref")]
public class RefCommand : EnvironmentCommand, ICommand
{
    [CommandOption("file", 'f', IsRequired = true, Description = "List of files to check references of. Full name with extension is supported")]
    public List<string> Files { get; set; } = new();

    [CommandOption("create-deployment-data", IsRequired = false)]
    public string? DeplyomentSystem { get; set; }
    
    /// <summary>
    /// never optimize :)))))))))
    /// </summary>
    public ValueTask ExecuteAsync(IConsole console)
    {
        Repo repo = Repo.BuildFromCurrentLocation();
        HashSet<BicepFile> live = new();

        foreach (var file in Files)
        {
            FileInfo target = new(file);

            if (!target.Exists)
            {
                console.Error.WriteLine($"the file {file} does not exist.");
                continue;
            }

            foreach (var templateDomain in repo.Templates.Domains)
            {
                foreach (var templateFile in templateDomain.Files)
                {
                    if (templateFile.References.Any(x => x.FullName == target.FullName))
                    {
                        foreach (var env in repo.Live.Environments)
                        {
                            if(Environments is not null)
                            {
                                if(!Environments.Contains(env.Info.Name))
                                {
                                    continue;
                                }
                            }

                            foreach (var envDomain in env.Domains)
                            {
                                foreach (var liveFile in envDomain.Files)
                                {
                                    if (liveFile.References.Any(x => x.FullName == templateFile.Info.FullName))
                                    {
                                        live.Add(liveFile);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var env in repo.Live.Environments)
            {
                if (Environments is not null)
                {
                    if (!Environments.Contains(env.Info.Name))
                    {
                        continue;
                    }
                }

                foreach (var envDomain in env.Domains)
                {
                    foreach (var liveFile in envDomain.Files)
                    {
                        if (liveFile.References.Any(x => x.FullName == target.FullName)) {
                            live.Add(liveFile);
                        }
                    }
                }
            }
        }
        Dictionary<string, Dictionary<string, string>> deploymentData = new();
        foreach (var file in live)
        {

            if(DeplyomentSystem == "ado")
            {
                Dictionary<string, string> data = new()
                {
                    { "RESOURCE_GROUP", $"{file.Info.Directory.Name}_{file.Info.Name}".Replace(".bicep", "") },
                    { "DEPLOYMENT_FILE", file.Info.FullName }
                };

                deploymentData[data["RESOURCE_GROUP"].ToUpper()] = data;
            } else
            {
                console.Output.WriteLine(Path.GetRelativePath(Environment.CurrentDirectory, file.Info.FullName));
            }
        }

        if(deploymentData.Count > 0)
        {
            console.Output.WriteLine(JsonSerializer.Serialize(deploymentData));
        }

        return default;
    }
}
