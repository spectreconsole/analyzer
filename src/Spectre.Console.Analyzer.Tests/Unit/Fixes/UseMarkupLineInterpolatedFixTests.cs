namespace Spectre.Console.Analyzer.Tests.Unit.Fixes;

public class UseMarkupLineInterpolatedFixTests
{
    private static readonly DiagnosticResult _expectedDiagnostic = new(
        Descriptors.S1030_AvoidInterpolationInMarkupLine.Id,
        DiagnosticSeverity.Warning);

    [Fact]
    public async Task AnsiConsoleExtensions_MarkupLine_replaced_with_MarkupLineInterpolated()
    {
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        IAnsiConsole console = AnsiConsole.Console;
        var world = ""world"";
        console.MarkupLine($""Hello, {world}"");
    }
}";

        const string FixedSource = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        IAnsiConsole console = AnsiConsole.Console;
        var world = ""world"";
        console.MarkupLineInterpolated($""Hello, {world}"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyCodeFixAsync(Source, _expectedDiagnostic.WithLocation(10, 9), FixedSource);
    }

    [Fact]
    public async Task AnsiConsole_MarkupLine_replaced_with_MarkupLineInterpolated()
    {
        const string Source = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        var world = ""world"";
        AnsiConsole.MarkupLine($""Hello, {world}"");
    }
}";

        const string FixedSource = @"
using Spectre.Console;

class TestClass
{
    void TestMethod()
    {
        var world = ""world"";
        AnsiConsole.MarkupLineInterpolated($""Hello, {world}"");
    }
}";

        await SpectreAnalyzerVerifier<UseMarkupLineInterpolatedAnalyzer>
            .VerifyCodeFixAsync(Source, _expectedDiagnostic.WithLocation(9, 9), FixedSource);
    }
}