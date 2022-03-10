---
Title: Using build counters in Azure DevOps
Slug: build-counters-in-azure-devops
Date: 2019-03-13
RedirectFrom: 2019/03/build-counters-in-azure-devops/index.html
Tags:
- Azure DevOps
---

We recently migrated some builds from TeamCity to Azure DevOps at my client, and couldn't
find a feature analog to TeamCity's `%build.counter%` which we've been using 
to get automatically incremented version numbers for artifacts.

<!--excerpt-->

Since we couldn't find the corresponding feature in Azure DevOps we settled on using the
`$(Build.BuildId)` variable which is the canonical ID for a build. Not exactly ideal
but it solved the immediate issue.

Today I found a way of accomplishing the same thing in Azure DevOps, but sadly it's not 
properly documented even if it's [briefly mentioned][1] in the official documentation. 
The following lines in your `azure-devops.yml` will set the build number to an
incrementing number that's scoped to the build definition.

```yml
name: 1.0.$(myBuildCounter)
variables:
  myBuildCounter: $[counter('buildCounter',1)]
```

The example above will create a build definition counter called `buildCounter` that
will be initialized to the value `1`. This value will be incremented for
each new build. If you ever want to reset or reseed this value you'll have to use the 
[Azure DevOps HTTP API][2].

If you have an option, consider using something like [GitVersion][3] 
or [MinVer][4] to dynamically calculate a version number from a git repository.

[1]:https://docs.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&tabs=yaml%2Cbatch#set-variables-using-expressions
[2]:https://docs.microsoft.com/en-us/rest/api/azure/devops/build/definitions?view=vsts-rest-tfs-4.1
[3]:https://gitversion.readthedocs.io/en/latest/
[4]:https://github.com/adamralph/minver