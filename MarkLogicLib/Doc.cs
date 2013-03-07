using System;
using ServiceStack.Text;
using System.IO;

namespace MarkLogicLib
{
  public class Doc
  {
    public const String JSON   = "json";
    public const String XML    = "xml";
    public const String BINARY = "binary";
    public const String TEXT   = "text";

    String textContent = null; // for text, json
    // TODO binary content
    byte[] fileContent = null;
    // TODO XML content

    public string type { get; set; }

    public bool exists { get; set; }

    public Doc ()
    {
      exists = false;
      type = JSON;
    }

    public Doc(String type) {
      this.type = type;
    }

    public void setJsonContent(String content) {
      this.textContent = content;
    }

    public JsonObject getJsonContent() {
      JsonObject.Parse( getTextContent ());
    }

    public void setFileContent(byte[] content) {
      this.fileContent = content;
    }

    public byte[] getFileContent() {
      return fileContent;
    }

    public void detectType(string extension) {
      // TODO link this to remote server
      // NB those below are default mime - file extension mappings for ML 6
      switch (extension) {
      case ".xml":
      case ".xsd":
      case ".sgml":
      case ".sgm":
      case ".svg":
      case ".xhtml":
      case ".xslt":
      case ".xsl":
        type = XML;
        break;
      case ".txt":
      case ".csv":
      case ".asc":
      case ".css":
      case ".etx":
      case ".htc":
      case ".html":
      case ".htm":
      case ".js":
      case ".m3u8":
      case ".manifest":
      case ".rtf":
      case ".rtx":
      case ".si":
      case ".sl":
      case ".tsv":
      case ".wml":
      case ".wmls":
      case ".xqy":
      case ".xqe":
      case ".xq":
      case ".xquery":
        type = TEXT;
        break;
      case ".json":
        type = JSON;
        break;
      default:
        type = BINARY;
      }
    }

    /*
    public void setContent(Object obj) {
      switch (this.type) {
      case JSON:
        textContent = JsonSerializer.SerializeToString<Response>(obj);
        break;
      case TEXT:
        textContent = obj.ToString();
        break;
      case XML:
        // TODO
        break;
      case BINARY:
        break;
      }
    }*/

    public String getTextContent() {
      return textContent;
    }

    public void toFile(string path) {
      File.WriteAllBytes (path, fileContent);
    }

    public void fromFile(string path) {
      this.setFileContent(File.ReadAllBytes(path));
      // determine type from file extension
      this.detectType(path.Substring(path.LastIndexOf(".")));
    }
  }
}

