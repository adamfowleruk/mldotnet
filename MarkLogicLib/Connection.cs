using System;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.Service;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceInterface;
using System.Collections;

namespace MarkLogicLib {
  public class Connection {

    public Options options { get; set; }

    private IRestClient restClient = null; // see how this supports Digest, Basic, and HTTPS -> Automatically once user/pass configured (see configure)

    private String txid = null;


    // static searches for convenience
    
    static String queryNameUrisUpdatedSince = "urismodifiedsince";
    /*
    static Object[] queryUrisUpdatedSince = {
      Object[] options = new Object[] {
        "return-results" = true,
        "page-length" = 200,
        "transform-results" = new Object {
          apply = "raw"
        },
        constraint = new Object { // TODO put inside properties fragment constraint
          name = "lastmodified",
          range = new Object {
            type = "xs:dateTime",
            element = {
              ns = "http://marklogic.com/xdmp/property",
              name = "last-modified"
            }
          }
        }
      }
    };*/

    
    static string queryUrisUpdatedSince = "{options : {" +
      "\"return-results\" : true," +
      "\"page-length\" : 200, " + 
      "\"transform-results\" : {\"apply\" : \"raw\"}," +
      "\"constraint\" : { " +
        "\"name\" : \"lastmodified\"," + 
        "\"range\" : {" +
          "\"type\" : \"xs:dateTime\"," +
            "\"element\" : {" +
              "\"ns\" : \"http://marklogic.com/xdmp/property\"," +
              "\"name\" : \"last-modified\"" +
            "}" +
          "}" +
        "}" +
        // TODO add URI base constraint
      "}" +
    "}"; // TODO research obj to json for nested objects in servicestack/c#



	  public Connection () {
      options = new Options();
      configure (options);
	  }

    private String completePath(String path,Hashtable parameters) 
    {
      String nextChar = "?";
      if (path.Contains ("?")) {
        nextChar = "&";
      }
      String newPath = (String)path.Clone ();
      foreach (String key in parameters.Keys) {
        newPath += nextChar + key + "=" + (String)parameters[key]; // TODO param encode for uri
        nextChar = "&";
      }
      return newPath;
    }


    public Response doGet(String path,Hashtable parameters) {
      try {
        Response resp = restClient.Get<Response>(completePath(path,parameters));
        resp.inError = false;
        return resp;
      } catch (Exception e) {
        Response resp = new Response();
        resp.inError = true;
        resp.exception = e;
        return resp;
      }
    }

    public Response doPut(String path,Hashtable parameters,Doc doc) {
      try {
        Response resp =  restClient.Put<Response> (completePath (path, parameters), doc.getTextContent()); // TODO serialise document to restClient URL as string content format json
      resp.inError = false;
      return resp;
    } catch (Exception e) {
      Response resp = new Response();
      resp.inError = true;
      resp.exception = e;
      return resp;
    }
    }

    public Response doPost(String path,Hashtable parameters,Doc doc) {
      try {
        Response resp =  restClient.Post<Response> (completePath (path, parameters), doc.getTextContent()); // TODO serialise document to restClient URL as string content format json
    resp.inError = false;
    return resp;
  } catch (Exception e) {
    Response resp = new Response();
    resp.inError = true;
    resp.exception = e;
    return resp;
  }
    }

    public Response doDelete(String path,Hashtable parameters) {
      try {
        Response resp =  restClient.Delete<Response> (completePath (path, parameters));
  resp.inError = false;
  return resp;
} catch (Exception e) {
  Response resp = new Response();
  resp.inError = true;
  resp.exception = e;
  return resp;
}
    }
    
    /**
     * Function allowing MLDB's underlying REST invocation mechanism to be used for an arbitrary request. 
     * Useful for future proofing should some new functionality come out, or bug discovered that prevents
     * your use of a JavaScript Driver API call.
     * options = {method: "GET|POST|PUT|DELETE", path: "/v1/somepath?key=value&format=json"}
     * content = undefined for GET, DELETE, json for PUT, whatever as required for POST
     */
    public Response doRequest(String path,Hashtable parameters,Doc doc,String method) {
      switch (method) {
      case "GET":
        return doGet (path, parameters);
      case "POST":
        return doPost (path, parameters, doc);
      case "PUT":
        return doPut (path, parameters, doc);
      case "DELETE":
        return doDelete (path, parameters);
      default:
        return null;
      }
    }



		// TODO ENSURE ALL METHODS FROM MLDB ARE SUPPORTED

    // DRIVER CONFIGURATION
    /**
     * Provide configuration information to this database. This is merged with the defaults.
     */
	  public void configure(Options options) {
      this.options = options;

      // NB This automatically uses Basic and Digest authentication where required
      this.restClient = new JsonServiceClient(this.options.getConnectionString()) {
        UserName = this.options.username,
        Password = this.options.password,
        AlwaysSendBasicAuthHeader = true // TODO check this is always OK -> NB Must set REST server to Basic, not Digest (ServiceStack doesn't work with this for some reason)
      };

	  }

	  public void setLogger() {
	  }


    // DATABASE MANAGEMENT
    /**
     * Does this database exist? Returns an object, not boolean, to the callback
     */
	  public bool exists() {
	    // TODO perform check in exists()
	    return false;
	  }

    /**
     * Creates the database and rest server if it does not already exist
     */
		public void create() {
		}
    
    /**
     * Destroys the database and rest api instance
     */
		public void destroy() {
		}

		// DOCUMENT MANAGEMENT
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Fetches a document with the given URI.
     * 
     * https://docs.marklogic.com/REST/GET/v1/documents
     */
    public Response get(String uri) {
      Hashtable qp = new Hashtable ();
      qp.Add ("uri", uri);
      return doGet("/v1/documents", qp);
		}
    
    /**
     * Fetches the metadata for a document with the given URI. Metadata document returned in result.doc
     * 
     * https://docs.marklogic.com/REST/GET/v1/documents
     */
    public Response metadata() {
      String path = @"/v1/documents";
      Hashtable qp = new Hashtable ();
      qp.Add ("category", "metadata");
      Response response = doGet (path, qp);
      return response;
    }

    
    public Response save(Doc doc,String docuri) {
      return save (doc, docuri, null);
    }
		
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Saves new docs with GUID-timestamp, new docs with specified id, or updates doc with specified id
     * NB handle json being an array of multiple docs rather than a single json doc
     * If no docuri is specified, one is generated by using a combination of the time and a large random number.
     *
     * https://docs.marklogic.com/REST/PUT/v1/documents
     */
    public Response save(Doc doc,String docuri,Doc properties) {
      String path = @"/v1/documents";
      Hashtable qp = new Hashtable ();
      qp.Add ("uri", docuri);
      return doPut (path, qp,doc);
	  }
    
    /**
     * Updates the document with the specified uri by only modifying the passed in properties.
     * NB May not be possible in V6 REST API elegantly - may need to do a full fetch, update, save
     */
		public void merge() {
		}
		
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Deletes the specified document
     * 
     * https://docs.marklogic.com/REST/DELETE/v1/documents
     */ 
    public Response delete(String docuri) {
      String path = @"/v1/documents";
      Hashtable qp = new Hashtable ();
      qp.Add ("uri", docuri);
      return doDelete (path, qp);
		}

    // SEARCH FUNCTIONS
    /**
     * Returns all documents in a collection, optionally matching against the specified fields
     * http://docs.marklogic.com/REST/GET/v1/search
     */
		public DocList collect() {
      return null; // TODO change from null
		}
    
    /**
     * Lists all documents in a directory, to the specified depth (default: 1), optionally matching the specified fields
     * http://docs.marklogic.com/REST/GET/v1/search
     */
    public DocList list() {
      return null; // TODO change from null
		}
    
    /**
     * Performs a simple key-value search. Of most use to JSON programmers.
     * 
     * https://docs.marklogic.com/REST/GET/v1/keyvalue
     */
    public void keyvalue() {
      return ; // TODO change from void
		}
    
    /**
     * Performs a search:search via REST
     * http://docs.marklogic.com/REST/GET/v1/search
     *
     * See supported search grammar http://docs.marklogic.com/guide/search-dev/search-api#id_41745 
     */ 
    public void search() {
      return ; // TODO change from void
		}
    
    /**
     * Performs a search:search via REST
     * http://docs.marklogic.com/REST/GET/v1/search
     *
     * See supported search grammar http://docs.marklogic.com/guide/search-dev/search-api#id_41745 
     */ 
    public void searchCollection() {
      return ; // TODO change from void
		}
    
    /**
     * Performs a structured search.
     * http://docs.marklogic.com/REST/GET/v1/search
     * 
     * Uses structured search instead of cts:query style searches. See http://docs.marklogic.com/guide/search-dev/search-api#id_53458
     */
    public Response structuredSearch(string options_opt,string query_opt) {
      String path = @"/v1/search";
      Hashtable qp = new Hashtable ();
      qp.Add ("format", "json");
      if (null != query_opt) {
        qp.Add ("structuredQuery", query_opt);
      }

      if (options_opt != null) {
        qp.Add ("options", options_opt);
      }
      
      // make transaction aware
      if (null != this.txid) {
        qp.Add ("txid", this.txid);
      }

      Response response = doGet (path, qp);

      return response; 
		}

    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Saves search options with the given name. These are referred to by mldb.structuredSearch.
     * http://docs.marklogic.com/REST/PUT/v1/config/query/*
     *
     * For structured serch options see http://docs.marklogic.com/guide/rest-dev/search#id_48838
     */
    public bool saveSearchOptions(String name,String options) {
      String path = @"/v1/config/query/" + name;
      Hashtable qp = new Hashtable ();
      qp.Add ("format", "json");

      Doc doc = new Doc ();
      doc.setJsonContent (options);

      Response response = doPut(path,qp,doc); 
      return !response.inError; // TODO log any errors and pass to caller as exception
		}
    
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Ensures a search has been saved to the server. If it hasn't, it saves the search options to the server.
     * Returns true if it exists on the server, or has been successfully created on the server by this call.
     */
    public bool ensureSearchSaved(String name,String options) {
      // get search options from DB
      
      String path = @"/v1/config/query/" + name;
      Hashtable qp = new Hashtable ();
      qp.Add ("format", "json");

      Response getResult = doGet (path,qp); // TODO verify the URI name of the persisted search options
      if (getResult.inError) {
        // if error (does not exist) then persist search options
        return saveSearchOptions (name,options);
      } else {
        return true;
      }
    }

		public DocRefs listURIs(String uri) {
      // TODO listURIs
      return null; // TODO change from null
    }

    public DocRefs listURIsSinceVersion(String uribase,String mvccVersion) {
      // TODO listURIsSinceVersion
      return null; // TODO change from null
    }
    
    // NEEDED FOR FILE SYNC PROJECT - Note modified since is the server's last-modified date string
    public DocRefs listURIsModifiedSince(String uribase,String modifiedSince) {
      ArrayList list = new ArrayList ();

      bool optionsResult = ensureSearchSaved (queryNameUrisUpdatedSince, queryUrisUpdatedSince);
      if (!optionsResult) {
        return null;
      }

      // TODO sanity check on W3C modified since date format

      string search = "{query: { \"and-query\": {" +
        // TODO uri base constraint
        "\"range-constraint-query\": {" +
          "\"constraint-name\": \"last-modified\"," +
          "\"value\": [\"" + modifiedSince + "\"]" +
        "}" +
       "}}";

      Response searchResult = structuredSearch (queryNameUrisUpdatedSince, search);
      if (searchResult.inError) {
        return null;
      }

      Doc jsonResults = searchResult.doc;
      Object jsonObject = jsonResults.toJsonObject ();

      // TODO loop until we load all the URIs in each page of search results

      // parse response for URI list (multiple lines in http response text content?
      return null;
    }

    public Response beginTransaction() {
      return beginTransaction ("client-txn");
    }

		// TRANSACTIONS
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Opens a new transaction. Optionally, specify your own name.
     * http://docs.marklogic.com/REST/POST/v1/transactions
     */
    public Response beginTransaction(String txname) {
      String path = @"/v1/transactions";
      Hashtable qp = new Hashtable ();
      qp.Add ("category", "metadata");
      if (null != txname) {
        qp["name"] = txname;
      } else {
        qp["name"] = "client-txn";
      }
      this.txid = (String)qp["name"];
      return doPost (path, qp,null);
      // TODO add error check and throw
		}
		
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Commits the open transaction
     * http://docs.marklogic.com/REST/POST/v1/transactions/*
     */
    public Response commitTransaction() {
      String path = @"/v1/transactions/" + this.txid;
      Hashtable qp = new Hashtable ();
      qp.Add ("result", "commit");
      return doPost (path, qp,null);
		}
		
    // NEEDED FOR FILE SYNC PROJECT
    /**
     * Rolls back the open transaction.
     * http://docs.marklogic.com/REST/POST/v1/transactions/*
     */
    public Response rollbackTransaction() {
      String path = @"/v1/transactions/" + this.txid;
      Hashtable qp = new Hashtable ();
      qp.Add ("result", "rollback");
      return doPost (path, qp,null);
		}

		// REST API EXTENSIONS

    // UTILITY FUNCTIONS
    
    // POTENTIALLY NEEDED FOR FILE SYNC PROJECT (for saving all docs in a folder (uri) )
    /**
     * Inserts many JSON documents. FAST aware, TRANSACTION aware.
     */
    public void saveAll() {
      return ; // TODO change from void
    }

  }
}

