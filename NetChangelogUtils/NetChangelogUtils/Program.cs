using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;

namespace NetChangelogUtils
{
    internal class Program
    {
        static int Main(string[] args)
        {

            Option<bool> dryRunOption = new("--dry-run")
            {
                Description = "Use to preview changes made by the tool",
                DefaultValueFactory = parseResult => false,
            };

            Option<bool> ignoreUnscopedOption = new("--ignore-unscoped")
            {
                Description = "Use to ignore changes that have not been added to a scope",
                DefaultValueFactory = parseResult => false,
            };

            Option<string> projectPathOption = new("--project", "-p")
            {
                Description = "Use to manually set path to the project directory",
                DefaultValueFactory = parseResult => Directory.GetCurrentDirectory(),
            };

            // Root command
            var rootCommand = new RootCommand("NetChangelogUtils");
            rootCommand.Options.Add(dryRunOption);
            rootCommand.Options.Add(ignoreUnscopedOption);
            rootCommand.Options.Add(projectPathOption);
         
            ParseResult parseResult = rootCommand.Parse(args);
            if (parseResult.Errors.Count == 0)
            {
                var options = new CliOptions
                {
                    DryRun = parseResult.GetValue(dryRunOption),
                    IgnoreUnscoped = parseResult.GetValue(ignoreUnscopedOption),
                    Path = parseResult.GetValue(projectPathOption)
                };

                var changelogUtils = new NetChangelogService();
                changelogUtils.Run(options);

                return 0;
            }
            foreach (ParseError parseError in parseResult.Errors)
            {
                Console.Error.WriteLine(parseError.Message);
                rootCommand.Parse("-h").Invoke();

            }
            return 1;
        }
    }
}
