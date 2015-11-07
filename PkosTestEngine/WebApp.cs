using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using PkosTestEngine.Controls;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Linq;

namespace PkosTestEngine
{
  /// <summary>
  /// Summary description for WebApp
  /// </summary>
  [CodedUITest]
  public class WebApp
  {
    public WebApp( )
    {
    }


    //private WebPageCollection _webPageCollection { get; set; }
    private Boolean _launched = false;

    [DllImport( "user32.dll", CharSet = CharSet.Auto )]
    private static extern IntPtr FindWindow( string strClassName, string strWindowName );

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    private static extern bool SetForegroundWindow( IntPtr hWnd );

    #region Attributes

    #region InitialUrl
    public string InitialUrl { get; set; }
    #endregion
    
    #region WebPageCollection
    private WebPageCollection _webPageCollection;
    private WebPageCollection WebPageCollection
    {
      get
      {
        if ( _webPageCollection == null )
        {
          _webPageCollection =  WebPageCollection.GetWebPageCollection( this.Browser, Cache );
        }
        return _webPageCollection;
      }
    }
    #endregion

    #region Browser

    private BrowserWindow _browser;

    /// <summary>
    /// Internet Explorer Browser
    /// </summary>
    public BrowserWindow Browser
    {
      get
      {
        if ( _browser == null )
        {

          Browser = new BrowserWindow( );

        }
        return _browser;
      }
      private set { _browser = value; }
    }

    #endregion

    //#region MainBody
    //private HtmlControl MainBody
    //{
    //  get { return this.WebPageCollection.CurrentWebPage.Body as HtmlControl; }
    //}
    //#endregion

    #region CurrentPage
    protected WebPage CurrentPage
    {
      get { return this.WebPageCollection.CurrentWebPage; }
    }
    #endregion

    #region CachePath
    /// <summary>
    /// Path of the XML file where the control are cached, if null no control nor page will be cached.
    /// </summary>
    public string CachePath
    {
      get { return ConfigurationManager.AppSettings[ "CachePath" ]; }
    }
    #endregion

    #region Cache
    private CacheProvider Cache
    {
      get
      {
        if ( CachePath != null )
        {
          return CacheProvider.GetCacheProvider( CachePath );
        }
        return null;
      }
    }
    #endregion

    #endregion

    #region private Methods

    #endregion

    #region Public Methods

    /// <summary>
    /// Lauch web application
    /// </summary>
    /// <param name="url">Initial url</param>
    /// <param name="startNewBrowser">Init a new browser instead of find the current</param>
    public void LaunchApp( string url, bool startNewBrowser = false  )
    {
        if ( string.IsNullOrEmpty( url ) )
        {
            throw new AssertFailedException( "url must be set." );
        }
        InitialUrl = url;

        if ( startNewBrowser )
        {
            Process currentProcess = Process.Start( "IExplore.exe", InitialUrl );
            SetForegroundWindow( currentProcess.MainWindowHandle );
            _browser = BrowserWindow.FromProcess( currentProcess );
            _launched = true;
            return;
        }

        var processes = Process.GetProcessesByName( "iexplore" );
        var redirectUrl = new Uri( InitialUrl );
        try
        {
            foreach ( Process currentProcess in processes )
            {
                if ( !string.IsNullOrEmpty( currentProcess.MainWindowTitle ) )
                {
                    try
                    {
                        _browser = BrowserWindow.FromProcess( currentProcess );

                        if ( _browser.Uri.Authority.Replace( "www.", "" ) == redirectUrl.Authority.Replace( "www.", "" ) ) // if there are more than one app opened.
                        {

                            if ( _browser.Uri.AbsoluteUri != InitialUrl )
                            {
                                Browser.NavigateToUrl( redirectUrl );
                            }
                            SetForegroundWindow( currentProcess.MainWindowHandle );
                            _launched = true;
                            return;
                        }

                    }
                    catch
                    {
                        Browser = BrowserWindow.Locate( currentProcess.MainWindowTitle );
                    }
                }
            }
        }
        catch ( Exception e )
        {
            throw new AssertFailedException( e.Message );
        }
        throw new AssertFailedException( "There is no Internet Explorer Opened" );

    }

    /// <summary>
    /// Find a Control in the Current Webpage
    /// </summary>
    /// <typeparam name="T">Html control</typeparam>
    /// <param name="selector">CSS like selector, e.g. #btn1   .className1 </param>
    /// <param name="customName">Custom name to give to control, if null or empty "selector" will be the name</param>
    /// <returns>HtmlControl</returns>
    public T FindControl<T>( string selector, bool ignoreIfCached=false, string customName="" ) where T : HtmlControl, new( )
    {
      if ( _launched )
      {
        var control = this.WebPageCollection.CurrentWebPage.FindControlInBody<T>( selector, ignoreIfCached, customName );
        if ( control != null )
        {
          return control;
        }
        throw new AssertFailedException( string.Format( "Control with select {0} cannot be find", selector ) );
      }
      throw new AssertFailedException("WebApp needs to be launched first");
    }

    /// <summary>
    /// Return a collection of control, these are not cached.
    /// </summary>
    /// <typeparam name="T">HtmlControl</typeparam>
    /// <param name="selector">control selector</param>
    /// <returns></returns>
    public ReadOnlyCollection<T> FindControls<T>( string selector ) where T : HtmlControl, new()
    {
      if ( _launched )
      {
        return this.WebPageCollection.CurrentWebPage.FindControls<T>( selector );
      }
      throw new AssertFailedException( "WebApp needs to be launched first" );
    }

    /// <summary>
    /// Takes screenshot and save to file
    /// </summary>
    /// <param name="fileName">File name</param>
    public void TakeScreenshot( string fileName )
    {
      string extension = Path.GetExtension( fileName );
      ImageFormat format = ImageFormat.Jpeg;
      switch ( extension )
      {
        case ".png":
          format = ImageFormat.Png;
          break;

        case ".bmp":
          format = ImageFormat.Bmp;
          break;

        default:
          format = ImageFormat.Jpeg;
          break;
      }

      Image image = this.Browser.CaptureImage();
      image.Save( fileName, format );
    }

    /// <summary>
    /// Close the application
    /// </summary>
    public void CloseApp( )
    {
        if ( _launched )
        {
            Browser.Close( );
        }
    }
    #endregion

    #region Additional test attributes

    // You can use the following additional attributes as you write your tests:

    ////Use TestInitialize to run code before running each test 
    //[TestInitialize()]
    //public void MyTestInitialize()
    //{        
    //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
    //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
    //}

    ////Use TestCleanup to run code after each test has run
    //[TestCleanup()]
    //public void MyTestCleanup()
    //{        
    //    // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
    //    // For more information on generated code, see http://go.microsoft.com/fwlink/?LinkId=179463
    //}

    #endregion

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }
    private TestContext testContextInstance;
  }
}
