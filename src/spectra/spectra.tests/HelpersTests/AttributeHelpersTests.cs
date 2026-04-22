using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using spectra.tool.Helpers;

namespace spectra.tests.HelpersTests;

public class AttributeHelpersTests
{
    private static MethodDeclarationSyntax ParseMethod(string methodCode)
    {
        string wrapped = $"class C {{ {methodCode} }}";
        SyntaxTree tree = CSharpSyntaxTree.ParseText(wrapped);
        return tree.GetRoot()
            .DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .First();
    }

    [Fact]
    public void FindAttr_ShortName_ReturnsAttribute()
    {
        MethodDeclarationSyntax method = ParseMethod("[BusinessStep] public void MyMethod() {}");
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_FullName_ReturnsAttribute()
    {
        MethodDeclarationSyntax method = ParseMethod("[BusinessStepAttribute] public void MyMethod() {}");
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_QualifiedShortName_ReturnsAttribute()
    {
        MethodDeclarationSyntax method = ParseMethod("[MyNamespace.BusinessStep] public void MyMethod() {}");
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_QualifiedFullName_ReturnsAttribute()
    {
        MethodDeclarationSyntax method = ParseMethod("[MyNamespace.BusinessStepAttribute] public void MyMethod() {}");
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_DeeplyQualifiedName_ReturnsAttribute()
    {
        MethodDeclarationSyntax method = ParseMethod("[My.Deep.Namespace.BusinessStepAttribute] public void MyMethod() {}");
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_MultipleAttributes_ReturnsBusinessStep()
    {
        MethodDeclarationSyntax method = ParseMethod("[Obsolete][BusinessStep][SomeOther] public void MyMethod() {}");
        AttributeSyntax? attr = AttributeHelpers.FindAttr(method);
        Assert.NotNull(attr);
        Assert.Equal("BusinessStep", attr!.Name.ToString());
    }

    [Fact]
    public void FindAttr_BusinessStepNotFirst_StillFindsIt()
    {
        MethodDeclarationSyntax method = ParseMethod("[Obsolete][SomeOther][BusinessStepAttribute] public void MyMethod() {}");
        AttributeSyntax? attr = AttributeHelpers.FindAttr(method);
        Assert.NotNull(attr);
        Assert.Equal("BusinessStepAttribute", attr!.Name.ToString());
    }
    
    [Fact]
    public void FindAttr_NoAttributes_ReturnsNull()
    {
        MethodDeclarationSyntax method = ParseMethod("public void MyMethod() {}");
        Assert.Null(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_UnrelatedAttribute_ReturnsNull()
    {
        MethodDeclarationSyntax method = ParseMethod("[Obsolete] public void MyMethod() {}");
        Assert.Null(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_PartialNameMatch_DoesNotMatch()
    {
        MethodDeclarationSyntax method = ParseMethod("[NotBusinessStep] public void MyMethod() {}");
        Assert.Null(AttributeHelpers.FindAttr(method));
    }

    [Fact]
    public void FindAttr_PartialQualifiedNameMatch_DoesNotMatch()
    {
        MethodDeclarationSyntax method = ParseMethod("[SomeBusinessStep] public void MyMethod() {}");
        Assert.Null(AttributeHelpers.FindAttr(method));
    }

    [Theory]
    [InlineData("[BusinessStep] public void M() {}")]
    [InlineData("[BusinessStepAttribute] public void M() {}")]
    [InlineData("[Ns.BusinessStep] public void M() {}")]
    [InlineData("[Ns.BusinessStepAttribute] public void M() {}")]
    [InlineData("[A.B.C.BusinessStep] public void M() {}")]
    [InlineData("[A.B.C.BusinessStepAttribute] public void M() {}")]
    public void FindAttr_AllValidForms_ReturnsNonNull(string methodCode)
    {
        MethodDeclarationSyntax method = ParseMethod(methodCode);
        Assert.NotNull(AttributeHelpers.FindAttr(method));
    }
}