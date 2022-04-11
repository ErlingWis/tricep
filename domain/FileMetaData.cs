namespace tricep.cli.services;

public class FileMetaData
{
    public FileInfo Info { get; set; }
    public FileMetaData(FileInfo info)
    {
        Info = info;
    }
}

