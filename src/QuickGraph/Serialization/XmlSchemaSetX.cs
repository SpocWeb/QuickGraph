using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace QuickGraph.Serialization
{
    public static class XmlSchemaSetX
    {
        public static XmlSchemaSet AsXmlSchemaSet(this IEnumerable<KeyValuePair<string, Stream>> streamsByName)
            => new XmlSchemaSet().AddSchemas(streamsByName);

        public static XmlSchemaSet AddSchemas(this XmlSchemaSet xmlSchemaSet, IEnumerable<KeyValuePair<string, Stream>> streamsByName)
        {
            foreach (var pair in streamsByName)
            {
                xmlSchemaSet.AddSchema(pair);
            }

            return xmlSchemaSet;
        }

        public static XmlSchema AddSchema(this XmlSchemaSet xmlSchemaSet, KeyValuePair<string, Stream> pair)
            => AddSchema(xmlSchemaSet, pair.Value);

        public static XmlSchema AddSchema(this XmlSchemaSet xmlSchemaSet, Stream pair)
        {
            var xmlSchema = XmlSchema.Read(pair, null);
            return xmlSchemaSet.Add(xmlSchema);
        }
    }
}