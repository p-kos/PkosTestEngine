using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using PkosTestEngine.Controls;
using System.Diagnostics;

namespace PkosTestEngine
{
  public class CacheProvider
  {
    private const string XMLROOTNAME = "webapp";
    private const string XMLPAGENODENAME = "page";
    private const string CONTROLKEYNAME = "selector";

    private static CacheProvider _instance;

    private CacheProvider( string cachePath )
    {
      this.CachePath = cachePath;
    }

    public string CachePath { get; set; }

    private Dictionary<string, WebPage> LoadPages( XmlNodeList controlsNode, BrowserWindow browserWindow )
    {
      Dictionary<string, WebPage> results = new Dictionary<string, WebPage>();
      foreach ( XmlElement pageNode in controlsNode )
      {
        string id = null;
        string mainTitle = null;
        string absolutePath = null;
        string relativePageUrl = null;
        string redirectingPage = "False";
        string frameDocument = "False";
        List<string> otherTitles = null;
        bool withMenu = false;
        foreach ( XmlAttribute attr in pageNode.Attributes )
        {
          switch ( attr.Name )
          {
            case "id":
              id = attr.Value;
              break;
            case "title":
              mainTitle = attr.Value;
              break;
            case "absolutePath":
              absolutePath = attr.Value;
              break;
            case "pageUrl":
              relativePageUrl = new Uri( attr.Value ).AbsoluteUri;
              break;
            case "redirectingPage":
              redirectingPage = attr.Value;
              break;
            case "frameDocument":
              frameDocument = attr.Value;
              break;
            case "withMenu":
              bool res = Boolean.TryParse( attr.Value, out withMenu );
              withMenu = !res ? res : withMenu;
              break;
          }
        }
        if ( pageNode.GetElementsByTagName( "titles" ).Count > 0 )
        {
          otherTitles = new List<string>();
          XmlElement titlesRootNode = pageNode.GetElementsByTagName( "titles" )[ 0 ] as XmlElement;

          foreach ( XmlElement titleNode in titlesRootNode.GetElementsByTagName( "add" ) )
          {
            otherTitles.Add( titlesRootNode.Value );
          }
        }
        WebPage webPage = new WebPage( browserWindow, id, mainTitle, absolutePath, relativePageUrl, otherTitles, redirectingPage, frameDocument, this );
        results.Add( absolutePath, webPage );
      }
      return results;
    }

    /// <summary>
    /// Return a single intance of CacheProvider
    /// </summary>
    /// <param name="cachePath"></param>
    /// <returns></returns>
    public static CacheProvider GetCacheProvider( string cachePath )
    {
      if ( _instance == null )
      {
        _instance = new CacheProvider( cachePath );
      }
      return _instance;
    }

    /// <summary>
    /// Save page into cache
    /// </summary>
    /// <param name="page">pages</param>
    public void SavePage( WebPage page )
    {
      if ( !string.IsNullOrEmpty( CachePath ) )
      {

        XmlDocument xDoc = new XmlDocument();
        try
        {
          xDoc.Load( CachePath );
        }
        catch
        {
        }
        XmlNodeList nodeList = xDoc.GetElementsByTagName( XMLROOTNAME );

        if ( nodeList.Count == 0 )
        {
          XmlElement node = xDoc.CreateElement( XMLROOTNAME );
          xDoc.AppendChild( node );
        }

        XmlNodeList webPages = ( nodeList[ 0 ] as XmlElement ).GetElementsByTagName( XMLPAGENODENAME );
        if ( webPages.Count == 0 )
        {
          XmlElement pageNode = xDoc.CreateElement( XMLPAGENODENAME );
          pageNode.SetAttribute( "redirectingPage", page.RedirectingPage.ToString() );
          pageNode.SetAttribute( "frameDocument", page.FrameDocument.ToString() );
          pageNode.SetAttribute( "title", page.Title );
          pageNode.SetAttribute( "absolutePath", page.AbsolutePath );
          pageNode.SetAttribute( "pageUrl", page.PageUrl );

          ( nodeList[ 0 ] as XmlElement ).AppendChild( pageNode );
        }
        else
        {
          bool exists = false;
          foreach ( XmlElement pageNode in webPages )
          {
            if ( pageNode.GetAttribute( "absolutePath" ) == page.AbsolutePath )
            {
              pageNode.SetAttribute( "redirectingPage", page.RedirectingPage.ToString() );
              pageNode.SetAttribute( "frameDocument", page.FrameDocument.ToString() );
              pageNode.SetAttribute( "title", page.Title );
              pageNode.SetAttribute( "pageUrl", page.PageUrl );
              exists = true;
              break;
            }
          }
          if ( !exists )
          {
            XmlElement pageNode = xDoc.CreateElement( XMLPAGENODENAME );
            pageNode.SetAttribute( "redirectingPage", page.RedirectingPage.ToString() );
            pageNode.SetAttribute( "frameDocument", page.FrameDocument.ToString() );
            pageNode.SetAttribute( "title", page.Title );
            pageNode.SetAttribute( "absolutePath", page.AbsolutePath );
            pageNode.SetAttribute( "pageUrl", page.PageUrl );
            ( nodeList[ 0 ] as XmlElement ).AppendChild( pageNode );
          }
        }

        xDoc.Save( CachePath );
      }

    }

    /// <summary>
    /// Load web pages from Cache
    /// </summary>
    /// <param name="browserWindow">Parent Browser Windows</param>
    /// <returns>Dictionary of WebPage where absolutePath is the key</returns>
    public Dictionary<string, WebPage> LoadWebPageCollectionFromCache( BrowserWindow browserWindow )
    {
      if ( CachePath == null )
      {
        return new Dictionary<string, WebPage>();
      }
      XmlDocument xDoc = new XmlDocument();
      try
      {
        xDoc.Load( CachePath );

      }
      catch
      {
        return new Dictionary<string, WebPage>();
      }
      XmlNodeList controlsNode = xDoc.GetElementsByTagName( XMLPAGENODENAME );
      return LoadPages( controlsNode, browserWindow );
    }

    public Dictionary<string, HtmlControl> LoadControls( WebPage page )
    {
      Dictionary<string, HtmlControl> controls = new Dictionary<string, HtmlControl>();
      XmlElement pageNode = null;
      XmlDocument xDoc = new XmlDocument();
      try
      {
        xDoc.Load( CachePath );
      }
      catch
      {
      }
      XmlNodeList nodeList = xDoc.GetElementsByTagName( XMLROOTNAME );

      if ( nodeList.Count == 0 )
      {
        Trace.WriteLine( "There is no data set yet in XML" );
        return controls;
      }

      int pagesIndex = 0;
      XmlNodeList pageNodes = ( nodeList[ pagesIndex ] as XmlElement ).GetElementsByTagName( XMLPAGENODENAME );


      foreach ( XmlElement currentpageNode in pageNodes )
      {
        if ( currentpageNode.GetAttribute( "absolutePath" ) == page.AbsolutePath )
        {
          pageNode = currentpageNode;
          break;
        }
      }
      if ( pageNode == null )
      {
        Trace.WriteLine( "Page not found in Cache" );
        return controls;
      }

      //foreach ( XmlElement controlNode in pageNode.GetElementsByTagName( XMLCONTROLNODENAME ) )
      foreach ( XmlElement controlNode in pageNode.ChildNodes )
      {
        PropertyExpressionCollection searchProperties = new PropertyExpressionCollection();
        string key = null;

        foreach ( XmlAttribute attr in controlNode.Attributes )
        {
          if ( attr.Name == CONTROLKEYNAME )
          {
            key = attr.Value;
          }
          else
          {
            searchProperties.Add( attr.Name, attr.Value );
          }
        }

        HtmlControl control = null;

        //switch ( controlNode.GetAttribute( "controlType" ) )
        switch ( controlNode.Name )
        {
          case "HtmlEdit":
            control = new HtmlEdit() { Container = page };
            break;
          case "HtmlInputButton":
            control = new HtmlInputButton() { Container = page };
            break;
          case "HtmlDiv":
            control = new HtmlDiv() { Container = page };
            break;
          case "HtmlTable":
            control = new HtmlTable() { Container = page };
            break;
          case "HtmlSpan":
            control = new HtmlSpan() { Container = page };
            break;
          case "HtmlComboBox":
            control = new HtmlComboBox() { Container = page };
            break;
          case "HtmlCell":
            control = new HtmlCell() { Container = page };
            break;
        }
        if ( control != null )
        {
          control.SearchProperties.AddRange( searchProperties );

          if ( string.IsNullOrEmpty( key ) )
          {
            key = string.IsNullOrEmpty( control.Id ) ? "." + control.Class : "#" + control.Id;
          }
          controls.Add( key, control );
        }

      }

      return controls;
    }

    /// <summary>
    /// Save Control into cache
    /// </summary>
    /// <typeparam name="T">HtmlControl type</typeparam>
    /// <param name="page">Current Page</param>
    /// <param name="control">Control to Save</param>
    /// <param name="key">selector to find the control</param>
    public void SaveControl<T>( WebPage page, T control, string key="" ) where T : HtmlControl, new()
    {
      XmlDocument xDoc = new XmlDocument();
      try
      {
        xDoc.Load( CachePath );
      }
      catch
      {
      }
      XmlNodeList nodeList = xDoc.GetElementsByTagName( XMLROOTNAME );
      int pagesIndex = 0;
      XmlNodeList pageNodes = ( nodeList[ pagesIndex ] as XmlElement ).GetElementsByTagName( XMLPAGENODENAME );

      foreach ( XmlElement pageNode in pageNodes )
      {
        if ( pageNode.GetAttribute( "absolutePath" ) == page.AbsolutePath )
        {
          bool found = false;
          XmlNodeList controlNodeList = pageNode.ChildNodes; 
          foreach ( XmlElement controlNode in controlNodeList )
          {
            if ( controlNode.Name == typeof( T ).Name && key == controlNode.GetAttribute( CONTROLKEYNAME ) )
            {
              found = true;
              break;
            }
          }

          #region If control does not exists

          if ( !found )
          {
            XmlElement newControl = xDoc.CreateElement( typeof(T).Name );
            if ( !string.IsNullOrEmpty( key ) )
            {
              newControl.SetAttribute( CONTROLKEYNAME, key );
            }
            if ( !string.IsNullOrEmpty( control.Id ) )
            {
              newControl.SetAttribute( HtmlControl.PropertyNames.Id, control.Id );
            }
            if ( !string.IsNullOrEmpty( control.Name ) )
            {
              newControl.SetAttribute( HtmlControl.PropertyNames.Name, control.Name );
            }
            if ( !string.IsNullOrEmpty( control.Class ) )
            {
              newControl.SetAttribute( HtmlControl.PropertyNames.Class, control.Class );
            }
            //if ( !string.IsNullOrEmpty( control.ControlDefinition ) )
            //{
            //  newControl.SetAttribute( HtmlControl.PropertyNames.ControlDefinition, control.ControlDefinition );
            //}
            foreach ( PropertyExpression propertyExpression in control.SearchProperties )
            {
              if ( 
                propertyExpression.PropertyName != HtmlControl.PropertyNames.Id
                && propertyExpression.PropertyName != HtmlControl.PropertyNames.Name
                && propertyExpression.PropertyName != HtmlControl.PropertyNames.ControlDefinition
                )
              {
                newControl.SetAttribute( propertyExpression.PropertyName, propertyExpression.PropertyValue );
              }
            }
            pageNode.AppendChild( newControl );
          }

          #endregion

          break;
        }
      }
      xDoc.Save( CachePath );
    }
  }
}