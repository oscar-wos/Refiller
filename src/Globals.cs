namespace Refiller;

public partial class Refiller
{
    public RefillerConfig Config { get; set; } = new();
    public override string ModuleName => "Refiller";
    public override string ModuleAuthor => "github.com/oscar-wos/Refiller";
    public override string ModuleVersion => "1.0.1";
}