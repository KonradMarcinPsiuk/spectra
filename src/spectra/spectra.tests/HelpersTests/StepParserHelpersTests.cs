using System.Text.RegularExpressions;
using spectra.tool.Helpers;

namespace spectra.tests.HelpersTests;

public class StepParserHelpersTests
{
    private static Match GetMatch(string input) =>
        StepParserHelpers.StepComment.Match(input);

    [Theory]
    [InlineData("// [step: MyStep] Some description", "MyStep",       "Some description")]
    [InlineData("// [step:MyStep] Some description",  "MyStep",       "Some description")]  
    [InlineData("// [ step : MyStep ] Desc",          "MyStep",       "Desc")]    
    [InlineData("//[step:MyStep]Desc",                "MyStep",       "Desc")]    
    [InlineData("// [step: My Step Name] Do thing",   "My Step Name", "Do thing")]
    [InlineData("// [step: Foo]",                     "Foo",          "")]        
    [InlineData("// [step: Foo] ",                    "Foo",          "")]        
    [InlineData("// [STEP: Foo] desc",                "Foo",          "desc")]
    public void Match_ValidInput_CapturesNameAndDescription(
        string input, string expectedName, string expectedDesc)
    {
        Match match = GetMatch(input);

        Assert.True(match.Success);
        Assert.Equal(expectedName, match.Groups["stepname"].Value);
        Assert.Equal(expectedDesc, match.Groups["stepdesc"].Value.Trim());
    }

    [Theory]
    [InlineData("// [step:]")]             
    [InlineData("// [step:   ]")]          
    [InlineData("// step: Foo] desc")]     
    [InlineData("// [step: Foo desc")]     
    [InlineData("/* [step: Foo] desc */")] 
    [InlineData("[step: Foo] desc")]       
    [InlineData("# [step: Foo] desc")]     
    [InlineData("Hello world")]            
    [InlineData("")]                       
    public void Match_InvalidInput_DoesNotMatch(string input)
    {
        Match match = GetMatch(input);

        Assert.False(match.Success);
    }
    
    [Fact]
    public void Regex_HasExpectedNamedGroups()
    {
        string[] groupNames = StepParserHelpers.StepComment.GetGroupNames();

        Assert.Contains("stepname", groupNames);
        Assert.Contains("stepdesc", groupNames);
    }

    [Fact]
    public void Regex_IsCompiled()
    {
        Assert.Equal(
            RegexOptions.Compiled,
            StepParserHelpers.StepComment.Options & RegexOptions.Compiled);
    } 

    [Fact]
    public void StepName_IsTrimmed_WhenSurroundedBySpaces()
    {
        Match match = GetMatch("//  [  step  :   My Step   ]  Some desc  ");

        Assert.True(match.Success);
        Assert.Equal("My Step", match.Groups["stepname"].Value);
    }

    [Fact]
    public void Description_CanContainSpecialCharacters()
    {
        Match match = GetMatch("// [step: Init] Load file from C:\\path\\to\\file.txt");

        Assert.True(match.Success);
        Assert.Equal("Init", match.Groups["stepname"].Value);
        Assert.Contains("C:\\path\\to\\file.txt", match.Groups["stepdesc"].Value);
    }

    [Fact]
    public void Description_CanBeMultipleWords()
    {
        Match match = GetMatch("// [step: Checkout] User clicks the checkout button");

        Assert.True(match.Success);
        Assert.Equal("User clicks the checkout button", match.Groups["stepdesc"].Value);
    }

    [Fact]
    public void Match_OnlyFirstOccurrence_WhenMultipleStepsOnOneLine()
    {
        Match match = GetMatch("// [step: First] desc // [step: Second] desc2");

        Assert.True(match.Success);
        Assert.Equal("First", match.Groups["stepname"].Value);
    }
}