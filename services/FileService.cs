using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace tricep.cli.services;

public class FileServiceBase
{
    public const string ModuleDirectoryName = "modules";
    public const string LiveDirectoryName = "live";
    public const string TemplateDirectoryName = "templates";

    public DirectoryInfo ModulesDirectory { get; set; }
    public DirectoryInfo LiveDirectory { get; set; }
    public DirectoryInfo TemplatesDirectory { get; set; }
    public DirectoryInfo RootDirectory { get; set; }

    public FileServiceBase()
    {
        RootDirectory = new DirectoryInfo(".");
        ModulesDirectory = new DirectoryInfo(ModuleDirectoryName);
        LiveDirectory = new DirectoryInfo(LiveDirectoryName);
        TemplatesDirectory = new DirectoryInfo(TemplateDirectoryName);
    }


}

public class MonoRepoFileService : FileServiceBase
{
    class DirectoryMissingException : Exception
    {
        public DirectoryMissingException(DirectoryInfo directory) : base($"{directory.Name} is missing, expected at path {directory.FullName}") { }
    }
    class DirectoryExistsException : Exception
    {
        public DirectoryExistsException(DirectoryInfo directory) : base($"{directory.Name} already exists, expected path {directory.FullName} to be non-existent") { }
    }

    public void Init(List<string>? environments)
    {
        if (ModulesDirectory.Exists) throw new DirectoryExistsException(ModulesDirectory);
        if (LiveDirectory.Exists) throw new DirectoryExistsException(LiveDirectory);
        if (TemplatesDirectory.Exists) throw new DirectoryExistsException(TemplatesDirectory);

        ModulesDirectory.Create();
        LiveDirectory.Create();
        TemplatesDirectory.Create();

        environments ??= new List<string> { "dev", "test", "prod" };

        foreach (var env in environments)
        {
            LiveDirectory.CreateSubdirectory(env);
        }
    }
    public void AssertFolderStructure()
    {
        if (!ModulesDirectory.Exists) throw new DirectoryMissingException(ModulesDirectory);
        if (!LiveDirectory.Exists) throw new DirectoryMissingException(LiveDirectory);
        if (!TemplatesDirectory.Exists) throw new DirectoryMissingException(TemplatesDirectory);
    }

    public void NewModule(string category, string name)
    {
        AssertFolderStructure();

        var categoryDir = ModulesDirectory.CreateSubdirectory(category);

        string fileName = $"{name}.bicep";

        using var fileWriter = File.CreateText(Path.Combine(categoryDir.FullName, fileName));
        fileWriter.WriteLine($"//[tricep]");
        fileWriter.WriteLine($"//type={ModuleDirectoryName}");
        fileWriter.WriteLine($"//[tricep]");
        fileWriter.WriteLine();
        fileWriter.WriteLine("param location string = resourceGroup().location");
    }

    public List<FileInfo> NewLive(string domain, string filename, List<string>? environments, bool skipTemplate)
    {
        AssertFolderStructure();
        
        FileInfo? templateFile = null;
        
        if(!skipTemplate)
        {
            templateFile = NewTemplate(domain, filename);
        }
        
        List<FileInfo> createdFiles = new();

        foreach (var env in LiveDirectory.GetDirectories())
        {
            if (environments is not null)
            {
                if (!environments.Contains(env.Name)) continue;
            }

            var projectDir = env.CreateSubdirectory(domain);

            string fileName = $"{filename}.bicep";
            string filePath = Path.Combine(projectDir.FullName, fileName);
            using var fileWriter = File.CreateText(filePath);

            fileWriter.WriteLine($"//[tricep]");
            fileWriter.WriteLine($"//type={LiveDirectory}");
            fileWriter.WriteLine($"//env={env.Name}");
            fileWriter.WriteLine($"//[tricep]");
            fileWriter.WriteLine();
            fileWriter.WriteLine("param location string = resourceGroup().location");

            if(templateFile is not null)
            {
                fileWriter.WriteLine();
                var relative = Path.GetRelativePath(projectDir.FullName, templateFile.FullName);
                fileWriter.WriteLine($"module template '{relative}' = {{");
                fileWriter.WriteLine($"\tname: '{filename}'");
                fileWriter.WriteLine($"\tparams: {{");
                fileWriter.WriteLine($"\t\tlocation: location");
                fileWriter.WriteLine($"\t}}");
                fileWriter.WriteLine($"}}");
            }

            createdFiles.Add(new FileInfo(filePath));
        }
        return createdFiles;
    }

    public FileInfo NewTemplate(string project, string template)
    {
        AssertFolderStructure();

        var projectDir = TemplatesDirectory.CreateSubdirectory(project);

        string fileName = $"{template}.bicep";
        var filePath = Path.Combine(projectDir.FullName, fileName);
        using var fileWriter = File.CreateText(filePath);

        fileWriter.WriteLine($"//[tricep]");
        fileWriter.WriteLine($"//type={TemplateDirectoryName}");
        fileWriter.WriteLine($"//[tricep]");
        fileWriter.WriteLine();
        fileWriter.WriteLine("param location string = resourceGroup().location");

        return new FileInfo(filePath);
    }
}