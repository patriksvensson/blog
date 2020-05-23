using System;
using System.Linq;
using Spectre.Cli;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace site.Commands
{
    [Description("Creates a new blog post draft")]
    public sealed class NewBlogPostCommand : Command<NewBlogPostCommand.Settings>
    {
        public sealed class Settings : CommandSettings
        {
            [CommandArgument(0, "<TITLE>")]
            [Description("The title of the blog post")]
            public string Title { get; set; }

            [CommandOption("-t|--tag <TAG>")]
            [Description("Adds a tag to the blog post")]
            public string[] Tags { get; set; }

            [CommandOption("-f|--force")]
            [Description("Overwrites the blog post if it already exist")]
            public bool Force { get; set; }
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Title))
            {
                return ValidationResult.Error("No title specified.");
            }
            return ValidationResult.Success();
        }

        public override int Execute(CommandContext context, Settings settings)
        {
            var (slug, published, content) = GenerateContent(settings);
            var relativePath = $"./input/posts/{published.Year}/{slug}.md";
            var absolutePath = Path.GetFullPath(relativePath);

            if (File.Exists(absolutePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("A blog post with the same slug already exist.");
                Console.ResetColor();
                return -1;
            }

            File.WriteAllText(absolutePath, content);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Created new blog post at {0}", relativePath);
            Console.ResetColor();

            return 0;
        }

        private (string slug, DateTime published, string content) GenerateContent(Settings settings)
        {
            var slug = GetSlug(settings);
            var published = DateTime.Now;

            var builder = new StringBuilder();
            builder.AppendLine("---");
            builder.AppendLine($"Title: {settings.Title}");
            builder.AppendLine($"Slug: {slug}");
            builder.AppendLine($"Published: {published.ToString("yyyy-MM-dd")}");
            builder.AppendLine($"Draft: True");

            if (settings.Tags?.Length > 0)
            {
                builder.AppendLine("Tags:");
                builder.AppendLine(string.Join(Environment.NewLine, settings.Tags.Select(tag => $"- {tag}")));
            }

            builder.AppendLine("---");
            builder.AppendLine();
            builder.AppendLine("_To be written_");

            return (slug, published, builder.ToString());
        }

        private string GetSlug(Settings settings)
        {
            var builder = new StringBuilder(settings.Title.Length);
            foreach (var token in settings.Title)
            {
                if (char.IsWhiteSpace(token))
                {
                    builder.Append("-");
                }
                else if (char.IsLetter(token))
                {
                    builder.Append(token);
                }
            }

            var title = builder.ToString();
            title = title.Replace("--", "-");

            return title.ToLowerInvariant();
        }
    }
}