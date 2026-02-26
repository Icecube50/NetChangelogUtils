namespace NetChangelogUtils
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var path = Console.ReadLine();

            var changlogUtils = new NetChangelogService();
            changlogUtils.Run(new CliOptions
            {
                DryRun = true,
                Path = path,
            });
        }

      
    }
}
