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
}
