using System.Diagnostics;
using System.Threading.Tasks;
using Baseline.Dates;
using StoryTeller;

namespace Storyteller.Redux
{
    public class ReduxSagaExtension : IExtension
    {
        private Process _process;
        public string Url { get; }

        public ReduxSagaExtension(string url)
        {
            Url = url;
            Server = new WebSocketServer();
        }

        public WebSocketServer Server { get; set; }

        public void Dispose()
        {
            Server.SendCloseMessage();
            Server.Dispose();
        }

        public Task Start()
        {
            return Task.Factory.StartNew(() =>
            {
                Server.Start();
            });
        }

        private void launchPage()
        {
            var url = Url.Contains("?")
                ? Url + $"&StorytellerPort={Server.Port}"
                : $"{Url}?StorytellerPort={Server.Port}";

            _process = ProcessLauncher.GotoUrl(url);
        }

        public void BeforeEach(ISpecContext context)
        {
            Server.SendCloseMessage();
            Server.ClearAll();

            launchPage();

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