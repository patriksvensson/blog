---
Title: Conditionals in XAML
Slug: conditionals-in-xaml
Date: 2017-12-03
RedirectFrom: 2017/12/conditionals-in-xaml/index.html
Tags:
- .NET
- C#
- WPF
---

Ever wanted to display things conditionally in XAML based on a pre-processor directive like `DEBUG`?

Start by adding the following to your `AssemblyInfo.cs`

```
#if DEBUG
[assembly: XmlnsDefinition("http://github.com/me/myproject/debug", "MyProjectNamespace")]
#endif
```

Now, in your XAML file, you should be able to add a new namespace pointing to this definition. We also need to import the [XAML compatibility](https://docs.microsoft.com/en-us/dotnet/framework/wpf/advanced/markup-compatibility-mc-language-features) namespace. To suppress the errors given by the XAML processor, use the `mc:Ignorable` attribute.

You should now be able to use `AlternateContent`, `Choice` and (optionally) `Fallback` elements in your XAML code to show
different elements depending on your build configuration.


```xaml
<Window x:Class="MyProjectNamespace.MyView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    xmlns:debug="http://github.com/me/myproject/debug"
    mc:Ignorable="mc">

    <mc:AlternateContent>
        <mc:Choice Requires="debug">
            <TextBlock Text="Debugging!" />
        </mc:Choice>
        <mc:Fallback>
            <TextBlock Text="Not debugging!" />
        </mc:Fallback>
    </mc:AlternateContent>

</Window>
```

Be aware that content within `AlternateContent` will not show in the designer. If this is something you need, I recommend that you use a different technique.

**Update (2017-12-05)**  

The `AlternateContent` element doesn't seem to be supported in UWP applications.

<blockquote class="twitter-tweet" data-lang="en"><p lang="en" dir="ltr">WPF only. The attribute does not exist in UWP.</p>&mdash; diederik krols (@diederikkrols) <a href="https://twitter.com/diederikkrols/status/937942865484898305?ref_src=twsrc%5Etfw">December 5, 2017</a></blockquote>
<script async src="https://platform.twitter.com/widgets.js" charset="utf-8"></script>
