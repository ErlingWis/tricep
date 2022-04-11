using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tricep.cli.services;

public class LiveDirectory : DirectoryMetaData
{
    public List<DomainContainer> Environments { get; set; }
    public LiveDirectory(DirectoryInfo info) : base(info)
    {
        Environments = new();
        Build();
    }

    public void Build()
    {
        foreach(var env in Info.GetDirectories())
        {
            var envDir = new DomainContainer(env);
            Environments.Add(envDir);
        }
    }
}

public class Repo
{
    class RepoNotFoundException : Exception
    {
        public RepoNotFoundException() : base("Current directory does not have the expected structure. Are you in the right directory?") { }
    }

    public DomainContainer Modules { get; }
    public DomainContainer Templates { get; }
    public LiveDirectory Live { get; }

    public Repo(DirectoryInfo modules, DirectoryInfo templates, DirectoryInfo live)
    {
        Modules = new(modules);
        Templates = new(templates);
        Live = new(live);
    }

    public static Repo BuildFromCurrentLocation()
    {
        DirectoryInfo modules = new(FileServiceBase.ModuleDirectoryName);
        DirectoryInfo templates = new(FileServiceBase.TemplateDirectoryName);
        DirectoryInfo live = new(FileServiceBase.LiveDirectoryName);
    
        if(!modules.Exists || !live.Exists || !templates.Exists)
        {
            throw new RepoNotFoundException();
        }

        return new Repo(modules, templates, live);
    
    }
}

