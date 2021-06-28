using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

// Credit: James M. Croft, find original source here https://github.com/jamesmcroft/WinUX-UWP-Toolkit/blob/develop/WinUX.Common/Extensions/Extensions.Url.cs
public static class UriExtensions
{
    /// <summary>
    /// Extracts the query string of a Uri value into a collection of KeyValuePairs of key (query parameter) and value.
    /// </summary>
    /// <param name="uri">
    /// The Uri to extract the query results from.
    /// </param>
    /// <returns>
    /// Returns a collection of KeyValuePairs containing the values of the query.
    /// </returns>
    public static IEnumerable<KeyValuePair<string, string>> ExtractQueryValues(this Uri uri)
    {
        if (string.IsNullOrWhiteSpace(uri.Query))
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        var query = uri.Query.TrimStart('?');

        return query.Split('&')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Split('='))
                .Select(
                    x =>
                        new KeyValuePair<string, string>(
                            WebUtility.UrlDecode(x[0]),
                            x.Length == 2 && !string.IsNullOrWhiteSpace(x[1]) ? WebUtility.UrlDecode(x[1]) : null));
    }

    /// <summary>
    /// Extracts the value from the Uri value's query string if the given key exists.
    /// </summary>
    /// <param name="uri">
    /// The Uri to extract the query value from.
    /// </param>
    /// <param name="key">
    /// The key of the query item.
    /// </param>
    /// <returns>
    /// Returns the value for the given key.
    /// </returns>
    public static string ExtractQueryValue(this Uri uri, string key)
    {
        var query = uri.ExtractQueryValues().ToList();

        var item = query.FirstOrDefault(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

        return string.IsNullOrWhiteSpace(item.Value) ? null : item.Value;
    }
}