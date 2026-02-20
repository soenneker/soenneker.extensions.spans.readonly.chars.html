using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Soenneker.Extensions.Char;
using Soenneker.Extensions.Spans.Readonly.Chars;

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

        if (next.IsAsciiWhiteSpace())
            return false;

        // Accept: <a, </a, <!doctype, <!--
        if (!(next.IsAsciiLetter() || next == '/' || next == '!'))
            return false;

        int gt = s[(lt + 2)..].IndexOf('>');
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
        if (tagName.IsEmpty || html.IsEmpty)
            return false;

        bool tagAscii = tagName.IsAscii();
        int len = tagName.Length;

        int i = 0;
        while (true)
        {
            int lt = html.Slice(i).IndexOf('<');
            if (lt < 0)
                return false;

            i += lt + 1;
            if ((uint)i >= (uint)html.Length)
                return false;

            if (html[i] == '/')
                continue;

            if ((uint)(i + len) > (uint)html.Length)
                continue;

            var candidate = html.Slice(i, len);

            bool match;
            if (tagAscii)
            {
                // only run ASCII path if candidate is ASCII too
                match = candidate.IsAscii() && candidate.EqualsAsciiIgnoreCase_AssumeAscii(tagName);
            }
            else
            {
                match = candidate.Equals(tagName, StringComparison.OrdinalIgnoreCase);
            }

            if (!match)
                continue;

            int end = i + len;
            if (end == html.Length)
                return true;

            char boundary = html[end];
            if (boundary is '>' or '/' or ' ' or '\t' or '\r' or '\n')
                return true;
        }
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
        if ((uint)start >= (uint)span.Length)
            return -1;

        int idx = span.Slice(start).IndexOfAny('c', 'C');
        return idx < 0 ? -1 : start + idx;
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
        if ((uint)idx > (uint)span.Length)
            return false;

        if ((uint)(idx + 5) > (uint)span.Length)
            return false;

        // ASCII case-fold via | 0x20
        return (span[idx + 0] | 0x20u) == 'c'
               && (span[idx + 1] | 0x20u) == 'l'
               && (span[idx + 2] | 0x20u) == 'a'
               && (span[idx + 3] | 0x20u) == 's'
               && (span[idx + 4] | 0x20u) == 's';
    }
}
