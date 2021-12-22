---
Title: Intellisense for your GitHub Actions workflows
Slug: intellisense-for-your-github-actions-workflows
Published: 2021-12-22
Tags:
- GitHub
- GitHub Actions
---

Although I'm a big fan of GitHub, I'm not a big fan of YAML.
When it comes to orchestrating my builds, I usually prefer some kind 
of build script, but bootstrapping that build script is a necessary 
evil so we're stuck with YAML for that part.

<!--excerpt-->

Today I found out that there is a JSON schema for GitHub Actions over 
at [json.schemastore.org](https://json.schemastore.org/github-workflow.json), 
and that means that we can use 
[Red Hat's YAML VSCode extension](https://marketplace.visualstudio.com/items?itemName=redhat.vscode-yaml).

![A GitHub Actions job](/images/gh_workflow_schema.png)

_"What does a JSON schema have to do with a VSCode YAML extension?"_ you 
might ask. Excellent question. It turns out that Red Hat's YAML extension 
has support for JSON schemas out of the box.

To enable this for your GitHub Actions workflows, add the following 
line to the top of your workflow file:

```
# yaml-language-server: $schema=https://json.schemastore.org/github-workflow.json
```

Sanity restored.