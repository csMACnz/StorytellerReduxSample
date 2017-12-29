using System;
using System.Threading.Tasks;
using Baseline.Dates;
using Storyteller.Redux;
using StoryTeller;

namespace ReduxSamples
{
    public class SeleniumReduxSagaExtension : IExtension
    {
        public string Url { get; }

        private Lazy<BrowserDriver> _phantomDriver = new Lazy<BrowserDriver>(() => new BrowserDriver());

        public SeleniumReduxSagaExtension(string url)
        {
            Url = url;
            Server = new WebSocketServer();
        }

        public WebSocketServer Server { get; set; }

        public void Dispose()
        {
            Server.SendCloseMessage();
            Server.Dispose();
            _phantomDriver.Value.Dispose();
        }

        public Task Start()
        {
            return Task.Factory.StartNew(() =>
            {
                Server.Start();
            });
        }

        private void LaunchPage()
        {
            var url = Url.Contains("?")
                ? Url + $"&StorytellerPort={Server.Port}"
                : $"{Url}?StorytellerPort={Server.Port}";
            _phantomDriver.Value.LaunchUrl(url);
        }

        public void BeforeEach(ISpecContext context)
        {
            Server.SendCloseMessage();
            Server.ClearAll();

            LaunchPage();

            var reduxContext = new ReduxSpecContext(context);
            Server.CurrentContext = reduxContext;

            context.State.Store(reduxContext);
            context.State.Store(Server);

            Server.WaitForConnection(15.Seconds()).Wait();
        }

        public void AfterEach(ISpecContext context)
        {

        }
    }
}