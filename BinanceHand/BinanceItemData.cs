using System;
using System.Collections.Generic;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Objects.Models.Futures;
using CryptoExchange.Net.Sockets;
using BrightIdeasSoftware;
using Binance.Net.Enums;
using TradingLibrary;
using TradingLibrary.Base;

namespace BinanceHand
{
    public class BinanceItemData : TradeItemData
    {
        public UpdateSubscription aggSub;
        public UpdateSubscription hoSub;

        public MakerOrderData makerOrderData = new MakerOrderData();

        public bool klineFirst = true;

        public FuturesOrderType OrderType;
        public OrderSide OrderSide;
        public bool ReduceOnly;
        public TimeInForce Condition;
        public string clientOrderID;

        public UpdateSubscription markSub;
        public int Leverage;
        public int maxLeverage;
        public decimal MarkPrice;
        public decimal maintMargin;
        public decimal InitialMargin;
        public decimal notianalValue;
        public decimal maxNotionalValue;
        public decimal minNotionalValue;

        public List<BinanceFuturesBracket> brackets;

        public decimal minSize;


        public decimal simulBhtPrc;
        public DateTime simulBhtTime;

        public decimal simulProfitRate;

        public bool simulLorS;

        public decimal ms3secTot = 0;
        public decimal ms3secAvg;
        public decimal md3secTot = 0;
        public decimal md3secAvg;

        public decimal ms8secTot = 0;
        public decimal ms8secAvg;
        public decimal md8secTot = 0;
        public decimal md8secAvg;

        public decimal ROE;

        public BinanceItemData(BinanceFuturesUsdtSymbol fu, int n) : base(fu.Name.Trim().ToUpper(), n)
        {
            minSize = fu.LotSizeFilter.MinQuantity;
            hoDiff = fu.PriceFilter.TickSize;
            minNotionalValue = fu.MinNotionalFilter.MinNotional;
        }
    }

    public class MakerOrderData
    {
        public bool HoOn = false;
        public UpdateSubscription hoSub = null;

        public decimal ask;
        public decimal bid;
        public DateTime abUpdatedTime;

    }
}
