using System;
using System.Collections.Generic;
using Binance.Net.Objects.Spot.MarketData;
using CryptoExchange.Net.Sockets;

namespace BinanceHand
{
    class ItemData
    {
        public UpdateSubscription sub;

        public string Name;
        public decimal Rate = 0m;

        public bool buyOrder = false;
        public bool sellOrder = false;
        public bool sellNow = false;

        public bool cheFirst = true;

        public bool scSave = false;
        public int scSaveTimeStamp;

        public bool real = false;
        public int realTimeStamp;

        public Stick secStick = new Stick();
        public List<Stick> secStickList = new List<Stick>();

        public Stick minStick = new Stick();
        public List<Stick> minStickList = new List<Stick>();

        public int listViewIndex;

        public bool isChartShowing = false;
        public bool isAggOn = false;
        public short FlatMinRow;

        public bool isSpot;

        public ItemData(string n, bool s)
        {
            Name = n;
            isSpot = s;
        }
    }

    class Stick
    {
        public decimal[] Price = new decimal[4];

        public decimal Ms = 0;
        public decimal Md = 0;

        public DateTime Time;
    }

    public enum Price
    {
        High = 0,
        Low = 1,
        Open = 2,
        Close = 3
    }
}
