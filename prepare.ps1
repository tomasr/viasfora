#
# Copy necessary assemblies from VS2010 SP1 SDK folder
# for building our extension
#

$vs = "${env:ProgramFiles(x86)}\Microsoft Visual Studio 10.0\Common7\IDE\PublicAssemblies"
$vssdk = "${env:ProgramFiles(x86)}\Microsoft Visual Studio 2010 SDK SP1"
$basedir = "$vssdk\VisualStudioIntegration\Common\Assemblies"
$targetdir = '.\refs\'

if ( !(Test-Path $targetdir) ) {
  [void](mkdir $targetdir)
}

Copy-Item $basedir\v4.0\Microsoft.VisualStudio.CoreUtility.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Editor.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Text.Data.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Text.Logic.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Text.UI.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Text.UI.Wpf.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Text.UI.Wpf.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Language.Intellisense.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Language.StandardClassification.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Shell.10.0.dll $targetdir
Copy-Item $basedir\v4.0\Microsoft.VisualStudio.Shell.Immutable.10.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.OLE.Interop.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.Shell.Interop.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.Shell.Interop.8.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.Shell.Interop.9.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.Shell.Interop.10.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.TextManager.Interop.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.TextManager.Interop.8.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.TextManager.Interop.9.0.dll $targetdir
Copy-Item $basedir\v2.0\Microsoft.VisualStudio.TextManager.Interop.10.0.dll $targetdir

Copy-Item $vs\Microsoft.VisualStudio.ComponentModelHost.dll $targetdir
