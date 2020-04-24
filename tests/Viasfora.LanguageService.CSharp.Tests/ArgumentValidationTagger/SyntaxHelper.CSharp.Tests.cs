using Winterdom.Viasfora.LanguageService.Core.Tests.ArgumentValidationTagger;
using Xunit;

namespace Winterdom.Viasfora.LanguageService.CSharp.Tests.ArgumentValidationTagger {
  public class SyntaxHelperCSharpTests : SyntaxHelperTests {


    [Fact]
    public void test_simple_if_throw_statement() {
      var test = @"
class MyTest {

  void myfn(string arg1) {
    if (arg1 == null) throw new ArgumentNullException(nameof(of arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_ctor_simple_if_throw_statement() {
      var test = @"
class MyTest {

  MyTest(string arg1) {
    if (arg1 == null) throw new ArgumentNullException(nameof(of arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_ctor_simple_coalesce_throw_statement() {
      var test = @"
class MyTest {
  
  private string myarg1;
  MyTest(string arg1) {
    this.myarg1 = arg1 ?? throw new ArgumentNullException(nameof(myarg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 6, column: 5, expectation: true);
    }


    [Fact]
    public void test_multiple_if_throw_statement() {
      var test = @"
class MyTest {

  void myfn(string arg1, string arg2) {
    if (arg1 == null) throw new ArgumentNullException(nameof(of arg1));
    if (arg2 == null) throw new ArgumentNullException(nameof(of arg2));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 6, column: 5, expectation: true);
    }

    [Fact]
    public void test_ctor_multiple_if_throw_statement() {
      var test = @"
class MyTest {

  MyTest(string arg1, string arg2) {
    if (arg1 == null) throw new ArgumentNullException(nameof(of arg1));
    if (arg2 == null) throw new ArgumentNullException(nameof(of arg2));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 6, column: 5, expectation: true);
    }

    [Fact]
    public void test_ctor_multiple_coalesce_throw_statement() {
      var test = @"
class MyTest {
  
  private string myarg1;
  private string myarg2;
  MyTest(string arg1, string arg2) {
    this.myarg1 = arg1 ?? throw new ArgumentNullException(nameof(myarg1));
    this.myarg2 = arg2 ?? throw new ArgumentNullException(nameof(myarg2));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 8, column: 5, expectation: true);
    }



    [Fact]
    public void test_skip_if_previous_is_not_argument_throw_syntax() {
      var test = @"
class MyTest {

  void myfn(string arg1, string arg2) {
    if (arg1 == null) throw new ArgumentNullException(nameof(of arg1));
    Foo();
    if (arg2 == null) throw new ArgumentNullException(nameof(of arg2));
  }

  void Foo() { }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 7, column: 5, expectation: false);
    }

    [Fact]
    public void test_argument_out_of_range_exception() {
      var test = @"
class MyTest {

  void myfn(int i) {
    if (i < 0) throw new ArgumentOutOfRange(nameof(of arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_argument_exception() {
      var test = @"
class MyTest {

  void myfn(int i) {
    if (i < 0) throw new ArgumentException(""lorem ipsum"", nameof(of arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_skip_expression_function_calls() {
      var test = @"
class MyTest {

  void myfn(int i) {
    if (IsSomething(i)) throw new ArgumentException(""lorem ipsum"");
  }

  bool IsSomething(int i) {
    return false;
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: false);
    }

    [Fact]
    public void test_skip_member_access_with_same_name_like_paramter() {
      var test = @"
class MyTest {

  void myfn(int i) {
    if (this.i == 0) throw new InvalidOperationException();
  }

  int i;
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: false);
    }

    [Fact]
    public void test_skip_multiple_if_statements() {
      var test = @"
class MyTest {

  void myfn(string arg1) {
    if (arg1 == null) {
      var x = 5;
      throw new ArgumentNullException(nameof(of arg1));
    }
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: false);
    }


    [Fact]
    public void test_throw_function_syntax() {
      var test = @"
class MyTest {

  void myfn(string arg1) {
    if (arg1 == null) throw ThrowHelper.ArgumentNull(nameof(arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_bool_expression() {
      var test = @"
class MyTest {

  void myfn(bool arg1) {
    if (arg1) throw new ArgumentException(nameof(arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_multiline_if_statement() {
      var test = @"
class MyTest {

  void myfn(bool arg1) {
    if (arg1) {
      throw new ArgumentException(nameof(arg1));
    }
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true); // Position if
      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 6, column: 10, expectation: true); // Position throw
      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 7, column: 6, expectation: true); // }
    }

    [Fact]
    public void test_skip_multiline_if_statement_inside_another_block() {
      var test = @"
class MyTest {

  void myfn(bool arg1) {
    if (something == something2) {
      if (arg1) {
        throw new ArgumentException(nameof(arg1));
      }
    }
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 7, column: 12, expectation: false);
    }

    [Fact]
    public void test_skip_method_itself() {
      var test = @"
class MyTest {

  void myfn(bool arg1) {
    if (arg1) throw new ArgumentException(nameof(arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 4, column: 5, expectation: false);
    }

    [Fact]
    public void test_member_access_parameter() {
      var test = @"
class MyTest {

  void myfn(string arg1) {
    if (arg1.Length == 0) throw new ArgumentException(nameof(arg1));
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_skip_property() {
      var test = @"
class MyTest {

  public bool MyProp1 => False;
  public bool MyProp2 { get; set; }
  public bool MyProp3 {
    get {
      return false;
    }
    set {
      if (value) throw new ArgumentException();
    }
  }
}
";

      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 4, column: 5, expectation: false);
      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 27, expectation: false);
      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 5, column: 32, expectation: false);
      VerifyCSharpIsIfArgumentThrowSyntaxStatement(test, line: 11, column: 10, expectation: false);
    }
  }
}
