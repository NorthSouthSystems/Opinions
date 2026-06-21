using System.Xml;
using System.Xml.Linq;

namespace NorthSouthSystems.Xml.Linq;

public static class XElementSimpleStreamer
{
    /// <summary>
    /// Provides a simple (primitive) way to stream XElements from an XmlReader.
    ///
    /// Code adapted from: http://blogs.msdn.com/xmlteam/archive/2007/03/24/streaming-with-linq-to-xml-part-2.aspx.
    /// </summary>
    /// <param name="reader">An XmlReader created using one of the many XmlReader.Create overloads.</param>
    /// <param name="elementName">
    /// When any element is found in the source xml document, if the element's name matches the elementName
    /// parameter, that element is read into an XElement and yielded into the enumeration.
    /// </param>
    public static IEnumerable<XElement> Stream(XmlReader reader, XName elementName) =>
        StreamIterator(Throw.IfNull(reader), Throw.IfNull(elementName));

    private static IEnumerable<XElement> StreamIterator(XmlReader reader, XName elementName)
    {
        reader.MoveToContent();

        bool yielded;

        do
        {
            // We track yielded in order to short-circuit reader.Read() because XElement.ReadFrom will
            // have already advanced the XmlReader to the next Node.
            yielded = false;

            if (reader.NodeType == XmlNodeType.Element
                && XName.Get(reader.LocalName, reader.NamespaceURI) == elementName)
            {
                yield return (XElement)XNode.ReadFrom(reader);

                yielded = true;
            }
        } while (yielded || reader.Read());
    }
}