using System;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using System.Collections.Generic;

namespace MarkLogicLib.Objects
{

  [Route("/v1/search", "GET")] // TODO split these in to Search and SearchResponse objects???
  public class Search : IReturn<SearchResponse>
  {
    // request fields
    public string Q { get; set; }
    public string StructuredQuery { get; set; } // TODO how to put this in to the content of the POST version of /v1/search ? 
    public string Options {get;set;}
    public int Start {get;set;}
    public int PageLength {get;set;}
    public string Format { get; set;}
    public string View { get; set;}
    public string Txid { get; set;}
    public string Collection { get; set;}
    public string Directory { get; set;}
  }

  public class SearchResponse : MarkLogicObject {
    
    // response fields
    public string SnippetFormat {get;set;}
    public int Total {get;set;}
    public int Start {get;set;}
    public int PageLength {get;set;}
    public SearchResult[] Results { get; set; }
    public string Warning {get;set;}
    
    public override string ToString ()
    {
      return "SnippetFormat: " + SnippetFormat + ", Total: " + Total + ", Start: " + Start + ", Warning: " + Warning;
    }

    public static SearchResponse ParseJson(string json) {
      var map = JsonObject.Parse(json);

      return new SearchResponse {
        Total = int.Parse (map["total"]),
        Start = int.Parse (map["start"]),
        PageLength = int.Parse (map ["page-length"]),
        SnippetFormat = map ["snippet-format"],
        Warning = map["warning"],
        Results = JsonObject.Parse(json).GetUnescaped("results").FromJson<SearchResult[]>() 
      };
    }
  }

  // Sub elements of SearchResponse

  public class SearchResult
  {
    public string Uri {get;set;}
    public long Index { get; set; }
    public string Path {get;set;}
    public double Score {get;set;}
    public double Fitness {get;set;}
    public double Confidence {get;set;}
    public string Content { get; set; } // JSON or XML content (probably XML, no matter what ?format=json says)

    public override string ToString ()
    {
      return string.Format ("[SearchResult: Uri={0}, Index={1}, Path={2}, Score={3}, Fitness={4}, Confidence={5}, Content={6}]", Uri, Index, Path, Score, Fitness, Confidence, Content);
    }
  }
}

