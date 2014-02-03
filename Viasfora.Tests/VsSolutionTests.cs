using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Winterdom.Viasfora;
using Xunit;

namespace Viasfora.Tests {
  public class VsSolutionTests {
    [Fact]
    public void MakeRelativePath_NestedPath() {
      String slnPath = @"C:\users\myuser\documents\Visual Studio 10.0\Project\MySolution\";
      String filePath = Path.Combine(slnPath, @"MyProject\Files\File1.txt");
      String relative = VsSolution.MakeRelativePath(slnPath, filePath);
      Assert.Equal(@"MyProject\Files\File1.txt", relative);
    }
    [Fact]
    public void MakeRelativePath_ParentPath() {
      String slnPath = @"C:\users\myuser\documents\Visual Studio 10.0\Project\MySolution\";
      String filePath = Path.Combine(slnPath, @"..\..\MyProject\Files\File1.txt");
      String relative = VsSolution.MakeRelativePath(slnPath, Path.GetFullPath(filePath));
      Assert.Equal(@"..\..\MyProject\Files\File1.txt", relative);
    }

  }
}
