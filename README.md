Tarantula
=========

A silverlight based graphical book search engine

This application allows you to search Amazon.com for books and view the search results as an interactive physics based 'web' which you can navigate and manipulate. 

Update 6/22/2013: Amazon seem to have removed their crossdomain.xml and clientaccesspolicy.xml files meaning that silverlight applications can no longer make cross domain requests to the Amazon web services. This is fixable by proxying calls to the Amazon web service through the same domain as the Silverlight application, but unfortunately this is left as a task for the reader :)

Building source
---------------
To build the application you will need Visual Studio 2012 with the Silverlight 5 SDK installed. You will also need to have an Amazon web services account and set some constants to access the amazon web services.

The important constants can be found in Tarantula/Constants.cs

public const string MY_AWS_ASSOCIATE_TAG = "";

public const string MY_AWS_ACCESS_KEY_ID = "";

public const string MY_AWS_SECRET_KEY = "";