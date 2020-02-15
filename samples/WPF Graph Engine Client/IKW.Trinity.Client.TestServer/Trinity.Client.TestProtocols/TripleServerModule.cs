using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace Trinity.Client.TestProtocols
{
    public class TripleModule : TripleServerBase
    {
        // Let's use RX to setup Subscription based processing of object with the intention of
        // the UI viewModel and WPF code-behind to hook-up to presenting to the UI; our Test Server
        // is pushing triples to our WPF client

        private IObserver<TripleStreamReader> TripleStreamReceivedActionObserver {get;set;}
        private IObserver<(long CellIdOnTriple, TripleStore NewTripleStore)> TripleSavedToMemoryCloudActionObserver {get; set; }
        private IObserver<Triple> TripleObjectProjectedActionObserver {get; set; }
        private IObserver<TripleStore> TripleStoreReadyInMemoryCloudActionObserver {get; set; }

        private IObservable<TripleStreamReader> TripleStreamReceivedActionSubscriber {get;set;}
        private IObservable<(long CellIdOnTriple, TripleStore NewTripleStore)> TripleSaveToMemoryCloudActionSubscriber {get;set;}
        private IObservable<Triple> TripleObjectProjectedActionSubscriber {get;set;}
        private IObservable<TripleStore> TripleStoreReadyInMemoryCloudActionSubscriber {get;set;}

        public ReactiveProperty<Triple> ExternalTripleReceivedAction {get; private set;}
        public ReactiveProperty<TripleStore> ExternalTripleSavedToMemoryAction {get; private set;}

        // Declare the Subscription for proper clean-up

        private IDisposable TripleObjectReceivedActionSubscription {get;set;}
        private IDisposable TripleSaveToMemoryCloudActionSubscription {get;set;}
        private IDisposable TripleObjectProjectedActionSubscription {get;set;}
        private IDisposable TripleStoreReadyInMemoryCloudActionSubscription {get;set;}

        private readonly TaskPoolScheduler  _moduleSubscribeOnScheduler;
        private readonly NewThreadScheduler _moduleObserverOnScheduler;

        private readonly TripleEqualityCompare _tripleEqualityCompare = null;
        private readonly TripleStoreEqualityCompare _tripleStoreEqualityCompare = null;

        // We modify the default class constructor 
        public TripleModule()
        {
            _moduleSubscribeOnScheduler = TaskPoolScheduler.Default;
            _moduleObserverOnScheduler  = NewThreadScheduler.Default;

            _tripleEqualityCompare = new TripleEqualityCompare();
            _tripleStoreEqualityCompare = new TripleStoreEqualityCompare();

            SetupObservers();
            SetUpObservables();
            SetupExternalReactiveProperties();
        }

        /// <summary>
        ///  
        /// </summary>
        private class TripleEqualityCompare : IEqualityComparer<Triple>
        {
            private readonly Triple EmptyTriple = new Triple(null);
            public bool Equals(Triple tripleObjectX, Triple tripleObjectY)
            {
                if (tripleObjectX == EmptyTriple)
                {
                    return false;
                }else if (tripleObjectY == EmptyTriple)
                {
                    return false;
                }else if (tripleObjectX != EmptyTriple && tripleObjectY != EmptyTriple)
                {
                    return tripleObjectX.Namespace.Equals(tripleObjectY.Namespace) &&
                           tripleObjectX.Subject.Equals(tripleObjectY.Subject) &&
                           tripleObjectX.Predicate.Equals(tripleObjectY.Predicate) &&
                           tripleObjectX.Object.Equals(tripleObjectY.Object);
                }else
                {
                    return false;
                }

            }

            public int GetHashCode(Triple obj)
            {
                return obj.Subject.GetHashCode();
            }
        }

        private class TripleStoreEqualityCompare : IEqualityComparer<TripleStore>
        {
            private readonly TripleStore EmptyTripleStore = new TripleStore()
            {
                CellId = 0,
                TripleCell = new Triple(null)
            };

            public bool Equals(TripleStore tripleStoreX, TripleStore tripleStoreY)
            {
                if (tripleStoreX == EmptyTripleStore)
                {
                    return false;
                }
                else if (tripleStoreY == EmptyTripleStore)
                {
                    return false;
                }
                else if (tripleStoreX != EmptyTripleStore && tripleStoreY != EmptyTripleStore)
                {
                    return !tripleStoreX.CellId.Equals(tripleStoreY.CellId) &&
                           tripleStoreX.TripleCell.Namespace.Equals(tripleStoreY.TripleCell.Namespace) &&
                           tripleStoreX.TripleCell.Subject.Equals(tripleStoreY.TripleCell.Subject) &&
                           tripleStoreX.TripleCell.Predicate.Equals(tripleStoreY.TripleCell.Predicate) &&
                           tripleStoreX.TripleCell.Object.Equals(tripleStoreY.TripleCell.Object);
                }
                else
                {
                    return false;
                }
            }

            public int GetHashCode(TripleStore obj)
            {
                return obj.CellId.GetHashCode(); 
            }
        }

        /// <summary>
        ///   Let's new-up and make-ready our reactive properties for use 
        /// </summary>
        private void SetupExternalReactiveProperties()
        {
            // We initialize the ReactiveProperties to trigger based on the internal Observable

            ExternalTripleReceivedAction = 
                new ReactiveProperty<Triple>(source: TripleObjectProjectedActionSubscriber,
                                             initialValue: new Triple(null), 
                                             mode: ReactivePropertyMode.None, 
                                             equalityComparer: _tripleEqualityCompare);

            ExternalTripleSavedToMemoryAction =
                new ReactiveProperty<TripleStore>(source: TripleStoreReadyInMemoryCloudActionSubscriber,
                    initialValue: default,
                    mode: ReactivePropertyMode.None, 
                    equalityComparer: _tripleStoreEqualityCompare);
        }

        /// <summary>
        /// Setup processing for Observables ... these are the code fragments that listen observers
        /// </summary>
        private void SetUpObservables()
        {
            // Proper Initialization ...

            TripleStreamReceivedActionSubscriber = Observable
                .Empty<TripleStreamReader>()
                .ObserveOn(scheduler: _moduleObserverOnScheduler);

            TripleSaveToMemoryCloudActionSubscriber = Observable
                .Empty<(long CellIdOnTriple, TripleStore NewTripleStore)>()
                .ObserveOn(scheduler: _moduleObserverOnScheduler);

            TripleObjectProjectedActionSubscriber = Observable
                .Empty<Triple>()
                .ObserveOn(scheduler: _moduleObserverOnScheduler);

            TripleStoreReadyInMemoryCloudActionSubscriber = Observable
                .Empty<TripleStore>()
                .ObserveOn(scheduler: _moduleObserverOnScheduler);

            // Reactive Subscriber Setup

            TripleObjectReceivedActionSubscription = TripleStreamReceivedActionSubscriber
                .SubscribeOn(_moduleObserverOnScheduler)
                .Select(selector: tripleObject => tripleObject)
                .ObserveOn(_moduleObserverOnScheduler)
                .Subscribe(TripleStreamReceivedActionObserver);

            TripleSaveToMemoryCloudActionSubscription = TripleSaveToMemoryCloudActionSubscriber
                .SubscribeOn(_moduleObserverOnScheduler)
                .Select(selector: tripleObject => tripleObject)
                .ObserveOn(_moduleObserverOnScheduler)
                .Subscribe(TripleSavedToMemoryCloudActionObserver);

            TripleObjectProjectedActionSubscription = TripleObjectProjectedActionSubscriber
                .SubscribeOn(_moduleObserverOnScheduler)
                .Select(selector: tripleObject => tripleObject)
                .ObserveOn(_moduleObserverOnScheduler)
                .Subscribe(TripleObjectProjectedActionObserver);

            TripleStoreReadyInMemoryCloudActionSubscription = TripleStoreReadyInMemoryCloudActionSubscriber
                .SubscribeOn(_moduleObserverOnScheduler)
                .Select(selector: tripleObject => tripleObject)
                .ObserveOn(_moduleObserverOnScheduler)
                .Subscribe(TripleStoreReadyInMemoryCloudActionObserver);
        }

        private void SetupObservers()
        {
            TripleStreamReceivedActionObserver =
                Observer.Create<TripleStreamReader>(onNext: async tripleStreamReader =>
                    {
                        Triple myTriple = tripleStreamReader.triples[0];

                        // Push the Triple Generated from the sever-side to the Client

                        TripleObjectProjectedActionObserver.OnNext(myTriple);

                        TripleStore myTripleStore = new TripleStore()
                        {
                            CellId = Trinity.Core.Lib.CellIdFactory.NewCellId(),
                            TripleCell = tripleStreamReader.triples[0]
                        };

                        (long CellIdOnTriple, TripleStore NewTripleStore) sourceTripleContext = (tripleStreamReader.m_cellId, myTripleStore);

                        // Let's save the TripleStore to the MemoryCloud and let the Client know 

                        using var tripleStreamReceivedActionTask = Task.Factory.StartNew(function: async () =>
                            {
                                await Task.Delay(0).ConfigureAwait(false);

                                TripleSavedToMemoryCloudActionObserver.OnNext(sourceTripleContext);

                                Debug.WriteLine($"Triple Received from Server via StreamTriplesAsync: Subject node: {tripleStreamReader.triples[0].Subject}");

                                return;
                            },
                            cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.AttachedToParent,
                            scheduler: TaskScheduler.Current).Unwrap();

                        var taskResult = tripleStreamReceivedActionTask.ConfigureAwait(false);

                        await taskResult;
                    },
                    onCompleted: () => { },
                    onError: errorContext => { });

            TripleSavedToMemoryCloudActionObserver = 
                Observer.Create<(long CellIdOnTriple, TripleStore NewTripleStore)>(onNext: async sourceTripleStore =>
                    {
                        using var tripleStoreSavedToMemoryCloudTask = Task.Factory.StartNew(function: async () =>
                            {
                                await Task.Yield();

                                // await Task.Delay(0).ConfigureAwait(false);

                                var (cellIdOnTriple, newTripleStore) = sourceTripleStore;

                                if (Global.CloudStorage.IsLocalCell(cellIdOnTriple)) return;

                                Global.CloudStorage.SaveTripleStore(newTripleStore);

                                //Global.LocalStorage.SaveTripleStore(newTripleStore);

                                // Let the Client know we have saved a New TripleStore to the MemoryCloud

                                TripleStoreReadyInMemoryCloudActionObserver.OnNext(newTripleStore);

                                Debug.WriteLine($"A New TripleStore Cell has been generated and Saved to the MemoryCloud");
                                Debug.WriteLine($"The WPF Client should be able to retrieve TripleStore Cell from the MemoryCloud: {newTripleStore.CellId}");
                            },
                            cancellationToken: CancellationToken.None,
                            creationOptions: TaskCreationOptions.HideScheduler,
                            scheduler: TaskScheduler.Current).Unwrap();

                        var taskResult = tripleStoreSavedToMemoryCloudTask.ConfigureAwait(false);

                        await taskResult;
                    },
                    onCompleted: () => { },
                    onError: errorContext => { });

            TripleObjectProjectedActionObserver = 
                Observer.Create<Triple>(onNext: sourceTriple =>
                    {
                        ExternalTripleReceivedAction.Value = sourceTriple;
                         
                        //using var tripleReceivedFromServerTask = Task.Factory.StartNew(function: async () =>
                        //    {
                        //        await Task.Delay(0).ConfigureAwait(false);

                        //        ExternalTripleReceivedAction.Value = sourceTriple;

                        //        Debug.WriteLine($"A New Triple has been received from the TripleStore Test Sever.");
                        //    },
                        //    cancellationToken: CancellationToken.None,
                        //    creationOptions: TaskCreationOptions.AttachedToParent,
                        //    scheduler: TaskScheduler.Current).Unwrap();

                        //var taskResult = tripleReceivedFromServerTask.ConfigureAwait(false);

                        //await taskResult;
                    },
                    onCompleted: () => { },
                    onError: errorContext => { });

            TripleStoreReadyInMemoryCloudActionObserver = 
                Observer.Create<TripleStore>(onNext: sourceTripleStore =>
                    {
                        ExternalTripleSavedToMemoryAction.Value = sourceTripleStore;

                        //using var tripleReceivedFromServerTask = Task.Factory.StartNew(function: async () =>
                        //    {
                        //        await Task.Delay(0).ConfigureAwait(false);

                                

                        //        Debug.WriteLine($"Notify the Client a new TripleStore object has been Saved to the MemoryCloud");

                        //        return;
                        //    },
                        //    cancellationToken: CancellationToken.None,
                        //    creationOptions: TaskCreationOptions.AttachedToParent,
                        //    scheduler: TaskScheduler.Current).Unwrap();

                        //var taskResult = tripleReceivedFromServerTask.ConfigureAwait(false);

                        //await taskResult;
                    },
                    onCompleted: () => { },
                    onError: errorContext => { });
        }

        public override string GetModuleName() => "TripleModule";

        // To be received on the server side
        public override void PostTriplesToServerHandler(TripleStreamReader request, ErrorCodeResponseWriter response)
        {
            // push to UI
            Debug.WriteLine("UI busy.");
            Debug.WriteLine($"Triple Received from Client via PostTripleToServer: Subject node: {request.triples[0].Subject}");
            //Thread.Sleep(10 + (new Random().Next(10)));
            Debug.WriteLine("UI done");
            response.errno = 100;
        }

        // To be received on the client side
        public override void StreamTriplesAsyncHandler(TripleStreamReader request, ErrorCodeResponseWriter response)
        {
            TripleStreamReceivedActionObserver.OnNext(request);

            response.errno = 99;
        }
    }
}
