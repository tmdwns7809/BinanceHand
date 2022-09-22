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
using TradingLibrary.Base.Settings;
using TradingLibrary.Base.Enum;
using TradingLibrary.Base.SticksDB;
using System.Data.SQLite;

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

        BinanceClient client;
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

            Settings.Load(Settings.ProgramBinanceFutures);

            Trading.instance = new Trading(this, false, 4.2m, 20);
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

            BinanceSticksDB.StartThread();
            BinanceSticksDB.SetDB();
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
            miniSizeCheckBox.MouseUp += (sender, e) => { if (Trading.instance.itemDataShowing != null) OrderBoxSetting(autoSizeCheckBox.Checked, miniSizeCheckBox.Checked); };

            Trading.instance.SetRadioButton_CheckBox(ROCheckBox, "RO",
                miniSizeCheckBox.Size, new Point(miniSizeCheckBox.Location.X + miniSizeCheckBox.Width + 5, miniSizeCheckBox.Location.Y));

            Trading.instance.SetRadioButton_CheckBox(autoSizeCheckBox, "Auto Size",
                new Size(50, GTCRadioButton.Height),
                new Point(orderSizeTextBox0.Location.X + 10, orderSizeTextBox0.Location.Y + orderSizeTextBox0.Size.Height + 5 + (maximumBalanceForSizeTextBox0.Size.Height - autoSizeCheckBox.Size.Height) / 2));
            autoSizeCheckBox.MouseUp += (sender, e) => { if (Trading.instance.itemDataShowing != null) OrderBoxSetting(autoSizeCheckBox.Checked, miniSizeCheckBox.Checked); };

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

            var n = 0;
            var newSymbolList = new List<string>();
            var noSymbolList = new List<string>();
            foreach (var s in result.Data.Symbols)
            {
                var code = s.Name.Trim().ToUpper();
                if (s.Status != SymbolStatus.Trading)
                {
                    noSymbolList.Add(code);
                    continue;
                }

                n++;

                if (Trading.instance.itemDataDic.ContainsKey(code))
                {
                    Trading.instance.itemDataDic[code].number = n;
                    continue;
                }

                var itemData = new BinanceItemData(s, n);
                Trading.instance.itemDataDic.Add(itemData.Code, itemData);
                symbolList.Add(code);
                newSymbolList.Add(code);
                Trading.instance.CodeListView.AddObject(itemData);
                BinanceSticksDB.requestTRTaskQueue.Enqueue(new Task(() => {
                    var result1 = client.FuturesUsdt.GetBracketsAsync(itemData.Code).Result;
                    if (!result1.Success)
                        BaseFunctions.ShowError(this);

                    foreach (var brackets in result1.Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    var data = client.FuturesUsdt.ChangeInitialLeverageAsync(itemData.Code, 1).Result;
                    if (!data.Success)
                        BaseFunctions.ShowError(this, "leverage change fail");
                    itemData.Leverage = 1;
                }));
            }
            BinanceSticksDB.codesCount = Trading.instance.itemDataDic.Count;

            if (n != Trading.instance.itemDataDic.Count)
            {
                if (newSymbolList.Count != 0)
                    BaseFunctions.ShowError(this);

                foreach (var code in noSymbolList)
                    if (Trading.instance.itemDataDic.ContainsKey(code))
                    {
                        var conn = BinanceSticksDB.DBDic[BaseChartTimeSet.OneMinute];
                        conn.Close();
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        conn.Open();

                        new SQLiteCommand("Begin", conn).ExecuteNonQuery();

                        new SQLiteCommand("CREATE TABLE IF NOT EXISTS '" + code + "' " +
                            "('" + BaseSticksDB.DBTimeColumnName + "' INTEGER, '" + BaseSticksDB.DBOpenColumnName + "' TEXT, '" + BaseSticksDB.DBHighColumnName + "' TEXT, '" +
                            BaseSticksDB.DBLowColumnName + "' TEXT, '" + BaseSticksDB.DBCloseColumnName + "' TEXT, '" + BaseSticksDB.DBBaseVolumeColumnName + "' TEXT, '" + BaseSticksDB.DBTakerBuyBaseVolumeColumnName + "' TEXT, '" +
                            BaseSticksDB.DBQuoteVolumeColumnName + "' TEXT, '" + BaseSticksDB.DBTakerBuyQuoteVolumeColumnName + "' TEXT, '" + BaseSticksDB.DBTradeCountColumnName + "' INTEGER)", conn).ExecuteNonQuery();

                        new SQLiteCommand("CREATE UNIQUE INDEX IF NOT EXISTS '" + code + "_index' ON '" + code + "' ('" + BaseSticksDB.DBTimeColumnName + "')", conn).ExecuteNonQuery();

                        var reader = new SQLiteCommand("SELECT *, rowid FROM '" + code + "' ORDER BY rowid DESC LIMIT 1", conn).ExecuteReader();
                        DateTime startTime = default;
                        if (reader.Read())
                        {
                            startTime = DateTime.ParseExact(reader[BaseSticksDB.DBTimeColumnName].ToString(), BaseFunctions.DBTimeFormat, null);
                            new SQLiteCommand("DELETE FROM '" + code + "' WHERE rowid='" + reader["rowid"].ToString() + "'", conn).ExecuteNonQuery();
                        }

                        var itemData = Trading.instance.itemDataDic[code];
                        var v = itemData.listDic[BaseChartTimeSet.OneMinute];
                        var first = true;
                        
                        foreach (var tradeStick in v.list)
                        {
                            if (tradeStick.Time < startTime)
                                continue;
                            else if (first)
                            {
                                if (tradeStick.Time != startTime)
                                    BaseFunctions.ShowError(this);

                                first = false;
                            }

                            var stick = tradeStick.original as IBinanceStreamKline;
                            new SQLiteCommand("INSERT INTO '" + code + "' " +
                            "('" + BaseSticksDB.DBTimeColumnName + "', '" + BaseSticksDB.DBOpenColumnName + "', '" + BaseSticksDB.DBHighColumnName + "', '" +
                                BaseSticksDB.DBLowColumnName + "', '" + BaseSticksDB.DBCloseColumnName + "', '" + BaseSticksDB.DBBaseVolumeColumnName + "', '" + BaseSticksDB.DBTakerBuyBaseVolumeColumnName + "', '" +
                                BaseSticksDB.DBQuoteVolumeColumnName + "', '" + BaseSticksDB.DBTakerBuyQuoteVolumeColumnName + "', '" + BaseSticksDB.DBTradeCountColumnName + "') values " +
                            "('" + stick.OpenTime.ToString(BaseFunctions.DBTimeFormat) + "', '" + stick.Open + "', '" + stick.High + "', '" + stick.Low + "', '" + stick.Close
                             + "', '" + stick.BaseVolume + "', '" + stick.TakerBuyBaseVolume + "', '" + stick.QuoteVolume + "', '" + stick.TakerBuyQuoteVolume + "', '" + stick.TradeCount + "')", conn).ExecuteNonQuery();
                        }

                        var stick2 = v.lastStick.original as IBinanceStreamKline;
                        new SQLiteCommand("INSERT INTO '" + code + "' " +
                        "('" + BaseSticksDB.DBTimeColumnName + "', '" + BaseSticksDB.DBOpenColumnName + "', '" + BaseSticksDB.DBHighColumnName + "', '" +
                            BaseSticksDB.DBLowColumnName + "', '" + BaseSticksDB.DBCloseColumnName + "', '" + BaseSticksDB.DBBaseVolumeColumnName + "', '" + BaseSticksDB.DBTakerBuyBaseVolumeColumnName + "', '" +
                            BaseSticksDB.DBQuoteVolumeColumnName + "', '" + BaseSticksDB.DBTakerBuyQuoteVolumeColumnName + "', '" + BaseSticksDB.DBTradeCountColumnName + "') values " +
                        "('" + stick2.OpenTime.ToString(BaseFunctions.DBTimeFormat) + "', '" + stick2.Open + "', '" + stick2.High + "', '" + stick2.Low + "', '" + stick2.Close
                         + "', '" + stick2.BaseVolume + "', '" + stick2.TakerBuyBaseVolume + "', '" + stick2.QuoteVolume + "', '" + stick2.TakerBuyQuoteVolume + "', '" + stick2.TradeCount + "')", conn).ExecuteNonQuery();

                        new SQLiteCommand("Commit", conn).ExecuteNonQuery();

                        conn.Close();

                        Trading.instance.itemDataDic.Remove(code);
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
                var result2 = socketClient.FuturesUsdt.SubscribeToKlineUpdatesAsync(newSymbolList, KlineInterval.OneMinute, OnKlineUpdates).Result;
                if (!result2.Success)
                    BaseFunctions.ShowError(this);

                Trading.instance.UpdateReqRcv(Trading.instance.KlineReqTextBox, symbolList.Count);
            }

            if (newSymbolList.Count != 0 && BinanceSticksDB.requestTRTaskQueue.Count == 0)
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
        List<TradeStick> SetRSIAandGetList(IEnumerable<IBinanceKline> data, DateTime startTime, ChartValues vc = null)
        {
            var list = new List<TradeStick>();
            foreach (var stickReal in data)
            {
                var stick = GetStick(stickReal);
                BaseFunctions.SetRSIAandDiff(list, stick, int.MinValue, int.MinValue, vc);
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
                    foreach (TradeItemData item in Trading.instance.itemDataDic.Values)
                        if (item.Last30minStickFluc > 3)
                            count++;

                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1DetectHistoryName, DBHelper.conn1DetectHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat),
                        DBHelper.conn1DetectHistoryColumn1, "직전 1시간 이내 3% 이상 변동 종목 개수", DBHelper.conn1DetectHistoryColumn2, count.ToString());
                }
            }, BaseFunctions.tokenSource.Token);

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

            var result = client.FuturesUsdt.Account.GetAccountInfoAsync().Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            foreach (var s in result.Data.Assets)
                if (s.Asset == "USDT")
                    lock (Trading.instance.assetLocker)
                    {
                        Trading.instance.assetDic[AssetTextMaintenanceMargin].Amount = s.MaintMargin;
                        Trading.instance.assetDic[AssetTextMarginBalance].Amount = s.MarginBalance;
                        Trading.instance.assetDic[Trading.AssetTextAvailableBalance].Amount = s.AvailableBalance;
                        Trading.instance.assetDic[Trading.AssetTextWalletBalance].Amount = s.WalletBalance;
                        Trading.instance.assetDic[AssetTextMarginRatio].Amount = Math.Round(s.MaintMargin / s.MarginBalance * 100, 2);
                    }
            foreach (var s in result.Data.Positions)
                if (s.EntryPrice != 0m)
                {
                    var itemData = (BinanceItemData)Trading.instance.itemDataDic[s.Symbol];

                    var result1 = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates).Result;
                    if (!result1.Success)
                        BaseFunctions.ShowError(this);

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
                (itemData.listDic[BaseChartTimeSet.OneMinute].lastStick == default ? default : itemData.listDic[BaseChartTimeSet.OneMinute].lastStick.Price[3]);
        }
        void OrderBoxSetting(bool autoSize, bool minSize)
        {
            var itemDataShowing = Trading.instance.itemDataShowing as BinanceItemData;

            if (itemDataShowing.RealEnter && autoSize && minSize)
                BaseFunctions.ShowError(this, "orderBoxError");
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
                BaseFunctions.ShowError(this, "orderBoxError");
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
                BaseFunctions.ShowError(this, "orderBoxError");
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
                orderSizeTextBox1.Text = GetMinQuan(itemDataShowing, GetCurrentPrice(itemDataShowing)).ToString(BaseFunctions.ForDecimalString);

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
                orderSizeTextBox1.Text = GetMinQuan(itemDataShowing, GetCurrentPrice(itemDataShowing)).ToString(BaseFunctions.ForDecimalString);

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
            if (!Trading.instance.itemDataDic.ContainsKey(data.Symbol))
                SetItemDataList();

            var itemData = Trading.instance.itemDataDic[data.Symbol] as BinanceItemData;

            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;
                lock (Trading.minLocker)
                    if (klineFirstFinal && Trading.instance.KlineRcv < Trading.instance.KlineReq)
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
                            BaseFunctions.foundItemList = new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>[]
                            {
                                new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>(),
                                new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>()
                            };
                    }

                    Trading.instance.KlineRcv++;
                }

                if (newStick.Time != Trading.firstFinalMin)
                    BaseFunctions.ShowError(this, "final wrong");

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
                var dropLongFirstVC = BaseFunctions.maxCV.index;
                var dropShortFirstVC = BaseFunctions.maxCV.index;

                for (int i = BaseChartTimeSet.OneMinute.index; i <= BaseFunctions.maxCV.index; i++)
                {
                    var v = itemData.listDic.Values[i];
                    var vc = itemData.listDic.Keys[i];

                    if (v.first)
                    {
                        if (BaseFunctions.maxCV.index > BaseChartTimeSet.OneDay.index)
                            BaseFunctions.ShowError(this);
                        else if (vc == BaseChartTimeSet.OneMinute)
                        {
                            v.lastStick = new TradeStick() { Time = newStick.Time, original = data.Data };

                            BinanceSticksDB.requestTRTaskQueue.Enqueue(new Task(() =>
                            {
                                BinanceSticksDB.UpdateDB(itemData.Code, newStick.Time, client, this);

                                var vm2 = itemData.listDic[BaseChartTimeSet.OneMinute];
                                for (int j = BaseFunctions.minCV.index; j <= BaseFunctions.maxCV.index; j++)
                                {
                                    var list = new List<TradeStick>();

                                    var vc2 = itemData.listDic.Keys[j];

                                    var conn = BinanceSticksDB.DBDic[itemData.listDic.Keys[j]];
                                    conn.Open();
                                    var reader2 = new SQLiteCommand("SELECT * FROM '" + itemData.Code + "' WHERE " +
                                        "(" + BaseSticksDB.DBTimeColumnName + "<='" + newStick.Time.ToString(BaseFunctions.DBTimeFormat) + "') AND " +
                                        "(" + BaseSticksDB.DBTimeColumnName + ">='" +
                                            newStick.Time.Subtract(BaseFunctions.ReadyTimeToCheckBeforeStart).Date.AddSeconds(-vc2.seconds * (BaseFunctions.TotalNeedDays + BaseFunctions.BaseLoadNeedDays - 1)).ToString(BaseFunctions.DBTimeFormat) + "')", conn).ExecuteReader();
                                    while (reader2.Read())
                                        list.Add(BinanceSticksDB.GetTradeStickFromSQL(reader2));
                                    conn.Close();

                                    var v2 = itemData.listDic.Values[j];

                                    TradeStick firstStick = default;
                                    TradeStick lastStick = default;
                                    lock (itemData.listDicLocker)
                                        firstStick = v2.list.Count == 0 ? v2.lastStick : v2.list[0];

                                    if (list[list.Count - 1].Time != firstStick.Time)
                                        BaseFunctions.ShowError(this);
                                    else
                                    {
                                        lastStick = list.Last();
                                        list.RemoveAt(list.Count - 1);
                                    }

                                    if (j > BaseChartTimeSet.OneMinute.index)
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
                                                            BaseFunctions.ShowError(this);

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
                                            BaseFunctions.ShowError(this);
                                    }

                                    lock (itemData.listDicLocker)
                                        v2.list.InsertRange(0, list);
                                }

                                if (BinanceSticksDB.doneCount == BinanceSticksDB.codesCount)
                                    Task.Run(() =>
                                    {
                                        var startTime = Trading.firstFinalMin;
                                        if (BaseFunctions.ReadyTimeToCheckBeforeStart > TimeSpan.Zero)
                                        {
                                            BaseFunctions.AddOrChangeLoadingText("Start: " + startTime.ToString(BaseFunctions.TimeFormat) + "(simulating " + BaseFunctions.ST + "...)", false);
                                            RunStartSimul(startTime);
                                        }
                                        else
                                        {
                                            if (BaseFunctions.NowTime().Second > 55)
                                            {
                                                var nowTime = BaseFunctions.NowTime();
                                                Thread.Sleep(63000 - nowTime.Second * 1000 - nowTime.Millisecond);
                                            }

                                            if (Trading.instance.KlineRcv < Trading.instance.KlineReq)
                                                while (Trading.instance.KlineRcv != Trading.instance.KlineReq)
                                                {
                                                    Thread.Sleep(100);
                                                    if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                                        BaseFunctions.ShowError(this);
                                                }
                                            else if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                                BaseFunctions.ShowError(this);

                                            startTime = Trading.firstFinalMin;
                                        }
                                        BaseFunctions.AddOrChangeLoadingText("Start: " + startTime.ToString(BaseFunctions.TimeFormat) + "(" + BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + ")", false);
                                        Trading.loadingDone = true;
                                        Trading.loadingDoneTime = startTime;
                                    });
                            }));
                        }
                        else
                            v.lastStick = new TradeStick() { Time = newStick.Time.AddSeconds(-(int)newStick.Time.TimeOfDay.TotalSeconds % vc.seconds) };

                        v.first = false;
                    }

                    var timeDiff = newStick.Time.Subtract(v.lastStick.Time).TotalSeconds;
                    if (timeDiff > vc.seconds || timeDiff < 0)
                        BaseFunctions.ShowError(this);
                    else if (timeDiff == vc.seconds)
                    {
                        lock (itemData.listDicLocker)
                            v.list.Add(v.lastStick);
                        v.lastStick = new TradeStick() { Time = newStick.Time };

                        if (vc == BaseChartTimeSet.OneMinute)
                            v.lastStick.original = data.Data;

                        if ((int)v.lastStick.Time.Subtract(v.list[v.list.Count - 1].Time).TotalSeconds != vc.seconds)
                            BaseFunctions.ShowError(this);
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
                        BaseFunctions.SetRSIAandDiff(v.list, v.lastStick, int.MinValue, int.MinValue, vc);
                        BaseFunctions.OneChartFindConditionAndAdd(itemData, vc, vm.lastStick, v.lastStick);

                        if (v.lastStick.indicator != null && !double.IsNaN(v.lastStick.indicator.RSIA) && Math.Abs(v.lastStick.indicator.RSIA) > Math.Abs(itemData.RSIAHighest))
                        {
                            itemData.RSIAHighest = (int)v.lastStick.indicator.RSIA;
                            itemData.RSIAHighestChart = vc.Text;
                        }

                        if (v.lastStick.lastFoundStick != null)
                        {
                            var c = (int)v.lastStick.Time.Subtract(v.lastStick.lastFoundStick.Time).TotalSeconds / vc.seconds;
                            if (c <= BaseFunctions.BaseLoadNeedDays)
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
                                    BaseFunctions.AlertStart(Math.Round(v.lastStick.lastFoundStick.indicator.RSIA, 2) + "\n" +
                                        itemData.Code + "\n" + newStick.Time.ToString(BaseFunctions.TimeFormat) + "\n" + vc.Text, true, true);
                            }
                        }

                        if (vc == BaseChartTimeSet.OneHour && v.list.Count > 0)
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
                            //BaseFunctions.AlertStart(Enum.GetName(typeof(Position), j) + "\n" + itemData.Code + "\n" + newStick.Time.ToString(BaseFunctions.TimeFormat) + "\n" + positionData.foundList[positionData.foundList.Count - 1].chartValues.Text);

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
                                BaseFunctions.foundItemList[j].Add(itemData.number, (itemData, positionData.foundList));
                        else
                        if (positionData.Enter)
                        {
                            var v = itemData.listDic[positionData.EnterFoundForExit.chartValues];
                            if (BaseFunctions.ExitConditionFinal(itemData, (Position)j, vm.lastStick, v.lastStick, v.list.Count - 1))
                            {
                                Trading.TradingSimulExitSetting(itemData, positionData, !BaseFunctions.calSimul || positionData.Real);

                                if (itemData.isAuto && itemData.RealEnter)
                                {
                                    itemData.DetectTime = newStick.Time;
                                    if (!Trading_PlaceOrder(itemData, (Position)j == Position.Long ? Position.Short : Position.Long, false, true))
                                        BaseFunctions.ShowError(this, "order fail");

                                    Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + positionData.EnterTime.ToString(BaseFunctions.TimeFormat),
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
                            var conditionResult2 = BaseFunctions.AllItemFindCondition();
                            if (conditionResult2.found && conditionResult2.position[j])
                                foreach (var foundItem in BaseFunctions.foundItemList[j].Values)
                                {
                                    var iD = foundItem.itemData as BinanceItemData;
                                    var positionData2 = iD.positionData[j];
                                    if (BaseFunctions.calSimul)
                                        positionData2.Real = false;     // checkTrend
                                    Trading.TradingSimulEnterSetting(iD, iD.positionData[j], iD.listDic.Values[BaseFunctions.minCV.index].lastStick, !BaseFunctions.calSimul || positionData2.Real);

                                    if (!iD.RealEnter && (orders[j].Count + positions[j].Count) < Trading.instance.LimitUpDownControl[j].Value && 
                                        (!BaseFunctions.calSimul || positionData2.Real))
                                    {
                                        try
                                        {
                                            iD.DetectTime = newStick.Time;
                                            if (!Trading_PlaceOrder(iD, (Position)j, false, true))
                                                BaseFunctions.ShowError(this, "order fail");
                                            else
                                            {
                                                orders[j].Add(iD.Code, iD);

                                                UpdateAssets();

                                                lock (CountLocker)
                                                    EnterCount[j]++;

                                                Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0,
                                                    BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + iD.positionData[j].EnterTime.ToString(BaseFunctions.TimeFormat),
                                                    DBHelper.conn1OrderHistoryColumn1, iD.Code + "~1 매수(자동)", DBHelper.conn1OrderHistoryColumn2, "종가:" + iD.positionData[j].EnterPrice);
                                                BaseFunctions.AlertStart("Real Enter\n" +
                                                    Enum.GetName(typeof(Position), j) + "\n" +
                                                    iD.Code + "\n" +
                                                    newStick.Time.ToString(BaseFunctions.TimeFormat) + "\n" +
                                                    iD.positionData[j].foundList[iD.positionData[j].foundList.Count - 1].chartValues.Text + "\n" +
                                                    "Enter Count: " + EnterCount[j], true, true);
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            BaseFunctions.ShowError(this, e.Message);
                                            throw;
                                        }
                                    }

                                    if (BaseFunctions.lastResultDataForCheckTrend[j] == null || BaseFunctions.lastResultDataForCheckTrend[j].ExitTime != default)
                                    {
                                        var backResultData = new BackResultData() { EnterTime = newStick.Time, Code = foundItem.itemData.Code };
                                        backResultData.beforeResultData = BaseFunctions.lastResultDataForCheckTrend[j];
                                        BaseFunctions.lastResultDataForCheckTrend[j] = backResultData;
                                    }

                                    var resultData = BaseFunctions.lastResultDataForCheckTrend[j];
                                    foundItem.itemData.resultDataForMetric[j] = resultData;
                                    resultData.ingItems.Add(foundItem.itemData.Code, foundItem.itemData.Code);
                                }
                        }
                    }

                Trading.instance.CheckAndUpdateClosePNL(itemData, data.Data.Close);
            }
            else
            {
                if (data0.Timestamp.Subtract(itemData.lastReceiveTime).TotalSeconds >= 1)
                    Trading.instance.CheckAndUpdateClosePNL(itemData, data.Data.Close);

                itemData.lastReceiveTime = data0.Timestamp;
            }
        }

        void RunStartSimul(DateTime enterStartTime)
        {
            var simulStartTime = enterStartTime.Subtract(BaseFunctions.ReadyTimeToCheckBeforeStart);
            var loadStartTime = simulStartTime.Date;

            var currentTime = loadStartTime;
            var beforeFinal = false;
            var Final = false;
            while (true)
            {
                if (currentTime == loadStartTime)
                    foreach (var itemData in Trading.instance.itemDataDic.Values)
                        lock (itemData.listDicLocker)
                            for (int i = BaseChartTimeSet.OneMinute.index; i <= BaseFunctions.maxCV.index; i++)
                            {
                                var v = itemData.listDic.Values[i];
                                for (int j = 0; j < v.list.Count; j++)
                                    if (v.list[j].Time >= loadStartTime)
                                    {
                                        v.currentIndex = j;
                                        break;
                                    }
                                v.lastStickForS = i == BaseChartTimeSet.OneMinute.index ?
                                    (v.list.Count > 0 ? v.list[v.currentIndex] : v.lastStick) :
                                    new TradeStick() { Time = (v.list.Count > 0 ? v.list[v.currentIndex] : v.lastStick).Time };
                            }
                else if (!beforeFinal && currentTime >= Trading.firstFinalMin.AddMinutes(-1))
                {
                    if (currentTime > Trading.firstFinalMin.AddMinutes(-1))
                        BaseFunctions.ShowError(this);

                    var nowTime = BaseFunctions.NowTime();
                    if (nowTime.Second > 20)
                    {
                        nowTime = BaseFunctions.NowTime();
                        Thread.Sleep(63000 - nowTime.Second * 1000 - nowTime.Millisecond);
                    }

                    if (Trading.instance.KlineRcv < Trading.instance.KlineReq)
                        while (Trading.instance.KlineRcv != Trading.instance.KlineReq)
                        {
                            Thread.Sleep(100);
                            if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                                BaseFunctions.ShowError(this);
                        }
                    else if (Trading.instance.KlineRcv > Trading.instance.KlineReq)
                        BaseFunctions.ShowError(this);

                    if (currentTime == Trading.firstFinalMin.AddMinutes(-1))
                        beforeFinal = true;
                }
                else if (beforeFinal)
                    Final = true;

                BaseFunctions.foundItemList = new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>[]
                {
                    new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>(),
                    new SortedList<int, (BaseItemData itemData, List<(DateTime foundTime, ChartValues chartValues)> foundList)>()
                };

                foreach (var itemData in Trading.instance.itemDataDic.Values)
                {
                    var vm = itemData.listDic[BaseChartTimeSet.OneMinute];

                    if (vm.lastStickForS.Time > currentTime)
                        continue;

                    for (int i = (int)Position.Long; i <= (int)Position.Short; i++)
                    {
                        itemData.positionData[i].foundList = new List<(DateTime foundTime, ChartValues chartValues)>();
                        itemData.positionData[i].found = false;
                    }

                    for (int i = BaseChartTimeSet.OneMinute.index; i <= BaseFunctions.maxCV.index; i++)
                    {
                        var v = itemData.listDic.Values[i];
                        var vc = itemData.listDic.Keys[i];

                        if (i != BaseChartTimeSet.OneMinute.index)
                        {
                            var timeDiff = currentTime.Subtract(v.lastStickForS.Time).TotalSeconds;
                            if (timeDiff == vc.seconds)
                            {
                                if (!CompareSticks(v.lastStickForS, v.list[v.currentIndex]))
                                {
                                    BaseFunctions.ShowError(this);
                                    CompareSticks(v.lastStickForS, v.list[v.currentIndex]);
                                }

                                BaseFunctions.SetRSIAandDiff(v.list, v.list[v.currentIndex], v.currentIndex - 1);

                                v.currentIndex++;

                                lock (itemData.listDicLocker)
                                    v.lastStickForS = new TradeStick() { Time = v.currentIndex < v.list.Count ? v.list[v.currentIndex].Time : v.lastStick.Time };

                                if (v.lastStickForS.Time != currentTime)
                                    BaseFunctions.ShowError(this);
                            }
                            else if (timeDiff > vc.seconds || timeDiff < 0)
                                BaseFunctions.ShowError(this);

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
                                BaseFunctions.ShowError(this);
                        }

                        if (!BaseFunctions.calOnlyFullStick)
                            BaseFunctions.SetRSIAandDiff(v.list, v.lastStickForS, v.currentIndex - 1);

                        BaseFunctions.OneChartFindConditionAndAdd(itemData, vc, vm.lastStickForS, v.lastStickForS, vm.currentIndex - 1, v.currentIndex - 1);
                    }

                    for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                    {
                        var positionData = itemData.positionData[j];
                        if ((BaseFunctions.canLStogether ? !positionData.Enter : (!itemData.positionData[(int)Position.Long].Enter && !itemData.positionData[(int)Position.Short].Enter)) && positionData.found)
                            BaseFunctions.foundItemList[j].Add(itemData.number, (itemData, positionData.foundList));
                        else if (positionData.Enter)
                        {
                            var v = itemData.listDic[positionData.EnterFoundForExit.chartValues];
                            if (BaseFunctions.ExitConditionFinal(itemData, (Position)j, vm.lastStickForS, v.lastStickForS, v.currentIndex - 1))
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
                    var conditionResult = BaseFunctions.AllItemFindCondition();
                    if (conditionResult.found)
                        for (int j = (int)Position.Long; j <= (int)Position.Short; j++)
                            if (conditionResult.position[j])
                                foreach (var foundItem in BaseFunctions.foundItemList[j].Values)
                                {
                                    var minV = foundItem.itemData.listDic[BaseChartTimeSet.OneMinute];
                                    var positionData = foundItem.itemData.positionData[j];
                                    BaseFunctions.EnterSetting(positionData, minV.lastStickForS);

                                    if (BaseFunctions.lastResultDataForCheckTrend[j] == null || BaseFunctions.lastResultDataForCheckTrend[j].ExitTime != default)
                                    {
                                        var backResultData = new BackResultData() { EnterTime = currentTime, Code = foundItem.itemData.Code };
                                        backResultData.beforeResultData = BaseFunctions.lastResultDataForCheckTrend[j];
                                        BaseFunctions.lastResultDataForCheckTrend[j] = backResultData;
                                    }

                                    var resultData = BaseFunctions.lastResultDataForCheckTrend[j];
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
                BaseFunctions.AlertStart("margin ratio 50%!!", true, true);

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
                    var itemData = Trading.instance.itemDataDic[position.Symbol] as BinanceItemData;

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
                        var result = await socketClient.FuturesUsdt.SubscribeToMarkPriceUpdatesAsync(itemData.Code, 3000, OnMarkPriceUpdates);
                        if (!result.Success)
                            BaseFunctions.ShowError(this);

                        itemData.markSub = result.Data;

                        Invoke(new Action(() => {
                            itemData.RealEnterPrice = position.EntryPrice;
                            itemData.RealEnterTime = BaseFunctions.NowTime();
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

                            if (itemData == Trading.instance.itemDataShowing)
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
            }
            else if (data.UpdateData.Status == OrderStatus.PartiallyFilled)
            {
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;
                var tradData = itemData.positionWhenOrder ? itemData.ExitTradeData : itemData.EnterTradeData;
                tradData.Detect_Stick_Time = itemData.DetectTime;
                tradData.Here_Get_Time = BaseFunctions.NowTime();
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
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((data.UpdateData.AveragePrice / itemData.RealEnterPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.Exit_Send_Price__Diff = itemData.OrderPrice.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(itemData.OrderPrice / data.UpdateData.AveragePrice * 100, 2) + "%)";
                        resultData.ProfitRate = (double)Math.Round((itemData.RealEnterPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }

                    
                    if (resultData.Exit_Send_Amount__Diff == default)
                    {
                        resultData.Profit_Rate = resultData.ProfitRate + "%(" + 
                            Math.Round((resultData.EnterCommision / (itemData.RealEnterPrice * data.UpdateData.AccumulatedQuantityOfFilledTrades) + data.UpdateData.Commission / (data.UpdateData.AveragePrice * data.UpdateData.AccumulatedQuantityOfFilledTrades)) * 100, 2) + "%)";
                        resultData.Profit_Value = data.UpdateData.RealizedProfit.ToString(BaseFunctions.ForDecimalString) + "$(" + (data.UpdateData.RealizedProfit - resultData.EnterCommision - data.UpdateData.Commission).ToString(BaseFunctions.ForDecimalString) + ")";
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";
                    }

                    Trading.instance.UpdateResult(itemData, itemData.isAuto);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && !itemData.positionWhenOrder) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = itemData.resultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.Exit_Send_Amount__Diff = itemData.OrderAmount.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(Math.Abs(itemData.OrderFilled - itemData.Size) / itemData.OrderAmount * 100, 0) + "%)";
                        resultData.Exit_Send_Time__Diff = resultData.ExitSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.ExitSendTime).Milliseconds / 1000D, 1) + "s)";

                        itemData.ExitCanceledTradeData = itemData.ExitTradeData;

                        Trading_PlaceOrder(itemData, itemData.RealPosition == Position.Long ? Position.Short : Position.Long, true, itemData.isAuto);
                    }
                    else
                    {
                        if (data.UpdateData.AveragePrice != 0)
                        {
                            itemData.RealEnterPrice = data.UpdateData.AveragePrice;
                            resultData.EnterCommision = data.UpdateData.Commission;
                            resultData.Enter_Send_Price__Diff = itemData.OrderPrice.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(data.UpdateData.AveragePrice / itemData.OrderPrice * 100, 2) + "%)";
                        }
                        resultData.Enter_Send_Time__Diff = resultData.EnterSendTime.ToString(BaseFunctions.TimeFormat) + "(" + Math.Round((data.UpdateData.UpdateTime - resultData.EnterSendTime).Milliseconds / 1000D, 1) + "s)";
                        resultData.Enter_Send_Amount__Diff = itemData.OrderAmount.ToString(BaseFunctions.ForDecimalString) + "(" + Math.Round(data.UpdateData.AccumulatedQuantityOfFilledTrades / itemData.OrderAmount * 100, 0) + "%)";
                    }

                    (itemData.isAuto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).RefreshObject(resultData);
                }

                for (int i = (int)Position.Long; i <= (int)Position.Short; i++)
                    orders[i].Remove(itemData.Code);
                UpdateAssets();

                var tradData = itemData.positionWhenOrder ? itemData.ExitTradeData : itemData.EnterTradeData;
                tradData.Detect_Stick_Time = itemData.DetectTime;
                tradData.Here_Get_Time = BaseFunctions.NowTime();
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
            var itemData = Trading.instance.itemDataShowing as BinanceItemData;

            var vc = Trading.instance.mainChart.Tag as ChartValues;

            var start = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            DateTime endTime = BaseFunctions.NowTime();
            if (loadNew)
            {
                itemData.showingStickList.Clear();

                end = 0;
                start = end - Trading.instance.chartViewSticksSize;
            }
            else
                endTime = DateTime.Parse(chart.Series[0].Points[0].AxisLabel).AddSeconds(-1);

            var size = Trading.instance.baseLoadSticksSize;

            var result = client.FuturesUsdt.Market.GetKlinesAsync(itemData.Code, (KlineInterval)vc.original, null, endTime, size + BaseFunctions.TotalNeedDays - 1).Result;
            if (!result.Success)
                BaseFunctions.ShowError(this);

            BaseFunctions.BinanceUpdateWeightNow(result.ResponseHeaders);

            var list = SetRSIAandGetList(result.Data, endTime.AddSeconds(-(int)endTime.TimeOfDay.TotalSeconds % vc.seconds).AddSeconds(-(size - 1) * vc.seconds), vc);

            if (!loadNew)
            {
                var firstCount = list.Count;
                list.AddRange(itemData.showingStickList.GetRange(0, BaseFunctions.NeedDays + 1));
                for (int i = firstCount; i < list.Count; i++)
                    BaseFunctions.SetRSIAandDiff(list, list[i], i - 1, int.MinValue, vc);
                list.RemoveRange(firstCount, list.Count - firstCount);
            }

            for (int i = list.Count - 1; i >= 0; i--)
                Trading.instance.InsertFullChartPoint(chart, list[i]);

            if (loadNew)
            {
                itemData.showingStick = list[list.Count - 1];
                itemData.currentPriceWhenLoaded = itemData.showingStick.Price[3];
                list.RemoveAt(list.Count - 1);
            }
            itemData.showingStickList.InsertRange(0, list);
            BaseFunctions.SetRSIAandDiff(itemData.showingStickList, itemData.showingStick, int.MinValue, int.MinValue, vc);

            start += list.Count + 1;
            end += list.Count + 1;

            Trading.instance.ZoomX(chart, start, end);
            
            Trading.instance.AdjustChart(chart);

            itemData.showingVC = vc;
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
            var minList = itemData.listDic[BaseChartTimeSet.OneMinute];

            tradeData.Send_Close_Price = itemData.AggOn && !itemData.AggFirst ? itemData.secStick.Price[3] : minList.lastStick.Price[3];
            tradeData.Last_Min_Qnt = minList.lastStick.Ms + minList.lastStick.Md;

            decimal priceRate = 0.1m;
            if (!auto && !decimal.TryParse(orderPriceTextBox1.Text, out priceRate))
            {
                BaseFunctions.ShowError(this, "priceRate input error");
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
                BaseFunctions.ShowError(this, "order input error");
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

            var orderType = OrderType.Limit;
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
                        timeInForce = TimeInForce.GoodTillCancel;
                }
                else
                    orderType = OrderType.Market;
            }
            else
                timeInForce = TimeInForce.GoodTillCancel;


            if (!itemData.RealEnter && price * quantity < itemData.minNotionalValue)
            {
                BaseFunctions.ShowError(this, "prcie quantity error");
                return false;
            }

            if (orderType == OrderType.Market)
                price = null;

            if (!auto)
                orderSizeTextBox1.Text = quantity.ToString(BaseFunctions.ForDecimalString);

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
                        itemData.DetectTime = BaseFunctions.NowTime();
                    }
                    else
                        itemData.isAutoHandOut = false;
                }
            }

            var result = client.FuturesUsdt.Order.PlaceOrderAsync(
                itemData.Code
                , position == Position.Long ? OrderSide.Buy : OrderSide.Sell
                , orderType
                , quantity
                , PositionSide.Both
                , timeInForce
                , itemData.positionWhenOrder && (auto || ROCheckBox.Checked)
                , price).Result;
            if (!result.Success)
            {
                BaseFunctions.ShowError(this, "order fail");
                return false;
            }

            tradeData.Here_Send_Time = BaseFunctions.NowTime();
            tradeData.Send_Price = price == null ? 0 : (decimal)price;
            tradeData.Send_Qnt = quantity;
            tradeData.Position = position;

            var result2 = client.FuturesUsdt.Order.CancelAllOrdersAfterTimeoutAsync(itemData.Code, TimeSpan.FromSeconds(1)).Result;
            if (!result2.Success)
                BaseFunctions.ShowError(this, "timeout cancel order fail");

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

                itemData.resultData.ExitSendTime = BaseFunctions.NowTime();
            }
            else
            {
                itemData.EnterTradeData = tradeData;

                itemData.RealPosition = position == Position.Long ? Position.Long : Position.Short;

                itemData.resultData = new TradeResultData { Code = itemData.Code };
                itemData.resultData.Position = position == Position.Long ? "Long" : "Short";
                itemData.resultData.EnterSendTime = BaseFunctions.NowTime();
                if (InvokeRequired)
                    BeginInvoke(new Action(() => {
                        (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });
                    }));
                else
                    (auto ? Trading.instance.realAutoResultListView : Trading.instance.realHandResultListView).InsertObjects(0, new List<TradeResultData> { itemData.resultData });

                itemData.Size = 0;
            }

            Trading.instance.dbHelper.SaveData1(DBHelper.conn1OrderHistoryName, DBHelper.conn1OrderHistoryColumn0, BaseFunctions.NowTime().ToString(BaseFunctions.TimeFormat) + "~" + tradeData.Here_Send_Time.ToString(BaseFunctions.TimeFormat),
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