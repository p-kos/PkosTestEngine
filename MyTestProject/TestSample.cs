﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
using PkosTestEngine;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
using System.Linq;


namespace MyTestProject
{
    /// <summary>
    /// Summary description for TestSample
    /// </summary>
    [CodedUITest]
    public class TestSample: WebApp
    {
        public TestSample()
        {
        }

        [TestMethod]
        public void TestSample1()
        {
            LaunchApp("https://github.com/", true);
            FindControls<HtmlEdit>("input[name='q']").FirstOrDefault().Text = "PkosTestEngine";
            Keyboard.SendKeys("\n");

            FindControl<HtmlDiv>("#container").FindControl<HtmlHyperlink>("a[href='https://github.com/p-kos/PkosTestEngine']").Click();
            CloseApp( );
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
