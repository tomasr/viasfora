using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Winterdom.Viasfora.Rainbow;
using RSpan = Microsoft.CodeAnalysis.Text.TextSpan;

namespace Winterdom.Viasfora.Languages.Roslyn {

  public class RoslynCSharpBraceExtractor : IBraceExtractor {
    private ITextBuffer buffer;
    private CSharpParseOptions options;
    private SyntaxTree syntaxTree;

    public String BraceList {
      get { return "()[]{}"; }
    }

    public RoslynCSharpBraceExtractor(ITextBuffer buffer) {
      this.buffer = buffer;
      this.options = new CSharpParseOptions(
        documentationMode: DocumentationMode.None,
        kind: SourceCodeKind.Regular
        );
      this.buffer.Changing += OnBufferChanging;
      this.syntaxTree = null;
    }

    private void OnBufferChanging(object sender, TextContentChangingEventArgs e) {
    }

    public void Reset() {
      this.syntaxTree = null;
    }

    public IEnumerable<CharPos> Extract(Util.ITextChars text) {
      ParseIfNeeded();

      SyntaxNode node = null;
      if ( !syntaxTree.TryGetRoot(out node) )
        yield break;

      int start = text.AbsolutePosition;
      text.SkipRemainder();
      int end = text.AbsolutePosition;
      if ( end == start ) {
        yield break;
      }
      var rspan = new RSpan(start, end - start);
      var candidates = from token in node.DescendantTokens(rspan, descendIntoTrivia: false)
                       where IsTokenInSpan(token, rspan) 
                          && IsBrace(token)
                          && !token.IsMissing
                       select token;

      foreach ( var c in candidates ) {
        yield return new CharPos(CharFromToken(c), c.Span.Start);
      }

    }

    private bool IsTokenInSpan(SyntaxToken token, RSpan rspan) {
      return token.Span.Start >= rspan.Start &&
             token.Span.Start <= rspan.End;
    }

    private char CharFromToken(SyntaxToken token) {
      if ( token.IsKind(SyntaxKind.OpenParenToken) )
        return '(';
      if ( token.IsKind(SyntaxKind.CloseParenToken) )
        return ')';
      if ( token.IsKind(SyntaxKind.OpenBraceToken) )
        return '{';
      if ( token.IsKind(SyntaxKind.CloseBraceToken) )
        return '}';
      if ( token.IsKind(SyntaxKind.OpenBracketToken) )
        return '[';
      if ( token.IsKind(SyntaxKind.CloseBracketToken) )
        return ']';
      throw new InvalidOperationException("Unexpected token type: " + token.Kind());
    }

    private bool IsBrace(SyntaxToken token) {
      return token.IsKind(SyntaxKind.OpenParenToken)
          || token.IsKind(SyntaxKind.CloseParenToken)
          || token.IsKind(SyntaxKind.OpenBraceToken)
          || token.IsKind(SyntaxKind.CloseBraceToken)
          || token.IsKind(SyntaxKind.OpenBracketToken)
          || token.IsKind(SyntaxKind.CloseBracketToken);
    }

    private void ParseIfNeeded() {
      if ( syntaxTree != null )
        return;
      // TODO: Avoid parsing the entire thing every time!!!!
      String text = this.buffer.CurrentSnapshot.GetText();
      syntaxTree = CSharpSyntaxTree.ParseText(text, this.options);
    }
  }
}
