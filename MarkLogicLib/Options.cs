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
    public string baseuri {get;set;}

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
      baseuri = "/";
    }

    public String getConnectionString() {
      String cs = "http";
      if (ssl) {
        cs += "s";
      }
      cs += "://" + host + ":" + port;
      return cs;
    }

    public void setConnectionString(String cs) {
      // parse cs for protocol
      if (cs.StartsWith("https://")) {
        ssl = true;
        cs = cs.Substring(8);
      } else {
        ssl = false;
        cs = cs.Substring(7); // http://
      }

      // check for username/password
      int atpos = cs.IndexOf ("@");
      if (-1 != atpos) {
        String userpass = cs.Substring(0,atpos);
        cs = cs.Substring(atpos + 1);

        // parse
        int colonpos = userpass.IndexOf(":");
        username = userpass.Substring(0,colonpos);
        password = userpass.Substring(colonpos + 1);
      }

      // check for host/port
      int pos = cs.IndexOf (":");
      if (-1 == pos) {
        int pos2 = cs.IndexOf ("/");
        if (-1 == pos) {
          host = cs;
          cs = "/";
        } else {
          host = cs.Substring(0,pos2);
          cs = cs.Substring(pos2 + 1);
        }
      } else {
        host = cs.Substring (0, pos);
        cs = cs.Substring (pos + 1);
        int pos2 = cs.IndexOf ("/");
        if (-1 == pos2) {
          port = cs;
          cs = "/";
        } else {
          port = cs.Substring(0,pos2);
          cs = cs.Substring(pos2 + 1);
        }
      }

      // get baseuri
      baseuri = cs;

    }
  }
}

