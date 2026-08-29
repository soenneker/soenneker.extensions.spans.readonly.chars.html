[![](https://img.shields.io/nuget/v/soenneker.extensions.spans.readonly.chars.html.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.spans.readonly.chars.html/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.spans.readonly.chars.html/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.spans.readonly.chars.html/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.extensions.spans.readonly.chars.html.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.extensions.spans.readonly.chars.html/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.extensions.spans.readonly.chars.html/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.extensions.spans.readonly.chars.html/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Extensions.Spans.ReadOnly.Chars.Html
A collection of helpful ReadOnlySpan (char) html-related extension methods.

## Installation

```bash
dotnet add package Soenneker.Extensions.Spans.ReadOnly.Chars.Html
```

## Quick start

```csharp
using Soenneker.Extensions.Spans.ReadOnly.Chars.Html;

// Given an existing ReadOnlySpan<char> named s:
var result = s.LooksLikeHtml();
```

## Common operations

- `LooksLikeHtml()` - Determines whether the specified character span appears to contain valid HTML-like content.
- `ContainsOpenTag()` - Determines whether the specified HTML content contains an open tag with the given tag name. The search is case-insensitive and matches only valid open tags.
- `IndexOfClassStart()` - Finds the index of the first occurrence of the character 'c' or 'C' in the specified span, starting the search from the given index. Returns the zero-based index of the first occurrence of 'c' or 'C' if found; otherwise, -1 if the character is not found. This method performs a case-sensitive search for the character 'c' or 'C'.
- `IsClassKeywordAt()` - Determines whether the characters at the specified index within the span represent the keyword 'class', using a case-insensitive comparison. The comparison is performed in a case-insensitive manner.
