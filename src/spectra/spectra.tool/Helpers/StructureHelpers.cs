using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace spectra.tool.Helpers;

internal static class StructureHelpers
{
    internal static string? InvocationName(InvocationExpressionSyntax inv) => inv.Expression switch
    {
        IdentifierNameSyntax id => id.Identifier.Text,
        MemberAccessExpressionSyntax ma => ma.Name.Identifier.Text,
        _ => null
    };
}