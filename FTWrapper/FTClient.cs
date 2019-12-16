using FTWrapper.Events;
using Futu.OpenApi;
using Futu.OpenApi.Pb;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Futu.OpenApi.Pb.TrdCommon;

namespace FTWrapper
{
    /*
    class Limitation1
    {
        public (int reqNum, int duration) freq1;
        public (int reqNum, int duration) freq2;
    }

    class RequestWithLimition
    {
        private FTClient client = null;

        private static readonly Limitation unLockLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation placeOrderLmt = new Limitation { freq1 = (15, 30), freq2 = (5, 1) };
        private static readonly Limitation modifyOrderLmt = new Limitation { freq1 = (20, 30), freq2 = (5, 1) };
        private static readonly Limitation getMaxTrdQtyLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderListLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderFillListLmt = new Limitation { freq1 = (10, 30) };        
        private static readonly Limitation getRehabLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getSymbolsLmt = new Limitation { freq1 = (10, 30) };


        private static readonly object reqHisKLListLock = new object();
        private static readonly object reqHisKLQueueLock = new object();
        private static readonly Limitation reqHistoryKLLmt = new Limitation { freq1 = (10, 30) };

        // variable for request history KLines limitation
        private Dictionary<ReqHisKL, DateTime> reqHisKLDic = new Dictionary<ReqHisKL, DateTime>();
        private ObservableCollection<ReqHisKL> reqHisKLQueue = new ObservableCollection<ReqHisKL>();
        private ManualResetEvent mreReqHisData = new ManualResetEvent(false);
        private Thread reqHisDataThread;
        private bool isReqHisDataRunning = false;

        // variable for placing order limitation
        private Dictionary<ReqHisKL, DateTime> placeOrderDic = new Dictionary<ReqHisKL, DateTime>();
        private ObservableCollection<ReqHisKL> placeOrderQueue = new ObservableCollection<ReqHisKL>();
        private ManualResetEvent mreplaceOrder = new ManualResetEvent(false);
        private Thread placeOrderThread;
        private bool isPlaceOrderRunning = false;

        // variable for modifying order limitation
        private Dictionary<ReqHisKL, DateTime> modifyOrderDic = new Dictionary<ReqHisKL, DateTime>();
        private ObservableCollection<ReqHisKL> modifyOrderQueue = new ObservableCollection<ReqHisKL>();
        private ManualResetEvent mremodifyOrder = new ManualResetEvent(false);
        private Thread modifyOrderThread;
        private bool ismodifyOrderRunning = false;

        public RequestWithLimition(FTClient client)
        {
            this.client = client;
            reqHisKLQueue.CollectionChanged += ReqHisKLQueue_CollectionChanged;
            placeOrderQueue.CollectionChanged += PlaceOrderQueue_CollectionChanged;
            Start();
        }

        private void PlaceOrderQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Start()
        {
            reqHisDataThread = new Thread(HandleHisDataQueue);
            reqHisDataThread.IsBackground = true;
            reqHisDataThread.Start();


        }




        #region Request History KLine
        public void HandleHisDataQueue()
        {
            bool inProcesss = false;
            while (true)
            {
                if (!inProcesss)
                {
                    inProcesss = true;
                    isReqHisDataRunning = true;
                    mreReqHisData.Reset();
                    Task.Run(() => ProcessReqHisKLQueue())
                        .ContinueWith(result =>
                        {                            
                            inProcesss = false;
                            isReqHisDataRunning = false;
                            if (reqHisKLQueue.Count > 0)
                                mreReqHisData.Set();
                        });
                }                    
                mreReqHisData.WaitOne();
            }
        }
        
        private void ReqHisKLQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (reqHisKLQueue.Count > 0)
            {
                if (!isReqHisDataRunning)
                    mreReqHisData.Set();
            }
            else
                mreReqHisData.Reset();

            if (e.NewItems != null) {
                foreach (var item in e.NewItems)
                {
                    //Console.WriteLine("{0} added", ((ReqHisKL)item).Security.Code);
                }
            }
            
        }
        private static QotRequestHistoryKL.Request.Builder MakeReqHisKLBuilder(ReqHisKL request)
        {
            QotRequestHistoryKL.Request.Builder reqBuilder = QotRequestHistoryKL.Request.CreateBuilder();
            QotRequestHistoryKL.C2S.Builder csReqBuilder = QotRequestHistoryKL.C2S.CreateBuilder();
            QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
            stock.SetCode(request.Security.Code);
            stock.SetMarket((int)request.Security.Market);
            csReqBuilder.Security = stock.Build();
            csReqBuilder.RehabType = (int)request.RehabType;
            csReqBuilder.KlType = (int)request.KLType;
            csReqBuilder.BeginTime = request.Begin.ToString("yyyy-MM-dd");
            csReqBuilder.EndTime = request.End.ToString("yyyy-MM-dd");
            reqBuilder.SetC2S(csReqBuilder);
            return reqBuilder;
        }
        public async void RequestHistoryKL(ReqHisKL request)
        {
            try
            {
                // if found in the queue, update request and exit
                var item = reqHisKLQueue.Where(x => x.Security == request.Security)
                                        .LastOrDefault();
                if (item != null)
                {
                    item.Begin = request.Begin;
                    item.End = request.End;
                    item.KLType = request.KLType;
                    return;
                }

                
                // remove out-of-date item
                const int removal_period = 3600; //
                foreach (var req in reqHisKLDic.Where(x => (DateTime.Now - x.Value).TotalSeconds > removal_period).ToList())
                {
                    lock (reqHisKLListLock)
                    {
                        reqHisKLDic.Remove(req.Key);
                    }
                }

                var lmt = reqHistoryKLLmt.freq1;
                int tmp = reqHisKLDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration).Count();
                bool isAdded = false;
                if (tmp < lmt.reqNum)
                {
                    lock (reqHisKLListLock)
                    {
                        reqHisKLDic.Add(request, DateTime.Now);
                    }
                    QotRequestHistoryKL.Request.Builder reqBuilder = MakeReqHisKLBuilder(request);
                    bool succeed = await client.RequestHistoryKL(reqBuilder.Build());
                    if (succeed)
                        isAdded = true;
                    else
                    {
                        // Console.WriteLine("{0} request history KL failed", request.Security.Code);
                    }                        
                }
                if (!isAdded)
                {
                    lock (reqHisKLQueueLock)
                    {
                        reqHisKLQueue.Add(request);
                    }
                    lock (reqHisKLListLock)
                    {
                        reqHisKLDic.Remove(request);
                    }
                }                
            }
            catch (Exception e)
            {
                throw e;
            }            
        }
        private async Task<List<(ReqHisKL request, bool succeed)>> ProcessReqHisKLQueue()
        {
            List<(ReqHisKL request, bool succeed)> result = new List<(ReqHisKL request, bool succeed)>();
            var lmt = reqHistoryKLLmt.freq1;
            HistoryKLQuota quota = await client.RequestHistoryKLQuota();
            //Console.WriteLine("Remain Quota(ProcessReqHisKLQueue):" + quota.RemainQuota);
            List<ReqHisKL> queue = null;
            lock (reqHisKLQueueLock)
            {
                queue = reqHisKLQueue.ToList();
            }            
            for (int i = 0; i < queue.Count; i++)
            {
                var request = queue[i];
                if (quota.RemainQuota <= 0)
                {
                    result.Add((request, false));
                    lock (reqHisKLQueueLock)
                    {
                        reqHisKLQueue.Remove(request);
                    }
                }
                else
                {
                    var list = reqHisKLDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration)?
                        .OrderByDescending(x => x.Value)?.ToList();
                    if (list != null && list.Count < lmt.reqNum)
                    {
                        QotRequestHistoryKL.Request.Builder reqBuilder = MakeReqHisKLBuilder(request);
                        bool succeed = await client.RequestHistoryKL(reqBuilder.Build());
                        if (succeed)
                        {
                            lock (reqHisKLListLock)
                            {
                                reqHisKLDic.Add(request, DateTime.Now);
                            }
                            lock (reqHisKLQueueLock)
                            {
                                reqHisKLQueue.Remove(request);
                            }
                            quota.RemainQuota--;
                        } 
                        else
                        {
                            await Task.Delay(2000);
                            i--;
                        }
                    }
                    else
                    {
                        if (list != null)
                        {
                            var last = list.FirstOrDefault();
                            await Task.Delay(30 * 1000 - (int)(DateTime.Now - last.Value).TotalMilliseconds);
                        }
                        i--;
                    }
                }                
            }
            return result;
        }
        #endregion Request History Klines
    }
*/

    public class ConnCallback : FTSPI_Conn
    {
        #region EventHandlers
        public event EventHandler<InitConnectedEventArgs> InitConnected;
        public event EventHandler<DisconnectedEventArgs> Disconnected;

        protected virtual void OnInitConnected(InitConnectedEventArgs e)
        {
            InitConnected?.Invoke(this, e);
        }
        protected virtual void OnDisconnected(DisconnectedEventArgs e)
        {
            Disconnected?.Invoke(this, e);
        }

        #endregion

        private FTClient ftClient = null;
        private string prop = string.Empty;
        public ConnCallback() { }
        
        public ConnCallback(FTClient client, string propName)
        {
            ftClient = client;
            prop = propName;
        }
        public void OnInitConnect(FTAPI_Conn client, long errCode, string desc)
        {
            Console.WriteLine("InitConnected");
            if (errCode == 0)
            {                
                ftClient.GetType().GetProperty(prop).SetValue(ftClient, true);
                QotGetSubInfo.Request.Builder infoBuilder = QotGetSubInfo.Request.CreateBuilder();
                QotGetSubInfo.C2S.Builder csInfoBuilder = QotGetSubInfo.C2S.CreateBuilder();
                infoBuilder.SetC2S(csInfoBuilder);
                //ftClient.Qot.GetSubInfo(infoBuilder.Build());
            }
            else
            {
                ftClient.GetType().GetProperty(prop).SetValue(ftClient, false);
            }
            InitConnectedEventArgs args = new InitConnectedEventArgs(client, errCode, desc);
            OnInitConnected(args);
        }

        public void OnDisconnect(FTAPI_Conn client, long errCode)
        {
            Console.WriteLine("Disconnected");
            ftClient.GetType().GetProperty(prop).SetValue(ftClient, false);
            DisconnectedEventArgs args = new DisconnectedEventArgs(client, errCode);
            OnDisconnected(args);
        }
    }

    public class QotCallback : FTSPI_Qot
    {
        private FTClient ftClient = null;

        #region EventHandlers
        public event EventHandler<GlobalStateEventArgs> GlobalState;
        public event EventHandler<SubscriptionEventArgs> Subscription;
        public event EventHandler<RegQotPushEventArgs> RegQotPush;
        public event EventHandler<GetSubInfoEventArgs> GetSubInfo;
        public event EventHandler<GetTickerEventArgs> GetTicker;
        public event EventHandler<GetBasicQotEventArgs> GetBasicQot;
        public event EventHandler<GetOrderBookEventArgs> GetOrderBook;
        public event EventHandler<GetKLEventArgs> GetKL;
        public event EventHandler<GetRTEventArgs> GetRT;
        public event EventHandler<GetBrokerEventArgs> GetBroker;
        public event EventHandler<RequestRehabEventArgs> RequestRehab;
        public event EventHandler<RequestHistoryKLQuotaEventArgs> RequestHistoryKLQuota;
        public event EventHandler<RequestHistoryKLEventArgs> RequestHistoryKL;
        public event EventHandler<GetTradeDateEventArgs> GetTradeDate;
        public event EventHandler<GetStaticInfoEventArgs> GetStaticInfo;
        public event EventHandler<GetSecuritySnapshotEventArgs> GetSecuritySnapshot;
        public event EventHandler<GetPlateSetEventArgs> GetPlateSet;
        public event EventHandler<GetPlateSecurityEventArgs> GetPlateSecurity;
        public event EventHandler<GetReferenceEventArgs> GetReference;
        public event EventHandler<GetOwnerPlateEventArgs> GetOwnerPlate;
        public event EventHandler<GetHoldingChangeListEventArgs> GetHoldingChangeList;
        public event EventHandler<GetOptionChainEventArgs> GetOptionChain;
        public event EventHandler<GetWarrantEventArgs> GetWarrant;
        public event EventHandler<GetCapitalFlowEventArgs> GetCapitalFlow;
        public event EventHandler<GetCapitalDistributionEventArgs> GetCapitalDistribution;
        public event EventHandler<GetUserSecurityEventArgs> GetUserSecurity;
        public event EventHandler<ModifyUserSecurityEventArgs> ModifyUserSecurity;
        public event EventHandler<NotifyEventArgs> Notify;
        public event EventHandler<UpdateBasicQotEventArgs> UpdateBasicQot;
        public event EventHandler<UpdateKLEventArgs> UpdateKL;
        public event EventHandler<UpdateRTEventArgs> UpdateRT;
        public event EventHandler<UpdateTickerEventArgs> UpdateTicker;
        public event EventHandler<UpdateOrderBookEventArgs> UpdateOrderBook;
        public event EventHandler<UpdateBrokerEventArgs> UpdateBroker;
        public event EventHandler<UpdateOrderDetailEventArgs> UpdateOrderDetail;
        public event EventHandler<StockFilterEventArgs> StockFilter;
        public event EventHandler<GetCodeChangeEventArgs> GetCodeChange;

        protected virtual void OnGlobalState(GlobalStateEventArgs e)
        {
            GlobalState?.Invoke(this, e);
        }
        protected virtual void OnSubscription(SubscriptionEventArgs e)
        {
            Subscription?.Invoke(this, e);
        }
        protected virtual void OnRegQotPush(RegQotPushEventArgs e)
        {
            RegQotPush?.Invoke(this, e);
        }
        protected virtual void OnGetSubInfo(GetSubInfoEventArgs e)
        {
            GetSubInfo?.Invoke(this, e);
        }
        protected virtual void OnGetTicker(GetTickerEventArgs e)
        {
            GetTicker?.Invoke(this, e);
        }
        protected virtual void OnGetBasicQot(GetBasicQotEventArgs e)
        {
            GetBasicQot?.Invoke(this, e);
        }
        protected virtual void OnGetOrderBook(GetOrderBookEventArgs e)
        {
            GetOrderBook?.Invoke(this, e);
        }
        protected virtual void OnGetKL(GetKLEventArgs e)
        {
            GetKL?.Invoke(this, e);
        }
        protected virtual void OnGetRT(GetRTEventArgs e)
        {
            GetRT?.Invoke(this, e);
        }
        protected virtual void OnGetBroker(GetBrokerEventArgs e)
        {
            GetBroker?.Invoke(this, e);
        }
        protected virtual void OnRequestRehab(RequestRehabEventArgs e)
        {
            RequestRehab?.Invoke(this, e);
        }
        protected virtual void OnRequestHistoricalKLQuota(RequestHistoryKLQuotaEventArgs e)
        {
            RequestHistoryKLQuota?.Invoke(this, e);
        }
        protected virtual void OnRequestHistoricalKL(RequestHistoryKLEventArgs e)
        {
            RequestHistoryKL?.Invoke(this, e);
        }
        protected virtual void OnGetTradeDate(GetTradeDateEventArgs e)
        {
            GetTradeDate?.Invoke(this, e);
        }
        protected virtual void OnGetStaticInfo(GetStaticInfoEventArgs e)
        {
            GetStaticInfo?.Invoke(this, e);
        }
        protected virtual void OnGetSecuritySnapshot(GetSecuritySnapshotEventArgs e)
        {
            GetSecuritySnapshot?.Invoke(this, e);
        }
        protected virtual void OnGetPlateSet(GetPlateSetEventArgs e)
        {
            GetPlateSet?.Invoke(this, e);
        }
        protected virtual void OnGetPlateSecurity(GetPlateSecurityEventArgs e)
        {
            GetPlateSecurity?.Invoke(this, e);
        }
        protected virtual void OnGetReference(GetReferenceEventArgs e)
        {
            GetReference?.Invoke(this, e);
        }
        protected virtual void OnGetOwnerPlate(GetOwnerPlateEventArgs e)
        {
            GetOwnerPlate?.Invoke(this, e);
        }
        protected virtual void OnGetHoldingChangeList(GetHoldingChangeListEventArgs e)
        {
            GetHoldingChangeList?.Invoke(this, e);
        }
        protected virtual void OnGetOptionChain(GetOptionChainEventArgs e)
        {
            GetOptionChain?.Invoke(this, e);
        }
        protected virtual void OnGetWarrant(GetWarrantEventArgs e)
        {
            GetWarrant?.Invoke(this, e);
        }
        protected virtual void OnGetCapitalFlow(GetCapitalFlowEventArgs e)
        {
            GetCapitalFlow?.Invoke(this, e);
        }
        protected virtual void OnGetCapitalDistribution(GetCapitalDistributionEventArgs e)
        {
            GetCapitalDistribution?.Invoke(this, e);
        }
        protected virtual void OnGetUserSecurity(GetUserSecurityEventArgs e)
        {
            GetUserSecurity?.Invoke(this, e);
        }
        protected virtual void OnModifyUserSecurity(ModifyUserSecurityEventArgs e)
        {
            ModifyUserSecurity?.Invoke(this, e);
        }
        protected virtual void OnNotify(NotifyEventArgs e)
        {
            Notify?.Invoke(this, e);
        }
        protected virtual void OnUpdateBasicQot(UpdateBasicQotEventArgs e)
        {
            UpdateBasicQot?.Invoke(this, e);
        }
        protected virtual void OnUpdateKL(UpdateKLEventArgs e)
        {
            UpdateKL?.Invoke(this, e);
        }
        protected virtual void OnUpdateRT(UpdateRTEventArgs e)
        {
            UpdateRT?.Invoke(this, e);
        }
        protected virtual void OnUpdateTicker(UpdateTickerEventArgs e)
        {
            UpdateTicker?.Invoke(this, e);
        }
        protected virtual void OnUpdateOrderBook(UpdateOrderBookEventArgs e)
        {
            UpdateOrderBook?.Invoke(this, e);
        }
        protected virtual void OnUpdateBroker(UpdateBrokerEventArgs e)
        {
            UpdateBroker?.Invoke(this, e);
        }
        protected virtual void OnUpdateOrderDetail(UpdateOrderDetailEventArgs e)
        {
            UpdateOrderDetail?.Invoke(this, e);
        }
        protected virtual void OnStockFilter(StockFilterEventArgs e)
        {
            StockFilter?.Invoke(this, e);
        }
        protected virtual void OnGetCodeChange(GetCodeChangeEventArgs e)
        {
            GetCodeChange?.Invoke(this, e);
        }
        #endregion
        public QotCallback() { }
        public QotCallback(FTClient client)
        {
            ftClient = client;
        }
        public void OnReply_GetGlobalState(FTAPI_Conn client, int nSerialNo, GetGlobalState.Response rsp)
        {
            //Console.WriteLine("Recv GetGlobalState: {0} {1}", nSerialNo, rsp);
            //Console.WriteLine(rsp);
            GlobalStateEventArgs args = new GlobalStateEventArgs(client, nSerialNo, rsp);
            OnGlobalState(args);
        }

        public void OnReply_Sub(FTAPI_Conn client, int nSerialNo, QotSub.Response rsp)
        {
            //Console.WriteLine("{0} Sub receiving reply", nSerialNo);
            SubscriptionEventArgs args = new SubscriptionEventArgs(client, nSerialNo, rsp);
            OnSubscription(args);
        }

        public void OnReply_RegQotPush(FTAPI_Conn client, int nSerialNo, QotRegQotPush.Response rsp)
        {
            //Console.WriteLine(rsp);
            RegQotPushEventArgs args = new RegQotPushEventArgs(client, nSerialNo, rsp);
            OnRegQotPush(args);
        }

        public void OnReply_GetSubInfo(FTAPI_Conn client, int nSerialNo, QotGetSubInfo.Response rsp)
        {
            //Console.WriteLine("{0} GetSubInfo receiving reply.", nSerialNo);
            GetSubInfoEventArgs args = new GetSubInfoEventArgs(client, nSerialNo, rsp);
            OnGetSubInfo(args);
        }

        public void OnReply_GetTicker(FTAPI_Conn client, int nSerialNo, QotGetTicker.Response rsp)
        {
            /*
            int market = rsp.S2C.Security.Market;
            string code = rsp.S2C.Security.Code;
            foreach (var ticker in rsp.S2C.TickerListList)
            {
                long vol = ticker.Volume;
                DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0)).AddHours(8).AddSeconds(ticker.Timestamp);
                double price = ticker.Price;                
            }*/
            //Console.WriteLine(rsp);
            GetTickerEventArgs args = new GetTickerEventArgs(client, nSerialNo, rsp);
            OnGetTicker(args);
        }

        public void OnReply_GetBasicQot(FTAPI_Conn client, int nSerialNo, QotGetBasicQot.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetBasicQotEventArgs args = new GetBasicQotEventArgs(client, nSerialNo, rsp);
            OnGetBasicQot(args);
        }

        public void OnReply_GetOrderBook(FTAPI_Conn client, int nSerialNo, QotGetOrderBook.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetOrderBookEventArgs args = new GetOrderBookEventArgs(client, nSerialNo, rsp);
            OnGetOrderBook(args);
        }

        public void OnReply_GetKL(FTAPI_Conn client, int nSerialNo, QotGetKL.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetKLEventArgs args = new GetKLEventArgs(client, nSerialNo, rsp);
            OnGetKL(args);
        }

        public void OnReply_GetRT(FTAPI_Conn client, int nSerialNo, QotGetRT.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetRTEventArgs args = new GetRTEventArgs(client, nSerialNo, rsp);
            OnGetRT(args);
        }

        public void OnReply_GetBroker(FTAPI_Conn client, int nSerialNo, QotGetBroker.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetBrokerEventArgs args = new GetBrokerEventArgs(client, nSerialNo, rsp);
            OnGetBroker(args);
        }

        public void OnReply_GetHistoryKL(FTAPI_Conn client, int nSerialNo, QotGetHistoryKL.Response rsp)
        {
            /* no longer used */
        }

        public void OnReply_GetHistoryKLPoints(FTAPI_Conn client, int nSerialNo, QotGetHistoryKLPoints.Response rsp)
        {
            /* no longer used */
        }

        public void OnReply_GetRehab(FTAPI_Conn client, int nSerialNo, QotGetRehab.Response rsp)
        {
            /* no longer used */
        }

        public void OnReply_RequestRehab(FTAPI_Conn client, int nSerialNo, QotRequestRehab.Response rsp)
        {
            //Console.WriteLine(rsp);
            RequestRehabEventArgs args = new RequestRehabEventArgs(client, nSerialNo, rsp);
            OnRequestRehab(args);
        }

        public void OnReply_RequestHistoryKL(FTAPI_Conn client, int nSerialNo, QotRequestHistoryKL.Response rsp)
        {
            /*
            foreach (var kl in rsp.S2C.KlListList)
            {
                long vol = kl.Volume;
                DateTime time = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddSeconds(kl.Timestamp);
                double open = kl.OpenPrice;
                double close = kl.ClosePrice;
                double high = kl.HighPrice;
                double low = kl.LowPrice;
            }*/
            //Console.WriteLine("{0} RequestHistoryKL receiving reply.", nSerialNo);
            RequestHistoryKLEventArgs args = new RequestHistoryKLEventArgs(client, nSerialNo, rsp);
            OnRequestHistoricalKL(args);
            if (rsp.RetType == (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("KL count:{0}", rsp.S2C.KlListCount);                
            }
            else
            {
                //Console.WriteLine("Serial [{1}] KL request error:{0}", rsp.RetMsg, nSerialNo);
                //int i = 0;
            }
            
        }

        public void OnReply_RequestHistoryKLQuota(FTAPI_Conn client, int nSerialNo, QotRequestHistoryKLQuota.Response rsp)
        {
            //Console.WriteLine(rsp);
            RequestHistoryKLQuotaEventArgs args = new RequestHistoryKLQuotaEventArgs(client, nSerialNo, rsp);
            OnRequestHistoricalKLQuota(args);
        }

        public void OnReply_GetTradeDate(FTAPI_Conn client, int nSerialNo, QotGetTradeDate.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetTradeDateEventArgs args = new GetTradeDateEventArgs(client, nSerialNo, rsp);
            OnGetTradeDate(args);
        }

        public void OnReply_GetStaticInfo(FTAPI_Conn client, int nSerialNo, QotGetStaticInfo.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetStaticInfoEventArgs args = new GetStaticInfoEventArgs(client, nSerialNo, rsp);
            OnGetStaticInfo(args);
        }

        public void OnReply_GetSecuritySnapshot(FTAPI_Conn client, int nSerialNo, QotGetSecuritySnapshot.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetSecuritySnapshotEventArgs args = new GetSecuritySnapshotEventArgs(client, nSerialNo, rsp);
            OnGetSecuritySnapshot(args);
        }

        public void OnReply_GetPlateSet(FTAPI_Conn client, int nSerialNo, QotGetPlateSet.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetPlateSetEventArgs args = new GetPlateSetEventArgs(client, nSerialNo, rsp);
            OnGetPlateSet(args);
        }

        public void OnReply_GetPlateSecurity(FTAPI_Conn client, int nSerialNo, QotGetPlateSecurity.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetPlateSecurityEventArgs args = new GetPlateSecurityEventArgs(client, nSerialNo, rsp);
            OnGetPlateSecurity(args);
        }

        public void OnReply_GetReference(FTAPI_Conn client, int nSerialNo, QotGetReference.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetReferenceEventArgs args = new GetReferenceEventArgs(client, nSerialNo, rsp);
            OnGetReference(args);
        }

        public void OnReply_GetOwnerPlate(FTAPI_Conn client, int nSerialNo, QotGetOwnerPlate.Response rsp)
        {
            //Console.WriteLine(rsp);
            GetOwnerPlateEventArgs args = new GetOwnerPlateEventArgs(client, nSerialNo, rsp);
            OnGetOwnerPlate(args);
        }

        public void OnReply_GetHoldingChangeList(FTAPI_Conn client, int nSerialNo, QotGetHoldingChangeList.Response rsp)
        {
            GetHoldingChangeListEventArgs args = new GetHoldingChangeListEventArgs(client, nSerialNo, rsp);
            OnGetHoldingChangeList(args);
        }

        public void OnReply_GetOptionChain(FTAPI_Conn client, int nSerialNo, QotGetOptionChain.Response rsp)
        {
            GetOptionChainEventArgs args = new GetOptionChainEventArgs(client, nSerialNo, rsp);
            OnGetOptionChain(args);
        }

        public void OnReply_GetWarrant(FTAPI_Conn client, int nSerialNo, QotGetWarrant.Response rsp)
        {
            GetWarrantEventArgs args = new GetWarrantEventArgs(client, nSerialNo, rsp);
            OnGetWarrant(args);
        }

        public void OnReply_GetCapitalFlow(FTAPI_Conn client, int nSerialNo, QotGetCapitalFlow.Response rsp)
        {
            GetCapitalFlowEventArgs args = new GetCapitalFlowEventArgs(client, nSerialNo, rsp);
            OnGetCapitalFlow(args);
        }

        public void OnReply_GetCapitalDistribution(FTAPI_Conn client, int nSerialNo, QotGetCapitalDistribution.Response rsp)
        {
            GetCapitalDistributionEventArgs args = new GetCapitalDistributionEventArgs(client, nSerialNo, rsp);
            OnGetCapitalDistribution(args);
        }

        public void OnReply_GetUserSecurity(FTAPI_Conn client, int nSerialNo, QotGetUserSecurity.Response rsp)
        {
            GetUserSecurityEventArgs args = new GetUserSecurityEventArgs(client, nSerialNo, rsp);
            OnGetUserSecurity(args);
        }

        public void OnReply_ModifyUserSecurity(FTAPI_Conn client, int nSerialNo, QotModifyUserSecurity.Response rsp)
        {
            ModifyUserSecurityEventArgs args = new ModifyUserSecurityEventArgs(client, nSerialNo, rsp);
            OnModifyUserSecurity(args);
        }

        public void OnReply_Notify(FTAPI_Conn client, int nSerialNo, Notify.Response rsp)
        {
            Console.WriteLine(rsp);
            NotifyEventArgs args = new NotifyEventArgs(client, nSerialNo, rsp);
            OnNotify(args);
        }

        public void OnReply_UpdateBasicQot(FTAPI_Conn client, int nSerialNo, QotUpdateBasicQot.Response rsp)
        {
            UpdateBasicQotEventArgs args = new UpdateBasicQotEventArgs(client, nSerialNo, rsp);
            OnUpdateBasicQot(args);
        }

        public void OnReply_UpdateKL(FTAPI_Conn client, int nSerialNo, QotUpdateKL.Response rsp)
        {
            UpdateKLEventArgs args = new UpdateKLEventArgs(client, nSerialNo, rsp);
            OnUpdateKL(args);
        }

        public void OnReply_UpdateRT(FTAPI_Conn client, int nSerialNo, QotUpdateRT.Response rsp)
        {
            UpdateRTEventArgs args = new UpdateRTEventArgs(client, nSerialNo, rsp);
            OnUpdateRT(args);
        }

        public void OnReply_UpdateTicker(FTAPI_Conn client, int nSerialNo, QotUpdateTicker.Response rsp)
        {
            //Console.WriteLine("Recv OnReply_UpdateTicker: {0}", nSerialNo);
            UpdateTickerEventArgs args = new UpdateTickerEventArgs(client, nSerialNo, rsp);
            OnUpdateTicker(args);
        }

        public void OnReply_UpdateOrderBook(FTAPI_Conn client, int nSerialNo, QotUpdateOrderBook.Response rsp)
        {
            UpdateOrderBookEventArgs args = new UpdateOrderBookEventArgs(client, nSerialNo, rsp);
            OnUpdateOrderBook(args);
        }

        public void OnReply_UpdateBroker(FTAPI_Conn client, int nSerialNo, QotUpdateBroker.Response rsp)
        {
            UpdateBrokerEventArgs args = new UpdateBrokerEventArgs(client, nSerialNo, rsp);
            OnUpdateBroker(args);
        }

        public void OnReply_UpdateOrderDetail(FTAPI_Conn client, int nSerialNo, QotUpdateOrderDetail.Response rsp)
        {
            UpdateOrderDetailEventArgs args = new UpdateOrderDetailEventArgs(client, nSerialNo, rsp);
            OnUpdateOrderDetail(args);
        }

        public void OnReply_StockFilter(FTAPI_Conn client, int nSerialNo, QotStockFilter.Response rsp)
        {

        }

        public void OnReply_GetCodeChange(FTAPI_Conn client, int nSerialNo, QotGetCodeChange.Response rsp)
        {

        }
    }

    public class TrdCallback : FTSPI_Trd
    {
        #region EnventHandlers

        public event EventHandler<GetAccListEventArgs> GetAccList;
        public event EventHandler<UnlockTradeEventArgs> UnlockTrade;
        public event EventHandler<SubAccPushEventArgs> SubAccPush;
        public event EventHandler<GetFundsEventArgs> GetFunds;
        public event EventHandler<GetPositionListEventArgs> GetPositionList;
        public event EventHandler<GetMaxTrdQtysEventArgs> GetMaxTrdQtys;
        public event EventHandler<GetOrderListEventArgs> GetOrderList;
        public event EventHandler<GetOrderFillListEventArgs> GetOrderFillList;
        public event EventHandler<GetHistoryOrderListEventArgs> GetHistoryOrderList;
        public event EventHandler<GetHistoryOrderFillListEventArgs> GetHistoryOrderFillList;
        public event EventHandler<UpdateOrderEventArgs> UpdateOrder;
        public event EventHandler<UpdateOrderFillEventArgs> UpdateOrderFill;
        public event EventHandler<PlaceOrderEventArgs> PlaceOrder;
        public event EventHandler<ModifyOrderEventArgs> ModifyOrder;
        
        protected virtual void OnGetAccList(GetAccListEventArgs e)
        {
            GetAccList?.Invoke(this, e);
        }
        protected virtual void OnUnlockTrade(UnlockTradeEventArgs e)
        {
            UnlockTrade?.Invoke(this, e);
        }
        protected virtual void OnSubAccPush(SubAccPushEventArgs e)
        {
            SubAccPush?.Invoke(this, e);
        }
        protected virtual void OnGetFunds(GetFundsEventArgs e)
        {
            GetFunds?.Invoke(this, e);
        }
        protected virtual void OnGetPositionList(GetPositionListEventArgs e)
        {
            GetPositionList?.Invoke(this, e);
        }
        protected virtual void OnGetMaxTrdQtys(GetMaxTrdQtysEventArgs e)
        {
            GetMaxTrdQtys?.Invoke(this, e);
        }
        protected virtual void OnGetOrderList(GetOrderListEventArgs e)
        {
            GetOrderList?.Invoke(this, e);
        }
        protected virtual void OnGetOrderFillList(GetOrderFillListEventArgs e)
        {
            GetOrderFillList?.Invoke(this, e);
        }
        protected virtual void OnGetHistoryOrderList(GetHistoryOrderListEventArgs e)
        {
            GetHistoryOrderList?.Invoke(this, e);
        }
        protected virtual void OnGetHistoryOrderFillList(GetHistoryOrderFillListEventArgs e)
        {
            GetHistoryOrderFillList?.Invoke(this, e);
        }
        protected virtual void OnUpdateOrder(UpdateOrderEventArgs e)
        {
            UpdateOrder?.Invoke(this, e);
        }
        protected virtual void OnUpdateOrderFill(UpdateOrderFillEventArgs e)
        {
            UpdateOrderFill?.Invoke(this, e);
        }
        protected virtual void OnPlaceOrder(PlaceOrderEventArgs e)
        {
            PlaceOrder?.Invoke(this, e);
        }
        protected virtual void OnModifyOrder(ModifyOrderEventArgs e)
        {
            ModifyOrder?.Invoke(this, e);
        }

        #endregion

        private FTClient ftClient = null;
        private string pwdMD5 = string.Empty;

        public TrdCallback(FTClient client)
        {
            ftClient = client;
        }

        public void OnReply_GetAccList(FTAPI_Conn client, int nSerialNo, TrdGetAccList.Response rsp)
        {            
            GetAccListEventArgs args = new GetAccListEventArgs(client, nSerialNo, rsp);
            OnGetAccList(args);
        }

        public void OnReply_UnlockTrade(FTAPI_Conn client, int nSerialNo, TrdUnlockTrade.Response rsp)
        {
            /*
            Console.WriteLine("Recv UnlockTrade: {0} {1}", nSerialNo, rsp);
            if (rsp.RetType != (int)Common.RetType.RetType_Succeed)
            {
                Console.WriteLine("error code is {0}", rsp.RetMsg);
            }
            else
            {
                FTAPI_Trd trd = client as FTAPI_Trd;

                TrdPlaceOrder.Request.Builder req = TrdPlaceOrder.Request.CreateBuilder();
                TrdPlaceOrder.C2S.Builder cs = TrdPlaceOrder.C2S.CreateBuilder();
                Common.PacketID.Builder packetID = Common.PacketID.CreateBuilder().SetConnID(trd.GetConnectID()).SetSerialNo(0);
                TrdCommon.TrdHeader.Builder trdHeader = TrdCommon.TrdHeader.CreateBuilder().SetAccID(this.accID).SetTrdEnv((int)TrdCommon.TrdEnv.TrdEnv_Real).SetTrdMarket((int)TrdCommon.TrdMarket.TrdMarket_HK);
                cs.SetPacketID(packetID).SetHeader(trdHeader).SetTrdSide((int)TrdCommon.TrdSide.TrdSide_Sell).SetOrderType((int)TrdCommon.OrderType.OrderType_AbsoluteLimit).SetCode("01810").SetQty(100.00).SetPrice(10.2).SetAdjustPrice(true);
                req.SetC2S(cs);

                uint serialNo = trd.PlaceOrder(req.Build());
                Console.WriteLine("Send PlaceOrder: {0}, {1}", serialNo, req);
            }*/
            UnlockTradeEventArgs args = new UnlockTradeEventArgs(client, nSerialNo, rsp);
            OnUnlockTrade(args);
        }

        public void OnReply_SubAccPush(FTAPI_Conn client, int nSerialNo, TrdSubAccPush.Response rsp)
        {
            SubAccPushEventArgs args = new SubAccPushEventArgs(client, nSerialNo, rsp);
            OnSubAccPush(args);
        }

        public void OnReply_GetFunds(FTAPI_Conn client, int nSerialNo, TrdGetFunds.Response rsp)
        {
            GetFundsEventArgs args = new GetFundsEventArgs(client, nSerialNo, rsp);
            OnGetFunds(args);
        }

        public void OnReply_GetPositionList(FTAPI_Conn client, int nSerialNo, TrdGetPositionList.Response rsp)
        {
            GetPositionListEventArgs args = new GetPositionListEventArgs(client, nSerialNo, rsp);
            OnGetPositionList(args);
        }

        public void OnReply_GetMaxTrdQtys(FTAPI_Conn client, int nSerialNo, TrdGetMaxTrdQtys.Response rsp)
        {
            GetMaxTrdQtysEventArgs args = new GetMaxTrdQtysEventArgs(client, nSerialNo, rsp);
            OnGetMaxTrdQtys(args);
        }

        public void OnReply_GetOrderList(FTAPI_Conn client, int nSerialNo, TrdGetOrderList.Response rsp)
        {
            GetOrderListEventArgs args = new GetOrderListEventArgs(client, nSerialNo, rsp);
            OnGetOrderList(args);
        }

        public void OnReply_GetOrderFillList(FTAPI_Conn client, int nSerialNo, TrdGetOrderFillList.Response rsp)
        {
            GetOrderFillListEventArgs args = new GetOrderFillListEventArgs(client, nSerialNo, rsp);
            OnGetOrderFillList(args);
        }

        public void OnReply_GetHistoryOrderList(FTAPI_Conn client, int nSerialNo, TrdGetHistoryOrderList.Response rsp)
        {
            GetHistoryOrderListEventArgs args = new GetHistoryOrderListEventArgs(client, nSerialNo, rsp);
            OnGetHistoryOrderList(args);
        }

        public void OnReply_GetHistoryOrderFillList(FTAPI_Conn client, int nSerialNo, TrdGetHistoryOrderFillList.Response rsp)
        {
            GetHistoryOrderFillListEventArgs args = new GetHistoryOrderFillListEventArgs(client, nSerialNo, rsp);
            OnGetHistoryOrderFillList(args);
        }

        public void OnReply_UpdateOrder(FTAPI_Conn client, int nSerialNo, TrdUpdateOrder.Response rsp)
        {
            // Console.WriteLine("Recv UpdateOrder: {0} {1}", nSerialNo, rsp);
            UpdateOrderEventArgs args = new UpdateOrderEventArgs(client, nSerialNo, rsp);
            OnUpdateOrder(args);
        }

        public void OnReply_UpdateOrderFill(FTAPI_Conn client, int nSerialNo, TrdUpdateOrderFill.Response rsp)
        {
            //Console.WriteLine("Recv UpdateOrderFill: {0} {1}", nSerialNo, rsp);
            UpdateOrderFillEventArgs args = new UpdateOrderFillEventArgs(client, nSerialNo, rsp);
            OnUpdateOrderFill(args);
        }

        public void OnReply_PlaceOrder(FTAPI_Conn client, int nSerialNo, TrdPlaceOrder.Response rsp)
        {
            /*
            Console.WriteLine("Recv PlaceOrder: {0} {1}", nSerialNo, rsp);
            if (rsp.RetType != (int)Common.RetType.RetType_Succeed)
            {
                Console.WriteLine("error code is {0}", rsp.RetMsg);
            }*/
            PlaceOrderEventArgs args = new PlaceOrderEventArgs(client, nSerialNo, rsp);
            OnPlaceOrder(args);
        }

        public void OnReply_ModifyOrder(FTAPI_Conn client, int nSerialNo, TrdModifyOrder.Response rsp)
        {
            ModifyOrderEventArgs args = new ModifyOrderEventArgs(client, nSerialNo, rsp);
            OnModifyOrder(args);
        }
    }

    public class FTClient
    {
        #region Global EventHandlers
        public event EventHandler<ErrorEventArgs> OnError;
        #endregion
        public string Host { get; private set; } = "localhost";
        public ushort Port { get; private set; } = 0;
        public bool IsEnableEncrypt { get; private set; } = false;
        public ulong QotConnectID { get; private set; }
        public ulong TrdConnectID { get; private set; }
        public int ConnectStatus { get; private set; }
        //private RequestWithLimition requestWithLimition;
        private CRequestHistoryKL requestHistoryKL;

        internal FTAPI_Qot Qot { get; private set; }
        internal FTAPI_Trd Trd { get; private set; }

        public bool IsQotConnected { get; internal set; }
        public bool IsTrdConnected { get; internal set; }
        public bool IsLocked { get; internal set; } = true; // lock for trade
        public int ServerVersion { get; internal set; }
        public QotCallback QotCallback { private set; get; }
        public ConnCallback QotConnCallback { private set; get; }
        public ConnCallback TrdConnCallback { private set; get; }
        public TrdCallback TrdCallback { private set; get; }
        public IList<TrdAcc> TrdAccs { get; private set; }

        public FTClient(string host, ushort port, bool isEnableEncrypt = false, string info = "FTClient")
        {
            this.Host = host;
            this.Port = port;
            this.IsEnableEncrypt = isEnableEncrypt;
            
                        
            FTAPI.Init();
            Qot = new FTAPI_Qot();            

            QotConnCallback = new ConnCallback(this, "IsQotConnected");
            QotCallback = new QotCallback();
            Qot.SetConnCallback(QotConnCallback);            
            Qot.SetQotCallback(QotCallback);
            Qot.SetClientInfo(info, 1);

            Trd = new FTAPI_Trd();
            TrdConnCallback = new ConnCallback(this, "IsTrdConnected");
            TrdCallback = new TrdCallback(this);
            Trd.SetConnCallback(TrdConnCallback);
            Trd.SetTrdCallback(TrdCallback);
            TrdCallback.UnlockTrade += TrdCallback_UnlockTrade; ;
            Trd.SetClientInfo(info, 1);

            QotCallback.Notify += QotCallback_Notify;
            QotCallback.Subscription += QotCallback_Subscription;

            QotConnCallback.InitConnected += QotConnCallback_InitConnected;
            TrdConnCallback.InitConnected += TrdConnCallback_InitConnected;
            TrdCallback.GetAccList += TrdCallback_GetAccList;
            TrdCallback.SubAccPush += TrdCallback_SubAccPush;
        }

        private void TrdCallback_SubAccPush(object sender, SubAccPushEventArgs e)
        {
            if (e.Result.RetType != (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("error code is {0}", e.Result.RetMsg);
                ErrorEventArgs arg = new ErrorEventArgs(e.Client, e.SerialNo, e.Result.ErrCode, e.Result.RetType, e.Result.RetMsg, e.Result.S2C);
                OnError?.Invoke(this, arg);
            }
        }

        private void TrdCallback_GetAccList(object sender, GetAccListEventArgs e)
        {
            if (e.Result.RetType != (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("error code is {0}", e.Result.RetMsg);
                ErrorEventArgs arg = new ErrorEventArgs(e.Client, e.SerialNo, e.Result.ErrCode, e.Result.RetType, e.Result.RetMsg, e.Result.S2C);
                OnError?.Invoke(this, arg);
            }
            else
            {
                this.TrdAccs = e.Result.S2C.AccListList;
            }
        }

        private void TrdConnCallback_InitConnected(object sender, InitConnectedEventArgs e)
        {
            TrdConnectID = e.Client.GetConnectID();
            GetAccList();
        }

        private void QotConnCallback_InitConnected(object sender, InitConnectedEventArgs e)
        {
            QotConnectID = e.Client.GetConnectID();            
        }

        private void QotCallback_Subscription(object sender, SubscriptionEventArgs e)
        {
            if (e.Result.RetType != (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("error code is {0}", e.Result.RetMsg);
                ErrorEventArgs arg = new ErrorEventArgs(e.Client, e.SerialNo, e.Result.ErrCode, e.Result.RetType, e.Result.RetMsg, e.Result.S2C);
                OnError?.Invoke(this, arg);
            }
        }

        private void QotCallback_Notify(object sender, NotifyEventArgs e)
        {
            if (e.Result.RetType != (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("error code is {0}", e.Result.RetMsg);
                ErrorEventArgs arg = new ErrorEventArgs(e.Client, e.SerialNo, e.Result.ErrCode, e.Result.RetType, e.Result.RetMsg, e.Result.S2C);
                OnError?.Invoke(this, arg);
            }
        }

        private void TrdCallback_UnlockTrade(object sender, UnlockTradeEventArgs e)
        {
            if (e.Result.RetType != (int)Common.RetType.RetType_Succeed)
            {
                //Console.WriteLine("error code is {0}", e.Result.RetMsg);
                
                ErrorEventArgs arg = new ErrorEventArgs(e.Client, e.SerialNo, e.Result.ErrCode, e.Result.RetType, e.Result.RetMsg, e.Result.S2C);
                OnError?.Invoke(this, arg);
                
                IsLocked = true;
            }
            else
                IsLocked = false;
        }

        internal void Dispose()
        {
            Qot.Close();
            Trd.Close();
        }

        public uint GetAccList()
        {
            TrdGetAccList.Request req = TrdGetAccList.Request.CreateBuilder().SetC2S(TrdGetAccList.C2S.CreateBuilder().SetUserID(0)).Build();
            uint serialNo = Trd.GetAccList(req);
            return serialNo;
        }
        
        public uint UnlockTrade(string pwd = null, string pwdMD5 = null)
        {
            if (!IsTrdConnected)
            {
                ErrorEventArgs args = new ErrorEventArgs(null, 0, 0, 0, "Unlock failed, TrdCtx has been connected", null);
                OnError?.Invoke(this, args);
                return 9999999;
            }
            string unlockPwdMd5 = null;
            if (string.IsNullOrEmpty(pwdMD5))
            {
                MD5 md5 = MD5.Create();
                byte[] encryptionBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(pwd));
                unlockPwdMd5 = BitConverter.ToString(encryptionBytes).Replace("-", "").ToLower();
            }
            else
                unlockPwdMd5 = pwdMD5;
            TrdUnlockTrade.Request req = null;
            if (IsLocked)
                req = TrdUnlockTrade.Request.CreateBuilder().SetC2S(TrdUnlockTrade.C2S.CreateBuilder().SetUnlock(true).SetPwdMD5(unlockPwdMd5)).Build();
            else
            {
                req = TrdUnlockTrade.Request.CreateBuilder().SetC2S(TrdUnlockTrade.C2S.CreateBuilder().SetUnlock(false)).Build();
                //IsLocked = true;
            }                

            uint serialNo = Trd.UnlockTrade(req);
            return serialNo;
        }

        public void Connect()
        {
            Qot.InitConnect(Host, Port, IsEnableEncrypt);
            Trd.InitConnect(Host, Port, IsEnableEncrypt);

            Limitation lmt = new Limitation { freq1 = (10, 30) };
            requestHistoryKL = new CRequestHistoryKL(this, lmt, RequestHistoryKL, RequestHistoryKLQuota<int>);            
        }

        public async Task<(bool IsQotConnected, bool IsTrdConnected)> ConnectAsync()
        {
            try
            {
                bool isQotConnected = await QotConnectAsync();
                bool isTrdConnected = await TrdConnectAsync();
                return (isQotConnected, isTrdConnected);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal async Task<bool> QotConnectAsync(int? timeout = null)
        {
            try
            {
                bool result = await FTTaskExt.FromEvent<InitConnectedEventArgs, bool>(
                    handler => QotConnCallback.InitConnected += new EventHandler<InitConnectedEventArgs>(handler),
                    () => Qot.InitConnect(Host, Port, IsEnableEncrypt),
                    handler => QotConnCallback.InitConnected -= new EventHandler<InitConnectedEventArgs>(handler),
                    CancellationToken.None, this, timeout);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal async Task<bool> TrdConnectAsync(int? timeout = null)
        {
            try
            {
                bool result = await FTTaskExt.FromEvent<InitConnectedEventArgs, bool>(
                    handler => TrdConnCallback.InitConnected += new EventHandler<InitConnectedEventArgs>(handler),
                    () => { return Trd.InitConnect(Host, Port, IsEnableEncrypt); },
                    handler => TrdConnCallback.InitConnected -= new EventHandler<InitConnectedEventArgs>(handler),
                    CancellationToken.None, this, timeout);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Disconnect()
        {
            Qot.Close();
            Trd.Close();
        }

        public async Task<List<QotCommon.KLine>> GetKL(Security security, 
                                    QotCommon.KLType klType = QotCommon.KLType.KLType_1Min, 
                                    QotCommon.RehabType rehabType = QotCommon.RehabType.RehabType_Forward,                                    
                                    int reqNum = 1000, bool unSub = false, int retry_lmt = 5, int retry_interval = 5)
        {
            try
            {
                await Sub(security, FTUtil.KLTypeToSubType(klType), retry_lmt, retry_interval);

                // get kl
                QotGetKL.Request.Builder klBuilder = QotGetKL.Request.CreateBuilder();
                QotGetKL.C2S.Builder csKLBuilder = QotGetKL.C2S.CreateBuilder();
                QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
                stock.SetCode(security.Code);
                stock.SetMarket((int)security.Market);
                csKLBuilder.SetSecurity(stock);
                csKLBuilder.SetRehabType((int)rehabType);
                csKLBuilder.SetReqNum(reqNum);
                csKLBuilder.SetKlType((int)klType);
                klBuilder.SetC2S(csKLBuilder);
                

                List<QotCommon.KLine> kLines = await FTTaskExt.FromEvent<GetKLEventArgs, List<QotCommon.KLine>>(
                    handler => {
                        QotCallback.GetKL += new EventHandler<GetKLEventArgs>(handler);
                    },
                    () => {
                        uint ret = Qot.GetKL(klBuilder.Build());
                        return ret;
                    },
                    handler => {
                        QotCallback.GetKL -= new EventHandler<GetKLEventArgs>(handler);
                    },
                    CancellationToken.None, null, null, null,
                    () =>
                    {
                        // unsubscribe quote
                        if (unSub)
                            Task.Delay(61 * 1000).ContinueWith(result => Unsub(security, FTUtil.KLTypeToSubType(klType)));
                    });
                return kLines;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public uint SubAccPush()
        {
            uint serialNo = 0;
            if (TrdAccs != null && TrdAccs.Count > 0)
            {
                TrdSubAccPush.Request.Builder subAccPushBuilder = TrdSubAccPush.Request.CreateBuilder();
                TrdSubAccPush.C2S.Builder csSubBuilder = TrdSubAccPush.C2S.CreateBuilder();
                foreach (var acc in TrdAccs)
                {
                    csSubBuilder.AddAccIDList(acc.AccID);
                }
                subAccPushBuilder.SetC2S(csSubBuilder);
                serialNo = Trd.SubAccPush(subAccPushBuilder.Build());
            }
            return serialNo;
        }

        /*
         * Get contract details including price spread 
         */
        public async Task<Contract> RequestContractDetail(Security security, int retry_lmt = 5, int retry_interval = 5)
        {
            try
            {
                await Sub(security, QotCommon.SubType.SubType_Basic, retry_lmt, retry_interval);                    

                // get static info
                QotGetStaticInfo.Request.Builder infoBuilder = QotGetStaticInfo.Request.CreateBuilder();
                QotGetStaticInfo.C2S.Builder csInfoBuilder = QotGetStaticInfo.C2S.CreateBuilder();
                QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
                stock.SetCode(security.Code);
                stock.SetMarket((int)security.Market);
                csInfoBuilder.AddSecurityList(stock);
                infoBuilder.SetC2S(csInfoBuilder);
                // get basic qot
                QotGetBasicQot.Request.Builder basicQotBuilder = QotGetBasicQot.Request.CreateBuilder();
                QotGetBasicQot.C2S.Builder csBasicQotBuilder = QotGetBasicQot.C2S.CreateBuilder();
                csBasicQotBuilder.AddSecurityList(stock);
                basicQotBuilder.SetC2S(csBasicQotBuilder);

                Contract contract = await FTTaskExt.FromEvent<EventArgs, Contract>(
                    handler => {
                        QotCallback.GetBasicQot += new EventHandler<GetBasicQotEventArgs>(handler);
                        QotCallback.GetStaticInfo += new EventHandler<GetStaticInfoEventArgs>(handler);
                        },
                    () => {                       
                        uint ret_si = Qot.GetStaticInfo(infoBuilder.Build());
                        uint ret_bq = Qot.GetBasicQot(basicQotBuilder.Build());
                        return (ret_si, ret_bq);
                    },
                    handler => {
                        QotCallback.GetBasicQot -= new EventHandler<GetBasicQotEventArgs>(handler);
                        QotCallback.GetStaticInfo -= new EventHandler<GetStaticInfoEventArgs>(handler);
                    },
                    CancellationToken.None, security, null, null,
                    () =>
                    {
                        // unsubscribe quote
                        Task.Delay(61 * 1000).ContinueWith(result => Unsub(security, QotCommon.SubType.SubType_Basic));
                    });
                return contract;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Subscription
        /// </summary>
        /// <param name="security"></param>
        /// <param name="subType">Subscription Type</param>
        /// <param name="retry_lmt">Retry times</param>
        /// <param name="retry_interval">Retry interval</param>
        /// <returns></returns>
        public async Task<bool> Sub(Security security, QotCommon.SubType subType, int retry_lmt = 5, int retry_interval = 5)
        {
            // security
            QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
            stock.SetCode(security.Code);
            stock.SetMarket((int)security.Market);

            // subscription
            QotSub.Request.Builder subBuilder = QotSub.Request.CreateBuilder();
            QotSub.C2S.Builder csSubBuilder = QotSub.C2S.CreateBuilder();
            csSubBuilder.AddSubTypeList((int)subType);
            csSubBuilder.AddSecurityList(stock);
            csSubBuilder.SetIsSubOrUnSub(true);
            csSubBuilder.SetIsRegOrUnRegPush(true);
            subBuilder.SetC2S(csSubBuilder);

            SubInfo subInfo = await GetSubInfo();
            if (subInfo.RemainQuota == 0)
            {
                for (int i = 0; i < retry_lmt; i++)
                {
                    await Task.Delay(1000 * retry_interval);
                    subInfo = await GetSubInfo();
                    if (subInfo.RemainQuota > 0)
                        break;
                }
                if (subInfo.RemainQuota == 0)
                    throw new OutOfQuotaException();
            }
            Qot.Sub(subBuilder.Build());
            return true;
        }

        /// <summary>
        /// Get global state
        /// </summary>
        public uint GetGlobalState()
        {
            GetGlobalState.Request req = Futu.OpenApi.Pb.GetGlobalState.Request.CreateBuilder().SetC2S(Futu.OpenApi.Pb.GetGlobalState.C2S.CreateBuilder().SetUserID(900019)).Build();
            uint serialNo = Qot.GetGlobalState(req);
            return serialNo;
        }

        /// <summary>
        /// Unsubscription
        /// </summary>
        /// <param name="security"></param>
        /// <param name="subType"></param>
        public void Unsub(Security security, QotCommon.SubType subType)
        {
            // security
            QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
            stock.SetCode(security.Code);
            stock.SetMarket((int)security.Market);

            // subscription
            QotSub.Request.Builder subBuilder = QotSub.Request.CreateBuilder();
            QotSub.C2S.Builder csSubBilder = QotSub.C2S.CreateBuilder();
            csSubBilder.AddSubTypeList((int)subType);
            csSubBilder.AddSecurityList(stock);
            csSubBilder.SetIsSubOrUnSub(false);
            subBuilder.SetC2S(csSubBilder);
            Qot.Sub(subBuilder.Build());
        }

        /// <summary>
        /// Get contract static info 
        /// </summary>
        /// <param name="security"></param>
        /// <returns></returns>
        public async Task<Contract> RequestContractInfo(Security security)
        {
            try
            {
                // get static info
                QotGetStaticInfo.Request.Builder infoBuilder = QotGetStaticInfo.Request.CreateBuilder();
                QotGetStaticInfo.C2S.Builder csInfoBuilder = QotGetStaticInfo.C2S.CreateBuilder();
                QotCommon.Security.Builder stock = QotCommon.Security.CreateBuilder();
                stock.SetCode(security.Code);
                stock.SetMarket((int)security.Market);
                csInfoBuilder.AddSecurityList(stock);
                infoBuilder.SetC2S(csInfoBuilder);                

                Contract contract = await FTTaskExt.FromEvent<GetStaticInfoEventArgs, Contract>(
                    handler => {
                        QotCallback.GetStaticInfo += new EventHandler<GetStaticInfoEventArgs>(handler);
                    },
                    () => {
                        return Qot.GetStaticInfo(infoBuilder.Build());
                    },
                    handler => {
                        QotCallback.GetStaticInfo -= new EventHandler<GetStaticInfoEventArgs>(handler);
                    },
                    CancellationToken.None, security, null, null, null);
                return contract;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<Contract>> RequestSymbols(Security security)
        {
            try
            {
                QotGetPlateSecurity.Request.Builder secReqBuilder = QotGetPlateSecurity.Request.CreateBuilder();
                QotGetPlateSecurity.C2S.Builder csSecBuilder = QotGetPlateSecurity.C2S.CreateBuilder();
                QotCommon.Security.Builder sec = QotCommon.Security.CreateBuilder();
                sec.SetMarket((int)security.Market);
                sec.SetCode(security.Code);
                csSecBuilder.SetPlate(sec.Build());
                secReqBuilder.SetC2S(csSecBuilder);

                List<Contract> contracts = await FTTaskExt.FromEvent<GetPlateSecurityEventArgs, List<Contract>>(
                    handler => {
                        QotCallback.GetPlateSecurity += new EventHandler<GetPlateSecurityEventArgs>(handler);
                    },
                    () => {
                        return Qot.GetPlateSecurity(secReqBuilder.Build());
                    },
                    handler => {
                        QotCallback.GetPlateSecurity -= new EventHandler<GetPlateSecurityEventArgs>(handler);
                    },
                    CancellationToken.None, security.Market, null, null, null);
                return contracts;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<PlateInfo>> RequestPlateSet(QotCommon.QotMarket market, QotCommon.PlateSetType plateSetType = QotCommon.PlateSetType.PlateSetType_All)
        {
            try
            {
                QotGetPlateSet.Request.Builder setReqBuilder = QotGetPlateSet.Request.CreateBuilder();
                QotGetPlateSet.C2S.Builder csSecBuilder = QotGetPlateSet.C2S.CreateBuilder();
                csSecBuilder.SetMarket((int)market);
                csSecBuilder.SetPlateSetType((int)plateSetType);
                setReqBuilder.SetC2S(csSecBuilder);

                List<PlateInfo> plates = await FTTaskExt.FromEvent<GetPlateSetEventArgs, List<PlateInfo>>(
                    handler => {
                        QotCallback.GetPlateSet += new EventHandler<GetPlateSetEventArgs>(handler);
                    },
                    () => {
                        return Qot.GetPlateSet(setReqBuilder.Build());
                    },
                    handler => {
                        QotCallback.GetPlateSet -= new EventHandler<GetPlateSetEventArgs>(handler);
                    },
                    CancellationToken.None, (market, plateSetType), null, null, null);
                return plates;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<SubInfo> GetSubInfo()
        {
            try
            {
                // get static info
                QotGetSubInfo.Request.Builder infoBuilder = QotGetSubInfo.Request.CreateBuilder();
                QotGetSubInfo.C2S.Builder csInfoBuilder = QotGetSubInfo.C2S.CreateBuilder();
                infoBuilder.SetC2S(csInfoBuilder);
                
                SubInfo subInfo = await FTTaskExt.FromEvent<GetSubInfoEventArgs, SubInfo>(
                    handler => {
                        QotCallback.GetSubInfo += new EventHandler<GetSubInfoEventArgs>(handler);
                    },
                    () => {
                        uint ret = Qot.GetSubInfo(infoBuilder.Build());
                        //Console.WriteLine("{0} getting sub info", ret);
                        return ret;
                    },
                    handler => {
                        QotCallback.GetSubInfo -= new EventHandler<GetSubInfoEventArgs>(handler);
                    },
                    CancellationToken.None, null, null, null, null);
                return subInfo;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        internal void RequestCurrentTime()
        {
            //eClientSocket.reqCurrentTime();
            
        }

        public async Task<T> RequestHistoryKLQuota<T>()
        {
            QotRequestHistoryKLQuota.Request.Builder reqBuilder = QotRequestHistoryKLQuota.Request.CreateBuilder();
            QotRequestHistoryKLQuota.C2S.Builder csBuilder = QotRequestHistoryKLQuota.C2S.CreateBuilder();
            csBuilder.SetBGetDetail(true);
            reqBuilder.SetC2S(csBuilder);
            //Qot.RequestHistoryKLQuota(reqBuilder.Build());

            HistoryKLQuota quota = await FTTaskExt.FromEvent<RequestHistoryKLQuotaEventArgs, HistoryKLQuota>(
                    handler => {
                        QotCallback.RequestHistoryKLQuota += new EventHandler<RequestHistoryKLQuotaEventArgs>(handler);
                    },
                    () => {
                        return Qot.RequestHistoryKLQuota(reqBuilder.Build());
                    },
                    handler => {
                        QotCallback.RequestHistoryKLQuota -= new EventHandler<RequestHistoryKLQuotaEventArgs>(handler);
                    },
                    CancellationToken.None, null, null, null, null);

            if (typeof(T) == typeof(int))
                return (T)Convert.ChangeType(quota.RemainQuota, typeof(T));
            else if (typeof(T) == typeof(HistoryKLQuota))
                return (T)Convert.ChangeType(quota, typeof(T));
            else
                return (T)Convert.ChangeType(null, typeof(T));
        }

        internal async Task<bool> RequestHistoryKL(QotRequestHistoryKL.Request request)
        {
            try
            {
                bool succeed = await FTTaskExt.FromEvent<RequestHistoryKLEventArgs, bool>(
                    handler => {
                        QotCallback.RequestHistoryKL += new EventHandler<RequestHistoryKLEventArgs>(handler);
                    },
                    () => {
                        return Qot.RequestHistoryKL(request);
                    },
                    handler => {
                        QotCallback.RequestHistoryKL -= new EventHandler<RequestHistoryKLEventArgs>(handler);
                    },
                    CancellationToken.None, null, null, null, null);
                return succeed;
            }
            catch (Exception e)
            {
                //Console.WriteLine("{0} request KL failed (ex).", request.C2S.Security.Code);
                return false;
            }
            
        }

        public void RequestHistoryData(Security security, DateTime beginTime, DateTime endTime, QotCommon.KLType kLType, QotCommon.RehabType rehabType = QotCommon.RehabType.RehabType_None)
        {
            ReqHisKL req = new ReqHisKL { Security = security, Begin = beginTime, End = endTime, KLType = kLType, RehabType = rehabType };
            requestHistoryKL.SendRequest(req);
            //return true;
        }

        public void RequestMarketData(Security security, QotCommon.SubType subType, int retry_lmt = 5, int retry_interval = 5)
        {
            try
            {
                Sub(security, subType, retry_lmt, retry_interval);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Cancel market data subscription
        /// </summary>
        /// <param name="security"></param>
        public void CancelMarketData(Security security)
        {
            try
            {
                Unsub(security, QotCommon.SubType.SubType_Ticker);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// PlaceOrder
        /// </summary>
        /// <param name="order">contain order deatails</param>
        /// <param name="accId">specify an account, will use the first account if omitted</param>
        /// <param name="trdEnv">real or paper account</param>
        /// <returns></returns>
        public uint PlaceOrder(Order order)
        {
            ulong accId = 0;
            if (order.AccId != null)
                accId = (ulong)order.AccId;
            else
            {
                TrdAcc acc = this.TrdAccs.FirstOrDefault(x => x.TrdEnv == (int)order.TradeEnv);
                if (acc != null)
                {
                    accId = acc.AccID;
                    order.AccId = accId;
                }                    
                else
                    throw new Exception("No account found");
            }
                

            TrdPlaceOrder.Request.Builder req = TrdPlaceOrder.Request.CreateBuilder();
            TrdPlaceOrder.C2S.Builder cs = TrdPlaceOrder.C2S.CreateBuilder();
            Common.PacketID.Builder packetID = Common.PacketID.CreateBuilder().SetConnID(Trd.GetConnectID()).SetSerialNo(0);
            TrdCommon.TrdHeader.Builder trdHeader = TrdCommon.TrdHeader.CreateBuilder().SetAccID(accId).SetTrdEnv((int)order.TradeEnv).SetTrdMarket((int)order.SecMarket);
            cs.SetPacketID(packetID).SetHeader(trdHeader)
                .SetTrdSide((int)order.TradeSide)
                .SetOrderType((int)order.OrderType)
                .SetCode(order.Code)
                .SetQty(order.Quantity)
                .SetPrice(order.Price)
                .SetAdjustPrice(order.IsAdjustPrice)
                .SetAdjustSideAndLimit(order.AdjSideAndLimit);
            if (!string.IsNullOrEmpty(order.Remark))
                cs.SetRemark(order.Remark);
            req.SetC2S(cs);

            uint serialNo = Trd.PlaceOrder(req.Build());
            return serialNo;
        }

        public uint ModifyOrder(Order order, ModifyOrderOp modifyOrderOp = ModifyOrderOp.ModifyOrderOp_Normal, bool forAll = false)
        {
            ulong accId = 0;
            if (order.AccId != null)
                accId = (ulong)order.AccId;
            else
            {
                ErrorEventArgs args = new ErrorEventArgs(null, 0, 0, 0, "Modify order failed, account is not assigned", null);
                OnError?.Invoke(this, args);
            }

            TrdModifyOrder.Request.Builder req = TrdModifyOrder.Request.CreateBuilder();
            TrdModifyOrder.C2S.Builder cs = TrdModifyOrder.C2S.CreateBuilder();
            Common.PacketID.Builder packetID = Common.PacketID.CreateBuilder().SetConnID(Trd.GetConnectID()).SetSerialNo(0);
            TrdCommon.TrdHeader.Builder trdHeader = TrdCommon.TrdHeader.CreateBuilder().SetAccID(accId).SetTrdEnv((int)order.TradeEnv).SetTrdMarket((int)order.SecMarket);
            cs.SetPacketID(packetID).SetHeader(trdHeader).SetForAll(forAll).SetModifyOrderOp((int)modifyOrderOp).SetOrderID(order.OrderId);
            if (modifyOrderOp == ModifyOrderOp.ModifyOrderOp_Normal)
                cs.SetQty(order.Quantity)
                    .SetPrice(order.Price)
                    .SetAdjustPrice(order.IsAdjustPrice)
                    .SetAdjustSideAndLimit(order.AdjSideAndLimit);

            req.SetC2S(cs);

            uint serialNo = Trd.ModifyOrder(req.Build());
            return serialNo;
        }
        public uint CancelOrder(Order order)
        {            
            return ModifyOrder(order, ModifyOrderOp.ModifyOrderOp_Cancel);
        }
        public uint CancelAllOrders(ulong accId)
        {
            Order order = new Order { AccId = accId };
            return ModifyOrder(order, ModifyOrderOp.ModifyOrderOp_Cancel, true);
        }
    }

    public class Security : IEquatable<Security>
    {
        public QotCommon.QotMarket Market { get; set; }
        public string Code { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Security);
        }

        public bool Equals(Security p)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(p, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, p))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != p.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (Market == p.Market) && (Code == p.Code);
        }

        public override int GetHashCode()
        {
            return (int)Market * 0x00010000 + Code.GetHashCode();
        }

        public static bool operator ==(Security lhs, Security rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Security lhs, Security rhs)
        {
            return !(lhs == rhs);
        }
    }

    public class PlateInfo
    {
        public QotCommon.Security Plate { get; set; }
        public string Name { get; set; }
        public QotCommon.PlateSetType PlateType { get; set; }
    }

    public class Contract
    {
        /*
         * BasicQot
         */
        public Security Security { get; set; }
        public bool IsSuspended { get; set; }
        public string ListTime { get; set; }
        public double PriceSpread { get; set; }
        /*
         * SecurityStaticBasic
         */
        public QotCommon.SecurityType SecurityType { get; set; }
        public string Name { get; set; }
        public bool Delisting { get; set; }
        public int LotSize { get; set; }
    }

    public class SubInfo
    {
        public int UsedQuota { get; set; }
        public int RemainQuota { get; set; }
    }

    public class HistoryKLQuota
    { 
        public class DetailItem
        {
            public Security Security { get; set; }
            public long RequestTimestamp { get; set; }
        }
        public int UsedQuota { get; set; }
        public int RemainQuota { get; set; }
        public List<DetailItem> DetailItems { get; set; } = new List<DetailItem>();
    }
    
    internal static class FTTaskExt
    {
        public static async Task<T> FromEvent<TEventArgs, T>(
            Action<EventHandler<TEventArgs>> registerEvent,
            Func<object> action,
            Action<EventHandler<TEventArgs>> unregisterEvent,
            CancellationToken token,
            object parameter = null,
            int? timeout = 3000,
            System.Action beforeAction = null,
            System.Action afterAction = null)
        {
            beforeAction?.Invoke();

            Security security = parameter != null ? parameter as Security : null;
            uint nSerialNo = 0;
            uint nSI = 0;
            uint nBQ = 0;
            Type type = typeof(uint);

            var tcs = new TaskCompletionSource<T>();
            if (timeout != null)
            {
                var ct = new CancellationTokenSource((int)timeout);
                ct.Token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
            }

            Contract contract = new Contract();
            EventHandler<TEventArgs> handler = (sender, args) =>
            {
                /*
                PropertyInfo pi = args.GetType().GetProperty("SerialNo");
                if (pi != null)
                    Console.WriteLine(args.ToString() + ", serial no:" + ((dynamic)args).SerialNo);
                else
                    Console.WriteLine(args.ToString());*/
                    
                #region get contract info

                if (args.GetType() == typeof(GetStaticInfoEventArgs) && typeof(T) == typeof(Contract))
                {
                    GetStaticInfoEventArgs arg = args as GetStaticInfoEventArgs;
                    if ((type == typeof(uint) && nSerialNo == arg.SerialNo) ||
                        (type == typeof((uint, uint)) && nSI == arg.SerialNo))
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            foreach (var item in arg.Result.S2C.StaticInfoListList)
                            {
                                if (security != null && item.Basic.Security.Code == security.Code
                                && item.Basic.Security.Market == (int)security.Market)
                                {
                                    contract.Security = (Security)parameter;
                                    contract.LotSize = item.Basic.LotSize;
                                    contract.SecurityType = (QotCommon.SecurityType)item.Basic.SecType;
                                    contract.Delisting = item.Basic.Delisting;
                                    contract.Name = item.Basic.Name;
                                    if ((contract.PriceSpread != 0 && type == typeof((uint, uint)))
                                        || type == typeof(uint))
                                        tcs.TrySetResult((T)(object)contract);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                else if (args.GetType() == typeof(GetBasicQotEventArgs) && typeof(T) == typeof(Contract))
                {
                    GetBasicQotEventArgs arg = args as GetBasicQotEventArgs;
                    if ((type == typeof(uint) && nSerialNo == arg.SerialNo) ||
                        (type == typeof((uint, uint)) && nBQ == arg.SerialNo))
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            foreach (var item in arg.Result.S2C.BasicQotListList)
                            {
                                if (security != null && item.Security.Code == security.Code
                                && item.Security.Market == (int)security.Market)
                                {
                                    contract.Security = (Security)parameter;
                                    contract.IsSuspended = item.IsSuspended;
                                    contract.Security = security;
                                    contract.ListTime = item.ListTime;
                                    contract.PriceSpread = item.PriceSpread;
                                    if ((!string.IsNullOrEmpty(contract.Name) && type == typeof((uint, uint)))
                                        || type == typeof(uint))
                                        tcs.TrySetResult((T)(object)contract);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion get contract info

                # region Connect Handling
                else if (args.GetType() == typeof(InitConnectedEventArgs) && typeof(TEventArgs) == typeof(InitConnectedEventArgs))
                {
                    InitConnectedEventArgs arg = args as InitConnectedEventArgs;
                    if (arg.ErrCode == 0)
                    {
                        tcs.TrySetResult((T)(object)true);
                    }
                    else
                    {
                        Exception ex = new Exception(arg.Message);
                        ex.Data.Add("ErrCode", arg.ErrCode);
                        ex.Data.Add("Message", arg.Message);
                        ex.Source = "FTTaskExt.FromEvent";
                        tcs.TrySetException(ex);
                    }
                }
                #endregion Connect Handling

                #region get subscription info
                else if (args.GetType() == typeof(GetSubInfoEventArgs) && typeof(TEventArgs) == typeof(GetSubInfoEventArgs))
                {
                    GetSubInfoEventArgs arg = args as GetSubInfoEventArgs;
                    if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                    {
                        SubInfo subInfo = new SubInfo();
                        subInfo.UsedQuota = arg.Result.S2C.TotalUsedQuota;
                        subInfo.RemainQuota = arg.Result.S2C.RemainQuota;
                        foreach (var item in arg.Result.S2C.ConnSubInfoListList)
                        {

                        }
                        tcs.TrySetResult((T)(object)subInfo);
                    }
                    else
                    {
                        Exception ex = new Exception(arg.Result.RetMsg);
                        ex.Source = "FTTaskExt.FromEvent";
                        tcs.TrySetException(ex);
                    }
                }
                #endregion

                #region RequestHistoryKLQuota
                else if (args.GetType() == typeof(RequestHistoryKLQuotaEventArgs) && typeof(TEventArgs) == typeof(RequestHistoryKLQuotaEventArgs))
                {
                    RequestHistoryKLQuotaEventArgs arg = args as RequestHistoryKLQuotaEventArgs;
                    if (arg.SerialNo == nSerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            HistoryKLQuota quota = new HistoryKLQuota();
                            quota.UsedQuota = arg.Result.S2C.UsedQuota;
                            quota.RemainQuota = arg.Result.S2C.RemainQuota;
                            foreach (var item in arg.Result.S2C.DetailListList)
                            {
                                quota.DetailItems.Add(new HistoryKLQuota.DetailItem
                                {
                                    Security = new Security
                                    {
                                        Code = item.Security.Code,
                                        Market = (QotCommon.QotMarket)item.Security.Market
                                    },
                                    RequestTimestamp = item.RequestTimeStamp
                                });
                            }
                            tcs.TrySetResult((T)(object)quota);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion

                #region RequestHistoryKL
                else if (args.GetType() == typeof(RequestHistoryKLEventArgs) && typeof(TEventArgs) == typeof(RequestHistoryKLEventArgs))
                {
                    RequestHistoryKLEventArgs arg = args as RequestHistoryKLEventArgs;
                    if (arg.SerialNo == nSerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            tcs.TrySetResult((T)(object)true);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);

                        }
                    }
                }
                #endregion

                #region GetKL
                else if (args.GetType() == typeof(GetKLEventArgs) && typeof(TEventArgs) == typeof(GetKLEventArgs))
                {
                    GetKLEventArgs arg = args as GetKLEventArgs;
                    if (arg.SerialNo == nSerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            List<QotCommon.KLine> kLines = new List<QotCommon.KLine>();
                            foreach (var item in arg.Result.S2C.KlListList)
                            {
                                kLines.Add(item);
                            }
                            tcs.TrySetResult((T)(object)kLines);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion

                #region Subscription
                else if (args.GetType() == typeof(SubscriptionEventArgs) && typeof(TEventArgs) == typeof(SubscriptionEventArgs))
                {
                    SubscriptionEventArgs arg = args as SubscriptionEventArgs;
                    if (arg.SerialNo == nSerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            tcs.TrySetResult((T)(object)true);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion

                #region Get plate security
                else if (args.GetType() == typeof(GetPlateSecurityEventArgs) && 
                typeof(TEventArgs) == typeof(GetPlateSecurityEventArgs))
                {
                    GetPlateSecurityEventArgs arg = args as GetPlateSecurityEventArgs;
                    List<Contract> contracts = new List<Contract>();
                    if (nSerialNo == arg.SerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            foreach (var info in arg.Result.S2C.StaticInfoListList)
                            {
                                contracts.Add(new Contract
                                {
                                    Security = new Security
                                    {
                                        Market = (QotCommon.QotMarket)info.Basic.Security.Market,
                                        Code = info.Basic.Security.Code
                                    },
                                    LotSize = info.Basic.LotSize,
                                    SecurityType = (QotCommon.SecurityType)info.Basic.SecType,
                                    Delisting = info.Basic.Delisting,
                                    Name = info.Basic.Name,
                                });
                            }
                            tcs.TrySetResult((T)(object)contracts);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion

                #region Get plate set
                else if (args.GetType() == typeof(GetPlateSetEventArgs) &&
                typeof(TEventArgs) == typeof(GetPlateSetEventArgs))
                {
                    GetPlateSetEventArgs arg = args as GetPlateSetEventArgs;
                    List<PlateInfo> plates = new List<PlateInfo>();
                    //(QotCommon.QotMarket Market, QotCommon.PlateSetType plate)? p = parameter as (QotCommon.QotMarket, QotCommon.PlateSetType)?;
                    if (nSerialNo != arg.SerialNo)
                    {
                        if (arg.Result.RetType == (int)Common.RetType.RetType_Succeed)
                        {
                            foreach (var info in arg.Result.S2C.PlateInfoListList)
                            {
                                plates.Add(new PlateInfo
                                {
                                    Plate = info.Plate,
                                    PlateType = (QotCommon.PlateSetType)info.PlateType,
                                    Name = info.Name
                                });
                            }
                            tcs.TrySetResult((T)(object)plates);
                        }
                        else
                        {
                            Exception ex = new Exception(arg.Result.RetMsg);
                            ex.Source = "FTTaskExt.FromEvent";
                            tcs.TrySetException(ex);
                        }
                    }
                }
                #endregion
            };
            registerEvent(handler);

            try
            {
                using (token.Register(() => tcs.SetCanceled()))
                {
                    object ret = action();
                    if (ret.GetType() == typeof(uint))
                        nSerialNo = (uint)ret;
                    if (ret.GetType() == typeof((uint, uint)))
                    {
                        nSI = ((Tuple<uint, uint>)ret).Item1;
                        nBQ = ((Tuple<uint, uint>)ret).Item2;
                    }
                    type = ret.GetType();
                    return await tcs.Task;
                }
            }
            finally
            {
                unregisterEvent(handler);
                afterAction?.Invoke();
            }
        }

        public static async Task<TEventArgs> FromEventToAsync<TEventArgs>(
            Action<EventHandler<TEventArgs>> registerEvent,
            System.Action action,
            Action<EventHandler<TEventArgs>> unregisterEvent,
            CancellationToken token,
            System.Action beforeAction = null,
            System.Action afterAction = null
            )
        {
            beforeAction?.Invoke();
            var tcs = new TaskCompletionSource<TEventArgs>();
            EventHandler<TEventArgs> handler = (sender, args) => tcs.TrySetResult(args);
            registerEvent(handler);

            try
            {
                using (token.Register(() => tcs.SetCanceled()))
                {
                    action();
                    return await tcs.Task;
                }
            }
            finally
            {
                unregisterEvent(handler);
                afterAction?.Invoke();
            }
        }
    }
}
