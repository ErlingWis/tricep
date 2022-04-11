namespace tricep.cli.services;

public class DirectoryMetaData
{
    public DirectoryInfo Info { get; set; }
    public DirectoryMetaData(DirectoryInfo info)
    {
        Info = info;
    }
}

