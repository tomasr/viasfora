
Imports Winterdom.Viasfora.LanguageService.Core.Tests.ArgumentValidationTagger
Imports Xunit

Namespace ArgumentValidationTagger
  Public Class SyntaxHelperBasic
    Inherits SyntaxHelperTests

    <Fact>
    Public Sub test_simple_if_throw_statement()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As String)
    If arg1 Is Nothing Then Throw new ArgumentNullException(Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_ctor_simple_if_throw_statement()
      Dim test = "
Class MyClass

  Sub New(arg1 As String)
    If arg1 Is Nothing Then Throw new ArgumentNullException(Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_multiple_if_throw_statement()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As String, arg2 As String)
    If arg1 Is Nothing Then Throw new ArgumentNullException(Nameof(arg1))
    If arg2 Is Nothing Then Throw new ArgumentNullException(Nameof(arg2))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=6, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_ctor_multple_if_throw_statement()
      Dim test = "
Class MyClass

  Sub New(arg1 As String, arg2 As String)
    If arg1 Is Nothing Then Throw new ArgumentNullException(Nameof(arg1))
    If arg2 Is Nothing Then Throw new ArgumentNullException(Nameof(arg2))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=6, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_skip_if_previous_is_not_argument_throw_syntax()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As String, arg2 As String)
    If arg1 Is Nothing Then Throw new ArgumentNullException(Nameof(arg1))
    Foo()
    If arg2 Is Nothing Then Throw new ArgumentNullException(Nameof(arg2))
  End Sub

  Sub Foo()
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=7, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_argument_out_of_range_exception()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If arg1 < 0 Then Throw new ArgumentOutOfRangeException(Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_argument_exception()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If arg1 < 0 Then Throw new ArgumentException(""lorem ipsum"", Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_skip_expression_function_calls()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If IsSomething(arg1) Then Throw new ArgumentException(""lorem ipsum"")
  End Sub

  Function IsSomething(arg As Integer) As Boolean
  End Function

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_member_access_with_same_name_like_paramter()
      Dim test = "
Class MyClass

  Sub myfn(myparam As Integer)
    If Me.myparam = 0 Then Throw new InvalidOperationException()
  End Sub

  private myparam As Integer

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_multiple_if_statements()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As String)
    If arg1 Is Nothing Then
      Dim x = 5
      Throw new ArgumentNullException(nameof(arg1))
    End If
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_throw_function_syntax()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If arg1 < 0 Then Throw ThrowHelper.ArgumentOutOfRange(nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_bool_expression()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Boolean)
    If arg1 Then Throw new ArgumentException(Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_multiline_if_statement()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If arg1 < 0 Then 
      Throw new ArgumentException(""lorem ipsum"", Nameof(arg1))
    End If
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=6, column:=10, expectation:=True)
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=7, column:=7, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_skip_multiline_if_statement_inside_another_block()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If something1 = something2 Then
      If arg1 < 0 Then Throw new ArgumentException(""lorem ipsum"", Nameof(arg1))
    End If
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=6, column:=10, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_method_itself()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As Integer)
    If arg1 < 0 Then Throw new ArgumentException(""lorem ipsum"", Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=4, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_member_access_parameter()
      Dim test = "
Class MyClass

  Sub myfn(arg1 As String)
    If arg1.Length = 0 Then Throw new ArgumentException(Nameof(arg1))
  End Sub

End Class
"
      VerifyBasicIsIfArgumentThrowSyntaxStatement(test, line:=5, column:=5, expectation:=True)
    End Sub

  End Class
End Namespace