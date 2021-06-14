using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Trinity.Client;
using Trinity.Client.TestProtocols;
using Trinity.Client.TestProtocols.TripleServer;
using Trinity.Diagnostics;

namespace Trinity.WPF.TestClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrinityClient TrinityTripleModuleClient { get; set; } = new TrinityClient("GenNexusPrime.inknowworks.dev.net:10222");
        private TripleModule  TripleClientSideModule { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();

            TrinityConfig.CurrentRunningMode = RunningMode.Client;

            TrinityTripleModuleClient.RegisterCommunicationModule<TripleModule>();

            // Hook up to Graph Engine Reactive .. 

            TrinityTripleModuleClient.Start();

            TripleClientSideModule = TrinityTripleModuleClient.GetCommunicationModule<TripleModule>();

            var uiSyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            var token = Task.Factory.CancellationToken;

            // Setup Reactive Processing .. 

            TripleClientSideModule.TripleBySubjectReceivedAction
              .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
              .Do(onNext: async subscriberSource =>
                          {
                              var msg = "TripleBySubjectReceivedAction-1";

                              using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(
                                  async () =>
                                  {
                                      await Task.Factory.StartNew(() =>
                                          {
                                              ResponseTextBlock.Items.Add(
                                                  $"{msg} Subscription happened on this Thread: {Thread.CurrentThread.ManagedThreadId}");

                                          }, token,
                                          TaskCreationOptions.None,
                                          uiSyncContext);
                                  });

                              var upDateOnUITread = await reactiveGraphEngineResponseTask;
                          })
              .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
              .Subscribe(onNext: async tripleStore =>
                         {
                             using var reactiveGetTripleBySubjectTask = Task.Factory.StartNew(async () =>
                               {
                                   await Task.Factory.StartNew(() =>
                                       {
                                           CellIdTb.Text    = tripleStore.CellId.ToString();
                                           NameSpaceTb.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                           SubjectTb.Text   = tripleStore.TripleCell.Subject;
                                           PredicateTb.Text = tripleStore.TripleCell.Predicate;
                                           ObjectTb.Text    = tripleStore.TripleCell.Object;

                                       }, token,
                                       TaskCreationOptions.None,
                                       uiSyncContext);
                               }).ContinueWith(_ =>
                               {
                                   ResponseTextBlock.Items.Add("Task TripleObjectStreamedFromServerReceivedAction Complete...");
                               }, uiSyncContext);

                             var upDateOnUITread = reactiveGetTripleBySubjectTask;

                             await upDateOnUITread;
                         });


            // Reactive Event Stream Processing: TripleClientSideModule.TripleObjectStreamedFromServerReceivedAction

            TripleClientSideModule.TripleObjectStreamedFromServerReceivedAction.Subscribe(onNext: async tripleObjectFromServer =>
            {
                using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                {
                    await Task.Factory.StartNew(() =>
                    {
                        NameSpaceTb.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        SubjectTb.Text   = tripleObjectFromServer.Subject;
                        PredicateTb.Text = tripleObjectFromServer.Predicate;
                        ObjectTb.Text    = tripleObjectFromServer.Object;

                    }, token,
                    TaskCreationOptions.None,
                    uiSyncContext);
                }).ContinueWith(_ =>
                {
                    ResponseTextBlock.Items.Add("Task TripleObjectStreamedFromServerReceivedAction Complete...");
                }, uiSyncContext);

                var upDateOnUITread = reactiveGraphEngineResponseTask;

                await upDateOnUITread;
            });

            TripleClientSideModule.ServerStreamedTripleSavedToMemoryCloudAction
                        .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                        .Do(onNext: async subscriberSource =>
                            {
                                var msg = "ServerStreamedTripleSavedToMemoryCloudAction-1";

                                using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(
                                    async () =>
                                    {
                                        await Task.Factory.StartNew(() =>
                                        {
                                            ResponseTextBlock.Items.Add("Incoming TripleStore Object retrieved from MemoryCloud.");
                                            ResponseTextBlock.Items.Add($"{msg} Subscription happened on this Thread: {Thread.CurrentThread.ManagedThreadId}");

                                        },
                                            token,
                                            TaskCreationOptions.None,
                                            uiSyncContext);
                                    });

                                var upDateOnUITread = await reactiveGraphEngineResponseTask;
                            })
                        .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                        .Subscribe(onNext: async tripleObjectFromMC =>
                           {
                               using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                               {
                                   await Task.Factory.StartNew(() =>
                                       {
                                           var myTripleStore = tripleObjectFromMC.NewTripleStore;

                                           CellIdTb.Text    = tripleObjectFromMC.NewTripleStore.CellId.ToString();
                                           NameSpaceTb.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                                           SubjectTb.Text   = myTripleStore.TripleCell.Subject;
                                           PredicateTb.Text = myTripleStore.TripleCell.Predicate;
                                           ObjectTb.Text    = myTripleStore.TripleCell.Object;

                                       }, token,
                                          TaskCreationOptions.None,
                                          uiSyncContext);
                               }).ContinueWith(_ =>
                                   {
                                       ResponseTextBlock.Items.Add("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
                                   }, uiSyncContext);

                               var upDateOnUITread = reactiveGraphEngineResponseTask;

                               await upDateOnUITread;
                           });

            TripleClientSideModule.ClientPostedTripleStoreReadyInMemoryCloudHotAction.Connect();

            TripleClientSideModule.TripleByCellIdReceivedAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: async subscriberSource =>
                    {
                        var msg = "TripleByCellIdReceivedAction-1";

                        using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(
                            async () =>
                            {
                                await Task.Factory.StartNew(() =>
                                {
                                    ResponseTextBlock.Items.Add($"Reactive Async - Server-Side Get Request on behalf of the Client.");
                                    ResponseTextBlock.Items.Add($"{msg} Subscription happened on this Thread: {Thread.CurrentThread.ManagedThreadId}");
                                },
                                token,
                                TaskCreationOptions.None,
                                uiSyncContext);
                        });

                        var upDateOnUITread = await reactiveGraphEngineResponseTask;
                    })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Synchronize()
                .Subscribe(onNext: async tripleObjectFromGetRequest =>
                {
                    using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(async () =>
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            var myTripleStore = tripleObjectFromGetRequest.TripleCell;

                            CellIdTb.Text    = tripleObjectFromGetRequest.CellId.ToString();
                            NameSpaceTb.Text = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                            SubjectTb.Text   = myTripleStore.Subject;
                            PredicateTb.Text = myTripleStore.Predicate;
                            ObjectTb.Text    = myTripleStore.Object;

                        }, 
                        token,
                        TaskCreationOptions.None,
                        uiSyncContext);
                    }).ContinueWith(_ =>
                    {
                        ResponseTextBlock.Items.Add("Task ServerStreamedTripleSavedToMemoryCloudAction Complete...");
                    }, uiSyncContext);

                    var upDateOnUITread = reactiveGraphEngineResponseTask;

                    await upDateOnUITread;
                });

            TripleClientSideModule.ClientPostedTripleSavedToMemoryCloudAction
                .ObserveOn(TripleClientSideModule.ObserverOnNewThreadScheduler)
                .Do(onNext: async subscriberSource =>
                    {
                        var msg = "ClientPostedTripleSavedToMemoryCloudAction-1";

                        using var reactiveGraphEngineResponseTask = Task.Factory.StartNew(
                            async () =>
                            {
                                await Task.Factory.StartNew(() =>
                                                            {
                                                                ResponseTextBlock.Items.Add($"Success! Found the Triple in the TripleStore MemoryCloud.");
                                                                ResponseTextBlock.Items.Add($"{msg} Subscription happened on this Thread: {Thread.CurrentThread.ManagedThreadId}");
                                                            },
                                    token,
                                    TaskCreationOptions.None,
                                    uiSyncContext);
                            });

                        var upDateOnUITread = await reactiveGraphEngineResponseTask;
                    })
                .SubscribeOn(TripleClientSideModule.SubscribeOnEventLoopScheduler)
                .Synchronize()
                .Subscribe(onNext: async tripleStoreMemoryContext =>
                {
                    //using var reactToTriplePostedSavedToMemoryCloudTask = Task.Factory.StartNew(async () =>
                    //{
                    //        //await Task.Yield();

                    //        await Task.Delay(0).ConfigureAwait(false);

                    //    Log.WriteLine("Success! Found the Triple in the TripleStore MemoryCloud");

                    //    var tripleStore = tripleStoreMemoryContext.NewTripleStore;
                    //    var subjectNode = tripleStore.TripleCell.Subject;
                    //    var predicateNode = tripleStore.TripleCell.Predicate;
                    //    var objectNode = tripleStore.TripleCell.Object;

                    //    Log.WriteLine($"Triple CellId in MemoryCloud: {tripleStoreMemoryContext.NewTripleStore.CellId}");
                    //    Log.WriteLine($"Subject Node: {subjectNode}");
                    //    Log.WriteLine($"Predicate Node: {predicateNode}");
                    //    Log.WriteLine($"Object Node: {objectNode}");

                    //}, cancellationToken: CancellationToken.None,
                    //       creationOptions: TaskCreationOptions.HideScheduler,
                    //       scheduler: TaskScheduler.Current).Unwrap().ContinueWith(async _ =>
                    //       {
                    //           await Task.Delay(0);

                    //           Log.WriteLine("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
                    //       }, cancellationToken: CancellationToken.None);

                    //var storeFromMemoryCloudTask = reactToTriplePostedSavedToMemoryCloudTask;

                    //await storeFromMemoryCloudTask;

                    using var retrieveTripleStoreFromMemoryCloudTask = Task.Factory.StartNew(async () =>
                    {
                        await Task.Factory.StartNew(() =>
                        {
                            foreach (var tripleNode in Global.LocalStorage.TripleStore_Selector())
                            {
                                if (tripleStoreMemoryContext.NewTripleStore.CellId != tripleNode.CellId)
                                    continue;

                                var node          = tripleNode.TripleCell;
                                var subjectNode   = node.Subject;
                                var predicateNode = node.Predicate;
                                var objectNode    = node.Object;

                                ResponseTextBlock
                                    .Items
                                    .Add($"Triple CellId in MemoryCloud: {tripleNode.CellId}");

                                ResponseTextBlock.Items.Add($"Subject Node: {subjectNode}");
                                ResponseTextBlock.Items.Add($"Predicate Node: {predicateNode}");
                                ResponseTextBlock.Items.Add($"Object Node: {objectNode}");

                                break;
                            }
                        }, 
                            token,
                            TaskCreationOptions.None,
                            uiSyncContext);
                    }).ContinueWith(_ =>
                        {
                            ResponseTextBlock.Items.Add("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
                        }, uiSyncContext);

                    var upDateOnUITread = retrieveTripleStoreFromMemoryCloudTask;

                    await upDateOnUITread;
                });

            //TripleClientSideModule.ClientPostedTripleSavedToMemoryCloudAction.Subscribe(onNext: async tripleStoreMemoryContext =>
            //{
            //    using var retrieveTripleStoreFromMemoryCloudTask = Task.Factory.StartNew(async () =>
            //    {
            //        await Task.Factory.StartNew(() =>
            //        {
            //            foreach (var tripleNode in Global.LocalStorage.TripleStore_Selector())
            //            {
            //                //if (tripleStoreMemoryContext.CellId != tripleNode.CellId)
            //                //    continue;

            //                var node          = tripleNode.TripleCell;
            //                var subjectNode   = node.Subject;
            //                var predicateNode = node.Predicate;
            //                var objectNode    = node.Object;

            //                ResponseTextBlock
            //                   .Items
            //                   .Add($"Triple CellId in MemoryCloud: {tripleNode.CellId}");

            //                ResponseTextBlock.Items.Add($"Subject Node: {subjectNode}");
            //                ResponseTextBlock.Items.Add($"Predicate Node: {predicateNode}");
            //                ResponseTextBlock.Items.Add($"Object Node: {objectNode}");

            //                break;
            //            }
            //        }, 
            //        token,
            //        TaskCreationOptions.None,
            //        uiSyncContext);
            //    }).ContinueWith(_ =>
            //    {
            //        ResponseTextBlock.Items.Add("Task ClientPostedTripleSavedToMemoryCloudAction Complete...");
            //    }, uiSyncContext);

            //    var upDateOnUITread = retrieveTripleStoreFromMemoryCloudTask;

            //    await upDateOnUITread;
            //});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void TalkToGraphEngineClientBtn_Click(object sender, RoutedEventArgs e)
        {
            var uiSyncContext = TaskScheduler.FromCurrentSynchronizationContext();

            List<Triple> triples = new List<Triple> { new Triple { Subject = "WPF-GraphEngineClient", Predicate = "isA", Object = "Success" } };

            Task taskResult;
            using var graphEngineRPCTask = Task.Factory.StartNew(async () =>
             {
                 using var message =
                     new TripleStreamWriter(triples);

                 var rsp = await TrinityTripleModuleClient
                     .PostTriplesToServer(message);

                 var token = Task.Factory.CancellationToken;

                 await Task.Factory.StartNew(
                     () =>
                     {
                         this.ResponseTextBlock.Items.Add(
                             $"GE Server Response: {rsp.errno}");
                     }, token, TaskCreationOptions.None,
                     uiSyncContext);
             })
           .ContinueWith(_ =>
                         {
                             ResponseTextBlock.Items.Add("Task graphEngineRPCTask Complete...");
                         }, uiSyncContext);
            {
                taskResult = graphEngineRPCTask;
            }

            await taskResult;
        }
    }
}
