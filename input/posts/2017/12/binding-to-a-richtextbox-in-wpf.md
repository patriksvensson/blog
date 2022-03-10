---
Title: Binding to a RichTextBox in WPF
Slug: binding-to-a-richtextbox-in-wpf
Date: 2017-12-11
RedirectFrom: 2017/12/binding-to-a-richtextbox-in-wpf/index.html
Tags:
- .NET
- C#
- WPF
---

I've been doing some WPF development the last couple of weeks, and one thing that bugged me was that there is no way (as far as I know) to bind content to a `RichTextBox`. This makes it kind of difficult to follow the MVVM pattern since the view model needs intimate knowledge of the view.

In my case, I wanted to bind the textbox against a resource using a `pack://` URI, so I threw together some code to do this. I am far from an expert on WPF, so if you have any suggestions then please let me know!

```csharp
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MyApp.Controls
{
    public sealed class BindableRichTextBox : RichTextBox
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source",
                typeof(Uri), typeof(BindableRichTextBox),
                new PropertyMetadata(OnSourceChanged));

        public Uri Source
        {
            get => GetValue(SourceProperty) as Uri;
            set => SetValue(SourceProperty, value);
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is BindableRichTextBox rtf && rtf.Source != null)
            {
                var stream = Application.GetResourceStream(rtf.Source);
                if (stream != null)
                {
                    var range = new TextRange(rtf.Document.ContentStart, rtf.Document.ContentEnd);
                    range.Load(stream.Stream, DataFormats.Rtf);
                }
            }
        }
    }
}
```

You can now use a binding as you normally would in your XAML (or simply set it directly).

```xml
<Window x:Class="MyApp.MyView"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:ui="clr-namespace:MyApp.Controls"
        mc:Ignorable="d">
    <StackPanel>
        <ui:BindableRichTextBox Source="{Binding Licenses}" />
        <ui:BindableRichTextBox Source="pack://application:,,,/MyApp;component/MyApp/Resources/Document.rtf" />
    </StackPanel>
</Window>
```
