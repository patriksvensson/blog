---
Title: Culture agnostic string comparisons
Slug: culture-agnostic-string-comparisons
Date: 2013-08-22
RedirectFrom: 2013/08/culture-agnostic-string-comparisons/index.html
Tags:
- .NET
- C#
---

Something I've seen a lot at different clients is naive string comparison. The most common case is to do something involving String.ToLower() on both strings that are being compared and then an equality comparison of the result.

<!--excerpt-->

This is not always the correct way of doing it, and especially not if you at some point in time want people who's using different culture settings than you to run your application. You can read more about this problem in [this excellent blog post by Jeff Atwood][1]. 

What you should do is simply to use the overloads of the [String.Equals][2] method that takes a `StringComparison`, or use a [StringComparer][3] directly.

    [Fact]
    public void Test_String_Equality()
    {
       // Given
       string first = "HELLO WORLD";
       string second = "hello world";

       // When
       bool result = first.Equals(second, StringComparison.OrdinalIgnoreCase);
    
       // Then
       Assert.True(result);
    }

The `StringComparer` is also very valuable to use with collections when you need to do a lookup for a specific string (i.e. `HashSet<string>` or `IDictionary<String, T>`).

    [Fact]
    public void Test_String_Lookup()
    {
       // Given
       var dictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
       dictionary.Add("HELLO WORLD", 42);

       // When
       var result = dictionary.ContainsKey("hello world");

       // Then
       Assert.True(result);
    }


  [1]: http://www.codinghorror.com/blog/2008/03/whats-wrong-with-turkey.html
  [2]: http://msdn.microsoft.com/en-us/library/c64xh8f9.aspx
  [3]: http://msdn.microsoft.com/en-us/library/system.stringcomparer.aspx