namespace Spectre.Console.Analyzer.FixProviders;

/// <summary>
/// Fix provider to change MarkupLine calls to MarkupLineInterpolated calls.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public class UseMarkupLineInterpolatedFix : CodeFixProvider
{
    /// <inheritdoc />
    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        Descriptors.S1030_AvoidInterpolationInMarkupLine.Id);

    /// <inheritdoc />
    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

        var diagnostic = context.Diagnostics[0];
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (root?.FindNode(diagnosticSpan) is not InvocationExpressionSyntax invocationExpression)
        {
            return;
        }

        context.RegisterCodeFix(
            new SwitchToMarkupLineInterpolatedAction(
                context.Document,
                invocationExpression,
                "Convert MarkupLine to MarkupLineInterpolated"),
            context.Diagnostics);
    }
}