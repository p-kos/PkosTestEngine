using System;
using System.Collections.Generic;
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
using System.Linq;

namespace PkosTestEngine
{
  /// <summary>
  /// Summary description for TestSample
  /// </summary>
  [CodedUITest]
  public class TestSample:WebApp
  {
    public TestSample( )
    {
    }

    [TestMethod]
    public void CodedUITestMethod1( )
    {
      LaunchApp( "http://localhost:51449/login.aspx" );
      FindControl<HtmlEdit>( "#txtUserName" ).Text = "marco";
      FindControl<HtmlEdit>( "#txtPassword" ).Text = "a";
      FindControl<HtmlInputButton>( "#btnLogin" ).Click( );
      FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink rmRootLink" ).Where( item => item.InnerText == "Tax" ).FirstOrDefault().Click();
      FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Levy Management" ).FirstOrDefault( ).Click( );
      FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Reports" ).FirstOrDefault( ).Click( );
      var controls = FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>().Where( item => item.InnerText == "Reports" ).ToList();
      foreach ( HtmlHyperlink control in controls )
      {
        try
        {
          Mouse.Hover( control );
          var newItem = FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Levy Abstract Reports" ).FirstOrDefault();
          if ( newItem != null )
          {
            newItem.Click();
            break;
          }
        }
        catch{}
      }
      //var controls = FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Reports" );
      //FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Levy Abstract Reports" ).FirstOrDefault( ).Click( );
      //var controls = FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlListItem>();//.Where( li => !string.IsNullOrEmpty( li.InnerText ) && li.InnerText.Contains( "Tax" ) );


      //FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink rmRootLink" ).Where( item => item.InnerText == "Asmt Admin" ).FirstOrDefault().Click();
      //FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Assessment Maintenance" ).FirstOrDefault( ).Click( );
      //FindControl<HtmlDiv>( "#topmenu" ).FindControls<HtmlHyperlink>( ".rmLink" ).Where( item => item.InnerText == "Valuation Maintenance" ).FirstOrDefault( ).Click();
      //FindControl<HtmlCell>( "#ctl00_ContentPlaceHolder_mbSearch" ).Click();
      //FindControl<HtmlEdit>( "#ctl00_ContentPlaceHolder_txtDisplayName" ).Text = "Smith t";
      //FindControl<HtmlSpan>( "#ctl00_btnSearch" ).Click();
      //var results = FindControl<HtmlTable>( "#ctl00_ContentPlaceHolder_grdRevObjs" ).FindValues( td => !string.IsNullOrEmpty( td.InnerText ) && td.InnerText.Contains( "SMITH TIMOTHY R & TAMARA K" ) );
      //results[0].FindControls<HtmlCheckBox>()[ 0 ].Checked = true;

    }

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
