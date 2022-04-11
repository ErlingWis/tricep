


using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using tricep.cli.services;

namespace tricep.cli;

[Command("new template")]
public class NewTemplateCommand : DomainFileCommand,ICommand
{
    private readonly MonoRepoFileService _fileService;

    public NewTemplateCommand(MonoRepoFileService fileService)
    {
        _fileService = fileService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _fileService.NewTemplate(Domain!, Filename!);

        return default;
    }
}
