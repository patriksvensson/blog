---
title: Introducing OpenCLI 
description: About three years ago, I was invited to meetings with the System.CommandLine team at Microsoft. They were in the process of finalizing System.CommandLine and wanted input from people in the community, and I was included due to my work on Spectre.Console and Spectre.Console.Cli.
slug: introducing-open-cli
repository: https://github.com/spectreconsole/open-cli
date: 2025-07-07
tags:
- OpenCLI
- Open Source
---

About three years ago, I was invited to meetings with the [System.CommandLine][1] team at Microsoft. They were in the process of finalizing System.CommandLine and wanted input from people in the community, and I was included due to my work on [Spectre.Console][2] and [Spectre.Console.Cli][3].

One of the things discussed briefly during those meetings was an open specification to describe the structure of a CLI, something akin to [OpenAPI][4], but for command-line applications. I absolutely _LOVED_ this idea, because I saw how many problems it could solve, not just for documentation generation, but also for creating strongly typed wrappers to interact with CLI tools programmatically.

Three years have passed, and I haven't seen anyone take the initiative to build something like this.  
So I figured: it's time I do something myself.

That's why today I want to introduce the [OpenCLI][5] initiative which (for now) is maintained under the _Spectre.Console_ GitHub organization. The specification is _a draft and still incomplete_, but I think it acts as a great starting point.

To paraphrase the specification:

> The OpenCLI specification (OCS) defines a standard, platform and language-agnostic interface to CLI applications, which allows both humans and computers to understand how a CLI tool should be invoked without access to source code or documentation.

I genuinely believe that today, especially with the growing interest in [MCP][6] (Model Context Protocol) and CLI automation, there's huge potential in standardizing how we describe command-line applications.

If you think this is a good idea or have thoughts, suggestions, or feedback, please don't hesitate to reach out.  
I'm interested in collaborating with anyone who is interested in this topic!

https://opencli.org  
https://github.com/spectreconsole/open-cli

[1]: https://github.com/dotnet/command-line-api
[2]: https://spectreconsole.net/
[3]: https://spectreconsole.net/cli/
[4]: https://spec.openapis.org/
[5]: https://opencli.org
[6]: https://modelcontextprotocol.io