using System;
using NUnit.Framework;
using MarkLogicLib;
using ServiceStack.Text;

namespace MarkLogicTests
{
    [TestFixture()]
    public class AddGetDeleteDoc
    {
        [Test()]
        public void TestCase ()
        {
            Connection conn = new Connection ();
            Animal penguin = new Animal();
            penguin.name = "Penguin";
            penguin.summary = "Penguins are cool";

            String docuri = "/documents/1.json";

            Doc doc = new Doc ();
            doc.setJsonContent (JsonSerializer.SerializeToString<Animal> (penguin));
            Response result = conn.save (doc, docuri, null);
            Assert.IsFalse (result.inError);

            // now fetch to ensure it is there
            result = conn.get (docuri);
            Assert.IsFalse (result.inError);
            Doc retdoc = result.doc;
            Assert.IsTrue (retdoc.exists);

            // now delete
            result = conn.delete (docuri);
            Assert.IsFalse (result.inError);

            // now fetch to ensure it is deleted
            result = conn.get (docuri);
            Assert.IsFalse (result.inError);
            Doc deldoc = result.doc;
            Assert.IsFalse (deldoc.exists);
        }
    }
}

