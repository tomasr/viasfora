using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Winterdom.Viasfora.LanguageService.Core {
  public class RoslynException : Exception {

    public SyntaxNode SyntaxNode { get; private set; }
    public TextSpan TextSpan { get; private set; }

    private RoslynException(string message) : base(message) { }
    private RoslynException(string message, Exception innerException) : base(message, innerException) { }

    public static RoslynException Create(
      string message,
      SyntaxNode node,
      TextSpan span,
      Exception innerException = null
    ) {
      if ( String.IsNullOrEmpty(message) ) throw new ArgumentException($"{nameof(message)} must not be empty.", nameof(message));
      if ( node == null ) throw new ArgumentNullException(nameof(node));

      string textSpanText = null;
      if (!span.IsEmpty) {
        textSpanText = $"Span: {span.ToString()}\nSpanValue: `{node.FindNode(span)?.GetText()}`";
      }

      message += $"\n\nSyntaxNode: `{node.ToFullString()}`({node.GetType().FullName}{textSpanText})";

      var ex = new RoslynException(message, innerException);
      ex.SyntaxNode = node;
      ex.TextSpan = span;
      return ex;
    }

    public static RoslynException Create(
      string message,
      SyntaxNode node,
      Exception innerException = null
    ) {
      return Create(message, node, new TextSpan(), innerException);
    }

  }
}
