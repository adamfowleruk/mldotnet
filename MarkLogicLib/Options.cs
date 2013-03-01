using System;
using ServiceStack.ServiceHost;

namespace MarkLogicLib
{
  /**
   * Holds options for the underlying database Connection
   */
  public class Options : IReturn<Options>
  {
    public String host { get; set; }
    public String port { get; set; }
    public String adminport { get; set; }
    public bool ssl { get; set; }
    public String auth { get; set; }
    public String username{ get; set; }
    public String password { get; set; }
    public String database { get; set; } // IGNORED
    public String searchoptions { get; set; }
    public int fastthreads { get; set; }
    public int fastparts { get; set; }

    public Options ()
    {
      host = "localhost";
      port = "9090";
      adminport = "8002";
      ssl = false;
      auth = "digest";
      username = "admin";
      password = "admin";
      database = "mldbtest";
      searchoptions = null;
      fastthreads = 10;
      fastparts = 100;
    }

    public String getConnectionString() {
      String cs = "http";
      if (ssl) {
        cs += "s";
      }
      cs += "://" + host + ":" + port;
      return cs;
    }
  }
}

