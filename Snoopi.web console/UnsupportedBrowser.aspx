<%@ Page Language="C#" AutoEventWireup="True" CodeFile="UnsupportedBrowser.aspx.cs" Inherits="Snoopi.web.UnsupportedBrowser" Culture="auto" UICulture="auto" %>
<%@ Import Namespace="dg.Utilities" %>
<%@ Import Namespace="Snoopi.core" %>
<%@ Import Namespace="Snoopi.web.Localization" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<link rel="stylesheet" href="/not-supported/style.css">
</head>
<body runat="server" id="body">

<div class="vhcentered">
	<div class="frame">
		<h1><%= UnsupportedBrowserStrings.GetHtml(@"Title")%></h1>
		<p><%= UnsupportedBrowserStrings.GetHtml(@"Paragraph1")%></p>
		<p><%= UnsupportedBrowserStrings.GetHtml(@"Paragraph2")%></p>
		<ul class="browsers">
			<li><div class="image"><a href="http://www.google.com/chrome"><img src="chrome.gif" alt="" /></a></div><a class="desc" href="http://www.google.com/chrome">Chrome 16.0+</a><div class="strong"><%= UnsupportedBrowserStrings.GetHtml(@"Recommended")%></div></li>
			<li><div class="image"><a href="http://www.mozilla.com/firefox/"><img src="firefox.gif" alt="" /></a></div><a class="desc" href="http://www.mozilla.com/firefox/">Firefox 8.0+</a></li>
			<li><div class="image"><a href="http://www.apple.com/safari/download/"><img src="safari.gif" alt="" /></a></div><a class="desc" href="http://www.apple.com/safari/download/">Safari 5.1+</a></li>
			<li><div class="image"><a href="http://www.opera.com/download/"><img src="opera.gif" alt="" /></a></div><a class="desc" href="http://www.opera.com/download/">Opera 10.0+</a></li>
			<li><div class="image"><a href="http://www.microsoft.com/windows/Internet-explorer/default.aspx"><img src="ie.gif" alt="" /></a></div><a class="desc" href="http://www.microsoft.com/windows/Internet-explorer/default.aspx">Internet Explorer 9+</a></li>
		</ul>

	</div>
</div>

</body>
</html>
