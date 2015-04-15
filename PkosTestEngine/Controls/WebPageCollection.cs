using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;

namespace PkosTestEngine.Controls
{
  public class WebPageCollection
  {

    private static WebPageCollection _instance;
    private WebPageCollection(BrowserWindow broswerWindow, CacheProvider cacheProvider = null)
    {
      
      Browser = broswerWindow;
      Cache = cacheProvider;
      WebPages = new Dictionary<string, WebPage>( );
      if ( Cache != null )
      {
        WebPages = Cache.LoadWebPageCollectionFromCache( this.Browser );
      }
    }
    
    private Dictionary<string,WebPage> WebPages { get; set; }

    private BrowserWindow Browser { get; set; }

    private CacheProvider Cache { get; set; }

    #region CurrentWebPage
    private WebPage _currentWebpage = null;
    public WebPage CurrentWebPage 
    {
      get
      {
        _currentWebpage = null;
        Uri uri = Browser.Uri;
        //string key = uri.Segments[ uri.Segments.Length - 1 ].Replace( ".", "" );
        string key = uri.AbsolutePath;
        string windowsIETitle = "- Windows Internet Explorer";
        string id = null;
        string mainTitle = Browser.Title.Replace( windowsIETitle, "" ).Trim( );
        if ( WebPages.ContainsKey( key ) )
        {
          if ( WebPages[ key ].PageUrl != uri.AbsoluteUri )
          {
            _currentWebpage = new WebPage( Browser, id, mainTitle, uri.AbsolutePath, uri.AbsoluteUri, null, "False", "False", Cache );
            WebPages[ key ] = _currentWebpage;
            // Save to Cache
            if ( Cache != null )
            {
              Cache.SavePage( _currentWebpage );
            }
          }
          return WebPages[ key ];
        }
        if ( _currentWebpage == null )
        {

          _currentWebpage = new WebPage( Browser, id, mainTitle, uri.AbsolutePath, uri.AbsoluteUri, null, "False", "False", Cache );
          WebPages.Add( key, _currentWebpage );

          // Save to Cache
          if ( Cache != null )
          {
            Cache.SavePage( _currentWebpage );
          }

        }
        return _currentWebpage;
      }
    }
    #endregion

    /// <summary>
    /// Return a Instance of WebPageCollection
    /// </summary>
    /// <param name="browserWindow"></param>
    /// <returns></returns>
    public static WebPageCollection GetWebPageCollection( BrowserWindow browserWindow, CacheProvider cacheProvider = null )
    {
      if ( _instance == null )
      {
        _instance = new WebPageCollection( browserWindow, cacheProvider );
      }
      return _instance;
    }
  }
}
