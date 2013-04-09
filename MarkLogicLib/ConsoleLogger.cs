using System;

namespace MarkLogicLib
{
  public class ConsoleLogger : ILogger
  {
    public ConsoleLogger ()
    {
    }

    public void log(string msg) {
      Console.WriteLine(msg);
    }
  }
}

