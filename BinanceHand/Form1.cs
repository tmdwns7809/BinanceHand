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
using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Futures.Socket;
using Binance.Net.Objects.Models.Spot.Socket;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Sockets;
using Binance.Net.Objects.Models.Spot;
using Binance.Net.Interfaces;
using System.Windows.Forms.DataVisualization.Charting;
using Binance.Net.Objects.Models.Futures;
using TradingLibrary.Trading;
using TradingLibrary;
using TradingLibrary.Base;
using TradingLibrary.Base.Enum;
using TradingLibrary.Base.DB;
using TradingLibrary.Base.Values;
using TradingLibrary.Base.Values.Chart;
using TradingLibrary.Base.Weight;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Binance.Net.Objects.Options;

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        string AssetTextUnrealizedPNL = "Unrealized PNL";
        string AssetTextMarginRatio = "Margin Ratio";
        string AssetTextMaintenanceMargin = "Maintenance Margin";
        string AssetTextMarginBalance = "Margin Balance";

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
        TextBox maximumBalanceForSizeTextBox0 = new TextBox();
        TextBox maximumBalanceForSizeTextBox1 = new TextBox();
        TextBox maximumPercentOfAvgSecForSizeTextBox0 = new TextBox();
        TextBox maximumPercentOfAvgSecForSizeTextBox1 = new TextBox();

        bool testnet = false;

        BinanceRestClient client;
        BinanceSocketClient socketClient;

        List<string> symbolList = new List<string>();

        int[] EnterCount = new int[2];
        object CountLocker = new object();

        bool klineFirstFinal = true;
        bool klineSecondFinal = true;

        decimal autoLimitAvgSecPercent = 1;

        Dictionary<string, BinanceItemData>[] positions = new Dictionary<string, BinanceItemData>[] { new Dictionary<string, BinanceItemData>(), new Dictionary<string, BinanceItemData>() };
        Dictionary<string, BinanceItemData>[] orders = new Dictionary<string, BinanceItemData>[] { new Dictionary<string, BinanceItemData>(), new Dictionary<string, BinanceItemData>() };

        public Form1()
        {
            InitializeComponent();

            FormClosed += Form1_FormClosed;
            Load += Form1_Load;

            //Trading.instance = new Trading(this, Settings.ProgramBinanceFutures, 8.412m, 20);
            Trading.instance = new Trading(this, Settings.ProgramBinanceFutures, 8.4103m, 20);
            Trading.instance.HoONandOFF += Trading_HoONandOFF;
            Trading.instance.AggONandOFF += Trading_AggONandOFF;
            Trading.instance.ShowChartAdditional += Trading_ShowChartAdditional;
            Trading.instance.ResetOrderView += Trading_ResetOrderView;
            Trading.instance.LoadMoreAdditional += Trading_LoadMoreAdditional;
            Trading.instance.PlaceOrder += Trading_PlaceOrder;
            Trading.instance.CodeListView.Columns.Remove(Trading.instance.CodeListView.GetColumn("Name"));
            Trading.instance.CodeListView2.Columns.Remove(Trading.instance.CodeListView2.GetColumn("Name"));
            Trading.instance.realHandResultListView.Columns.Remove(Trading.instance.realHandResultListView.GetColumn("Name"));
            Trading.instance.realAutoResultListView.Columns.Remove(Trading.instance.realAutoResultListView.GetColumn("Name"));
            foreach (var listView in Trading.instance.simulResultListView)
                listView.Columns.Remove(listView.GetColumn("Name"));

            BaseFunctions.baseInstance = Trading.instance;

            SetOrderView();

            SetClientAndKey();

            TradingLibrary.Base.DB.Binance.FuturesUSD.StartThread();
            DBManager.BaseName = TradingLibrary.Base.DB.Binance.FuturesUSD.BASE_NAME;
            TradingLibrary.Base.DB.Binance.FuturesUSD.SetDB();

            KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {

                case System.Windows.Forms.Keys.Enter:
                    var itemData = BaseFunctions.baseInstance.showingItemData as BinanceItemData;
                    if (itemData != null && leverageTextBox0.Focused)
                    {
                        var le = int.Parse(leverageTextBox0.Text);
                        var data = client.UsdFuturesApi.Account.ChangeInitialLeverageAsync(itemData.Code, le).Result;
                        if (!data.Success)
                            Error.Show();
                        itemData.Leverage = le;

                        Alert.Start("change success", false, true);
                    }
                    break;

                default:
                    return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        void SetOrderView()
        {
            new List<Control>()
                {
                    GTCRadioButton, IOCRadioButton, PORadioButton, leverageTextBox0, leverageTextBox1,
                    orderPriceTextBox0, orderPriceTextBox1, orderPriceTextBox2, marketRadioButton,
                    orderSizeTextBox0, orderSizeTextBox1, miniSizeCheckBox, ROCheckBox,
                    autoSizeCheckBox, maximumBalanceForSizeTextBox0, maximumBalanceForSizeTextBox1,
                    maximumPercentOfAvgSecForSizeTextBox0, maximumPercentOfAvgSecForSizeTextBox1
                }.ForEach(new Action<Control>((control) => { Trading.instance.orderGroupBox.Controls.Add(control); }));

            Trading.instance.SetRadioButton_CheckBox(GTCRadioButton, "GTC",
                new Size(40, 16), new Point(5, 15));

            Trading.instance.SetRadioButton_CheckBox(IOCRadioButton, "IOC",
                GTCRadioButton.Size, new Point(GTCRadioButton.Location.X + GTCRadioButton.Size.Width + 5, GTCRadioButton.Location.Y));
            //marketRadioButton.CheckedChanged += (sender, e) => { if (marketRadioButton.Checked) orderPriceTextBox1.Enabled = false; else orderPriceTextBox1.Enabled = true; };

            Trading.instance.SetRadioButton_CheckBox(PORadioButton, "PO",
              GTCRadioButton.Size, new Point(IOCRadioButton.Location.X + IOCRadioButton.Size.Width + 5, IOCRadioButton.Location.Y));

            Trading.instance.SetTextBox(leverageTextBox0, "", true);
            leverageTextBox0.Size = new Size(41, 21);
            leverageTextBox0.Location = new Point(PORadioButton.Location.X + PORadioButton.Width + 5, PORadioButton.Location.Y);
            leverageTextBox0.ReadOnly = false;
            leverageTextBox0.BorderStyle = BorderStyle.Fixed3D;

            Trading.instance.SetTextBox(leverageTextBox1, "/ 125", true);
            leverageTextBox1.Size = new Size(41, 21);
            leverageTextBox1.Location = new Point(leverageTextBox0.Location.X + leverageTextBox0.Width + 5, leverageTextBox0.Location.Y + 5);

            Trading.instance.SetTextBox(orderPriceTextBox0, "Price", true);
            orderPriceTextBox0.Size = new Size(35, 21);
            orderPriceTextBox0.Location = new Point(GTCRadioButton.Location.X, GTCRadioButton.Location.Y + GTCRadioButton.Size.Height + 5 + (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);

            Trading.instance.SetTextBox(orderPriceTextBox1, "", true);
            orderPriceTextBox1.Size = orderPriceTextBox0.Size;
            orderPriceTextBox1.Location = new Point(orderPriceTextBox0.Location.X + orderPriceTextBox0.Size.Width + 3, orderPriceTextBox0.Location.Y - (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);
            orderPriceTextBox1.ReadOnly = false;
            orderPriceTextBox1.BackColor = ColorSet.ControlBack;

            Trading.instance.SetTextBox(orderPriceTextBox2, "%", true);
            orderPriceTextBox2.Size = orderPriceTextBox0.Size;
            orderPriceTextBox2.Location = new Point(orderPriceTextBox1.Location.X + orderPriceTextBox1.Size.Width + 3, orderPriceTextBox0.Location.Y);

            Trading.instance.SetRadioButton_CheckBox(marketRadioButton, "Market",
                new Size(100, GTCRadioButton.Height), new Point(orderPriceTextBox2.Location.X + orderPriceTextBox2.Size.Width + 5, orderPriceTextBox2.Location.Y));

            Trading.instance.SetTextBox(orderSizeTextBox0, "Size", true);
            orderSizeTextBox0.Size = orderPriceTextBox0.Size;
            orderSizeTextBox0.Location = new Point(orderPriceTextBox0.Location.X, orderPriceTextBox0.Location.Y + orderPriceTextBox0.Size.Height + 5 + (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);

            Trading.instance.SetTextBox(orderSizeTextBox1, "", true);
            orderSizeTextBox1.Size = orderPriceTextBox0.Size;
            orderSizeTextBox1.Location = new Point(orderSizeTextBox0.Location.X + orderSizeTextBox0.Size.Width + 3, orderSizeTextBox0.Location.Y - (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);
            orderSizeTextBox1.ReadOnly = false;
            orderSizeTextBox1.BackColor = ColorSet.ControlBack;

            Trading.instance.SetRadioButton_CheckBox(miniSizeCheckBox, "Min",
                new Size(50, GTCRadioButton.Height), new Point(orderSizeTextBox1.Location.X + orderSizeTextBox1.Width, orderSizeTextBox0.Location.Y));
            miniSizeCheckBox.MouseUp += (sender, e) => { if (Trading.instance.showingItemData != null) OrderBoxSetting(autoSizeCheckBox.Checked, miniSizeCheckBox.Checked); };

            Trading.instance.SetRadioButton_CheckBox(ROCheckBox, "RO",
                miniSizeCheckBox.Size, new Point(miniSizeCheckBox.Location.X + miniSizeCheckBox.Width + 5, miniSizeCheckBox.Location.Y));

            Trading.instance.SetRadioButton_CheckBox(autoSizeCheckBox, "Auto Size",
                new Size(50, GTCRadioButton.Height),
                new Point(orderSizeTextBox0.Location.X + 10, orderSizeTextBox0.Location.Y + orderSizeTextBox0.Size.Height + 5 + (maximumBalanceForSizeTextBox0.Size.Height - autoSizeCheckBox.Size.Height) / 2));
            autoSizeCheckBox.MouseUp += (sender, e) => { if (Trading.instance.showingItemData != null) OrderBoxSetting(autoSizeCheckBox.Checked, miniSizeCheckBox.Checked); };

            Trading.instance.SetTextBox(maximumBalanceForSizeTextBox0, "", true);
            maximumBalanceForSizeTextBox0.Size = new Size(50, GTCRadioButton.Height);
            maximumBalanceForSizeTextBox0.Location = new Point(autoSizeCheckBox.Location.X + autoSizeCheckBox.Size.Width + 3, 
                autoSizeCheckBox.Location.Y - (maximumBalanceForSizeTextBox0.Size.Height - maximumBalanceForSizeTextBox1.Size.Height) / 2);
            maximumBalanceForSizeTextBox0.ReadOnly = false;
            maximumBalanceForSizeTextBox0.BackColor = ColorSet.ControlBack;

            Trading.instance.SetTextBox(maximumBalanceForSizeTextBox1, "% of Balance", true);
            maximumBalanceForSizeTextBox1.Size = new Size(100, GTCRadioButton.Height);
            maximumBalanceForSizeTextBox1.Location = new Point(maximumBalanceForSizeTextBox0.Location.X + maximumBalanceForSizeTextBox0.Size.Width + 3, 
                maximumBalanceForSizeTextBox0.Location.Y + (maximumBalanceForSizeTextBox0.Size.Height - maximumBalanceForSizeTextBox1.Size.Height) / 2);

            Trading.instance.SetTextBox(maximumPercentOfAvgSecForSizeTextBox0, "", true);
            maximumPercentOfAvgSecForSizeTextBox0.Size = maximumBalanceForSizeTextBox0.Size;
            maximumPercentOfAvgSecForSizeTextBox0.Location = new Point(maximumBalanceForSizeTextBox0.Location.X, maximumBalanceForSizeTextBox0.Location.Y + maximumBalanceForSizeTextBox0.Height + 5);
            maximumPercentOfAvgSecForSizeTextBox0.ReadOnly = false;
            maximumPercentOfAvgSecForSizeTextBox0.BackColor = ColorSet.ControlBack;

            Trading.instance.SetTextBox(maximumPercentOfAvgSecForSizeTextBox1, "% of AvgSecQuan", true);
            maximumPercentOfAvgSecForSizeTextBox1.Size = maximumBalanceForSizeTextBox1.Size;
            maximumPercentOfAvgSecForSizeTextBox1.Location = new Point(maximumBalanceForSizeTextBox1.Location.X, maximumBalanceForSizeTextBox1.Location.Y + maximumBalanceForSizeTextBox1.Height + 5);

            Trading.instance.buyButton.Size = new Size(Trading.instance.orderGroupBox.Width / 3 - 10, Trading.instance.orderGroupBox.Height - maximumPercentOfAvgSecForSizeTextBox0.Location.Y - 30);
            Trading.instance.buyButton.Location = new Point(5, maximumPercentOfAvgSecForSizeTextBox0.Location.Y + maximumPercentOfAvgSecForSizeTextBox0.Height + 5);
            Trading.instance.buyButton.BringToFront();

            Trading.instance.positionCloseButton.Size = Trading.instance.buyButton.Size;
            Trading.instance.positionCloseButton.Location = new Point(Trading.instance.buyButton.Location.X + Trading.instance.buyButton.Width + 10, Trading.instance.buyButton.Location.Y);
            Trading.instance.positionCloseButton.BringToFront();

            Trading.instance.sellButton.Size = Trading.instance.buyButton.Size;
            Trading.instance.sellButton.Location = new Point(Trading.instance.positionCloseButton.Location.X + Trading.instance.positionCloseButton.Width + 10, Trading.instance.buyButton.Location.Y);
            Trading.instance.sellButton.BringToFront();
        }
        void SetClientAndKey()
        {
            //var clientOption = new BinanceClientOptions();
            client = new BinanceRestClient(delegate (BinanceRestOptions options)
            {
                if (!testnet)
                {
                    options.UsdFuturesOptions.AutoTimestamp = false;
                    options.UsdFuturesOptions.TradeRulesBehaviour = TradeRulesBehaviour.None;
                }
            });
            //var socketOption = new BinanceSocketClientOptions();
            socketClient = new BinanceSocketClient(delegate (BinanceSocketOptions options)
            {
                if (!testnet)
                {
                    options.ReconnectInterval = TimeSpan.FromMinutes(1);
                }
            });

            if (testnet)       //testnet
            {
                if (true)
                {   /*future*/
                    //clientOption.BaseAddressUsdtFutures = "https://testnet.binancefuture.com";
                    //clientOption.BaseAddressCoinFutures = "https://testnet.binancefuture.com";
                    //clientOption.ApiCredentials = new ApiCredentials(BinanceBase.testnet_API_Key, BinanceBase.testnet_Secret_Key);
                    //socketOption.BaseAddressUsdtFutures = "wss://stream.binancefuture.com";
                    //socketOption.BaseAddressCoinFutures = "wss://stream.binancefuture.com";
                    //socketOption.BaseAddress = "wss://stream.binancefuture.com";
                    //socketOption.ApiCredentials = new ApiCredentials(BinanceBase.testnet_API_Key, BinanceBase.testnet_Secret_Key);
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
                client.SetApiCredentials(new ApiCredentials(TradingLibrary.Base.Keys.future2_API_Key, TradingLibrary.Base.Keys.future2_Secret_Key));
                socketClient.SetApiCredentials(new ApiCredentials(TradingLibrary.Base.Keys.future2_API_Key, TradingLibrary.Base.Keys.future2_Secret_Key));
                //clientOption.ApiCredentials = new BinanceApiCredentials();
                //clientOption.UsdFuturesApiOptions.AutoTimestamp = false;
                //clientOption.UsdFuturesApiOptions.TradeRulesBehaviour = TradeRulesBehaviour.None;
                //socketOption.ApiCredentials = new BinanceApiCredentials();
                //socketOption.AutoReconnect = true;
                //socketOption.UsdFuturesStreamsOptions.ReconnectInterval = TimeSpan.FromMinutes(1);
                //clientOption.LogVerbosity = LogVerbosity.Debug;
                //clientOption.LogWriters = new List<TextWriter> { Console.Out };
            }

            //client = new BinanceClient(clientOption);
            //socketClient = new BinanceSocketClient(socketOption);

            //socketClient.UnsubscribeAllAsync().Wait();
        }

        void Form1_Load(object sender, EventArgs e)
        {
            SetItemDataList();

            SubscribeToUserStream(testnet, true);

            GetAccountInfo();

            Trading.instance.LoadItemData();

            var result = socketClient.UsdFuturesApi.SubscribeToKlineUpdatesAsync(symbolList, KlineInterval.OneMinute, OnKlineUpdates).Result;
            if (!result.Success)
                Error.Show();

            Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
        }
        void SetItemDataList()
        {
            var result = client.UsdFuturesApi.ExchangeData.GetExchangeInfoAsync().Result;
            if (!result.Success)
                Error.Show();

            BinanceWeightManager.UpdateWeightNow(result.ResponseHeaders);

            BinanceWeightManager.UpdateLimit(result.Data.RateLimits);

            var n = 0;
            var newSymbolList = new List<string>();
            var noSymbolList = new List<string>();
            var lastSymbol = "";
            
            foreach (var s in result.Data.Symbols)
                if (s.Status == SymbolStatus.Trading && s.Name.Contains("USDT"))
                        lastSymbol = s.Name.Trim().ToUpper();

            foreach (var s in result.Data.Symbols)
            {
                var code = s.Name.Trim().ToUpper();
                if (s.Status != SymbolStatus.Trading)
                {
                    noSymbolList.Add(code);
                    continue;
                }

                if (!s.Name.Contains("USDT")
                    ||
                    (s.Name != "BTCUSDT" && s.Name != "EOSUSDT"
                    //&& s.Name != lastSymbol
                    //&& s.Name != "WLDUSDT" && s.Name != "EOSUSDT"
                    //&& s.Name != "MYROUSDT"
                    )
                    )
                    continue;

                n++;

                if (BaseFunctions.itemDataDic.ContainsKey(code))
                {
                    BaseFunctions.itemDataDic[code].number = n;
                    continue;
                }

                var itemData = new BinanceItemData(s, n);
                BaseFunctions.itemDataDic.Add(itemData.Code, itemData);
                symbolList.Add(code);
                newSymbolList.Add(code);
                Trading.instance.CodeListView.AddObject(itemData);
                TradingLibrary.Base.DB.Binance.FuturesUSD.requestTRTaskQueue.Enqueue(new Task(() => {
                    var result1 = client.UsdFuturesApi.Account.GetBracketsAsync(itemData.Code).Result;
                    if (!result1.Success)
                        Error.Show();

                    foreach (var brackets in result1.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    //var data = client.UsdFuturesApi.Account.ChangeInitialLeverageAsync(itemData.Code, 1).Result;
                    //if (!data.Success)
                    //    Error.Show(this, "leverage change fail");
                    //itemData.Leverage = 1;
                }));
            }
            TradingLibrary.Base.DB.Binance.FuturesUSD.codesCount = BaseFunctions.itemDataDic.Count;

            if (n != BaseFunctions.itemDataDic.Count)
            {
                if (newSymbolList.Count != 0)   // 무조건 noSymbol만 온다고 가정
                    Error.Show();

                foreach (var code in noSymbolList)
                    if (BaseFunctions.itemDataDic.ContainsKey(code))
                    {
                        var conn = TradingLibrary.Base.DB.Binance.FuturesUSD.DBDic[ChartTimeSet.OneMinute];
                        conn.Close();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        conn.Open();

                        new SQLiteCommand("Begin", conn).ExecuteNonQuery();

                        new SQLiteCommand("CREATE TABLE IF NOT EXISTS '" + code + "'" +
                            " ('" + Columns.TIME + "' INTEGER" +
                            ", '" + Columns.OPEN + "' TEXT, '" + Columns.HIGH + "' TEXT" +
                            ", '" + Columns.LOW + "' TEXT, '" + Columns.CLOSE + "' TEXT" +
                            ", '" + Columns.BASE_VOLUME + "' TEXT, '" + Columns.TAKER_BUY_BASE_VOLUME + "' TEXT" +
                            ", '" + Columns.QUOTE_VOLUME + "' TEXT, '" + Columns.TAKER_BUY_QUOTE_VOLUME + "' TEXT" +
                            ", '" + Columns.TRADE_COUNT + "' INTEGER)", conn).ExecuteNonQuery();

                        new SQLiteCommand("CREATE UNIQUE INDEX IF NOT EXISTS '" + code + "_index'" +
                            " ON '" + code + "' ('" + Columns.TIME + "')", conn).ExecuteNonQuery();

                        var reader = new SQLiteCommand("SELECT *, rowid FROM '" + code + "' ORDER BY rowid DESC LIMIT 1", conn).ExecuteReader();
                        DateTime startTime = default;
                        if (reader.Read())
                        {
                            startTime = DateTime.ParseExact(reader[Columns.TIME].ToString(), Formats.TIME, null);
                            new SQLiteCommand("DELETE FROM '" + code + "' WHERE rowid='" + reader["rowid"].ToString() + "'", conn).ExecuteNonQuery();
                        }

                        var itemData = BaseFunctions.itemDataDic[code];
                        var v = itemData.listDic[ChartTimeSet.OneMinute];
                        var first = true;
                        
                        foreach (var tradeStick in v.list)
                        {
                            if (tradeStick.Time < startTime)
                                continue;
                            else if (first)
                            {
                                if (tradeStick.Time != startTime)
                                    Error.Show();

                                first = false;
                            }

                            var stick = tradeStick.original as IBinanceStreamKline;
                            new SQLiteCommand("INSERT INTO '" + code + "'" +
                                " ('" + Columns.TIME + "'" +
                                ", '" + Columns.OPEN + "', '" + Columns.HIGH + "'" +
                                ", '" + Columns.LOW + "', '" + Columns.CLOSE + "'" +
                                ", '" + Columns.BASE_VOLUME + "', '" + Columns.TAKER_BUY_BASE_VOLUME + "'" +
                                ", '" + Columns.QUOTE_VOLUME + "', '" + Columns.TAKER_BUY_QUOTE_VOLUME + "'" +
                                ", '" + Columns.TRADE_COUNT + "') values" +
                                " ('" + stick.OpenTime.ToString(Formats.DB_TIME) + "'" +
                                ", '" + stick.OpenPrice + "', '" + stick.HighPrice + "'" +
                                ", '" + stick.LowPrice + "', '" + stick.ClosePrice + "'" +
                                ", '" + stick.Volume + "', '" + stick.TakerBuyBaseVolume + "'" +
                                ", '" + stick.QuoteVolume + "', '" + stick.TakerBuyQuoteVolume + "'" +
                                ", '" + stick.TradeCount + "')", conn).ExecuteNonQuery();
                        }

                        var stick2 = v.lastStick.original as IBinanceStreamKline;
                        new SQLiteCommand("INSERT INTO '" + code + "'" +
                            " ('" + Columns.TIME + "'" +
                            ", '" + Columns.OPEN + "', '" + Columns.HIGH + "'" +
                            ", '" + Columns.LOW + "', '" + Columns.CLOSE + "'" +
                            ", '" + Columns.BASE_VOLUME + "', '" + Columns.TAKER_BUY_BASE_VOLUME + "'" +
                            ", '" + Columns.QUOTE_VOLUME + "', '" + Columns.TAKER_BUY_QUOTE_VOLUME + "'" +
                            ", '" + Columns.TRADE_COUNT + "') values" +
                            " ('" + stick2.OpenTime.ToString(Formats.DB_TIME) + "'" +
                            ", '" + stick2.OpenPrice + "', '" + stick2.HighPrice + "'" +
                            ", '" + stick2.LowPrice + "', '" + stick2.ClosePrice + "'" +
                            ", '" + stick2.Volume + "', '" + stick2.TakerBuyBaseVolume + "'" +
                            ", '" + stick2.QuoteVolume + "', '" + stick2.TakerBuyQuoteVolume + "'" +
                            ", '" + stick2.TradeCount + "')", conn).ExecuteNonQuery();

                        new SQLiteCommand("Commit", conn).ExecuteNonQuery();

                        conn.Close();

                        BaseFunctions.itemDataDic.Remove(code);
                        Trading.instance.CodeListView.RemoveObject(itemData);
                        Trading.instance.CodeListView.Refresh();
                        Trading.instance.CodeListView2.RemoveObject(itemData);
                        Trading.instance.CodeListView2.Refresh();
                        symbolList.Remove(code);
                        Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
                    }
            }
            else if (symbolList.Count != newSymbolList.Count)
            {
                var result2 = socketClient.UsdFuturesApi.SubscribeToKlineUpdatesAsync(newSymbolList, KlineInterval.OneMinute, OnKlineUpdates).Result;
                if (!result2.Success)
                    Error.Show();

                Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
            }

            if (newSymbolList.Count != 0 && TradingLibrary.Base.DB.Binance.FuturesUSD.requestTRTaskQueue.Count == 0)
                Error.Show();
        }
        List<TradeStick> LoadSticks(string Code, KlineInterval interval, DateTime startTime, DateTime endTime)
        {
            var vc = CandleBaseFunctions.ChartValuesDic[interval];

            var plusStartTime = startTime.AddSeconds(-vc.seconds * (Strategy.IndNeedDays - 1));
            var size = (int)endTime.Subtract(plusStartTime).TotalSeconds / vc.seconds + 1;
            var result2 = client.UsdFuturesApi.ExchangeData.GetKlinesAsync(Code, interval, plusStartTime, endTime, size).Result;

            if (!result2.Success)
            {
                Error.Show();
                return default;
            }

            BinanceWeightManager.UpdateWeightNow(result2.ResponseHeaders);

            return SetRSIAandGetList(result2.Data.ToList(), startTime, vc);
        }
        TradeStick GetStick(IBinanceKline stickReal, ChartValues cv)
        {
            var stick = new TradeStick(cv);
            stick.Price[0] = stickReal.HighPrice;
            stick.Price[1] = stickReal.LowPrice;
            stick.Price[2] = stickReal.OpenPrice;
            stick.Price[3] = stickReal.ClosePrice;

            stick.Ms = stickReal.TakerBuyBaseVolume;
            stick.Md = stickReal.Volume - stickReal.TakerBuyBaseVolume;

            stick.Time = stickReal.OpenTime;

            return stick;
        }
        TradeStick CalLastStick(List<TradeStick> minList, DateTime time, ChartValues cv)
        {
            var stick = new TradeStick(cv) { Time = time };

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
        List<TradeStick> SetRSIAandGetList(List<IBinanceKline> data, DateTime startTime, ChartValues vc)
        {
            var list = new List<TradeStick>();
            foreach (var stickReal in data)
            {
                var stick = GetStick(stickReal, vc);
                Strategy.SetRSIAandDiff(list, stick, int.MinValue, int.MinValue, vc);
                list.Add(stick);
            }

            var startIndex = BaseFunctions.GetStartIndex(list, startTime);
            if (startIndex > 0)
                list.RemoveRange(0, startIndex);

            return list;
        }
        List<TradeStick> GetList(List<IBinanceKline> data, ChartValues cv)
        {
            var list = new List<TradeStick>();
            foreach (var stickReal in data)
            {
                var stick = GetStick(stickReal, cv);
                list.Add(stick);
            }

            return list;
        }

        void SubscribeToUserStream(bool testnet, bool testnetFutures)
        {
            var result = ((testnet && !testnetFutures) ? client.SpotApi.Account.StartUserStreamAsync() : client.UsdFuturesApi.Account.StartUserStreamAsync()).Result;
            if (!result.Success)
                Error.Show();

            BinanceWeightManager.UpdateWeightNow(result.ResponseHeaders);

            var listenKey = result.Data;

            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(29 * 60000);

                    var result1 = ((testnet && !testnetFutures) ? client.SpotApi.Account.KeepAliveUserStreamAsync(listenKey) : client.UsdFuturesApi.Account.KeepAliveUserStreamAsync(listenKey)).Result;
                    if (!result1.Success)
                        Error.Show();

                    BinanceWeightManager.UpdateWeightNow(result1.ResponseHeaders);

                    var count = 0;
                    foreach (TradeItemData item in BaseFunctions.itemDataDic.Values)
                        if (item.Last30minStickFluc > 3)
                            count++;

                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, Trading.instance.NowTime().ToString(Formats.TIME),
                        DBHelper.conn1DetectHistoryColumn1, "직전 1시간 이내 3% 이상 변동 종목 개수", DBHelper.conn1DetectHistoryColumn2, count.ToString());
                }
            }, BaseFunctions.tokenSource.Token);

            var result2 = 
                socketClient.UsdFuturesApi.SubscribeToUserDataUpdatesAsync(listenKey,
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
                        },
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        },
                        data => {
                            var a = data;
                        }).Result;
            if (!result2.Success)
                Error.Show();
        }
        void GetAccountInfo()
        {
            Trading.instance.assetDic.Add(Trading.AssetTextWalletBalance, new Asset { AssetName = Trading.AssetTextWalletBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[Trading.AssetTextWalletBalance]);
            Trading.instance.assetDic.Add(AssetTextUnrealizedPNL, new Asset { AssetName = AssetTextUnrealizedPNL });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextUnrealizedPNL]);
            Trading.instance.assetDic.Add(AssetTextMarginBalance, new Asset { AssetName = AssetTextMarginBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMarginBalance]);
            Trading.instance.assetDic.Add(AssetTextMaintenanceMargin, new Asset { AssetName = AssetTextMaintenanceMargin });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMaintenanceMargin]);
            Trading.instance.assetDic.Add(AssetTextMarginRatio, new Asset { AssetName = AssetTextMarginRatio });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[AssetTextMarginRatio]);
            Trading.instance.assetDic.Add(Trading.AssetTextAvailableBalance, new Asset { AssetName = Trading.AssetTextAvailableBalance });
            Trading.instance.assetsListView.AddObject(Trading.instance.assetDic[Trading.AssetTextAvailableBalance]);

            var result = client.UsdFuturesApi.Account.GetAccountInfoAsync().Result;
            if (!result.Success)
                Error.Show();

            BinanceWeightManager.UpdateWeightNow(result.ResponseHeaders);

            foreach (var s in result.Data.Assets)
                if (s.Asset == "USDT")
                    lock (Trading.instance.assetLocker)
                    {
                        Trading.instance.assetDic[AssetTextMaintenanceMargin].Amount = s.MaintMargin;
                        Trading.instance.assetDic[AssetTextMarginBalance].Amount = s.MarginBalance;
                        Trading.instance.assetDic[Trading.AssetTextAvailableBalance].Amount = s.AvailableBalance;
                        Trading.instance.assetDic[Trading.AssetTextWalletBalance].Amount = s.WalletBalance;
                        if (s.MarginBalance > 0)
                            Trading.instance.assetDic[AssetTextMarginRatio].Amount = Math.Round(s.MaintMargin / s.MarginBalance * 100, 2);
                    }
            foreach (var s in result.Data.Positions)
                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)BaseFunctions.itemDataDic[s.Symbol];

                    var result1 = socketClient.UsdFuturesApi.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates).Result;
                    if (!result1.Success)
                        Error.Show();

                    itemData.markSub = result1.Data;

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2);
                    itemData.maintMargin = s.MaintMargin;

                    Trading.instance.CodeListView.RemoveObject(itemData);
                    Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });
                    if (itemData.star)
                    {
                        Trading.instance.CodeListView2.RemoveObject(itemData);
                        Trading.instance.CodeListView2.InsertObjects(0, new List<BinanceItemData> { itemData });
                    }
                }

            var result2 = client.UsdFuturesApi.Account.GetPositionInformationAsync().Result;
            if (!result2.Success)
                Error.Show();

            BinanceWeightManager.UpdateWeightNow(result2.ResponseHeaders);

            foreach (var s in result2.Data)
            {
                //if (s.MarginType == FuturesMarginType.Isolated)
                //{
                //    var result3 = client.UsdFuturesApi.Account.ChangeMarginTypeAsync(s.Symbol, FuturesMarginType.Cross).Result;
                //    if (!result3.Success)
                //        Error.Show();

                //    BaseFunctions.BinanceUpdateWeightNow(result3.ResponseHeaders);
                //}

                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)BaseFunctions.itemDataDic[s.Symbol];

                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    itemData.Size = s.Quantity;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.RealEnterPrice = s.EntryPrice;
                    itemData.PNL = Math.Round(s.UnrealizedPnl, 2);
                    itemData.ClosePNL = itemData.PNL;
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);
                    itemData.maxNotionalValue = s.MaxNotional;
                    itemData.RealPosition = itemData.Size > 0 ? Position.Long : Position.Short;
                    itemData.RealEnterN = (int)itemData.RealPosition + 1;
                    positions[(int)itemData.RealPosition].Add(itemData.Code, itemData);
                    orders[(int)itemData.RealPosition].Remove(itemData.Code);
                    itemData.RealEnter = true;

                    UpdateAssets();

                    Trading.instance.CodeListView.RefreshObject(itemData);
                    Trading.instance.CodeListView2.RefreshObject(itemData);

                    var result4 = client.UsdFuturesApi.Account.GetBracketsAsync(s.Symbol).Result;
                    if (!result4.Success)
                        Error.Show();

                    BinanceWeightManager.UpdateWeightNow(result4.ResponseHeaders);

                    foreach (var brackets in result4.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                }
            }
        }

        decimal GetMinQuan(BinanceItemData itemData, decimal price)
        {
            if (price == default)
                return 0;
            else
                return (int)(itemData.minNotionalValue / price / itemData.minSize + 1) * itemData.minSize;
        }
        decimal GetCurrentPrice(TradeItemData itemData)
        {
            return itemData.AggOn && !itemData.AggFirst ? itemData.secStick.Price[3] : 
                (itemData.listDic[ChartTimeSet.OneMinute].lastStick == default ? default : itemData.listDic[ChartTimeSet.OneMinute].lastStick.Price[3]);
        }
        void OrderBoxSetting(bool autoSize, bool minSize)
        {
            var itemDataShowing = Trading.instance.showingItemData as BinanceItemData;

            if (itemDataShowing.RealEnter && autoSize && minSize)
                Error.Show();
            else if (itemDataShowing.RealEnter && autoSize && !minSize)
            {
                if (Trading.instance.positionExitPriceMarket)
                    marketRadioButton.Checked = true;
                else
                    GTCRadioButton.Checked = true;

                orderSizeTextBox1.Enabled = false;
                orderSizeTextBox1.Text = Math.Abs(itemDataShowing.Size).ToString();

                miniSizeCheckBox.AutoCheck = false;
                miniSizeCheckBox.Checked = false;

                ROCheckBox.AutoCheck = true;
                ROCheckBox.Checked = true;

                autoSizeCheckBox.AutoCheck = true;
                autoSizeCheckBox.Checked = true;
                maximumBalanceForSizeTextBox0.Enabled = false;
                maximumPercentOfAvgSecForSizeTextBox0.Enabled = false;
                maximumBalanceForSizeTextBox0.Text = "-";
                maximumPercentOfAvgSecForSizeTextBox0.Text = "-";
            }
            else if (itemDataShowing.RealEnter && !autoSize && minSize)
                Error.Show();
            else if (itemDataShowing.RealEnter && !autoSize && !minSize)
            {
                if (Trading.instance.positionExitPriceMarket)
                    marketRadioButton.Checked = true;
                else
                    GTCRadioButton.Checked = true;

                orderSizeTextBox1.Enabled = true;
                orderSizeTextBox1.Text = Math.Abs(itemDataShowing.Size).ToString();

                miniSizeCheckBox.AutoCheck = false;
                miniSizeCheckBox.Checked = false;

                ROCheckBox.AutoCheck = true;
                ROCheckBox.Checked = true;

                autoSizeCheckBox.AutoCheck = true;
                autoSizeCheckBox.Checked = false;
                maximumBalanceForSizeTextBox0.Enabled = false;
                maximumPercentOfAvgSecForSizeTextBox0.Enabled = false;
                maximumBalanceForSizeTextBox0.Text = "-";
                maximumPercentOfAvgSecForSizeTextBox0.Text = "-";
            }
            else if (!itemDataShowing.RealEnter && autoSize && minSize)
                Error.Show();
            else if (!itemDataShowing.RealEnter && autoSize && !minSize)
            {
                IOCRadioButton.Checked = true;

                orderSizeTextBox1.Enabled = false;
                orderSizeTextBox1.Text = "auto";

                miniSizeCheckBox.AutoCheck = false;
                miniSizeCheckBox.Checked = false;

                ROCheckBox.AutoCheck = false;
                ROCheckBox.Checked = false;

                autoSizeCheckBox.AutoCheck = true;
                autoSizeCheckBox.Checked = true;
                maximumBalanceForSizeTextBox0.Enabled = true;
                maximumPercentOfAvgSecForSizeTextBox0.Enabled = true;
                maximumBalanceForSizeTextBox0.Text = Trading.instance.maximumBalancePercent.ToString();
                maximumPercentOfAvgSecForSizeTextBox0.Text = Trading.instance.maximumPercentOfAvgSec.ToString();
            }
            else if (!itemDataShowing.RealEnter && !autoSize && minSize)
            {
                IOCRadioButton.Checked = true;

                orderSizeTextBox1.Enabled = false;
                orderSizeTextBox1.Text = GetMinQuan(itemDataShowing, GetCurrentPrice(itemDataShowing)).ToString(Formats.FOR_DECIMAL_STRING);

                miniSizeCheckBox.AutoCheck = true;
                miniSizeCheckBox.Checked = true;

                ROCheckBox.AutoCheck = false;
                ROCheckBox.Checked = false;

                autoSizeCheckBox.AutoCheck = false;
                autoSizeCheckBox.Checked = false;
                maximumBalanceForSizeTextBox0.Enabled = false;
                maximumPercentOfAvgSecForSizeTextBox0.Enabled = false;
                maximumBalanceForSizeTextBox0.Text = "-";
                maximumPercentOfAvgSecForSizeTextBox0.Text = "-";
            }
            else if (!itemDataShowing.RealEnter && !autoSize && !minSize)
            {
                IOCRadioButton.Checked = true;

                orderSizeTextBox1.Enabled = true;
                orderSizeTextBox1.Text = GetMinQuan(itemDataShowing, GetCurrentPrice(itemDataShowing)).ToString(Formats.FOR_DECIMAL_STRING);

                miniSizeCheckBox.AutoCheck = true;
                miniSizeCheckBox.Checked = false;

                ROCheckBox.AutoCheck = false;
                ROCheckBox.Checked = false;

                autoSizeCheckBox.AutoCheck = true;
                autoSizeCheckBox.Checked = false;
                maximumBalanceForSizeTextBox0.Enabled = false;
                maximumPercentOfAvgSecForSizeTextBox0.Enabled = false;
                maximumBalanceForSizeTextBox0.Text = "-";
                maximumPercentOfAvgSecForSizeTextBox0.Text = "-";
            }
        }

        void OnKlineUpdates(DataEvent<IBinanceStreamKlineData> data0)
        {
            var data = data0.Data;
            if (!BaseFunctions.itemDataDic.ContainsKey(data.Symbol))
                SetItemDataList();

            var itemData = BaseFunctions.itemDataDic[data.Symbol] as BinanceItemData;

            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;
                lock (Trading.minLocker)
                    if (klineFirstFinal && Trading.instance.KlineRcv < Trading.instance.KlineReq)
                        Trading.instance.UpdateReqRcv(Trading.instance.KlineRcvTextBox, Trading.instance.KlineRcv + 1);
            }

            if (data.Data.Final)
            {
                var vm = itemData.listDic[ChartTimeSet.OneMinute];
                var newStick = GetStick(data.Data, ChartTimeSet.OneMinute);

                lock (Trading.minLocker)
                {
                    if (newStick.Time > Trading.firstFinalMin)
                    {
                        if (klineFirstFinal)
                            klineFirstFinal = false;
                        else if (klineSecondFinal)
                            klineSecondFinal = false;
                        else if (Trading.instance.KlineRcv != Trading.instance.KlineReq)
                            SetItemDataList();

                        Trading.firstFinalMin = newStick.Time;
                        Trading.instance.KlineRcv = 0;
                        Trading.instance.UpdateFoundInText(default, true);
                        if (Trading.loadingDone)
                            Strategy.foundItemList = new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>[]
                            {
                                new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>(),
                                new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>()
                            };
                    }

                    Trading.instance.KlineRcv++;
                }

                if (newStick.Time != Trading.firstFinalMin)
                    Error.Show();

                if (Trading.instance.KlineRcv % 20 == 0 || Trading.instance.KlineRcv == Trading.instance.KlineReq)
                    Trading.instance.UpdateReqRcv(Trading.instance.KlineRcvTextBox, Trading.instance.KlineRcv);

                if (Trading.loadingDone)
                    for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                    {
                        itemData.positionData[j].foundList = new List<(DateTime foundTime, ChartValues chartValues)>();
                        itemData.positionData[j].found = false;
                    }

                itemData.RSIAHighest = 0;
                itemData.RSIAHighestChart = "";
                itemData.dropLongCount = 0;
                itemData.dropShortCount = 0;
                itemData.dropLongShortest = TimeSpan.Zero;
                itemData.dropShortShortest = TimeSpan.Zero;
                itemData.dropLongText = "";
                itemData.dropShortText = "";
                var dropLongFirstVC = Strategy.maxCV.index;
                var dropShortFirstVC = Strategy.maxCV.index;

                for (int i = ChartTimeSet.OneMinute.index; i <= Strategy.maxCV.index; i++)
                {
                    var v = itemData.listDic.Values[i];
                    var vc = itemData.listDic.Keys[i];

                    if (v.first)
                    {
                        if (Strategy.maxCV.index > ChartTimeSet.OneWeek.index)
                            Error.Show();
                        else if (vc == ChartTimeSet.OneMinute)
                        {
                            v.lastStick = new TradeStick(vc) { Time = newStick.Time, original = data.Data };

                            TradingLibrary.Base.DB.Binance.FuturesUSD.requestTRTaskQueue.Enqueue(new Task(() =>
                            {
                                TradingLibrary.Base.DB.Binance.FuturesUSD.UpdateDB(itemData.Code, newStick.Time, client, this, BaseFunctions.loadingListBox);

                                var vm2 = itemData.listDic[ChartTimeSet.OneMinute];
                                for (int j = Strategy.minCV.index; j <= Strategy.maxCV.index; j++)
                                {
                                    var list = new List<TradeStick>();

                                    var vc2 = itemData.listDic.Keys[j];

                                    var conn = TradingLibrary.Base.DB.Binance.FuturesUSD.DBDic[itemData.listDic.Keys[j]];
                                    conn.Open();
                                    var reader2 = new SQLiteCommand("SELECT * FROM '" + itemData.Code + "' WHERE " +
                                        "(" + Columns.TIME + "<='" + newStick.Time.ToString(Formats.TIME) + "') AND " +
                                        "(" + Columns.TIME + ">='" +
                                            newStick.Time.Subtract(Strategy.ReadyTimeToCheckBeforeStart).Date.
                                            AddSeconds(-vc2.seconds * (Strategy.IndNeedDays + Strategy.BaseLoadNeedDays - 1)).ToString(Formats.DB_TIME) + "')", conn).ExecuteReader();
                                    while (reader2.Read())
                                        list.Add(TradingLibrary.Base.DB.Binance.FuturesUSD.GetTradeStickFromSQL(reader2, vc));
                                    conn.Close();

                                    var v2 = itemData.listDic.Values[j];

                                    TradeStick firstStick = default;
                                    TradeStick lastStick = default;
                                    lock (itemData.listDicLocker)
                                        firstStick = v2.list.Count == 0 ? v2.lastStick : v2.list[0];

                                    if (list[list.Count - 1].Time != firstStick.Time)
                                        Error.Show();
                                    else
                                    {
                                        lastStick = list.Last();
                                        list.RemoveAt(list.Count - 1);
                                    }

                                    if (j > ChartTimeSet.OneMinute.index)
                                    {
                                        lock (itemData.listDicLocker)
                                            for (int k = 0; k < vm2.list.Count; k++)
                                                if (vm2.list[k].Time < firstStick.Time)
                                                    continue;
                                                else
                                                {
                                                    if (vm2.list[k].Time >= newStick.Time)
                                                    {
                                                        if (vm2.list[k].Time > newStick.Time)
                                                            Error.Show();

                                                        break;
                                                    }

                                                    if (vm2.list[k].Time == firstStick.Time)
                                                        firstStick.Price[2] = vm2.list[k].Price[2];

                                                    if (vm2.list[k].Price[0] > firstStick.Price[0])
                                                        firstStick.Price[0] = vm2.list[k].Price[0];
                                                    if (vm2.list[k].Price[1] < firstStick.Price[1])
                                                        firstStick.Price[1] = vm2.list[k].Price[1];

                                                    firstStick.Ms += vm2.list[k].Ms;
                                                    firstStick.Md += vm2.list[k].Md;

                                                    firstStick.TCount += vm2.list[k].TCount;
                                                }

                                        if (lastStick.Time != firstStick.Time)
                                            Error.Show();
                                    }

                                    lock (itemData.listDicLocker)
                                        v2.list.InsertRange(0, list);
                                }

                                if (TradingLibrary.Base.DB.Binance.FuturesUSD.doneCount == TradingLibrary.Base.DB.Binance.FuturesUSD.codesCount)
                                    Task.Run(() =>
                                    {
                                        var startTime = Trading.firstFinalMin;
                                        if (Strategy.ReadyTimeToCheckBeforeStart > TimeSpan.Zero)
                                        {
                                            Log.Add(this, BaseFunctions.loadingListBox, "Start: " + startTime.ToString(Formats.TIME) + "(simulating " + Strategy.ST + "...)");
                                            RunStartSimul(startTime);
                                        }
                                        else
                                        {
                                            if (Trading.instance.NowTime().Second > 55)
                                            {
                                                var nowTime = Trading.instance.NowTime();
                                                Thread.Sleep(63000 - nowTime.Second * 1000 - nowTime.Millisecond);
                                            }

                                            if (Trading.instance.KlineRcv < Trading.instance.KlineReq)
                                                while (Trading.instance.KlineRcv != Trading.instance.KlineReq)
                                                {
                                                    Thread.Sleep(100);
                                                    if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                                        Error.Show();
                                                }
                                            else if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                                Error.Show();

                                            startTime = Trading.firstFinalMin;
                                        }
                                        Log.Add(this, BaseFunctions.loadingListBox, "Start: " + startTime.ToString(Formats.TIME) + "(" + Trading.instance.NowTime().ToString(Formats.TIME) + ")");
                                        Trading.loadingDone = true;
                                        Trading.loadingDoneTime = startTime;
                                    });
                            }));
                        }
                        else
                            v.lastStick = new TradeStick(vc) { Time = newStick.Time.AddSeconds(-(int)newStick.Time.TimeOfDay.TotalSeconds % vc.seconds) };

                        v.first = false;
                    }

                    var timeDiff = newStick.Time.Subtract(v.lastStick.Time).TotalSeconds;
                    if (timeDiff > vc.seconds || timeDiff < 0)
                        Error.Show();
                    else if (timeDiff == vc.seconds)
                    {
                        lock (itemData.listDicLocker)
                            v.list.Add(v.lastStick);
                        v.lastStick = new TradeStick(vc) { Time = newStick.Time };

                        if (vc == ChartTimeSet.OneMinute)
                            v.lastStick.original = data.Data;

                        if ((int)v.lastStick.Time.Subtract(v.list[v.list.Count - 1].Time).TotalSeconds != vc.seconds)
                            Error.Show();
                    }

                    if (v.lastStick.Price[1] == 0)
                    {
                        v.lastStick.Price[1] = newStick.Price[1];
                        v.lastStick.Price[2] = newStick.Price[2];
                    }

                    lock (itemData.listDicLocker)
                    {
                        if (newStick.Price[0] > v.lastStick.Price[0])
                            v.lastStick.Price[0] = newStick.Price[0];
                        if (newStick.Price[1] < v.lastStick.Price[1])
                            v.lastStick.Price[1] = newStick.Price[1];
                        v.lastStick.Price[3] = newStick.Price[3];

                        v.lastStick.Ms += newStick.Ms;
                        v.lastStick.Md += newStick.Md;

                        v.lastStick.TCount += newStick.TCount;
                    }

                    if (Trading.loadingDone)
                    {
                        Strategy.SetRSIAandDiff(v.list, v.lastStick, int.MinValue, int.MinValue, vc);
                        if (i >= Strategy.minCV.index && i <= Strategy.maxCV.index)
                            Strategy.ChartFindConditionAndAdd(itemData, vc, vm.lastStick, v.lastStick);

                        if (v.lastStick.indicator != null && !double.IsNaN(v.lastStick.indicator.RSIA) && Math.Abs(v.lastStick.indicator.RSIA) > Math.Abs(itemData.RSIAHighest))
                        {
                            itemData.RSIAHighest = (int)v.lastStick.indicator.RSIA;
                            itemData.RSIAHighestChart = vc.Text;
                        }

                        if (v.lastStick.lastFoundStick != null)
                        {
                            var c = (int)v.lastStick.Time.Subtract(v.lastStick.lastFoundStick.Time).TotalSeconds / vc.seconds;
                            if (c <= Strategy.BaseLoadNeedDays)
                            {
                                var t = vm.lastStick.Time.Subtract(v.lastStick.lastFoundStick.Time);
                                if (v.lastStick.lastFoundStick.indicator.RSIA > 0)
                                {
                                    if (itemData.dropLongShortest == TimeSpan.Zero || t < itemData.dropLongShortest)
                                        itemData.dropLongShortest = t;

                                    itemData.dropLongCount++;
                                    itemData.dropLongText += vc.Text + ":" + c + " ";
                                    if (itemData.dropLongCount == 1)
                                        dropLongFirstVC = vc.index;
                                }
                                else
                                {
                                    if (itemData.dropShortShortest == TimeSpan.Zero || t < itemData.dropShortShortest)
                                        itemData.dropShortShortest = t;

                                    itemData.dropShortCount++;
                                    itemData.dropShortText += vc.Text + ":" + c + " ";
                                    if (itemData.dropShortCount == 1)
                                        dropShortFirstVC = vc.index;
                                }

                                if (itemData.star && vm.lastStick.Time == v.lastStick.Time && vm.lastStick.Time.Subtract(Trading.loadingDoneTime).TotalMinutes > 3 &&
                                    (v.list[v.list.Count - 1].lastFoundStick == null || v.list[v.list.Count - 1].lastFoundStick.Time != v.lastStick.lastFoundStick.Time) &&
                                    vc.index - (v.lastStick.lastFoundStick.indicator.RSIA > 0 ? dropLongFirstVC : dropShortFirstVC) >= 0)
                                    Alert.Start(Math.Round(v.lastStick.lastFoundStick.indicator.RSIA, 2) + "\n" +
                                        itemData.Code + "\n" + newStick.Time.ToString(Formats.TIME) + "\n" + vc.Text, true, true);
                            }
                        }

                        if (vc == ChartTimeSet.OneHour && v.list.Count > 0)
                        {
                            var lastStick2 = v.list[v.list.Count - 1];
                            itemData.LastHour = Math.Round((lastStick2.Price[3] > lastStick2.Price[2] ? (lastStick2.Price[3] / lastStick2.Price[2] - 1) : -(lastStick2.Price[2] / lastStick2.Price[3] - 1)) * 100, 2);
                        }
                    }
                }

                if (Trading.loadingDone)
                    for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                    {
                        var positionData = itemData.positionData[j];

                        var foundCount = 0;
                        var foundText = "";

                        if (positionData.found)
                        {
                            //Alert.Start(Enum.GetName(typeof(Position), j) + "\n" + itemData.Code + "\n" + newStick.Time.ToString(Formats.TIME) + "\n" + positionData.foundList[positionData.foundList.Count - 1].chartValues.Text, true);

                            foundCount = positionData.foundList.Count - 1;
                            foundText = positionData.foundList[positionData.foundList.Count - 1].chartValues.Text;
                            Trading.instance.UpdateFoundInText(true);
                        }

                        if (j == (int)Position.Long)
                        {
                            itemData.foundListCountLong = foundCount;
                            itemData.foundListLongHighestText = foundText;
                        }
                        else
                        {
                            itemData.foundListCount = foundCount + itemData.foundListCountLong;
                            itemData.foundListCountShort = foundCount;
                            itemData.foundListShortHighestText = foundText;
                        }

                        if (!itemData.positionData[(int)Position.Long].Enter && !itemData.positionData[(int)Position.Short].Enter && positionData.found)
                            lock (Trading.minLocker)
                                Strategy.foundItemList[j].Add(itemData.number, (itemData, positionData.foundList));
                        else
                        if (positionData.Enter)
                        {
                            var v = itemData.listDic[positionData.EnterFoundForExit.chartValues];
                            if (Strategy.ExitConditionFinal(itemData, (Position)j, vm.lastStick, v.lastStick, v.list.Count - 1))
                            {
                                Trading.TradingSimulExitSetting(itemData, positionData, !Strategy.calSimul || positionData.Real);

                                if (itemData.isAuto && itemData.RealEnter)
                                {
                                    itemData.DetectTime = newStick.Time;
                                    if (!Trading_PlaceOrder(itemData, (Position)j == Position.Long ? Position.Short : Position.Long, false, true))
                                        Error.Show(this, "order fail");

                                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, Trading.instance.NowTime().ToString(Formats.TIME) + "~" + positionData.EnterTime.ToString(Formats.TIME),
                                        DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 매도(자동)", DBHelper.conn1OrderHistoryColumn2, "전송가:" + newStick.Price[3]);
                                }

                                var profitRow = (double)((Position)j == Position.Long ? vm.lastStick.Price[3] / positionData.EnterPrice : positionData.EnterPrice / vm.lastStick.Price[3]);
                                var resultData = new BackResultData()
                                {
                                    Code = itemData.Code,
                                    EnterTime = positionData.EnterTime,
                                    ExitTime = vm.lastStick.Time,
                                    ProfitRate = Math.Round((profitRow - 1) * 100, 2),
                                    LorS = (Position)j
                                };

                                if (itemData.resultDataForMetric[j] != null)
                                {
                                    if (itemData.resultDataForMetric[j].Code == itemData.Code)
                                        itemData.resultDataForMetric[j].ExitTime = resultData.ExitTime;
                                    lock (itemData.resultDataForMetric[j].locker)
                                    {
                                        itemData.resultDataForMetric[j].Count++;
                                        itemData.resultDataForMetric[j].ProfitRate += profitRow;
                                        if ((profitRow - 1) * 100 > BaseFunctions.commisionRate + BaseFunctions.slippage)
                                        {
                                            itemData.resultDataForMetric[j].WinCount++;
                                            itemData.resultDataForMetric[j].WinProfitRateSum += profitRow;
                                        }
                                        itemData.resultDataForMetric[j].doneResults.Add(resultData);
                                        itemData.resultDataForMetric[j].ingItems.Remove(itemData.Code);
                                    }
                                    itemData.resultDataForMetric[j] = null;
                                }
                            }
                        }

                        if (Trading.instance.KlineRcv == Trading.instance.KlineReq)
                        {
                            var conditionResult2 = Strategy.AllItemFindCondition();
                            if (conditionResult2.found && conditionResult2.position[j])
                                foreach (var foundItem in Strategy.foundItemList[j].Values)
                                {
                                    var iD = foundItem.itemData as BinanceItemData;
                                    var positionData2 = iD.positionData[j];
                                    if (Strategy.calSimul)
                                        positionData2.Real = false;     // checkTrend
                                    Trading.TradingSimulEnterSetting(iD, iD.positionData[j], iD.listDic.Values[Strategy.minCV.index].lastStick, !Strategy.calSimul || positionData2.Real);

                                    if (!iD.RealEnter && (orders[j].Count + positions[j].Count) < Trading.instance.LimitUpDownControl[j].Value && 
                                        (!Strategy.calSimul || positionData2.Real))
                                    {
                                        try
                                        {
                                            iD.DetectTime = newStick.Time;
                                            if (!Trading_PlaceOrder(iD, (Position)j, false, true))
                                                Error.Show(this, "order fail");
                                            else
                                            {
                                                orders[j].Add(iD.Code, iD);

                                                UpdateAssets();

                                                lock (CountLocker)
                                                    EnterCount[j]++;

                                                Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0,
                                                    Trading.instance.NowTime().ToString(Formats.TIME) + "~" + iD.positionData[j].EnterTime.ToString(Formats.TIME),
                                                    DBHelper.conn1OrderHistoryColumn1, iD.Code + "~1 매수(자동)", DBHelper.conn1OrderHistoryColumn2, "종가:" + iD.positionData[j].EnterPrice);
                                                Alert.Start("Real Enter\n" +
                                                    Enum.GetName(typeof(Position), j) + "\n" +
                                                    iD.Code + "\n" +
                                                    newStick.Time.ToString(Formats.TIME) + "\n" +
                                                    iD.positionData[j].foundList[iD.positionData[j].foundList.Count - 1].chartValues.Text + "\n" +
                                                    "Enter Count: " + EnterCount[j], true, true);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Error.Show(this, e.Message);
                                            throw;
                                        }
                                    }

                                    if (Strategy.lastResultDataForCheckTrend[j] == null || Strategy.lastResultDataForCheckTrend[j].ExitTime != default)
                                    {
                                        var backResultData = new BackResultData() { EnterTime = newStick.Time, Code = foundItem.itemData.Code };
                                        backResultData.beforeResultData = Strategy.lastResultDataForCheckTrend[j];
                                        Strategy.lastResultDataForCheckTrend[j] = backResultData;
                                    }

                                    var resultData = Strategy.lastResultDataForCheckTrend[j];
                                    foundItem.itemData.resultDataForMetric[j] = resultData;
                                    resultData.ingItems.Add(foundItem.itemData.Code, foundItem.itemData.Code);
                                }
                        }
                    }

                Trading.instance.CheckAndUpdateClosePNL(itemData, data.Data.ClosePrice);
            }
            else
            {
                if (data0.Timestamp.Subtract(itemData.lastReceiveTime).TotalSeconds >= 1)
                    Trading.instance.CheckAndUpdateClosePNL(itemData, data.Data.ClosePrice);

                itemData.lastReceiveTime = data0.Timestamp;
            }
        }

        void RunStartSimul(DateTime enterStartTime)
        {
            var simulStartTime = enterStartTime.Subtract(Strategy.ReadyTimeToCheckBeforeStart);
            var loadStartTime = simulStartTime.Date;

            var currentTime = loadStartTime;
            var beforeFinal = false;
            var Final = false;
            while (true)
            {
                if (currentTime == loadStartTime)
                    foreach (var itemData in BaseFunctions.itemDataDic.Values)
                        lock (itemData.listDicLocker)
                            for (int i = ChartTimeSet.OneMinute.index; i <= Strategy.maxCV.index; i++)
                            {
                                var v = itemData.listDic.Values[i];
                                for (int j = 0; j < v.list.Count; j++)
                                    if (v.list[j].Time >= loadStartTime)
                                    {
                                        v.currentIndex = j;
                                        break;
                                    }
                                v.lastStickForS = i == ChartTimeSet.OneMinute.index ?
                                    (v.list.Count > 0 ? v.list[v.currentIndex] : v.lastStick) :
                                    new TradeStick(itemData.listDic.Keys[i]) { Time = (v.list.Count > 0 ? v.list[v.currentIndex] : v.lastStick).Time };
                            }
                else if (!beforeFinal && currentTime >= Trading.firstFinalMin.AddMinutes(-1))
                {
                    if (currentTime > Trading.firstFinalMin.AddMinutes(-1))
                        Error.Show();

                    var nowTime = Trading.instance.NowTime();
                    if (nowTime.Second > 20)
                    {
                        nowTime = Trading.instance.NowTime();
                        Thread.Sleep(63000 - nowTime.Second * 1000 - nowTime.Millisecond);
                    }

                    if (Trading.instance.KlineRcv < Trading.instance.KlineReq)
                        while (Trading.instance.KlineRcv != Trading.instance.KlineReq)
                        {
                            Thread.Sleep(100);
                            if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                Error.Show();
                        }
                    else if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                        Error.Show();

                    if (currentTime == Trading.firstFinalMin.AddMinutes(-1))
                        beforeFinal = true;
                }
                else if (beforeFinal)
                    Final = true;

                Strategy.foundItemList = new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>[]
                {
                    new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>(),
                    new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>()
                };

                foreach (var itemData in BaseFunctions.itemDataDic.Values)
                {
                    var vm = itemData.listDic[ChartTimeSet.OneMinute];

                    if (vm.lastStickForS.Time > currentTime)
                        continue;

                    for (int i = (int)Position.Long; i <= (int)Position.Short; i++)
                    {
                        itemData.positionData[i].foundList = new List<(DateTime foundTime, ChartValues chartValues)>();
                        itemData.positionData[i].found = false;
                    }

                    for (int i = ChartTimeSet.OneMinute.index; i <= Strategy.maxCV.index; i++)
                    {
                        var v = itemData.listDic.Values[i];
                        var vc = itemData.listDic.Keys[i];

                        if (i != ChartTimeSet.OneMinute.index)
                        {
                            var timeDiff = currentTime.Subtract(v.lastStickForS.Time).TotalSeconds;
                            if (timeDiff == vc.seconds)
                            {
                                if (!CompareSticks(v.lastStickForS, v.list[v.currentIndex]))
                                {
                                    Error.Show();
                                    CompareSticks(v.lastStickForS, v.list[v.currentIndex]);
                                }

                                Strategy.SetRSIAandDiff(v.list, v.list[v.currentIndex], v.currentIndex - 1);

                                v.currentIndex++;

                                lock (itemData.listDicLocker)
                                    v.lastStickForS = new TradeStick(vc) { Time = v.currentIndex < v.list.Count ? v.list[v.currentIndex].Time : v.lastStick.Time };

                                if (v.lastStickForS.Time != currentTime)
                                    Error.Show();
                            }
                            else if (timeDiff > vc.seconds || timeDiff < 0)
                                Error.Show();

                            if (v.lastStickForS.Price[1] == 0)
                            {
                                v.lastStickForS.Price[1] = vm.lastStickForS.Price[1];
                                v.lastStickForS.Price[2] = vm.lastStickForS.Price[2];
                            }

                            if (vm.lastStickForS.Price[0] > v.lastStickForS.Price[0])
                                v.lastStickForS.Price[0] = vm.lastStickForS.Price[0];
                            if (vm.lastStickForS.Price[1] < v.lastStickForS.Price[1])
                                v.lastStickForS.Price[1] = vm.lastStickForS.Price[1];
                            v.lastStickForS.Price[3] = vm.lastStickForS.Price[3];

                            v.lastStickForS.Ms += vm.lastStickForS.Ms;
                            v.lastStickForS.Md += vm.lastStickForS.Md;

                            v.lastStickForS.TCount += vm.lastStickForS.TCount;
                        }
                        else
                        {
                            if (currentTime > v.lastStickForS.Time)
                                v.currentIndex++;

                            v.lastStickForS = Final ? v.lastStick : v.list[v.currentIndex];

                            if (v.lastStickForS.Time != currentTime)
                                Error.Show();
                        }

                        if (!Strategy.calOnlyFullStick)
                            Strategy.SetRSIAandDiff(v.list, v.lastStickForS, v.currentIndex - 1);

                        Strategy.ChartFindConditionAndAdd(itemData, vc, vm.lastStickForS, v.lastStickForS, vm.currentIndex - 1, v.currentIndex - 1);
                    }

                    for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                    {
                        var positionData = itemData.positionData[j];
                        if ((Strategy.canLStogether ? !positionData.Enter : (!itemData.positionData[(int)Position.Long].Enter && !itemData.positionData[(int)Position.Short].Enter)) && positionData.found)
                            Strategy.foundItemList[j].Add(itemData.number, (itemData, positionData.foundList));
                        else if (positionData.Enter)
                        {
                            var v = itemData.listDic[positionData.EnterFoundForExit.chartValues];
                            if (Strategy.ExitConditionFinal(itemData, (Position)j, vm.lastStickForS, v.lastStickForS, v.currentIndex - 1))
                            {
                                positionData.Enter = false;

                                var profitRow = (double)((Position)j == Position.Long ? vm.lastStickForS.Price[3] / positionData.EnterPrice : positionData.EnterPrice / vm.lastStickForS.Price[3]);
                                var resultData = new BackResultData()
                                {
                                    Code = itemData.Code,
                                    EnterTime = positionData.EnterTime,
                                    ExitTime = vm.lastStickForS.Time,
                                    ProfitRate = Math.Round((profitRow - 1) * 100, 2),
                                    LorS = (Position)j
                                };

                                if (itemData.resultDataForMetric[j] != null)
                                {
                                    if (itemData.resultDataForMetric[j].Code == itemData.Code)
                                        itemData.resultDataForMetric[j].ExitTime = resultData.ExitTime;
                                    lock (itemData.resultDataForMetric[j].locker)
                                    {
                                        itemData.resultDataForMetric[j].Count++;
                                        itemData.resultDataForMetric[j].ProfitRate += profitRow;
                                        if ((profitRow - 1) * 100 > BaseFunctions.commisionRate + BaseFunctions.slippage)
                                        {
                                            itemData.resultDataForMetric[j].WinCount++;
                                            itemData.resultDataForMetric[j].WinProfitRateSum += profitRow;
                                        }
                                        itemData.resultDataForMetric[j].doneResults.Add(resultData);
                                        itemData.resultDataForMetric[j].ingItems.Remove(itemData.Code);
                                    }
                                    itemData.resultDataForMetric[j] = null;
                                }
                            }
                        }
                    }
                }

                if (currentTime >= simulStartTime)
                {
                    var conditionResult = Strategy.AllItemFindCondition();
                    if (conditionResult.found)
                        for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                            if (conditionResult.position[j])
                                foreach (var foundItem in Strategy.foundItemList[j].Values)
                                {
                                    var minV = foundItem.itemData.listDic[ChartTimeSet.OneMinute];
                                    var positionData = foundItem.itemData.positionData[j];
                                        Strategy.EnterSetting(positionData, minV.lastStickForS);

                                    if (Strategy.lastResultDataForCheckTrend[j] == null || Strategy.lastResultDataForCheckTrend[j].ExitTime != default)
                                    {
                                        var backResultData = new BackResultData() { EnterTime = currentTime, Code = foundItem.itemData.Code };
                                        backResultData.beforeResultData = Strategy.lastResultDataForCheckTrend[j];
                                        Strategy.lastResultDataForCheckTrend[j] = backResultData;
                                    }

                                    var resultData = Strategy.lastResultDataForCheckTrend[j];
                                    foundItem.itemData.resultDataForMetric[j] = resultData;
                                    resultData.ingItems.Add(foundItem.itemData.Code, foundItem.itemData.Code);
                                }
                }

                if (Final)
                    break;

                currentTime = currentTime.AddMinutes(1);
            }
        }
        bool CompareSticks(TradeStick kline, TradeStick kline2)
        {
            return kline.Time == kline2.Time &&
                kline.Price[0] == kline2.Price[0] && kline.Price[1] == kline2.Price[1] && kline.Price[2] == kline2.Price[2] && kline.Price[3] == kline2.Price[3] &&
                kline.Ms == kline2.Ms && kline.Md == kline2.Md;
        }

        void OnAggregatedTradeUpdates(DataEvent<BinanceStreamAggregatedTrade> data0)
        {
            var data = data0.Data;
            var itemData = BaseFunctions.itemDataDic[data.Symbol] as BinanceItemData;

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
            var itemData = BaseFunctions.itemDataDic[data.Symbol.ToUpper()] as BinanceItemData;

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
            var itemData = BaseFunctions.itemDataDic[data.Symbol] as BinanceItemData;

            if (itemData.RealEnter)
            {
                itemData.MarkPrice = data.MarkPrice;
                itemData.notianalValue = data.MarkPrice * Math.Abs(itemData.Size);
                itemData.InitialMargin = Math.Round(itemData.notianalValue / itemData.Leverage, 2);
                itemData.PNLa = (itemData.MarkPrice - itemData.RealEnterPrice) * itemData.Size;
                itemData.PNL = Math.Round(itemData.PNLa, 2);
                itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                for (int i = 0; i < itemData.brackets.Count; i++)
                    if (itemData.notianalValue > itemData.brackets[i].Floor && itemData.notianalValue <= itemData.brackets[i].Cap)
                    {
                        itemData.maintMargin = 0;

                        for (int j = 0; j < i; j++)
                            itemData.maintMargin += (itemData.brackets[j].Cap - itemData.brackets[j].Floor) * itemData.brackets[j].MaintenanceMarginRatio;

                        itemData.maintMargin += (itemData.notianalValue - itemData.brackets[i].Floor) * itemData.brackets[i].MaintenanceMarginRatio;

                        break;
                    }
            }

            UpdateAssets();
        }
        void UpdateAssets()
        {
            var maintMargin = 0m;
            var unPNL = 0m;
            var inMargins = 0m;
            for (int i = (int)Position.Long; i <= (int)Position.Short; i++)
            {
                foreach (var p in positions[i].Values)
                {
                    maintMargin += p.maintMargin;
                    unPNL += p.PNLa;
                    inMargins += p.InitialMargin;
                }
                foreach (var o in orders[i].Values)
                    inMargins += o.OrderPrice * o.OrderAmount / o.Leverage;
            }

            var marginBalance = Trading.instance.assetDic[Trading.AssetTextWalletBalance].Amount + unPNL;

            lock (Trading.instance.assetLocker)
            {
                Trading.instance.assetDic[AssetTextMarginBalance].Amount = Math.Round(marginBalance, 4);
                Trading.instance.assetDic[AssetTextMaintenanceMargin].Amount = Math.Round(maintMargin, 4);
                Trading.instance.assetDic[AssetTextMarginRatio].Amount = Math.Round(maintMargin / marginBalance * 100, 2);
                Trading.instance.assetDic[Trading.AssetTextAvailableBalance].Amount = marginBalance - inMargins;
                Trading.instance.assetDic[AssetTextUnrealizedPNL].Amount = Math.Round(unPNL, 2);
            }

            if (Trading.instance.assetDic[AssetTextMarginRatio].Amount > 50)
                Alert.Start("margin ratio 50%!!", true, true);

            if (!InvokeRequired)
                Trading.instance.assetsListView.Refresh();
            else
                BeginInvoke(new Action(() => { Trading.instance.assetsListView.Refresh(); }));
        }
        async void OnAccountUpdates(DataEvent<BinanceFuturesStreamAccountUpdate> data0)
        {
            var data = data0.Data;
            foreach (var balance in data.UpdateData.Balances)
                if (balance.Asset == "USDT")
                    lock (Trading.instance.assetLocker)
                    {
                        var asset = Trading.instance.assetDic[Trading.AssetTextWalletBalance];
                        asset.Amount = balance.WalletBalance;
                        Trading.instance.assetsListView.RefreshObject(asset);
                    }

            if (data.UpdateData.Reason == AccountUpdateReason.Order)
                foreach (var position in data.UpdateData.Positions)
                {
                    var itemData = BaseFunctions.itemDataDic[position.Symbol] as BinanceItemData;

                    if (position.Quantity == 0)
                    {
                        await itemData.markSub.CloseAsync();

                        Invoke(new Action(() => {
                            positions[(int)itemData.RealPosition].Remove(itemData.Code);

                            Trading.TradingRealExitSetting(itemData);

                            UpdateAssets();
                        }));
                    }
                    else if (itemData.RealEnter)
                        itemData.Size = position.Quantity;
                    else
                    {
                        var result = await socketClient.UsdFuturesApi.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates);
                        if (!result.Success)
                            Error.Show();

                        itemData.markSub = result.Data;

                        Invoke(new Action(() => {
                            itemData.RealEnterPrice = position.EntryPrice;
                            itemData.RealEnterTime = Trading.instance.NowTime();
                            itemData.Size = position.Quantity;
                            itemData.PNL = position.UnrealizedPnl;
                            itemData.RealPosition = itemData.Size > 0 ? Position.Long : Position.Short;
                            itemData.RealEnterN = (int)itemData.RealPosition + 1;
                            positions[(int)itemData.RealPosition].Add(itemData.Code, itemData);
                            orders[(int)itemData.RealPosition].Remove(itemData.Code);
                            itemData.RealEnter = true;
                            Trading.instance.CodeListView.RemoveObject(itemData);
                            Trading.instance.CodeListView.InsertObjects(0, new List<BinanceItemData> { itemData });
                            if (itemData.star)
                            {
                                Trading.instance.CodeListView2.RemoveObject(itemData);
                                Trading.instance.CodeListView2.InsertObjects(0, new List<BinanceItemData> { itemData });
                            }

                            UpdateAssets();

                            if (itemData == Trading.instance.showingItemData)
                            {
                                Trading_ResetOrderView();

                                Trading.instance.ChangeChartAreaBorderColor(itemData.RealPosition == Position.Long ? ColorSet.VolumeTaker : ColorSet.VolumeMaker, false);
                            }
                        }));
                    }
                }
        }
        void OnOrderUpdates(DataEvent<BinanceFuturesStreamOrderUpdate> data0)
        {
            var data = data0.Data;
            var itemData = BaseFunctions.itemDataDic[data.UpdateData.Symbol] as BinanceItemData;

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
            }
            else if (data.UpdateData.Status == OrderStatus.PartiallyFilled)
            {
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;
                var tradData = itemData.positionWhenOrder ? itemData.ExitTradeData : itemData.EnterTradeData;
                tradData.Detect_Stick_Time = itemData.DetectTime;
                tradData.Here_Get_Time = Trading.instance.NowTime();
                tradData.Server_Get_Time = data.UpdateData.UpdateTime;
                tradData.Get_Price = data.UpdateData.AveragePrice;
                tradData.Get_Qnt = data.UpdateData.AccumulatedQuantityOfFilledTrades;
            }
            else if (data.UpdateData.Status == OrderStatus.Filled || data.UpdateData.Status == OrderStatus.Canceled
                || data.UpdateData.Status == OrderStatus.Rejected || data.UpdateData.Status == OrderStatus.Expired)
            {
                if (data.UpdateData.Status == OrderStatus.Filled && itemData.positionWhenOrder)
                {
                    var resultData = itemData.resultData;

                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((data.UpdateData.AveragePrice / itemData.RealEnterPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(itemData.OrderPrice / data.UpdateData.AveragePrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((itemData.RealEnterPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }

                    
                    if (resultData.Exit_Send_Amount__Diff == default)
                    {
                        resultData.Profit_Rate = resultData.ProfitRate + "%(" + 
                            Math.Round((resultData.EnterCommision / (itemData.RealEnterPrice * data.UpdateData.AccumulatedQuantityOfFilledTrades) + data.UpdateData.Fee / (data.UpdateData.AveragePrice * data.UpdateData.AccumulatedQuantityOfFilledTrades)) * 100, 2) + "%)";
                        resultData.Profit_Value = data.UpdateData.RealizedProfit.ToString(Formats.FOR_DECIMAL_STRING) + "$(" + (data.UpdateData.RealizedProfit - resultData.EnterCommision - data.UpdateData.Fee).ToString(Formats.FOR_DECIMAL_STRING) + ")";
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(Formats.TIME) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";
                    }

                    Trading.instance.UpdateResult(itemData, itemData.isAuto);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && !itemData.positionWhenOrder) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = itemData.resultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(Math.Abs(itemData.OrderFilled - itemData.Size) / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(Formats.TIME) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";

                        itemData.ExitCanceledTradeData = itemData.ExitTradeData;

                        Trading_PlaceOrder(itemData, itemData.RealPosition == Position.Long ? Position.Short : Position.Long, true, itemData.isAuto);
                    }
                    else
                    {
                        if (data.UpdateData.AveragePrice != 0)
                        {
                            itemData.RealEnterPrice = data.UpdateData.AveragePrice;
                            resultData.EnterCommision = data.UpdateData.Fee;
                            resultData.Enter_Send_Price__Diff = itemData.OrderPrice.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        }
                        resultData.Enter_Send_Time__Diff = resultData.EnterSendTime.ToString(Formats.TIME) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.EnterSendTime).Milliseconds / 1000D, 1) + "s)";
                        resultData.Enter_Send_Amount__Diff = itemData.OrderAmount.ToString(Formats.FOR_DECIMAL_STRING) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                    }

                    (itemData.isAuto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).RefreshObject(resultData);
                }

                for (int i = (int)Position.Long; i <= (int)Position.Short; i++)
                    orders[i].Remove(itemData.Code);
                UpdateAssets();

                var tradData = itemData.positionWhenOrder ? itemData.ExitTradeData : itemData.EnterTradeData;
                tradData.Detect_Stick_Time = itemData.DetectTime;
                tradData.Here_Get_Time = Trading.instance.NowTime();
                tradData.Server_Get_Time = data.UpdateData.UpdateTime;
                tradData.Get_Price = data.UpdateData.AveragePrice;
                tradData.Get_Qnt = data.UpdateData.AccumulatedQuantityOfFilledTrades;
            }
        }

        void Trading_ShowChartAdditional(TradeItemData itemData0)
        {
            var itemData = itemData0 as BinanceItemData;
            if (itemData.Leverage == 0)
            {
                var result = client.UsdFuturesApi.Account.GetPositionInformationAsync(itemData.Code).Result;
                if (!result.Success)
                    Error.Show();

                BinanceWeightManager.UpdateWeightNow(result.ResponseHeaders);

                foreach (var data in result.Data)
                {
                    if (data.Symbol != itemData.Code)
                        break;

                    itemData.Leverage = data.Leverage;
                    itemData.maxNotionalValue = data.MaxNotional;

                    var result1 = client.UsdFuturesApi.Account.GetBracketsAsync(itemData.Code).Result;
                    if (!result1.Success)
                        Error.Show();

                    BinanceWeightManager.UpdateWeightNow(result1.ResponseHeaders);

                    foreach (var brackets in result1.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                    Invoke(new Action(() =>
                    {
                        if (itemData == Trading.instance.showingItemData)
                            leverageTextBox1.Text = "/ " + itemData.maxLeverage;
                    }));
                }
            }
        }
        void Trading_ResetOrderView()
        {
            var itemDataShowing = Trading.instance.showingItemData as BinanceItemData;

            leverageTextBox0.Text = itemDataShowing.Leverage.ToString();
            if (itemDataShowing.RealEnter)
            {
                OrderBoxSetting(true, false);
                Trading.instance.positionCloseButton.Enabled = true;
                Trading.instance.positionCloseButton.BackColor = ColorSet.Button;
            }
            else
            {
                OrderBoxSetting(Trading.instance.autoSizeDefault, Trading.instance.minSizeDefault);
                Trading.instance.positionCloseButton.Enabled = false;
                Trading.instance.positionCloseButton.BackColor = ColorSet.ButtonSelected;
            }

            orderPriceTextBox1.Text = Trading.instance.limitPrice.ToString();
        }
        void Trading_LoadMoreAdditional(Chart chart, bool loadNew)
        {
            var itemData = Trading.instance.showingItemData as BinanceItemData;

            var vc = BaseFunctions.mainChart.Tag as ChartValues;

            DateTime endTime = Trading.instance.NowTime();
            if (loadNew)
                itemData.showingStickList.Clear();
            else
                endTime = DateTime.Parse(chart.Series[0].Points[0].AxisLabel).AddSeconds(-1);

            List<IBinanceKline> blist = new List<IBinanceKline>();
            var size = Trading.instance.chartViewSticksSize;

            var loadSize = size + Strategy.IndNeedDays - 1;
            var loadEndTime = endTime;

            while (loadSize > 0)
            {
                var result = client.UsdFuturesApi.ExchangeData.GetKlinesAsync(itemData.Code, (KlineInterval)vc.original, null, loadEndTime, loadSize > 1500 ? 1500 : loadSize).Result;
                if (!result.Success)
                    Error.Show();

                BinanceWeightManager.UpdateWeightNow(result.ResponseHeaders);

                var blist2 = result.Data.ToList();
                if (blist2.Count == 0)
                    break;

                blist.InsertRange(0, blist2);

                loadSize -= blist2.Count;
                loadEndTime = blist2[0].OpenTime.AddSeconds(-1);
            }

            var list = GetList(blist, vc);

            if (list.Count == 0)
            {
                return;
            }

            var nowStick = itemData.showingStick;
            if (!loadNew)
                list.AddRange(itemData.showingStickList);
            else
            {
                nowStick = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
            }

            DateTime startTime = default;
            if (vc.index <= ChartTimeSet.OneDay.index)
            {
                startTime = endTime
                    .AddSeconds(-(int)endTime.TimeOfDay.TotalSeconds % vc.seconds)
                    .AddSeconds(-(size - 1) * vc.seconds);
            } 
            else if (vc.index < ChartTimeSet.OneMonth.index)
            {
                startTime = endTime
                    .AddSeconds(-(size - 1) * vc.seconds);
            }
            else
            {
                startTime = endTime
                    .AddMonths(-(size - 1));
            }

            var startIndex = BaseFunctions.GetStartIndex(list, startTime);

            for (int i = 0; i < list.Count; i++)
                Strategy.SetRSIAandDiff(list, list[i], i - 1, int.MinValue, vc);
            Strategy.SetRSIAandDiff(list, nowStick, list.Count - 1, int.MinValue, vc);

            if (startIndex > 0)
                list.RemoveRange(0, startIndex);

            BaseFunctions.baseInstance.ClearChart(chart);

            for (int i = 0; i < list.Count; i++)
                Trading.instance.AddFullChartPoint(chart, list[i]);
            Trading.instance.AddFullChartPoint(chart, nowStick);

            itemData.showingVC = vc;
            itemData.showingStickList = list;
            itemData.showingStick = nowStick;
            itemData.currentPriceWhenLoaded = itemData.showingStick.Price[3];
        }
        void Trading_AggONandOFF(TradeItemData itemData, bool on)
        {
            if (on)
            {
                Task.Run(new Action(() =>
                {
                    var result = socketClient.UsdFuturesApi.SubscribeToAggregatedTradeUpdatesAsync(itemData.Code, OnAggregatedTradeUpdates).Result;
                    if (!result.Success)
                        Error.Show();

                    (itemData as BinanceItemData).aggSub = result.Data;
                }));
            }
            else
            {
                Task.Run(new Action(() =>
                {
                    socketClient.UnsubscribeAsync((itemData as BinanceItemData).aggSub).Wait();
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
                    var result = socketClient.UsdFuturesApi.SubscribeToPartialOrderBookUpdatesAsync(itemData.Code, (int)Trading.instance.hoChart.Tag, 500, OnHoUpdates).Result;
                    if (!result.Success)
                        Error.Show();

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
        bool Trading_PlaceOrder(TradeItemData itemData0, Position position, bool market, bool auto, bool autoCancel = true)
        {
            var itemData = itemData0 as BinanceItemData;

            if (itemData.RealEnter && itemData.RealPosition == position)
            {
                MessageBox.Show("position again?");
                return false;
            }

            itemData.autoCancel = autoCancel;

            var tradeData = new TradeData();
            var minList = itemData.listDic[ChartTimeSet.OneMinute];

            tradeData.Send_Close_Price = itemData.AggOn && !itemData.AggFirst ? itemData.secStick.Price[3] : minList.lastStick.Price[3];
            tradeData.Last_Min_Qnt = minList.lastStick.Ms + minList.lastStick.Md;

            decimal priceRate = 0.1m;
            if (!auto && !decimal.TryParse(orderPriceTextBox1.Text, out priceRate))
            {
                Error.Show(this, "priceRate input error");
                return false;
            }

            decimal? price = position == Position.Long ?
                    (int)(tradeData.Send_Close_Price * (1 + priceRate / 100) / itemData.hoDiff + 1) * itemData.hoDiff :
                    (int)(tradeData.Send_Close_Price * (1 - priceRate / 100) / itemData.hoDiff) * itemData.hoDiff;

            decimal quantity = itemData.RealEnter ? Math.Abs(itemData.Size) : default;
            decimal limitBalancePercent = default;
            decimal limitAvgSecPercent = default;
            if (!auto && ((orderSizeTextBox1.Enabled && (!decimal.TryParse(orderSizeTextBox1.Text, out quantity) || quantity <= 0))
                    || (!itemData.RealEnter && autoSizeCheckBox.AutoCheck && autoSizeCheckBox.Checked && 
                        (!decimal.TryParse(maximumBalanceForSizeTextBox0.Text, out limitBalancePercent) || limitBalancePercent <= 0) &&
                        (!decimal.TryParse(maximumPercentOfAvgSecForSizeTextBox0.Text, out limitAvgSecPercent) || limitAvgSecPercent <= 0))))
            {
                Error.Show(this, "order input error");
                return false;
            }
            else if (!itemData.RealEnter && (auto || miniSizeCheckBox.Checked || autoSizeCheckBox.Checked))
            {
                var minQuan = GetMinQuan(itemData, (decimal)price);
                if (!auto && miniSizeCheckBox.Checked)
                    quantity = minQuan;
                else
                {
                    if (auto)
                    {
                        limitAvgSecPercent = autoLimitAvgSecPercent;
                        limitBalancePercent = Trading.instance.BudgetRateControl.Value;
                    }

                    quantity = (int)(limitAvgSecPercent / 100 * tradeData.Last_Min_Qnt / 60 / itemData.minSize + 1) * itemData.minSize;
                    if (quantity < minQuan)
                        quantity = minQuan;

                    var budget = Trading.instance.assetDic[Trading.AssetTextWalletBalance].Amount * limitBalancePercent / 100;
                    if (budget > Trading.instance.assetDic[Trading.AssetTextAvailableBalance].Amount)
                        budget = Trading.instance.assetDic[Trading.AssetTextAvailableBalance].Amount;

                    var limitAmount = budget < itemData.minNotionalValue ? minQuan : 
                        ((int)(budget / price / itemData.minSize + 1) * itemData.minSize);

                    if (limitAmount < quantity)
                        quantity = limitAmount;
                }
            }

            var orderType = FuturesOrderType.Limit;
            TimeInForce? timeInForce = null;
            if (!auto)
            {
                if (!marketRadioButton.Checked && !market)
                {
                    if (PORadioButton.Checked)
                        timeInForce = TimeInForce.GoodTillCrossing;
                    else if (IOCRadioButton.Checked)
                        timeInForce = TimeInForce.ImmediateOrCancel;
                    else
                        timeInForce = TimeInForce.GoodTillCanceled;
                }
                else
                    orderType = FuturesOrderType.Market;
            }
            else
                timeInForce = TimeInForce.GoodTillCrossing;


            if (!itemData.RealEnter && price * quantity < itemData.minNotionalValue)
            {
                Error.Show(this, "prcie quantity error");
                return false;
            }

            if (orderType == FuturesOrderType.Market)
                price = null;

            if (!auto)
                orderSizeTextBox1.Text = quantity.ToString(Formats.FOR_DECIMAL_STRING);

            itemData.positionWhenOrder = itemData.RealEnter;
            if (!itemData.positionWhenOrder)
                itemData.isAuto = auto;
            else
            {
                itemData.ExitCanceledTradeData = null;

                if (itemData.isAuto)
                {
                    if (itemData.EnterTradeData.Detect_Stick_Time == itemData.DetectTime)
                    {
                        itemData.isAutoHandOut = true;
                        itemData.DetectTime = Trading.instance.NowTime();
                    }
                    else
                        itemData.isAutoHandOut = false;
                }
            }

            var result = client.UsdFuturesApi.Trading.PlaceOrderAsync(
                itemData.Code
                , position == Position.Long ? OrderSide.Buy : OrderSide.Sell
                , orderType
                , quantity
                , price
                , PositionSide.Both
                , timeInForce
                , itemData.positionWhenOrder && (auto || ROCheckBox.Checked)).Result;
            if (!result.Success)
            {
                Error.Show(this, "order fail");
                return false;
            }

            tradeData.Here_Send_Time = Trading.instance.NowTime();
            tradeData.Send_Price = price == null ? 0 : (decimal)price;
            tradeData.Send_Qnt = quantity;
            tradeData.Position = position;

            var result2 = client.UsdFuturesApi.Trading.CancelAllOrdersAfterTimeoutAsync(itemData.Code, TimeSpan.FromSeconds(1)).Result;
            if (!result2.Success)
                Error.Show(this, "timeout cancel order fail");

            if (itemData.positionWhenOrder)
            {
                itemData.ExitTradeData = tradeData;

                if (itemData.resultData == default)
                {
                    itemData.resultData = new TradeResultData { Code = itemData.Code };
                    itemData.resultData.Position = position == Position.Long ? "Short" : "Long";

                    if (InvokeRequired)
                        BeginInvoke(new Action(() => {
                            (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                        }));
                    else
                        (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                }

                itemData.resultData.ExitSendTime = Trading.instance.NowTime();
            }
            else
            {
                itemData.EnterTradeData = tradeData;

                itemData.RealPosition = position == Position.Long ? Position.Long : Position.Short;

                itemData.resultData = new TradeResultData { Code = itemData.Code };
                itemData.resultData.Position = position == Position.Long ? "Long" : "Short";
                itemData.resultData.EnterSendTime = Trading.instance.NowTime();
                if (InvokeRequired)
                    BeginInvoke(new Action(() => {
                        (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                    }));
                else
                    (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });

                itemData.Size = 0;
            }

            Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, Trading.instance.NowTime().ToString(Formats.TIME) + "~" + tradeData.Here_Send_Time.ToString(Formats.TIME),
                DBHelper.conn1OrderHistoryColumn1, itemData.Code + "~2 " + (position == Position.Long ? "매수" : "매도") + "주문전송", DBHelper.conn1OrderHistoryColumn2, "주문가격:" + price + " 주문수량:" + quantity);

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