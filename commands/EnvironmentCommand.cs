using CliFx.Attributes;
using CliFx.Extensibility;
using System.Text.RegularExpressions;

namespace tricep.cli;

public abstract class EnvironmentCommand
{
    [CommandOption("environments", 'e', Description = "list of environments that you want configured", IsRequired = false)]
    public List<string>? Environments { get; set; }

}

public class NameValidator : BindingValidator<string>
{
    public override BindingValidationError? Validate(string? value)
    {
        if (value is null)
        {
            return new BindingValidationError("expected non-null value");
        }

        Regex reg = new("^[a-z0-9_]+$");
        var match = reg.Match(value);

        if (!match.Success)
        {
            return new BindingValidationError($"the value must only contain lowercase alphanumerics and underscores");
        }

        return null;
    }
}
public class FilenameValidator : NameValidator
{
    public override BindingValidationError? Validate(string? value)
    {
        var result = base.Validate(value);

        if (result is not null) return result;

        // this will probably never get hit because of the regex in the parent validator.
        if (value!.Contains(".bicep"))
        {
            return new BindingValidationError("the filename must not contain .bicep extension");
        }

        return null;
    }
}
public abstract class DomainFileCommand
{
    [CommandOption("domain", 'd', Description = "the file domain", IsRequired = true, Validators = new Type[] { typeof(NameValidator) })]
    public string? Domain { get; set; }

    [CommandOption("filename", 'f', Description = "the name of the file", IsRequired = true, Validators = new Type[] { typeof(FilenameValidator) })]
    public string? Filename { get; set; }
}