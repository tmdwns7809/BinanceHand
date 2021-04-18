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

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        #region Global vars
        string startTime;

        //DBHelper dbHelper = DBHelper.GetInstance();

        Dictionary<string, ItemData> SPOTItemDataList = new Dictionary<string, ItemData>();
        Dictionary<string, ItemData> FUItemDataList = new Dictionary<string, ItemData>();
        Dictionary<string, ItemData> FCItemDataList = new Dictionary<string, ItemData>();
        List<ItemData> FUPositionItemData = new List<ItemData>();

        ItemData itemDataShowing;

        static int amtAnPrcAlpha = 130;
        Color msAmtColor = Color.FromArgb(amtAnPrcAlpha, Color.Orange);
        Color mdAmtColor = Color.FromArgb(amtAnPrcAlpha, Color.ForestGreen);
        Color msAndMdAmtColor = Color.FromArgb(amtAnPrcAlpha, 145, 151, 17);
        Color plsPrcColor = Color.FromArgb(amtAnPrcAlpha, Color.Red);
        Color mnsPrcColor = Color.FromArgb(amtAnPrcAlpha, Color.Blue);

        int baseChartViewSticksSize = 120;

        static int gridAlpha = 90;
        Color gridColor = Color.FromArgb(gridAlpha, Color.Gray);

        decimal FUMarginBalance;
        decimal FUMaintMargin;
        decimal FUAvailableBalance;
        decimal FUBalance;
        string ForDecimalString = "0.#############################";
        #endregion

        public Form1()
        {
            InitializeComponent();

            SetClientAndKey();

            SetComponents();
        }

        #region SetClientAndKey vars
        BinanceClient client;
        BinanceSocketClient socketClient;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        #endregion
        void SetClientAndKey()
        {
            var clientOption = new BinanceClientOptions();
            var socketOption = new BinanceSocketClientOptions();

            if (true)       //testnet
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
            {
                clientOption.ApiCredentials = new ApiCredentials("DpLZEH9lDIuoYvs5L9YcDshSXW1rN9Rlw6OMj7zVQwR7wuBGt9vtyntEDNUfUb50", "xXhj8yvUrLgVxlO4BioadC4RQSoydDLR2UFIaMcqBCdX1WM2LjVsM3cxC1Y9lgVJ");
                clientOption.AutoTimestamp = false;
                clientOption.TradeRulesBehaviour = TradeRulesBehaviour.None;
                socketOption.ApiCredentials = new ApiCredentials("DpLZEH9lDIuoYvs5L9YcDshSXW1rN9Rlw6OMj7zVQwR7wuBGt9vtyntEDNUfUb50", "xXhj8yvUrLgVxlO4BioadC4RQSoydDLR2UFIaMcqBCdX1WM2LjVsM3cxC1Y9lgVJ");
                socketOption.AutoReconnect = true;
                socketOption.ReconnectInterval = TimeSpan.FromMinutes(1);
                //clientOption.LogVerbosity = LogVerbosity.Debug;
                //clientOption.LogWriters = new List<TextWriter> { Console.Out };
            }

            client = new BinanceClient(clientOption);
            socketClient = new BinanceSocketClient(socketOption);

            SubscribeToUserStream(true, true);
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
                listenKey = client.Spot.UserStream.StartUserStream().Data;

            var _keepAliveTask = Task.Run(async () =>
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
                        await client.Spot.UserStream.KeepAliveUserStreamAsync(listenKey);

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
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        });
                else
                    socketClient.Spot.SubscribeToUserDataUpdates(listenKey, null, null, null, null);
            }
            else
                socketClient.Spot.SubscribeToUserDataUpdates(listenKey, null, null, null, null);


            mainTabControl.SelectedTab = futuresUTab;
            futuresUTabControl.SelectedTab = FUPositionTab;


            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            var n = 0;
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
                var itemData = new ItemData(null, s, null);
                FUItemDataList.Add(itemData.Name, itemData);
                FUSymbolList.Add(itemData.Name);

                n++;
            }

            var ga = client.FuturesUsdt.Account.GetAccountInfo();
            foreach (var s in ga.Data.Assets)
            {
                if (s.Asset == "USDT")
                {
                    FUMarginBalance = s.MarginBalance;
                    FUMarginBalanceTextBox1.Text = Math.Round(FUMarginBalance, 2).ToString();
                    FUAvailableBalance = s.AvailableBalance;
                    FUAvailBalanceTextBox1.Text = Math.Round(FUAvailableBalance, 2).ToString();
                    FUBalance = s.WalletBalance;
                    FUBalanceTextBox1.Text = Math.Round(FUBalance, 2).ToString();
                }
            }
            foreach (var s in ga.Data.Positions)
            {
                if (s.EntryPrice != 0m)
                {
                    var itemData = FUItemDataList[s.Symbol];

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2); 
                    itemData.maintMargin = s.MaintMargin;

                    FUPositionListView.AddObject(itemData);
                }
            }

            FUMaintMargin = 0;
            foreach (ItemData itemData in FUPositionListView.Objects)
                FUMaintMargin += itemData.maintMargin;
            FUMaintMarginTextBox1.Text = Math.Round(FUMaintMargin, 2).ToString();

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
                    var itemData = FUItemDataList[s.Symbol];

                    itemData.Position = true;
                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    itemData.Size = s.PositionAmount;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.EntryPrice = Math.Round(s.EntryPrice, 2);
                    itemData.PNL = Math.Round(s.UnrealizedProfit, 2);
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                    if (itemData.Size > 0)
                        itemData.PositionLong = true;
                    else
                        itemData.PositionLong = false;

                    foreach (var brackets in client.FuturesUsdt.GetBrackets(s.Symbol).Data)
                        itemData.brackets = brackets.Brackets.ToList();
                    for (int i = 0; i < itemData.brackets.Count; i++)
                        if (itemData.notianalValue > itemData.brackets[i].Floor && itemData.notianalValue <= itemData.brackets[i].Cap)
                            itemData.nowBracketIndex = i;

                    FUPositionListView.UncheckObject(itemData);
                }
            }

            var gbr = client.FuturesUsdt.GetBrackets("BTCUSDT");
            var gI = client.FuturesUsdt.GetIncomeHistory();
            var gpm = client.FuturesUsdt.GetPositionMode();
        }

        void SetComponents()
        {
            startTime = DateTime.UtcNow.ToString();
            Text = "Binance       UTC-" + startTime;

            SetComponentsLocationAndSize();

            SetChart();

            SetSymbolsListView();

            SetMainListView();
        }
        void SetComponentsLocationAndSize()
        {
            WindowState = FormWindowState.Maximized;

            SetChartLocationAndSize();

            SetMainTabControlLocationAndSize();

            SetSymbolsListViewLocationAndSize();
        }
        void SetChartLocationAndSize()
        {
            chartTabControl.Location = new Point(0, 0);
            chartTabControl.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.8), (int)(Screen.GetWorkingArea(this).Size.Height * 0.8));

            secChartTab.Size = new Size(chartTabControl.Size.Width - 8, chartTabControl.Size.Height - 26);
            minChartTab.Size = secChartTab.Size;

            marketComboBox.Size = new Size(60, nameTextBox.Size.Height);
            marketComboBox.Location = chartTabControl.Location;
            marketComboBox.SelectedItem = marketComboBox.Items[0];
            marketComboBox.BringToFront();
            nameTextBox.Location = new Point(marketComboBox.Location.X + marketComboBox.Size.Width, marketComboBox.Location.Y);
            nameTextBox.BringToFront();

            chart1.Location = new Point(3, 3);
            chart1.Size = new Size(secChartTab.Size.Width - 8, secChartTab.Size.Height - 8);
            chart2.Location = chart1.Location;
            chart2.Size = chart1.Size;

            priceTextBox.Location = new Point(chartTabControl.Location.X + chartTabControl.Size.Width - 230, chartTabControl.Location.Y + chartTabControl.Size.Height - 20);
            priceTextBox.BackColor = chart1.BackColor;
            priceTextBox.BringToFront();
            amtTextBox.Location = new Point(priceTextBox.Location.X + priceTextBox.Size.Width, priceTextBox.Location.Y);
            amtTextBox.BackColor = chart1.BackColor;
            amtTextBox.BringToFront();
            timeDiffTextBox.Location = new Point(amtTextBox.Location.X + amtTextBox.Size.Width, amtTextBox.Location.Y);
            timeDiffTextBox.Size = new Size(30, amtTextBox.Size.Height);
            timeDiffTextBox.BackColor = chart1.BackColor;
            timeDiffTextBox.BringToFront();
        }
        void SetMainTabControlLocationAndSize()
        {
            mainTabControl.Location = new Point(chartTabControl.Location.X, chartTabControl.Location.Y + chartTabControl.Size.Height);
            mainTabControl.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.5), Screen.GetWorkingArea(this).Size.Height - chartTabControl.Size.Height - 30);

            futuresUTab.Size = new Size(mainTabControl.Size.Width - 8, mainTabControl.Size.Height - 26);
            logTab.Size = futuresUTab.Size;

            futuresUTabControl.Location = chart1.Location;
            futuresUTabControl.Size = new Size(futuresUTab.Size.Width - 8, futuresUTab.Size.Height - 8);

            FUMarginRatioTextBox0.Location =
                new Point(futuresUTabControl.Location.X + futuresUTabControl.Size.Width - FUMarginRatioTextBox0.Size.Width - FUMarginRatioTextBox1.Size.Width
                    - FUMaintMarginTextBox0.Size.Width - FUMaintMarginTextBox1.Size.Width - FUMarginBalanceTextBox0.Size.Width - FUMarginBalanceTextBox1.Size.Width
                    - FUAvailBalanceTextBox0.Size.Width - FUAvailBalanceTextBox1.Size.Width - FUBalanceTextBox0.Size.Width - FUBalanceTextBox1.Size.Width, futuresUTabControl.Location.Y);
            FUMarginRatioTextBox1.Location =
                new Point(FUMarginRatioTextBox0.Location.X + FUMarginRatioTextBox0.Size.Width, futuresUTabControl.Location.Y);
            FUMaintMarginTextBox0.Location =
                new Point(FUMarginRatioTextBox1.Location.X + FUMarginRatioTextBox1.Size.Width, futuresUTabControl.Location.Y);
            FUMaintMarginTextBox1.Location =
                new Point(FUMaintMarginTextBox0.Location.X + FUMaintMarginTextBox0.Size.Width, futuresUTabControl.Location.Y);
            FUMarginBalanceTextBox0.Location = 
                new Point(FUMaintMarginTextBox1.Location.X + FUMaintMarginTextBox1.Size.Width, futuresUTabControl.Location.Y);
            FUMarginBalanceTextBox1.Location =
                new Point(FUMarginBalanceTextBox0.Location.X + FUMarginBalanceTextBox0.Size.Width, futuresUTabControl.Location.Y);
            FUAvailBalanceTextBox0.Location =
                new Point(FUMarginBalanceTextBox1.Location.X + FUMarginBalanceTextBox1.Size.Width, futuresUTabControl.Location.Y);
            FUAvailBalanceTextBox1.Location =
                new Point(FUAvailBalanceTextBox0.Location.X + FUAvailBalanceTextBox0.Size.Width, futuresUTabControl.Location.Y);
            FUBalanceTextBox0.Location =
                new Point(FUAvailBalanceTextBox1.Location.X + FUAvailBalanceTextBox1.Size.Width, futuresUTabControl.Location.Y);
            FUBalanceTextBox1.Location =
                new Point(FUBalanceTextBox0.Location.X + FUBalanceTextBox0.Size.Width, futuresUTabControl.Location.Y);
            FUMarginRatioTextBox0.BringToFront();
            FUMarginRatioTextBox1.BringToFront();
            FUMaintMarginTextBox0.BringToFront();
            FUMaintMarginTextBox1.BringToFront();
            FUMarginBalanceTextBox0.BringToFront();
            FUMarginBalanceTextBox1.BringToFront();
            FUAvailBalanceTextBox0.BringToFront();
            FUAvailBalanceTextBox1.BringToFront();
            FUBalanceTextBox0.BringToFront();
            FUBalanceTextBox1.BringToFront();
            FUMarginRatioTextBox0.BackColor = Color.FromArgb(futuresUTab.BackColor.R, futuresUTab.BackColor.G, futuresUTab.BackColor.B);
            FUMarginRatioTextBox1.BackColor = FUMaintMarginTextBox0.BackColor;
            FUMaintMarginTextBox0.BackColor = FUMaintMarginTextBox0.BackColor;
            FUMaintMarginTextBox1.BackColor = FUMaintMarginTextBox0.BackColor;
            FUMarginBalanceTextBox0.BackColor = FUMaintMarginTextBox0.BackColor;
            FUMarginBalanceTextBox1.BackColor = FUMaintMarginTextBox0.BackColor;
            FUAvailBalanceTextBox0.BackColor = FUMaintMarginTextBox0.BackColor;
            FUAvailBalanceTextBox1.BackColor = FUMaintMarginTextBox0.BackColor;
            FUBalanceTextBox0.BackColor = FUMaintMarginTextBox0.BackColor;
            FUBalanceTextBox1.BackColor = FUMaintMarginTextBox0.BackColor;

            FUPositionTab.Size = new Size(futuresUTabControl.Size.Width - 8, futuresUTabControl.Size.Height - 26);
            FUOpenOrdersTab.Size = FUPositionTab.Size;
            FUOrderHistoryTab.Size = FUPositionTab.Size;
            FUTradeHistoryTab.Size = FUPositionTab.Size;
            FUTransactionHistoryTab.Size = FUPositionTab.Size;
            FUAssetsTab.Size = FUPositionTab.Size;

            FUPositionListView.Location = chart1.Location;
            FUPositionListView.Size = new Size(FUPositionTab.Size.Width - 8, FUPositionTab.Size.Height - 8);
            FUOpenOrdersListView.Location = chart1.Location;
            FUOpenOrdersListView.Size = FUPositionListView.Size;
            FUOrderHistoryListView.Location = chart1.Location;
            FUOrderHistoryListView.Size = FUPositionListView.Size;
            FUTradeHistoryListView.Location = chart1.Location;
            FUTradeHistoryListView.Size = FUPositionListView.Size;
            FUTransHistoryListView.Location = chart1.Location;
            FUTransHistoryListView.Size = FUPositionListView.Size;
            FUAssetsListView.Location = chart1.Location;
            FUAssetsListView.Size = FUPositionListView.Size;

            logListBox.Location = chart1.Location;
            logListBox.Size = futuresUTabControl.Size;
        }
        void SetSymbolsListViewLocationAndSize()
        {
            spotKlineRcvTextBox.Size = new Size(25, 14);
            spotListView.Location = new Point(mainTabControl.Location.X + mainTabControl.Size.Width, mainTabControl.Location.Y);
            spotListView.Size = new Size((Screen.GetWorkingArea(this).Size.Width - mainTabControl.Size.Width) / 3, mainTabControl.Size.Height - spotKlineRcvTextBox.Size.Height);
            spotKlineRcvTextBox.Location = new Point(spotListView.Location.X, spotListView.Location.Y + spotListView.Size.Height + 3);
            spotKlineReqTextBox.Size = new Size(47, spotKlineRcvTextBox.Height);
            spotKlineReqTextBox.Location = new Point(spotKlineRcvTextBox.Location.X + spotKlineRcvTextBox.Size.Width, spotKlineRcvTextBox.Location.Y);
            spotAggRcvTextBox.Size = new Size(7, spotKlineRcvTextBox.Height);
            spotAggRcvTextBox.Location = new Point(spotKlineReqTextBox.Location.X + spotKlineReqTextBox.Size.Width + spotKlineRcvTextBox.Size.Width, spotKlineReqTextBox.Location.Y);
            spotAggReqTextBox.Size = new Size(30, spotKlineRcvTextBox.Height);
            spotAggReqTextBox.Location = new Point(spotAggRcvTextBox.Location.X + spotAggRcvTextBox.Size.Width, spotAggRcvTextBox.Location.Y);

            futureUListView.Location = new Point(spotListView.Location.X + spotListView.Size.Width, spotListView.Location.Y);
            futureUListView.Size = new Size(spotListView.Size.Width, spotListView.Size.Height);
            futureUKlineRcvTextBox.Size = new Size(spotKlineRcvTextBox.Width, spotKlineRcvTextBox.Height);
            futureUKlineRcvTextBox.Location = new Point(futureUListView.Location.X, spotKlineRcvTextBox.Location.Y);
            futureUKlineReqTextBox.Size = new Size(spotKlineReqTextBox.Width, spotKlineRcvTextBox.Height);
            futureUKlineReqTextBox.Location = new Point(futureUKlineRcvTextBox.Location.X + futureUKlineRcvTextBox.Size.Width, futureUKlineRcvTextBox.Location.Y);
            futureUAggRcvTextBox.Size = new Size(spotAggRcvTextBox.Width, spotKlineRcvTextBox.Height);
            futureUAggRcvTextBox.Location = new Point(futureUKlineReqTextBox.Location.X + futureUKlineReqTextBox.Size.Width + futureUKlineRcvTextBox.Size.Width, futureUKlineReqTextBox.Location.Y);
            futureUAggReqTextBox.Size = new Size(spotAggReqTextBox.Width, spotKlineRcvTextBox.Height);
            futureUAggReqTextBox.Location = new Point(futureUAggRcvTextBox.Location.X + futureUAggRcvTextBox.Size.Width, futureUAggRcvTextBox.Location.Y);

            futureCListView.Location = new Point(futureUListView.Location.X + futureUListView.Size.Width, futureUListView.Location.Y);
            futureCListView.Size = new Size(spotListView.Size.Width, spotListView.Size.Height);
            futureCKlineRcvTextBox.Size = new Size(futureUKlineRcvTextBox.Width, spotKlineRcvTextBox.Height);
            futureCKlineRcvTextBox.Location = new Point(futureCListView.Location.X, spotKlineRcvTextBox.Location.Y);
            futureUKlineReqTextBox.Size = new Size(futureUKlineReqTextBox.Width, spotKlineRcvTextBox.Height);
            futureCKlineReqTextBox.Location = new Point(futureCKlineRcvTextBox.Location.X + futureCKlineRcvTextBox.Size.Width, futureCKlineRcvTextBox.Location.Y);
            futureCAggRcvTextBox.Size = new Size(futureUAggRcvTextBox.Width, spotKlineRcvTextBox.Height);
            futureCAggRcvTextBox.Location = new Point(futureCKlineReqTextBox.Location.X + futureCKlineReqTextBox.Size.Width + futureCKlineRcvTextBox.Size.Width, futureCKlineReqTextBox.Location.Y);
            futureCAggReqTextBox.Size = new Size(futureUAggReqTextBox.Width, spotKlineRcvTextBox.Height);
            futureCAggReqTextBox.Location = new Point(futureCAggRcvTextBox.Location.X + futureCAggRcvTextBox.Size.Width, futureCAggRcvTextBox.Location.Y);
        }

        void SetChart()
        {
            chart1.AxisViewChanged += (sender, e) => { AdjustChart(chart1); };

            var chartAreaMain = chart1.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMain.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisX.MajorGrid.Interval = 10;
            chartAreaMain.AxisX.MajorTickMark.Size = 0.4f;
            chartAreaMain.AxisX.LabelStyle.Interval = 30;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.secChartLabel;

            chartAreaMain.AxisY.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisY.ScrollBar.Enabled = false;
            chartAreaMain.AxisY.MajorTickMark.Enabled = false;
            chartAreaMain.AxisY.IsStartedFromZero = false;
            chartAreaMain.AxisY.LabelStyle.Enabled = false;

            chartAreaMain.Position = new ElementPosition(0, 0, 100, 100);


            var chartAreaMsMd = chart1.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = Color.Transparent;
            chartAreaMsMd.BorderColor = Color.Transparent;
            chartAreaMsMd.Position.FromRectangleF(chartAreaMain.Position.ToRectangleF());
            chartAreaMsMd.Position.Height -= 50;
            chartAreaMsMd.Position.Y += 50;

            chartAreaMsMd.AxisX.MajorGrid.Enabled = false;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = Color.Transparent;
            chartAreaMsMd.AxisX.MajorTickMark.Size = chartAreaMain.AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScrollBar.LineColor = Color.Transparent;

            chartAreaMsMd.AxisY.MajorGrid.Enabled = false;
            chartAreaMsMd.AxisY.ScrollBar.Enabled = false;
            chartAreaMsMd.AxisY.MajorTickMark.Enabled = false;
            chartAreaMsMd.AxisY.LabelStyle.Enabled = false;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;


            var seriesPrice = chart1.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = Color.FromArgb(0, plsPrcColor);
            seriesPrice.YAxisType = AxisType.Primary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            var seriesMsAndMdAmt = chart1.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = AxisType.Primary;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            var seriesMsOrMdAmt = chart1.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = AxisType.Primary;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;

            chart2.AxisViewChanged += (sender, e) => { AdjustChart(chart2); };
            chart2.AxisScrollBarClicked += (sender, e) =>
            {
                if (e.ButtonType == ScrollBarButtonType.SmallDecrement && (int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum == 0)
                {
                    LoadMinutesMore(itemDataShowing);
                    e.IsHandled = true;
                }
            };

            chartAreaMain = chart2.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMain.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisX.MajorGrid.Interval = 10;
            chartAreaMain.AxisX.MajorTickMark.Size = 0.4f;
            chartAreaMain.AxisX.LabelStyle.Interval = chart1.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.minChartLabel;

            chartAreaMain.AxisY.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisY.ScrollBar.Enabled = false;
            chartAreaMain.AxisY.MajorTickMark.Enabled = false;
            chartAreaMain.AxisY.IsStartedFromZero = false;
            chartAreaMain.AxisY.LabelStyle.Enabled = false;

            chartAreaMain.Position = chart1.ChartAreas[0].Position;


            chartAreaMsMd = chart2.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = Color.Transparent;
            chartAreaMsMd.BorderColor = Color.Transparent;
            chartAreaMsMd.Position.FromRectangleF(chartAreaMain.Position.ToRectangleF());
            chartAreaMsMd.Position.Height -= 50;
            chartAreaMsMd.Position.Y += 50;

            chartAreaMsMd.AxisX.MajorGrid.Enabled = false;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = Color.Transparent;
            chartAreaMsMd.AxisX.MajorTickMark.Size = chartAreaMain.AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = Color.Transparent;
            chartAreaMsMd.AxisX.ScrollBar.LineColor = Color.Transparent;

            chartAreaMsMd.AxisY.MajorGrid.Enabled = false;
            chartAreaMsMd.AxisY.ScrollBar.Enabled = false;
            chartAreaMsMd.AxisY.MajorTickMark.Enabled = false;
            chartAreaMsMd.AxisY.LabelStyle.Enabled = false;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;


            seriesPrice = chart2.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = Color.FromArgb(0, plsPrcColor);
            seriesPrice.YAxisType = AxisType.Primary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            seriesMsAndMdAmt = chart2.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = AxisType.Primary;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            seriesMsOrMdAmt = chart2.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = AxisType.Primary;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;
        }
        void SetSymbolsListView()
        {
            var nameColumnSize = 7;
            var flucColumnSize = 3;
            var durColumnSize = 3;
            var winColumnSize = 6;
            var proColumnSize = 6;

            var nameColumn = new OLVColumn("이름(현물)", "Name");
            var flucColumn = new OLVColumn("분봉", "RateBfor");
            var durColumn = new OLVColumn("지속", "AggReadyRow");
            var winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            var proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = nameColumnSize;
            flucColumn.FreeSpaceProportion = flucColumnSize;
            durColumn.FreeSpaceProportion = durColumnSize;
            winColumn.FreeSpaceProportion = winColumnSize;
            proColumn.FreeSpaceProportion = proColumnSize;
            spotListView.AllColumns.Add(nameColumn);
            spotListView.AllColumns.Add(flucColumn);
            spotListView.AllColumns.Add(durColumn);
            spotListView.AllColumns.Add(winColumn);
            spotListView.AllColumns.Add(proColumn);
            spotListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });

            nameColumn = new OLVColumn("이름(선물U)", "Name");
            flucColumn = new OLVColumn("분봉", "RateBfor");
            durColumn = new OLVColumn("지속", "AggReadyRow");
            winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = nameColumnSize;
            flucColumn.FreeSpaceProportion = flucColumnSize;
            durColumn.FreeSpaceProportion = durColumnSize;
            winColumn.FreeSpaceProportion = winColumnSize;
            proColumn.FreeSpaceProportion = proColumnSize;
            futureUListView.AllColumns.Add(nameColumn);
            futureUListView.AllColumns.Add(flucColumn);
            futureUListView.AllColumns.Add(durColumn);
            futureUListView.AllColumns.Add(winColumn);
            futureUListView.AllColumns.Add(proColumn);
            futureUListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });

            nameColumn = new OLVColumn("이름(선물C)", "Name");
            flucColumn = new OLVColumn("분봉", "RateBfor");
            durColumn = new OLVColumn("지속", "AggReadyRow");
            winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = nameColumnSize;
            flucColumn.FreeSpaceProportion = flucColumnSize;
            durColumn.FreeSpaceProportion = durColumnSize;
            winColumn.FreeSpaceProportion = winColumnSize;
            proColumn.FreeSpaceProportion = proColumnSize;
            futureCListView.AllColumns.Add(nameColumn);
            futureCListView.AllColumns.Add(flucColumn);
            futureCListView.AllColumns.Add(durColumn);
            futureCListView.AllColumns.Add(winColumn);
            futureCListView.AllColumns.Add(proColumn);
            futureCListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });
        }
        void SetMainListView()
        {
            var nameColumn = new OLVColumn("Symbol", "Name");
            var leverageColumn = new OLVColumn("Leverage", "Leverage");
            var sizeColumn = new OLVColumn("Size", "Size");
            var entryPriceColumn = new OLVColumn("Entry Price", "EntryPrice");
            var markPriceColumn = new OLVColumn("Mark Price", "MarkPrice");
            var initialmarginColumn = new OLVColumn("Initial Margin", "InitialMargin");
            var PNLColumn = new OLVColumn("PNL", "PNL");
            var ROEColumn = new OLVColumn("ROE(%)", "ROE");
            nameColumn.FreeSpaceProportion = 3; 
            leverageColumn.FreeSpaceProportion = 2;
            sizeColumn.FreeSpaceProportion = 2;
            entryPriceColumn.FreeSpaceProportion = 2;
            markPriceColumn.FreeSpaceProportion = 2;
            initialmarginColumn.FreeSpaceProportion = 2;
            PNLColumn.FreeSpaceProportion = 2;
            ROEColumn.FreeSpaceProportion = 2;
            FUPositionListView.AllColumns.Add(nameColumn);
            FUPositionListView.AllColumns.Add(leverageColumn);
            FUPositionListView.AllColumns.Add(sizeColumn);
            FUPositionListView.AllColumns.Add(entryPriceColumn);
            FUPositionListView.AllColumns.Add(markPriceColumn);
            FUPositionListView.AllColumns.Add(initialmarginColumn);
            FUPositionListView.AllColumns.Add(PNLColumn);
            FUPositionListView.AllColumns.Add(ROEColumn);
            FUPositionListView.Columns.AddRange(new ColumnHeader[] 
                { nameColumn, leverageColumn, sizeColumn, entryPriceColumn, markPriceColumn, 
                    initialmarginColumn, PNLColumn, ROEColumn });
        }

        void ShowChart(ItemData itemData)
        {
            if (itemData.isChartShowing)
                return;

            if (itemDataShowing != null && itemDataShowing.isChartShowing)
                itemDataShowing.isChartShowing = false;
            itemDataShowing = itemData;

            itemData.isChartShowing = true;

            if (itemData.isSpot)
            {
                Text = itemData.Name + "-Spot      Binance       UTC-" + startTime;
                marketComboBox.SelectedItem = marketComboBox.Items[0];
            }
            else if (itemData.isFutureUsdt)
            {
                Text = itemData.Name + "-Future(Usdt)      Binance       UTC-" + startTime;
                marketComboBox.SelectedItem = marketComboBox.Items[1];
            }
            else
            {
                Text = itemData.Name + "-Future(Coin)      Binance       UTC-" + startTime;
                marketComboBox.SelectedItem = marketComboBox.Items[2];
            }

            nameTextBox.Text = itemData.Name;

            foreach (var se in chart1.Series)
                se.Points.Clear();
            foreach (var se in chart2.Series)
                se.Points.Clear();

            Stick stick;
            for (int i = 0; i < itemData.minStickList.Count; i++)
            {
                stick = itemData.minStickList[i];
                AddFullChartPoint(chart2, stick);
            }
            if (itemData.minStickList.Count <= baseChartViewSticksSize)
                LoadMinutesMore(itemData);

            chart2.ChartAreas[0].RecalculateAxesScale();
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(chart2.Series[0].Points.Count - baseChartViewSticksSize + 1, chart2.Series[0].Points.Count);
            chart2.ChartAreas[0].RecalculateAxesScale();
            AdjustChart(chart2);

            if (itemData.isAggOn)
            {
                for (int i = 0; i < itemData.secStickList.Count; i++)
                {
                    stick = itemData.secStickList[i];
                    AddFullChartPoint(chart1, stick);
                }

                AddFullChartPoint(chart1, itemData.secStick);

                chart1.ChartAreas[0].RecalculateAxesScale();
                if (chart1.Series[0].Points.Count > baseChartViewSticksSize)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count - baseChartViewSticksSize + 1, chart1.Series[0].Points.Count);
                    chart1.ChartAreas[0].RecalculateAxesScale();
                }
                else
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                    chart1.ChartAreas[0].RecalculateAxesScale();
                }
                AdjustChart(chart1);

                chart1.ChartAreas[0].AxisY.MajorGrid.Interval = chart1.Series[0].Points.Last().YValues[0] * 0.005;
            }
            else
                SetAggOn(itemData, true);
        }
        void LoadMinutesMore(ItemData itemData)
        {
            var start = (int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            if (double.IsNaN(chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum) || double.IsNaN(chart2.ChartAreas[0].AxisX.ScaleView.ViewMaximum))
            {
                start = 1;
                end = itemData.minStickList.Count;
            }

            if (end - start + 1 <= baseChartViewSticksSize)
                start = end - baseChartViewSticksSize + 1;

            DateTime endTime;
            if (itemData.minStickList.Count == 0)
                endTime = DateTime.UtcNow;
            else
                endTime = itemData.minStickList[0].Time.AddMinutes(-1);

            IEnumerable<IBinanceKline> klines;
            if (itemData.isSpot)
                klines = client.Spot.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 300).Data;
            else if (itemData.isFutureUsdt)
                klines = client.FuturesUsdt.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 300).Data;
            else
                klines = client.FuturesCoin.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 300).Data;

            foreach (var se in chart2.Series)
                se.Points.Clear();

            var newMinStickList = new List<Stick>();
            Stick stick;
            foreach (var kline in klines)
            {
                stick = new Stick();
                stick.Price[0] = kline.High;
                stick.Price[1] = kline.Low;
                stick.Price[2] = kline.Open;
                stick.Price[3] = kline.Close;
                stick.Ms = kline.TakerBuyBaseVolume;
                stick.Md = kline.BaseVolume - kline.TakerBuyBaseVolume;
                stick.Time = kline.OpenTime;
                stick.TCount = kline.TradeCount;
                AddFullChartPoint(chart2, stick);
                newMinStickList.Add(stick);
            }

            start += newMinStickList.Count;
            end += newMinStickList.Count;

            for (int i = 0; i < itemData.minStickList.Count; i++)
            {
                stick = itemData.minStickList[i];
                AddFullChartPoint(chart2, stick);
                newMinStickList.Add(stick);
            }

            if (itemData.isSpot)
                klines = client.Spot.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, newMinStickList.Last().Time.AddMinutes(1), null, null).Data;
            else if (itemData.isFutureUsdt)
                klines = client.FuturesUsdt.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, newMinStickList.Last().Time.AddMinutes(1), null, null).Data;
            else
                klines = client.FuturesCoin.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, newMinStickList.Last().Time.AddMinutes(1), null, null).Data;

            int last = klines.Count();
            short j = 0;
            foreach (var kline in klines)
            {
                stick = new Stick();
                stick.Price[0] = kline.High;
                stick.Price[1] = kline.Low;
                stick.Price[2] = kline.Open;
                stick.Price[3] = kline.Close;
                stick.Ms = kline.TakerBuyBaseVolume;
                stick.Md = kline.BaseVolume - kline.TakerBuyBaseVolume;
                stick.Time = kline.OpenTime;
                stick.TCount = kline.TradeCount;
                if (j != last - 1)
                {
                    AddFullChartPoint(chart2, stick);
                    newMinStickList.Add(stick);
                }
                else
                    AddStartChartPoint(chart2, stick);
                j++;
            }

            start -= j - 1;
            end -= j - 1;

            itemData.minStickList = newMinStickList;

            chart2.ChartAreas[0].RecalculateAxesScale();
            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
        }
        void AddFullChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { stick.Price[0], stick.Price[1], stick.Price[2], stick.Price[3] });
            if (stick.Price[2] > stick.Price[3] || (stick.Price[2] == stick.Price[3] && stick.Price[2] - stick.Price[1] >= stick.Price[0] - stick.Price[3]))
            {
                chart.Series[0].Points.Last().Color = mnsPrcColor;
                chart.Series[0].Points.Last().BackSecondaryColor = mnsPrcColor;
            }
            else
            {
                chart.Series[0].Points.Last().Color = plsPrcColor;
                chart.Series[0].Points.Last().BackSecondaryColor = plsPrcColor;
            }

            if (stick.Ms > stick.Md)
            {
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().Label, stick.Md);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().Label, stick.Ms - stick.Md);
                chart.Series[2].Points.Last().Color = msAmtColor;
            }
            else
            {
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().Label, stick.Ms);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().Label, stick.Md - stick.Ms);
                chart.Series[2].Points.Last().Color = mdAmtColor;
            }
        }
        void AddStartChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { stick.Price[2], stick.Price[2], stick.Price[2], stick.Price[2] });
            chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().Label, 0);
            chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().Label, 0);

            if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed && (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= chart.Series[0].Points.Count)
            {
                chart.ChartAreas[0].RecalculateAxesScale();
                chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallIncrement);
            }
        }
        void UpdateChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.Last().YValues[0] = (double)stick.Price[0];
            chart.Series[0].Points.Last().YValues[1] = (double)stick.Price[1];
            chart.Series[0].Points.Last().YValues[2] = (double)stick.Price[2];
            chart.Series[0].Points.Last().YValues[3] = (double)stick.Price[3];

            if (stick.Price[2] > stick.Price[3] || (stick.Price[2] == stick.Price[3] && stick.Price[2] - stick.Price[1] >= stick.Price[0] - stick.Price[3]))
            {
                chart.Series[0].Points.Last().Color = mnsPrcColor;
                chart.Series[0].Points.Last().BackSecondaryColor = mnsPrcColor;
            }
            else
            {
                chart.Series[0].Points.Last().Color = plsPrcColor;
                chart.Series[0].Points.Last().BackSecondaryColor = plsPrcColor;
            }

            if (stick.Ms > stick.Md)
            {
                chart.Series[1].Points.Last().YValues[0] = (double)stick.Md;
                chart.Series[2].Points.Last().YValues[0] = (double)(stick.Ms - stick.Md);
                chart.Series[2].Points.Last().Color = msAmtColor;
            }
            else
            {
                chart.Series[1].Points.Last().YValues[0] = (double)stick.Ms;
                chart.Series[2].Points.Last().YValues[0] = (double)(stick.Md - stick.Ms);
                chart.Series[2].Points.Last().Color = mdAmtColor;
            }
        }
        void AdjustChart(Chart chart)
        {
            var priceLow = double.MaxValue;
            var priceHigh = double.MinValue;
            var msOrMdHigh = double.MinValue;

            for (int i = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum - 1; i < (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum; i++)
            {
                if (i >= chart.Series[0].Points.Count || i < 0)
                    continue;

                if (chart.Series[0].Points[i].YValues[1] < priceLow)
                    priceLow = chart.Series[0].Points[i].YValues[1];
                
                if (chart.Series[0].Points[i].YValues[0] > priceHigh)
                    priceHigh = chart.Series[0].Points[i].YValues[0];

                if (chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0] > msOrMdHigh)
                    msOrMdHigh = chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0];
            }

            if (priceLow == double.MaxValue || priceHigh == double.MinValue || msOrMdHigh == double.MinValue)
                return;

            chart.ChartAreas[0].AxisY.ScaleView.Zoom(2 * priceLow - priceHigh, priceHigh);
            chart.ChartAreas[1].AxisY.ScaleView.Zoom(0, msOrMdHigh);

            chart.ChartAreas[0].AxisY.MajorGrid.Interval = (double)priceHigh * 0.005;
        }

        #region SetItemDataList vars
        List<string>[] spotSymbolList = Enumerable.Range(0, 5).Select(i => new List<string>()).ToArray();
        List<string> FUSymbolList = new List<string>();
        List<string> futuresCSymbolList = new List<string>();
        #endregion
        void SetItemDataList()
        {
            var exchangeInfo = client.Spot.System.GetExchangeInfo();
            short n = 0;
            foreach (var s in exchangeInfo.Data.Symbols)
            {
                var itemData = new ItemData(s, null, null);
                SPOTItemDataList.Add(itemData.Name, itemData);
                spotSymbolList[n / 300].Add(itemData.Name);

                n++;
            }

            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            n = 0;
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
                var itemData = new ItemData(null, s, null);
                FUItemDataList.Add(itemData.Name, itemData);
                FUSymbolList.Add(itemData.Name);

                n++;
            }

            var exchangeInfo3 = client.FuturesCoin.System.GetExchangeInfo();
            n = 0;
            foreach (var s in exchangeInfo3.Data.Symbols)
            {
                var itemData = new ItemData(null, null, s);
                FCItemDataList.Add(itemData.Name, itemData);
                futuresCSymbolList.Add(itemData.Name);

                n++;
            }
        }
        #region Form1_Load vars
        delegate void MarkUpdates(BinanceFuturesUsdtStreamMarkPrice data, ItemData itemData);
        MarkUpdates markUpdates;
        delegate void AggUpdates(BinanceStreamAggregatedTrade data, ItemData itemData);
        AggUpdates aggUpdates;
        delegate void KlineUpdates(IBinanceStreamKlineData data, ItemData itemData);
        KlineUpdates klineUpdates;
        #endregion
        void Form1_Load(object sender, EventArgs e)
        {
            /*
            SetItemDataList();

            aggUpdates += new AggUpdates(OnAggregatedTradeUpdates);
            klineUpdates += new KlineUpdates(OnKlineUpdates);

            var symbols = 0;
            for (int i = 0; i < 5; i++)
            {
                socketClient.Spot.SubscribeToKlineUpdates(spotSymbolList[i], KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, spotItemDataList[data.Symbol]); });
                symbols += spotSymbolList[i].Count;
            }
            spotKlineReqTextBox.Text = "/" + symbols + "(K)";
            
            socketClient.FuturesUsdt.SubscribeToKlineUpdates(futureUSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, futureUItemDataList[data.Symbol]); });
            futureUKlineReqTextBox.Text = "/" + futureUSymbolList.Count + "(K)";

            socketClient.FuturesCoin.SubscribeToKlineUpdates(futureCSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, futureCItemDataList[data.Symbol]); });
            futureCKlineReqTextBox.Text = "/" + futureCSymbolList.Count + "(K)";*/

            markUpdates += new MarkUpdates(OnMarkPriceUpdates);

            foreach (var itemData in FUItemDataList)
                if (itemData.Value.Position)
                    itemData.Value.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Value.Name, 3000,
                        data => { BeginInvoke(markUpdates, data, itemData.Value); }).Data;
        }


        #region OnKlineUpdates vars
        short spotAggOnNow = 0;
        short futureUAggOnNow = 0;
        short futureCAggOnNow = 0;
        short spotKlineRcv = 0;
        short futureUKlineRcv = 0;
        short futureCKlineRcv = 0;
        #endregion
        void OnKlineUpdates(IBinanceStreamKlineData data, ItemData itemData)
        {
            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;

                if (itemData.isSpot)
                {
                    spotKlineRcv++;
                    spotKlineRcvTextBox.Text = spotKlineRcv.ToString();
                }
                else if (itemData.isFutureUsdt)
                {
                    futureUKlineRcv++;
                    futureUKlineRcvTextBox.Text = futureUKlineRcv.ToString();
                }
                else
                {
                    futureCKlineRcv++;
                    futureCKlineRcvTextBox.Text = futureCKlineRcv.ToString();
                }
            }

            if (data.Data.Final)
            {
                if (data.Data.High + data.Data.Low < data.Data.Close * 2)
                    itemData.RateBfor = Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                else
                    itemData.RateBfor = -Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);

                if (Math.Abs(itemData.RateBfor) > 1)
                {
                    if (itemData.isSpot && !itemData.isAggReady && itemData.RateBfor > 1 && data.Data.TradeCount > 180)
                    {
                        itemData.isAggReady = true;
                        itemData.AggReadyRow = 0;
                        spotListView.AddObject(itemData);
                    }
                    else if (!itemData.isSpot && !itemData.isAggReady && data.Data.TradeCount > 180)
                    {
                        if (itemData.RateBfor > 1)
                            itemData.LorS = true;
                        else
                            itemData.LorS = false;
                        itemData.isAggReady = true;
                        itemData.AggReadyRow = 0;
                        if (itemData.isFutureUsdt)
                            futureUListView.AddObject(itemData);
                        else
                            futureCListView.AddObject(itemData);
                    }
                    itemData.FlatMinRow = 0;
                    itemData.AggReadyRow++;
                }
                else if (itemData.isAggReady && itemData.FlatMinRow < 5)
                {
                    itemData.FlatMinRow++;
                    itemData.AggReadyRow++;
                }
                else if (itemData.isAggReady && !itemData.isChartShowing)
                {
                    socketClient.Unsubscribe(itemData.sub);
                    itemData.isAggReady = false;
                    itemData.isAggOn = false;
                    itemData.aggFirst = true;
                    if (itemData.isSpot)
                    {
                        spotListView.RemoveObject(itemData);
                        spotAggOnNow--;
                        spotAggRcv--;
                        spotAggReq--;
                        spotAggRcvTextBox.Text = spotAggRcv.ToString();
                        spotAggReqTextBox.Text = spotAggReq.ToString();
                    }
                    else if (itemData.isFutureUsdt)
                    {
                        futureUListView.RemoveObject(itemData);
                        futureUAggOnNow--;
                        futureUAggRcv--;
                        futureUAggReq--;
                        futureUAggRcvTextBox.Text = futureUAggRcv.ToString();
                        futureUAggReqTextBox.Text = futureUAggReq.ToString();
                    }
                    else
                    {
                        futureCListView.RemoveObject(itemData);
                        futureCAggOnNow--;
                        futureCAggRcv--;
                        futureCAggReq--;
                        futureCAggRcvTextBox.Text = futureCAggRcv.ToString();
                        futureCAggReqTextBox.Text = futureCAggReq.ToString();
                    }
                }

                if (itemData.isAggReady)
                {
                    if (itemData.isSpot)
                        spotListView.RefreshObject(itemData);
                    else if (itemData.isFutureUsdt)
                        futureUListView.RefreshObject(itemData);
                    else
                        futureCListView.RefreshObject(itemData);
                }

                if (itemData.isAggReady && !itemData.isAggOn &&
                    ((itemData.isSpot && spotAggOnNow < 5) || (!itemData.isSpot && itemData.isFutureUsdt && futureUAggOnNow < 5) || (!itemData.isSpot && !itemData.isFutureUsdt && futureCAggOnNow < 5)))
                    SetAggOn(itemData, false);

                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;
                itemData.minStick.TCount = data.Data.TradeCount;
                itemData.minStickList.Add(itemData.minStick);


                if (itemData.isChartShowing)
                {
                    UpdateChartPoint(chart2, itemData.minStick);

                    itemData.minStick = new Stick();

                    itemData.minStick.Price[2] = data.Data.Close;
                    itemData.minStick.Time = data.Data.OpenTime.AddMinutes(1);
                    AddStartChartPoint(chart2, itemData.minStick);
                    AdjustChart(chart2);
                }
                else
                    itemData.minStick = new Stick();
            }
            else if (itemData.isChartShowing)
            {
                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;
                itemData.minStick.TCount = data.Data.TradeCount;

                if (itemData.minStick.Time.ToString("HH:mm") != chart2.Series[0].Points[chart2.Series[0].Points.Count - 1].AxisLabel.Substring(3, 5))
                    AddStartChartPoint(chart2, itemData.minStick);

                UpdateChartPoint(chart2, itemData.minStick);
            }
        }
        #region OnAggregatedTradeUpdates vars
        short spotAggRcv = 0;
        short futureUAggRcv = 0;
        short futureCAggRcv = 0;
        #endregion
        void OnAggregatedTradeUpdates(BinanceStreamAggregatedTrade data, ItemData itemData)
        {
            if (!itemData.isAggReady || !itemData.isAggOn)
                return;

            if (itemData.aggFirst)
            {
                itemData.aggFirst = false;

                if (itemData.isSpot)
                {
                    spotAggRcv++;
                    spotAggRcvTextBox.Text = spotAggRcv.ToString();
                }
                else if (itemData.isFutureUsdt)
                {
                    futureUAggRcv++;
                    futureUAggRcvTextBox.Text = futureUAggRcv.ToString();
                }
                else
                {
                    futureCAggRcv++;
                    futureCAggRcvTextBox.Text = futureCAggRcv.ToString();
                }
            }

            itemData.date = data.TradeTime.ToString("yyyy-MM-dd");
            itemData.newTime = data.TradeTime.ToString(ItemData.secChartLabel);

            if (data.TradeTime.Second != itemData.secStick.Time.Second || data.TradeTime.Minute != itemData.secStick.Time.Minute || data.TradeTime.Hour != itemData.secStick.Time.Hour)
            {
                if (itemData.secStick.Price[1] != decimal.Zero)
                {
                    itemData.secStickList.Add(itemData.secStick);

                    {   // 시뮬레이션용
                        if (itemData.secStickList.Count <= ItemData.amt0)
                        {
                            itemData.secMsList0Tot += itemData.secStickList.Last().Ms;
                            itemData.secMdList0Tot += itemData.secStickList.Last().Md;
                        }
                        else
                        {
                            itemData.secMsList0Tot = itemData.secMsList0Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt0].Ms + itemData.secStickList.Last().Ms;
                            itemData.secMsList0Avg = itemData.secMsList0Tot / ItemData.amt0;
                            itemData.secMdList0Tot = itemData.secMdList0Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt0].Md + itemData.secStickList.Last().Md;
                            itemData.secMdList0Avg = itemData.secMdList0Tot / ItemData.amt0;
                        }
                        if (itemData.secStickList.Count <= ItemData.amt1)
                        {
                            itemData.secMsList1Tot += itemData.secStickList.Last().Ms;
                            itemData.secMdList1Tot += itemData.secStickList.Last().Md;
                        }
                        else
                        {
                            itemData.secMsList1Tot = itemData.secMsList1Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt1].Ms + itemData.secStickList.Last().Ms;
                            itemData.secMsList1Avg = itemData.secMsList1Tot / ItemData.amt1;
                            itemData.secMdList1Tot = itemData.secMdList1Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt1].Md + itemData.secStickList.Last().Md;
                            itemData.secMdList1Avg = itemData.secMdList1Tot / ItemData.amt1;

                            if (!itemData.chuQReady)
                                itemData.chuQReady = true;
                        }

                        if (itemData.secStickList.Last().Ms > itemData.secStickList.Last().Md)
                        {
                            itemData.pureSecCountQ.Enqueue(1);
                            itemData.pureSecCountQTot = itemData.pureSecCountQTot - itemData.pureSecCountQ.Dequeue() + 1;
                        }
                        else
                        {
                            itemData.pureSecCountQ.Enqueue(0);
                            itemData.pureSecCountQTot = itemData.pureSecCountQTot - itemData.pureSecCountQ.Dequeue();
                        }
                        itemData.pureSecCountQAvg = itemData.pureSecCountQTot / itemData.pureSecCountQ.Count;
                    }
                }

                itemData.secStick = new Stick();

                itemData.secStick.Price[2] = data.Price;
                itemData.secStick.Price[0] = data.Price;
                itemData.secStick.Price[1] = data.Price;

                itemData.secStick.Time = data.TradeTime;

                if (itemData.isChartShowing)
                {
                    AddStartChartPoint(chart1, itemData.secStick);

                    if (chart1.Series[0].Points.Count == baseChartViewSticksSize + 1)
                    {
                        chart1.ChartAreas[0].RecalculateAxesScale();
                        chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count - baseChartViewSticksSize + 1, chart1.Series[0].Points.Count);
                        chart1.ChartAreas[0].RecalculateAxesScale();
                    }

                    AdjustChart(chart1);
                }

                if (itemData.secStickList.Count > 900)
                {
                    itemData.secStickList.RemoveAt(0);
                    if (itemData.isChartShowing)
                    {
                        chart1.Series[0].Points.RemoveAt(0);
                        chart1.Series[1].Points.RemoveAt(0);
                        chart1.Series[2].Points.RemoveAt(0);
                    }
                }
            }

            if (data.Price > itemData.secStick.Price[0])
                itemData.secStick.Price[0] = data.Price;
            else if (data.Price < itemData.secStick.Price[1])
                itemData.secStick.Price[1] = data.Price;
            itemData.secStick.Price[3] = data.Price;

            if (data.BuyerIsMaker)
                itemData.secStick.Md += data.Quantity;
            else
                itemData.secStick.Ms += data.Quantity;

            {       // 시뮬레이션
                if (!itemData.buyOrder && itemData.chuQReady
                    &&
                    (
                       ((itemData.isSpot || (!itemData.isSpot && itemData.LorS)) && BuyConditionMs(itemData))
                       ||
                       (!itemData.isSpot && !itemData.LorS && BuyConditionMd(itemData))
                    )
                )
                {
                    itemData.hasAll = true;
                    itemData.buyOrder = true;
                    itemData.sellOrder = false;

                    itemData.bhtPrc = itemData.secStick.Price[3];
                    itemData.bhtTime = itemData.newTime;
                }
                else if (itemData.buyOrder && itemData.hasAll && !itemData.sellOrder)
                {
                    if (!itemData.tooManyAmt
                        &&
                        (
                            ((itemData.isSpot || (!itemData.isSpot && itemData.LorS)) && BuyCondition2ndMs(itemData))
                            ||
                            (!itemData.isSpot && !itemData.LorS && BuyCondition2ndMd(itemData))
                        )
                    )
                    {
                        itemData.tooManyAmt = true;
                        itemData.tooManyAmtTime = itemData.newTime;
                    }

                    if (itemData.bhtTime != itemData.newTime
                        &&
                        (
                            ((itemData.isSpot || (!itemData.isSpot && itemData.LorS)) && SellConditionMs(itemData))
                            ||
                            (!itemData.isSpot && !itemData.LorS && SellConditionMd(itemData))
                            ||
                            (
                                itemData.tooManyAmt && itemData.tooManyAmtTime != itemData.newTime
                                &&
                                (
                                    ((itemData.isSpot || (!itemData.isSpot && itemData.LorS)) && SellCondition2ndMs(itemData))
                                    ||
                                    (!itemData.isSpot && !itemData.LorS && SellCondition2ndMd(itemData))
                                )
                            )
                        )
                    )
                    {
                        if (!itemData.isSpot && !itemData.LorS)
                            itemData.profitRate = itemData.bhtPrc / data.Price * 0.999m / 1.001m;
                        else
                            itemData.profitRate = data.Price / itemData.bhtPrc * 0.999m / 1.001m;

                        if (itemData.profitRate > 1.002m)
                            itemData.win++;
                        else
                            itemData.lose++;
                        itemData.winLoseTot++;
                        itemData.WinPrecantage = Math.Round((double)itemData.win / itemData.winLoseTot, 2) + "(" + itemData.winLoseTot + ")";

                        itemData.profitRateSum += itemData.profitRate;
                        itemData.profitRateMul *= itemData.profitRate;
                        itemData.ProfitRateSumAndMul = Math.Round(((double)itemData.profitRateSum / itemData.winLoseTot - 1) * 100, 2) + "(" +
                            Math.Round((Math.Pow((double)itemData.profitRateMul, (double)1 / itemData.winLoseTot) - 1) * 100, 2) + ")";

                        if (itemData.isSpot)
                            spotListView.RefreshObject(itemData);
                        else if (itemData.isFutureUsdt)
                            futureUListView.RefreshObject(itemData);
                        else
                            futureCListView.RefreshObject(itemData);

                        itemData.hasAll = false;
                        itemData.buyOrder = false;
                        itemData.sellOrder = false;
                    }
                }
            }

            if (itemData.isChartShowing)
            {
                timeDiffTextBox.Text = Math.Round((DateTime.UtcNow - data.TradeTime).TotalSeconds, 1).ToString();
                amtTextBox.Text = data.Quantity.ToString(ForDecimalString);
                priceTextBox.Text = data.Price.ToString(ForDecimalString);
                if (data.BuyerIsMaker)
                {
                    amtTextBox.ForeColor = mdAmtColor;
                    priceTextBox.ForeColor = mdAmtColor;
                    timeDiffTextBox.ForeColor = mdAmtColor;
                }
                else
                {
                    amtTextBox.ForeColor = msAmtColor;
                    priceTextBox.ForeColor = msAmtColor;
                    timeDiffTextBox.ForeColor = msAmtColor;
                }

                UpdateChartPoint(chart1, itemData.secStick);
            }
        }
        void OnMarkPriceUpdates(BinanceFuturesUsdtStreamMarkPrice data, ItemData itemData)
        {
            if (itemData.Position)
            {
                itemData.MarkPrice = Math.Round(data.MarkPrice, 2);
                itemData.notianalValue = data.MarkPrice * Math.Abs(itemData.Size);
                itemData.InitialMargin = Math.Round(itemData.notianalValue / itemData.Leverage, 2);
                itemData.PNL = Math.Round((itemData.MarkPrice - itemData.EntryPrice) * itemData.Size, 2);
                itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                if (itemData.notianalValue <= itemData.brackets[itemData.nowBracketIndex].Floor)
                    itemData.nowBracketIndex--;
                else if (itemData.notianalValue > itemData.brackets[itemData.nowBracketIndex].Cap)
                    itemData.nowBracketIndex++;

                itemData.maintMargin = itemData.notianalValue * itemData.brackets[itemData.nowBracketIndex].MaintenanceMarginRatio;

                FUMaintMargin = 0;
                FUMarginBalance = FUBalance;
                FUAvailableBalance = FUBalance;
                foreach (ItemData i in FUPositionListView.Objects)
                {
                    FUMaintMargin += i.maintMargin;
                    FUMarginBalance += i.PNL;
                    FUAvailableBalance = FUAvailableBalance + i.PNL - i.InitialMargin;
                }
                FUMaintMarginTextBox1.Text = Math.Round(FUMaintMargin, 2).ToString();
                FUMarginBalanceTextBox1.Text = Math.Round(FUMarginBalance, 2).ToString();
                FUAvailBalanceTextBox1.Text = Math.Round(FUAvailableBalance, 2).ToString();
                FUMarginRatioTextBox1.Text = Math.Round(FUMaintMargin / FUMarginBalance * 100, 2).ToString();

                FUPositionListView.UpdateObject(itemData);
            }
        }

        #region SetAggOn vars
        short spotAggReq = 0;
        short futureUAggReq = 0;
        short futureCAggReq = 0;
        #endregion
        void SetAggOn(ItemData itemData, bool addObject)
        {
            itemData.isAggReady = true;
            itemData.isAggOn = true;

            itemData.hasAll = false;
            itemData.buyOrder = false;
            itemData.sellOrder = false;
            itemData.chuQReady = false;

            itemData.win = 0;
            itemData.lose = 0;
            itemData.winLoseTot = 0;
            itemData.WinPrecantage = "0.00(0)";
            itemData.profitRateSum = 0;
            itemData.profitRateMul = 1m;
            itemData.ProfitRateSumAndMul = "0.00(0)";

            itemData.secStickList.Clear();
            itemData.secStick = new Stick();
            if (itemData.isSpot)
            {
                itemData.sub = socketClient.Spot.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, SPOTItemDataList[data2.Symbol]); }).Data;
                if (addObject)
                    spotListView.AddObject(itemData);
                spotListView.RemoveObject(itemData);
                spotListView.InsertObjects(0, new List<ItemData> { itemData });
                spotListView.RefreshObject(itemData);
                spotAggOnNow++;
                spotAggReq++;
                spotAggReqTextBox.Text = "/" + spotAggReq + "(A)";
            }
            else if (itemData.isFutureUsdt)
            {
                itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, FUItemDataList[data2.Symbol]); }).Data;
                if (addObject)
                    futureUListView.AddObject(itemData);
                futureUListView.RemoveObject(itemData);
                futureUListView.InsertObjects(0, new List<ItemData> { itemData });
                futureUListView.RefreshObject(itemData);
                futureUAggOnNow++;
                futureUAggReq++;
                futureUAggReqTextBox.Text = "/" + futureUAggReq + "(A)";
            }
            else
            {
                itemData.sub = socketClient.FuturesCoin.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, FCItemDataList[data2.Symbol]); }).Data;
                if (addObject)
                    futureCListView.AddObject(itemData);
                futureCListView.RemoveObject(itemData);
                futureCListView.InsertObjects(0, new List<ItemData> { itemData });
                futureCListView.RefreshObject(itemData);
                futureCAggOnNow++;
                futureCAggReq++;
                futureCAggReqTextBox.Text = "/" + futureCAggReq + "(A)";
            }
        }
        bool BuyConditionMs(ItemData itemData)
        {
            if (
                (
                    itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.secMsList1Avg * 2
                    && itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.secMdList1Avg * 1.5m
                    && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMsList1Avg * 2
                    && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMdList1Avg * 1.5m
                    &&
                    (
                        itemData.pureSecCountQAvg < 0.8m
                        ||
                        (
                            itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.secMsList0Avg * 2
                            && itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.secMdList0Avg * 1.5m
                            && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMsList0Avg * 2
                            && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMdList0Avg * 1.5m
                        )
                    )
                )
                ||
                BuyCondition2ndMs(itemData)
            )
            {
                itemData.tooManyAmt = false;
                if (BuyCondition2ndMs(itemData))
                {
                    itemData.tooManyAmt = true;
                    itemData.tooManyAmtTime = itemData.newTime;
                }

                return true;
            }
            else
                return false;
        }
        bool BuyConditionMd(ItemData itemData)
        {
            if (
                (
                    itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.secMdList1Avg * 2
                    && itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.secMsList1Avg * 1.5m
                    && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMdList1Avg * 2
                    && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMsList1Avg * 1.5m
                    &&
                    (
                        itemData.pureSecCountQAvg > 0.2m
                        ||
                        (
                            itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.secMdList0Avg * 2
                            && itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.secMsList0Avg * 1.5m
                            && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMdList0Avg * 2
                            && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMsList0Avg * 1.5m
                        )
                    )
                )
                || 
                BuyCondition2ndMd(itemData)
            )
            {
                itemData.tooManyAmt = false;
                if (BuyCondition2ndMd(itemData))
                {
                    itemData.tooManyAmt = true;
                    itemData.tooManyAmtTime = itemData.newTime;
                }

                return true;
            }
            else
                return false;
        }
        bool BuyCondition2ndMs(ItemData itemData)
        {
            if (itemData.secStick.Ms - itemData.secStick.Md > itemData.secMsList1Avg * 12
                && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMdList1Avg * 10
                && itemData.secStick.Ms / (itemData.secStick.Ms + itemData.secStick.Md) > 0.9m)
                return true;
            else
                return false;
        }
        bool BuyCondition2ndMd(ItemData itemData)
        {
            if (itemData.secStick.Md - itemData.secStick.Ms > itemData.secMdList1Avg * 12
                && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMsList1Avg * 10
                && itemData.secStick.Md / (itemData.secStick.Ms + itemData.secStick.Md) > 0.9m)
                return true;
            else
                return false;
        }
        bool SellConditionMs(ItemData itemData)
        {
            if (itemData.secStickList.Last().Md > itemData.secMsList0Avg
                && itemData.secStickList.Last().Md > itemData.secMsList1Avg * 2
                && itemData.secStickList.Last().Md > itemData.secStickList.Last().Ms)
                return true;
            else
                return false;
        }
        bool SellConditionMd(ItemData itemData)
        {
            if (itemData.secStickList.Last().Ms > itemData.secMdList0Avg
                && itemData.secStickList.Last().Ms > itemData.secMdList1Avg * 2
                && itemData.secStickList.Last().Ms > itemData.secStickList.Last().Md)
                return true;
            else
                return false;
        }
        bool SellCondition2ndMs(ItemData itemData)
        {
            if (itemData.secStickList.Last().Ms < itemData.secMsList1Avg)
                return true;
            else
                return false;
        }
        bool SellCondition2ndMd(ItemData itemData)
        {
            if (itemData.secStickList.Last().Md < itemData.secMdList1Avg)
                return true;
            else
                return false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (tokenSource != null)
                tokenSource.Cancel();

            socketClient.UnsubscribeAll();
            //dbHelper.Close();
        }

        void ListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            if (((ItemData)e.Model).isAggOn)
            {
                var itemData = (ItemData)e.Model;

                if (itemData.isSpot || itemData.LorS)
                    e.Item.ForeColor = plsPrcColor;
                else
                    e.Item.ForeColor = mnsPrcColor;
            }
        }
        void ListView_SelectionChanged(object sender, EventArgs e)
        {
            if (((FastDataListView)sender).SelectedIndices.Count != 1)
                return;

            ShowChart(((FastDataListView)sender).SelectedObject as ItemData);
        }

        void nameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (marketComboBox.SelectedItem.ToString())
            {
                case "S":
                    if (!SPOTItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(SPOTItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                case "F_U":
                    if (!FUItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(FUItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                case "F_C":
                    if (!FCItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(FCItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                default:
                    break;
            }
        }
    }
}
