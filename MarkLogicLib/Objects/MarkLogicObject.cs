using System;
using ServiceStack.ServiceInterface.ServiceModel;

namespace MarkLogicLib.Objects
{
  public abstract class MarkLogicObject
  {
    /*
      webEx.StatusCode  = 400
      webEx.ErrorCode   = ArgumentNullException
      webEx.Message     = Value cannot be null. Parameter name: Name
      webEx.StackTrace  = (your Server Exception StackTrace - if DebugMode is enabled)
      webEx.ResponseDto = (your populated Response DTO)
      webEx.ResponseStatus   = (your populated Response Status DTO)
      webEx.GetFieldErrors() = (individual errors for each field if any)
    */
    public ResponseStatus ResponseStatus { get; set; }
  }
}

