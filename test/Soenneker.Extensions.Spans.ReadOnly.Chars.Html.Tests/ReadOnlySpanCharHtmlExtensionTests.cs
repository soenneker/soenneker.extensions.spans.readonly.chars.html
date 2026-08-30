using System;
using AwesomeAssertions;
using Soenneker.Tests.Unit;

namespace Soenneker.Extensions.Spans.ReadOnly.Chars.Html.Tests;

public sealed class ReadOnlySpanCharHtmlExtensionTests : UnitTest
{
    [Test]
    public void LooksLikeHtml_FindsValidTagAfterLiteralLessThan()
    {
        "1 < 2, then <strong>yes</strong>".AsSpan().LooksLikeHtml().Should().BeTrue();
    }
}
