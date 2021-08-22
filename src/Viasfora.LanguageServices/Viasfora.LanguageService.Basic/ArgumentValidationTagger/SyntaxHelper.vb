Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace ArgumentValidationTagger

  Public Class SyntaxHelper


    Public Shared Function IsAnyIfArgumentThrowSyntaxStatement(node As SyntaxNode) As Boolean
      If node Is Nothing Then
        Return False
      End If

      Dim methodBlockBaseSyntax = node.FirstAncestorOrSelf(Of MethodBlockBaseSyntax)()
      If methodBlockBaseSyntax Is Nothing OrElse methodBlockBaseSyntax Is node Then
        Return False
      End If

      Dim accessorBlockSyntax = TryCast(methodBlockBaseSyntax, AccessorBlockSyntax)
      If accessorBlockSyntax IsNot Nothing Then
        ' Skip Properties
        Return False
      End If

      For Each statementSyntax In methodBlockBaseSyntax.Statements
        If node.Span.IntersectsWith(statementSyntax.Span) Then
          Return IsAnyIfArgumentThrowSyntaxStatement(statementSyntax)
        End If
      Next

      Return False
    End Function

    Public Shared Function IsAnyIfArgumentThrowSyntaxStatement(statementSyntax As StatementSyntax) As Boolean
      If statementSyntax Is Nothing Then
        Return False
      End If

      Dim methodBlockBaseSyntax = statementSyntax.FirstAncestorOrSelf(Of MethodBlockBaseSyntax)()
      If methodBlockBaseSyntax Is Nothing Then
        Return False
      End If

      Dim methodBaseSyntax = methodBlockBaseSyntax.BlockStatement
      If methodBaseSyntax Is Nothing Then
        Return False
      End If

      If methodBaseSyntax.ParameterList Is Nothing Then
        Return False
      End If

      Dim parameterNames = methodBaseSyntax.ParameterList.Parameters _
        .Select(Function(x) x.Identifier.Identifier.ValueText)

      If Not IsAnyIfArgumentThrowSyntaxStatementCore(statementSyntax, parameterNames) Then
        Return False
      End If

      If Not CheckPreviouslyStatements(statementSyntax, methodBlockBaseSyntax.Statements, parameterNames) Then
        Return False
      End If

      Return True
    End Function

    Friend Shared Function IsAnyIfArgumentThrowSyntaxStatementCore(statementSyntax As StatementSyntax, parameterNames As IEnumerable(Of String)) As Boolean
      Dim multiLineIfBlockSyntax = TryCast(statementSyntax, MultiLineIfBlockSyntax)
      If multiLineIfBlockSyntax IsNot Nothing Then
        Return IsIfArgumentThrowSyntaxStatementCore(multiLineIfBlockSyntax, parameterNames)
      End If

      Dim singleLineIfStatementSyntax = TryCast(statementSyntax, SingleLineIfStatementSyntax)
      If singleLineIfStatementSyntax IsNot Nothing Then
        Return IsIfArgumentThrowSyntaxStatementCore(singleLineIfStatementSyntax, parameterNames)
      End If

      Return False
    End Function

    Private Shared Function IsIfArgumentThrowSyntaxStatementCore(multiLineIfBlockSyntax As MultiLineIfBlockSyntax, parameterNames As IEnumerable(Of String)) As Boolean
      If multiLineIfBlockSyntax.Statements.Count <> 1 Then
        Return False
      End If

      Return IsIfArgumentThrowSyntaxStatementCore(multiLineIfBlockSyntax.IfStatement.Condition, multiLineIfBlockSyntax.Statements(0), parameterNames)
    End Function

    Private Shared Function IsIfArgumentThrowSyntaxStatementCore(singleLineIfStatementSyntax As SingleLineIfStatementSyntax, parameterNames As IEnumerable(Of String)) As Boolean
      If singleLineIfStatementSyntax.Statements.Count <> 1 Then
        Return False
      End If

      Return IsIfArgumentThrowSyntaxStatementCore(singleLineIfStatementSyntax.Condition, singleLineIfStatementSyntax.Statements(0), parameterNames)
    End Function

    Private Shared Function IsIfArgumentThrowSyntaxStatementCore(condition As ExpressionSyntax, trueStatementSyntax As StatementSyntax, parameterNames As IEnumerable(Of String)) As Boolean
      If condition Is Nothing Then
        Return False
      End If

      If trueStatementSyntax Is Nothing Then
        Return False
      End If

      Dim throwStatement = TryCast(trueStatementSyntax, ThrowStatementSyntax)
      If throwStatement Is Nothing Then
        Return False
      End If

      ' the if expression must contain at least one parameter(identifername)
      If Not ExpressionContainsParameter(condition, parameterNames) AndAlso
          Not ExpressionContainsParameter(throwStatement.Expression, parameterNames) Then

        Return False
      End If

      Return True
    End Function

    Private Shared Function ExpressionContainsParameter(expressionSyntax As ExpressionSyntax, parameterNames As IEnumerable(Of String)) As Boolean
      If expressionSyntax Is Nothing Then
        Return False
      End If

      Dim identifiers = expressionSyntax.DescendantNodesAndSelf() _
        .OfType(Of IdentifierNameSyntax)

      Return identifiers.Any(
        Function(x)
          Dim memberAccessExpressionSyntax = x.FirstAncestorOrSelf(Of MemberAccessExpressionSyntax)()
          If memberAccessExpressionSyntax IsNot Nothing AndAlso memberAccessExpressionSyntax.SpanStart >= expressionSyntax.SpanStart Then
            If memberAccessExpressionSyntax.Expression IsNot x Then
              Return False
            End If
          End If

          Dim invocationExpression = x.FirstAncestorOrSelf(Of InvocationExpressionSyntax)()
          If invocationExpression IsNot Nothing AndAlso invocationExpression.SpanStart >= expressionSyntax.SpanStart Then
            Return False
          End If

          Return parameterNames.Contains(x.Identifier.ValueText, StringComparer.OrdinalIgnoreCase)
        End Function
      )
    End Function

    Private Shared Function CheckPreviouslyStatements(statementSyntax As StatementSyntax, parentStatementList As SyntaxList(Of StatementSyntax), parameterNames As IEnumerable(Of String)) As Boolean
      If parentStatementList.Count < 2 Then
        Return True
      End If

      Dim startIdx = parentStatementList.IndexOf(statementSyntax)
      For i = startIdx - 1 To 0 Step -1
        If Not IsAnyIfArgumentThrowSyntaxStatementCore(parentStatementList(i), parameterNames) Then
          Return False
        End If
      Next

      Return True
    End Function

  End Class

End Namespace

