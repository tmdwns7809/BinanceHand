using System;
using System.Collections.Generic;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Futures.MarketData;
using CryptoExchange.Net.Sockets;
using BrightIdeasSoftware;
using Binance.Net.Enums;

namespace BinanceHand
{
    class ItemData
    {
        public UpdateSubscription sub;

        public string Name;

        public short Real = 0;

        public bool AggFirst = true;
        public bool klineFirst = true;

        public Stick secStick = new Stick();
        public List<Stick> secStickList = new List<Stick>();
        public List<Stick> oldSecStickList = new List<Stick>();
        public static string secChartLabel = "HH:mm:ss";
        public int secLastIndex;
        public int count10sec;
        public double ms10secTot = 0;
        public double ms10secAvg;
        public double md10secTot = 0;
        public double md10secAvg;
        public double ms10secDev;
        public double ms10secSDev;
        public double md10secDev;
        public double md10secSDev;
        public double msSDevRatio;
        public double mdSDevRatio;
        public decimal SDevRatioPrice;
        public decimal lowestSDevRatioPrice;
        public decimal minLowestSDevRatioPrice = 0m;
        public double lowestSDevRatio = double.MaxValue;
        public double minLowestSDevRatio = 0.0;
        public int lowestSDevCount10sec;
        public int minLowestSDevCount10sec;
        public decimal price10secHighest;
        public decimal price10secLowest;
        public int index;

        public Stick minStick = new Stick();
        public List<Stick> minStickList = new List<Stick>();
        public static string minChartLabel = "dd HH:mm";

        public Stick hourStick = new Stick();
        public List<Stick> hourStickList = new List<Stick>();
        public static string hourChartLabel = "MM-dd HH";

        public Stick dayStick = new Stick();
        public List<Stick> dayStickList = new List<Stick>();
        public static string dayChartLabel = "yyyy-MM-dd";

        public bool isChartShowing = false;
        public short Real1Condition;
        public short Real2Condition;
        public bool LorS = true;

        public bool order = false;
        public DateTime OrderTime;
        public OrderType OrderType;
        public OrderSide OrderSide;
        public decimal orderStartClosePrice;
        public decimal OrderPrice;
        public decimal OrderAmount;
        public decimal OrderFilled;
        public bool ReduceOnly;
        public TimeInForce Condition;
        public string clientOrderID;
        public long orderID;

        public bool positionWhenOrder = false;
        public bool position = false;
        public UpdateSubscription markSub;
        public int Leverage;
        public int maxLeverage;
        public decimal Size;
        public decimal EntryPrice;
        public decimal MarkPrice;
        public decimal maintMargin;
        public decimal InitialMargin;
        public decimal notianalValue;
        public decimal maxNotionalValue;
        public decimal minNotionalValue;
        public decimal PNL;
        public decimal ROE;

        public List<BinanceFuturesBracket> brackets;

        public decimal minSize;
        public decimal priceTickSize;

        public ItemData(BinanceFuturesUsdtSymbol fu)
        {
            Name = fu.Name.Trim().ToUpper();
            minSize = fu.LotSizeFilter.MinQuantity;
            priceTickSize = fu.PriceFilter.TickSize;
            minNotionalValue = fu.MinNotionalFilter.MinNotional;
        }
    }

    class Stick
    {
        public decimal[] Price = new decimal[4];        // 0: High, 1: Low, 2: Open, 3: Close

        public decimal Ms = 0;
        public decimal Md = 0;

        public int TCount = 0;

        public DateTime Time;
    }
}
