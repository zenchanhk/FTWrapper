using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Futu.OpenApi;
using Futu.OpenApi.Pb;
using static Futu.OpenApi.Pb.TrdCommon;

namespace FTWrapper
{
    public class Order
    {
        public ulong OrderId { get; set; }
        public ulong? AccId { get; set; }
        public TrdEnv TradeEnv { get; set; } = TrdEnv.TrdEnv_Real;
        public TrdSide TradeSide { get; set; }
        public OrderType OrderType { get; set; }
        public string Code { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        //是否调整价格，如果价格不合法，是否调整到合法价位，true调整，false不调整
        public bool IsAdjustPrice { get; set; }
        //调整方向和调整幅度百分比限制，正数代表向上调整，负数代表向下调整，具体值代表调整幅度限制，如：0.015代表向上调整且幅度不超过1.5%；-0.01代表向下调整且幅度不超过1%
        public double AdjSideAndLimit { get; set; }
        public TrdSecMarket SecMarket { get; set; }
        //用户备注字符串，最多只能传64字节。可用于标识订单唯一信息等，下单填上，订单结构就会带上。
        public string Remark { get; set; }
    }
}
