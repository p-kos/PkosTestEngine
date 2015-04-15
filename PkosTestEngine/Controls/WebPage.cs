using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PkosTestEngine.Controls
{
  public class WebPage : HtmlDocument
  {
    public WebPage( UITestControl searchLimitContainer, string id, string mainTitle, string absolutePath, string relativePageUrl, List<string> otherTitles = null, string redirectingPage = "False", string frameDocument = "False", CacheProvider cacheProvider=null) :
      base( searchLimitContainer )
    {
      #region Search Criteria

      this.SearchProperties[ HtmlDocument.PropertyNames.Id ] = id;
      this.SearchProperties[ HtmlDocument.PropertyNames.RedirectingPage ] = redirectingPage;
      this.SearchProperties[ HtmlDocument.PropertyNames.FrameDocument ] = frameDocument;
      this.FilterProperties[ HtmlDocument.PropertyNames.Title ] = mainTitle;
      this.FilterProperties[ HtmlDocument.PropertyNames.AbsolutePath ] = absolutePath;
      this.FilterProperties[ HtmlDocument.PropertyNames.PageUrl ] = string.IsNullOrEmpty( relativePageUrl ) ? null : relativePageUrl;
      this.WindowTitles.Add( mainTitle );

      #endregion

      Browser = searchLimitContainer as BrowserWindow;
      if ( Browser != null )
      {
        this.Browser.WaitForControlReady();
        //Body = ( Browser.CurrentDocumentWindow.GetChildren()[ 0 ] as UITestControl ) as HtmlControl;
      }
      
      
      Cache = cacheProvider;
      
    }

    private CacheProvider Cache { get; set; }

    #region Controls
    private Dictionary<string, HtmlControl> _controls ;
    private Dictionary<string, HtmlControl> Controls
    {
      get
      {
        if ( _controls == null )
        {
          if ( Cache == null )
          {
            _controls = new Dictionary<string, HtmlControl>();
          }
          else
          {
            _controls = Cache.LoadControls( this );
          }
        }
        return _controls;
      }
    }
    #endregion

    private BrowserWindow Browser { get; set; }

    /// <summary>
    /// Store the root control for the page.
    /// </summary>
    private UITestControl Body
    {
      get
      {
        this.Browser.WaitForControlReady( );
        return ( Browser.CurrentDocumentWindow.GetChildren( )[ 0 ] as UITestControl ) as HtmlControl;
      }
    }

    /// <summary>
    /// Find Control in the current page
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public T FindControlInBody<T>( string selector, bool ignoreIfCached = false, string customName = "" ) where T : HtmlControl, new( )
    {
      if ( string.IsNullOrEmpty( customName ) )
      {
        customName = selector;
      }
      if ( this.Body != null )
      {
        if ( this.Controls.ContainsKey( customName ) )
        {
          if ( !ignoreIfCached && this.Controls[ customName ] is T )
          {
            return this.Controls[ customName ] as T;
          }
          else
          {
            this.Controls.Remove( customName );
          }
        }

        T control = this.Body.FindControl<T>( selector );
        if ( control != null )
        {
          Controls.Add( customName, control );
          if ( Cache != null )
          {
            Cache.SaveControl( this, control, customName );
          }
          return control;
        }
        throw new AssertFailedException( string.Format( "Control with selector {0} could not be find", selector ) );
      }

      throw new AssertFailedException( "Body is null." );
    }
    public ReadOnlyCollection<T> FindControlsInBody<T>( string selector ) where T : HtmlControl, new( )
    {
      return this.Body.FindControls<T>( selector );
    }
  }

 
}

