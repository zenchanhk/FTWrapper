using System;
using System.Collections.Generic;
using System.Linq;
using Futu.OpenApi;
using Futu.OpenApi.Pb;

namespace FTWrapper.Events
{
    #region ConnCallbacks
    public class InitConnectedEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public long ErrCode { get; private set; }
        public string Message { get; private set; }
        public InitConnectedEventArgs(FTAPI_Conn client, long errCode, string message)
        {
            Client = client;
            ErrCode = errCode;
            Message = message;
        }
    }

    public class DisconnectedEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public long ErrCode { get; private set; }
        public DisconnectedEventArgs(FTAPI_Conn client, long errCode)
        {
            Client = client;
            ErrCode = errCode;
        }
    }
    #endregion

    #region QotCallbacks
    public class ErrorEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public int ErrCode { get; set; }
        public int RetType { get; set; }
        public string RetMsg { get; set; }
        public Object S2C { get; private set; }
        public ErrorEventArgs(FTAPI_Conn client, int nSerialNo, int errCode, int retType, string retMsg, Object s2c)
        {
            Client = client;
            SerialNo = nSerialNo;
            ErrCode = errCode;
            RetType = retType;
            RetMsg = retMsg;
            S2C = s2c;
        }
    }
    public class GlobalStateEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public GetGlobalState.Response Result { get; private set; }
        public GlobalStateEventArgs(FTAPI_Conn client, int nSerialNo, GetGlobalState.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class SubscriptionEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotSub.Response Result { get; private set; }
        public SubscriptionEventArgs(FTAPI_Conn client, int nSerialNo, QotSub.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class RegQotPushEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotRegQotPush.Response Result { get; private set; }
        public RegQotPushEventArgs(FTAPI_Conn client, int nSerialNo, QotRegQotPush.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetSubInfoEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetSubInfo.Response Result { get; private set; }
        public GetSubInfoEventArgs(FTAPI_Conn client, int nSerialNo, QotGetSubInfo.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetTickerEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetTicker.Response Result { get; private set; }
        public GetTickerEventArgs(FTAPI_Conn client, int nSerialNo, QotGetTicker.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetBasicQotEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetBasicQot.Response Result { get; private set; }
        public GetBasicQotEventArgs(FTAPI_Conn client, int nSerialNo, QotGetBasicQot.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetOrderBookEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetOrderBook.Response Result { get; private set; }
        public GetOrderBookEventArgs(FTAPI_Conn client, int nSerialNo, QotGetOrderBook.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetKLEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetKL.Response Result { get; private set; }
        public GetKLEventArgs(FTAPI_Conn client, int nSerialNo, QotGetKL.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetRTEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetRT.Response Result { get; private set; }
        public GetRTEventArgs(FTAPI_Conn client, int nSerialNo, QotGetRT.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetBrokerEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetBroker.Response Result { get; private set; }
        public GetBrokerEventArgs(FTAPI_Conn client, int nSerialNo, QotGetBroker.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class RequestRehabEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotRequestRehab.Response Result { get; private set; }
        public RequestRehabEventArgs(FTAPI_Conn client, int nSerialNo, QotRequestRehab.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class RequestHistoryKLQuotaEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotRequestHistoryKLQuota.Response Result { get; private set; }
        public RequestHistoryKLQuotaEventArgs(FTAPI_Conn client, int nSerialNo, QotRequestHistoryKLQuota.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class RequestHistoryKLEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotRequestHistoryKL.Response Result { get; private set; }
        public RequestHistoryKLEventArgs(FTAPI_Conn client, int nSerialNo, QotRequestHistoryKL.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetTradeDateEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetTradeDate.Response Result { get; private set; }
        public GetTradeDateEventArgs(FTAPI_Conn client, int nSerialNo, QotGetTradeDate.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetStaticInfoEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetStaticInfo.Response Result { get; private set; }
        public GetStaticInfoEventArgs(FTAPI_Conn client, int nSerialNo, QotGetStaticInfo.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetSecuritySnapshotEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetSecuritySnapshot.Response Result { get; private set; }
        public GetSecuritySnapshotEventArgs(FTAPI_Conn client, int nSerialNo, QotGetSecuritySnapshot.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetPlateSetEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetPlateSet.Response Result { get; private set; }
        public GetPlateSetEventArgs(FTAPI_Conn client, int nSerialNo, QotGetPlateSet.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetPlateSecurityEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetPlateSecurity.Response Result { get; private set; }
        public GetPlateSecurityEventArgs(FTAPI_Conn client, int nSerialNo, QotGetPlateSecurity.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetReferenceEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetReference.Response Result { get; private set; }
        public GetReferenceEventArgs(FTAPI_Conn client, int nSerialNo, QotGetReference.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetOwnerPlateEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetOwnerPlate.Response Result { get; private set; }
        public GetOwnerPlateEventArgs(FTAPI_Conn client, int nSerialNo, QotGetOwnerPlate.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetHoldingChangeListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetHoldingChangeList.Response Result { get; private set; }
        public GetHoldingChangeListEventArgs(FTAPI_Conn client, int nSerialNo, QotGetHoldingChangeList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetOptionChainEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetOptionChain.Response Result { get; private set; }
        public GetOptionChainEventArgs(FTAPI_Conn client, int nSerialNo, QotGetOptionChain.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetWarrantEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetWarrant.Response Result { get; private set; }
        public GetWarrantEventArgs(FTAPI_Conn client, int nSerialNo, QotGetWarrant.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetCapitalFlowEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetCapitalFlow.Response Result { get; private set; }
        public GetCapitalFlowEventArgs(FTAPI_Conn client, int nSerialNo, QotGetCapitalFlow.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetCapitalDistributionEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetCapitalDistribution.Response Result { get; private set; }
        public GetCapitalDistributionEventArgs(FTAPI_Conn client, int nSerialNo, QotGetCapitalDistribution.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetUserSecurityEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetUserSecurity.Response Result { get; private set; }
        public GetUserSecurityEventArgs(FTAPI_Conn client, int nSerialNo, QotGetUserSecurity.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class ModifyUserSecurityEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotModifyUserSecurity.Response Result { get; private set; }
        public ModifyUserSecurityEventArgs(FTAPI_Conn client, int nSerialNo, QotModifyUserSecurity.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class NotifyEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public Notify.Response Result { get; private set; }
        public NotifyEventArgs(FTAPI_Conn client, int nSerialNo, Notify.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateBasicQotEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateBasicQot.Response Result { get; private set; }
        public UpdateBasicQotEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateBasicQot.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateKLEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateKL.Response Result { get; private set; }
        public UpdateKLEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateKL.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateRTEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateRT.Response Result { get; private set; }
        public UpdateRTEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateRT.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateTickerEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateTicker.Response Result { get; private set; }
        public UpdateTickerEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateTicker.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateOrderBookEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateOrderBook.Response Result { get; private set; }
        public UpdateOrderBookEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateOrderBook.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateBrokerEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateBroker.Response Result { get; private set; }
        public UpdateBrokerEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateBroker.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateOrderDetailEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotUpdateOrderDetail.Response Result { get; private set; }
        public UpdateOrderDetailEventArgs(FTAPI_Conn client, int nSerialNo, QotUpdateOrderDetail.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class StockFilterEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotStockFilter.Response Result { get; private set; }
        public StockFilterEventArgs(FTAPI_Conn client, int nSerialNo, QotStockFilter.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetCodeChangeEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public QotGetCodeChange.Response Result { get; private set; }
        public GetCodeChangeEventArgs(FTAPI_Conn client, int nSerialNo, QotGetCodeChange.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }
    #endregion


    #region TrdCallbacks
    public class GetAccListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetAccList.Response Result { get; private set; }
        public GetAccListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetAccList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UnlockTradeEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdUnlockTrade.Response Result { get; private set; }
        public UnlockTradeEventArgs(FTAPI_Conn client, int nSerialNo, TrdUnlockTrade.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class SubAccPushEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdSubAccPush.Response Result { get; private set; }
        public SubAccPushEventArgs(FTAPI_Conn client, int nSerialNo, TrdSubAccPush.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetFundsEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetFunds.Response Result { get; private set; }
        public GetFundsEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetFunds.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetPositionListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetPositionList.Response Result { get; private set; }
        public GetPositionListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetPositionList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetMaxTrdQtysEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetMaxTrdQtys.Response Result { get; private set; }
        public GetMaxTrdQtysEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetMaxTrdQtys.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetOrderListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetOrderList.Response Result { get; private set; }
        public GetOrderListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetOrderList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetOrderFillListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetOrderFillList.Response Result { get; private set; }
        public GetOrderFillListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetOrderFillList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetHistoryOrderListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetHistoryOrderList.Response Result { get; private set; }
        public GetHistoryOrderListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetHistoryOrderList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class GetHistoryOrderFillListEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdGetHistoryOrderFillList.Response Result { get; private set; }
        public GetHistoryOrderFillListEventArgs(FTAPI_Conn client, int nSerialNo, TrdGetHistoryOrderFillList.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateOrderEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdUpdateOrder.Response Result { get; private set; }
        public UpdateOrderEventArgs(FTAPI_Conn client, int nSerialNo, TrdUpdateOrder.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class UpdateOrderFillEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdUpdateOrderFill.Response Result { get; private set; }
        public UpdateOrderFillEventArgs(FTAPI_Conn client, int nSerialNo, TrdUpdateOrderFill.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class PlaceOrderEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdPlaceOrder.Response Result { get; private set; }
        public PlaceOrderEventArgs(FTAPI_Conn client, int nSerialNo, TrdPlaceOrder.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }

    public class ModifyOrderEventArgs : EventArgs
    {
        public FTAPI_Conn Client { get; private set; }
        public int SerialNo { get; private set; }
        public TrdModifyOrder.Response Result { get; private set; }
        public ModifyOrderEventArgs(FTAPI_Conn client, int nSerialNo, TrdModifyOrder.Response result)
        {
            Client = client;
            SerialNo = nSerialNo;
            Result = result;
        }
    }
    #endregion
}

