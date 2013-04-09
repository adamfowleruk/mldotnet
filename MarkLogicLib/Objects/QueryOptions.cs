using System;
using ServiceStack.ServiceHost;

namespace MarkLogicLib.Objects
{
  
  [Route("/v1/config/query/{Name}", "GET")]
  [Route("/v1/config/query/{Name}", "POST")]
  [Route("/v1/config/query/{Name}", "PUT")]
  [Route("/v1/config/query/{Name}", "DELETE")]
  public class QueryOptions : IReturn<QueryOptionsResponse>
  {
    // request fields
    public string Name { get; set; }


  }
  
  public class QueryOptionsResponse : MarkLogicObject 
  {
    public string Options {get;set;}
  }
}

