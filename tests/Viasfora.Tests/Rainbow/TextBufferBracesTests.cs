using Microsoft.VisualStudio.Text;
using System;
using System.Linq;
using Winterdom.Viasfora;
using Winterdom.Viasfora.Rainbow;
using Xunit;

namespace Viasfora.Tests.Rainbow {
  public class TextBufferBracesTests : VsfVsTestBase {
    [Fact]
    public void CanParseEntireFile() {
      var textBuffer = GetCSharpTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      Assert.Equal(32, cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count());
    }

    [Fact]
    public void CanFindExtraClosingBraces() {
      var textBuffer = GetCSharpTextBuffer("RainbowErrors.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      Assert.Equal(2, cache.ErrorBracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count());
    }

    [Fact]
    public void CanInvalidate() {
      var textBuffer = GetCSharpTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();

      // force the cache to parse entire file
      cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count();

      // invalidate starting in Get_<T>
      var endpoint = new SnapshotPoint(snapshot, 357);
      cache.Invalidate(endpoint);
      // 331 is the position of the closing } of method before this
      Assert.Equal(331, cache.LastParsedPosition);
    }

    [Fact]
    public void CanGetSurroundingBraces() {
      var textBuffer = GetCSharpTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      // force parsing all text
      cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count();

      // we're looking at the {} for this function
      //public static bool Has<T>(this IPropertyOwner owner) {
      //  return owner.Properties.ContainsProperty(typeof(T));
      //}
      var line10 = snapshot.GetLineFromLineNumber(9);
      var matchingBraces = cache.GetBracePairFromPosition(line10.Start + 10, RainbowHighlightMode.TrackInsertionPoint);
      Assert.NotNull(matchingBraces);
      Assert.Equal('{', matchingBraces.Item1.Brace);
      Assert.Equal('}', matchingBraces.Item2.Brace);
    }

    [Fact]
    public void CanGetSurroundingBraces_TrackNextScope() {
      var textBuffer = GetCSharpTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      // force parsing all text
      cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count();

      // we're looking at the {} for this function, positioned just at the {
      //public static bool Has<T>(this IPropertyOwner owner) {
      //  return owner.Properties.ContainsProperty(typeof(T));
      //}
      var line9 = snapshot.GetLineFromLineNumber(8);
      Console.WriteLine(line9.GetText());
      var openingBrace = line9.End - 1;
      Assert.Equal('{', openingBrace.GetChar());
      var matchingBraces = cache.GetBracePairFromPosition(openingBrace, RainbowHighlightMode.TrackNextScope);
      Assert.NotNull(matchingBraces);
      Assert.Equal(openingBrace.Position, matchingBraces.Item1.Position);
    }

    [Fact]
    public void CanGetSurroundingBraces_TrackInsertionPoint() {
      var textBuffer = GetCSharpTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      // force parsing all text
      cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count();

      // we're looking at the {} for this function, positioned just at the {
      //public static bool Has<T>(this IPropertyOwner owner) {
      //  return owner.Properties.ContainsProperty(typeof(T));
      //}
      var line9 = snapshot.GetLineFromLineNumber(8);
      var openingBrace = line9.Start + line9.GetText().IndexOf('{');
      Assert.Equal('{', openingBrace.GetChar());
      var matchingBraces = cache.GetBracePairFromPosition(openingBrace, RainbowHighlightMode.TrackInsertionPoint);
      Assert.NotNull(matchingBraces);
      // we should have gotten braces starting *before* the search position!
      Assert.True(openingBrace.Position > matchingBraces.Item1.Position);
    }

    [Fact]
    public void ParsingPlainTextFileDoesntScanDocument() {
      var textBuffer = GetPlainTextBuffer("Rainbow1.txt");
      var snapshot = textBuffer.CurrentSnapshot;
      var cache = new TextBufferBraces(snapshot, GetLang(textBuffer));
      var span = snapshot.GetSpan();
      Assert.Equal(0, cache.BracesInSpans(new NormalizedSnapshotSpanCollection(span)).Count());
      Assert.True(cache.LastParsedPosition <= 0);
    }
  }
}
