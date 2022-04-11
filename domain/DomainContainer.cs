namespace tricep.cli.services;

public class DomainContainer : DirectoryMetaData
{
    public List<Domain> Domains { get; }
    public string Name { get; }
    public DomainContainer(DirectoryInfo info) : base(info)
    {
        Domains = new();
        Name = info.Name;
        Build();
    }

    private void Build()
    {
        foreach (var domain in Info.GetDirectories())
        {
            var domainDirectory = new Domain(domain);
            Domains.Add(domainDirectory);
        }
    }
}

