using Winterdom.Viasfora.LanguageService.Core.Tests.MethodOverloadsTagger;
using Xunit;

namespace Winterdom.Viasfora.LanguageService.CSharp.Tests.MethodOverloadsTagger {
  public class SyntaxHelperCSharpTests : SyntaxHelperTests {

    [Fact]
    public void test_parameter_default() {
      var test = @"
class MyTest {

  void Foo() {
    Foo(true);
  }
  
  void Foo(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_with_this() {
      var test = @"
class MyTest {

  void Foo() {
    this.Foo(true);
  }
  
  void Foo(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_with_return() {
      var test = @"
class MyTest {

  bool Foo() {
    return this.Foo(true);
  }
  
  bool Foo(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_calling_same_name_with_core_prefix() {
      var test = @"
class MyTest {

  void Foo() {
    FooCore(true);
  }
  
  void FooCore(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_with_null_checking() {
      var test = @"
class MyTest {

  void Foo(string arg1, string arg2) {
    if (arg1 == null) throw new ArgumentNullException(nameof(arg1));
    if (arg2 == null) throw new ArgumentNullException(nameof(arg2));

    FooCore(true);
  }
  
  void FooCore(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }

    [Fact]
    public void test_generic() {
      var test = @"
class MyTest {

  string Foo() {
    return Foo<string>();
  }
  
  T Foo<T>() {
    return FooCore<T>(true);
  }
  T FooCore(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
      VerifyCSharpIsMethodOverload(test, line: 9, column: 5, expectation: true);
    }


    [Fact]
    public void test_async() {
      var test = @"
class MyTest {

  async Task Foo() {
    await Foo(true);
  }
  
  async Task Foo(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }


    [Fact]
    public void test_async_return() {
      var test = @"
class MyTest {

  async Task<bool> Foo() {
    return await Foo(true);
  }
  
  async Task<bool> Foo(bool v) {
  }
}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 5, expectation: true);
    }






    [Fact]
    public void test_skip_empty() {
      var test = @"
class MyTest {

  void Foo() {
  }
  
}
";

      VerifyCSharpIsMethodOverload(test, line: 4, column: 10, expectation: false);
    }

    [Fact]
    public void test_skip_abstract() {
      var test = @"
abstract class MyTest {

  protected abstract void Foo();  

}
";

      VerifyCSharpIsMethodOverload(test, line: 4, column: 10, expectation: false);
    }

    [Fact]
    public void test_skip_interface() {
      var test = @"
interface MyTest {

  void Foo();

}
";

      VerifyCSharpIsMethodOverload(test, line: 4, column: 10, expectation: false);
    }


    [Fact]
    public void test_skip_multi_statements() {
      var test = @"
class MyTest {

  void Foo() {
    Bar();
    Baz();
  }

  void Bar() {}
  void Baz() {}

}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 8, expectation: false);
    }

    [Fact]
    public void test_skip_calling_another_class() {
      var test = @"
class MyTest {

  void Foo() {
    Console.WriteLine();
  }

}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 8, expectation: false);
    }


    [Fact]
    public void test_skip_expressions() {
      var test = @"
class MyTest {

  void Foo() {
    bar = 42;
  }

  int bar;

}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 8, expectation: false);
    }

    [Fact]
    public void test_skip_throw_statement() {
      var test = @"
class MyTest {

  void Foo() {
    throw new NotImplentedException();
  }

}
";

      VerifyCSharpIsMethodOverload(test, line: 5, column: 8, expectation: false);
    }

  }
}
