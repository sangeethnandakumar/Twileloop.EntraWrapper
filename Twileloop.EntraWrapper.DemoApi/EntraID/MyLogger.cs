using Microsoft.Extensions.Options;
using Spectre.Console;

namespace Twileloop.EntraWrapper.DemoApi.EntraID
{
    public class MyLogger : IEntraEventLogger
    {
        private readonly ILogger<MyLogger> logger;
        private readonly IOptions<EntraConfig> entraConfig;
        private readonly IOptions<SecurityOptions> secuityOptions;

        public MyLogger(ILogger<MyLogger> logger, IOptions<EntraConfig> entraConfig, IOptions<SecurityOptions> secuityOptions)
        {
            this.logger = logger;
            this.entraConfig = entraConfig;
            this.secuityOptions = secuityOptions;
        }

        public void OnFailure(string message)
        {
            AnsiConsole.Write(new Markup($"[cornflowerblue]SECURITY LOG:[/] [red](Failure)[/] - [skyblue2]{message}[/]"));
            AnsiConsole.WriteLine();
        }

        public void OnInfo(string message)
        {
            AnsiConsole.Write(new Markup($"[cornflowerblue]SECURITY LOG:[/] [lightslateblue](Info)[/] - [skyblue2]{message}[/]"));
            AnsiConsole.WriteLine();
        }

        public void OnSuccess(string message)
        {
            AnsiConsole.Write(new Markup($"[cornflowerblue]SECURITY LOG:[/] [lightgreen](Success)[/] - [skyblue2]{message}[/]"));
            AnsiConsole.WriteLine();
        }
    }
}
