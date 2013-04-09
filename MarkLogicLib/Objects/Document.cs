using System;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using System.Xml;

namespace MarkLogicLib.Objects
{
  
  [Route("/v1/documents", "GET")]
  public class Document : IReturn<DocumentResponse>
  {
    // request fields
    public string Uri { get; set; }
    public string Category { get; set;}
    public string Format { get; set;}
    public string Transform { get; set;}
    public string Txid { get; set;}
  }
  
  public class DocumentResponse : MarkLogicObject 
  {
    // TODO store document content here
    // TODO merge in functionality from base Doc class
  }


  
  [Route("/v1/documents", "PUT")]
  public class PutDocument : IReturn<PutDocumentResponse>
  {
    public string Uri { get; set;}
    public string Category { get; set;}
    public string Format { get; set;}
    public string Lang { get; set;}
    public string Collection { get; set;}
    public string Quality { get; set;}
    public string Extract { get; set;}
    public string Repair { get; set;}
    public string Transform { get; set;}
    public string Forest_Name { get; set;}
    public string Txid { get; set;}
  }

  public class PutDocumentResponse : MarkLogicObject
  {
  }


  
  [Route("/v1/documents", "DELETE")]
  public class DeleteDocument : IReturn<DeleteDocumentResponse>
  {
    public string Uri { get; set;}
    public string Category { get; set;}
    public string Txid { get; set;}
  }

  public class DeleteDocumentResponse : MarkLogicObject
  {
  }

  public class Metadata : MarkLogicObject {
    public Properties properties {get;set;}
  }


  /**
   * Document properties fragment (override to support custom properties. Remember to overrid ParseJson too).
   */
  public class Properties : MarkLogicObject {
    public DateTime LastModified {get;set;}
    
    public static Properties ParseJson(string json) {
      var map = JsonObject.Parse(json);

      string formatString= "yyyy-MM-ddTHH:mm:ssZ";
      //System.Globalization.CultureInfo cInfo= new System.Globalization.CultureInfo("en-GB", true);
      DateTime lm= System.DateTime.ParseExact(map ["last-modified"], formatString, null);

      return new Properties {
        LastModified = lm
      };
    }

    public override string ToString ()
    {
      return string.Format ("[Properties: LastModified={0}]", LastModified);
    }
  }

}

