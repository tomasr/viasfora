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
    private static readonly CharPos[] empty = new CharPos[0];
    private CharList<CharPos> braces;
    private bool needsParsing;

    public String BraceList {
      get { return "()[]{}"; }
    }

    public int ReparseWindow {
      get { return 1; }
    }

    public RoslynCSharpBraceExtractor(ITextBuffer buffer) {
      this.buffer = buffer;
      this.buffer.Changing += OnBufferChanging;
      this.options = CSharpParseOptions
                    .Default
                    .WithDocumentationMode(DocumentationMode.None);
      this.braces = new CharList<CharPos>();
      this.needsParsing = true;
    }

    private void OnBufferChanging(object sender, TextContentChangingEventArgs e) {
      this.needsParsing = true;
    }

    public void Reset() {
    }

    public IEnumerable<CharPos> Extract(Util.ITextChars text) {
      if ( text.EndOfLine )
        return empty;

      ReparseIfNeeded();

      int start = text.AbsolutePosition;
      text.SkipRemainder();
      int end = text.AbsolutePosition;

      return this.braces.FindInRange(start, end);
    }

    private void ReparseIfNeeded() {
      if ( !needsParsing )
        return;

      this.braces = new CharList<CharPos>();

      // TODO: Get the existing parsed tree from
      // VS2015 if we have it
      String toParse = this.buffer.CurrentSnapshot.GetText();
      var parsed = CSharpSyntaxTree.ParseText(toParse, this.options);
      var rootNode = parsed.GetRoot();

      //var parsed = SyntaxFactory.ParseTokens(toParse, 0, 0, options);
      var tokens = from token in rootNode.DescendantTokens(null, true)
                   where IsBrace(token) && !token.IsMissing
                   select token;

      foreach ( var token in tokens ) {
        this.braces.Add(new CharPos(
          CharFromToken(token),
          token.Span.Start
        ));
      }
      this.needsParsing = false;
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
  }
}
