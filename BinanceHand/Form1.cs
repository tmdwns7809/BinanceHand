using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using Binance.Net;
using Binance.Net.Enums;
using Binance.Net.Objects.Spot;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Sockets;
using Binance.Net.Objects.Spot.MarketStream;
using Binance.Net.Interfaces;
using System.Windows.Forms.DataVisualization.Charting;
using BrightIdeasSoftware;
using Binance.Net.Objects.Futures.MarketStream;
using Binance.Net.Objects.Futures.UserStream;
using System.Drawing.Imaging;
using IrrKlang;
using System.Drawing.Drawing2D;
using System.Data.SQLite;
using TradingLibrary.Trading;
using TradingLibrary;

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
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        delegate void OrderUpdates(BinanceFuturesStreamOrderUpdate data, BinanceItemData itemData);
        OrderUpdates orderUpdates;
        delegate void AccountUpdates(BinanceFuturesStreamAccountUpdate data);
        AccountUpdates accountUpdates;
        delegate void MarkUpdates(BinanceFuturesUsdtStreamMarkPrice data, BinanceItemData itemData);
        MarkUpdates markUpdates;
        delegate void AggUpdates(BinanceStreamAggregatedTrade data, BinanceItemData itemData);
        AggUpdates aggUpdates;
        delegate void KlineUpdates(IBinanceStreamKlineData data, BinanceItemData itemData);
        KlineUpdates klineUpdates;
        delegate void HoUpdates(IBinanceFuturesEventOrderBook data, BinanceItemData itemData);
        HoUpdates hoUpdates;

        List<string> symbolList = new List<string>();

        public Form1()
        {
            InitializeComponent();

            FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            Trading.instance = new Trading(this, @"C:\Users\tmdwn\source\repos\BinanceHand\", 0.1m, (0, -1), 20);
            Trading.instance.HoONandOFF += Trading_HoONandOFF;
            Trading.instance.AggONandOFF += Trading_AggONandOFF;
            Trading.instance.ShowChartAdditional += Trading_ShowChartAdditional;
            Trading.instance.ResetOrderView += Trading_ResetOrderView;
            Trading.instance.LoadMoreAdditional += Trading_LoadMoreAdditional;
            Trading.instance.PlaceOrder += Trading_PlaceOrder;

            SetOrderView();

            SetClientAndKey();

            Load += Form1_Load;
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
                GTCRadioButton.Size, new Point(orderPriceTextBox2.Location.X + orderPriceTextBox2.Size.Width + 10, orderPriceTextBox2.Location.Y));

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
                PORadioButton.Size, new Point(miniSizeCheckBox.Location.X + miniSizeCheckBox.Size.Width + 10, miniSizeCheckBox.Location.Y));

            Trading.instance.SetRadioButton_CheckBox(autoSizeCheckBox, "Auto",
                new Size(50, GTCRadioButton.Height),
                new Point(orderSizeTextBox0.Location.X + 20, orderSizeTextBox0.Location.Y + orderSizeTextBox0.Size.Height + 10 + (autoSizeTextBox0.Size.Height - autoSizeCheckBox.Size.Height) / 2));
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
                    clientOption.ApiCredentials = new ApiCredentials("91a72629c127988a0f3fd76d0b0e8bd07d2897ce772f3d46950e778024e44779", "8f3064b7fd15884a575e3a4f8646aac9e668931eab20ac975bd8bf372ff2b324");
                    socketOption.BaseAddressUsdtFutures = "wss://stream.binancefuture.com";
                    socketOption.BaseAddressCoinFutures = "wss://stream.binancefuture.com";
                    socketOption.BaseAddress = "wss://stream.binancefuture.com";
                    socketOption.ApiCredentials = new ApiCredentials("91a72629c127988a0f3fd76d0b0e8bd07d2897ce772f3d46950e778024e44779", "8f3064b7fd15884a575e3a4f8646aac9e668931eab20ac975bd8bf372ff2b324");
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
            {   // 2 api: 1FTacwabmgbRuWQp69WtsdfI0xTC6W8WdINGz6Y63t82quT3uWSfda5sywM6hahD secret: F0XsXiKbV5oH7K5yynAhugTuFSEKESCXN6TSjLHT7fKADavdJ3pEJdrafqeSzuAX
                // 1 api: DpLZEH9lDIuoYvs5L9YcDshSXW1rN9Rlw6OMj7zVQwR7wuBGt9vtyntEDNUfUb50 secret: xXhj8yvUrLgVxlO4BioadC4RQSoydDLR2UFIaMcqBCdX1WM2LjVsM3cxC1Y9lgVJ
                clientOption.ApiCredentials = new ApiCredentials("1FTacwabmgbRuWQp69WtsdfI0xTC6W8WdINGz6Y63t82quT3uWSfda5sywM6hahD", "F0XsXiKbV5oH7K5yynAhugTuFSEKESCXN6TSjLHT7fKADavdJ3pEJdrafqeSzuAX");
                clientOption.AutoTimestamp = false;
                clientOption.TradeRulesBehaviour = TradeRulesBehaviour.None;
                socketOption.ApiCredentials = new ApiCredentials("1FTacwabmgbRuWQp69WtsdfI0xTC6W8WdINGz6Y63t82quT3uWSfda5sywM6hahD", "F0XsXiKbV5oH7K5yynAhugTuFSEKESCXN6TSjLHT7fKADavdJ3pEJdrafqeSzuAX");
                socketOption.AutoReconnect = true;
                socketOption.ReconnectInterval = TimeSpan.FromMinutes(1);
                //clientOption.LogVerbosity = LogVerbosity.Debug;
                //clientOption.LogWriters = new List<TextWriter> { Console.Out };
            }

            client = new BinanceClient(clientOption);
            socketClient = new BinanceSocketClient(socketOption);
        }
        void Form1_Load(object sender, EventArgs e)
        {
            SetItemDataList();

            SubscribeToUserStream(testnet, true);

            GetAccountInfo();

            aggUpdates += new AggUpdates(OnAggregatedTradeUpdates);
            klineUpdates += new KlineUpdates(OnKlineUpdates);
            markUpdates += new MarkUpdates(OnMarkPriceUpdates);
            accountUpdates = new AccountUpdates(OnAccountUpdates);
            orderUpdates = new OrderUpdates(OnOrderUpdates);
            hoUpdates = new HoUpdates(OnHoUpdates);

            socketClient.FuturesUsdt.SubscribeToKlineUpdates(symbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, Trading.instance.itemDataList[data.Symbol]); });
            Trading.instance.KlineReqTextBox.Text = "/" + symbolList.Count + "(K)";
        }
        void SetItemDataList()
        {
            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
                if (s.Name == "BTCSTUSDT")
                    continue;

                var itemData = new BinanceItemData(s);
                Trading.instance.itemDataList.Add(itemData.Code, itemData);
                symbolList.Add(itemData.Code);
                Trading.instance.CodeListView.AddObject(itemData);

                //var data = client.FuturesUsdt.ChangeInitialLeverage(itemData.Name, 1);
                //if (!data.Success)
                //    MessageBox.Show("fail");
            }
        }
        void SubscribeToUserStream(bool testnet, bool testnetFutures)
        {
            string listenKey;

            if (testnet)
            {
                if (testnetFutures)
                    listenKey = client.FuturesUsdt.UserStream.StartUserStream().Data;
                else
                    listenKey = client.Spot.UserStream.StartUserStream().Data;
            }
            else
            {
                var r = client.FuturesUsdt.UserStream.StartUserStream();
                listenKey = r.Data;
            }

            Task.Run(async () =>
            {
                while (true)
                {
                    if (testnet)
                    {
                        if (testnetFutures)
                            await client.FuturesUsdt.UserStream.KeepAliveUserStreamAsync(listenKey);
                        else
                            await client.Spot.UserStream.KeepAliveUserStreamAsync(listenKey);
                    }
                    else
                        await client.FuturesUsdt.UserStream.KeepAliveUserStreamAsync(listenKey);

                    await Task.Delay(TimeSpan.FromMinutes(30));
                }
            }, tokenSource.Token);

            if (testnet)
            {
                if (testnetFutures)
                    socketClient.FuturesUsdt.SubscribeToUserDataUpdates(listenKey,
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        },
                        data => { BeginInvoke(accountUpdates, data); },
                        data => { BeginInvoke(orderUpdates, data, Trading.instance.itemDataList[data.UpdateData.Symbol]); },
                        data => {
                            var a = data;
                        });
                else
                    socketClient.Spot.SubscribeToUserDataUpdates(listenKey, null, null, null, null);
            }
            else
                socketClient.FuturesUsdt.SubscribeToUserDataUpdates(listenKey,
                        null,
                        data => {
                            var a = data;
                        },
                        data => { BeginInvoke(accountUpdates, data); },
                        data => { BeginInvoke(orderUpdates, data, Trading.instance.itemDataList[data.UpdateData.Symbol]); },
                        data => {
                            var a = data;
                        });
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

            var ga = client.FuturesUsdt.Account.GetAccountInfo();
            foreach (var s in ga.Data.Assets)
            {
                if (s.Asset == "USDT")
                {
                    Trading.instance.assetDic[AssetTextMarginBalance].Amount = s.MarginBalance;
                    Trading.instance.assetDic[AssetTextAvailableBalance].Amount = s.AvailableBalance;
                    Trading.instance.assetDic[AssetTextWalletBalance].Amount = s.WalletBalance;
                }
            }
            foreach (var s in ga.Data.Positions)
            {
                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataList[s.Symbol];

                    itemData.position = true;
                    itemData.Has = 1;

                    itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Code, 3000,
                        data => {
                            BeginInvoke(markUpdates, data, itemData);
                        }).Data;

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2);
                    itemData.maintMargin = s.MaintMargin;

                    Trading.instance.CodeListView.RemoveObject(itemData);
                    Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });
                }
            }
            var gp = client.FuturesUsdt.GetPositionInformation();
            foreach (var s in gp.Data)
            {
                if (s.MarginType == FuturesMarginType.Isolated)
                {
                    var cm = client.FuturesUsdt.ChangeMarginType(s.Symbol, FuturesMarginType.Cross);
                    if (!cm.Success)
                        MessageBox.Show("마진 타입 변경 실패");
                }

                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataList[s.Symbol];

                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    if (s.PositionAmount < 0) itemData.LorS = false;
                    itemData.Size = s.PositionAmount;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.EntryPrice = s.EntryPrice;
                    itemData.PNL = Math.Round(s.UnrealizedProfit, 2);
                    itemData.ClosePNL = itemData.PNL;
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                    foreach (var brackets in client.FuturesUsdt.GetBrackets(s.Symbol).Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxNotionalValue = s.MaxNotionalValue;
                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;

                    Trading.instance.CodeListView.RefreshObject(itemData);
                }
            }
        }
        void OnKlineUpdates(IBinanceStreamKlineData data, BinanceItemData itemData)
        {
            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;

                Trading.instance.KlineRcv++;
                Trading.instance.KlineRcvTextBox.Text = Trading.instance.KlineRcv.ToString();
            }

            if (data.Data.Final)
            {
                if (data.Data.High / data.Data.Low > 1.02m)
                {
                    if (itemData.Real == 0)
                    {
                        Trading.instance.SetAgg(itemData, true);
                        Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, DateTime.UtcNow.ToString(Trading.instance.secTimeFormat), 
                            DBHelper.conn1DetectHistoryColumn1, itemData.Code, DBHelper.conn1DetectHistoryColumn2, "Real 1 in : 전분폭 > 2%");
                    }

                    itemData.Real1Condition = 0;
                }
                else if (itemData.Real1Condition < 15)
                    itemData.Real1Condition++;
                else if (itemData.Real == 1 && !itemData.isChartShowing && !itemData.simulEnterOrder)
                {
                    Trading.instance.SetAgg(itemData, false);
                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, DateTime.UtcNow.ToString(Trading.instance.secTimeFormat),
                        DBHelper.conn1DetectHistoryColumn1, itemData.Code, DBHelper.conn1DetectHistoryColumn2, "Real out : 15연속 전분폭 < 2%");
                }

                if (itemData.Real == 2)
                {
                    if (itemData.Real2Condition > 5)
                    {
                        itemData.Real = 1;
                        Trading.instance.CodeListView.RefreshObject(itemData);
                        Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, DateTime.UtcNow.ToString(Trading.instance.secTimeFormat),
                            DBHelper.conn1DetectHistoryColumn1, itemData.Code, DBHelper.conn1DetectHistoryColumn2, "Real 2 out : 5분연속 기준 미달");

                        List<TradeStick> secList = new List<TradeStick>();
                        secList.AddRange(itemData.oldSecStickList);
                        secList.AddRange(itemData.secStickList);
                        Trading.instance.dbHelper.SaveSticksCSVData(itemData.Code + "_" + secList[0].Time.ToString(Trading.instance.secTimeFormatSCV), secList);
                    }
                    else
                        itemData.Real2Condition++;
                }

                if (itemData.isChartShowing && itemData.Real == 0 && Trading.instance.chartNow.TabIndex == Trading.instance.minChart.TabIndex)
                {
                    itemData.minStick = new TradeStick();
                    itemData.minStick.Price[0] = data.Data.High;
                    itemData.minStick.Price[1] = data.Data.Low;
                    itemData.minStick.Price[2] = data.Data.Open;
                    itemData.minStick.Price[3] = data.Data.Close;
                    itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                    itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                    itemData.minStick.Time = data.Data.OpenTime;
                    itemData.minStick.TCount = data.Data.TradeCount;
                    itemData.minStickList.Add(itemData.minStick);

                    Trading.instance.UpdateChartPoint(Trading.instance.minChart, itemData.minStick);
                }
            }
            else if (itemData.isChartShowing && itemData.Real == 0 && Trading.instance.chartNow.TabIndex == Trading.instance.minChart.TabIndex)
            {
                if (itemData.minStick.Time != data.Data.OpenTime)
                {
                    itemData.minStick = new TradeStick();
                    itemData.minStick.Price[2] = data.Data.Open;
                    itemData.minStick.Price[3] = itemData.minStick.Price[2];
                    itemData.minStick.Time = data.Data.OpenTime;
                    Trading.instance.AddStartChartPoint(Trading.instance.minChart, itemData.minStick);
                }

                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;

                Trading.instance.UpdateChartPoint(Trading.instance.minChart, itemData.minStick);
            }
        }
        void Trading_AggONandOFF(TradeItemData itemData, bool on)
        {
            if (on)
                (itemData as BinanceItemData).aggSub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Code,
                    data2 => { BeginInvoke(aggUpdates, data2, Trading.instance.itemDataList[data2.Symbol]); }).Data;
            else
                socketClient.Unsubscribe((itemData as BinanceItemData).aggSub);
        }
        void OnAggregatedTradeUpdates(BinanceStreamAggregatedTrade data, BinanceItemData itemData)
        {
            itemData.hoCheGu = data.BuyerIsMaker ? (short)2 : (short)1;
            itemData.newPrice = data.Price;
            itemData.newTime = data.TradeTime;
            itemData.newAmt = data.Quantity;

            if (itemData.AggFirst)
            {
                Trading.instance.AggFirst(itemData);

                Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, DateTime.Now.ToString("yyyy-MM-dd") + " " + itemData.newTime,
                    DBHelper.conn1DetectHistoryColumn1, itemData.Code + "~첫체결", DBHelper.conn1DetectHistoryColumn2, " 종가:" + itemData.newPrice);
            }

            Trading.instance.AggMain(itemData);
        }
        void OnHoUpdates(IBinanceFuturesEventOrderBook data, BinanceItemData itemData)
        {
            if (!itemData.isChartShowing)
                return;

            itemData.hoHighQuan = 0;

            itemData.hoIndex = 0;
            foreach (var bid in data.Bids)
            {
                itemData.ho.MsPrice[itemData.hoIndex] = bid.Price;
                itemData.ho.MsQuan[itemData.hoIndex] = bid.Quantity;

                Trading.instance.HoMain0(itemData, true);

                itemData.hoIndex++;
            }

            itemData.hoIndex = 20;
            foreach (var ask in data.Asks)
            {
                itemData.ho.MdPrice[itemData.hoIndex] = ask.Price;
                itemData.ho.MdQuan[itemData.hoIndex] = ask.Quantity;

                Trading.instance.HoMain0(itemData, false);

                itemData.hoIndex++;
            }

            Trading.instance.HoMain1(itemData);
        }
        void Trading_ShowChartAdditional(TradeItemData itemData0)
        {
            var itemData = itemData0 as BinanceItemData;
            if (itemData.Leverage == 0)
            {
                foreach (var data in client.FuturesUsdt.GetPositionInformation(itemData.Code).Data)
                {
                    if (data.Symbol != itemData.Code)
                        break;

                    itemData.Leverage = data.Leverage;
                    itemData.maxNotionalValue = data.MaxNotionalValue;

                    foreach (var brackets in client.FuturesUsdt.GetBrackets(itemData.Code).Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                    leverageTextBox1.Text = "/ " + itemData.maxLeverage.ToString();
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

            orderPriceTextBox1.Text = "0.0";
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
        }
        void Trading_LoadMoreAdditional(Chart chart, int limit, bool loadNew)
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            KlineInterval interval = KlineInterval.OneMinute;
            ref List<TradeStick> list = ref itemDataShowing.secStickList;
            ref TradeStick stick = ref itemDataShowing.secStick;

            if (chart.TabIndex == Trading.instance.minChart.TabIndex)
            {
                list = ref itemDataShowing.minStickList;
                stick = ref itemDataShowing.minStick;
            }
            else if (chart.TabIndex == Trading.instance.hourChart.TabIndex)
            {
                interval = KlineInterval.OneHour;
                list = ref itemDataShowing.hourStickList;
                stick = ref itemDataShowing.hourStick;
            }
            else if (chart.TabIndex == Trading.instance.dayChart.TabIndex)
            {
                interval = KlineInterval.OneDay;
                list = ref itemDataShowing.dayStickList;
                stick = ref itemDataShowing.dayStick;
            }

            var start = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            if (loadNew)
            {
                Trading.instance.ClearChart(chart);
                list.Clear();

                start = -Trading.instance.baseChartViewSticksSize + 2;
                end = 0;
            }

            DateTime? endTime = null;
            if (loadNew)
                endTime = DateTime.UtcNow;
            else if (chart.TabIndex != Trading.instance.secChart.TabIndex)
            {
                endTime = list[0].Time;
                list.RemoveAt(0);

                foreach (var se in chart.Series)
                    se.Points.RemoveAt(0);
            }

            if (limit > 0)
            {
                if (chart.TabIndex != Trading.instance.secChart.TabIndex)
                {
                    var klines = client.FuturesUsdt.Market.GetKlines(itemDataShowing.Code, interval, null, endTime, limit).Data;
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
                            stick = newStick;
                        }
                        else
                            list.Insert(0, newStick);
                    }
                }
                else
                {
                    if (limit > itemDataShowing.oldSecStickList.Count)
                        limit = itemDataShowing.oldSecStickList.Count;

                    var oldCount = itemDataShowing.oldSecStickList.Count;
                    for (int i = oldCount - 1; oldCount - i <= limit; i--)
                    {
                        Trading.instance.InsertFullChartPoint(chart, itemDataShowing.oldSecStickList[i]);
                        list.Insert(0, itemDataShowing.oldSecStickList[i]);
                        itemDataShowing.oldSecStickList.RemoveAt(i);
                    }
                }

                start += limit;
                end += limit;
            }

            chart.ChartAreas[0].RecalculateAxesScale();
            chart.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
            chart.ChartAreas[0].RecalculateAxesScale();
            Trading.instance.AdjustChart(chart);
        }
        void OnMarkPriceUpdates(BinanceFuturesUsdtStreamMarkPrice data, BinanceItemData itemData)
        {
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
        void OnAccountUpdates(BinanceFuturesStreamAccountUpdate data)
        {
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

                    if (position.PositionAmount == 0)
                    {
                        socketClient.Unsubscribe(itemData.markSub);

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

                        Trading.instance.CodeListView.RemoveObject(itemData);
                        Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });

                        Trading.instance.chartNow.ChartAreas[0].AxisY2.StripLines.Clear();
                    }
                    else if (itemData.position)
                        itemData.Size = position.PositionAmount;
                    else
                    {
                        itemData.position = true;
                        itemData.EntryPrice = position.EntryPrice;
                        itemData.Size = position.PositionAmount;
                        itemData.PNL = position.UnrealizedPnl;
                        itemData.Has = 1;
                        Trading.instance.CodeListView.RemoveObject(itemData);
                        Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });

                        Trading_ResetOrderView();

                        itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Code, 3000,
                            data2 => { BeginInvoke(markUpdates, data2, itemData); }).Data;

                        var strip = new StripLine();
                        strip.Interval = double.NaN;
                        strip.ForeColor = ForeColor;
                        strip.TextLineAlignment = StringAlignment.Center;
                        Trading.instance.chartNow.ChartAreas[0].AxisY2.StripLines.Add(strip);
                    }
                }
        }
        void OnOrderUpdates(BinanceFuturesStreamOrderUpdate data, BinanceItemData itemData)
        {
            if (data.UpdateData.Status == OrderStatus.New)
            {
                itemData.OrderTime = data.UpdateData.CreateTime;
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
                if (data.UpdateData.Status == OrderStatus.Filled && !itemData.position)
                {
                    TradeResultData resultData;
                    if (Trading.instance.realHandResultListView.Items.Count == 0)
                    {
                        resultData = new TradeResultData { Symbol = itemData.Code };
                        Trading.instance.realHandResultListView.InsertObjects(0, new List<TradeResultData> { resultData });
                    }
                    else
                        resultData = Trading.instance.realHandResultListView.GetModelObject(0) as TradeResultData;
                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.LastGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                        resultData.ProfitRate = (double)Math.Round((data.UpdateData.AveragePrice / itemData.EntryPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.LastGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                        resultData.ProfitRate = (double)Math.Round((itemData.EntryPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }
                    resultData.ProfitRateAndValue = resultData.ProfitRate + ", " + data.UpdateData.RealizedProfit.ToString(ForDecimalString);
                    resultData.LastGapAndTimeAndSuccessAmount = resultData.LastGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.LastTime).TotalSeconds, 1)
                        + ", " + resultData.LastGapAndTimeAndSuccessAmount;

                    Trading.instance.UpdateResult(itemData, itemData.isAuto);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && itemData.position) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = Trading.instance.realHandResultListView.GetModelObject(0) as TradeResultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.LastGapAndTimeAndSuccessAmount = Math.Round((itemData.OrderAmount - Math.Abs(itemData.Size)) / itemData.OrderAmount, 2).ToString();

                        Trading_PlaceOrder(itemData, !itemData.LorS, true, itemData.isAuto);
                    }
                    else
                    {
                        if (data.UpdateData.AveragePrice != 0)
                        {
                            itemData.EntryPrice = data.UpdateData.AveragePrice;

                            if (data.UpdateData.Side == OrderSide.Buy)
                                resultData.EntryGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                            else
                                resultData.EntryGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                        }

                        resultData.EntryGapAndTimeAndSuccessAmount = resultData.EntryGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.EntryTime).TotalSeconds, 1)
                            + ", " + Math.Round(Math.Abs(itemData.Size) / itemData.OrderAmount, 2);
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
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.OnClosed(e);

            if (tokenSource != null)
                tokenSource.Cancel();

            socketClient.UnsubscribeAll();
            Trading.instance.dbHelper.Close();
        }
        void Trading_HoONandOFF(TradeItemData itemData0, bool on)
        {
            var itemData = itemData0 as BinanceItemData;

            if (on)  //100, 500
                itemData.hoSub = socketClient.FuturesUsdt.SubscribeToPartialOrderBookUpdates(itemData.Name, 
                    (int)Trading.instance.hoChart.Tag, 100, data2 => { BeginInvoke(hoUpdates, data2, Trading.instance.itemDataList[data2.Symbol.ToUpper()]); }).Data;
            else
                socketClient.Unsubscribe(itemData.hoSub);
        }
        void Trading_PlaceOrder(TradeItemData itemData0, bool buy, bool market, bool auto)
        {
            var itemData = itemData0 as BinanceItemData;

            if (itemData.secStick.Price[3] == default)
            {
                MessageBox.Show("close price error");
                return;
            }
            else
                itemData.orderStartClosePrice = itemData.secStick.Price[3];

            decimal priceRate = 0.1m;
            decimal quantity = itemData.position ?
                Math.Abs(itemData.Size) :
                (int)(itemData.minNotionalValue / itemData.orderStartClosePrice / itemData.minSize + 1) * itemData.minSize;
            int limitPercent = default;

            if (!auto &&
                (!decimal.TryParse(orderPriceTextBox1.Text, out priceRate)
                    || (!itemData.position && (!decimal.TryParse(orderSizeTextBox1.Text, out quantity) || quantity <= 0))
                    || !int.TryParse(autoSizeTextBox0.Text, out limitPercent) || limitPercent <= 0))
            {
                MessageBox.Show("input error");
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
                    quantity = (int)(itemData.minNotionalValue / itemData.orderStartClosePrice / itemData.minSize + 1) * itemData.minSize;
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

            if (!itemData.position)
            {
                if (price * quantity > itemData.maxNotionalValue)
                {
                    MessageBox.Show("lower leverage");
                    return;
                }
                if (price * quantity < itemData.minNotionalValue)
                {
                    MessageBox.Show("too small");
                    return;
                }
            }

            if (!auto)
                orderSizeTextBox1.Text = quantity.ToString();

            var a = client.FuturesUsdt.Order.PlaceOrder(
                itemData.Code
                , buy ? OrderSide.Buy : OrderSide.Sell
                , orderType
                , quantity
                , PositionSide.Both
                , timeInForce
                , auto ? true : ROCheckBox.Checked
                , price);
            itemData.positionWhenOrder = itemData.position;

            if (!a.Success)
            {
                MessageBox.Show("order fail" + a.Error);
                return;
            }

            var b = client.FuturesUsdt.Order.CancelAllOrdersAfterTimeout(itemData.Code, TimeSpan.FromSeconds(1));

            if (!b.Success)
            {
                MessageBox.Show("cancel timer order fail" + b.Error);
                return;
            }

            if (itemData.position)
            {
                if (itemData.resultData == default)
                {
                    itemData.resultData = new TradeResultData { Symbol = itemData.Code };
                    (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                }

                itemData.resultData.LastTime = DateTime.UtcNow;
                if (!market)
                    itemData.resultData.LastGapAndTimeAndSuccessAmount = "1";
            }
            else
            {
                itemData.LorS = buy;

                itemData.resultData = new TradeResultData { Symbol = itemData.Code };
                itemData.resultData.EntryTime = DateTime.UtcNow;
                itemData.resultData.EntryGapAndTimeAndSuccessAmount = "0";
                (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });

                itemData.Size = 0;
            }

            Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, DateTime.Now.ToString(Trading.instance.secTimeFormat) + "~" + itemData.secStick.Time.ToString(Trading.instance.secTimeFormat),
                DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 " + (buy ? "매수" : "매도") + "주문전송", DBHelper.conn1OrderHistoryColumn2, "주문가격:" + price + " 주문수량:" + quantity);
        }
    }
}
