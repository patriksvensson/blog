---
Title: Debugging an UWP store app with WinDbg
Slug: debugging-an-uwp-store-app-with-windbg
Date: 2019-06-27
RedirectFrom: 2019/06/debugging-an-uwp-store-app-with-windbg/index.html
Tags:
- Uwp
- Debugging
- Microsoft Store
- WinDbg
---

I was encountering a rather irritating bug a couple of days ago that only manifested itself
when built via the native toolchain in release mode. I couldn't get it to manifest itself
when running from Visual Studio for some reason and the problem occurred so early in the app's
lifecycle that I didn't have time to create a minidump of the process.

<!--excerpt-->

Luckily, after some reading I learned that you could connect [WinDbg][1] directly to a 
UWP app via the `plmPackage` and `plmApp` parameters but this was kind of cumbersome - 
especially when debugging different versions and architecture of the package - so I decided
to write a little PowerShell script to automate starting of the debugging session.

```powershell
# Replace this value with the application ID that you 
# find in the applications Package.appxmanifest file. 
$AppName = "App"

# Get the package.
$Package = Get-AppxPackage -Name "MyPublisher.MyCoolPackage";
if($null -eq $Package) {
    throw "Could not find package!";
}

# Launch WinDbg
$WinDebugPath = "C:\Program Files (x86)\Windows Kits\10\Debuggers\$($Package.Architecture)\windbg.exe";
&$WinDebugPath -plmPackage $Package.PackageFullName -plmApp $AppName
```

Please note that you need to have [Windows Driver Kit][2] installed for this to work.

Happy debugging!

[1]:https://en.wikipedia.org/wiki/WinDbg
[2]:https://docs.microsoft.com/en-us/windows-hardware/drivers/download-the-wdk