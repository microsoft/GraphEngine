using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Diagnostics;
using ErrorCodeResponseReader = Trinity.Client.TestProtocols.ErrorCodeResponseReader;

namespace Trinity.WPF.TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrinityClient client = new TrinityClient("localhost:5304");

        public MainWindow()
        {
            InitializeComponent(); 
            
            TrinityConfig.CurrentRunningMode = RunningMode.Client;

            client.RegisterCommunicationModule<TripleModule>();

            // Hook up to Graph Engine Reactive .. 

            client.Start();

            var tripleModule = client.GetCommunicationModule<TripleModule>();

            var uiSyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            var token = Task.Factory.CancellationToken;

            // Setup Reactive Processing .. 

            tripleModule.ExternalTripleReceivedAction.Subscribe(onNext: async tripleObjectFromServer =>
            {
                using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                {
                    await Task.Factory.StartNew(() =>
                        {
                            NameSpaceTB.Text  = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                            SubjectTB.Text    = tripleObjectFromServer.Subject;
                            PredicateTB.Text  = tripleObjectFromServer.Predicate;
                            ObjectTB.Text     = tripleObjectFromServer.Object;

                        }, token, 
                           TaskCreationOptions.None, 
                           uiSyncContext).ConfigureAwait(false);

                }).ContinueWith(_ =>
                    {
                        ResponseTextBlock.Items.Add("Task ExternalTripleReceivedAction Complete...");
                    }, uiSyncContext);

                var upDateOnUITread = reactiveGraphEngineResponseTask.ConfigureAwait(false);

                await upDateOnUITread;
            });

            tripleModule.ExternalTripleSavedToMemoryAction.Subscribe(onNext: async tripleStoreMemoryContext =>
            {
                using var retrieveTripleStoreFromMemoryCloudTask = Task.Factory.StartNew(async () =>
                {
                    await Task.Factory.StartNew(() =>
                        {
                            foreach (var tripleNode in Global.LocalStorage.TripleStore_Selector())
                            {
                                if (tripleStoreMemoryContext.CellId != tripleNode.CellId) continue;

                                var node          = tripleNode.TripleCell;
                                var subjectNode   = node.Subject;
                                var predicateNode = node.Predicate;
                                var objectNode    = node.Object;

                                ResponseTextBlock.Items.Add($"Triple CellId in MemoryCloud: {tripleNode.CellId}");
                                ResponseTextBlock.Items.Add($"Subject Node: {subjectNode}");
                                ResponseTextBlock.Items.Add($"Predicate Node: {predicateNode}");
                                ResponseTextBlock.Items.Add($"Object Node: {objectNode}");

                                break;
                            }
                        }, token, 
                           TaskCreationOptions.None, 
                           uiSyncContext).ConfigureAwait(false);

                }).ContinueWith(_ =>
                    {
                        ResponseTextBlock.Items.Add("Task ExternalTripleSavedToMemoryAction Complete...");
                    }, uiSyncContext);

                var upDateOnUITread = retrieveTripleStoreFromMemoryCloudTask.ConfigureAwait(false);

                await upDateOnUITread;
            });
        }

        private async void TalkToGraphEngineClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var uiSyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            List<Triple> triples = new List<Triple> {new Triple {Subject = "WPF-GraphEngineClient", Predicate = "isA", Object = "Success"}};

            Task graphEngineRPCTask = Task.Factory.StartNew(async () =>
            {
                await Task.Yield();

                var message = new TripleStreamWriter(triples);

                var rsp = await client.PostTriplesToServer(message).ConfigureAwait(false);

                var token = Task.Factory.CancellationToken;

                await Task.Factory.StartNew(() =>
                {
                    this.ResponseTextBlock.Items.Add($"GE Server Response: {rsp.errno}");
                },
                    token, TaskCreationOptions.None, uiSyncContext).ConfigureAwait(false);
            }).ContinueWith(_ => 
            {
                ResponseTextBlock.Items.Add("Task graphEngineRPCTask Complete...");
            }
            , uiSyncContext);

            var taskResult = graphEngineRPCTask.ConfigureAwait(false);

            await taskResult;
        }
    }
}
