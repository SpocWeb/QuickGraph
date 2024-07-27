using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Schema;

namespace QuickGraph.Serialization
{
    /// <summary> A resolver that loads graphml DTD and XSD schemas from embedded resources instead of URLs. </summary>
    public sealed class GraphMlXmlResolver 
        : XmlResolver
    {
        public const string GraphMlNamespace = "http://graphml.graphdrawing.org/xmlns";
        public const string GraphMlDtdNs = "http://www.graphdrawing.org/dtds/graphml.dtd";
        public const string GraphmlXsdNs = "http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";
        public const string GraphmlStructureXsd = "http://graphml.graphdrawing.org/xmlns/1.0/graphml-structure.xsd";

        // ReSharper disable InconsistentNaming
        public static Stream GraphMl_Dtd() => typeof(GraphMlExtensions).Assembly.GetManifestResourceStream(typeof(GraphMlExtensions), "graphml.dtd");
        public static Stream Graphml_Xsd() => typeof(GraphMlExtensions).Assembly.GetManifestResourceStream(typeof(GraphMlExtensions), "graphml.xsd");
        public static Stream Graphml_Structure_Xsd() => typeof(GraphExtensions).Assembly.GetManifestResourceStream(typeof(GraphMlExtensions), "graphml-structure.xsd");
        // ReSharper restore InconsistentNaming

        public static readonly GraphMlXmlResolver GraphMlStructureXsdResolver
            = new GraphMlXmlResolver(new Uri(GraphmlStructureXsd), Graphml_Structure_Xsd);

        public static readonly GraphMlXmlResolver GraphMlXsdResolver
            = new GraphMlXmlResolver(new Uri(GraphmlXsdNs), Graphml_Xsd, GraphMlStructureXsdResolver);

        public Uri Uri { get; }
        public Func<Stream> Stream { get; }

        readonly XmlResolver baseResolver;

        public GraphMlXmlResolver(Uri uri, Func<Stream> stream, XmlResolver baseResolver = null)
        {
            Uri = uri;
            Stream = stream;
            this.baseResolver = baseResolver ?? new XmlUrlResolver();
        }


        private static readonly Dictionary<string, Stream> _StreamsByName = new Dictionary<string, Stream>
        {
            //{ GraphMlDtdNs, GraphMl_Dtd },
            //{ GraphmlXsdNs, Graphml_Xsd },
            { GraphmlStructureXsd, Graphml_Structure_Xsd() },
        };

        public static IReadOnlyDictionary<string, Stream> StreamsByName => _StreamsByName;

        public static XmlSchemaSet XmlSchemaSet = new XmlSchemaSet
        {
            XmlResolver = GraphMlXsdResolver
        };// StreamsByName.AsXmlSchemaSet();

        static GraphMlXmlResolver()
        {
            XmlSchemaSet.AddSchema(Graphml_Structure_Xsd());
            XmlSchemaSet.AddSchema(Graphml_Xsd());
        }

#if !SILVERLIGHT
        ICredentials _credentials;
        public override ICredentials Credentials
        {
            set => _credentials = value;
        }
#endif
 
        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if (absoluteUri == Uri)
            {
                //Stream.Close();
                //Stream.Seek(0, SeekOrigin.Begin);
                return Stream();
            }
            //if (absoluteUri.AbsoluteUri == GraphMlDtdNs)
            //    return GraphMl_Dtd;
            //if (absoluteUri.AbsoluteUri == GraphmlXsdNs)
            //    return Graphml_Xsd;
            //if (absoluteUri.AbsoluteUri == GraphmlStructureXsd)
            //    return Graphml_Structure_Xsd;

            return baseResolver.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
