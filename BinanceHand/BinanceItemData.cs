using System;
using System.Collections.Generic;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Futures.MarketData;
using CryptoExchange.Net.Sockets;
using BrightIdeasSoftware;
using Binance.Net.Enums;
using TradingLibrary;

namespace BinanceHand
{
    class BinanceItemData : TradeItemData
    {
        public UpdateSubscription aggSub;
        public UpdateSubscription hoSub;

        public bool klineFirst = true;

        public double SDevRatio;
        public decimal lastSDevRatioPrice;
        public double lastSDevRatio;
        public int lastSDevCount10sec;
        public decimal priceDiffStep;
        public decimal priceRange;
        public decimal priceDiffRate;
        public decimal priceStandard;


        public OrderType OrderType;
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
        public decimal priceTickSize;

        public bool simulEnterOrder = false;

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

        public BinanceItemData(BinanceFuturesUsdtSymbol fu) : base(fu.Name.Trim().ToUpper())
        {
            minSize = fu.LotSizeFilter.MinQuantity;
            priceTickSize = fu.PriceFilter.TickSize;
            minNotionalValue = fu.MinNotionalFilter.MinNotional;
        }
    }
}
