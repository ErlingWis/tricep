


using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using tricep.cli.services;

namespace tricep.cli;

[Command("init")]
public class InitCommand : EnvironmentCommand, ICommand
{
    private readonly MonoRepoFileService _fileService;


    public InitCommand(MonoRepoFileService fileService)
    {
        _fileService = fileService;
    }

    public ValueTask ExecuteAsync(IConsole console)
    {
        _fileService.Init(Environments);

        console.Output.WriteLine($"created new monorepo");

        return default;
    }
}