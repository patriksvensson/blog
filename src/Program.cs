using MonorailCss.Theme;
using MyLittleContentEngine.BlogSite;
using MyLittleContentEngine.BlogSite.Components;
using MyLittleContentEngine.MonorailCss;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddBlogSite(_ => new BlogSiteOptions
    {
        // Basic site information
        SiteTitle = "patriksvensson.se",
        Description = "Patrik Svensson's blog",
        CanonicalBaseUrl = "https://patriksvensson.se",

        ColorScheme = new AlgorithmicColorScheme()
        {
            PrimaryHue = 200,
            ColorSchemeGenerator = i => (i + 260, i + 15, i -15),
            BaseColorName = ColorNames.Neutral,
        },

        AdditionalRoutingAssemblies = [typeof(Program).Assembly],
        ContentRootPath = "Content",
        BlogContentPath = "blog",
        BlogBaseUrl = "/blog",
        TagsPageUrl = "/tags",

        // Blog configuration
        AuthorName = "Patrik Svensson",
        AuthorBio = "",
        EnableRss = true,
        EnableSitemap = true,

        // Navigation links
        MainSiteLinks =
        [
            new HeaderLink("About", "/about"),
            new HeaderLink("Hire me", "/hire"),
        ],

        // Advanced customization
        ExtraStyles = """
                      .blog-header {
                          background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                      }
                      """,

        AdditionalHtmlHeadContent = """
                                    <link rel="preconnect" href="https://fonts.googleapis.com">
                                    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
                                    <link href="https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&family=Noto+Sans+Display:ital,wght@0,100..900;1,100..900&display=swap" rel="stylesheet">

                                    """,

        // Social media links
        Socials =
        [
            new SocialLink(
                Icon: SocialIcons.GithubIcon,
                Url: "https://github.com/patriksvensson"
            ),
            new SocialLink(
                Icon: SocialIcons.MastadonIcon,
                Url: "https://mstdn.social/@patriksvensson"
            ),
            new SocialLink(
                Icon: SocialIcons.BlueskyIcon,
                Url: "https://bsky.app/patriksvensson.se"
            ),
            new SocialLink(
                Icon: SocialIcons.LinkedInIcon,
                Url: "https://www.linkedin.com/in/psvensson82/"
            )
        ],

        // Project showcase
        MyWork =
        [
            new Project(
                Title: "Spectre.Console",
                Description:
                "A .NET library that makes it easier to create beautiful, cross platform, console applications.",
                Url: "https://spectreconsole.net"
            ),
            new Project(
                Title: "Cake",
                Description:
                " Cake (C# Make) is a free and open source cross-platform build automation system with a C# DSL for tasks such as compiling code, copying files and folders, running unit tests, compressing files and building NuGet packages.\n\n",
                Url: "https://cakebuild.net"
            ),
            new Project(
                Title: "OpenCLI",
                Description:
                "The OpenCLI specification (OCS) defines a standard, platform and language agnostic interface to CLI applications which allows both humans and computers to understand how a CLI tool should be invoked without access to source code or documentation.",
                Url: "https://opencli.org"
            )
        ],
    });

var app = builder.Build();
app.UseBlogSite();
app.MapStaticAssets();
await app.RunBlogSiteAsync(args);