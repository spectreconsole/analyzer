namespace Spectre.Console.Analyzer;

/// <summary>
/// Analyzer to enforce the use of MarkupLineInterpolated over MarkupLine when using string interpolation.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UseMarkupLineInterpolatedAnalyzer : SpectreAnalyzer
{
    private const string MarkupLine = "MarkupLine";

    private static readonly DiagnosticDescriptor _diagnosticDescriptor =
        Descriptors.S1030_AvoidInterpolationInMarkupLine;

    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_diagnosticDescriptor);

    /// <inheritdoc />
    protected override void AnalyzeCompilation(CompilationStartAnalysisContext compilationStartContext)
    {
        var spectreConsoleType =
            compilationStartContext.Compilation.GetTypeByMetadataName("Spectre.Console.AnsiConsole");
        var spectreConsoleInterface =
            compilationStartContext.Compilation.GetTypeByMetadataName("Spectre.Console.AnsiConsoleExtensions");

        compilationStartContext.RegisterOperationAction(
            context =>
            {
                var invocationOperation = (IInvocationOperation)context.Operation;

                // if this operation isn't an invocation against MarkupLine stop analyzing and return;
                if (!invocationOperation.TargetMethod.Name.Equals(MarkupLine))
                {
                    return;
                }

                int argumentIndex = -1;

                if (invocationOperation.TargetMethod.ContainingType.Equals(
                        spectreConsoleType,
                        SymbolEqualityComparer.Default))
                {
                    // This is a call to AnsiConsole.MarkupLine
                    argumentIndex = 0;
                }
                else if (invocationOperation.TargetMethod.ContainingType.Equals(
                             spectreConsoleInterface,
                             SymbolEqualityComparer.Default))
                {
                    // This is a call to AnsiConsoleExtensions.MarkupLine
                    argumentIndex = 1;
                }

                if (argumentIndex == -1)
                {
                    return;
                }

                // if there are no arguments stop analyzing and return
                if (invocationOperation.Arguments.Length <= argumentIndex)
                {
                    return;
                }

                var argument = invocationOperation.Arguments[argumentIndex].Value;
                if (argument is not IInterpolatedStringOperation interpolatedString)
                {
                    return;
                }

                var hasInterpolation = interpolatedString.Parts.OfType<IInterpolationOperation>().Any();
                if (!hasInterpolation)
                {
                    return; // no actual interpolation in the string, future analyzer to suggest MarkupLine
                }

                // Here it is MarkupLine with string interpolation.
                var displayString = SymbolDisplay.ToDisplayString(
                    invocationOperation.TargetMethod,
                    SymbolDisplayFormat.CSharpShortErrorMessageFormat
                        .WithParameterOptions(SymbolDisplayParameterOptions.None)
                        .WithGenericsOptions(SymbolDisplayGenericsOptions.None));

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        _diagnosticDescriptor,
                        invocationOperation.Syntax.GetLocation(),
                        displayString));
            }, OperationKind.Invocation);
    }
}