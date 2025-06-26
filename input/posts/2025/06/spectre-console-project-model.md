---
Title: Updates to Spectre.Console's project model
Slug: spectre-console-project-model
Date: 2025-06-26
Tags:
- Spectre.Console
- Open Source
---

I have encountered numerous challenges with issues and pull requests in most open source projects, particularly in Spectre.Console. Frequently, submitted issues are not reproducible or lack essential information. In many cases, contributors neglect to follow the issue template, which creates additional work for us. 

The same applies to pull requests. We often receive submissions that are misaligned with the goals of the project and that we cannot or do not wish to merge, resulting in contributors wasting their time. To our credit, we have a [CONTRIBUTING.md][1] file that clearly states that all pull requests must be associated with a corresponding issue. The root of the problem, however, is that contributors often disregard the rules we have established, thereby increasing the workload for maintainers.

As a result of this, we currently have [234 open issues][2] and [35 open pull requests][3]. Often, when attempting to go through this backlog, we find that bug reports lack sufficient detail to reproduce the reported problem.

Of course, not all issues and pull requests are like this (far from it), but the noise adds up.

## A possible solution

About a year ago, I was granted access to the beta of [Ghostty][4] and noticed that they approach issue and PR management differently from most open source projects. Mitchell Hashimoto later [elaborated on this approach on Twitter][5].

In summary, they have moved away from receiving bug reports and feature requests via GitHub issues, relying instead on GitHub Discussions. Only when a bug discussed in a thread is deemed reproducible, or when a feature idea is considered sufficiently mature, do they create an issue with a complete description. All pull requests must have an associated issue. If someone opens an issue without a prior discussion that has been approved by a maintainer, the issue is promptly closed.

This policy has resulted in all [issues within Ghostty being well-defined][6] and containing all the necessary information to implement a solution.

## Going forward

After careful consideration and internal discussions among the Spectre.Console maintenance team, we have decided to adopt a similar approach. Accordingly, before the end of the summer, we intend to:

* Decommission the GitHub Projects we previously attempted to use
* Introduce new issue and pull request templates
* Close all existing issues and pull requests, accompanied by a message stating that if the item is still relevant, a GitHub Discussion should be created

This should guarantee that all issues are actionable, and that pull requests aren't a waste of the author's time.

[1]: https://github.com/spectreconsole/spectre.console/blob/main/CONTRIBUTING.md
[2]: https://github.com/spectreconsole/spectre.console/issues
[3]: https://github.com/spectreconsole/spectre.console/pulls
[4]: https://ghostty.org/
[5]: https://x.com/mitchellh/status/1852039089547309552
[6]: https://github.com/ghostty-org/ghostty/issues