using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Winterdom.Viasfora.LanguageService.CSharp.ArgumentValidationTagger {
  public static class SyntaxHelper {

    public static bool IsAnyIfArgumentThrowSyntaxStatement(SyntaxNode node) {
      if ( node == null )
        return false;

      var baseMethodDeclarationSyntax = node.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
      if ( baseMethodDeclarationSyntax == null || baseMethodDeclarationSyntax == node)
        return false;

      if ( baseMethodDeclarationSyntax.Body == null || baseMethodDeclarationSyntax.Body.Statements == null )
        return false;

      foreach (var statementSyntax in baseMethodDeclarationSyntax.Body.Statements) {
        if (node.Span.IntersectsWith(statementSyntax.Span)) {
          return IsAnyIfArgumentThrowSyntaxStatement(statementSyntax);
        }
      }

      return false;
    }

    public static bool IsAnyIfArgumentThrowSyntaxStatement(StatementSyntax statementSyntax) {
      if ( statementSyntax == null ) 
        return false;

      var baseMethodDeclarationSyntax = statementSyntax.FirstAncestorOrSelf<BaseMethodDeclarationSyntax>();
      if ( baseMethodDeclarationSyntax == null )
        return false;

      bool isCtor = baseMethodDeclarationSyntax is ConstructorDeclarationSyntax;

      var parameterNames = baseMethodDeclarationSyntax.ParameterList.Parameters
        .Select(x => x.Identifier.ValueText);

      if ( !IsAnyIfArgumentThrowSyntaxStatementCore(statementSyntax, isCtor, parameterNames) )
        return false;

      if ( !CheckPreviouslyStatements(statementSyntax, isCtor, parameterNames) )
        return false;

      return true;
    }

    private static bool CheckPreviouslyStatements(StatementSyntax statementSyntax, bool isCtor, IEnumerable<string> parameterNames) {
      var blockSyntax = statementSyntax.Parent as BlockSyntax;
      if ( blockSyntax == null || blockSyntax.Statements.Count < 2 ) 
        return true;

      var statementList = blockSyntax.Statements;

      var startIdx = statementList.IndexOf(statementSyntax);
      for (var i=startIdx - 1; i>-1; --i ) {
        if ( !IsAnyIfArgumentThrowSyntaxStatementCore(statementList[i], isCtor, parameterNames) )
          return false;
      }

      return true;
    }

    private static bool IsAnyIfArgumentThrowSyntaxStatementCore(StatementSyntax statementSyntax, bool isCtor, IEnumerable<string> parameterNames) {
      var ifStatementSyntax = statementSyntax as IfStatementSyntax;
      if ( ifStatementSyntax != null )
        return IsIfArgumentThrowSyntaxStatementCore(ifStatementSyntax, parameterNames);

      if ( isCtor ) {
        if ( IsAssignmentCoalescThrowStatement(statementSyntax, parameterNames) )
          return true;
      }

      return false;
    }

    private static bool IsIfArgumentThrowSyntaxStatementCore(IfStatementSyntax ifStatementSyntax, IEnumerable<string> parameterNames) {
      // check if statement with only one statement in true part
      StatementSyntax innerIfStatementSyntax;
      if ( ifStatementSyntax.Statement is BlockSyntax blockSyntax ) {
        if ( blockSyntax.Statements.Count != 1 )
          return false;
        innerIfStatementSyntax = blockSyntax.Statements[0];
      } else
        innerIfStatementSyntax = ifStatementSyntax.Statement;

      // the inner if statement must be a throw Statement
      var throwStatementSyntax = innerIfStatementSyntax as ThrowStatementSyntax;
      if ( throwStatementSyntax == null )
        return false;

      // the if expression must contain at least one parameter(identifername)
      if ( !ExpressionContainsParameter(ifStatementSyntax.Condition, parameterNames) && !ExpressionContainsParameter(throwStatementSyntax.Expression, parameterNames) )
        return false;
      
      return true;
    }

    private static bool ExpressionContainsParameter(ExpressionSyntax expressionSyntax, IEnumerable<string> parameterNames) {
      if ( expressionSyntax == null ) 
        return false;

      // checks if the givven expression contains any ParameterNames. 
      // Skips MemberAccessExpressions like `this.XXX`

      var identifiers = expressionSyntax.DescendantNodesAndSelf()
        .OfType<IdentifierNameSyntax>()
      ;

      return identifiers.Any(x => {
        var memberAccessExpressionSyntax = x.FirstAncestorOrSelf<MemberAccessExpressionSyntax>();
        if ( memberAccessExpressionSyntax != null && memberAccessExpressionSyntax.SpanStart >= expressionSyntax.SpanStart ) {
          if (memberAccessExpressionSyntax.Expression != x)
            return false;
        }

        var invocationExpression = x.FirstAncestorOrSelf<InvocationExpressionSyntax>();
        if ( invocationExpression != null && invocationExpression.SpanStart >= expressionSyntax.SpanStart )
          return false;

        return parameterNames.Contains(x.Identifier.ValueText);
      });
    }

    private static bool IsAssignmentCoalescThrowStatement(StatementSyntax statementSyntax, IEnumerable<string> parameterNames) {
      var expressionStatementSyntax = statementSyntax as ExpressionStatementSyntax;
      if ( expressionStatementSyntax == null )
        return false;

      var assignmentExpressionSyntax = expressionStatementSyntax.Expression as AssignmentExpressionSyntax;
      if ( assignmentExpressionSyntax == null )
        return false;

      var binaryExpressionSyntax = assignmentExpressionSyntax.Right as BinaryExpressionSyntax;
      if ( binaryExpressionSyntax == null )
        return false;

      if ( binaryExpressionSyntax.Kind() != Microsoft.CodeAnalysis.CSharp.SyntaxKind.CoalesceExpression )
        return false;

      var leftIdentifierNameSyntax = binaryExpressionSyntax.Left as IdentifierNameSyntax;
      if ( leftIdentifierNameSyntax == null )
        return false;
      if ( !parameterNames.Contains(leftIdentifierNameSyntax.Identifier.ValueText) )
        return false;

      // skip check right throw ...
      // `this.xyz = xyz ?? xyzDefault;` should be also a valid expression 

      return true;
    }

  }
}
