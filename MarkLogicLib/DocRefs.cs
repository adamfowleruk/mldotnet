using System;
using System.Collections.Generic;
using System.Collections;

namespace MarkLogicLib
{
  public class DocRefs : IEnumerable
  {
    List<string> docuris = new List<string>();

    public DocRefs ()
    {
    }

    public void Add(string uri) {
      docuris.Add (uri);
    }

    public int Count() {
      return docuris.Count;
    }

    public IEnumerator GetEnumerator() {
      return docuris.GetEnumerator ();
    }
  }
}

