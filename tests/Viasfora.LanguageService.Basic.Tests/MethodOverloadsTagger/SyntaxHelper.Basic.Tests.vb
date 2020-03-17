
Imports Winterdom.Viasfora.LanguageService.Core.Tests.MethodOverloadsTagger
Imports Xunit

Namespace MethodOverloadsTagger

  Public Class SyntaxHelperBasic
    Inherits SyntaxHelperTests

    <Fact>
    Public Sub test_parameter_default()
      Dim test = "
Class MyClass

  Sub Foo()
    Foo(True)
  End Sub

  Sub Foo(v As Boolean)
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_with_me()
      Dim test = "
Class MyClass

  Sub Foo()
    Me.Foo(True)
  End Sub

  Sub Foo(v As Boolean)
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_with_return()
      Dim test = "
Class MyClass

  Function Foo() As Boolean
    Return Me.Foo(True)
  End Function

  Function Foo(v As Boolean) As Boolean
  End Function

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_calling_same_name_with_core_prefix()
      Dim test = "
Class MyClass

  Sub Foo()
    FooCore(True)
  End Sub

  Sub FooCore(v As Boolean)
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_with_null_checking()
      Dim test = "
Class MyClass

  Sub Foo(arg1 As String, arg2 As String)
    if arg1 Is Nothing Then Throw new ArgumentNullException(nameof(arg1))
    if arg2 Is Nothing Then Throw new ArgumentNullException(nameof(arg2))

    FooCore(True)
  End Sub

  Sub FooCore(v As Boolean)
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_generic()
      Dim test = "
Class MyClass

  Function Foo() As String
    Return Foo(Of String)()
  End Function

  Function Foo(Of T)() As T
    Return FooCore(Of T)()
  End Function

  Function FooCore(Of T)() As T
  End Function

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
      VerifyBasicIsMethodOverload(test, line:=9, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_async()
      Dim test = "
Class MyClass

  Async Function Foo() As Task
    Await Foo(True)
  End Function

  Async Function Foo(v As Boolean)
  End Function

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_async_return()
      Dim test = "
Class MyClass

  Async Function Foo() As Task(Of Boolean)
    Return Await Foo(True)
  End Function

  Async Function Foo(v As Boolean) As Task(Of Boolean)
  End Function

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=True)
    End Sub

    <Fact>
    Public Sub test_skip_empty()
      Dim test = "
Class MyClass

  Sub Foo()

  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_must_override()
      Dim test = "
MustInherit Class MyClass

  MustOverride Sub Foo()

End Class
"
      VerifyBasicIsMethodOverload(test, line:=4, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_interface()
      Dim test = "
Interface MyClass

  Sub Foo()

End Class
"
      VerifyBasicIsMethodOverload(test, line:=4, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_multiple_statements()
      Dim test = "
Class MyClass

  Sub Foo()
    Bar()
    Baz()
  End Sub

  Sub Bar()
  End Sub
  
  Sub Baz()
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_calling_another_class()
      Dim test = "
Class MyClass

  Sub Foo()
    Console.WriteLine()
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_skip_expression()
      Dim test = "
Class MyClass

  Sub Foo()
    bar = 42
  End Sub
  Private bar As Integer

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=False)
    End Sub

    <Fact>
    Public Sub test_throw_statement()
      Dim test = "
Class MyClass

  Sub Foo()
    Throw New NotImplementedException()
  End Sub

End Class
"
      VerifyBasicIsMethodOverload(test, line:=5, column:=5, expectation:=False)
    End Sub

  End Class
End Namespace
