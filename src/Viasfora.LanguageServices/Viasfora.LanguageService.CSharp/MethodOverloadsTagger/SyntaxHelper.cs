using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Winterdom.Viasfora.LanguageService.CSharp.MethodOverloadsTagger {
  public static class SyntaxHelper {

    public static bool IsMethodOverload(SyntaxNode node) {
      if ( node == null )
        return false;

      var methodDeclarationSyntax = node.FirstAncestorOrSelf<MethodDeclarationSyntax>();
      if ( methodDeclarationSyntax == null )
        return false;

      return IsMethodOverload(methodDeclarationSyntax);
    }

    public static bool IsMethodOverload(MethodDeclarationSyntax methodDeclarationSyntax) {
      if ( methodDeclarationSyntax.Body == null || methodDeclarationSyntax.Body.Statements.Count == 0 )
        return false;

      var statementList = methodDeclarationSyntax.Body.Statements;
      var parameterNames = methodDeclarationSyntax.ParameterList.Parameters
        .Select(x => x.Identifier.ValueText);

      // check argument validation
      for (var i=0; i< statementList.Count -1; ++i ) {
        var statementSyntax = statementList[i];
        if ( !ArgumentValidationTagger.SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatementCore(statementSyntax, false, parameterNames))
          return false;
      }

      // check last statement
      var lastStatement = statementList[statementList.Count - 1];
      ExpressionSyntax expressionSyntax = null;

      if ( lastStatement is ReturnStatementSyntax returnStatementSyntax )
        expressionSyntax = returnStatementSyntax.Expression;
      else if ( lastStatement is ExpressionStatementSyntax expressionStatementSyntax )
        expressionSyntax = expressionStatementSyntax.Expression;


      if ( expressionSyntax == null )
        return false;
      else
        return IsValidExpression(expressionSyntax);
    }

    private static bool IsValidExpression(ExpressionSyntax expressionSyntax) {
      var awaitExpressionSyntax = expressionSyntax as AwaitExpressionSyntax;
      if ( awaitExpressionSyntax != null )
        expressionSyntax = awaitExpressionSyntax.Expression;

      var invocationExpressionSyntax = expressionSyntax as InvocationExpressionSyntax;
      if ( invocationExpressionSyntax == null )
        return false;

      var memberAccessExpression = invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
      if ( memberAccessExpression != null ) {
        if ( !(memberAccessExpression.Expression is ThisExpressionSyntax) )
          return false;
      }

      return true;
    }

  }
}
