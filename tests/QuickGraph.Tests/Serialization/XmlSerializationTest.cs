﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.XPath;
using System.Xml;
using QuickGraph.Serialization;

namespace QuickGraph.Tests.Serialization
{
    [TestClass]
    public class XmlSerializationTest
    {
        [TestMethod]
        public void DeserializeFromXml()
        {
            var doc = new XPathDocument("GraphML/repro12273.xml");
            var ug = SerializationExtensions.DeserializeFromXml(doc,
                "graph", "node", "edge",
                _ => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                nav => nav.GetAttribute("id", ""),
                nav => new TaggedEdge<string, double>(
                    nav.GetAttribute("source", ""),
                    nav.GetAttribute("target", ""),
                    int.Parse(nav.GetAttribute("weight", ""))
                    )
                );

            var ug2 = SerializationExtensions.DeserializeFromXml(
                XmlReader.Create("GraphML/repro12273.xml"),
                "graph", "node", "edge", "",
                _ => new UndirectedGraph<string, TaggedEdge<string, double>>(),
                reader => reader.GetAttribute("id"),
                reader => new TaggedEdge<string, double>(
                    reader.GetAttribute("source"),
                    reader.GetAttribute("target"),
                    int.Parse(reader.GetAttribute("weight"))
                    )
                );

            Assert.AreEqual(ug.VertexCount, ug2.VertexCount);
            Assert.AreEqual(ug.EdgeCount, ug2.EdgeCount);
        }
    }
}
