using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace Winterdom.Viasfora.LanguageService.Core.Tests.MethodOverloadsTagger {
  public class SyntaxHelperTests {

    protected void VerifyCSharpIsMethodOverload(string input, int line, int column, bool expectation) {
      var tree = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(input);
      var source = tree.GetText();
      var root = tree.GetRoot();
      var inputSpan = TextSpan.FromBounds(source.Lines[line - 1].Start + column, source.Lines[line - 1].Start + column);

      var node = root.FindNode(inputSpan);
      bool result = CSharp.MethodOverloadsTagger.SyntaxHelper.IsMethodOverload(node);

      if ( result != expectation ) {
        if ( expectation ) {
          Assert.True(result, "Expected IsMethodOverload=True");
        } else {
          Assert.False(result, "Expected IsMethodOverload=False");
        }
      }

    }

    protected void VerifyBasicIsMethodOverload(string input, int line, int column, bool expectation) {
      var tree = Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxTree.ParseText(input);
      var source = tree.GetText();
      var root = tree.GetRoot();
      var inputSpan = TextSpan.FromBounds(source.Lines[line - 1].Start + column, source.Lines[line - 1].Start + column);

      var node = root.FindNode(inputSpan);
      bool result = Basic.MethodOverloadsTagger.SyntaxHelper.IsMethodOverload(node);

      if ( result != expectation ) {
        if ( expectation ) {
          Assert.True(result, "Expected IsMethodOverload=True");
        } else {
          Assert.False(result, "Expected IsMethodOverload=False");
        }
      }
    }

  }
}
