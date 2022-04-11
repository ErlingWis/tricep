


using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using tricep.cli.services;

namespace tricep.cli;

[Command("new live")]
public class NewLiveCommand : DomainFileCommand, ICommand
{
    private readonly MonoRepoFileService _fileService;

    public NewLiveCommand(MonoRepoFileService fileService)
    {
        _fileService = fileService;
    }

    [CommandOption("skip-template", Description = "skip creation of template", IsRequired = false)]
    public bool SkipTemplate { get; set; } = false;

    public ValueTask ExecuteAsync(IConsole console)
    {
        _fileService.NewLive(Domain!, Filename!, null, SkipTemplate);

        return default;
    }
}
