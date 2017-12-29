using System.IO;
using System.Threading.Tasks;
using Baseline;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using myapp;
using Storyteller.Redux;
using StoryTeller;
using StoryTeller.Engine;

namespace ReduxSamples
{
    public class Program
    {
        public static void Debug()
        {
            using (var runner = StoryTeller.StorytellerRunner.For<ReduxSampleSystem>())
            {
                runner.Run("Samples / Simple sending and value checking");
                runner.OpenResultsInBrowser();
            }
        }

        public static void Main(string[] args)
        {
            StorytellerAgent.Run(args, new ReduxSampleSystem());
        }
    }

    public class ReduxSampleSystem : SimpleSystem
    {
        private const string WebHostUrl = "http://localhost:5050";
        private IWebHost _host;

        public ReduxSampleSystem()
        {
            // No request should take longer than 250 milliseconds
            PerformancePolicies.PerfLimit(250, r => r.Type == "Http Request");
        }
        protected override void configureCellHandling(CellHandling handling)
        {
            handling.Extensions.Add(new SeleniumReduxSagaExtension($"{WebHostUrl}/counter"));
        }

        public override Task Warmup()
        {
            Startup.TestDriver = true;
            _host = WebHost.CreateDefaultBuilder()
                    .UseContentRoot(CalculateRelativeContentRootPath())
                    .UseStartup<Startup>()
                    .UseUrls(WebHostUrl)
                    .Build();

            _host.Start();
            
            string CalculateRelativeContentRootPath() =>
              Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                 @"..\..\..\..\myapp");

            return base.Warmup();
        }

        public override void Dispose()
        {
            if(_host != null)
            {
                _host.SafeDispose();
            }
            base.Dispose();
        }
    }
}