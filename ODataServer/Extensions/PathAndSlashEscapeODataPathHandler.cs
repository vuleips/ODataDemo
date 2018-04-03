using Microsoft.AspNet.OData.Routing;
using System;
using System.Text;

namespace ODataServer.Extensions
{
    // NOTE: please do not use in production
    // Reference: https://github.com/OData/ODataSamples/tree/master/WebApi/v4/ODataPathAndSlashEscapeSample
    public class PathAndSlashEscapeODataPathHandler : DefaultODataPathHandler
    {
        private const string EscapedQuote = "'";

        public override ODataPath Parse(string serviceRoot, string odataPath, IServiceProvider requestContainer)
        {
            if (!odataPath.Contains(EscapedQuote))
            {
                return base.Parse(serviceRoot, odataPath, requestContainer);
            }

            var pathBuilder = new StringBuilder();
            var queryStringIndex = odataPath.IndexOf('?');
            if (queryStringIndex == -1)
            {
                EscapeSlashBackslash(odataPath, pathBuilder);
            }
            else
            {
                EscapeSlashBackslash(odataPath.Substring(0, queryStringIndex), pathBuilder);
                pathBuilder.Append(odataPath.Substring(queryStringIndex));
            }
            return base.Parse(serviceRoot, pathBuilder.ToString(), requestContainer);
        }

        private void EscapeSlashBackslash(string uri, StringBuilder pathBuilder)
        {
            const string slash = "%2F";
            const string backSlash = "%5C";
            const string sharp = "%23";

            var startIndex = uri.IndexOf(EscapedQuote, StringComparison.OrdinalIgnoreCase);
            var endIndex = uri.IndexOf(EscapedQuote, startIndex + EscapedQuote.Length, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1 || endIndex == -1)
            {
                pathBuilder.Append(uri);
                return;
            }

            endIndex = endIndex + EscapedQuote.Length;
            pathBuilder.Append(uri.Substring(0, startIndex));
            for (var i = startIndex; i < endIndex; ++i)
            {
                switch (uri[i])
                {
                    case '/':
                        pathBuilder.Append(slash);
                        break;
                    case '\\':
                        pathBuilder.Append(backSlash);
                        break;
                    case '#':
                        pathBuilder.Append(sharp);
                        break;
                    default:
                        pathBuilder.Append(uri[i]);
                        break;
                }
            }
            EscapeSlashBackslash(uri.Substring(endIndex), pathBuilder);
        }
    }
}
