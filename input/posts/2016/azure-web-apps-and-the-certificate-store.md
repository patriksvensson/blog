---
Title: Azure Web Apps and the certificate store
Slug: azure-web-apps-and-the-certificate-store
Published: 2016-12-07
Tags:
- Azure
---

I was trying to load a certificate from the certificate store in an Azure Web App today,
and for some reason I could not find it via it's thumbprint. Since I thought I was 
looking in the wrong certificate store, I went to [Kudu](https://github.com/projectkudu/kudu) 
to take a closer look via the PowerShell debug console.

My code (probably) wasn't wrong but the certificate simply wasn't there.

After some googling I found out that WebApps in Azure doesn't have access to uploaded
certificates unless you add a special AppSetting variable called `WEBSITE_LOAD_CERTIFICATES`.

You can either set this setting to a comma separated list of the certificate thumbprint that you want to expose,
or you can do what I did (*YOLO&#8482;*) and set it to `*` which exposes all available certificates
to the application.

Also, if you're interested in Kudu (which is awesome), check out [devlead](https://twitter.com/devlead)'s
appearance on *Azure Podcast* where he spends 30 minutes or so talking about it: 
[http://azpodcast.azurewebsites.net/post/Episode-147-Kudu?utm_source=twitterfeed&utm_medium=twitter](http://azpodcast.azurewebsites.net/post/Episode-147-Kudu?utm_source=twitterfeed&utm_medium=twitter)  