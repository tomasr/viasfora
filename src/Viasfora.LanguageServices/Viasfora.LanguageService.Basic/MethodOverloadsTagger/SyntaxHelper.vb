Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace MethodOverloadsTagger

  Public Class SyntaxHelper

    Public Shared Function IsMethodOverload(node As SyntaxNode) As Boolean
      If node Is Nothing Then
        Return False
      End If

      Dim methodBlockSyntax = node.FirstAncestorOrSelf(Of MethodBlockSyntax)()

      Return IsMethodOverload(methodBlockSyntax)
    End Function

    Public Shared Function IsMethodOverload(methodBlockSyntax As MethodBlockSyntax) As Boolean
      If methodBlockSyntax Is Nothing Then
        Return False
      End If

      If methodBlockSyntax.SubOrFunctionStatement Is Nothing Then
        Return False
      End If

      If methodBlockSyntax.Statements.Count = 0 Then
        Return False
      End If

      Dim statementList = methodBlockSyntax.Statements
      Dim parameterNames = methodBlockSyntax.SubOrFunctionStatement.ParameterList.Parameters _
        .Select(Function(x) x.Identifier.Identifier.ValueText)

      ' check argument validation
      For i = 0 To statementList.Count - 2
        Dim statementSyntax = statementList(i)
        If Not ArgumentValidationTagger.SyntaxHelper.IsAnyIfArgumentThrowSyntaxStatementCore(statementSyntax, parameterNames) Then
          Return False
        End If
      Next

      ' check last statement
      Dim lastStatement = statementList(statementList.Count - 1)
      Dim expressionSyntax As ExpressionSyntax = Nothing

      Dim lastReturnStatement = TryCast(lastStatement, ReturnStatementSyntax)
      If lastReturnStatement IsNot Nothing Then
        expressionSyntax = lastReturnStatement.Expression
      Else
        Dim expressionStatementSyntax = TryCast(lastStatement, ExpressionStatementSyntax)
        If expressionStatementSyntax IsNot Nothing Then
          expressionSyntax = expressionStatementSyntax.Expression
        End If
      End If

      If expressionSyntax Is Nothing Then
        Return False
      Else
        Return IsValidExpression(expressionSyntax)
      End If
    End Function

    Private Shared Function IsValidExpression(expressionSyntax As ExpressionSyntax) As Boolean
      Dim awaitExpressionSyntax = TryCast(expressionSyntax, AwaitExpressionSyntax)
      If awaitExpressionSyntax IsNot Nothing Then
        expressionSyntax = awaitExpressionSyntax.Expression
      End If

      Dim invocationExpressionSyntax = TryCast(expressionSyntax, InvocationExpressionSyntax)
      If invocationExpressionSyntax Is Nothing Then
        Return False
      End If

      Dim memberAccessExpressionSyntax = TryCast(invocationExpressionSyntax.Expression, MemberAccessExpressionSyntax)
      If memberAccessExpressionSyntax IsNot Nothing Then
        If TypeOf memberAccessExpressionSyntax.Expression IsNot MeExpressionSyntax Then
          Return False
        End If
      End If

      Return True
    End Function

  End Class

End Namespace
