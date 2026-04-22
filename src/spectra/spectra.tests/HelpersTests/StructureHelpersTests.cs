using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using spectra.tool.Helpers;

namespace spectra.tests.HelpersTests;

public class StructureHelpersTests
{
    private static InvocationExpressionSyntax ParseInvocation(string expression)
    {
        SyntaxTree tree = CSharpSyntaxTree.ParseText($"{expression};");
        return tree.GetRoot()
                   .DescendantNodes()
                   .OfType<InvocationExpressionSyntax>()
                   .First();
    }
    
    [Fact]
    public void InvocationName_SimpleIdentifier_ReturnsMethodName()
    {
        InvocationExpressionSyntax inv = ParseInvocation("DoSomething()");
        Assert.Equal("DoSomething", StructureHelpers.InvocationName(inv));
    }

    [Fact]
    public void InvocationName_SimpleIdentifierWithArgs_ReturnsMethodName()
    {
        InvocationExpressionSyntax inv = ParseInvocation("Execute(1, 2, 3)");
        Assert.Equal("Execute", StructureHelpers.InvocationName(inv));
    }
    
    [Fact]
    public void InvocationName_MemberAccess_ReturnsMethodName()
    {
        InvocationExpressionSyntax inv = ParseInvocation("myObject.DoSomething()");
        Assert.Equal("DoSomething", StructureHelpers.InvocationName(inv));
    }

    [Fact]
    public void InvocationName_ChainedMemberAccess_ReturnsImmediateMethodName()
    {
        InvocationExpressionSyntax inv = ParseInvocation("a.b.Run()");
        Assert.Equal("Run", StructureHelpers.InvocationName(inv));
    }

    [Fact]
    public void InvocationName_StaticClassMemberAccess_ReturnsMethodName()
    {
        InvocationExpressionSyntax inv = ParseInvocation("MyService.Process(data)");
        Assert.Equal("Process", StructureHelpers.InvocationName(inv));
    }
    
    [Fact]
    public void InvocationName_ParenthesisedLambdaInvocation_ReturnsNull()
    {
        InvocationExpressionSyntax inv = ParseInvocation("(new Func<int>(() => 1))()");
        Assert.Null(StructureHelpers.InvocationName(inv));
    }

    [Theory]
    [InlineData("Foo()",             "Foo")]
    [InlineData("Bar(x, y)",         "Bar")]
    [InlineData("obj.Baz()",         "Baz")]
    [InlineData("svc.Run(a, b, c)",  "Run")]
    [InlineData("A.B.C()",           "C")]
    public void InvocationName_VariousCases_ReturnsExpectedName(string code, string expected)
    {
        InvocationExpressionSyntax inv = ParseInvocation(code);
        Assert.Equal(expected, StructureHelpers.InvocationName(inv));
    }
}