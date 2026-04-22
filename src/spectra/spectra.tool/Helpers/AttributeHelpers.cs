using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace spectra.tool.Helpers;

internal static class AttributeHelpers
{
    internal static AttributeSyntax? FindAttr(MethodDeclarationSyntax m)
    {
        foreach (AttributeListSyntax l in m.AttributeLists)
        {
            foreach (AttributeSyntax a in l.Attributes)
            {
                string n = a.Name.ToString();
                if (n is "BusinessStep" or "BusinessStepAttribute" 
                    || n.EndsWith(".BusinessStep") || n.EndsWith(".BusinessStepAttribute"))
                {
                    return a;
                }
            }
        }

        return null;
    }
}