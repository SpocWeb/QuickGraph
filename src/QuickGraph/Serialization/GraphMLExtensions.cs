using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Diagnostics.Contracts;
using QuickGraph.Algorithms;

namespace QuickGraph.Serialization
{
    public static class GraphMlExtensions
    {

#if !SILVERLIGHT

        // The following use of XmlWriter.Create fails in Silverlight.

        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            string fileName,
            VertexIdentity<TVertex> vertexIdentities,
            EdgeIdentity<TVertex, TEdge> edgeIdentities)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(fileName != null);
            Contract.Requires(fileName.Length > 0);

            var settings = new XmlWriterSettings() { Indent = true, IndentChars = "    " };
            var writer = XmlWriter.Create(fileName, settings);
            SerializeToGraphML(graph, writer, vertexIdentities, edgeIdentities);
            writer.Flush();
            writer.Close();
        }

        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            string fileName
            )
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(fileName != null);
            Contract.Requires(fileName.Length > 0);

            var settings = new XmlWriterSettings() { Indent = true, IndentChars = "    " };
            var writer = XmlWriter.Create(fileName, settings);
            SerializeToGraphML<TVertex, TEdge, TGraph>(graph, writer);
            writer.Flush();
            writer.Close();
        }

#endif

        public static void SerializeToGraphML<TVertex, TEdge,TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            XmlWriter writer,
            VertexIdentity<TVertex> vertexIdentities,
            EdgeIdentity<TVertex, TEdge> edgeIdentities)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            var serializer = new GraphMLSerializer<TVertex, TEdge,TGraph>();
            serializer.Serialize(writer, graph, vertexIdentities, edgeIdentities);
        }

        public static void SerializeToGraphML<TVertex, TEdge, TGraph>(
#if !NET20
this 
#endif
            TGraph graph,
            XmlWriter writer)
            where TEdge : IEdge<TVertex>
            where TGraph : IEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(graph != null);
            Contract.Requires(writer != null);

            var vertexIdentity = graph.GetVertexIdentity();
            var edgeIdentity = graph.GetEdgeIdentity();

            SerializeToGraphML(
                graph,
                writer,
                vertexIdentity,
                edgeIdentity
                );
        }

#if !SILVERLIGHT

        public static void DeserializeFromGraphML<TVertex, TEdge,TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            string fileName,
            IdentifiableVertexFactory<TVertex> vertexFactory,
            IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory
            )
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(fileName != null);
            Contract.Requires(fileName.Length > 0);

            var reader = new StreamReader(fileName);
            DeserializeFromGraphML(graph, reader, vertexFactory, edgeFactory);
        }

        public static void DeserializeFromGraphML<TVertex, TEdge,TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            TextReader reader,
            IdentifiableVertexFactory<TVertex> vertexFactory,
            IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory
            )
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(graph != null);
            Contract.Requires(reader != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);

            var settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Ignore, // settings.ProhibitDtd = false;
                XmlResolver = GraphMlXmlResolver.GraphMlDtdResolver,
#if !SILVERLIGHT
                ValidationFlags = XmlSchemaValidationFlags.None
#endif
            };

            using (var xreader = XmlReader.Create(reader, settings))
                DeserializeFromGraphML(graph, xreader, vertexFactory, edgeFactory);
        }

        public static void DeserializeFromGraphML<TVertex, TEdge,TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            XmlReader reader,
            IdentifiableVertexFactory<TVertex> vertexFactory,
            IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory
            )
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(graph != null);
            Contract.Requires(reader != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);

            var serializer = new GraphMLDeserializer<TVertex, TEdge,TGraph>();
            serializer.Deserialize(reader, graph, vertexFactory, edgeFactory);
        }

        public static void DeserializeAndValidateFromGraphML<TVertex, TEdge,TGraph>(
#if !NET20
            this 
#endif
            TGraph graph,
            TextReader reader,
            IdentifiableVertexFactory<TVertex> vertexFactory,
            IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory
            )
            where TEdge : IEdge<TVertex>
            where TGraph : IMutableVertexAndEdgeListGraph<TVertex, TEdge>
        {
            Contract.Requires(graph != null);
            Contract.Requires(reader != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);

            var serializer = new GraphMLDeserializer<TVertex, TEdge,TGraph>();
            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.None, //Schema,
                XmlResolver = GraphMlXmlResolver.GraphMlDtdResolver, 
                Schemas = GraphMlXmlResolver.XmlSchemaSet,
            };
            AddGraphMLSchema(settings);

            try
            {
                settings.ValidationEventHandler += ValidationEventHandler;

                // reader and validating
                using (var xReader = XmlReader.Create(reader, settings))
                    serializer.Deserialize(xReader, graph, vertexFactory, edgeFactory);
            }
            finally
            {
                settings.ValidationEventHandler -= ValidationEventHandler;
            }
        }

        private static void AddGraphMLSchema(XmlReaderSettings settings)
        {
            using (var xsdStream = typeof(GraphMlExtensions).Assembly.GetManifestResourceStream(typeof(GraphMlExtensions), "graphml.xsd"))
            using (var xsdReader = XmlReader.Create(xsdStream, settings))
                settings.Schemas.Add(GraphMlXmlResolver.GraphMlNamespace, xsdReader);
        }

        static void ValidationEventHandler(object sender, System.Xml.Schema.ValidationEventArgs e)
        {
            if(e.Severity == XmlSeverityType.Error)
                throw new InvalidOperationException(e.Message);
        }
#endif
    }
}
