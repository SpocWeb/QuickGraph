using System;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics.Contracts;
using System.ComponentModel;

namespace QuickGraph.Serialization
{
    public static class XmlReaderExtensions
    {
        public static bool[] ReadElementContentAsBooleanArray(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => Convert.ToBoolean(s));
        }

        public static int[] ReadElementContentAsInt32Array(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => Convert.ToInt32(s));
        }

        public static long[] ReadElementContentAsInt64Array(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => Convert.ToInt64(s));
        }

        public static float[] ReadElementContentAsSingleArray(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => Convert.ToSingle(s));
        }

        public static double[] ReadElementContentAsDoubleArray(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => Convert.ToDouble(s));
        }

        public static string[] ReadElementContentAsStringArray(XmlReader xmlReader, string localName, string namespaceURI)
        {
            return ReadElementContentAsArray(xmlReader, localName, namespaceURI, s => s);
        }

        /// <summary>
        /// Read contents of an XML element as an array of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlReader"></param>
        /// <param name="localName"></param>
        /// <param name="namespaceURI"></param>
        /// <param name="stringToT"></param>
        /// <returns></returns>
        public static T[] ReadElementContentAsArray<T>(XmlReader xmlReader, string localName, string namespaceURI,
                                                        Func<string, T> stringToT)
        {
            var str = xmlReader.ReadElementContentAsString(localName, namespaceURI);

            if (str == "null")
                return null;

            if (str.Length > 0 && str[str.Length - 1] == ' ')
            {
                str = str.Remove(str.Length - 1);
            }

            var strArray = str.Split(new char[1] { ' ' });

            var array = new T[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                array[i] = stringToT(strArray[i]);
            }
            return array;
        }
    }

    /// <summary>
    /// A GraphML ( http://graphml.graphdrawing.org/ ) format deserializer.
    /// </summary>
    /// <typeparam name="TVertex">type of a vertex</typeparam>
    /// <typeparam name="TEdge">type of an edge</typeparam>
    /// <typeparam name="TGraph">type of the graph</typeparam>
    /// <remarks>
    /// <para>
    /// Custom vertex, edge and graph attributes can be specified by 
    /// using the <see cref="System.Xml.Serialization.XmlAttributeAttribute"/>
    /// attribute on properties (field not suppored).
    /// </para>
    /// <para>
    /// The serializer uses LCG (lightweight code generation) to generate the 
    /// methods that writes the attributes to avoid paying the price of 
    /// Reflection on each vertex/edge. Since nothing is for free, the first
    /// time you will use the serializer *on a particular pair of types*, it
    /// will have to bake that method.
    /// </para>
    /// <para>
    /// Hyperedge, nodes, nested graphs not supported.
    /// </para>
    /// </remarks>
    public sealed class GraphMLDeserializer<TVertex, TEdge, TGraph>
        : SerializerBase
        where TEdge : IEdge<TVertex>
        where TGraph : IMutableVertexAndEdgeSet<TVertex, TEdge>
    {
        #region Compiler
        delegate void ReadVertexAttributesDelegate(
            XmlReader reader,
            string namespaceUri,
            TVertex v);
        delegate void ReadEdgeAttributesDelegate(
            XmlReader reader,
            string namespaceUri,
            TEdge e);
        delegate void ReadGraphAttributesDelegate(
            XmlReader reader,
            string namespaceUri,
            TGraph g);

        static class ReadDelegateCompiler
        {
            public static readonly ReadVertexAttributesDelegate VertexAttributesReader;
            public static readonly ReadEdgeAttributesDelegate EdgeAttributesReader;
            public static readonly ReadGraphAttributesDelegate GraphAttributesReader;
            public static readonly Action<TVertex> SetVertexDefault;
            public static readonly Action<TEdge> SetEdgeDefault;
            public static readonly Action<TGraph> SetGraphDefault; 

            static ReadDelegateCompiler()
            {
                VertexAttributesReader =
                    (ReadVertexAttributesDelegate)CreateReadDelegate(
                    typeof(ReadVertexAttributesDelegate),
                    typeof(TVertex)
                    //,"id"
                    );
                EdgeAttributesReader =
                    (ReadEdgeAttributesDelegate)CreateReadDelegate(
                    typeof(ReadEdgeAttributesDelegate),
                    typeof(TEdge)
                    //,"id", "source", "target"
                    );
                GraphAttributesReader =
                    (ReadGraphAttributesDelegate)CreateReadDelegate(
                    typeof(ReadGraphAttributesDelegate),
                    typeof(TGraph)
                    );
                SetVertexDefault =
                    (Action<TVertex>)CreateSetDefaultDelegate(
                        typeof(Action<TVertex>),
                        typeof(TVertex)
                    );
                SetEdgeDefault =
                    (Action<TEdge>)CreateSetDefaultDelegate(
                        typeof(Action<TEdge>),
                        typeof(TEdge)
                    );
                SetGraphDefault =
                    (Action<TGraph>)CreateSetDefaultDelegate(
                        typeof(Action<TGraph>),
                        typeof(TGraph)
                    );
            }

            static class Metadata
            {
                public static readonly MethodInfo ReadToFollowingMethod =
                    typeof(XmlReader).GetMethod(
                        "ReadToFollowing",
                        BindingFlags.Instance | BindingFlags.Public,
                        null,
                        new Type[] { typeof(string), typeof(string) },
                        null);
                public static readonly MethodInfo GetAttributeMethod =
                    typeof(XmlReader).GetMethod(
                        "GetAttribute",
                        BindingFlags.Instance | BindingFlags.Public,
                        null,
                        new Type[] { typeof(string) },
                        null);
                public static readonly PropertyInfo NameProperty =
                    typeof(XmlReader).GetProperty("Name");
                public static readonly PropertyInfo NamespaceUriProperty =
                    typeof(XmlReader).GetProperty("NamespaceUri");
                public static readonly MethodInfo StringEqualsMethod =
                    typeof(string).GetMethod(
                        "op_Equality",
                        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                        null,
                        new Type[] { typeof(string), typeof(string) },
                        null);
                public static readonly ConstructorInfo ArgumentExceptionCtor =
                    typeof(ArgumentException).GetConstructor(new Type[] { typeof(string) });

                private static readonly Dictionary<Type, MethodInfo> ReadContentMethods;

                static Metadata()
                {
                    ReadContentMethods = new Dictionary<Type, MethodInfo>();
                    ReadContentMethods.Add(typeof(bool), typeof(XmlReader).GetMethod("ReadElementContentAsBoolean", new Type[] { typeof(string), typeof(string) }));
                    ReadContentMethods.Add(typeof(int), typeof(XmlReader).GetMethod("ReadElementContentAsInt", new Type[] { typeof(string), typeof(string) }));
                    ReadContentMethods.Add(typeof(long), typeof(XmlReader).GetMethod("ReadElementContentAsLong", new Type[] { typeof(string), typeof(string) }));
                    ReadContentMethods.Add(typeof(float), typeof(XmlReader).GetMethod("ReadElementContentAsFloat", new Type[] { typeof(string), typeof(string) }));
                    ReadContentMethods.Add(typeof(double), typeof(XmlReader).GetMethod("ReadElementContentAsDouble", new Type[] { typeof(string), typeof(string) }));
                    ReadContentMethods.Add(typeof(string), typeof(XmlReader).GetMethod("ReadElementContentAsString", new Type[] { typeof(string), typeof(string) }));

                    var readerExtensions = typeof(XmlReaderExtensions);
                    ReadContentMethods.Add(typeof(bool[]), readerExtensions.GetMethod("ReadElementContentAsBooleanArray"));
                    ReadContentMethods.Add(typeof(int[]), readerExtensions.GetMethod("ReadElementContentAsInt32Array"));
                    ReadContentMethods.Add(typeof(long[]), readerExtensions.GetMethod("ReadElementContentAsInt64Array"));
                    ReadContentMethods.Add(typeof(float[]), readerExtensions.GetMethod("ReadElementContentAsSingleArray"));
                    ReadContentMethods.Add(typeof(double[]), readerExtensions.GetMethod("ReadElementContentAsDoubleArray"));
                    ReadContentMethods.Add(typeof(string[]), readerExtensions.GetMethod("ReadElementContentAsStringArray"));

                    ReadContentMethods.Add(typeof(IList<bool>), readerExtensions.GetMethod("ReadElementContentAsBooleanArray"));
                    ReadContentMethods.Add(typeof(IList<int>), readerExtensions.GetMethod("ReadElementContentAsInt32Array"));
                    ReadContentMethods.Add(typeof(IList<long>), readerExtensions.GetMethod("ReadElementContentAsInt64Array"));
                    ReadContentMethods.Add(typeof(IList<float>), readerExtensions.GetMethod("ReadElementContentAsSingleArray"));
                    ReadContentMethods.Add(typeof(IList<double>), readerExtensions.GetMethod("ReadElementContentAsDoubleArray"));
                    ReadContentMethods.Add(typeof(IList<string>), readerExtensions.GetMethod("ReadElementContentAsStringArray"));
                }

                public static bool TryGetReadContentMethod(Type type, out MethodInfo method)
                {
                    Contract.Requires(type != null);

                    bool result = ReadContentMethods.TryGetValue(type, out method);
                    Contract.Assert(!result || method != null, type.FullName);
                    return result;
                }
            }

            public static Delegate CreateSetDefaultDelegate(
                Type delegateType,
                Type elementType
                )
            {
                Contract.Requires(delegateType != null);
                Contract.Requires(elementType != null);

                var method = new DynamicMethod(
                    "Set" + elementType.Name + "Default",
                    typeof(void),
                    new Type[] { elementType },
                    elementType.Module
                    );
                var gen = method.GetILGenerator();

                // we need to create the swicth for each property
                foreach (var kv in SerializationHelper.GetAttributeProperties(elementType))
                {
                    var property = kv.Property;
                    var defaultValueAttribute = Attribute.GetCustomAttribute(property, typeof(DefaultValueAttribute))
                        as DefaultValueAttribute;
                    if (defaultValueAttribute == null)
                        continue;
                    var setMethod = property.GetSetMethod();
                    if (setMethod == null)
                        throw new InvalidOperationException("property " + property.Name + " is not settable");
                    if (property.PropertyType.IsArray)
                    {
                        throw new NotImplementedException("Default values for array types are not implemented");
                    }                    
                    var value = defaultValueAttribute.Value;
                    if (value != null &&
                        value.GetType() != property.PropertyType)
                        throw new InvalidOperationException("invalid default value type of property " + property.Name);
                    gen.Emit(OpCodes.Ldarg_0);
                    switch (Type.GetTypeCode(property.PropertyType))
                    {
                        case TypeCode.Int32:
                            gen.Emit(OpCodes.Ldc_I4, (int)value);
                            break;
                        case TypeCode.Int64:
                            gen.Emit(OpCodes.Ldc_I8, (long)value);
                            break;
                        case TypeCode.Single:
                            gen.Emit(OpCodes.Ldc_R4, (float)value);
                            break;
                        case TypeCode.Double:
                            gen.Emit(OpCodes.Ldc_R8, (double)value);
                            break;
                        case TypeCode.String:
                            gen.Emit(OpCodes.Ldstr, (string)value);
                            break;
                        case TypeCode.Boolean:
                            gen.Emit((bool)value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                            break;
                        default:
                            throw new InvalidOperationException("unsupported type " + property.PropertyType);
                    }
                    gen.EmitCall(setMethod.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, setMethod, null);
                }
                gen.Emit(OpCodes.Ret);

                //let's bake the method
                return method.CreateDelegate(delegateType);
            }

            public static Delegate CreateReadDelegate(
                Type delegateType,
                Type elementType
                //,params string[] ignoredAttributes
                )
            {
                Contract.Requires(delegateType != null);
                Contract.Requires(elementType != null);

                var method = new DynamicMethod(
                    "Read" + elementType.Name,
                    typeof(void),
                    //          reader, namespaceUri
                    new Type[] { typeof(XmlReader), typeof(string), elementType },
                    elementType.Module
                    );
                var gen = method.GetILGenerator();

                var key = gen.DeclareLocal(typeof(string));

                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldstr, "key");
                gen.EmitCall(OpCodes.Callvirt, Metadata.GetAttributeMethod, null);
                gen.Emit(OpCodes.Stloc_0);

                //// if (String.Equals(key, "id")) continue;
                //foreach (string ignoredAttribute in ignoredAttributes)
                //{
                //    gen.Emit(OpCodes.Ldloc_0);
                //    gen.Emit(OpCodes.Ldstr, ignoredAttribute);
                //    gen.EmitCall(OpCodes.Call, Metadata.StringEqualsMethod, null);
                //    gen.Emit(OpCodes.Brtrue, doWhile);
                //}

                // we need to create the swicth for each property
                var next = gen.DefineLabel();
                var @return = gen.DefineLabel();
                bool first = true;
                foreach (var kv in SerializationHelper.GetAttributeProperties(elementType))
                {
                    var property = kv.Property;

                    if (!first)
                    {
                        gen.MarkLabel(next);
                        next = gen.DefineLabel();
                    }
                    first = false;

                    // if (!key.Equals("foo"))
                    gen.Emit(OpCodes.Ldloc_0);
                    gen.Emit(OpCodes.Ldstr, kv.Name);
                    gen.EmitCall(OpCodes.Call, Metadata.StringEqualsMethod, null);
                    // if false jump to next
                    gen.Emit(OpCodes.Brfalse, next);

                    // do our stuff
                    MethodInfo readMethod = null;
                    if (!Metadata.TryGetReadContentMethod(property.PropertyType, out readMethod))
                        throw new ArgumentException(string.Format("Property {0} has a non-supported type", property.Name));

                    // do we have a set method ?
                    var setMethod = property.GetSetMethod();
                    if (setMethod == null)
                        throw new ArgumentException(string.Format("Property {0}.{1} has not set method", property.DeclaringType, property.Name));
                    // reader.ReadXXX
                    gen.Emit(OpCodes.Ldarg_2); // element
                    gen.Emit(OpCodes.Ldarg_0); // reader
                    gen.Emit(OpCodes.Ldstr, "data");
                    gen.Emit(OpCodes.Ldarg_1); // namespace uri
                    // When writing scalar values we call member methods of XmlReader, while for array values 
                    // we call our own static methods.  These two types of methods seem to need different opcode.
                    var opcode = readMethod.DeclaringType == typeof(XmlReaderExtensions) ? OpCodes.Call : OpCodes.Callvirt;
                    gen.EmitCall(opcode, readMethod, null);
                    gen.EmitCall(OpCodes.Callvirt, setMethod, null);

                    // jump to do while
                    gen.Emit(OpCodes.Br, @return);
                }

                // we don't know this parameter.. we throw
                gen.MarkLabel(next);
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Newobj, Metadata.ArgumentExceptionCtor);
                gen.Emit(OpCodes.Throw);

                gen.MarkLabel(@return);
                gen.Emit(OpCodes.Ret);

                //let's bake the method
                return method.CreateDelegate(delegateType);
            }
        }
        #endregion

        public void Deserialize(
            XmlReader reader,
            TGraph visitedGraph,
            IdentifiableVertexFactory<TVertex> vertexFactory,
            IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory)
        {
            Contract.Requires(reader != null);
            Contract.Requires(visitedGraph != null);
            Contract.Requires(vertexFactory != null);
            Contract.Requires(edgeFactory != null);

            var worker = new ReaderWorker(
                this,
                reader,
                visitedGraph,
                vertexFactory,
                edgeFactory);
            worker.Deserialize();
        }

        class ReaderWorker
        {
            private readonly GraphMLDeserializer<TVertex, TEdge, TGraph> serializer;
            private readonly XmlReader reader;
            private readonly TGraph visitedGraph;
            private readonly IdentifiableVertexFactory<TVertex> vertexFactory;
            private readonly IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory;
            private string graphMLNamespace = "";

            public ReaderWorker(
                GraphMLDeserializer<TVertex, TEdge, TGraph> serializer,
                XmlReader reader,
                TGraph visitedGraph,
                IdentifiableVertexFactory<TVertex> vertexFactory,
                IdentifiableEdgeFactory<TVertex, TEdge> edgeFactory
                )
            {
                Contract.Requires(serializer != null);
                Contract.Requires(reader != null);
                Contract.Requires(visitedGraph != null);
                Contract.Requires(vertexFactory != null);
                Contract.Requires(edgeFactory != null);

                this.serializer = serializer;
                this.reader = reader;
                this.visitedGraph = visitedGraph;
                this.vertexFactory = vertexFactory;
                this.edgeFactory = edgeFactory;
            }

            public GraphMLDeserializer<TVertex, TEdge, TGraph> Serializer => serializer;

            public XmlReader Reader => reader;

            public TGraph VisitedGraph => visitedGraph;

            public void Deserialize()
            {
                ReadHeader();
                ReadGraphHeader();
                ReadElements();
            }

            private void ReadHeader()
            {
                // read flow until we hit the graphml node
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == "graphml")
                    {
                        graphMLNamespace = reader.NamespaceURI;
                        return;
                    }
                }

                throw new ArgumentException("graphml node not found");
            }

            private void ReadGraphHeader()
            {
                if (!Reader.ReadToDescendant("graph", graphMLNamespace))
                    throw new ArgumentException("graph node not found");
            }

            private void ReadElements()
            {
                Contract.Requires(
                    Reader.Name == "graph" &&
                    Reader.NamespaceURI == graphMLNamespace,
                    "incorrect reader position");

                ReadDelegateCompiler.SetGraphDefault(VisitedGraph);

                var vertices = new Dictionary<string, TVertex>(StringComparer.Ordinal);

                // read vertices or edges
                var reader = Reader;
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.NamespaceURI == graphMLNamespace)
                    {
                        switch (reader.Name)
                        {
                            case "node":
                                ReadVertex(vertices);
                                break;
                            case "edge":
                                ReadEdge(vertices);
                                break;
                            case "data":
                                ReadDelegateCompiler.GraphAttributesReader(Reader, graphMLNamespace, VisitedGraph);
                                break;
                            default:
                                throw new InvalidOperationException(string.Format("invalid reader position {0}:{1}", Reader.NamespaceURI, Reader.Name));
                        }
                    }
                }
            }

            private void ReadEdge(Dictionary<string, TVertex> vertices)
            {
                Contract.Requires(vertices != null);
                Contract.Assert(
                    Reader.NodeType == XmlNodeType.Element &&
                    Reader.Name == "edge" &&
                    Reader.NamespaceURI == graphMLNamespace);

                // get subtree
                using (var subReader = Reader.ReadSubtree())
                {
                    // read id
                    string id = ReadAttributeValue(Reader, "id");
                    string sourceid = ReadAttributeValue(Reader, "source");
                    TVertex source;
                    if (!vertices.TryGetValue(sourceid, out source))
                        throw new ArgumentException("Could not find vertex " + sourceid);
                    string targetid = ReadAttributeValue(Reader, "target");
                    TVertex target;
                    if (!vertices.TryGetValue(targetid, out target))
                        throw new ArgumentException("Could not find vertex " + targetid);

                    var edge = edgeFactory(source, target, id);
                    ReadDelegateCompiler.SetEdgeDefault(edge);

                    // read data
                    while (subReader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element &&
                            reader.Name == "data" &&
                            reader.NamespaceURI == graphMLNamespace)
                            ReadDelegateCompiler.EdgeAttributesReader(subReader, graphMLNamespace, edge);
                    }

                    VisitedGraph.AddEdge(edge);
                }
            }

            private void ReadVertex(Dictionary<string, TVertex> vertices)
            {
                Contract.Requires(vertices != null);
                Contract.Assert(
                    Reader.NodeType == XmlNodeType.Element &&
                    Reader.Name == "node" &&
                    Reader.NamespaceURI == graphMLNamespace);

                // get subtree
                using (var subReader = Reader.ReadSubtree())
                {
                    // read id
                    string id = ReadAttributeValue(Reader, "id");
                    // create new vertex
                    TVertex vertex = vertexFactory(id);
                    // apply defaults
                    ReadDelegateCompiler.SetVertexDefault(vertex);
                    // read data
                    while (subReader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element &&
                            reader.Name == "data" &&
                            reader.NamespaceURI == graphMLNamespace)
                            ReadDelegateCompiler.VertexAttributesReader(subReader, graphMLNamespace, vertex);
                    }
                    // add to graph
                    VisitedGraph.AddVertex(vertex);
                    vertices.Add(id, vertex);
                }
            }

            private static string ReadAttributeValue(XmlReader reader, string attributeName)
            {
                Contract.Requires(reader != null);
                Contract.Requires(attributeName != null);
                reader.MoveToAttribute(attributeName);
                if (!reader.ReadAttributeValue())
                    throw new ArgumentException("missing " + attributeName + " attribute");
                return reader.Value;
            }
        }
    }
}
