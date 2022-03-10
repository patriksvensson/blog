---
Title: The singleton logger
Slug: the-singleton-logger
Date: 2014-06-18
RedirectFrom: 2014/06/the-singleton-logger/index.html
Tags:
- .NET
- Logging
---

It is widely accepted that you access logging frameworks via a static singleton instance, and most logging frameworks are designed to work like this. But why? Many people often refer to it as being a cross-cross cutting concern; and that it's therefore not important to do things by the book. Not only does most logging frameworks use the static singleton as a façade, but they also store process-wide state.

<!--excerpt-->

Cross-cutting concern or not; most developers normally won't argue against the dependency inversion principle, but when it comes to logging it all seem forgotten. One can understand that logging should be simple, but if we went down that path we could justify anything by the same standard.

A static singleton is to me (in 99.99% of the cases) a sign that something is wrong; a code smell regardless of the justifications that should be handled with caution. 

## Where am I going with this?

I've started using the excellent [Serilog](http://serilog.net/) recently, and I was glad to see that while it provides a static log facade doesn't seem to encourage it for any other reason than to be more easily adoptable by other frameworks. I just visited their [GitHub page](https://github.com/serilog/serilog/wiki/Getting-Started), and in the footer you can read:

> Configuring and using the Log class is an optional convenience that makes it easier for libraries to adopt Serilog. Serilog does not and will never adopt any static/process-wide state within the logging pipeline itself."". 

Sounds like a small step in the right direction.