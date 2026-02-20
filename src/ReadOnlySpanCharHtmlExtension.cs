using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Extensions.Char;

namespace Soenneker.Extensions.Spans.ReadOnly.Chars.Html;

/// <summary>
/// A collection of helpful ReadOnlySpan (char) html-related extension methods
/// </summary>
public static class ReadOnlySpanCharHtmlExtension
{
    /// <summary>
    /// Determines whether the specified character span appears to contain valid HTML-like content.
    /// </summary>
    /// <remarks>The method checks for a '<' character followed by a valid HTML tag indicator (such as a
    /// letter, '/', or '!'). It returns false for malformed tags or if whitespace immediately follows the '<'
    /// character.</remarks>
    /// <param name="s">The character span to evaluate for the presence of an HTML-like tag structure.</param>
    /// <returns>true if the character span contains a valid HTML-like tag; otherwise, false.</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool LooksLikeHtml(this ReadOnlySpan<char> s)
    {
        int lt = s.IndexOf('<');
        if (lt < 0)
            return false;

        if ((uint)(lt + 1) >= (uint)s.Length)
            return false;

        char next = s[lt + 1];

        // Likely not HTML if "< " etc.
        if (next == ' ' || next == '\t' || next == '\r' || next == '\n')
            return false;

        // Accept: <a, </a, <!doctype, <!--
        if (!(next.IsAsciiLetter() || next == '/' || next == '!'))
            return false;

        int gt = s.Slice(lt + 2).IndexOf('>');
        return gt >= 0;
    }

    /// <summary>
    /// Determines whether the specified HTML content contains an open tag with the given tag name.
    /// </summary>
    /// <remarks>The search is case-insensitive and matches only valid open tags. An open tag is defined as a
    /// '<' character immediately followed by the tag name and a valid boundary character, such as whitespace, '>', '/',
    /// or the end of the content. This method does not match closing tags or partial tag names.</remarks>
    /// <param name="html">The HTML content to search for an open tag. This span must not be empty.</param>
    /// <param name="tagName">The name of the tag to search for. This span must not be empty and is compared case-insensitively.</param>
    /// <returns>true if the specified tag name is found as an open tag in the HTML content; otherwise, false.</returns>
    [Pure]
    public static bool ContainsOpenTag(this ReadOnlySpan<char> html, ReadOnlySpan<char> tagName)
    {
        // Case-insensitive search for "<tag" with a boundary check so "div" doesn't match "<divvy".
        // Boundary after tagName must be one of: whitespace, '>', '/', or end.
        if (tagName.IsEmpty)
            return false;

        for (int i = 0; i < html.Length; i++)
        {
            if (html[i] != '<')
                continue;

            int start = i + 1;
            if (start >= html.Length)
                continue;

            // Skip closing tags: </tag
            if (html[start] == '/')
                continue;

            if (start + tagName.Length > html.Length)
                continue;

            if (!html.Slice(start, tagName.Length).Equals(tagName, StringComparison.OrdinalIgnoreCase))
                continue;

            int end = start + tagName.Length;
            if (end == html.Length)
                return true;

            char boundary = html[end];
            if (boundary == '>' || boundary == '/' || boundary == ' ' || boundary == '\t' || boundary == '\r' || boundary == '\n')
                return true;
        }

        return false;
    }

    /// <summary>
    /// Finds the index of the first occurrence of the character 'c' or 'C' in the specified span, starting the search
    /// from the given index.
    /// </summary>
    /// <remarks>This method performs a case-sensitive search for the character 'c' or 'C'. If the start index
    /// is out of range, the method will return -1.</remarks>
    /// <param name="span">The span of characters to search within. This parameter cannot be empty.</param>
    /// <param name="start">The zero-based index from which to start the search. Must be less than the length of the span.</param>
    /// <returns>The zero-based index of the first occurrence of 'c' or 'C' if found; otherwise, -1 if the character is not
    /// found.</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IndexOfClassStart(this ReadOnlySpan<char> span, int start)
    {
        for (int j = start; j < span.Length; j++)
        {
            char ch = span[j];
            if (ch is 'c' or 'C')
                return j;
        }
        return -1;
    }

    /// <summary>
    /// Determines whether the characters at the specified index within the span represent the keyword 'class', using a
    /// case-insensitive comparison.
    /// </summary>
    /// <remarks>The comparison is performed in a case-insensitive manner. The method returns false if the
    /// index plus the length of the keyword exceeds the span's length.</remarks>
    /// <param name="span">The span of characters to examine. Must not be empty.</param>
    /// <param name="idx">The zero-based index in the span at which to check for the keyword 'class'. Must be within the bounds of the
    /// span; otherwise, the method returns false.</param>
    /// <returns>true if the characters at the specified index match the keyword 'class' (case-insensitive); otherwise, false.</returns>
    [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsClassKeywordAt(this ReadOnlySpan<char> span, int idx)
    {
        if ((uint)(idx + 4) >= (uint)span.Length)
            return false;

        // case-insensitive "class"
        return span[idx + 0].ToAsciiLower() == 'c'
               && span[idx + 1].ToAsciiLower() == 'l'
               && span[idx + 2].ToAsciiLower() == 'a'
               && span[idx + 3].ToAsciiLower() == 's'
               && span[idx + 4].ToAsciiLower() == 's';
    }
}
