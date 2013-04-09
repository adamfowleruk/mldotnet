using System;

namespace MarkLogicLib
{
  /**
   * Represents a MarkLogic REST Server response
   */
  public class Response
  {
    public Response ()
    {
    }

    public Doc doc { get;  set; }
    public bool inError {get; set;}
    public string error { get; set;}
    public Exception exception { get; set;}
    public string statusCode { get; set; }
  }
}

