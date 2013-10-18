#
# Copy necessary assemblies from VS2010 SP1 SDK folder
# for building our extension
#

$basedir = "${env:ProgramFiles(x86)}\Microsoft Visual Studio 2010 SDK SP1\VisualStudioIntegration\Common\Assemblies\v4.0"
$targetdir = '.\refs\'

Copy-Item $basedir\Microsoft.VisualStudio.CoreUtility.dll $targetdir
Copy-Item $basedir\Microsoft.VisualStudio.Text.Data.dll $targetdir
Copy-Item $basedir\Microsoft.VisualStudio.Text.Logic.dll $targetdir
Copy-Item $basedir\Microsoft.VisualStudio.Text.UI.dll $targetdir
Copy-Item $basedir\Microsoft.VisualStudio.Text.UI.Wpf.dll $targetdir
