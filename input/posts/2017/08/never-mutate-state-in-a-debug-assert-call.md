---
Title: Never mutate state in a Debug.Assert call
Slug: never-mutate-state-in-a-debug-assert-call
Date: 2017-08-02
RedirectFrom: 2017/08/never-mutate-state-in-a-debug-assert-call/index.html
Tags:
- .NET
- C#
---

Yesterday I encountered a bug in a library I'm working on that only seemed
to occur when compiled in release mode. I was scratching my head for a little
while but suddenly it occured to me when I encountered the following line:

    Debug.Assert(reader.Read() == '-'); // Consume token

The problem with the above statement is that calls to `Debug.Assert` are conditional,
so when running under a non-debug build, the call won't be made and the expression
won't be evaluated.

## So what lessons can we learn from this?

Never mutate state in a `Debug.Assert` call.