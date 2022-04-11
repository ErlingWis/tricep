


using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using tricep.cli.services;

namespace tricep.cli;

[Command("new module")]
public class NewModuleCommand : DomainFileCommand,ICommand
{
    private readonly MonoRepoFileService _fileService;

    public NewModuleCommand(MonoRepoFileService fileService)
    {
        _fileService = fileService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _fileService.NewModule(Domain!, Filename!);

        return default;
    }
}
