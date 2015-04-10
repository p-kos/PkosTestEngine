## PkosTestEngine
==========================
This is a framework to test web applications  made in C# 

###Prerequesites

1. Visual Studio 2010 Premiun / Ultimate

###Usage

Create a new CodeUITest class, if these message to Record new test appears just cancel it. Then heritance form WebApp class and you can start the testing.

`
public class CodeUITest1: WebApp
{
	public CodeUITest1()
	{
	}
	[TestMethod]
	public void Test1()
	{
		LaunchApp("http://google.com");
		FindControl<HtmlEdit>("#lst-ib").Text = "CodeUITest";
		FindControls<HtmlDiv>(".rc")[0].FindControls<HtmlHyperLink>().Where(a=>a.InnerTextContains("VerifyCode")).FirstOrDefault().Click();
	} 
}
`
###Cache
You can cache all the controls found in each page so the next time the test ui will be faster because the app will not search the control again.
To do so just need to add an *AppSetting* into *App.config* 
`<add key="CachePath" value="E:\SomeWhere\In\Your\Disk\cache.xml"/>`
