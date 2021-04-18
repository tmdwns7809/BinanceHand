using System;
using System.Collections.Generic;
using Binance.Net.Objects.Spot.MarketData;
using Binance.Net.Objects.Futures.MarketData;
using CryptoExchange.Net.Sockets;
using BrightIdeasSoftware;
using Binance.Net.Objects.Futures.MarketData;

namespace BinanceHand
{
    class ItemData
    {
        public BinanceSymbol spotSymbol;
        public BinanceFuturesUsdtSymbol futureUsdtSymbol;
        public BinanceFuturesCoinSymbol futureCoinSymbol;

        public bool isSpot;
        public bool isFutureUsdt;

        public UpdateSubscription sub;

        public string Name;
        public decimal RateBfor = 0m;

        public string date;
        public string newTime;

        public bool hasAll = false;
        public bool buyOrder = false;
        public bool sellOrder = false;

        public bool aggFirst = true;
        public bool klineFirst = true;

        public bool scSave = false;
        public int scSaveTimeStamp;

        public Stick secStick = new Stick();
        public List<Stick> secStickList = new List<Stick>();
        public static string secChartLabel = "HH:mm:ss";

        public Stick minStick = new Stick();
        public List<Stick> minStickList = new List<Stick>();
        public static string minChartLabel = "dd HH:mm";

        public bool isChartShowing = false;
        public bool isAggReady = false;
        public bool isAggOn = false;
        public short FlatMinRow;
        public short AggReadyRow = 0;
        public bool LorS = true;
        public OLVListItem item;

        public short win;
        public short lose;
        public short winLoseTot;
        public string WinPrecantage = "0.00(0)";

        public decimal secMsList0Tot = 0;
        public decimal secMsList0Avg = 0;
        public decimal secMdList0Tot = 0;
        public decimal secMdList0Avg = 0;

        public decimal secMsList1Tot = 0;
        public decimal secMsList1Avg = 0;
        public decimal secMdList1Tot = 0;
        public decimal secMdList1Avg = 0;

        public Queue<decimal> pureSecCountQ = new Queue<decimal>();
        public decimal pureSecCountQTot = 0;
        public decimal pureSecCountQAvg = 0;

        public static short amt0 = 5;
        public static short amt1 = 20;

        public bool chuQReady = false;

        public bool tooManyAmt;
        public string tooManyAmtTime;

        public string bhtTime;
        public decimal bhtPrc;

        public decimal profitRate;
        public decimal profitRateSum;
        public decimal profitRateMul;
        public string ProfitRateSumAndMul = "0.00(0.00)";

        public UpdateSubscription markSub;
        public bool Position = false;
        public bool PositionLong;
        public int Leverage;
        public decimal Size;
        public decimal EntryPrice;
        public decimal MarkPrice;
        public decimal maintMargin;
        public decimal InitialMargin;
        public decimal notianalValue;
        public decimal PNL;
        public decimal ROE;

        public List<BinanceFuturesBracket> brackets;
        public int nowBracketIndex;


        public ItemData(BinanceSymbol s, BinanceFuturesUsdtSymbol fu, BinanceFuturesCoinSymbol fc)
        {

            if (s != null)
            {
                spotSymbol = s;
                Name = s.Name.Trim().ToUpper();
                isSpot = true;
            }
            else if (fu != null)
            {
                futureUsdtSymbol = fu;
                Name = fu.Name.Trim().ToUpper();
                isSpot = false;
                isFutureUsdt = true;
            }
            else if (fc != null)
            {
                futureCoinSymbol = fc;
                Name = fc.Name.Trim().ToUpper();
                isSpot = false;
                isFutureUsdt = false;
            }

            for (int i = 0; i < amt1; i++)
                pureSecCountQ.Enqueue(0);
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
