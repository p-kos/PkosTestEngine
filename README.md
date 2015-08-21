#PkosTestEngine

This is a framework to test web applications  made in C#, it just needs to use FindControl<T> statement and it will find the contorl in the current page, it does not need to specify the page when it changes, it like a write test cases.

It works for Internet Explorer for now.

##Prerequesites

1. Visual Studio 2010 Premiun / Ultimate

##Installation
You can clone the code and compile or you can download the compiled dll here [https://dl.dropboxusercontent.com/u/32795617/PkosTestEngine.zip](https://dl.dropboxusercontent.com/u/32795617/PkosTestEngine.zip "PkosTestEngine.zip")

##Getting Started

###Test Case
If you have a test case just convert it to code.

####Test Case #1:
1. Go to https://github.com
2. Search PkosTestEngine
3. Press [Return]
4. Click the first entry for search
  
###Syntax

Create a new CodeUITest class, if the message to Record new test appears just cancel it. Then inherits from WebApp class and you can start the testing.
The Internet Explorer must be opened before run test with the page (in this case https://github.com) opened, the reason is to not disturb to other pages in the browser, so the app will find that window.

	[CodedUITest]
	public class TestSample: WebApp
	{
		public TestSample()
		{
		}

		[TestMethod]
		public void TestSample1()
		{
			LaunchApp("https://www.google.com/");
			FindControl<HtmlEdit>("#lst-ib").Text = "CodeUITest";
			FindControl<HtmlButton>("button[name='btnG']").Click();
			FindControls<HtmlDiv>(".rc")[0]
				.FindControls<HtmlHyperlink>()
				.Where(a => a.InnerText.Contains("VerifyCode"))
				.FirstOrDefault().Click();
		}
	}

###FindControl&lt;T&gt;(string selector)
You can find any kind of control using selector the following selectors:

1. Id e.g. FindControl<HtmlEdit>("#text1")
2. Class e.g FindControl<HtmlEdit>(".texts") // it will return the first entry for control with the class

###FindControls&lt;T&gt;(string selector)
It returns a ReadOnlyCollection of the controls using the following selectors:

1. **Id** Collection with only one control asuming there is just one object with the id

	`FindControls<HtmlEdit>("#text1")`

2. **Class** Collection of the controls with css class .div1

	`FindControls<HtmlDiv>(".div1")`

3. **All objects**

	`FindControls<HtmlHyperlink>()`

	`FindControls<HtmlHyperLink>("a")`
4. **By Attribute value**
	
	a. All Tables with name *table1* 

	`FindControls<HtmlTable>("table[name='table1']")`

	b. All TD (Table cells) with class contains (ends, begin with, etc.) *pair*

	`FindControls<HtmlCell>("td[class*='pair']")`

###Other Extended methods
####Click()
It clicks over any kind of HtmlControl
`FindControl<HtmlButton>("#search").Click();`

####FindValues(Func&lt;HtmlCell, bool&gt; criteria)
It works only when the HtmlControl is a table (HtmlTable)
`ReadOnlyCollection<HtmlRow> results = FindControl<HtmlTable>("#results").FindValues(td=>!string.IsNullOrEmpty && td.InnerText == "some text");`

##Cache
You can cache all the controls found in each page so the next time the test ui will run faster because the app will not search the control over the page again.
To do so just need to add an *AppSetting* into **App.config**

	<appSettings>
		<add key="CachePath" value="E:\SomeWhere\In\Your\Disk\cache.xml"/>
	</appSettings>

If the file does not exists the app will create it. If the setting does not exists the app will not cache the controls.
###Do not use cache
If you want the app search the control even it is cached, you just need to delete the line where the control is saved in XML file.
Or you can specify by code search app the control even if is cached:

`FindControl<HtmlEdit>("#text1", true).Click();`


##Authors
Marco Zarate [[pkitos@gmail.com]()]

##Thanks
This code was inspired on Damian Zapart sample
[https://msdn.microsoft.com/en-us/magazine/dn532204.aspx]()

Thanks to my beautiful wife for all support and love.
##License
