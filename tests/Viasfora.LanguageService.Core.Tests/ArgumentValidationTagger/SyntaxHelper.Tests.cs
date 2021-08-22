using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Winterdom.Viasfora.LanguageService.Core.Tests.ArgumentValidationTagger {
  public class SyntaxHelperTests {

    protected void VerifyCSharpIsIfArgumentThrowSyntaxStatement(string input, int line, int column, bool expectation) {
      var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(input);
      var source = tree.GetText();
      var root = tree.GetRoot();
      var inputSpan = TextSpan.FromBounds(source.Lines[line - 1].Start + column, source.Lines[line - 1].Start + column);

      var node = root.FindNode(inputSpan);
      bool result = CSharp.ArgumentValidationTagger.SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatement(node);

      if ( result != expectation ) {
        if ( expectation ) {
          Assert.True(result, "Expected IsIfArgumentThrowSyntaxStatement=True");
        } else {
          Assert.False(result, "Expected IsIfArgumentThrowSyntaxStatement=False");
        }
      }

    }

    protected void VerifyBasicIsIfArgumentThrowSyntaxStatement(string input, int line, int column, bool expectation) {
      var tree = Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree.ParseText(input);
      var source = tree.GetText();
      var root = tree.GetRoot();
      var inputSpan = TextSpan.FromBounds(source.Lines[line - 1].Start + column, source.Lines[line - 1].Start + column);

      var node = root.FindNode(inputSpan);
      bool result = Basic.ArgumentValidationTagger.SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatement(node);

      if ( result != expectation ) {
        if ( expectation ) {
          Assert.True(result, "Expected IsIfArgumentThrowSyntaxStatement=True");
        } else {
          Assert.False(result, "Expected IsIfArgumentThrowSyntaxStatement=False");
        }
      }
    }
  }
}