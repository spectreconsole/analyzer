namespace Spectre.Console.Analyzer.Tests.Unit.Analyzers;

public class UseMarkupLineInterpolatedAnalyzerTests
{
    private static readonly DiagnosticResult _expectedDiagnostics = new(
        Descriptors.S1030_AvoidInterpolationInMarkupLine.Id,
        DiagnosticSeverity.Warning);

    [Fact]
    public async Task AnsiConsole_MarkupLine_With_Interpolation_Has_Warning()
    {
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        string world = ""World"";
        AnsiConsole.MarkupLine($""[bold]Hello, {world}[/]"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyAnalyzerAsync(Source, _expectedDiagnostics.WithLocation(9, 9));
    }

    [Fact]
    public async Task AnsiConsoleExtension_MarkupLine_With_Interpolation_Has_Warning()
    {
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        IAnsiConsole console = AnsiConsole.Console;
        string world = ""World"";
        console.MarkupLine($""[bold]Hello, {world}[/]"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyAnalyzerAsync(Source, _expectedDiagnostics.WithLocation(10, 9));
    }

    [Fact]
    public async Task MarkupLine_Without_Interpolation_Has_No_Warning()
    {
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {

        IAnsiConsole console = AnsiConsole.Console;
        console.MarkupLine(""[bold]Hello, world[/]"");
        AnsiConsole.MarkupLine(""[bold]Hello, world[/]"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyAnalyzerAsync(Source);
    }

    [Fact]
    public async Task MarkupLine_With_Interpolation_But_No_Replacement_Has_Warning()
    {
        // The only difference from the above test is that there is no replacement {..}
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        IAnsiConsole console = AnsiConsole.Console;
        console.MarkupLine($""[bold]Hello, world[/]"");
        AnsiConsole.MarkupLine($""[bold]Hello, world[/]"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyAnalyzerAsync(Source);
    }
}