﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Binance.Net.Objects.Spot.MarketStream;
using Binance.Net.Interfaces;
using System.Windows.Forms.DataVisualization.Charting;
using Binance.Net.Objects.Futures.MarketStream;
using Binance.Net.Objects.Futures.UserStream;
using TradingLibrary.Trading;
using TradingLibrary;
using TradingLibrary.Base;

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        string AssetTextMarginRatio = "Margin Ratio";
        string AssetTextMaintenanceMargin = "Maintenance Margin";
        string AssetTextMarginBalance = "Margin Balance";
        string AssetTextAvailableBalance = "Available Balance";
        string AssetTextWalletBalance = "Wallet Balance";

        RadioButton GTCRadioButton = new RadioButton();
        RadioButton IOCRadioButton = new RadioButton();
        RadioButton PORadioButton = new RadioButton();
        TextBox leverageTextBox0 = new TextBox();
        TextBox leverageTextBox1 = new TextBox();
        TextBox orderPriceTextBox0 = new TextBox();
        TextBox orderPriceTextBox1 = new TextBox();
        TextBox orderPriceTextBox2 = new TextBox();
        RadioButton marketRadioButton = new RadioButton();
        TextBox orderSizeTextBox0 = new TextBox();
        TextBox orderSizeTextBox1 = new TextBox();
        CheckBox miniSizeCheckBox = new CheckBox();
        CheckBox ROCheckBox = new CheckBox();
        CheckBox autoSizeCheckBox = new CheckBox();
        TextBox autoSizeTextBox0 = new TextBox();
        TextBox autoSizeTextBox1 = new TextBox();

        string ForDecimalString = "0.#############################";

        bool testnet = false;

        BinanceClient client;
        BinanceSocketClient socketClient;

        List<string> symbolList = new List<string>();

        Queue<Task> requestTRTaskQueue = new Queue<Task>();

        public Form1()
        {
            InitializeComponent();

            FormClosed += Form1_FormClosed;
            Load += Form1_Load;

            Trading.instance = new Trading(this, (0, -1), 20, false);
            Trading.instance.HoONandOFF += Trading_HoONandOFF;
            Trading.instance.AggONandOFF += Trading_AggONandOFF;
            Trading.instance.ShowChartAdditional += Trading_ShowChartAdditional;
            Trading.instance.ResetOrderView += Trading_ResetOrderView;
            Trading.instance.LoadMoreAdditional += Trading_LoadMoreAdditional;
            Trading.instance.PlaceOrder += Trading_PlaceOrder;
            Trading.instance.CodeListView.Columns.Remove(Trading.instance.CodeListView.GetColumn("Name"));
            Trading.instance.realHandResultListView.Columns.Remove(Trading.instance.realHandResultListView.GetColumn("Name"));
            Trading.instance.realAutoResultListView.Columns.Remove(Trading.instance.realAutoResultListView.GetColumn("Name"));
            foreach (var listView in Trading.instance.simulResultListView)
                listView.Columns.Remove(listView.GetColumn("Name"));

            SetOrderView();

            SetClientAndKey();

            StartThread();
        }
        void SetOrderView()
        {
            new List<Control>()
                {
                    GTCRadioButton, IOCRadioButton, PORadioButton, leverageTextBox0, leverageTextBox1,
                    orderPriceTextBox0, orderPriceTextBox1, orderPriceTextBox2, marketRadioButton,
                    orderSizeTextBox0, orderSizeTextBox1, miniSizeCheckBox, ROCheckBox,
                    autoSizeCheckBox, autoSizeTextBox0, autoSizeTextBox1
                }.ForEach(new Action<Control>((control) => { Trading.instance.orderGroupBox.Controls.Add(control); }));

            Trading.instance.SetRadioButton_CheckBox(GTCRadioButton, "GTC",
                new Size(50, 16), new Point(5, 15));

            Trading.instance.SetRadioButton_CheckBox(IOCRadioButton, "IOC",
                GTCRadioButton.Size, new Point(GTCRadioButton.Location.X + GTCRadioButton.Size.Width + 10, GTCRadioButton.Location.Y));
            //marketRadioButton.CheckedChanged += (sender, e) => { if (marketRadioButton.Checked) orderPriceTextBox1.Enabled = false; else orderPriceTextBox1.Enabled = true; };

            Trading.instance.SetRadioButton_CheckBox(PORadioButton, "Post Only",
              new Size(100, GTCRadioButton.Height), new Point(IOCRadioButton.Location.X + IOCRadioButton.Size.Width + 10, IOCRadioButton.Location.Y));

            Trading.instance.SetTextBox(leverageTextBox0, "", true);
            leverageTextBox0.Size = new Size(41, 21);
            leverageTextBox0.Location = new Point(PORadioButton.Location.X + PORadioButton.Width + 50, PORadioButton.Location.Y);
            leverageTextBox0.ReadOnly = false;
            leverageTextBox0.BorderStyle = BorderStyle.Fixed3D;

            Trading.instance.SetTextBox(leverageTextBox1, "/ 125", true);
            leverageTextBox1.Size = new Size(41, 21);
            leverageTextBox1.Location = new Point(leverageTextBox0.Location.X + leverageTextBox0.Width, leverageTextBox0.Location.Y);

            Trading.instance.SetTextBox(orderPriceTextBox0, "Price", true);
            orderPriceTextBox0.Size = new Size(41, 21);
            orderPriceTextBox0.Location = new Point(GTCRadioButton.Location.X, GTCRadioButton.Location.Y + GTCRadioButton.Size.Height + 7 + (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);

            Trading.instance.SetTextBox(orderPriceTextBox1, "", true);
            orderPriceTextBox1.Size = orderPriceTextBox0.Size;
            orderPriceTextBox1.Location = new Point(orderPriceTextBox0.Location.X + orderPriceTextBox0.Size.Width + 3, orderPriceTextBox0.Location.Y - (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);
            orderPriceTextBox1.ReadOnly = false;
            orderPriceTextBox1.BackColor = ColorSet.ControlBack;

            Trading.instance.SetTextBox(orderPriceTextBox2, "%", true);
            orderPriceTextBox2.Size = orderPriceTextBox0.Size;
            orderPriceTextBox2.Location = new Point(orderPriceTextBox1.Location.X + orderPriceTextBox1.Size.Width + 3, orderPriceTextBox0.Location.Y);

            Trading.instance.SetRadioButton_CheckBox(marketRadioButton, "Market",
                new Size(100, GTCRadioButton.Height), new Point(orderPriceTextBox2.Location.X + orderPriceTextBox2.Size.Width + 10, orderPriceTextBox2.Location.Y));

            Trading.instance.SetTextBox(orderSizeTextBox0, "Size", true);
            orderSizeTextBox0.Size = orderPriceTextBox0.Size;
            orderSizeTextBox0.Location = new Point(orderPriceTextBox0.Location.X, orderPriceTextBox0.Location.Y + orderPriceTextBox0.Size.Height + 7 + (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);

            Trading.instance.SetTextBox(orderSizeTextBox1, "", true);
            orderSizeTextBox1.Size = orderPriceTextBox0.Size;
            orderSizeTextBox1.Location = new Point(orderSizeTextBox0.Location.X + orderSizeTextBox0.Size.Width + 3, orderSizeTextBox0.Location.Y - (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);
            orderSizeTextBox1.ReadOnly = false;
            orderSizeTextBox1.BackColor = ColorSet.ControlBack;

            Trading.instance.SetRadioButton_CheckBox(miniSizeCheckBox, "Minimum Size",
                new Size(100, GTCRadioButton.Height), new Point(orderSizeTextBox1.Location.X + orderSizeTextBox1.Width, orderSizeTextBox0.Location.Y));
            miniSizeCheckBox.CheckedChanged += (sender, e) => { if (Trading.instance.itemDataShowing != null) TickMinSizeButton(miniSizeCheckBox.Checked); };

            Trading.instance.SetRadioButton_CheckBox(ROCheckBox, "Reduce Only",
                PORadioButton.Size, new Point(miniSizeCheckBox.Location.X, miniSizeCheckBox.Location.Y + miniSizeCheckBox.Height));

            Trading.instance.SetRadioButton_CheckBox(autoSizeCheckBox, "Auto",
                new Size(50, GTCRadioButton.Height),
                new Point(orderSizeTextBox0.Location.X + 20, ROCheckBox.Location.Y + ROCheckBox.Size.Height + 10 + (autoSizeTextBox0.Size.Height - autoSizeCheckBox.Size.Height) / 2));
            autoSizeCheckBox.CheckedChanged += (sender, e) => { if (Trading.instance.itemDataShowing != null) TickAutoSizeButton(autoSizeCheckBox.Checked); };

            Trading.instance.SetTextBox(autoSizeTextBox0, "", true);
            autoSizeTextBox0.Size = new Size(50, GTCRadioButton.Height);
            autoSizeTextBox0.Location = new Point(autoSizeCheckBox.Location.X + autoSizeCheckBox.Size.Width + 3, autoSizeCheckBox.Location.Y - (autoSizeTextBox0.Size.Height - autoSizeTextBox1.Size.Height) / 2);
            autoSizeTextBox0.ReadOnly = false;
            autoSizeTextBox0.BackColor = ColorSet.ControlBack;

            Trading.instance.SetTextBox(autoSizeTextBox1, "% of Balance", true);
            autoSizeTextBox1.Size = new Size(100, GTCRadioButton.Height);
            autoSizeTextBox1.Location = new Point(autoSizeTextBox0.Location.X + autoSizeTextBox0.Size.Width + 3, autoSizeTextBox0.Location.Y + (autoSizeTextBox0.Size.Height - autoSizeTextBox1.Size.Height) / 2);
        }
        void SetClientAndKey()
        {
            var clientOption = new BinanceClientOptions();
            var socketOption = new BinanceSocketClientOptions();

            if (testnet)       //testnet
            {
                if (true)
                {   /*future*/
                    clientOption.BaseAddressUsdtFutures = "https://testnet.binancefuture.com";
                    clientOption.BaseAddressCoinFutures = "https://testnet.binancefuture.com";
                    clientOption.ApiCredentials = new ApiCredentials(BinanceBase.testnet_API_Key, BinanceBase.testnet_Secret_Key);
                    socketOption.BaseAddressUsdtFutures = "wss://stream.binancefuture.com";
                    socketOption.BaseAddressCoinFutures = "wss://stream.binancefuture.com";
                    socketOption.BaseAddress = "wss://stream.binancefuture.com";
                    socketOption.ApiCredentials = new ApiCredentials(BinanceBase.testnet_API_Key, BinanceBase.testnet_Secret_Key);
                }
                else
                {   /*spot 안됌
                    clientOption.BaseAddress = "https://testnet.binance.vision/api";
                    clientOption.ApiCredentials = new ApiCredentials("KMx8rvC0pd8yuR7yYKQYZSqteiuSQsfcID4WQ24bpv4Qm6M3OL66kntmLWDPmnIf", "64hzy1tONvRvlXBLmIkb609rTW2QbCEbAcACkqlJDTP7Ljk28QNpuBMi6sWE5ylj");
                    socketOption.BaseAddress = "wss://testnet.binance.vision";
                    socketOption.ApiCredentials = new ApiCredentials("KMx8rvC0pd8yuR7yYKQYZSqteiuSQsfcID4WQ24bpv4Qm6M3OL66kntmLWDPmnIf", "64hzy1tONvRvlXBLmIkb609rTW2QbCEbAcACkqlJDTP7Ljk28QNpuBMi6sWE5ylj");*/
                }
            }
            else
            {
                clientOption.ApiCredentials = new ApiCredentials(BinanceBase.future2_API_Key, BinanceBase.future2_Secret_Key);
                clientOption.AutoTimestamp = false;
                clientOption.TradeRulesBehaviour = TradeRulesBehaviour.None;
                socketOption.ApiCredentials = new ApiCredentials(BinanceBase.future2_API_Key, BinanceBase.future2_Secret_Key);
                //socketOption.AutoReconnect = true;
                socketOption.ReconnectInterval = TimeSpan.FromMinutes(1);
                //clientOption.LogVerbosity = LogVerbosity.Debug;
                //clientOption.LogWriters = new List<TextWriter> { Console.Out };
            }

            client = new BinanceClient(clientOption);
            socketClient = new BinanceSocketClient(socketOption);

            //socketClient.UnsubscribeAllAsync().Wait();
        }

        void StartThread()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    while (requestTRTaskQueue.Count > 0 && requestTRTaskQueue.Peek() != null)
                    {
                        requestTRTaskQueue.Dequeue().RunSynchronously();

                        if (BaseFunctions.binance_weight_now + 50 > BaseFunctions.binance_weight_limit)
                            Thread.Sleep(60000);
                    }

                    Thread.Sleep(1000);
                }
            }, Trading.instance.tokenSource.Token);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            SetItemDataList();

            SubscribeToUserStream(testnet, true);

            GetAccountInfo();

            Trading.instance.LoadSavedItemData();

            var result = socketClient.FuturesUsdt.SubscribeToKlineUpdatesAsync(symbolList, KlineInterval.OneMinute, OnKlineUpdates).Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
        }
        void SetItemDataList()
        {
            var result = client.FuturesUsdt.System.GetExchangeInfoAsync().Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            BaseFunctions.BinanceUpdateWeightLimit(result.Data.RateLimits, this);

            var n = 1;
            foreach (var s in result.Data.Symbols)
            {
                if (s.Status != SymbolStatus.Trading)
                    continue;

                var itemData = new BinanceItemData(s, n++);
                Trading.instance.itemDataDic.Add(itemData.Code, itemData);
                symbolList.Add(itemData.Code);
                Trading.instance.CodeListView.AddObject(itemData);

                foreach (var interval in Enum.GetValues(typeof(KlineInterval)).Cast<KlineInterval>())
                    requestTRTaskQueue.Enqueue(new Task(() =>
                    {
                        var vc = BaseFunctions.ChartValuesDic[interval];
                        var v = itemData.listDic[vc];
                        v.found = false;

                        if (v.list.Count != 0)
                            BaseFunctions.ShowError(this);

                        var nowTime = BaseFunctions.NowTime();
                        v.list = LoadSticks(itemData.Code, interval, nowTime.Date, nowTime);
                        v.lastStick = v.list.Last();
                        v.list.RemoveAt(v.list.Count - 1);
                        BaseFunctions.OneChartFindConditionAndAdd(itemData, vc);
                    }));

                //var data = client.FuturesUsdt.ChangeInitialLeverage(itemData.Name, 1);
                //if (!data.Success)
                //    MessageBox.Show("fail");
            }

            if (requestTRTaskQueue.Count == 0)
                BaseFunctions.ShowError(this);
        }
        List<TradeStick> LoadSticks(string Code, KlineInterval interval, DateTime startTime, DateTime endTime)
        {
            var vc = BaseFunctions.ChartValuesDic[interval];

            var plusStartTime = startTime.AddSeconds(-vc.seconds * (BaseFunctions.TotalNeedDays - 1));
            var size = (int)endTime.Subtract(plusStartTime).TotalSeconds / vc.seconds + 1;
            var result2 = client.FuturesUsdt.Market.GetKlinesAsync(Code, interval, plusStartTime, endTime, size).Result;


            if (!result2.Success)
            {
                BaseFunctions.ShowError(this);
                return default;
            }

            BaseFunctions.BinanceUpdateWeightNow(result2.ResponseHeaders);

            return SetRSIAandGetList(result2.Data, startTime);
        }
        TradeStick GetStick(IBinanceKline stickReal)
        {
            var stick = new TradeStick();
            stick.Price[0] = stickReal.High;
            stick.Price[1] = stickReal.Low;
            stick.Price[2] = stickReal.Open;
            stick.Price[3] = stickReal.Close;

            stick.Ms = stickReal.TakerBuyBaseVolume;
            stick.Md = stickReal.BaseVolume - stickReal.TakerBuyBaseVolume;

            stick.Time = stickReal.OpenTime;

            return stick;
        }
        TradeStick CalLastStick(List<TradeStick> minList, DateTime time)
        {
            var stick = new TradeStick() { Time = time };

            if (minList[minList.Count - 1].Time >= time)
                for (int i = minList.Count - 1; i >= 0; i--)
                {
                    if (stick.Price[0] == 0)
                    {
                        stick.Price[1] = minList[i].Price[1];
                        stick.Price[3] = minList[i].Price[3];
                    }
                    if (minList[i].Price[0] > stick.Price[0])
                        stick.Price[0] = minList[i].Price[0];
                    if (minList[i].Price[1] < stick.Price[1])
                        stick.Price[1] = minList[i].Price[1];
                    stick.Price[2] = minList[i].Price[2];

                    stick.Ms += minList[i].Ms;
                    stick.Md += minList[i].Md;

                    stick.TCount += minList[i].TCount;

                    if (minList[i].Time == stick.Time)
                        break;
                }

            return stick;
        }
        List<TradeStick> SetRSIAandGetList(IEnumerable<IBinanceKline> data, DateTime startTime)
        {
            var list = new List<TradeStick>();
            foreach (var stickReal in data)
            {
                var stick = GetStick(stickReal);
                BaseFunctions.SetRSIAandDiff(list, stick);
                list.Add(stick);
            }

            var startIndex = BaseFunctions.GetStartIndex(list, startTime);
            if (startIndex > 0)
                list.RemoveRange(0, startIndex);

            return list;
        }

        void SubscribeToUserStream(bool testnet, bool testnetFutures)
        {
            var result = ((testnet && !testnetFutures) ? client.Spot.UserStream.StartUserStreamAsync() : client.FuturesUsdt.UserStream.StartUserStreamAsync()).Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            var listenKey = result.Data;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(29 * 60000);

                    var result1 = ((testnet && !testnetFutures) ? client.Spot.UserStream.KeepAliveUserStreamAsync(listenKey) : client.FuturesUsdt.UserStream.KeepAliveUserStreamAsync(listenKey)).Result;
                    if (!result1.Success)
                        BaseFunctions.ShowError(this);

                    BaseFunctions.BinanceUpdateWeightNow(result1.ResponseHeaders);

                    var count = 0;
                    foreach (var item in Trading.instance.itemDataDic.Values)
                        if (item.Last30minStickFluc > 3)
                            count++;

                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat),
                        DBHelper.conn1DetectHistoryColumn1, "직전 1시간 이내 3% 이상 변동 종목 개수", DBHelper.conn1DetectHistoryColumn2, count.ToString());
                }
            }, Trading.instance.tokenSource.Token);

            var result2 = ((testnet && !testnetFutures) ?
                socketClient.Spot.SubscribeToUserDataUpdatesAsync(listenKey, null, null, null, null) :
                socketClient.FuturesUsdt.SubscribeToUserDataUpdatesAsync(listenKey,
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        },
                        OnAccountUpdates,
                        OnOrderUpdates,
                        data => {
                            var a = data;
                        })).Result;
            if (!result2.Success)
                BaseFunctions.ShowError(this);
        }
        void GetAccountInfo()
        {
            Trading.instance.assetDic.Add(AssetTextMarginRatio, new Asset { AssetName = AssetTextMarginRatio });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMarginRatio]);
            Trading.instance.assetDic.Add(AssetTextMaintenanceMargin, new Asset { AssetName = AssetTextMaintenanceMargin });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMaintenanceMargin]);
            Trading.instance.assetDic.Add(AssetTextMarginBalance, new Asset { AssetName = AssetTextMarginBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMarginBalance]);
            Trading.instance.assetDic.Add(AssetTextAvailableBalance, new Asset { AssetName = AssetTextAvailableBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextAvailableBalance]);
            Trading.instance.assetDic.Add(AssetTextWalletBalance, new Asset { AssetName = AssetTextWalletBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextWalletBalance]);

            var result = client.FuturesUsdt.Account.GetAccountInfoAsync().Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            foreach (var s in result.Data.Assets)
            {
                if (s.Asset == "USDT")
                {
                    Trading.instance.assetDic[AssetTextMarginBalance].Amount = s.MarginBalance;
                    Trading.instance.assetDic[AssetTextAvailableBalance].Amount = s.AvailableBalance;
                    Trading.instance.assetDic[AssetTextWalletBalance].Amount = s.WalletBalance;
                }
            }
            foreach (var s in result.Data.Positions)
            {
                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataDic[s.Symbol];

                    itemData.RealEnter = true;
                    itemData.positionData[(int)(s.PositionSide == PositionSide.Long ? Position.Long : Position.Short)].Enter = true;
                    itemData.Has = 1;

                    var result1 = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates).Result;
                    if (!result1.Success)
                        BaseFunctions.ShowError(this);

                    itemData.markSub = result1.Data;

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2);
                    itemData.maintMargin = s.MaintMargin;

                    Trading.instance.CodeListView.RemoveObject(itemData);
                    Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });
                }
            }

            var result2 = client.FuturesUsdt.GetPositionInformationAsync().Result;
            if (!result2.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result2.ResponseHeaders);

            foreach (var s in result2.Data)
            {
                if (s.MarginType == FuturesMarginType.Isolated)
                {
                    var result3 = client.FuturesUsdt.ChangeMarginTypeAsync(s.Symbol, FuturesMarginType.Cross).Result;
                    if (!result3.Success)
                        BaseFunctions.ShowError(this);

                    BaseFunctions.BinanceUpdateWeightNow(result3.ResponseHeaders);
                }

                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataDic[s.Symbol];

                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    itemData.RealPosition = s.PositionSide == PositionSide.Long ? Position.Long : Position.Short;
                    itemData.Size = s.Quantity;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.RealEnterPrice = s.EntryPrice;
                    itemData.PNL = Math.Round(s.UnrealizedPnl, 2);
                    itemData.ClosePNL = itemData.PNL;
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);
                    itemData.maxNotionalValue = s.MaxNotional;

                    Trading.instance.CodeListView.RefreshObject(itemData);

                    var result4 = client.FuturesUsdt.GetBracketsAsync(s.Symbol).Result;
                    if (!result4.Success)
                        BaseFunctions.ShowError(this);

                    BaseFunctions.BinanceUpdateWeightNow(result4.ResponseHeaders);

                    foreach (var brackets in result4.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                }
            }
        }

        void TickMinSizeButton(bool on)
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            miniSizeCheckBox.Checked = on;
            if (itemDataShowing.RealEnter)
                orderSizeTextBox1.Text = Math.Abs(itemDataShowing.Size).ToString();
            else if (itemDataShowing.secStick != default)
                orderSizeTextBox1.Text = ((int)(itemDataShowing.minNotionalValue / itemDataShowing.secStick.Price[3] / itemDataShowing.minSize + 1) * itemDataShowing.minSize).ToString();
            else
                orderSizeTextBox1.Text = itemDataShowing.minSize.ToString();

            autoSizeCheckBox.Enabled = !on;
            autoSizeTextBox0.Enabled = !on;

            if (miniSizeCheckBox.Checked || autoSizeCheckBox.Checked)
                orderSizeTextBox1.Enabled = false;
            else
                orderSizeTextBox1.Enabled = true;
        }
        void TickAutoSizeButton(bool on)
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            autoSizeCheckBox.Checked = on;
            if (itemDataShowing.RealEnter)
                orderSizeTextBox1.Text = Math.Abs(itemDataShowing.Size).ToString();
            else
                orderSizeTextBox1.Text = itemDataShowing.minSize.ToString();

            if (miniSizeCheckBox.Checked || autoSizeCheckBox.Checked)
                orderSizeTextBox1.Enabled = false;
            else
                orderSizeTextBox1.Enabled = true;

            autoSizeTextBox0.Text = Trading.instance.autoSizeBudgetLimit.ToString();
        }

        void OnKlineUpdates(DataEvent<IBinanceStreamKlineData> data0)
        {
            var data = data0.Data;
            var itemData = Trading.instance.itemDataDic[data.Symbol] as BinanceItemData;

            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;
                Trading.instance.UpdateReqRcv(Trading.instance.KlineRcvTextBox, Trading.instance.KlineRcv + 1);
            }

            if (data.Data.Final)
            {
                var vm = itemData.listDic[BaseChartTimeSet.OneMinute];
                var newStick = GetStick(data.Data);

                lock (Trading.minLocker)
                {
                    if (newStick.Time > Trading.firstFinalMin)
                    {
                        Trading.firstFinalMin = newStick.Time;
                        BaseFunctions.foundItemList = new List<(BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)>)>[]
                            { new List<(BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)>)>(),
                                new List<(BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)>)>() };
                    }
                }

                for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                {
                    itemData.positionData[j].foundList = new List<(DateTime foundTime, ChartValues chartValues)>();
                    itemData.positionData[j].found = false;
                }

                for (int i = 0; i < itemData.listDic.Count; i++)
                {
                    var v = itemData.listDic.Values[i];
                    if (v.lastStick == default)
                        break;

                    var vc = itemData.listDic.Keys[i];
                    var timeDiff = newStick.Time.Subtract(v.lastStick.Time).TotalSeconds;
                    if (timeDiff > vc.seconds)
                    {
                        var list = LoadSticks(itemData.Code, (KlineInterval)vc.original, v.lastStick.Time, newStick.Time);

                        if (list.Count == 0 || (int)list[0].Time.Subtract(v.list[v.list.Count - 1].Time).TotalSeconds != vc.seconds)
                            BaseFunctions.ShowError(this);

                        v.lastStick = list.Last();
                        list.RemoveAt(list.Count - 1);
                        v.list.AddRange(list);

                        timeDiff = newStick.Time.Subtract(v.lastStick.Time).TotalSeconds;
                    }

                    if (Math.Abs(timeDiff) > vc.seconds)
                        BaseFunctions.ShowError(this);

                    if (v.first)
                    {
                        if (vc == BaseChartTimeSet.OneMinute)
                        {
                            if (timeDiff == vc.seconds)
                            {
                                var list = LoadSticks(itemData.Code, KlineInterval.OneMinute, v.lastStick.Time, newStick.Time);

                                if (list.Count != 2 || list[0].Time != v.lastStick.Time)
                                    BaseFunctions.ShowError(this);

                                v.lastStick = list[0];
                            }
                            else
                                v.lastStick = new TradeStick() { Time = newStick.Time };
                        }
                        else if (vc.seconds <= BaseChartTimeSet.OneDay.seconds)
                            v.lastStick = CalLastStick(vm.list, v.lastStick.Time);

                        v.first = false;
                    }

                    if (timeDiff == vc.seconds)
                    {
                        v.list.Add(v.lastStick);
                        v.lastStick = new TradeStick() { Time = newStick.Time };
                    }

                    if (v.lastStick.Price[1] == 0)
                    {
                        v.lastStick.Price[1] = newStick.Price[1];
                        v.lastStick.Price[2] = newStick.Price[2];
                    }

                    if (newStick.Price[0] > v.lastStick.Price[0])
                        v.lastStick.Price[0] = newStick.Price[0];
                    if (newStick.Price[1] < v.lastStick.Price[1])
                        v.lastStick.Price[1] = newStick.Price[1];
                    v.lastStick.Price[3] = newStick.Price[3];

                    v.lastStick.Ms += newStick.Ms;
                    v.lastStick.Md += newStick.Md;

                    v.lastStick.TCount += newStick.TCount;

                    BaseFunctions.SetRSIAandDiff(v.list, v.lastStick);
                    BaseFunctions.OneChartFindConditionAndAdd(itemData, vc);
                }

                for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                {
                    var positionData = itemData.positionData[j];
                    if (!itemData.positionData[(int)Position.Long].Enter && !itemData.positionData[(int)Position.Short].Enter && 
                        !itemData.RealEnter && positionData.found && newStick.Time == Trading.firstFinalMin)
                    {
                        lock (Trading.minLocker)
                            BaseFunctions.foundItemList[j].Add((itemData, positionData.foundList));

                        BaseFunctions.AlertStart(itemData.Code + "-" + positionData.foundList);

                        var conditionResult2 = BaseFunctions.AllItemFindCondition();
                        if (Trading.instance.autoRealTrading && conditionResult2.found && conditionResult2.position[j])
                            foreach (var foundItem in BaseFunctions.foundItemList[j])
                                if (!itemData.positionData[j].Enter && Trading_PlaceOrder(itemData, true, false, true))
                                {
                                    itemData.isAuto = true;
                                    BaseFunctions.EnterSetting(positionData, vm.lastStick);
                                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + positionData.EnterTime.ToString(BaseFunctions.TimeFormat),
                                        DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~1 매수(자동)", DBHelper.conn1OrderHistoryColumn2, "종가:" + positionData.EnterPrice);
                                }
                    }
                    else
                    if (positionData.Enter && itemData.RealEnter && itemData.isAuto && 
                        BaseFunctions.ExitConditionFinal(itemData, (Position)j) && Trading_PlaceOrder(itemData, (Position)j != Position.Long, false, true))
                    {
                        positionData.Enter = false;
                        Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + positionData.EnterTime.ToString(BaseFunctions.TimeFormat),
                            DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 매도(자동)", DBHelper.conn1OrderHistoryColumn2, "전송가:" + newStick.Price[3]);
                    }
                }
            }
        }

        void OnAggregatedTradeUpdates(DataEvent<BinanceStreamAggregatedTrade> data0)
        {
            var data = data0.Data;
            var itemData = Trading.instance.itemDataDic[data.Symbol] as BinanceItemData;

            itemData.hoCheGu = data.BuyerIsMaker ? 2 : 1;
            itemData.newPrice = data.Price;
            itemData.newTime = data.TradeTime;
            itemData.newAmt = data.Quantity;

            //if (itemData.isChartShowing && Trading.instance.mainChart.InvokeRequired)
            //    Invoke(new Action(() => { Trading.instance.AggMain(itemData); }));
            //else
                Trading.instance.AggMain(itemData);
        }
        void OnHoUpdates(DataEvent<IBinanceFuturesEventOrderBook> data0)
        {
            if (Trading.instance.hoChart.InvokeRequired)
            {
                BeginInvoke(new Action(() => { OnHoUpdates(data0); }));
                return;
            }

            var data = data0.Data;
            var itemData = Trading.instance.itemDataDic[data.Symbol.ToUpper()] as BinanceItemData;

            if (!itemData.isChartShowing)
                return;

            itemData.hoHighQuan = 0;

            itemData.hoIndex = 19;
            foreach (var bid in data.Bids)
            {
                itemData.ho.Price[itemData.hoIndex] = bid.Price;
                itemData.ho.Quan[itemData.hoIndex] = bid.Quantity;

                Trading.instance.HoMain0(itemData, true);

                itemData.hoIndex--;
            }

            itemData.hoIndex = 20;
            foreach (var ask in data.Asks)
            {
                itemData.ho.Price[itemData.hoIndex] = ask.Price;
                itemData.ho.Quan[itemData.hoIndex] = ask.Quantity;

                Trading.instance.HoMain0(itemData, false);

                itemData.hoIndex++;
            }

            Trading.instance.HoMain1(itemData);
        }

        void OnMarkPriceUpdates(DataEvent<BinanceFuturesUsdtStreamMarkPrice> data0)
        {
            if (Trading.instance.assetsListView.InvokeRequired)
            {
                BeginInvoke(new Action(() => { OnMarkPriceUpdates(data0); }));
                return;
            }

            var data = data0.Data;
            var itemData = Trading.instance.itemDataDic[data.Symbol] as BinanceItemData;

            if (itemData.RealEnter)
            {
                itemData.MarkPrice = Math.Round(data.MarkPrice, 2);
                itemData.notianalValue = data.MarkPrice * Math.Abs(itemData.Size);
                itemData.InitialMargin = Math.Round(itemData.notianalValue / itemData.Leverage, 2);
                itemData.PNL = Math.Round((itemData.MarkPrice - itemData.RealEnterPrice) * itemData.Size, 2);
                itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                for (int i = 0; i < itemData.brackets.Count; i++)
                {
                    if (itemData.notianalValue > itemData.brackets[i].Floor && itemData.notianalValue <= itemData.brackets[i].Cap)
                    {
                        itemData.maintMargin = 0;

                        for (int j = 0; j < i; j++)
                            itemData.maintMargin += (itemData.brackets[j].Cap - itemData.brackets[j].Floor) * itemData.brackets[j].MaintenanceMarginRatio;

                        itemData.maintMargin += (itemData.notianalValue - itemData.brackets[i].Floor) * itemData.brackets[i].MaintenanceMarginRatio;

                        break;
                    }
                }

                var walletBalance = Trading.instance.assetDic[AssetTextWalletBalance].Amount;
                var MaintMargin = Trading.instance.assetDic[AssetTextMaintenanceMargin];
                MaintMargin.Amount = Math.Round(itemData.maintMargin, 2);
                var MarginBalance = Trading.instance.assetDic[AssetTextMarginBalance];
                MarginBalance.Amount = Math.Round(walletBalance + itemData.PNL, 2);
                var asset = Trading.instance.assetDic[AssetTextAvailableBalance];
                asset.Amount = Math.Round(walletBalance + itemData.PNL - itemData.InitialMargin, 2);
                asset = Trading.instance.assetDic[AssetTextMarginRatio];
                asset.Amount = Math.Round(MaintMargin.Amount / MarginBalance.Amount * 100, 2);
                Trading.instance.assetsListView.Refresh();
            }
        }
        async void OnAccountUpdates(DataEvent<BinanceFuturesStreamAccountUpdate> data0)
        {
            var data = data0.Data;
            foreach (var balance in data.UpdateData.Balances)
                if (balance.Asset == "USDT")
                {
                    var asset = Trading.instance.assetDic[AssetTextWalletBalance];
                    asset.Amount = balance.WalletBalance;
                    Trading.instance.assetsListView.RefreshObject(asset);
                }

            if (data.UpdateData.Reason == AccountUpdateReason.Order)
                foreach (var position in data.UpdateData.Positions)
                {
                    var itemData = Trading.instance.itemDataDic[position.Symbol] as BinanceItemData;

                    if (position.Quantity == 0)
                    {
                        await itemData.markSub.CloseAsync();

                        Invoke(new Action(() => {
                            itemData.RealEnter = false;
                            itemData.Has = 0;
                            itemData.ClosePNL = 0;

                            Trading_ResetOrderView();

                            var asset = Trading.instance.assetDic[AssetTextMarginRatio];
                            asset.Amount = 0;
                            asset = Trading.instance.assetDic[AssetTextMaintenanceMargin];
                            asset.Amount = 0;
                            asset = Trading.instance.assetDic[AssetTextAvailableBalance];
                            asset.Amount = Trading.instance.assetDic[AssetTextWalletBalance].Amount;
                            asset = Trading.instance.assetDic[AssetTextMarginBalance];
                            asset.Amount = Trading.instance.assetDic[AssetTextWalletBalance].Amount;
                            Trading.instance.assetsListView.Refresh();

                            Trading.instance.mainChart.ChartAreas[0].AxisY2.StripLines.Clear();
                            Trading.instance.mainChart.ChartAreas[0].AxisX.StripLines.Clear();
                            Trading.instance.mainChart.ChartAreas[1].AxisX.StripLines.Clear();
                            Trading.instance.ChangeChartAreaBorderColor();
                        }));
                    }
                    else if (itemData.RealEnter)
                        itemData.Size = position.Quantity;
                    else
                    {
                        var result = await socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates);
                        if (!result.Success)
                            BaseFunctions.ShowError(this);

                        itemData.markSub = result.Data;

                        Invoke(new Action(() => {
                            itemData.RealEnter = true;
                            itemData.RealEnterPrice = position.EntryPrice;
                            itemData.Size = position.Quantity;
                            itemData.PNL = position.UnrealizedPnl;
                            itemData.Has = 1;
                            Trading.instance.CodeListView.RemoveObject(itemData);
                            Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });

                            Trading_ResetOrderView();

                            Trading.instance.ChangeChartAreaBorderColor(itemData.RealPosition == Position.Long ? ColorSet.VolumeTaker : ColorSet.VolumeMaker, false);
                        }));
                    }
                }
        }
        void OnOrderUpdates(DataEvent<BinanceFuturesStreamOrderUpdate> data0)
        {
            var data = data0.Data;
            var itemData = Trading.instance.itemDataDic[data.UpdateData.Symbol] as BinanceItemData;

            if (data.UpdateData.Status == OrderStatus.New)
            {
                itemData.OrderTime = data.UpdateData.UpdateTime;
                itemData.OrderType = data.UpdateData.OriginalType;
                itemData.OrderSide = data.UpdateData.Side;
                itemData.OrderPrice = data.UpdateData.Price;
                itemData.OrderAmount = data.UpdateData.Quantity;
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;
                itemData.ReduceOnly = data.UpdateData.IsReduce;
                itemData.Condition = data.UpdateData.TimeInForce;
                itemData.clientOrderID = data.UpdateData.ClientOrderId;
                itemData.orderID = data.UpdateData.OrderId.ToString();

                if (!itemData.RealEnter)
                {
                    var asset = Trading.instance.assetDic[AssetTextAvailableBalance];
                    asset.Amount -= itemData.OrderPrice * itemData.OrderAmount / itemData.Leverage;
                    Trading.instance.assetsListView.RefreshObject(asset);
                }
            }
            else if (data.UpdateData.Status == OrderStatus.PartiallyFilled)
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;
            else if (data.UpdateData.Status == OrderStatus.Filled || data.UpdateData.Status == OrderStatus.Canceled
                || data.UpdateData.Status == OrderStatus.Rejected || data.UpdateData.Status == OrderStatus.Expired)
            {
                if (data.UpdateData.Status == OrderStatus.Filled && itemData.positionWhenOrder)
                {
                    var resultData = itemData.resultData;

                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((data.UpdateData.AveragePrice / itemData.RealEnterPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + Math.Round(itemData.OrderPrice / data.UpdateData.AveragePrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((itemData.RealEnterPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }

                    
                    if (resultData.Exit_Send_Amount__Diff == default)
                    {
                        resultData.Profit_Rate = resultData.ProfitRate + "%(" + 
                            Math.Round((resultData.EnterCommision / (itemData.RealEnterPrice * data.UpdateData.AccumulatedQuantityOfFilledTrades) + data.UpdateData.Commission / (data.UpdateData.AveragePrice * data.UpdateData.AccumulatedQuantityOfFilledTrades)) * 100, 2) + "%)";
                        resultData.Profit_Value = data.UpdateData.RealizedProfit.ToString(ForDecimalString) + "$(" + (data.UpdateData.RealizedProfit - resultData.EnterCommision - data.UpdateData.Commission).ToString(ForDecimalString) + ")";
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";
                    }

                    Trading.instance.UpdateResult(itemData, itemData.isAuto);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && !itemData.positionWhenOrder) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = itemData.resultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round(Math.Abs(itemData.OrderFilled - itemData.Size) / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";

                        Trading_PlaceOrder(itemData, itemData.RealPosition != Position.Long, true, itemData.isAuto);
                    }
                    else
                    {
                        if (data.UpdateData.AveragePrice != 0)
                        {
                            itemData.RealEnterPrice = data.UpdateData.AveragePrice;
                            resultData.EnterCommision = data.UpdateData.Commission;
                            resultData.Enter_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        }
                        resultData.Enter_Send_Time__Diff = resultData.EnterSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.EnterSendTime).Milliseconds / 1000D, 1) + "s)";
                        resultData.Enter_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                    }

                    (itemData.isAuto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).RefreshObject(resultData);
                }

                if (!itemData.RealEnter && !itemData.positionWhenOrder)
                {
                    var asset = Trading.instance.assetDic[AssetTextAvailableBalance];
                    asset.Amount += itemData.OrderPrice * itemData.OrderAmount / itemData.Leverage;
                    Trading.instance.assetsListView.RefreshObject(asset);
                }
            }
        }

        void Trading_ShowChartAdditional(TradeItemData itemData0)
        {
            var itemData = itemData0 as BinanceItemData;
            if (itemData.Leverage == 0)
            {
                var result = client.FuturesUsdt.GetPositionInformationAsync(itemData.Code).Result;
                if (!result.Success)
                    BaseFunctions.ShowError(this);

                BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

                foreach (var data in result.Data)
                {
                    if (data.Symbol != itemData.Code)
                        break;

                    itemData.Leverage = data.Leverage;
                    itemData.maxNotionalValue = data.MaxNotional;

                    var result1 = client.FuturesUsdt.GetBracketsAsync(itemData.Code).Result;
                    if (!result1.Success)
                        BaseFunctions.ShowError(this);

                    BaseFunctions.BinanceUpdateWeightNow(result1.ResponseHeaders);

                    foreach (var brackets in result1.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                    Invoke(new Action(() =>
                    {
                        if (itemData == Trading.instance.itemDataShowing)
                            leverageTextBox1.Text = "/ " + itemData.maxLeverage;
                    }));
                }
            }
        }
        void Trading_ResetOrderView()
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            leverageTextBox0.Text = itemDataShowing.Leverage.ToString();

            TickAutoSizeButton(true);

            if (itemDataShowing.RealEnter)
            {
                TickMinSizeButton(false);
                if (Trading.instance.positionExitPriceMarket)
                    marketRadioButton.Checked = true;
                else
                    GTCRadioButton.Checked = true;
                ROCheckBox.Checked = true;
            }
            else
            {
                TickMinSizeButton(Trading.instance.miniSizeDefault);
                GTCRadioButton.Checked = true;
                ROCheckBox.Checked = false;
            }

            orderPriceTextBox1.Text = Trading.instance.limitPrice.ToString();
        }
        void Trading_LoadMoreAdditional(Chart chart, bool loadNew)
        {
            var itemData = Trading.instance.itemDataShowing as BinanceItemData;

            var vc = Trading.instance.mainChart.Tag as ChartValues;

            var start = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            DateTime endTime = BaseFunctions.NowTime();
            if (loadNew)
            {
                itemData.showingStickList.Clear();

                end = 0;
                start = end - Trading.instance.baseChartViewSticksSize;
            }
            else
                endTime = DateTime.Parse(chart.Series[0].Points[0].AxisLabel).AddSeconds(-1);

            var size = Trading.instance.baseLoadSticksSize;

            var result = client.FuturesUsdt.Market.GetKlinesAsync(itemData.Code, (KlineInterval)vc.original, null, endTime, size + BaseFunctions.TotalNeedDays - 1).Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            var list = SetRSIAandGetList(result.Data, endTime.AddSeconds(-(int)endTime.TimeOfDay.TotalSeconds % vc.seconds).AddSeconds(-(size - 1) * vc.seconds));

            for (int i = list.Count - 1; i >= 0; i--)
                Trading.instance.InsertFullChartPoint(chart, list[i]);

            if (loadNew)
            {
                itemData.showingStick = list[list.Count - 1];
                itemData.currentPriceWhenLoaded = itemData.showingStick.Price[3];
            }
            itemData.showingStickList.InsertRange(0, list);

            start += list.Count;
            end += list.Count;

            Trading.instance.ZoomX(chart, start, end);
            
            Trading.instance.AdjustChart(chart);
        }
        void Trading_AggONandOFF(TradeItemData itemData, bool on)
        {
            if (on)
            {
                Task.Run(new Action(() =>
                {
                    var result = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdatesAsync(itemData.Code, OnAggregatedTradeUpdates).Result;
                    if (!result.Success)
                        BaseFunctions.ShowError(this);

                    (itemData as BinanceItemData).aggSub = result.Data;

                    itemData.AggOn = true;
                }));
            }
            else
            {
                Task.Run(new Action(() =>
                {
                    socketClient.UnsubscribeAsync((itemData as BinanceItemData).aggSub).Wait();
                    itemData.AggOn = false;
                }));
            }
        }
        void Trading_HoONandOFF(TradeItemData itemData, bool on)
        {
            return;

            if (on)  //100, 500
            {
                Task.Run(new Action(() =>
                {
                    var result = socketClient.FuturesUsdt.SubscribeToPartialOrderBookUpdatesAsync(itemData.Code, (int)Trading.instance.hoChart.Tag, 500, OnHoUpdates).Result;
                    if (!result.Success)
                        Trading.ShowError(this);

                    (itemData as BinanceItemData).hoSub = result.Data;

                    itemData.HoOn = true;
                }));
            }
            else
            {
                Task.Run(new Action(() =>
                {
                    socketClient.UnsubscribeAsync((itemData as BinanceItemData).hoSub).Wait();

                    itemData.HoOn = false;
                }));
            }
        }
        bool Trading_PlaceOrder(TradeItemData itemData0, bool buy, bool market, bool auto, bool autoCancel = true)
        {
            var itemData = itemData0 as BinanceItemData;

            if (itemData.RealEnter && !(itemData.RealPosition == Position.Long ^ buy))
            {
                MessageBox.Show("position again?");
                return false;
            }

            if (itemData.secStick.Price[3] == default)
            {
                BaseFunctions.ShowError(this);
                return false;
            }
            else
                itemData.orderStartClosePrice = itemData.secStick.Price[3];

            itemData.autoCancel = autoCancel;

            decimal priceRate = 0.1m;
            decimal quantity = itemData.RealEnter ?
                Math.Abs(itemData.Size) :
                (int)(itemData.minNotionalValue / itemData.orderStartClosePrice / itemData.minSize + 1) * itemData.minSize;
            int limitPercent = default;

            if (!auto &&
                (!decimal.TryParse(orderPriceTextBox1.Text, out priceRate)
                    || (!itemData.RealEnter && (!decimal.TryParse(orderSizeTextBox1.Text, out quantity) || quantity <= 0))
                    || (!miniSizeCheckBox.Checked && (!int.TryParse(autoSizeTextBox0.Text, out limitPercent) || limitPercent <= 0))))
            {
                BaseFunctions.ShowError(this);
                orderPriceTextBox1.Clear();
                return false;
            }

            var orderType = OrderType.Limit;
            TimeInForce? timeInForce = null;
            decimal? price = buy ?
                    (int)(itemData.orderStartClosePrice * (1 + priceRate / 100) / itemData.hoDiff + 1) * itemData.hoDiff :
                    (int)(itemData.orderStartClosePrice * (1 - priceRate / 100) / itemData.hoDiff) * itemData.hoDiff;

            if (!auto)
            {
                if (!marketRadioButton.Checked && !market)
                {
                    if (PORadioButton.Checked)
                        timeInForce = TimeInForce.GoodTillCrossing;
                    else if (IOCRadioButton.Checked)
                        timeInForce = TimeInForce.ImmediateOrCancel;
                    else
                        timeInForce = TimeInForce.GoodTillCancel;
                }
                else
                {
                    orderType = OrderType.Market;
                    price = null;
                }
            }

            if (!auto && (miniSizeCheckBox.Checked || autoSizeCheckBox.Checked) && !itemData.RealEnter)
            {
                if (miniSizeCheckBox.Checked)
                    quantity = (int)(itemData.minNotionalValue / price / itemData.minSize + 1) * itemData.minSize;
                else
                {
                    quantity = (int)((itemData.ms10secAvg + itemData.md10secAvg) / 2 / 100 / itemData.minSize) * itemData.minSize;

                    var budget = Trading.instance.assetDic[AssetTextAvailableBalance].Amount * limitPercent / 100;
                    if (budget < itemData.minNotionalValue)
                        budget = itemData.minNotionalValue;

                    var limitAmount = (int)(budget / price / itemData.minSize + 1) * itemData.minSize;

                    if (limitAmount < quantity)
                        quantity = limitAmount;
                }
            }

            if (!itemData.RealEnter && (price * quantity > itemData.maxNotionalValue || price * quantity < itemData.minNotionalValue))
            {
                BaseFunctions.ShowError(this);
                return false;
            }

            if (!auto)
                orderSizeTextBox1.Text = quantity.ToString();

            itemData.positionWhenOrder = itemData.RealEnter;

            var result = client.FuturesUsdt.Order.PlaceOrderAsync(
                itemData.Code
                , buy ? OrderSide.Buy : OrderSide.Sell
                , orderType
                , quantity
                , PositionSide.Both
                , timeInForce
                , auto ? true : ROCheckBox.Checked
                , price).Result;
            if (!result.Success)
            {
                BaseFunctions.ShowError(this);
                return false;
            }

            var result2 = client.FuturesUsdt.Order.CancelAllOrdersAfterTimeoutAsync(itemData.Code, TimeSpan.FromSeconds(1)).Result;
            if (!result2.Success)
                BaseFunctions.ShowError(this);

            if (itemData.positionWhenOrder)
            {
                if (itemData.resultData == default)
                {
                    itemData.resultData = new TradeResultData { Code = itemData.Code };
                    itemData.resultData.Position = buy ? "Short" : "Long";
                    (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                }

                itemData.resultData.ExitSendTime = BaseFunctions.NowTime();
            }
            else
            {
                itemData.RealPosition = buy ? Position.Long : Position.Short;

                itemData.resultData = new TradeResultData { Code = itemData.Code };
                itemData.resultData.Position = buy ? "Long" : "Short";
                itemData.resultData.EnterSendTime = BaseFunctions.NowTime();
                (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });

                itemData.Size = 0;
            }

            Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + itemData.secStick.Time.ToString(BaseFunctions.TimeFormat),
                DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 " + (buy ? "매수" : "매도") + "주문전송", DBHelper.conn1OrderHistoryColumn2, "주문가격:" + price + " 주문수량:" + quantity);

            return true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.OnClosed(e);

            Task.Run(new Action(() =>
            {
                socketClient.UnsubscribeAllAsync();
            }));
        }
    }
}
