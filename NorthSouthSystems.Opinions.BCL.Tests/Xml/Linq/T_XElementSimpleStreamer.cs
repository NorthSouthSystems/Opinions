using NorthSouthSystems.Xml.Linq;
using System.Xml;
using System.Xml.Linq;

public class T_XElementSimpleStreamer
{
    [Fact]
    public void Basic()
    {
        XElement[] results;

        results = XElementSimpleStreamer.Stream(Reader(string.Empty), "Child").ToArray();
        results.Length.Should().Be(3);

        results[0].Value.Should().Be("A");
        results[1].Value.Should().Be("B");

        results[2].Elements().ToArray()[0].Value.Should().Be("AA");
        results[2].Elements().ToArray()[1].Value.Should().Be("BB");

        results = XElementSimpleStreamer.Stream(Reader(string.Empty), "Grandchild").ToArray();
        results.Length.Should().Be(2);

        results[0].Value.Should().Be("AA");
        results[1].Value.Should().Be("BB");

        const string ns = "Foobar";

        results = XElementSimpleStreamer.Stream(Reader(ns), XName.Get("Grandchild", ns)).ToArray();
        results.Length.Should().Be(2);

        results[0].Value.Should().Be("AA");
        results[1].Value.Should().Be("BB");
    }

    [Fact]
    public void Exceptions()
    {
        Action act;

        act = () => XElementSimpleStreamer.Stream(null, "Foobar").ToArray();
        act.Should().ThrowExactly<ArgumentNullException>();

        act = () => XElementSimpleStreamer.Stream(Reader(string.Empty), null).ToArray();
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    private static XmlReader Reader(XNamespace ns) =>
        new XElement(
                "Root",
                new XElement(ns + "Child", "A"),
                new XElement(ns + "Child", "B"),
                new XElement(
                    ns + "Child",
                    new XElement(ns + "Grandchild", "AA"),
                    new XElement(ns + "Grandchild", "BB")))
            .CreateReader();
}
