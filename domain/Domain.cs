namespace tricep.cli.services;

public class Domain : DirectoryMetaData
{
    public string Name { get; }
    public List<BicepFile> Files { get; }
    public Domain(DirectoryInfo info) : base(info)
    {
        Name = info.Name;
        Files = new();
        Build();
    }

    private void Build()
    {
        foreach (var file in Info.GetFiles())
        {
            var bicepFile = new BicepFile(file);
            Files.Add(bicepFile);
        }
    }
}

