using Microsoft.CodeAnalysis.Editing;

namespace Spectre.Console.Analyzer.CodeActions;

/// <summary>
/// Code action to switch to MarkupLineInterpolated.
/// </summary>
public class SwitchToMarkupLineInterpolatedAction : CodeAction
{
    private readonly Document _document;
    private readonly InvocationExpressionSyntax _originalInvocation;

    /// <summary>
    /// Initializes a new instance of the <see cref="SwitchToMarkupLineInterpolatedAction"/> class.
    /// </summary>
    /// <param name="document">Document to change.</param>
    /// <param name="originalInvocation">The method to change.</param>
    /// <param name="title">Title of the fix.</param>
    public SwitchToMarkupLineInterpolatedAction(Document document, InvocationExpressionSyntax originalInvocation, string title)
    {
        _document = document;
        _originalInvocation = originalInvocation;
        Title = title;
    }

    /// <inheritdoc />
    public override string Title { get; }

    /// <inheritdoc />
    public override string EquivalenceKey => Title;

    /// <inheritdoc />
    protected override async Task<Document> GetChangedDocumentAsync(CancellationToken cancellationToken)
    {
        var editor = await DocumentEditor.CreateAsync(_document, cancellationToken).ConfigureAwait(false);

        // Ensure the invocation is actually a method call on AnsiConsole
        if (_originalInvocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return _document;
        }

        // Replace MarkupLine with MarkupLineInterpolated
        var newMemberAccess = memberAccess.WithName(SyntaxFactory.IdentifierName("MarkupLineInterpolated"));

        // Create a new invocation with the updated method name
        var newInvocation = _originalInvocation.WithExpression(newMemberAccess);

        // Apply the fix by replacing the old invocation with the new one
        editor.ReplaceNode(_originalInvocation, newInvocation);

        return editor.GetChangedDocument();
    }
}