using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Futu.OpenApi;
using Futu.OpenApi.Pb;

namespace FTWrapper
{
    public class ReqHisKL
    {
        public Security Security { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public QotCommon.KLType KLType { get; set; }
        public QotCommon.RehabType RehabType { get; set; }
    }

    public class ReqPlaceOrder
    {
        public Security Security { get; set; }
        public ulong ConnectID { get; set; }
        public ulong AccID { get; set; }
        public TrdCommon.TrdEnv TrdEnv { get; set; }
        public TrdCommon.TrdMarket TrdMarket { get; set; }
        public double Qty { get; set; }
        public double Price { get; set; }
        public TrdCommon.TrdSide TrdSide { get; set; }
        public TrdCommon.OrderType OrderType { get; set; }
        public bool AdjustPrice { get; set; } //是否调整价格，如果价格不合法，是否调整到合法价位，true调整，false不调整
        public string Remark { get; set; } //用户备注字符串，最多只能传64字节。可用于标识订单唯一信息等，下单填上，订单结构就会带上。
    }


    class Limitation
    {
        public (int reqNum, int duration) freq1;
        public (int reqNum, int duration) freq2;
    }
    abstract class APIRestriction<TBuilder, TRequest, TReqClass>
    {
        /*
        private static readonly Limitation unLockLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation placeOrderLmt = new Limitation { freq1 = (15, 30), freq2 = (5, 1) };
        private static readonly Limitation modifyOrderLmt = new Limitation { freq1 = (20, 30), freq2 = (5, 1) };
        private static readonly Limitation getMaxTrdQtyLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderListLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderFillListLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getRehabLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getSymbolsLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation reqHistoryKLLmt = new Limitation { freq1 = (10, 30) }; */


        private static readonly object reqDicLock = new object();
        private static readonly object reqQueueLock = new object();        

        protected FTClient client = null;
        protected Limitation limitation = null;
        protected Func<Task<int>> GetQuota;
        protected Func<TRequest, Task<bool>> ReqFunc;

        // variable for request history KLines limitation
        protected Dictionary<TReqClass, DateTime> reqDic = new Dictionary<TReqClass, DateTime>();
        protected ObservableCollection<TReqClass> reqQueue = new ObservableCollection<TReqClass>();
        private ManualResetEvent mre = new ManualResetEvent(false);
        private Thread thread;
        private bool isThreadRunning = false;

        public APIRestriction(FTClient client, Limitation lmt, Func<TRequest, Task<bool>> reqFunc, Func<Task<int>> getQuota = null)
        {
            this.client = client;
            limitation = lmt;
            GetQuota = getQuota;
            ReqFunc = reqFunc;
            reqQueue.CollectionChanged += ReqQueue_CollectionChanged;
            Start();
        }

        private void Start()
        {
            thread = new Thread(HandleReqQueue);
            thread.IsBackground = true;
            thread.Start();
        }
        public void HandleReqQueue()
        {
            bool inProcesss = false;
            while (true)
            {
                if (!inProcesss)
                {
                    inProcesss = true;
                    isThreadRunning = true;
                    mre.Reset();
                    Task.Run(() => ProcessReqQueue())
                        .ContinueWith(result =>
                        {
                            inProcesss = false;
                            isThreadRunning = false;
                            if (reqQueue.Count > 0)
                                mre.Set();
                        });
                }
                mre.WaitOne();
            }
        }

        private void ReqQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (reqQueue.Count > 0)
            {
                if (!isThreadRunning)
                    mre.Set();
            }
            else
                mre.Reset();
        }
        protected abstract TBuilder MakeReqBuilder(TReqClass request);
        public virtual async Task<bool> SendRequest(TReqClass request)
        {
            try
            {   
                // remove out-of-date item
                const int removal_period = 3600; //
                foreach (var req in reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds > removal_period).ToList())
                {
                    lock (reqDicLock)
                    {
                        reqDic.Remove(req.Key);
                    }
                }

                var lmt = limitation.freq1;
                int tmp = 0;
                lock (reqDicLock)
                {
                    tmp = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration).Count();
                }                    

                var lmt2 = limitation.freq2;
                int tmp2 = 0;
                if (lmt2.reqNum > 0)
                {
                    lock (reqDicLock)
                    {
                        tmp2 = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt2.duration).Count();
                    }
                }                    

                bool isAdded = false;
                if (tmp < lmt.reqNum && ((lmt2.reqNum > 0 && tmp2 < lmt2.reqNum) || lmt2.reqNum == 0))
                {
                    lock (reqDicLock)
                    {
                        reqDic.Add(request, DateTime.Now);
                    }
                    TBuilder reqBuilder = MakeReqBuilder(request);
                    bool succeed = await ReqFunc(((dynamic)reqBuilder).Build());
                    if (succeed)
                        isAdded = true;
                    else
                    {
                        // Console.WriteLine("{0} request history KL failed", request.Security.Code);
                    }
                }
                if (!isAdded)
                {
                    lock (reqQueueLock)
                    {
                        reqQueue.Add(request);
                    }
                    lock (reqDicLock)
                    {
                        reqDic.Remove(request);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task<List<(TReqClass request, bool succeed)>> ProcessReqQueue()
        {
            try
            {
                List<(TReqClass request, bool succeed)> result = new List<(TReqClass request, bool succeed)>();
                var lmt = limitation.freq1;
                var lmt2 = limitation.freq2;
                //HistoryKLQuota quota = await client.RequestHistoryKLQuota();
                int quota = int.MaxValue;
                if (GetQuota != null)
                    quota = await GetQuota();
                //Console.WriteLine("Remain Quota(ProcessReqHisKLQueue):" + quota.RemainQuota);
                List<TReqClass> queue = null;
                lock (reqQueueLock)
                {
                    queue = this.reqQueue.ToList();
                }
                for (int i = 0; i < queue.Count; i++)
                {
                    var request = queue[i];
                    if (quota <= 0)
                    {
                        result.Add((request, false));
                        lock (reqQueueLock)
                        {
                            this.reqQueue.Remove(request);
                        }
                    }
                    else
                    {
                        List<KeyValuePair<TReqClass, DateTime>> list = null;
                        lock (reqDicLock)
                        {
                            list = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration)?
                                        .OrderByDescending(x => x.Value)?.ToList();
                        }

                        List<KeyValuePair<TReqClass, DateTime>> list2 = null;
                        if (lmt2.reqNum > 0)
                        {
                            lock (reqDicLock)
                            {
                                list2 = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt2.duration)?
                                            .OrderByDescending(x => x.Value)?.ToList();
                            }
                        }
                            

                        if (((list != null && list.Count < lmt.reqNum) || list == null) &&
                            ((list2 != null && list2.Count < lmt2.reqNum) || list2 == null))
                        {
                            TBuilder reqBuilder = MakeReqBuilder(request);
                            bool succeed = await ReqFunc(((dynamic)reqBuilder).Build());
                            if (succeed)
                            {
                                lock (reqDicLock)
                                {
                                    reqDic.Add(request, DateTime.Now);
                                }
                                lock (reqQueueLock)
                                {
                                    this.reqQueue.Remove(request);
                                }
                                quota--;
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
                                await Task.Delay(lmt.duration * 1000 - (int)(DateTime.Now - last.Value).TotalMilliseconds);
                            }
                            if (list2 != null)
                            {
                                var last = list2.FirstOrDefault();
                                await Task.Delay(lmt2.duration * 1000 - (int)(DateTime.Now - last.Value).TotalMilliseconds);
                            }
                            i--;
                        }
                    }
                }
                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }
    }

    abstract class APIRestrictionParallel<TBuilder, TRequest, TReqClass>
    {
        /*
        private static readonly Limitation unLockLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation placeOrderLmt = new Limitation { freq1 = (15, 30), freq2 = (5, 1) };
        private static readonly Limitation modifyOrderLmt = new Limitation { freq1 = (20, 30), freq2 = (5, 1) };
        private static readonly Limitation getMaxTrdQtyLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderListLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getOrderFillListLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getRehabLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation getSymbolsLmt = new Limitation { freq1 = (10, 30) };
        private static readonly Limitation reqHistoryKLLmt = new Limitation { freq1 = (10, 30) }; */


        private static readonly object reqDicLock = new object();
        private static readonly object reqQueueLock = new object();

        protected FTClient client = null;
        protected Limitation limitation = null;
        protected Func<Task<int>> GetQuota;
        protected Func<TRequest, Task<bool>> ReqFunc;

        // variable for request history KLines limitation
        protected Dictionary<TReqClass, DateTime> reqDic = new Dictionary<TReqClass, DateTime>();
        protected ObservableCollection<TReqClass> reqQueue = new ObservableCollection<TReqClass>();
        private ManualResetEvent mre = new ManualResetEvent(false);
        private Thread thread;
        private bool isThreadRunning = false;

        public APIRestrictionParallel(FTClient client, Limitation lmt, Func<TRequest, Task<bool>> reqFunc, Func<Task<int>> getQuota = null)
        {
            this.client = client;
            limitation = lmt;
            GetQuota = getQuota;
            ReqFunc = reqFunc;
            reqQueue.CollectionChanged += ReqQueue_CollectionChanged;
            Start();
        }

        private void Start()
        {
            thread = new Thread(HandleReqQueue);
            thread.IsBackground = true;
            thread.Start();
        }
        public void HandleReqQueue()
        {
            bool inProcesss = false;
            while (true)
            {
                if (!inProcesss)
                {
                    inProcesss = true;
                    isThreadRunning = true;
                    mre.Reset();
                    Task.Run(() => ProcessReqHisKLQueue())
                        .ContinueWith(result =>
                        {
                            inProcesss = false;
                            isThreadRunning = false;
                            if (reqQueue.Count > 0)
                                mre.Set();
                        });
                }
                mre.WaitOne();
            }
        }

        private void ReqQueue_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (reqQueue.Count > 0)
            {
                if (!isThreadRunning)
                    mre.Set();
            }
            else
                mre.Reset();
        }
        protected abstract TBuilder MakeReqBuilder(TReqClass request);
        public virtual async Task<bool> SendRequest(TReqClass request)
        {
            try
            {
                // remove out-of-date item
                const int removal_period = 3600; //
                foreach (var req in reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds > removal_period).ToList())
                {
                    lock (reqDicLock)
                    {
                        reqDic.Remove(req.Key);
                    }
                }

                var lmt = limitation.freq1;
                int tmp = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration).Count();

                var lmt2 = limitation.freq2;
                int tmp2 = 0;
                if (lmt2.reqNum > 0)
                    tmp2 = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt2.duration).Count();

                bool isAdded = false;
                if (tmp < lmt.reqNum && ((lmt2.reqNum > 0 && tmp2 < lmt2.reqNum) || lmt2.reqNum == 0))
                {
                    lock (reqDicLock)
                    {
                        reqDic.Add(request, DateTime.Now);
                    }
                    TBuilder reqBuilder = MakeReqBuilder(request);
                    bool succeed = await ReqFunc(((dynamic)reqBuilder).Build());
                    if (succeed)
                        isAdded = true;
                    else
                    {
                        // Console.WriteLine("{0} request history KL failed", request.Security.Code);
                    }
                }
                if (!isAdded)
                {
                    lock (reqQueueLock)
                    {
                        reqQueue.Add(request);
                    }
                    lock (reqDicLock)
                    {
                        reqDic.Remove(request);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private async Task<List<(TReqClass request, bool succeed)>> ProcessReqHisKLQueue()
        {
            List<(TReqClass request, bool succeed)> result = new List<(TReqClass request, bool succeed)>();
            var lmt = limitation.freq1;
            var lmt2 = limitation.freq2;
            //HistoryKLQuota quota = await client.RequestHistoryKLQuota();
            int quota = int.MaxValue;
            if (GetQuota != null)
                quota = await GetQuota();
            //Console.WriteLine("Remain Quota(ProcessReqHisKLQueue):" + quota.RemainQuota);
            List<TReqClass> queue = null;
            lock (reqQueueLock)
            {
                queue = this.reqQueue.ToList();
            }
            for (int i = 0; i < queue.Count; i++)
            {
                var request = queue[i];
                if (quota <= 0)
                {
                    result.Add((request, false));
                    lock (reqQueueLock)
                    {
                        this.reqQueue.Remove(request);
                    }
                }
                else
                {
                    var list = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt.duration)?
                        .OrderByDescending(x => x.Value)?.ToList();

                    dynamic list2 = null;
                    if (lmt2.reqNum > 0)
                        list2 = reqDic.Where(x => (DateTime.Now - x.Value).TotalSeconds <= lmt2.duration)?
                        .OrderByDescending(x => x.Value)?.ToList();

                    if (((list != null && list.Count < lmt.reqNum) || list == null) &&
                        ((list2 != null && list2.Count < lmt2.reqNum) || list2 == null))
                    {
                        TBuilder reqBuilder = MakeReqBuilder(request);
                        bool succeed = await ReqFunc(((dynamic)reqBuilder).Build());
                        if (succeed)
                        {
                            lock (reqDicLock)
                            {
                                reqDic.Add(request, DateTime.Now);
                            }
                            lock (reqQueueLock)
                            {
                                this.reqQueue.Remove(request);
                            }
                            quota--;
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
                            await Task.Delay(lmt.duration * 1000 - (int)(DateTime.Now - last.Value).TotalMilliseconds);
                        }
                        if (list2 != null)
                        {
                            var last = list2.FirstOrDefault();
                            await Task.Delay(lmt2.duration * 1000 - (int)(DateTime.Now - last.Value).TotalMilliseconds);
                        }
                        i--;
                    }
                }
            }
            return result;
        }
    }

    class CRequestHistoryKL : APIRestriction<QotRequestHistoryKL.Request.Builder, QotRequestHistoryKL.Request, ReqHisKL>
    {
        public CRequestHistoryKL(FTClient client, Limitation lmt, Func<QotRequestHistoryKL.Request, Task<bool>> reqFunc, Func<Task<int>> getQuota) :
            base(client, lmt, reqFunc, getQuota)
        { }

        protected override QotRequestHistoryKL.Request.Builder MakeReqBuilder(ReqHisKL request)
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

        public override async Task<bool> SendRequest(ReqHisKL request)
        {
            try
            {
                // if found in the queue, update request and exit
                var item = reqQueue.Where(x => x.Security == request.Security)
                                        .LastOrDefault();
                if (item != null)
                {
                    item.Begin = request.Begin;
                    item.End = request.End;
                    item.KLType = request.KLType;
                    return true;
                }

                return await base.SendRequest(request);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }

    class CPlaceOrder : APIRestriction<TrdPlaceOrder.Request.Builder, QotRequestHistoryKL.Request, ReqHisKL>
    {
        private ulong accID = 0;
        private ulong cnxID = 0;
        public CPlaceOrder(FTClient client, Limitation lmt, Func<QotRequestHistoryKL.Request, Task<bool>> reqFunc, Func<Task<int>> getQuota) :
            base(client, lmt, reqFunc, getQuota)
        { }

        protected override TrdPlaceOrder.Request.Builder MakeReqBuilder(ReqHisKL request)
        {
            TrdPlaceOrder.Request.Builder req = TrdPlaceOrder.Request.CreateBuilder();
            TrdPlaceOrder.C2S.Builder cs = TrdPlaceOrder.C2S.CreateBuilder();
            Common.PacketID.Builder packetID = Common.PacketID.CreateBuilder().SetConnID(cnxID).SetSerialNo(0);
            TrdCommon.TrdHeader.Builder trdHeader = TrdCommon.TrdHeader.CreateBuilder().SetAccID(this.accID).SetTrdEnv((int)TrdCommon.TrdEnv.TrdEnv_Real).SetTrdMarket((int)TrdCommon.TrdMarket.TrdMarket_HK);
            cs.SetPacketID(packetID).SetHeader(trdHeader).SetTrdSide((int)TrdCommon.TrdSide.TrdSide_Sell).SetOrderType((int)TrdCommon.OrderType.OrderType_AbsoluteLimit).SetCode("01810").SetQty(100.00).SetPrice(10.2).SetAdjustPrice(true);
            req.SetC2S(cs);
            return req;
        }

        public override async Task<bool> SendRequest(ReqHisKL request)
        {
            try
            {
                // if found in the queue, update request and exit
                var item = reqQueue.Where(x => x.Security == request.Security)
                                        .LastOrDefault();
                if (item != null)
                {
                    item.Begin = request.Begin;
                    item.End = request.End;
                    item.KLType = request.KLType;
                    return true;
                }

                return await base.SendRequest(request);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
