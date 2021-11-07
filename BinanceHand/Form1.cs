using System;
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

        int weight_limit;
        int weight_now;

        Queue<Task> requestTRTaskQueue = new Queue<Task>();
        CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        Dictionary<KlineInterval, ChartValues> binanceChartValuesDic = new Dictionary<KlineInterval, ChartValues>();

        public Form1()
        {
            InitializeComponent();

            FormClosed += Form1_FormClosed;
            Load += Form1_Load;

            Trading.instance = new Trading(this, @"C:\Users\tmdwn\source\repos\BinanceHand\", Commision.Binance, (0, -1), 20, false);
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

            SetButtonsView();

            SetOrderView();

            SetClientAndKey();

            StartThread();
        }
        void SetButtonsView()
        {
            EventHandler buttonClick = (sender, e) =>
            {
                if (Trading.instance.mainChart != (sender as Button).Tag) 
                    Trading.instance.SetChartNowOrLoad((ChartValues)(sender as Button).Tag);
            };

            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneSecond, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneMinute, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.ThreeMinutes, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.FiveMinutes, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.FifteenMinutes, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.ThirtyMinutes, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.TwoHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.FourHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.SixHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.EightHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.TwelveHour, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneDay, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.ThreeDay, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneWeek, new Button());
            Trading.instance.buttonDic.Add(BaseChartTimeSet.OneMonth, new Button());
            binanceChartValuesDic.Add(KlineInterval.OneMinute, BaseChartTimeSet.OneMinute);
            binanceChartValuesDic.Add(KlineInterval.ThreeMinutes, BaseChartTimeSet.ThreeMinutes);
            binanceChartValuesDic.Add(KlineInterval.FiveMinutes, BaseChartTimeSet.FiveMinutes);
            binanceChartValuesDic.Add(KlineInterval.FifteenMinutes, BaseChartTimeSet.FifteenMinutes);
            binanceChartValuesDic.Add(KlineInterval.ThirtyMinutes, BaseChartTimeSet.ThirtyMinutes);
            binanceChartValuesDic.Add(KlineInterval.OneHour, BaseChartTimeSet.OneHour);
            binanceChartValuesDic.Add(KlineInterval.TwoHour, BaseChartTimeSet.TwoHour);
            binanceChartValuesDic.Add(KlineInterval.FourHour, BaseChartTimeSet.FourHour);
            binanceChartValuesDic.Add(KlineInterval.SixHour, BaseChartTimeSet.SixHour);
            binanceChartValuesDic.Add(KlineInterval.EightHour, BaseChartTimeSet.EightHour);
            binanceChartValuesDic.Add(KlineInterval.TwelveHour, BaseChartTimeSet.TwelveHour);
            binanceChartValuesDic.Add(KlineInterval.OneDay, BaseChartTimeSet.OneDay);
            binanceChartValuesDic.Add(KlineInterval.ThreeDay, BaseChartTimeSet.ThreeDay);
            binanceChartValuesDic.Add(KlineInterval.OneWeek, BaseChartTimeSet.OneWeek);
            binanceChartValuesDic.Add(KlineInterval.OneMonth, BaseChartTimeSet.OneMonth);
            BaseChartTimeSet.OneMinute.index = (int)KlineInterval.OneMinute;
            BaseChartTimeSet.ThreeMinutes.index = (int)KlineInterval.ThreeMinutes;
            BaseChartTimeSet.FiveMinutes.index = (int)KlineInterval.FiveMinutes;
            BaseChartTimeSet.FifteenMinutes.index = (int)KlineInterval.FifteenMinutes;
            BaseChartTimeSet.ThirtyMinutes.index = (int)KlineInterval.ThirtyMinutes;
            BaseChartTimeSet.OneHour.index = (int)KlineInterval.OneHour;
            BaseChartTimeSet.TwoHour.index = (int)KlineInterval.TwoHour;
            BaseChartTimeSet.FourHour.index = (int)KlineInterval.FourHour;
            BaseChartTimeSet.SixHour.index = (int)KlineInterval.SixHour;
            BaseChartTimeSet.EightHour.index = (int)KlineInterval.EightHour;
            BaseChartTimeSet.TwelveHour.index = (int)KlineInterval.TwelveHour;
            BaseChartTimeSet.OneDay.index = (int)KlineInterval.OneDay;
            BaseChartTimeSet.ThreeDay.index = (int)KlineInterval.ThreeDay;
            BaseChartTimeSet.OneWeek.index = (int)KlineInterval.OneWeek;
            BaseChartTimeSet.OneMonth.index = (int)KlineInterval.OneMonth;

            for (var i = 0; i < Trading.instance.buttonDic.Count; i++)
            {
                var pair = Trading.instance.buttonDic.ElementAt(i);
                pair.Value.Tag = pair.Key;
                Trading.instance.SetButton(pair.Value, (pair.Value.Tag as ChartValues).Text, buttonClick);

                if (i == 0)
                {
                    pair.Value.Size = new Size(40, 20);
                    pair.Value.Location = new Point(Trading.instance.mainChart.Location.X + 5, Trading.instance.mainChart.Location.Y + 5);
                }
                else
                {
                    var pairBefore = Trading.instance.buttonDic.ElementAt(i - 1);
                    pair.Value.Size = pairBefore.Value.Size;
                    pair.Value.Location = new Point(pairBefore.Value.Location.X, pairBefore.Value.Location.Y + pairBefore.Value.Height + 5);
                }
            }
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

                        if (weight_now + 50 > weight_limit)
                            Thread.Sleep(60000);
                    }

                    Thread.Sleep(1000);
                }
            }, cancelTokenSource.Token);
        }

        void Form1_Load(object sender, EventArgs e)
        {
            SetItemDataList();

            SubscribeToUserStream(testnet, true);

            GetAccountInfo();

            var result = socketClient.FuturesUsdt.SubscribeToKlineUpdatesAsync(symbolList, KlineInterval.OneMinute, OnKlineUpdates).Result;
            if (!result.Success)
                Trading.ShowError(this);

            Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
        }
        void SetItemDataList()
        {
            var result = client.FuturesUsdt.System.GetExchangeInfoAsync().Result;
            if (!result.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result.ResponseHeaders);

            foreach (var l in result.Data.RateLimits)
            {
                switch (l.Type)
                {
                    case RateLimitType.RequestWeight:
                        if (l.Interval != RateLimitInterval.Minute || l.IntervalNumber != 1)
                            Trading.ShowError(this);

                        weight_limit = l.Limit;
                        break;

                    default:
                        break;
                }
            }
            
            foreach (var s in result.Data.Symbols)
            {
                if (s.Name == "BTCSTUSDT")
                    continue;

                var itemData = new BinanceItemData(s);
                Trading.instance.itemDataList.Add(itemData.Code, itemData);
                symbolList.Add(itemData.Code);
                Trading.instance.CodeListView.AddObject(itemData);

                requestTRTaskQueue.Enqueue(new Task(() =>
                {
                    var result2 = client.FuturesUsdt.Market.GetKlinesAsync(itemData.Code, KlineInterval.OneMinute, null, Trading.instance.NowTime(), 30).Result;

                    if (!result2.Success)
                        Trading.ShowError(this);

                    UpdateWeightNow(result2.ResponseHeaders);

                    var high = decimal.MinValue;
                    var low = decimal.MaxValue;
                    var quoteVolume = 0m;
                    var tradeCount = 0;
                    IBinanceKline lastKline = default;
                    itemData.minStickList.Clear();
                    foreach (var stickReal in result2.Data)
                    {
                        lastKline = stickReal;

                        var stick = new TradeStick();
                        stick.Price[0] = stickReal.High;
                        stick.Price[1] = stickReal.Low;
                        stick.Price[2] = stickReal.Open;
                        stick.Price[3] = stickReal.Close;

                        stick.Ms = stickReal.TakerBuyBaseVolume;
                        stick.Md = stickReal.BaseVolume - stickReal.TakerBuyBaseVolume;

                        stick.Time = stickReal.OpenTime;

                        itemData.minStickList.Add(stick);

                        if (stick.Price[0] > high)
                            high = stick.Price[0];
                        if (stick.Price[1] < low)
                            low = stick.Price[1];

                        quoteVolume += stickReal.QuoteVolume;
                        tradeCount += stickReal.TradeCount;
                    }

                    var seconds = (int)lastKline.CloseTime.Subtract(itemData.minStickList[0].Time).TotalSeconds;
                    itemData.Last30minStickFluc = Math.Round((high / low - 1) * 100, 2);
                    itemData.Budget = (int)(quoteVolume / (seconds * 200));
                    itemData.Count = tradeCount / seconds;

                    itemData.minCheckDone = true;
                }));

                for (int i = 0; i < 15; i++)
                {
                    KlineInterval interval = (KlineInterval)i;

                    if (interval == KlineInterval.OneMinute)
                        continue;

                    requestTRTaskQueue.Enqueue(new Task(() =>
                    {
                        var list = LoadAndCheckSudden(itemData, interval, Trading.instance.NowTime(), interval == KlineInterval.OneDay ? 30 : 10);

                        if (interval == KlineInterval.OneDay)
                        {
                            itemData.daysFromBorn = list.Count + 1;
                            if (list.Count > 0)
                                itemData.Last1dStickFluc = Math.Round((list[list.Count - 1].Price[0] / list[list.Count - 1].Price[1] - 1) * 100, 2);
                        }
                    }));
                }

                //var data = client.FuturesUsdt.ChangeInitialLeverage(itemData.Name, 1);
                //if (!data.Success)
                //    MessageBox.Show("fail");
            }

            if (requestTRTaskQueue.Count == 0)
                Trading.ShowError(this);
        }
        void SubscribeToUserStream(bool testnet, bool testnetFutures)
        {
            var result = ((testnet && !testnetFutures) ? client.Spot.UserStream.StartUserStreamAsync() : client.FuturesUsdt.UserStream.StartUserStreamAsync()).Result;
            if (!result.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result.ResponseHeaders);

            var listenKey = result.Data;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(29 * 60000);

                    var result1 = ((testnet && !testnetFutures) ? client.Spot.UserStream.KeepAliveUserStreamAsync(listenKey) : client.FuturesUsdt.UserStream.KeepAliveUserStreamAsync(listenKey)).Result;
                    if (!result1.Success)
                        Trading.ShowError(this);

                    UpdateWeightNow(result1.ResponseHeaders);

                    var count = 0;
                    foreach (var item in Trading.instance.itemDataList.Values)
                        if (item.Last30minStickFluc > 3)
                            count++;

                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, Trading.instance.NowTime().ToString(Trading.instance.secTimeFormat),
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
                Trading.ShowError(this);
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

            var task = client.FuturesUsdt.Account.GetAccountInfoAsync();
            task.Wait();

            var result = task.Result;
            if (!result.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result.ResponseHeaders);

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
                    var itemData = (BinanceItemData)Trading.instance.itemDataList[s.Symbol];

                    itemData.position = true;
                    itemData.Has = 1;

                    var result1 = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates).Result;
                    if (!result1.Success)
                        Trading.ShowError(this);

                    itemData.markSub = result1.Data;

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2);
                    itemData.maintMargin = s.MaintMargin;

                    Trading.instance.CodeListView.RemoveObject(itemData);
                    Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });
                }
            }

            var result2 = client.FuturesUsdt.GetPositionInformationAsync().Result;
            if (!result2.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result2.ResponseHeaders);

            foreach (var s in result2.Data)
            {
                if (s.MarginType == FuturesMarginType.Isolated)
                {
                    var result3 = client.FuturesUsdt.ChangeMarginTypeAsync(s.Symbol, FuturesMarginType.Cross).Result;
                    if (!result3.Success)
                        Trading.ShowError(this);

                    UpdateWeightNow(result3.ResponseHeaders);
                }

                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataList[s.Symbol];

                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    if (s.Quantity < 0) itemData.LorS = false;
                    itemData.Size = s.Quantity;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.EntryPrice = s.EntryPrice;
                    itemData.PNL = Math.Round(s.UnrealizedPnl, 2);
                    itemData.ClosePNL = itemData.PNL;
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);
                    itemData.maxNotionalValue = s.MaxNotional;

                    Trading.instance.CodeListView.RefreshObject(itemData);

                    var result4 = client.FuturesUsdt.GetBracketsAsync(s.Symbol).Result;
                    if (!result4.Success)
                        Trading.ShowError(this);

                    UpdateWeightNow(result4.ResponseHeaders);

                    foreach (var brackets in result4.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                }
            }
        }

        List<TradeStick> LoadAndCheckSudden(TradeItemData itemData, KlineInterval interval, DateTime closeTime, int limit)
        {
            var result2 = client.FuturesUsdt.Market.GetKlinesAsync(itemData.Code, interval, null, closeTime, limit).Result;

            if (!result2.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result2.ResponseHeaders);

            var list = new List<TradeStick>();

            foreach (var stickReal in result2.Data)
            {
                var stick = new TradeStick();
                stick.Price[0] = stickReal.High;
                stick.Price[1] = stickReal.Low;
                stick.Price[2] = stickReal.Open;
                stick.Price[3] = stickReal.Close;

                stick.Ms = stickReal.TakerBuyBaseVolume;
                stick.Md = stickReal.BaseVolume - stickReal.TakerBuyBaseVolume;

                stick.Time = stickReal.OpenTime;

                list.Add(stick);
            }

            var last = list.Last();
            list.Remove(last);

            var name = binanceChartValuesDic[interval].Text;
            if (Trading.instance.CheckSuddenBurstOfMin(false, list, last, itemData))
            {
                itemData.Suddens++;
                itemData.SuddenNames += name + ",";
            }
            else if (itemData.SuddenNames.Contains(name))
            {
                itemData.Suddens--;
                itemData.SuddenNames.Replace(name + ",", "");
            }

            return list;
        }

        void UpdateWeightNow(IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
        {
            var weights = headers.Where(p => p.Key == "x-mbx-used-weight" || p.Key == "x-mbx-used-weight-1m" || p.Key == "X-MBX-USED-WEIGHT-1M").ToList();

            weight_now = int.Parse(weights[0].Value.ElementAt(0).ToString());
        }

        void TickMinSizeButton(bool on)
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            miniSizeCheckBox.Checked = on;
            if (itemDataShowing.position)
                orderSizeTextBox1.Text = Math.Abs(itemDataShowing.Size).ToString();
            else if (itemDataShowing.secStick.Price[3] != 0)
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
            if (itemDataShowing.position)
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
            var itemData = Trading.instance.itemDataList[data.Symbol] as BinanceItemData;

            if (!itemData.minCheckDone)
                return;

            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;

                Trading.instance.UpdateReqRcv(Trading.instance.KlineRcvTextBox, Trading.instance.KlineRcv + 1);
            }

            if (itemData.Real != 0)
                return;

            if (itemData.minStick == default)
                itemData.minStick = new TradeStick();

            itemData.minStick.Price[0] = data.Data.High;
            itemData.minStick.Price[1] = data.Data.Low;
            itemData.minStick.Price[2] = data.Data.Open;
            itemData.minStick.Price[3] = data.Data.Close;
            itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
            itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
            itemData.minStick.Time = data.Data.OpenTime;
            itemData.minStick.TCount = data.Data.TradeCount;

            if (itemData.minStickList.Count != 0 && itemData.minStick.Time.Subtract(itemData.minStickList.Last().Time).TotalMinutes == 0)
                itemData.minStickList.RemoveAt(itemData.minStickList.Count - 1);

            Trading.instance.CheckSuddenBurstOfMin(false, itemData.minStickList, itemData.minStick, itemData);

            if (data.Data.Final)
            {
                Trading.instance.ChangeRealIf(itemData, data.Data.High / data.Data.Low);

                itemData.minStickList.Add(itemData.minStick);

                var high = decimal.MinValue;
                var low = decimal.MaxValue;
                var baseVolume = 0m;
                var tradeCount = 0;
                DateTime firstTime = default;
                for (int i = 1; i < 30; i++)
                {
                    if (i > itemData.minStickList.Count)
                        break;

                    var stick = itemData.minStickList[itemData.minStickList.Count - i];

                    if (stick.Price[0] > high)
                        high = stick.Price[0];
                    if (stick.Price[1] < low)
                        low = stick.Price[1];

                    baseVolume += stick.Ms + stick.Md;
                    tradeCount += stick.TCount;

                    firstTime = stick.Time;
                }

                var seconds = (int)data.Data.CloseTime.Subtract(firstTime).TotalSeconds;
                itemData.Last30minStickFluc = Math.Round((high / low - 1) * 100, 2);
                itemData.Budget = (int)(baseVolume * (high + low) / (seconds * 400));
                itemData.Count = tradeCount / seconds;

                var closeTime = data.Data.OpenTime.AddMinutes(1);

                if (closeTime.Minute % 3 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.ThreeMinutes, data.Data.CloseTime, 10); }));
                if (closeTime.Minute % 5 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.FiveMinutes, data.Data.CloseTime, 10); }));
                if (closeTime.Minute % 15 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.FifteenMinutes, data.Data.CloseTime, 10); }));
                if (closeTime.Minute % 30 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.ThirtyMinutes, data.Data.CloseTime, 10); }));
                if (closeTime.Minute % 60 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.OneHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 2 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.TwoHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 4 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.FourHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 6 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.SixHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 8 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.EightHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 12 == 0)
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.TwelveHour, data.Data.CloseTime, 10); }));
                if (closeTime.Hour % 24 == 0)
                {
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.OneDay, data.Data.CloseTime, 10); }));
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.ThreeDay, data.Data.CloseTime, 10); }));
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.OneWeek, data.Data.CloseTime, 10); }));
                    requestTRTaskQueue.Enqueue(new Task(() => { LoadAndCheckSudden(itemData, KlineInterval.OneMonth, data.Data.CloseTime, 10); }));
                }
            }
        }
        void OnAggregatedTradeUpdates(DataEvent<BinanceStreamAggregatedTrade> data0)
        {
            var data = data0.Data;
            var itemData = Trading.instance.itemDataList[data.Symbol] as BinanceItemData;

            if (itemData.Real == 0)
                return;

            itemData.hoCheGu = data.BuyerIsMaker ? (short)2 : (short)1;
            itemData.newPrice = data.Price;
            itemData.newTime = data.TradeTime;
            itemData.newAmt = data.Quantity;

            if (itemData.AggFirst)
            {
                itemData.AggFirst = false;

                Trading.instance.UpdateReqRcv(Trading.instance.aggRcvTextBox, Trading.instance.aggRcv + 1);

                Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, Trading.instance.NowTime().ToString("yyyy-MM-dd") + " " + itemData.newTime,
                    DBHelper.conn1DetectHistoryColumn1, itemData.Code + "~첫체결", DBHelper.conn1DetectHistoryColumn2, " 종가:" + itemData.newPrice);
            }

            if (itemData.isChartShowing && Trading.instance.mainChart.InvokeRequired)
                Invoke(new Action(() => { Trading.instance.AggMain(itemData); }));
            else
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
            var itemData = Trading.instance.itemDataList[data.Symbol.ToUpper()] as BinanceItemData;

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
            var itemData = Trading.instance.itemDataList[data.Symbol] as BinanceItemData;

            if (itemData.position)
            {
                itemData.MarkPrice = Math.Round(data.MarkPrice, 2);
                itemData.notianalValue = data.MarkPrice * Math.Abs(itemData.Size);
                itemData.InitialMargin = Math.Round(itemData.notianalValue / itemData.Leverage, 2);
                itemData.PNL = Math.Round((itemData.MarkPrice - itemData.EntryPrice) * itemData.Size, 2);
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
                    var itemData = Trading.instance.itemDataList[position.Symbol] as BinanceItemData;

                    if (position.Quantity == 0)
                    {
                        await itemData.markSub.CloseAsync();

                        Invoke(new Action(() => {
                            itemData.position = false;
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
                            Trading.instance.ChangeChartAreaBorderColor();
                        }));
                    }
                    else if (itemData.position)
                        itemData.Size = position.Quantity;
                    else
                    {
                        var result = await socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates);
                        if (!result.Success)
                            Trading.ShowError(this);

                        itemData.markSub = result.Data;

                        Invoke(new Action(() => {
                            itemData.position = true;
                            itemData.EntryPrice = position.EntryPrice;
                            itemData.Size = position.Quantity;
                            itemData.PNL = position.UnrealizedPnl;
                            itemData.Has = 1;
                            Trading.instance.CodeListView.RemoveObject(itemData);
                            Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });

                            Trading_ResetOrderView();

                            Trading.instance.ChangeChartAreaBorderColor(itemData.LorS ? ColorSet.VolumeTaker : ColorSet.VolumeMaker, false);
                        }));
                    }
                }
        }
        void OnOrderUpdates(DataEvent<BinanceFuturesStreamOrderUpdate> data0)
        {
            var data = data0.Data;
            var itemData = Trading.instance.itemDataList[data.UpdateData.Symbol] as BinanceItemData;

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

                if (!itemData.position)
                {
                    var asset = Trading.instance.assetDic[AssetTextAvailableBalance];
                    asset.Amount -= (itemData.OrderPrice * itemData.OrderAmount) / itemData.Leverage;
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
                    TradeResultData resultData;
                    if (Trading.instance.realHandResultListView.Items.Count == 0)
                    {
                        resultData = new TradeResultData { Code = itemData.Code, Name = itemData.Name };
                        Trading.instance.realHandResultListView.InsertObjects(0, new List<TradeResultData> { resultData });
                    }
                    else
                        resultData = Trading.instance.realHandResultListView.GetModelObject(0) as TradeResultData;
                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + Math.Round((data.UpdateData.AveragePrice / itemData.OrderPrice - 1) * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((data.UpdateData.AveragePrice / itemData.EntryPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + Math.Round((itemData.OrderPrice / data.UpdateData.AveragePrice - 1) * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((itemData.EntryPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }
                    resultData.Profit_Rate = resultData.ProfitRate + "%";
                    resultData.Profit_Value = data.UpdateData.RealizedProfit.ToString(ForDecimalString) + "$";
                    resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(Trading.instance.timeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";
                    if (resultData.Exit_Send_Amount__Diff == default)
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round((data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount - 1) * 100, 0) + "%)";

                    Trading.instance.UpdateResult(itemData, itemData.isAuto);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && !itemData.positionWhenOrder) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = Trading.instance.realHandResultListView.GetModelObject(0) as TradeResultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round((Math.Abs(itemData.Size) / itemData.OrderAmount - 1) * 100, 0) + "%)";

                        Trading_PlaceOrder(itemData, !itemData.LorS, true, itemData.isAuto);
                    }
                    else
                    {
                        if (data.UpdateData.AveragePrice != 0)
                        {
                            itemData.EntryPrice = data.UpdateData.AveragePrice;
                            resultData.Enter_Send_Price__Diff = itemData.OrderPrice.ToString(ForDecimalString) + "(" + 
                                Math.Round(((data.UpdateData.Side == OrderSide.Buy ? data.UpdateData.AveragePrice / itemData.OrderPrice : itemData.OrderPrice / data.UpdateData.AveragePrice) - 1) * 100, 2) + "%)";
                        }
                        resultData.Enter_Send_Time__Diff = resultData.EnterSendTime.ToString(Trading.instance.timeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.EnterSendTime).Milliseconds / 1000D, 1) + "s)";
                        resultData.Enter_Send_Amount__Diff = itemData.OrderAmount.ToString(ForDecimalString) + "(" + Math.Round((Math.Abs(itemData.Size) / itemData.OrderAmount - 1) * 100, 0) + "%)";
                    }

                    Trading.instance.realHandResultListView.RefreshObject(resultData);
                }

                if (!itemData.position && !itemData.positionWhenOrder)
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
                    Trading.ShowError(this);

                UpdateWeightNow(result.ResponseHeaders);

                foreach (var data in result.Data)
                {
                    if (data.Symbol != itemData.Code)
                        break;

                    itemData.Leverage = data.Leverage;
                    itemData.maxNotionalValue = data.MaxNotional;

                    var result1 = client.FuturesUsdt.GetBracketsAsync(itemData.Code).Result;
                    if (!result1.Success)
                        Trading.ShowError(this);

                    UpdateWeightNow(result1.ResponseHeaders);

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

            if (itemDataShowing.position)
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
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            KlineInterval interval = (KlineInterval)(Trading.instance.mainChart.Tag as ChartValues).index;
            List<TradeStick> list = new List<TradeStick>();
            TradeStick stick = new TradeStick();

            if (Trading.instance.mainChart.Tag == BaseChartTimeSet.OneMinute)
            {
                interval = KlineInterval.OneMinute;
                list = itemDataShowing.minStickList;
                stick = itemDataShowing.minStick;
            }

            var start = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            DateTime? endTime = null;
            if (loadNew)
            {
                Trading.instance.ClearChart(chart);
                list.Clear();

                start = -Trading.instance.baseChartViewSticksSize + 2;
                end = 0;

                endTime = Trading.instance.NowTime();
            }

            var limit = Trading.instance.baseLoadSticksSize;
            var result = client.FuturesUsdt.Market.GetKlinesAsync(itemDataShowing.Code, interval, null, endTime, limit).Result;
            if (!result.Success)
                Trading.ShowError(this);

            UpdateWeightNow(result.ResponseHeaders);

            var klines = result.Data;
            TradeStick newStick;

            var first = true;
            limit = 0;
            foreach (var kline in klines.Reverse())
            {
                limit++;

                newStick = new TradeStick();
                newStick.Price[0] = kline.High;
                newStick.Price[1] = kline.Low;
                newStick.Price[2] = kline.Open;
                newStick.Price[3] = kline.Close;
                newStick.Ms = kline.TakerBuyBaseVolume;
                newStick.Md = kline.BaseVolume - kline.TakerBuyBaseVolume;
                newStick.Time = kline.OpenTime;
                newStick.TCount = kline.TradeCount;
                Trading.instance.InsertFullChartPoint(chart, newStick);
                if (first && loadNew)
                {
                    first = false;
                    TradeStick.Copy(newStick, stick);
                }
                else
                    list.Insert(0, newStick);
            }

            start += limit;
            end += limit;

            chart.ChartAreas[0].RecalculateAxesScale();
            chart.ChartAreas[1].RecalculateAxesScale();

            chart.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
            
            chart.ChartAreas[0].RecalculateAxesScale();
            chart.ChartAreas[1].RecalculateAxesScale();
            
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
                        Trading.ShowError(this);

                    (itemData as BinanceItemData).aggSub = result.Data;
                }));
            }
            else
                socketClient.UnsubscribeAsync((itemData as BinanceItemData).aggSub);
        }
        void Trading_HoONandOFF(TradeItemData itemData, bool on)
        {
            if (on)  //100, 500
            {
                Task.Run(new Action(() =>
                {
                    var result = socketClient.FuturesUsdt.SubscribeToPartialOrderBookUpdatesAsync(itemData.Code, (int)Trading.instance.hoChart.Tag, 500, OnHoUpdates).Result;
                    if (!result.Success)
                        Trading.ShowError(this);

                    (itemData as BinanceItemData).hoSub = result.Data;

                    itemData.HoOn = on;
                }));
            }
            else
            {
                Task.Run(new Action(() =>
                {
                    socketClient.UnsubscribeAsync((itemData as BinanceItemData).hoSub).Wait();

                    itemData.HoOn = on;
                }));
            }
        }
        void Trading_PlaceOrder(TradeItemData itemData0, bool buy, bool market, bool auto, bool autoCancel = true)
        {
            var itemData = itemData0 as BinanceItemData;

            if (itemData.position && !(itemData.LorS ^ buy))
            {
                MessageBox.Show("position again?");
                return;
            }

            if (itemData.secStick.Price[3] == default)
            {
                Trading.ShowError(this);
                return;
            }
            else
                itemData.orderStartClosePrice = itemData.secStick.Price[3];

            itemData.autoCancel = autoCancel;

            decimal priceRate = 0.1m;
            decimal quantity = itemData.position ?
                Math.Abs(itemData.Size) :
                (int)(itemData.minNotionalValue / itemData.orderStartClosePrice / itemData.minSize + 1) * itemData.minSize;
            int limitPercent = default;

            if (!auto &&
                (!decimal.TryParse(orderPriceTextBox1.Text, out priceRate)
                    || (!itemData.position && (!decimal.TryParse(orderSizeTextBox1.Text, out quantity) || quantity <= 0))
                    || (!miniSizeCheckBox.Checked && (!int.TryParse(autoSizeTextBox0.Text, out limitPercent) || limitPercent <= 0))))
            {
                Trading.ShowError(this);
                orderPriceTextBox1.Clear();
                return;
            }

            var orderType = OrderType.Limit;
            TimeInForce? timeInForce = null;
            decimal? price = buy ?
                    (int)(itemData.orderStartClosePrice * (1 + priceRate / 100) / itemData.priceTickSize + 1) * itemData.priceTickSize :
                    (int)(itemData.orderStartClosePrice * (1 - priceRate / 100) / itemData.priceTickSize) * itemData.priceTickSize;

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

            if (!auto && (miniSizeCheckBox.Checked || autoSizeCheckBox.Checked) && !itemData.position)
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

            if (!itemData.position && (price * quantity > itemData.maxNotionalValue || price * quantity < itemData.minNotionalValue))
            {
                Trading.ShowError(this);
                return;
            }

            if (!auto)
                orderSizeTextBox1.Text = quantity.ToString();

            itemData.positionWhenOrder = itemData.position;

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
                Trading.ShowError(this);

            var result2 = client.FuturesUsdt.Order.CancelAllOrdersAfterTimeoutAsync(itemData.Code, TimeSpan.FromSeconds(1)).Result;
            if (!result2.Success)
                Trading.ShowError(this);

            if (itemData.positionWhenOrder)
            {
                if (itemData.resultData == default)
                {
                    itemData.resultData = new TradeResultData { Code = itemData.Code };
                    (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                }

                itemData.resultData.ExitSendTime = Trading.instance.NowTime();
            }
            else
            {
                itemData.LorS = buy;

                itemData.resultData = new TradeResultData { Code = itemData.Code };
                itemData.resultData.EnterSendTime = Trading.instance.NowTime();
                (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });

                itemData.Size = 0;
            }

            Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, Trading.instance.NowTime().ToString(Trading.instance.secTimeFormat) + "~" + itemData.secStick.Time.ToString(Trading.instance.secTimeFormat),
                DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 " + (buy ? "매수" : "매도") + "주문전송", DBHelper.conn1OrderHistoryColumn2, "주문가격:" + price + " 주문수량:" + quantity);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.OnClosed(e);

            Task.Run(new Action(() =>
            {
                socketClient.UnsubscribeAllAsync();
            }));

            if (cancelTokenSource != null)
                cancelTokenSource.Cancel();

            Trading.instance.dbHelper.Close();
        }
    }
}
