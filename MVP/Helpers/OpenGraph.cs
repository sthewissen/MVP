using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MVP.Helpers
{
    // This code comes from the awesome library OpenGraph-Net: https://github.com/ghorsey/OpenGraph-Net
    // Unfortunately at this time there's a bug in Mono/Xamarin preventing it from being used on Xamarin apps.
    // When the time comes the NuGet will be properly referenced instead.

    /// <summary>
    /// Represents Open Graph meta data parsed from HTML.
    /// </summary>
    public class OpenGraph
    {
        /// <summary>
        /// The open graph data.
        /// </summary>
        private readonly StructuredMetadataDictionary internalOpenGraphData;

        /// <summary>
        /// Prevents a default instance of the <see cref="OpenGraph" /> class from being created.
        /// </summary>
        private OpenGraph()
        {
            this.internalOpenGraphData = new StructuredMetadataDictionary();
            this.Namespaces = new Dictionary<string, OpenGraphNamespace>();
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public IDictionary<string, IList<StructuredMetadata>> Metadata => new ReadOnlyDictionary<string, IList<StructuredMetadata>>(this.internalOpenGraphData);

        /// <summary>
        /// Gets the namespaces.
        /// </summary>
        /// <value>The namespaces.</value>
        public IDictionary<string, OpenGraphNamespace> Namespaces { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type of open graph document.</value>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the title of the open graph document.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the image for the open graph document.
        /// </summary>
        /// <value>The image.</value>
        public Uri Image { get; private set; }

        /// <summary>
        /// Gets the URL for the open graph document.
        /// </summary>
        /// <value>The URL.</value>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets the original URL used to generate this graph.
        /// </summary>
        /// <value>The original URL.</value>
        public Uri OriginalUrl { get; private set; }

        /// <summary>
        /// Gets the original HTML content.
        /// </summary>
        /// <value>
        /// The original HTML content.
        /// </value>
        public string OriginalHtml { get; private set; }

        /// <summary>
        /// Gets the head prefix attribute value.
        /// </summary>
        /// <value>
        /// The head prefix attribute value.
        /// </value>
        public string HeadPrefixAttributeValue
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ns in this.Namespaces)
                {
                    _ = sb.AppendFormat(CultureInfo.InvariantCulture, " {0}", ns.Value);
                }

                return sb.ToString().Trim();
            }
        }

        /// <summary>
        /// Gets the HTML XML namespace values.
        /// </summary>
        /// <value>
        /// The HTML XML namespace values.
        /// </value>
        public string HtmlXmlnsValues
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var ns in this.Namespaces)
                {
                    _ = sb.AppendFormat(CultureInfo.InvariantCulture, " xmlns:{0}=\"{1}\"", ns.Value.Prefix, ns.Value.SchemaUri);
                }

                return sb.ToString().Trim();
            }
        }

        /// <summary>
        /// Makes the graph.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="type">The type.</param>
        /// <param name="image">The image.</param>
        /// <param name="url">The URL.</param>
        /// <param name="description">The description.</param>
        /// <param name="siteName">Name of the site.</param>
        /// <param name="audio">The audio.</param>
        /// <param name="video">The video.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="localeAlternates">The locale alternates.</param>
        /// <param name="determiner">The determiner.</param>
        /// <returns><see cref="OpenGraph"/>.</returns>
        public static OpenGraph MakeGraph(
            string title,
            string type,
            string image,
            string url,
            string description = "",
            string siteName = "",
            string audio = "",
            string video = "",
            string locale = "",
            IList<string> localeAlternates = null,
            string determiner = "")
        {
            var graph = new OpenGraph
            {
                Title = title,
                Type = type,
                Image = new Uri(image, UriKind.Absolute),
                Url = new Uri(url, UriKind.Absolute),
            };
            var ns = NamespaceRegistry.Instance.Namespaces["og"];

            graph.Namespaces.Add(ns.Prefix, ns);
            graph.AddMetadata(new StructuredMetadata(ns, "title", title));
            graph.AddMetadata(new StructuredMetadata(ns, "type", type));
            graph.AddMetadata(new StructuredMetadata(ns, "image", image));
            graph.AddMetadata(new StructuredMetadata(ns, "url", url));

            if (!string.IsNullOrWhiteSpace(description))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "description", description));
            }

            if (!string.IsNullOrWhiteSpace(siteName))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "site_name", siteName));
            }

            if (!string.IsNullOrWhiteSpace(audio))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "audio", audio));
            }

            if (!string.IsNullOrWhiteSpace(video))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "video", video));
            }

            if (!string.IsNullOrWhiteSpace(locale))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "locale", locale));
            }

            if (!string.IsNullOrWhiteSpace(determiner))
            {
                graph.AddMetadata(new StructuredMetadata(ns, "determiner", determiner));
            }

            if (graph.internalOpenGraphData.ContainsKey("og:locale"))
            {
                var localeElement = graph.internalOpenGraphData["og:locale"].First();
                foreach (var localeAlternate in localeAlternates ?? new List<string>())
                {
                    localeElement.AddProperty(new PropertyMetadata("alternate", localeAlternate));
                }
            }
            else
            {
                foreach (var localeAlternate in localeAlternates ?? new List<string>())
                {
                    graph.AddMetadata(new StructuredMetadata(ns, "locale:alternate", localeAlternate));
                }
            }

            return graph;
        }

        /// <summary>
        /// Downloads the HTML of the specified URL and parses it for open graph content.
        /// </summary>
        /// <param name="url">The URL to download the HTML from.</param>
        /// <param name="userAgent">The user agent to use when downloading content.  The default is <c>"facebookexternalhit"</c> which is required for some site (like amazon) to include open graph data.</param>
        /// <param name="validateSpecification">if set to <c>true</c> <see cref="OpenGraph"/> will validate against the specification.</param>
        /// <returns>
        ///   <see cref="OpenGraph" />.
        /// </returns>
        public static OpenGraph ParseUrl(string url, string userAgent = "facebookexternalhit", bool validateSpecification = false) => ParseUrlAsync(url, userAgent, validateSpecification).GetAwaiter().GetResult();

        /// <summary>
        /// Parses the URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="validateSpecification">if set to <c>true</c> validate minimum Open Graph specification.</param>
        /// <returns><see cref="Task{OpenGraph}"/>.</returns>
        public static Task<OpenGraph> ParseUrlAsync(string url, string userAgent = "facebookexternalhit", bool validateSpecification = false)
        {
            if (!Regex.IsMatch(url, "^https?://", RegexOptions.IgnoreCase))
            {
                url = $"http://{url}";
            }

            Uri uri = new Uri(url);
            return ParseUrlAsync(uri, userAgent, validateSpecification);
        }

        /// <summary>
        /// Parses the URL asynchronous.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="userAgent">The user agent.</param>
        /// <param name="validateSpecification">if set to <c>true</c> [validate specification].</param>
        /// <returns><see cref="Task{OpenGraph}"/>.</returns>
        public static async Task<OpenGraph> ParseUrlAsync(Uri url, string userAgent = "facebookexternalhit", bool validateSpecification = false)
        {
            OpenGraph result = new OpenGraph { OriginalUrl = url };

            HttpDownloader downloader = new HttpDownloader(url, null, userAgent);
            string html = await downloader.GetPageAsync().ConfigureAwait(false);
            result.OriginalHtml = html;

            return ParseHtml(result, html, validateSpecification);
        }

        /// <summary>
        /// Parses the HTML for open graph content.
        /// </summary>
        /// <param name="content">The HTML to parse.</param>
        /// <param name="validateSpecification">if set to <c>true</c> verify that the document meets the required attributes of the open graph specification.</param>
        /// <returns><see cref="OpenGraph"/>.</returns>
        public static OpenGraph ParseHtml(string content, bool validateSpecification = false)
        {
            OpenGraph result = new OpenGraph();
            return ParseHtml(result, content, validateSpecification);
        }

        /// <summary>
        /// Adds the meta element.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AddMetadata(StructuredMetadata element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var key = string.Concat(element.Namespace.Prefix, ":", element.Name);
            if (this.internalOpenGraphData.ContainsKey(key))
            {
                this.internalOpenGraphData[key].Add(element);
            }
            else
            {
                this.internalOpenGraphData.Add(key, new List<StructuredMetadata> { element });
            }
        }

        /// <summary>
        /// Adds the metadata.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="InvalidOperationException">The prefix {prefix} does not exist in the NamespaceRegistry.</exception>
        public void AddMetadata(string prefix, string name, string value)
        {
            if (!NamespaceRegistry.Instance.Namespaces.ContainsKey(prefix))
            {
                throw new InvalidOperationException($"The prefix {prefix} does not exist in the {nameof(NamespaceRegistry)}");
            }

            var ns = NamespaceRegistry.Instance.Namespaces[prefix];

            var metadata = new StructuredMetadata(ns, name, value);
            this.AddMetadata(metadata);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var doc = new HtmlDocument();

            var elements = this.internalOpenGraphData.SelectMany(og => og.Value).ToList();

            foreach (var structuredMetaElement in elements)
            {
                doc.DocumentNode.AppendChild(structuredMetaElement.CreateDocument().DocumentNode);
            }

            return doc.DocumentNode.InnerHtml;
        }

        /// <summary>
        /// Safes the HTML decode URL.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string.</returns>
        private static string HtmlDecodeUrl(string value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            // naive attempt
            var patterns = new Dictionary<string, string>
            {
                ["&amp;"] = "&",
            };

            foreach (var key in patterns)
            {
                value = value.Replace(key.Key, key.Value);
            }

            return value;
        }

        /// <summary>
        /// Gets the open graph key.
        /// </summary>
        /// <param name="metaTag">The meta tag.</param>
        /// <returns>Returns the key stored from the meta tag.</returns>
        private static string GetOpenGraphKey(HtmlNode metaTag)
        {
            if (metaTag.Attributes.Contains("property"))
            {
                return metaTag.Attributes["property"].Value;
            }

            return metaTag.Attributes["name"].Value;
        }

        /// <summary>
        /// Gets the open graph prefix.
        /// </summary>
        /// <param name="metaTag">The meta tag.</param>
        /// <returns>The prefix.</returns>
        private static string GetOpenGraphPrefix(HtmlNode metaTag)
        {
            var value = metaTag.Attributes.Contains("property") ? metaTag.Attributes["property"].Value : metaTag.Attributes["name"].Value;

            return value.Split(':')[0];
        }

        /// <summary>
        /// Cleans the open graph key.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// strips the namespace prefix from the value.
        /// </returns>
        private static string CleanOpenGraphKey(string prefix, string value)
        {
#if NETSTANDARD2_1
            return value.Replace(string.Concat(prefix, ":"), string.Empty, StringComparison.OrdinalIgnoreCase).ToLower(CultureInfo.InvariantCulture);
#else
            return value.Replace(string.Concat(prefix, ":"), string.Empty).ToLower(CultureInfo.InvariantCulture);
#endif
        }

        /// <summary>
        /// Gets the open graph value.
        /// </summary>
        /// <param name="metaTag">The meta tag.</param>
        /// <returns>Returns the value from the meta tag.</returns>
        private static string GetOpenGraphValue(HtmlNode metaTag)
        {
            if (!metaTag.Attributes.Contains("content"))
            {
                return string.Empty;
            }

            return metaTag.Attributes["content"].Value;
        }

        /// <summary>
        /// Initializes the <see cref="OpenGraph" /> class.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="document">The document.</param>
        private static void ParseNamespaces(OpenGraph result, HtmlDocument document)
        {
            const string NamespacePattern = @"(\w+):\s?(https?://[^\s]+)";

            HtmlNode head = document.DocumentNode.SelectSingleNode("//head");
            HtmlNode html = document.DocumentNode.SelectSingleNode("html");

            if (head != null && head.Attributes.Contains("prefix") && Regex.IsMatch(head.Attributes["prefix"].Value, NamespacePattern))
            {
                var matches = Regex.Matches(
                    head.Attributes["prefix"].Value,
                    NamespacePattern,
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    var prefix = match.Groups[1].Value;
                    if (NamespaceRegistry.Instance.Namespaces.ContainsKey(prefix))
                    {
                        result.Namespaces.Add(prefix, NamespaceRegistry.Instance.Namespaces[prefix]);
                        continue;
                    }

                    var ns = match.Groups[2].Value;
                    result.Namespaces.Add(prefix, new OpenGraphNamespace(prefix, ns));
                }
            }
            else if (html != null && html.Attributes.Any(a => a.Name.StartsWith("xmlns:", StringComparison.InvariantCultureIgnoreCase)))
            {
                var namespaces = html.Attributes.Where(a => a.Name.StartsWith("xmlns:", StringComparison.InvariantCultureIgnoreCase));
                foreach (var ns in namespaces)
                {
#if NETSTANDARD2_1
                    var prefix = ns.Name.ToLowerInvariant().Replace("xmlns:", string.Empty, StringComparison.InvariantCultureIgnoreCase);
#else
                    var prefix = ns.Name.ToLowerInvariant().Replace("xmlns:", string.Empty);
#endif
                    result.Namespaces.Add(prefix, new OpenGraphNamespace(prefix, ns.Value));
                }
            }
            else
            {
                // append the minimum og: prefix and namespace
                result.Namespaces.Add("og", NamespaceRegistry.Instance.Namespaces["og"]);
            }
        }

        /// <summary>
        /// Matches the namespace predicate.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> when the element has a namespace; otherwise <c>false</c>.</returns>
        private static bool MatchesNamespacePredicate(string value)
        {
#if NETSTANDARD2_1
            return value.IndexOf(':', StringComparison.OrdinalIgnoreCase) >= 0;
#else
            return value.IndexOf(':') >= 0;
#endif
        }

        /// <summary>
        /// Sets the namespace.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="prefix">The prefix.</param>
        private static void SetNamespace(OpenGraph graph, string prefix)
        {
            if (graph.Namespaces.Any(n => n.Key.Equals(prefix, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            if (NamespaceRegistry.Instance.Namespaces.Any(ns => ns.Key.Equals(prefix, StringComparison.CurrentCultureIgnoreCase)))
            {
                var ns = NamespaceRegistry.Instance.Namespaces.First(ns2 => ns2.Key.Equals(prefix, StringComparison.InvariantCultureIgnoreCase));
                graph.Namespaces.Add(ns.Key, ns.Value);
            }
        }

        /// <summary>
        /// Makes the document to parse.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The <see cref="HtmlDocument"/>.</returns>
        private static HtmlDocument MakeDocumentToParse(string content)
        {
            int indexOfClosingHead = Regex.Match(content, "</head>").Index;
            string toParse = content.Substring(0, indexOfClosingHead + 7);

            toParse += "<body></body></html>\r\n";

            var document = new HtmlDocument();
            document.LoadHtml(toParse);

            return document;
        }

        /// <summary>
        /// Parses the HTML.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="content">The content.</param>
        /// <param name="validateSpecification">if set to <c>true</c> [validate specification].</param>
        /// <returns><see cref="OpenGraph"/>.</returns>
        /// <exception cref="OpenGraphNet.InvalidSpecificationException">The parsed HTML does not meet the open graph specification.</exception>
        private static OpenGraph ParseHtml(OpenGraph result, string content, bool validateSpecification = false)
        {
            HtmlDocument document = MakeDocumentToParse(content);

            ParseNamespaces(result, document);

            HtmlNodeCollection allMeta = document.DocumentNode.SelectNodes("//meta");

            var openGraphMetaTags = from meta in allMeta ?? new HtmlNodeCollection(null)
                                    where (meta.Attributes.Contains("property") && MatchesNamespacePredicate(meta.Attributes["property"].Value)) ||
                                    (meta.Attributes.Contains("name") && MatchesNamespacePredicate(meta.Attributes["name"].Value))
                                    select meta;

            StructuredMetadata lastElement = null;
            foreach (HtmlNode metaTag in openGraphMetaTags)
            {
                var prefix = GetOpenGraphPrefix(metaTag);
                SetNamespace(result, prefix);
                if (!result.Namespaces.ContainsKey(prefix))
                {
                    continue;
                }

                string value = GetOpenGraphValue(metaTag);
                string property = GetOpenGraphKey(metaTag);
                var cleanProperty = CleanOpenGraphKey(prefix, property);

                value = HtmlDecodeUrl(property, value);

                if (lastElement != null && lastElement.IsMyProperty(property))
                {
                    lastElement.AddProperty(cleanProperty, value);
                }
                else if (IsChildOfExistingElement(result.internalOpenGraphData, property))
                {
                    var matchingElement =
                        result.internalOpenGraphData.First(kvp => kvp.Value.First().IsMyProperty(property));

                    var element = matchingElement.Value.FirstOrDefault(e => !e.Properties.ContainsKey(cleanProperty));
                    element?.AddProperty(cleanProperty, value);
                }
                else
                {
                    lastElement = new StructuredMetadata(result.Namespaces[prefix], cleanProperty, value);
                    result.AddMetadata(lastElement);
                }
            }

            result.Type = string.Empty;
            if (result.internalOpenGraphData.TryGetValue("og:type", out var type))
            {
                result.Type = (type.FirstOrDefault() ?? new NullMetadata()).Value ?? string.Empty;
            }

            result.Title = string.Empty;
            if (result.internalOpenGraphData.TryGetValue("og:title", out var title))
            {
                result.Title = (title.FirstOrDefault() ?? new NullMetadata()).Value ?? string.Empty;
            }

            result.Image = GetUri(result, "og:image");
            result.Url = GetUri(result, "og:url");

            if (validateSpecification)
            {
                ValidateSpecification(result);
            }

            return result;
        }

        /// <summary>
        /// Determines whether [is child of existing element] [the specified property].
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="property">The property.</param>
        /// <returns>
        ///   <c>true</c> if this child of existing element] [the specified property]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsChildOfExistingElement(StructuredMetadataDictionary collection, string property)
        {
            return collection.Any(kvp => kvp.Value.FirstOrDefault()?.IsMyProperty(property) ?? false);
        }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="property">The property.</param>
        /// <returns>The Uri.</returns>
        private static Uri GetUri(OpenGraph result, string property)
        {
            result.internalOpenGraphData.TryGetValue(property, out var url);

            try
            {
                return new Uri(url?.FirstOrDefault()?.Value ?? string.Empty);
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (UriFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// HTMLs the decode URLs.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value.</param>
        /// <returns>The decoded URL value.</returns>
        private static string HtmlDecodeUrl(string property, string value)
        {
            var urlPropertyPatterns = new[] { "image", "url^" };
            foreach (var urlPropertyPattern in urlPropertyPatterns)
            {
                if (Regex.IsMatch(property, urlPropertyPattern))
                {
                    return HtmlDecodeUrl(value);
                }
            }

            return value;
        }

        /// <summary>
        /// Validates the specification.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <exception cref="InvalidSpecificationException">The parsed HTML does not meet the open graph specification, missing element: {required}.</exception>
        private static void ValidateSpecification(OpenGraph result)
        {
            var prefixes = result.Namespaces.Select(ns => ns.Value.Prefix);

            var namespaces = NamespaceRegistry
                .Instance
                .Namespaces
                .Where(ns => prefixes.Contains(ns.Key) && ns.Value.RequiredElements.Count > 0)
                .Select(ns => ns.Value)
                .ToList();

            foreach (var ns in namespaces)
            {
                foreach (var required in ns.RequiredElements)
                {
                    if (!result.Metadata.ContainsKey(string.Concat(ns.Prefix, ":", required)))
                    {
                        throw new InvalidSpecificationException($"The parsed HTML does not meet the open graph specification, missing element: {required}");
                    }
                }
            }
        }
    }

    public class InvalidSpecificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSpecificationException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidSpecificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSpecificationException"/> class.
        /// </summary>
        public InvalidSpecificationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSpecificationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidSpecificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSpecificationException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        protected InvalidSpecificationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
    }

    /// <summary>
    /// A singleton to define supported namespaces.
    /// </summary>
    public sealed class NamespaceRegistry
    {
        /// <summary>
        /// The synchronization lock.
        /// </summary>
        private static readonly object SynchronizationLock = new object();

        /// <summary>
        /// The instance.
        /// </summary>
        private static NamespaceRegistry internalInstance;

        /// <summary>
        /// Prevents a default instance of the <see cref="NamespaceRegistry"/> class from being created.
        /// </summary>
        private NamespaceRegistry()
        {
            this.InternalNamespaces = new Dictionary<string, RegistryNamespace>
                                          {
                                              { "og", new RegistryNamespace("og", "http://ogp.me/ns#", "title", "type", "image", "url") },
                                              { "article", new RegistryNamespace("article", "http://ogp.me/ns/article#") },
                                              { "book", new RegistryNamespace("book", "http://ogp.me/ns/book#") },
                                              { "books", new RegistryNamespace("books", "http://ogp.me/ns/books#") },
                                              { "business", new RegistryNamespace("business", "http://ogp.me/ns/business#") },
                                              { "fitness", new RegistryNamespace("fitness", "http://ogp.me/ns/fitness#") },
                                              { "game", new RegistryNamespace("game", "http://ogp.me/ns/game#") },
                                              { "music", new RegistryNamespace("music", "http://ogp.me/ns/music#") },
                                              { "place", new RegistryNamespace("place", "http://ogp.me/ns/place#") },
                                              { "product", new RegistryNamespace("product", "http://ogp.me/ns/product#") },
                                              { "profile", new RegistryNamespace("profile", "http://ogp.me/ns/profile#") },
                                              { "restaurant", new RegistryNamespace("restaurant", "http://ogp.me/ns/restaurant#") },
                                              { "video", new RegistryNamespace("video", "http://ogp.me/ns/video#") },
                                          };
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static NamespaceRegistry Instance
        {
            get
            {
                if (internalInstance == null)
                {
                    lock (SynchronizationLock)
                    {
                        if (internalInstance == null)
                        {
                            internalInstance = new NamespaceRegistry();
                        }
                    }
                }

                return internalInstance;
            }
        }

        /// <summary>
        /// Gets the default namespace.
        /// </summary>
        /// <value>
        /// The default namespace.
        /// </value>
        public static OpenGraphNamespace DefaultNamespace => Instance.Namespaces["og"];

        /// <summary>
        /// Gets the namespaces.
        /// </summary>
        /// <value>
        /// The namespaces.
        /// </value>
        public IDictionary<string, RegistryNamespace> Namespaces => new ReadOnlyDictionary<string, RegistryNamespace>(this.InternalNamespaces);

        /// <summary>
        /// Gets the schemas.
        /// </summary>
        /// <value>
        /// The schemas.
        /// </value>
        private IDictionary<string, RegistryNamespace> InternalNamespaces { get; }

        /// <summary>
        /// Adds the namespace.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="schemaUri">The schema URI.</param>
        /// <param name="requiredElements">The required elements.</param>
        public void AddNamespace(string prefix, string schemaUri, params string[] requiredElements)
        {
            this.InternalNamespaces.Add(prefix, new RegistryNamespace(prefix, schemaUri, requiredElements));
        }

        /// <summary>
        /// Removes the namespace.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public void RemoveNamespace(string prefix)
        {
            this.InternalNamespaces.Remove(prefix);
        }
    }

    /// <summary>
    /// An OpenGraph Namespace.
    /// </summary>
    public class OpenGraphNamespace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGraphNamespace"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="schemaUri">The schema URI.</param>
        public OpenGraphNamespace(string prefix, Uri schemaUri)
        {
            this.Prefix = prefix;
            this.SchemaUri = schemaUri;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGraphNamespace"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="schemaUri">The schema URI.</param>
        public OpenGraphNamespace(string prefix, string schemaUri)
            : this(prefix, new Uri(schemaUri, UriKind.RelativeOrAbsolute))
        {
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        public string Prefix { get; }

        /// <summary>
        /// Gets the schema URI.
        /// </summary>
        /// <value>
        /// The schema URI.
        /// </value>
        public Uri SchemaUri { get; }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Concat(this.Prefix, ": ", this.SchemaUri.ToString());
        }
    }

    /// <summary>
    /// The list of known supported Open Graph namespaces.
    /// </summary>
    /// <seealso cref="OpenGraphNamespace" />
    public sealed class RegistryNamespace : OpenGraphNamespace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryNamespace"/> class.
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        /// <param name="schemaUri">The schema URI.</param>
        /// <param name="requiredElements">The required elements.</param>
        public RegistryNamespace(string prefix, string schemaUri, params string[] requiredElements)
            : base(prefix, schemaUri)
        {
            this.RequiredElements = new List<string>(requiredElements);
        }

        /// <summary>
        /// Gets the required elements.
        /// </summary>
        /// <value>The required elements.</value>
        public IList<string> RequiredElements { get; }
    }

    /// <summary>
    /// Represents an Open Graph meta element.
    /// </summary>
    public abstract class MetadataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataBase" /> class.
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected MetadataBase(OpenGraphNamespace ns, string name, string value)
        {
            this.Namespace = ns;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        /// <value>
        /// The namespace.
        /// </value>
        public OpenGraphNamespace Namespace { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>
        /// The values.
        /// </value>
        public string Value { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MetadataBase"/> to <see cref="string"/>.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator string(MetadataBase element)
        {
            return element?.Value;
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.Value))
            {
                return string.Empty;
            }

            HtmlDocument doc = this.CreateDocument();

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>The HTML snippet that represents this element.</returns>
        protected internal virtual HtmlDocument CreateDocument()
        {
            var doc = new HtmlDocument();

            var meta = doc.CreateElement("meta");
            meta.Attributes.Add("property", string.Concat(this.Namespace.Prefix, ":", this.Name));
            meta.Attributes.Add("content", this.Value);
            doc.DocumentNode.AppendChild(meta);

            return doc;
        }
    }

    /// <summary>
    /// Adds some helpful extension methods.
    /// </summary>
    public static class MetadataHelperExtensions
    {
        /// <summary>
        /// Values the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The value of the first item in the list.</returns>
        public static string Value(this IList<StructuredMetadata> metadata) => Value(metadata.FirstOrDefault());

        /// <summary>
        /// Values the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The value of the first item in the list.</returns>
        public static string Value(this IList<PropertyMetadata> metadata) => Value(metadata.FirstOrDefault());

        /// <summary>
        /// Namespaces the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The namespace of the first item in the list.</returns>
        public static OpenGraphNamespace Namespace(this IList<StructuredMetadata> metadata) => Namespace(metadata.FirstOrDefault());

        /// <summary>
        /// Namespaces the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The namespace of the first item in the list.</returns>
        public static OpenGraphNamespace Namespace(this IList<PropertyMetadata> metadata) => Namespace(metadata.FirstOrDefault());

        /// <summary>
        /// Names the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The name of the first item in the list.</returns>
        public static string Name(this IList<StructuredMetadata> metadata) => Name(metadata.FirstOrDefault());

        /// <summary>
        /// Names the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The name of the first item in the list.</returns>
        public static string Name(this IList<PropertyMetadata> metadata) => Name(metadata.FirstOrDefault());

        /// <summary>
        /// Values the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The value of the item, or an empty string when null.</returns>
        private static string Value(this MetadataBase metadata) => (metadata ?? new NullMetadata()).Value ?? string.Empty;

        /// <summary>
        /// Namespaces the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The namespace of the item when present; otherwise null.</returns>
        private static OpenGraphNamespace Namespace(this MetadataBase metadata) => (metadata ?? new NullMetadata()).Namespace;

        /// <summary>
        /// Names the specified metadata.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <returns>The name of the item when present; otherwise an empty string.</returns>
        private static string Name(this MetadataBase metadata) => (metadata ?? new NullMetadata()).Name ?? string.Empty;
    }

    /// <summary>
    /// Represents a null <see cref="MetadataBase"/>.
    /// </summary>
    /// <seealso cref="OpenGraphNet.Metadata" />
    public sealed class NullMetadata : StructuredMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullMetadata"/> class.
        /// </summary>
        public NullMetadata()
            : base(null, string.Empty, string.Empty)
        {
        }
    }

    /// <summary>
    /// A property meta element.
    /// </summary>
    /// <seealso cref="OpenGraphNet.Metadata" />
    public class PropertyMetadata : MetadataBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class.
        /// </summary>
        /// <param name="parentElement">The parent element.</param>
        /// <param name="ns">The ns.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public PropertyMetadata(StructuredMetadata parentElement, OpenGraphNamespace ns, string name, string value)
            : base(ns, name, value)
        {
            this.ParentElement = parentElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetadata"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public PropertyMetadata(string name, string value)
            : this(null, null, name, value)
        {
        }

        /// <summary>
        /// Gets the parent element.
        /// </summary>
        /// <value>
        /// The parent element.
        /// </value>
        public StructuredMetadata ParentElement { get; internal set; }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>
        /// The HTML snippet that represents this element.
        /// </returns>
        protected internal override HtmlDocument CreateDocument()
        {
            var doc = new HtmlDocument();

            var meta = doc.CreateElement("meta");
            meta.Attributes.Add("property", string.Concat(this.Namespace.Prefix, ":", this.ParentElement.Name, ":", this.Name));
            meta.Attributes.Add("content", this.Value);
            doc.DocumentNode.AppendChild(meta);

            return doc;
        }
    }

    /// <summary>
    /// A root structured element.
    /// </summary>
    /// <seealso cref="OpenGraphNet.Metadata" />
    [DebuggerDisplay("{" + nameof(Name) + "}: {" + nameof(Value) + "}")]
    public class StructuredMetadata : MetadataBase
    {
        /// <summary>
        /// The internal properties.
        /// </summary>
        private readonly Dictionary<string, IList<PropertyMetadata>> internalProperties = new Dictionary<string, IList<PropertyMetadata>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StructuredMetadata"/> class.
        /// </summary>
        /// <param name="ns">The ns.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public StructuredMetadata(OpenGraphNamespace ns, string name, string value)
            : base(ns, name, value)
        {
        }

        /// <summary>
        /// Gets the child elements.
        /// </summary>
        /// <value>
        /// The child elements.
        /// </value>
        public IDictionary<string, IList<PropertyMetadata>> Properties => new ReadOnlyDictionary<string, IList<PropertyMetadata>>(this.internalProperties);

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddProperty(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = name.Replace(this.Namespace + ":", string.Empty);
            name = name.Replace(this.Name + ":", string.Empty);

            var propertyElement = new PropertyMetadata(this, this.Namespace, name, value);
            this.AddProperty(propertyElement);
        }

        /// <summary>
        /// Adds the property.
        /// </summary>
        /// <param name="element">The element.</param>
        public void AddProperty(PropertyMetadata element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.ParentElement = this;
            element.Namespace = this.Namespace;

            if (this.internalProperties.ContainsKey(element.Name))
            {
                this.internalProperties[element.Name].Add(element);
            }
            else
            {
                this.internalProperties.Add(element.Name, new List<PropertyMetadata>() { element });
            }
        }

        /// <summary>
        /// Determines whether the specified property key is a property of this element.
        /// </summary>
        /// <param name="propertyKey">The property key.</param>
        /// <returns>
        ///   <c>true</c> if the specified property key is a child property of this element; otherwise, <c>false</c>.
        /// </returns>
        public bool IsMyProperty(string propertyKey)
        {
            if (propertyKey == null)
            {
                throw new ArgumentNullException(nameof(propertyKey));
            }

            var myKey = string.Concat(this.Namespace.Prefix, ":", this.Name);
            return propertyKey.StartsWith(myKey, StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyKey, myKey, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Creates the document.
        /// </summary>
        /// <returns>
        /// The HTML snippet that represents this element.
        /// </returns>
        protected internal override HtmlDocument CreateDocument()
        {
            var doc = base.CreateDocument();

            var elements = this.Properties.SelectMany(p => p.Value);

            foreach (var metaElement in elements)
            {
                doc.DocumentNode.AppendChild(metaElement.CreateDocument().DocumentNode);
            }

            return doc;
        }
    }

    /// <summary>
    /// A collection class to contain <see cref="StructuredMetadata"/> objects.
    /// </summary>
    public class StructuredMetadataDictionary : IDictionary<string, IList<StructuredMetadata>>
    {
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public int Count => this.InternalCollection.Count;

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> is read-only.
        /// </summary>
        public bool IsReadOnly => this.InternalCollection.IsReadOnly;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        public ICollection<string> Keys => this.InternalCollection.Keys;

        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"></see> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        public ICollection<IList<StructuredMetadata>> Values => this.InternalCollection.Values;

        /// <summary>
        /// Gets the internal collection.
        /// </summary>
        /// <value>
        /// The internal collection.
        /// </value>
        private IDictionary<string, IList<StructuredMetadata>> InternalCollection { get; } = new Dictionary<string, IList<StructuredMetadata>>();

        /// <summary>
        /// Gets or sets the <see cref="IList{StructuredMetadata}"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="IList{StructuredMetadata}"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns>The metadata at the current specified key.</returns>
        public IList<StructuredMetadata> this[string key]
        {
            get
            {
                var ns = NamespaceRegistry.DefaultNamespace;

                if (key?.IndexOf(':') < 0)
                {
                    key = string.Concat(ns.Prefix, ":", key);
                }

                if (!this.InternalCollection.ContainsKey(key))
                {
                    return new List<StructuredMetadata>();
                }

                return this.InternalCollection[key];
            }

            set => this.InternalCollection[key] = value;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, IList<StructuredMetadata>>> GetEnumerator() => this.InternalCollection.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        public void Add(KeyValuePair<string, IList<StructuredMetadata>> item) => this.InternalCollection.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        public void Clear() => this.InternalCollection.Clear();

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"></see> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> is found in the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, IList<StructuredMetadata>> item) => this.InternalCollection.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(KeyValuePair<string, IList<StructuredMetadata>>[] array, int arrayIndex) => this.InternalCollection.CopyTo(array, arrayIndex);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"></see>.</param>
        /// <returns>
        /// true if <paramref name="item">item</paramref> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"></see>; otherwise, false. This method also returns false if <paramref name="item">item</paramref> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"></see>.
        /// </returns>
        public bool Remove(KeyValuePair<string, IList<StructuredMetadata>> item) => this.InternalCollection.Remove(item);

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(string key, IList<StructuredMetadata> value) => this.InternalCollection.Add(key, value);

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key) => this.InternalCollection.ContainsKey(key);

        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key">key</paramref> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"></see>.
        /// </returns>
        public bool Remove(string key) => this.InternalCollection.Remove(key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"></see> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out IList<StructuredMetadata> value) => this.InternalCollection.TryGetValue(key, out value);
    }
}