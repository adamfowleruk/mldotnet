using System;
using System.Collections;

namespace MarkLogicLib
{
  public class DocRefs
  {
    public DocRefs ()
    {
      docuris = "";
    }

    public string docuris {get;set;}

    public ArrayList toArrayList() {
      ArrayList results = new ArrayList ();

      string[] splits = docuris.Split (',');
      foreach (string split in splits) {
        results.Add (split);
      }

      return results;
    }
  }
}

