using System.Text.RegularExpressions;

namespace tricep.cli.services;

public class BicepFile : FileMetaData
{

    public List<string> FileContent { get; set; }
    public List<FileInfo> References { get; set; }
    public BicepFile(FileInfo info) : base(info)
    {
        FileContent = new();
        FileContent = GetContent();
        References = GetReferences();
    }

    List<string> GetContent()
    {
        if (!Info.Exists)
        {
            return new();
        }

        return File.ReadLines(Info.FullName).ToList();
    }

    List<FileInfo> GetReferences()
    {
        Regex rex = new Regex("module\\W.+\\W'(.+)?'\\W=");

        List<FileInfo> refs = new();

        foreach(var line in FileContent)
        {
            var match = rex.Match(line);

            if(match.Success)
            {
                var path = match.Groups[1];

                var safePath = path.Value.Replace('/', Path.DirectorySeparatorChar);

                var refPath = Path.Combine(Info.Directory.FullName, safePath);

                FileInfo referencedFile = new(refPath);

                if(referencedFile.Exists)
                {
                    refs.Add(referencedFile);
                }
            }
        }

        return refs;
    }
}

