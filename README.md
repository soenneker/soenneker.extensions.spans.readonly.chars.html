[![](https://img.shields.io/nuget/v/soenneker.extensions.spans.readonly.chars.html.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.spans.readonly.chars.html/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.spans.readonly.chars.html/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.spans.readonly.chars.html/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.spans.readonly.chars.html.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.spans.readonly.chars.html/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.spans.readonly.chars.html/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.spans.readonly.chars.html/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Spans.ReadOnly.Chars.Html
Allocation-free HTML sniffing and tag-search helpers for `ReadOnlySpan<char>`.

## Installation

```bash
dotnet add package Soenneker.Extensions.Spans.ReadOnly.Chars.Html
```

## Detect HTML-like text

```csharp
using Soenneker.Extensions.Spans.ReadOnly.Chars.Html;

ReadOnlySpan<char> input = "prefix <article>content</article>";
bool looksLikeHtml = input.LooksLikeHtml();
```

`LooksLikeHtml()` searches for a bracketed sequence beginning with an ASCII letter, `/`, or `!`. It is a fast heuristic: it does not parse HTML, validate nesting, or sanitize content.

## Find an opening tag

```csharp
ReadOnlySpan<char> html = "<DIV class=\"notice\">Hello</DIV>";
bool containsDiv = html.ContainsOpenTag("div");
```

`ContainsOpenTag()` compares the tag name case-insensitively, rejects closing tags, and requires the name to be followed by whitespace, `>`, `/`, or the end of the input. It does not understand comments, scripts, quoted markup, or malformed documents, so use an HTML parser when correctness or security depends on document structure.

## Scan for a class attribute

```csharp
int index = html.IndexOfClassStart(0);
while (index >= 0)
{
    if (html.IsClassKeywordAt(index))
    {
        // Inspect boundaries and the following '=' as required by your parser.
        break;
    }

    index = html.IndexOfClassStart(index + 1);
}
```

These two methods are lexical building blocks only: `IndexOfClassStart()` finds any `c`/`C`, and `IsClassKeywordAt()` checks the next five characters without validating attribute-name boundaries.
