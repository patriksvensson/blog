---
Title: Displaying TeamCity build status on GitHub
Slug: displaying-teamcity-build-status-on-github
Published: 2014-01-24
Tags:
- TeamCity
- GitHub
---

Have you ever wanted to display the current CI build status for a TeamCity project in your GitHub README? I did but couldn't find any good, straight forward information about how to do it. Turns out it's quite simple.

<!--excerpt-->

![Build Status](/images/tc_status_successful.png)

The markup example below shows the build status icon with a link to the TeamCity build status page.

	[![Build Status](http://buildserver.com/app/rest/builds/buildType:id:BUILD_CONFIG_ID/statusIcon)]
	(http://buildserver.com/viewType.html?buildTypeId=BUILD_CONFIG_ID&guest=1)

Of course, you'll have to replace `buildserver.com` with your own build server, and `BUILD_CONFIG_ID` with the build configuration ID of your project. You can find the ID under "General Settings" for your TeamCity project.

You also need to enable guest user login in the TeamCity administration page for the build status page to be available to everyone.