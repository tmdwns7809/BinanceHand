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

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        #region Global vars
        string startTime;

        DBHelper dbHelper;

        Dictionary<string, ItemData> FUItemDataList = new Dictionary<string, ItemData>();

        ItemData itemDataShowing;

        static int PrcAlpha = 255;
        static int amtAlpha = 255;
        static int stripAlpha = 5;
        static int simulStripAlpha = 20;
        Color msAmtColor = Color.FromArgb(amtAlpha, 0, 89, 64);
        Color pureMsAmtColor = Color.FromArgb(amtAlpha, 19, 158, 119);
        Color realSmallAmtColor = Color.FromArgb(stripAlpha, Color.LightBlue);
        Color mdAmtColor = Color.FromArgb(amtAlpha, 130, 17, 43);
        Color pureMdAmtColor = Color.FromArgb(amtAlpha, 200, 24, 64);
        Color realBigAmtColor = Color.FromArgb(stripAlpha, Color.LightYellow);
        Color plsPrcColor = Color.FromArgb(PrcAlpha, 14, 203, 129);
        Color mnsPrcColor = Color.FromArgb(PrcAlpha, 207, 48, 74);
        Color earningColor = Color.FromArgb(simulStripAlpha, Color.Gold);
        Color losingColor = Color.FromArgb(simulStripAlpha, Color.LightGray);

        int baseChartViewSticksSize = 120;

        static int gridAlpha = 20;
        Color gridColor = Color.FromArgb(gridAlpha, Color.Gray);

        Color controlBackColor = Color.FromArgb(24, 26, 32);
        Color buttonSelectedColor = Color.DarkGray;
        Color buttonColor = Color.Gray;

        string ForDecimalString = "0.#############################";

        bool testnet = false;

        int maxSecListCount = 600;

        short FUAggRcv = 0;
        short FUAggReq = 0;
        short FUKlineRcv = 0;

        Chart chartNow;

        BinanceClient client;
        BinanceSocketClient socketClient;
        CancellationTokenSource tokenSource = new CancellationTokenSource();

        int numberOfFrames = 0;
        int currentFrame = 0;
        Image fail = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\fail0\fail0.gif");
        Image win0 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win0\win0.gif");
        Image win1 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win1\win1.gif");
        Image win2 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win2\win2.gif");
        Image win3 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win3\win3.gif");

        static ISoundEngine soundEngine = new ISoundEngine() { SoundVolume = 0.2f };
        ISoundSource failSound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\fail0\fail0.ogg");
        ISoundSource win0Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win0\win0.ogg");
        ISoundSource win1Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win1\win1.ogg");
        ISoundSource win2Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win2\win2.ogg");
        ISoundSource win3Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win3\win3.ogg");

        delegate void OrderUpdates(BinanceFuturesStreamOrderUpdate data, ItemData itemData);
        OrderUpdates orderUpdates;
        delegate void AccountUpdates(BinanceFuturesStreamAccountUpdate data);
        AccountUpdates accountUpdates;
        delegate void MarkUpdates(BinanceFuturesUsdtStreamMarkPrice data, ItemData itemData);
        MarkUpdates markUpdates;
        delegate void AggUpdates(BinanceStreamAggregatedTrade data, ItemData itemData);
        AggUpdates aggUpdates;
        delegate void KlineUpdates(IBinanceStreamKlineData data, ItemData itemData);
        KlineUpdates klineUpdates;
        delegate void HoUpdates(IBinanceFuturesEventOrderBook data, ItemData itemData);
        HoUpdates hoUpdates;

        List<string> FUSymbolList = new List<string>();

        bool positionExitPriceMarket = false;
        bool miniSizeDefault = true;

        decimal totalRateSum = 0;
        int totalTrades = 0;
        int totalWin = 0;
        decimal thisRateSum = 0;
        int thisTrades = 0;
        int thisWin = 0;
        decimal simulTotalRateSum = 0;
        int simulTotalTrades = 0;
        int simulTotalWin = 0;
        decimal simulThisRateSum = 0;
        int simulThisTrades = 0;
        int simulThisWin = 0;

        decimal commisionRate = 0.1m;

        string secTimeFormat = "yyyy-MM-dd HH:mm:ss";
        string secTimeFormatSCV = "yyyy-MM-dd HHmmss";
        string minTimeFormat = "yyyy-MM-dd HH:mm";

        Dictionary<string, Asset> assetDic = new Dictionary<string, Asset>();
        #endregion

        public Form1()
        {
            InitializeComponent();

            SetComponents();

            SetClientAndKey();
        }

        void SetComponents()
        {
            WindowState = FormWindowState.Maximized;

            startTime = DateTime.UtcNow.ToString();

            Text = "Binance       UTC-" + startTime;

            BackColor = Color.FromArgb(30, 32, 38);
            ForeColor = Color.AntiqueWhite;

            SetChart();

            SetOrderView();

            SetResultView();

            SetSymbolsListView();
        }
        void SetChart()
        {
            chartNow = minChart;

            secChart.BackColor = BackColor;
            secChart.ForeColor = ForeColor;
            secChart.Location = new Point(0, 0);
            secChart.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.8), (int)(Screen.GetWorkingArea(this).Size.Height * 0.8));

            secChart.Tag = 0.005;
            secChart.AxisViewChanged += (sender, e) => { AdjustChart(secChart); };
            secChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            secChart.MouseWheel += OnChartMouseWheel;

            var chartArea0 = secChart.ChartAreas.Add("chartArea0");
            chartArea0.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartArea0.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartArea0.AxisX.MajorGrid.LineColor = gridColor;
            chartArea0.AxisX.MajorGrid.Interval = 5;
            chartArea0.AxisX.MajorTickMark.Size = 0.4f;
            chartArea0.AxisX.LabelStyle.Interval = 30;
            chartArea0.AxisX.LabelStyle.Format = ItemData.secChartLabel;
            chartArea0.AxisX.LabelStyle.ForeColor = Color.Transparent;
            chartArea0.AxisX.ScrollBar.BackColor = Color.Transparent;
            chartArea0.AxisX.ScrollBar.ButtonColor = Color.Transparent;
            chartArea0.AxisX.ScrollBar.LineColor = Color.Transparent;
            chartArea0.AxisX.LineColor = gridColor;

            chartArea0.AxisY.Enabled = AxisEnabled.False;
            chartArea0.AxisY2.Enabled = AxisEnabled.True;
            chartArea0.AxisY2.MajorGrid.LineColor = gridColor;
            chartArea0.AxisY2.ScrollBar.Enabled = false;
            chartArea0.AxisY2.MajorTickMark.Enabled = false;
            chartArea0.AxisY2.IsStartedFromZero = false;
            chartArea0.AxisY2.LabelStyle.ForeColor = ForeColor;
            chartArea0.AxisY2.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea0.AxisY2.LineColor = gridColor;

            chartArea0.Position = new ElementPosition(0, 0, 100, 60);
            chartArea0.BackColor = controlBackColor;
            chartArea0.BorderColor = ForeColor;

            var chartArea1 = secChart.ChartAreas.Add("chartArea1");
            chartArea1.BackColor = chartArea0.BackColor;
            chartArea1.BorderColor = chartArea0.BorderColor;
            chartArea1.Position = new ElementPosition(chartArea0.Position.X, chartArea0.Position.Height - 5, chartArea0.Position.Width, 100 - chartArea0.Position.Height + 6);

            chartArea1.AxisX.ScaleView.SizeType = chartArea0.AxisX.ScaleView.SizeType;
            chartArea1.AxisX.ScaleView.Small​Scroll​Size = chartArea0.AxisX.ScaleView.Small​Scroll​Size;
            chartArea1.AxisX.MajorGrid.LineColor = gridColor;
            chartArea1.AxisX.MajorGrid.Interval = chartArea0.AxisX.MajorGrid.Interval;
            chartArea1.AxisX.MajorTickMark.LineColor = gridColor;
            chartArea1.AxisX.MajorTickMark.Size = chartArea0.AxisX.MajorTickMark.Size;
            chartArea1.AxisX.LabelStyle.Interval = chartArea0.AxisX.LabelStyle.Interval;
            chartArea1.AxisX.LabelStyle.Format = chartArea0.AxisX.LabelStyle.Format;
            chartArea1.AxisX.LabelStyle.ForeColor = ForeColor;
            chartArea1.AxisX.ScrollBar.BackColor = BackColor;
            chartArea1.AxisX.ScrollBar.ButtonColor = Color.FromArgb(18, 22, 28);
            chartArea1.AxisX.ScrollBar.LineColor = Color.FromArgb(150, Color.LightGray);
            chartArea1.AxisX.LineColor = gridColor;

            chartArea1.AxisY.Enabled = AxisEnabled.False;
            chartArea1.AxisY2.Enabled = AxisEnabled.True;
            chartArea1.AxisY2.MajorGrid.Enabled = false;
            chartArea1.AxisY2.ScrollBar.Enabled = false;
            chartArea1.AxisY2.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.LabelStyle.ForeColor = ForeColor;
            chartArea1.AxisY2.LineColor = gridColor;

            chartArea1.AlignWithChartArea = chartArea0.Name;

            var series0 = secChart.Series.Add("Price0");
            series0.ChartType = SeriesChartType.Candlestick;
            series0.XValueMember = "Time";
            series0.YValueMembers = "High,Low,Open,Close";
            series0.XValueType = ChartValueType.Time;
            series0.Color = plsPrcColor;
            series0.YAxisType = AxisType.Secondary;
            series0.ChartArea = chartArea0.Name;

            var series1 = secChart.Series.Add("Amount0");
            series1.ChartType = SeriesChartType.StackedColumn;
            series1.XValueType = ChartValueType.Time;
            series1.Color = mdAmtColor;
            series1.YAxisType = AxisType.Secondary;
            series1.ChartArea = chartArea1.Name;

            var series2 = secChart.Series.Add("Amount1");
            series2.ChartType = SeriesChartType.StackedColumn;
            series2.XValueType = ChartValueType.Time;
            series2.Color = pureMdAmtColor;
            series2.YAxisType = AxisType.Secondary;
            series2.ChartArea = chartArea1.Name;

            var series3 = secChart.Series.Add("Amount2");
            series3.ChartType = SeriesChartType.StackedColumn;
            series3.XValueType = ChartValueType.Time;
            series3.Color = msAmtColor;
            series3.YAxisType = AxisType.Secondary;
            series3.ChartArea = chartArea1.Name;

            var series4 = secChart.Series.Add("Amount3");
            series4.ChartType = SeriesChartType.StackedColumn;
            series4.XValueType = ChartValueType.Time;
            series4.Color = pureMsAmtColor;
            series4.YAxisType = AxisType.Secondary;
            series4.ChartArea = chartArea1.Name;


            minChart.BackColor = BackColor;
            minChart.ForeColor = ForeColor;
            minChart.Location = secChart.Location;
            minChart.Size = secChart.Size;

            minChart.Tag = 0.02;
            minChart.AxisViewChanged += (sender, e) => { AdjustChart(minChart); };
            minChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            minChart.MouseWheel += OnChartMouseWheel;

            chartArea0 = minChart.ChartAreas.Add(secChart.ChartAreas[0].Name);
            chartArea0.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartArea0.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartArea0.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartArea0.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartArea0.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartArea0.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartArea0.AxisX.LabelStyle.Format = ItemData.minChartLabel;
            chartArea0.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartArea0.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartArea0.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartArea0.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartArea0.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartArea0.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartArea0.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartArea0.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartArea0.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartArea0.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartArea0.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartArea0.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartArea0.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartArea0.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartArea0.Position = secChart.ChartAreas[0].Position;
            chartArea0.BackColor = secChart.ChartAreas[0].BackColor;
            chartArea0.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartArea1 = minChart.ChartAreas.Add(secChart.ChartAreas[1].Name);
            chartArea1.BackColor = secChart.ChartAreas[1].BackColor;
            chartArea1.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartArea1.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartArea1.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartArea1.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartArea1.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartArea1.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartArea1.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartArea1.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartArea1.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartArea1.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartArea1.AxisX.LabelStyle.Format = chartArea0.AxisX.LabelStyle.Format;
            chartArea1.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartArea1.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartArea1.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartArea1.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartArea1.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartArea1.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartArea1.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartArea1.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartArea1.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartArea1.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartArea1.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartArea1.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartArea1.AlignWithChartArea = chartArea0.Name;

            series0 = minChart.Series.Add(secChart.Series[0].Name);
            series0.ChartType = SeriesChartType.Candlestick;
            series0.XValueMember = "Time";
            series0.YValueMembers = "High,Low,Open,Close";
            series0.XValueType = ChartValueType.Time;
            series0.Color = secChart.Series[0].Color;
            series0.YAxisType = AxisType.Secondary;
            series0.ChartArea = chartArea0.Name;

            series1 = minChart.Series.Add(secChart.Series[1].Name);
            series1.ChartType = SeriesChartType.StackedColumn;
            series1.XValueType = ChartValueType.Time;
            series1.Color = secChart.Series[1].Color;
            series1.YAxisType = secChart.Series[1].YAxisType;
            series1.ChartArea = chartArea1.Name;

            series2 = minChart.Series.Add(secChart.Series[2].Name);
            series2.ChartType = SeriesChartType.StackedColumn;
            series2.XValueType = ChartValueType.Time;
            series2.Color = secChart.Series[2].Color;
            series2.YAxisType = secChart.Series[2].YAxisType;
            series2.ChartArea = chartArea1.Name;

            series3 = minChart.Series.Add(secChart.Series[3].Name);
            series3.ChartType = SeriesChartType.StackedColumn;
            series3.XValueType = ChartValueType.Time;
            series3.Color = secChart.Series[3].Color;
            series3.YAxisType = secChart.Series[3].YAxisType;
            series3.ChartArea = chartArea1.Name;

            series4 = minChart.Series.Add(secChart.Series[4].Name);
            series4.ChartType = SeriesChartType.StackedColumn;
            series4.XValueType = ChartValueType.Time;
            series4.Color = secChart.Series[4].Color;
            series4.YAxisType = secChart.Series[3].YAxisType;
            series4.ChartArea = chartArea1.Name;

            hourChart.BackColor = BackColor;
            hourChart.ForeColor = ForeColor;
            hourChart.Location = secChart.Location;
            hourChart.Size = secChart.Size;

            hourChart.Tag = 0.05;
            hourChart.AxisViewChanged += (sender, e) => { AdjustChart(hourChart); };
            hourChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            hourChart.MouseWheel += OnChartMouseWheel;

            chartArea0 = hourChart.ChartAreas.Add(secChart.ChartAreas[0].Name);
            chartArea0.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartArea0.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartArea0.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartArea0.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartArea0.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartArea0.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartArea0.AxisX.LabelStyle.Format = ItemData.hourChartLabel;
            chartArea0.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartArea0.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartArea0.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartArea0.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartArea0.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartArea0.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartArea0.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartArea0.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartArea0.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartArea0.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartArea0.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartArea0.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartArea0.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartArea0.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartArea0.Position = secChart.ChartAreas[0].Position;
            chartArea0.BackColor = secChart.ChartAreas[0].BackColor;
            chartArea0.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartArea1 = hourChart.ChartAreas.Add(secChart.ChartAreas[1].Name);
            chartArea1.BackColor = secChart.ChartAreas[1].BackColor;
            chartArea1.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartArea1.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartArea1.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartArea1.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartArea1.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartArea1.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartArea1.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartArea1.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartArea1.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartArea1.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartArea1.AxisX.LabelStyle.Format = chartArea0.AxisX.LabelStyle.Format;
            chartArea1.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartArea1.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartArea1.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartArea1.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartArea1.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartArea1.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartArea1.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartArea1.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartArea1.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartArea1.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartArea1.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartArea1.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartArea1.AlignWithChartArea = chartArea0.Name;

            series0 = hourChart.Series.Add(secChart.Series[0].Name);
            series0.ChartType = SeriesChartType.Candlestick;
            series0.XValueMember = "Time";
            series0.YValueMembers = "High,Low,Open,Close";
            series0.XValueType = ChartValueType.Time;
            series0.Color = secChart.Series[0].Color;
            series0.YAxisType = AxisType.Secondary;
            series0.ChartArea = chartArea0.Name;

            series1 = hourChart.Series.Add(secChart.Series[1].Name);
            series1.ChartType = SeriesChartType.StackedColumn;
            series1.XValueType = ChartValueType.Time;
            series1.Color = secChart.Series[1].Color;
            series1.YAxisType = secChart.Series[1].YAxisType;
            series1.ChartArea = chartArea1.Name;

            series2 = hourChart.Series.Add(secChart.Series[2].Name);
            series2.ChartType = SeriesChartType.StackedColumn;
            series2.XValueType = ChartValueType.Time;
            series2.Color = secChart.Series[2].Color;
            series2.YAxisType = secChart.Series[2].YAxisType;
            series2.ChartArea = chartArea1.Name;

            series3 = hourChart.Series.Add(secChart.Series[3].Name);
            series3.ChartType = SeriesChartType.StackedColumn;
            series3.XValueType = ChartValueType.Time;
            series3.Color = secChart.Series[3].Color;
            series3.YAxisType = secChart.Series[3].YAxisType;
            series3.ChartArea = chartArea1.Name;

            series4 = hourChart.Series.Add(secChart.Series[4].Name);
            series4.ChartType = SeriesChartType.StackedColumn;
            series4.XValueType = ChartValueType.Time;
            series4.Color = secChart.Series[4].Color;
            series4.YAxisType = secChart.Series[3].YAxisType;
            series4.ChartArea = chartArea1.Name;


            dayChart.BackColor = BackColor;
            dayChart.ForeColor = ForeColor;
            dayChart.Location = secChart.Location;
            dayChart.Size = secChart.Size;

            dayChart.Tag = 0.1;
            dayChart.AxisViewChanged += (sender, e) => { AdjustChart(dayChart); };
            dayChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            dayChart.MouseWheel += OnChartMouseWheel;

            chartArea0 = dayChart.ChartAreas.Add(secChart.ChartAreas[0].Name);
            chartArea0.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartArea0.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartArea0.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartArea0.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartArea0.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartArea0.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartArea0.AxisX.LabelStyle.Format = ItemData.dayChartLabel;
            chartArea0.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartArea0.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartArea0.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartArea0.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartArea0.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartArea0.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartArea0.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartArea0.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartArea0.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartArea0.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartArea0.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartArea0.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartArea0.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartArea0.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartArea0.Position = secChart.ChartAreas[0].Position;
            chartArea0.BackColor = secChart.ChartAreas[0].BackColor;
            chartArea0.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartArea1 = dayChart.ChartAreas.Add(secChart.ChartAreas[1].Name);
            chartArea1.BackColor = secChart.ChartAreas[1].BackColor;
            chartArea1.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartArea1.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartArea1.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartArea1.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartArea1.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartArea1.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartArea1.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartArea1.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartArea1.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartArea1.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartArea1.AxisX.LabelStyle.Format = chartArea0.AxisX.LabelStyle.Format;
            chartArea1.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartArea1.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartArea1.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartArea1.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartArea1.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartArea1.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartArea1.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartArea1.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartArea1.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartArea1.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartArea1.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartArea1.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartArea1.AlignWithChartArea = chartArea0.Name;

            series0 = dayChart.Series.Add(secChart.Series[0].Name);
            series0.ChartType = SeriesChartType.Candlestick;
            series0.XValueMember = "Time";
            series0.YValueMembers = "High,Low,Open,Close";
            series0.XValueType = ChartValueType.Time;
            series0.Color = secChart.Series[0].Color;
            series0.YAxisType = AxisType.Secondary;
            series0.ChartArea = chartArea0.Name;

            series1 = dayChart.Series.Add(secChart.Series[1].Name);
            series1.ChartType = SeriesChartType.StackedColumn;
            series1.XValueType = ChartValueType.Time;
            series1.Color = secChart.Series[1].Color;
            series1.YAxisType = secChart.Series[1].YAxisType;
            series1.ChartArea = chartArea1.Name;

            series2 = dayChart.Series.Add(secChart.Series[2].Name);
            series2.ChartType = SeriesChartType.StackedColumn;
            series2.XValueType = ChartValueType.Time;
            series2.Color = secChart.Series[2].Color;
            series2.YAxisType = secChart.Series[2].YAxisType;
            series2.ChartArea = chartArea1.Name;

            series3 = dayChart.Series.Add(secChart.Series[3].Name);
            series3.ChartType = SeriesChartType.StackedColumn;
            series3.XValueType = ChartValueType.Time;
            series3.Color = secChart.Series[3].Color;
            series3.YAxisType = secChart.Series[3].YAxisType;
            series3.ChartArea = chartArea1.Name;

            series4 = dayChart.Series.Add(secChart.Series[4].Name);
            series4.ChartType = SeriesChartType.StackedColumn;
            series4.XValueType = ChartValueType.Time;
            series4.Color = secChart.Series[4].Color;
            series4.YAxisType = secChart.Series[3].YAxisType;
            series4.ChartArea = chartArea1.Name;

            secButton.BackColor = buttonColor;
            secButton.ForeColor = ForeColor;
            secButton.Location = new Point(50, 20);
            secButton.Click += (sebder, e) => { if (!secChart.Visible) SetChartNowOrLoad(secChart); };
            secButton.BringToFront();

            minButton.BackColor = buttonSelectedColor;
            minButton.ForeColor = ForeColor;
            minButton.Location = new Point(secButton.Location.X + secButton.Size.Width + 1, secButton.Location.Y);
            minButton.Click += (sebder, e) => { if (!minChart.Visible) SetChartNowOrLoad(minChart); };
            minButton.BringToFront();

            hourButton.BackColor = buttonColor;
            hourButton.ForeColor = ForeColor;
            hourButton.Location = new Point(minButton.Location.X + minButton.Size.Width + 1, minButton.Location.Y);
            hourButton.Click += (sebder, e) => { if (!hourChart.Visible) SetChartNowOrLoad(hourChart); };
            hourButton.BringToFront();

            dayButton.BackColor = buttonColor;
            dayButton.ForeColor = ForeColor;
            dayButton.Location = new Point(hourButton.Location.X + hourButton.Size.Width + 1, hourButton.Location.Y);
            dayButton.Click += (sebder, e) => { if (!dayChart.Visible) SetChartNowOrLoad(dayChart); };
            dayButton.BringToFront();

            hoChart.BackColor = BackColor;
            hoChart.ForeColor = ForeColor;
            hoChart.Location = new Point(secChart.Location.X + secChart.Size.Width, secChart.Location.Y + secChart.Size.Height - 3 * secChart.Size.Height / 4);
            hoChart.Size = new Size(Screen.GetWorkingArea(this).Size.Width - secChart.Width - 10, 3 * secChart.Size.Height / 4);
            hoChart.Tag = 20;

            chartArea0 = hoChart.ChartAreas.Add(secChart.ChartAreas[0].Name);
            chartArea0.AxisX.MajorGrid.Enabled = false;
            chartArea0.AxisX.MajorTickMark.Enabled = false;
            chartArea0.AxisX.LabelStyle.Enabled = true;
            chartArea0.AxisX.LabelStyle.ForeColor = ForeColor;
            chartArea0.AxisX.LabelStyle.Interval = 1;
            chartArea0.AxisX.IntervalAutoMode = IntervalAutoMode.FixedCount;
            chartArea0.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartArea0.AxisY.MajorGrid.Enabled = false;
            chartArea0.AxisY.MajorTickMark.Enabled = false;
            chartArea0.AxisY.LabelStyle.Enabled = false;
            //chartArea0.AxisY.LabelStyle.Format = "{0:0,}K";
            //chartArea0.AxisY.LabelStyle.ForeColor = ForeColor;
            chartArea0.AxisY.ScaleView.Position = 0;
            chartArea0.AxisY.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartArea0.Position = new ElementPosition(0, 0, 100, 100);
            chartArea0.BackColor = secChart.ChartAreas[0].BackColor;
            chartArea0.BorderColor = secChart.ChartAreas[0].BorderColor;

            series0 = hoChart.Series.Add(secChart.Series[0].Name);
            series0.ChartType = SeriesChartType.StackedBar;
            series0.Color = msAmtColor;
            series0.ChartArea = chartArea0.Name;
            series0.YAxisType = AxisType.Primary;
            //series0.Label = "#VALY";
            //series0.LabelForeColor = ForeColor;
            //series0["LabelStyle"] = "Bottom";

            series1 = hoChart.Series.Add(secChart.Series[1].Name);
            series1.ChartType = SeriesChartType.StackedBar;
            series1.Color = mdAmtColor;
            series1.ChartArea = chartArea0.Name;
            series1.YAxisType = AxisType.Primary;

            for (int i = 0; i < (int)hoChart.Tag * 2; i++)
            {
                series0.Points.AddXY(0, 0);
                series1.Points.AddXY(0, 0);
            }

            hoHighPriceTextBox.BackColor = BackColor;
            hoHighPriceTextBox.ForeColor = ForeColor;
            hoHighPriceTextBox.Location = new Point(hoChart.Location.X + hoChart.Size.Width / 2 - hoHighPriceTextBox.Width / 2, hoChart.Location.Y + hoChart.Size.Height - hoHighPriceTextBox.Height);
        }
        void OnChartAxisScrollBarClicked(object sender, ScrollBarEventArgs e)
        {
            if (e.ButtonType == ScrollBarButtonType.SmallDecrement)
            {
                ChartScroll(chartNow, ScrollType.SmallDecrement);
                e.IsHandled = true;
            }
            else if (e.ButtonType == ScrollBarButtonType.SmallIncrement)
            {
                ChartScroll(chartNow, ScrollType.SmallIncrement);
                e.IsHandled = true;
            }
        }
        void OnChartMouseWheel(object sender, MouseEventArgs e)
        {
            if (chartNow == null)
                return;

            if (chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum > 100000)
                return;

            if (e.Delta < 0)
                ChartScroll(chartNow, ScrollType.SmallDecrement);
            else
                ChartScroll(chartNow, ScrollType.SmallIncrement);
        }
        void SetOrderView()
        {
            assetsListView.BackColor = controlBackColor;
            assetsListView.ForeColor = ForeColor;
            assetsListView.Location = new Point(10, secChart.Location.Y + secChart.Size.Height + 10);
            assetsListView.Size = new Size(350, Screen.GetWorkingArea(this).Size.Height - secChart.Size.Height - 50);
            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var column0 = new OLVColumn("Asset Name", "AssetName");
            column0.FreeSpaceProportion = 3;
            column0.HeaderFormatStyle = headerstyle;
            assetsListView.AllColumns.Add(column0);
            var column1 = new OLVColumn("Amount", "Amount");
            column1.FreeSpaceProportion = 3;
            column1.HeaderFormatStyle = headerstyle;
            assetsListView.AllColumns.Add(column1);
            assetsListView.Columns.AddRange(new ColumnHeader[] { column0, column1 });

            orderGroupBox.BackColor = BackColor;
            orderGroupBox.ForeColor = ForeColor;
            orderGroupBox.Location = new Point(assetsListView.Location.X + assetsListView.Width + 10, assetsListView.Location.Y);
            orderGroupBox.Size = new Size(350, assetsListView.Height);

            GTCRadioButton.BackColor = BackColor;
            GTCRadioButton.ForeColor = ForeColor;
            GTCRadioButton.Location = new Point(5, 15);

            IOCRadioButton.BackColor = BackColor;
            IOCRadioButton.ForeColor = ForeColor;
            IOCRadioButton.Location = new Point(GTCRadioButton.Location.X + GTCRadioButton.Size.Width + 3, GTCRadioButton.Location.Y);

            PORadioButton.BackColor = BackColor;
            PORadioButton.ForeColor = ForeColor;
            PORadioButton.Location = new Point(IOCRadioButton.Location.X + IOCRadioButton.Size.Width + 3, GTCRadioButton.Location.Y);

            leverageTextBox0.BackColor = controlBackColor;
            leverageTextBox0.ForeColor = ForeColor;
            leverageTextBox0.Location = new Point(PORadioButton.Location.X + PORadioButton.Size.Width + 20, GTCRadioButton.Location.Y);
            leverageTextBox0.KeyDown += (sender, e) =>
            {
                if (e.KeyCode != Keys.Enter)
                    return;

                if (!int.TryParse(leverageTextBox0.Text, out int leverage) || leverage < 1 || leverage > itemDataShowing.maxLeverage || itemDataShowing == null)
                {
                    leverageTextBox0.Clear();
                    return;
                }

                var data = client.FuturesUsdt.ChangeInitialLeverage(itemDataShowing.Name, leverage);

                if (!data.Success)
                {
                    MessageBox.Show("change fail");
                    return;
                }
                else
                    MessageBox.Show("success");

                itemDataShowing.Leverage = data.Data.Leverage;
                if (data.Data.MaxNotionalValue == "INF")
                    itemDataShowing.maxNotionalValue = decimal.MaxValue;
                else
                    itemDataShowing.maxNotionalValue = decimal.Parse(data.Data.MaxNotionalValue);

                FUListView.Focus();
            };

            leverageTextBox1.BackColor = BackColor;
            leverageTextBox1.ForeColor = ForeColor;
            leverageTextBox1.Location = new Point(leverageTextBox0.Location.X + leverageTextBox0.Size.Width + 5, leverageTextBox0.Location.Y + (leverageTextBox0.Size.Height - leverageTextBox1.Size.Height) / 2);

            orderPriceTextBox0.BackColor = BackColor;
            orderPriceTextBox0.ForeColor = ForeColor;
            orderPriceTextBox0.Location = new Point(GTCRadioButton.Location.X, GTCRadioButton.Location.Y + GTCRadioButton.Size.Height + 7 + (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);

            orderPriceTextBox1.BackColor = controlBackColor;
            orderPriceTextBox1.ForeColor = ForeColor;
            orderPriceTextBox1.Location = new Point(orderPriceTextBox0.Location.X + orderPriceTextBox0.Size.Width + 3, orderPriceTextBox0.Location.Y - (orderPriceTextBox1.Size.Height - orderPriceTextBox0.Size.Height) / 2);

            orderPriceTextBox2.BackColor = BackColor;
            orderPriceTextBox2.ForeColor = ForeColor;
            orderPriceTextBox2.Location = new Point(orderPriceTextBox1.Location.X + orderPriceTextBox1.Size.Width + 3, orderPriceTextBox0.Location.Y);

            marketRadioButton.BackColor = BackColor;
            marketRadioButton.ForeColor = ForeColor;
            marketRadioButton.Location = new Point(orderPriceTextBox2.Location.X + orderPriceTextBox2.Size.Width + 10, orderPriceTextBox0.Location.Y);
            marketRadioButton.CheckedChanged += (sender, e) => { if (marketRadioButton.Checked) orderPriceTextBox1.Enabled = false; else orderPriceTextBox1.Enabled = true; };

            orderSizeTextBox0.BackColor = BackColor;
            orderSizeTextBox0.ForeColor = ForeColor;
            orderSizeTextBox0.Location = new Point(orderPriceTextBox0.Location.X, orderPriceTextBox0.Location.Y + orderPriceTextBox0.Size.Height + 7 + (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);
            
            orderSizeTextBox1.BackColor = controlBackColor;
            orderSizeTextBox1.ForeColor = ForeColor;
            orderSizeTextBox1.Location = new Point(orderSizeTextBox0.Location.X + orderSizeTextBox0.Size.Width + 3, orderSizeTextBox0.Location.Y - (orderSizeTextBox1.Size.Height - orderSizeTextBox0.Size.Height) / 2);

            miniSizeCheckBox.BackColor = BackColor;
            miniSizeCheckBox.ForeColor = ForeColor;
            miniSizeCheckBox.Location = new Point(marketRadioButton.Location.X, orderSizeTextBox0.Location.Y);
            miniSizeCheckBox.CheckedChanged += (sender, e) => { if (itemDataShowing != null) TickMinSizeButton(miniSizeCheckBox.Checked); };

            autoSizeCheckBox.BackColor = BackColor;
            autoSizeCheckBox.ForeColor = ForeColor;
            autoSizeCheckBox.Location = new Point(orderSizeTextBox0.Location.X + 20, orderSizeTextBox0.Location.Y + orderSizeTextBox0.Size.Height + 10 + (autoSizeTextBox0.Size.Height - autoSizeCheckBox.Size.Height) / 2);
            autoSizeCheckBox.CheckedChanged += (sender, e) => { if (itemDataShowing != null) TickAutoSizeButton(autoSizeCheckBox.Checked); };

            autoSizeTextBox0.BackColor = controlBackColor;
            autoSizeTextBox0.ForeColor = ForeColor;
            autoSizeTextBox0.Location = new Point(autoSizeCheckBox.Location.X + autoSizeCheckBox.Size.Width + 3, autoSizeCheckBox.Location.Y - (autoSizeTextBox0.Size.Height - autoSizeTextBox1.Size.Height) / 2);

            autoSizeTextBox1.BackColor = BackColor;
            autoSizeTextBox1.ForeColor = ForeColor;
            autoSizeTextBox1.Location = new Point(autoSizeTextBox0.Location.X + autoSizeTextBox0.Size.Width + 3, autoSizeTextBox0.Location.Y + (autoSizeTextBox0.Size.Height - autoSizeTextBox1.Size.Height) / 2);

            buyButton.BackColor = Color.FromArgb(255, 14, 203, 129);
            buyButton.ForeColor = ForeColor;
            buyButton.Size = new Size((orderGroupBox.Size.Width - 5 * 3) / 2, orderGroupBox.Size.Height / 5 - 5);
            buyButton.Location = new Point(5, orderGroupBox.Size.Height - 5 - buyButton.Size.Height);
            buyButton.Click += (sender, e) => { PlaceOrder(OrderSide.Buy, false); };

            sellButton.BackColor = Color.FromArgb(255, 207, 48, 74);
            sellButton.ForeColor = ForeColor;
            sellButton.Size = buyButton.Size;
            sellButton.Location = new Point(buyButton.Location.X + buyButton.Size.Width + 5, buyButton.Location.Y);
            sellButton.Click += (sender, e) => { PlaceOrder(OrderSide.Sell, false); };

            ROCheckBox.BackColor = BackColor;
            ROCheckBox.ForeColor = ForeColor;
            ROCheckBox.Location = new Point(sellButton.Location.X + sellButton.Size.Width - ROCheckBox.Size.Width, sellButton.Location.Y - ROCheckBox.Size.Height - 3);

            timeDiffTextBox.BackColor = BackColor;
            timeDiffTextBox.ForeColor = ForeColor;
            timeDiffTextBox.Location = new Point(orderGroupBox.Location.X + 50, orderGroupBox.Location.Y - 10);
            timeDiffTextBox.Size = new Size(30, 15);
            timeDiffTextBox.BringToFront();

            gridItvTextBox.BackColor = BackColor;
            gridItvTextBox.ForeColor = ForeColor;
            gridItvTextBox.Location = new Point(timeDiffTextBox.Location.X + timeDiffTextBox.Size.Width, timeDiffTextBox.Location.Y);
            gridItvTextBox.BringToFront();

            autoTextBox.BackColor = BackColor;
            autoTextBox.ForeColor = ForeColor;
            autoTextBox.Location = new Point(gridItvTextBox.Location.X + gridItvTextBox.Size.Width, gridItvTextBox.Location.Y);
            autoTextBox.BringToFront();
        }
        void SetResultView()
        {
            mainResultGroupBox.BackColor = BackColor;
            mainResultGroupBox.ForeColor = ForeColor;
            mainResultGroupBox.Size = new Size((Screen.GetWorkingArea(this).Size.Width - orderGroupBox.Location.X - orderGroupBox.Size.Width) / 2 - 15, orderGroupBox.Size.Height);
            mainResultGroupBox.Location = new Point(orderGroupBox.Location.X + orderGroupBox.Size.Width + 10, orderGroupBox.Location.Y);

            totalWinRateTextBox.BackColor = BackColor;
            totalWinRateTextBox.ForeColor = ForeColor;
            totalWinRateTextBox.Location = new Point(10, 10);
            totalWinRateTextBox.BringToFront();

            todayWinRateTextBox.BackColor = BackColor;
            todayWinRateTextBox.ForeColor = ForeColor;
            todayWinRateTextBox.Location = new Point(totalWinRateTextBox.Location.X + totalWinRateTextBox.Width + 10, totalWinRateTextBox.Location.Y);
            todayWinRateTextBox.BringToFront();

            resultListView.BackColor = controlBackColor;
            resultListView.ForeColor = ForeColor;
            resultListView.Size = new Size(mainResultGroupBox.Width - 10, mainResultGroupBox.Height - totalWinRateTextBox.Height - totalWinRateTextBox.Location.Y);
            resultListView.Location = new Point(5, totalWinRateTextBox.Height + totalWinRateTextBox.Location.Y);
            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var column0 = new OLVColumn("EG(%), TD(s), SA", "EntryGapAndTimeAndSuccessAmount");
            column0.FreeSpaceProportion = 3;
            column0.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(column0);
            var column1 = new OLVColumn("LG(%), TD(s), SA", "LastGapAndTimeAndSuccessAmount");
            column1.FreeSpaceProportion = 3;
            column1.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(column1);
            var column2 = new OLVColumn("PR(%), RP($)", "ProfitRateAndValue");
            column2.FreeSpaceProportion = 3;
            column2.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(column2);
            resultListView.Columns.AddRange(new ColumnHeader[] { column0, column1, column2 });
            resultListView.FormatRow += (sender, e) =>
            {
                if (((ResultData)e.Model).ProfitRate > commisionRate)
                    e.Item.ForeColor = Color.Gold;
            };

            pictureBox.BackColor = Color.Transparent;
            if (resultListView.Size.Width / 4 * 3 > resultListView.Size.Height)
            {
                pictureBox.Size = new Size((resultListView.Size.Height - 10) / 3 * 4, resultListView.Size.Height - 10);
                pictureBox.Location = new Point(resultListView.Location.X + (resultListView.Size.Width - pictureBox.Size.Width) / 2, resultListView.Location.Y + 5);
            }
            else
            {
                pictureBox.Size = new Size(resultListView.Size.Width - 10, (resultListView.Size.Width - 10) / 4 * 3);
                pictureBox.Location = new Point(resultListView.Location.X + 5, resultListView.Location.Y + (resultListView.Size.Height - pictureBox.Size.Height) / 2);
            }
            pictureBox.Enabled = false;
            pictureBox.Paint += (sender, e) =>
            {
                currentFrame++;
                if (currentFrame == numberOfFrames)
                {
                    pictureBox.Visible = false;
                    pictureBox.Enabled = false;
                }
            };


            simulResultGroupBox.BackColor = BackColor;
            simulResultGroupBox.ForeColor = ForeColor;
            simulResultGroupBox.Size = new Size(mainResultGroupBox.Width, mainResultGroupBox.Height);
            simulResultGroupBox.Location = new Point(mainResultGroupBox.Location.X + mainResultGroupBox.Size.Width + 10, mainResultGroupBox.Location.Y);

            simulTotalWinRateTextBox.BackColor = BackColor;
            simulTotalWinRateTextBox.ForeColor = ForeColor;
            simulTotalWinRateTextBox.Location = new Point(10, 10);
            simulTotalWinRateTextBox.BringToFront();

            simulTodayWinRateTextBox.BackColor = BackColor;
            simulTodayWinRateTextBox.ForeColor = ForeColor;
            simulTodayWinRateTextBox.Location = new Point(simulTotalWinRateTextBox.Location.X + simulTotalWinRateTextBox.Width + 10, simulTotalWinRateTextBox.Location.Y);
            simulTodayWinRateTextBox.BringToFront();

            simulResultListView.BackColor = controlBackColor;
            simulResultListView.ForeColor = ForeColor;
            simulResultListView.Size = new Size(simulResultGroupBox.Width - 10, simulResultGroupBox.Height - simulTotalWinRateTextBox.Height - simulTotalWinRateTextBox.Location.Y);
            simulResultListView.Location = new Point(5, simulTotalWinRateTextBox.Height + simulTotalWinRateTextBox.Location.Y);
            column0 = new OLVColumn("Name", "Name");
            column0.FreeSpaceProportion = 3;
            column0.HeaderFormatStyle = headerstyle;
            simulResultListView.AllColumns.Add(column0);
            column1 = new OLVColumn("Time", "Time");
            column1.FreeSpaceProportion = 3;
            column1.HeaderFormatStyle = headerstyle;
            simulResultListView.AllColumns.Add(column1);
            column2 = new OLVColumn("PR(%)", "ProfitRate");
            column2.FreeSpaceProportion = 3;
            column2.HeaderFormatStyle = headerstyle;
            simulResultListView.AllColumns.Add(column2);
            simulResultListView.Columns.AddRange(new ColumnHeader[] { column0, column1, column2 });
            simulResultListView.FormatRow += (sender, e) =>
            {
                if (((SimulResultData)e.Model).ProfitRate > commisionRate)
                    e.Item.ForeColor = Color.Gold;
            };
        }
        void SetSymbolsListView()
        {
            FUGroupBox.BackColor = BackColor;
            FUGroupBox.ForeColor = ForeColor;
            FUGroupBox.Location = new Point(secChart.Location.X + secChart.Size.Width, secChart.Location.Y + 10);
            FUGroupBox.Size = new Size(hoChart.Width, secChart.Size.Height - hoChart.Height - 20);

            FUListView.BackColor = controlBackColor;
            FUListView.ForeColor = ForeColor;
            FUListView.Location = new Point(3, 15);
            FUListView.Size = new Size(FUGroupBox.Size.Width - 6, FUGroupBox.Size.Height - FUListView.Location.Y - 30);
            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var nameColumnSize = 6;
            var nameColumn = new OLVColumn("Name", "Name");
            nameColumn.FreeSpaceProportion = nameColumnSize;
            nameColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(nameColumn);
            var impColumnSize = 2;
            var impColumn = new OLVColumn("R", "Real");
            impColumn.FreeSpaceProportion = impColumnSize;
            impColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(impColumn);
            var flucColumnSize = 3;
            var flucColumn = new OLVColumn("SDP", "minLowestSDevRatioPrice");
            flucColumn.FreeSpaceProportion = flucColumnSize;
            flucColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(flucColumn);
            var countColumnSize = 3;
            var countColumn = new OLVColumn("SD", "minLowestSDevRatio");
            countColumn.FreeSpaceProportion = countColumnSize;
            countColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(countColumn);
            var durColumnSize = 3;
            var durColumn = new OLVColumn("Count", "minLowestSDevCount10sec");
            durColumn.FreeSpaceProportion = durColumnSize;
            durColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(durColumn);
            FUListView.Columns.AddRange(new ColumnHeader[] { nameColumn, impColumn, flucColumn, countColumn, durColumn });
            FUListView.SelectionChanged += (sender, e) => { if (FUListView.SelectedIndices.Count == 1) ShowChart(FUListView.SelectedObject as ItemData); };

            FUKlineRcvTextBox.BackColor = BackColor;
            FUKlineRcvTextBox.ForeColor = ForeColor;
            FUKlineRcvTextBox.Size = new Size(25, 14);
            FUKlineRcvTextBox.Location = new Point(FUListView.Location.X + 10, FUGroupBox.Size.Height - FUKlineRcvTextBox.Height - 10);

            FUKlineReqTextBox.BackColor = BackColor;
            FUKlineReqTextBox.ForeColor = ForeColor;
            FUKlineReqTextBox.Size = new Size(47, FUKlineRcvTextBox.Height);
            FUKlineReqTextBox.Location = new Point(FUKlineRcvTextBox.Location.X + FUKlineRcvTextBox.Size.Width, FUKlineRcvTextBox.Location.Y);

            FUAggRcvTextBox.BackColor = BackColor;
            FUAggRcvTextBox.ForeColor = ForeColor;
            FUAggRcvTextBox.Size = FUKlineRcvTextBox.Size;
            FUAggRcvTextBox.Location = new Point(FUKlineReqTextBox.Location.X + FUKlineReqTextBox.Size.Width + FUKlineRcvTextBox.Size.Width, FUKlineReqTextBox.Location.Y);

            FUAggReqTextBox.BackColor = BackColor;
            FUAggReqTextBox.ForeColor = ForeColor;
            FUAggReqTextBox.Size = FUKlineReqTextBox.Size;
            FUAggReqTextBox.Location = new Point(FUAggRcvTextBox.Location.X + FUAggRcvTextBox.Size.Width, FUAggRcvTextBox.Location.Y);

            realButton.BackColor = buttonColor;
            realButton.ForeColor = ForeColor;
            realButton.Location = new Point(FUAggReqTextBox.Location.X + FUAggReqTextBox.Width, FUAggReqTextBox.Location.Y + 2);
            realButton.Click += (sebder, e) => {
                var itemData = FUListView.SelectedObject as ItemData;

                if (itemData.Real >= 1)
                    SetAgg(itemData, false);
                else
                    SetAgg(itemData, true);
            };
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

        void LoadResultEffect(decimal profit)
        {
            if (profit <= commisionRate)
            {
                pictureBox.Image = fail;
                soundEngine.Play2D(failSound, false, false, false);
            }
            else if (profit <= commisionRate + 0.3m)
            {
                pictureBox.Image = win0;
                soundEngine.Play2D(win0Sound, false, false, false);
            }
            else if (profit <= commisionRate + 0.6m)
            {
                pictureBox.Image = win1;
                soundEngine.Play2D(win1Sound, false, false, false);
            }
            else if (profit <= commisionRate + 0.9m)
            {
                pictureBox.Image = win2;
                soundEngine.Play2D(win2Sound, false, false, false);
            }
            else
            {
                pictureBox.Image = win3;
                soundEngine.Play2D(win3Sound, false, false, false);
            }

            var dimension = new FrameDimension(pictureBox.Image.FrameDimensionsList[0]);
            currentFrame = 0;
            numberOfFrames = pictureBox.Image.GetFrameCount(dimension);
            pictureBox.Image.SelectActiveFrame(dimension, 0);
            pictureBox.Visible = true;
            pictureBox.Enabled = true;
        }

        void PlaceOrder(OrderSide orderSide, bool market)
        {
            if (!decimal.TryParse(orderPriceTextBox1.Text, out decimal priceRate) 
                || !decimal.TryParse(orderSizeTextBox1.Text, out decimal size) || size <= 0 
                || !int.TryParse(autoSizeTextBox0.Text, out int limitPercent) || limitPercent <= 0)
            {
                MessageBox.Show("input error");
                orderPriceTextBox1.Clear();
                return;
            }

            itemDataShowing.orderStartClosePrice = itemDataShowing.minStick.Price[3];

            var orderType = OrderType.Limit;
            TimeInForce? timeInForce = null;
            decimal? price = null;
            decimal quantity;

            if (!marketRadioButton.Checked && !market)
            {
                if (orderSide == OrderSide.Buy)
                    price = (int)(itemDataShowing.orderStartClosePrice * (1 + priceRate / 100) / itemDataShowing.priceTickSize) * itemDataShowing.priceTickSize;
                else
                    price = (int)(itemDataShowing.orderStartClosePrice * (1 - priceRate / 100) / itemDataShowing.priceTickSize) * itemDataShowing.priceTickSize;

                if (PORadioButton.Checked)
                    timeInForce = TimeInForce.GoodTillCrossing;
                else if (IOCRadioButton.Checked)
                    timeInForce = TimeInForce.ImmediateOrCancel;
                else
                    timeInForce = TimeInForce.GoodTillCancel;
            }
            else
                orderType = OrderType.Market;

            if (!miniSizeCheckBox.Checked && !autoSizeCheckBox.Checked)
                quantity = (int)(size / itemDataShowing.minSize) * itemDataShowing.minSize;
            else
            {
                if (itemDataShowing.position)
                    quantity = Math.Abs(itemDataShowing.Size);
                else if (miniSizeCheckBox.Checked)
                    quantity = (int)(itemDataShowing.minNotionalValue / price / itemDataShowing.minSize + 1) * itemDataShowing.minSize;
                else
                {
                    quantity = (int)((decimal)(itemDataShowing.ms10secAvg + itemDataShowing.md10secAvg) / 2 / 100 / itemDataShowing.minSize) * itemDataShowing.minSize;

                    var budget = assetDic["Available Balance"].Amount * limitPercent / 100;
                    if (budget < itemDataShowing.minNotionalValue)
                        budget = itemDataShowing.minNotionalValue;
                    var limitAmount = (int)(budget / price / itemDataShowing.minSize + 1) * itemDataShowing.minSize;

                    if (limitAmount < quantity)
                        quantity = limitAmount;
                }
            }

            if (!itemDataShowing.position)
            {
                if (price * quantity > itemDataShowing.maxNotionalValue)
                {
                    MessageBox.Show("lower leverage");
                    return;
                }
                if (price * quantity < itemDataShowing.minNotionalValue)
                {
                    MessageBox.Show("too small");
                    return;
                }
            }

            orderSizeTextBox1.Text = quantity.ToString();
            var a = client.FuturesUsdt.Order.PlaceOrder(
                itemDataShowing.Name
                , orderSide
                , orderType
                , quantity
                , PositionSide.Both
                , timeInForce
                , ROCheckBox.Checked
                , price);
            itemDataShowing.positionWhenOrder = itemDataShowing.position;

            if (!a.Success)
            {
                MessageBox.Show("order fail" + a.Error);
                return;
            }

            var b = client.FuturesUsdt.Order.CancelAllOrdersAfterTimeout(itemDataShowing.Name, TimeSpan.FromSeconds(2));

            if (!b.Success)
            {
                MessageBox.Show("cancel timer order fail" + b.Error);
                return;
            }

            if (itemDataShowing.position)
            {
                ResultData resultData;

                if (resultListView.Items.Count == 0)
                {
                    resultData = new ResultData { Symbol = itemDataShowing.Name };
                    resultListView.InsertObjects(0, new List<ResultData> { resultData });
                }
                else
                    resultData = resultListView.GetModelObject(0) as ResultData;

                resultData.LastTime = DateTime.UtcNow;
                if (!market)
                    resultData.LastGapAndTimeAndSuccessAmount = "1";
            }
            else
            {
                if (orderSide == OrderSide.Buy)
                    itemDataShowing.LorS = true;
                else
                    itemDataShowing.LorS = false;

                var resultData = new ResultData { Symbol = itemDataShowing.Name };
                resultData.EntryTime = DateTime.UtcNow;
                resultListView.InsertObjects(0, new List<ResultData> { resultData });

                itemDataShowing.Size = 0;
            }
        }

        void ResetOrderView()
        {
            leverageTextBox0.Text = itemDataShowing.Leverage.ToString();

            TickAutoSizeButton(true);

            if (itemDataShowing.position)
            {
                TickMinSizeButton(false);
                if (positionExitPriceMarket)
                    marketRadioButton.Checked = true;
                else
                    GTCRadioButton.Checked = true;
                ROCheckBox.Checked = true;
            }
            else
            {
                TickMinSizeButton(miniSizeDefault);
                GTCRadioButton.Checked = true;
                ROCheckBox.Checked = false;
            }

            orderPriceTextBox1.Text = "0.0";
        }
        void TickMinSizeButton(bool on)
        {
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
        
        void ShowChart(ItemData itemData)
        {
            if (itemData.isChartShowing)
                return;

            if (itemDataShowing != null && itemDataShowing.isChartShowing)
            {
                itemDataShowing.isChartShowing = false;
                socketClient.Unsubscribe(itemData.hoSub);
            }
            itemDataShowing = itemData;

            Text = itemData.Name + "-Future(Usdt)      Binance       UTC-" + startTime;

            foreach (var se in secChart.Series)
                se.Points.Clear();
            secChart.ChartAreas[1].AxisY2.StripLines.Clear();
            
            if (itemData.Real >= 1)
            {
                for (int i = 0; i < itemData.secStickList.Count; i++)
                    AddFullChartPoint(secChart, itemData.secStickList[i]);
                if (itemData.secStick.Time != default)
                    AddFullChartPoint(secChart, itemData.secStick);

                if (itemData.Real == 2)
                {
                    var strip = new StripLine();
                    strip.ForeColor = ForeColor;
                    strip.TextLineAlignment = StringAlignment.Center;
                    strip.TextAlignment = StringAlignment.Center;
                    strip.StripWidth = itemData.real2SmallAmt10secAvg * 2;
                    strip.BackColor = realSmallAmtColor;
                    strip.Text = strip.StripWidth.ToString();
                    strip.IntervalOffset = -strip.StripWidth / 2;
                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);

                    strip = new StripLine();
                    strip.ForeColor = ForeColor;
                    strip.TextLineAlignment = StringAlignment.Center;
                    strip.TextAlignment = StringAlignment.Center;
                    strip.StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                    strip.BackColor = realBigAmtColor;
                    strip.Text = strip.StripWidth.ToString();
                    strip.IntervalOffset = itemData.real2SmallAmt10secAvg;
                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);

                    strip = new StripLine();
                    strip.ForeColor = ForeColor;
                    strip.TextLineAlignment = StringAlignment.Center;
                    strip.TextAlignment = StringAlignment.Center;
                    strip.StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                    strip.BackColor = realBigAmtColor;
                    strip.Text = strip.StripWidth.ToString();
                    strip.IntervalOffset = -itemData.real2BigAmt10secAvg;
                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);
                }
            }

            if (itemData.Leverage == 0)
            {
                foreach (var data in client.FuturesUsdt.GetPositionInformation(itemData.Name).Data)
                {
                    if (data.Symbol != itemData.Name)
                        break;

                    itemData.Leverage = data.Leverage;
                    itemData.maxNotionalValue = data.MaxNotionalValue;

                    foreach (var brackets in client.FuturesUsdt.GetBrackets(itemData.Name).Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;
                    leverageTextBox1.Text = "/ " + itemData.maxLeverage.ToString();
                }
            }
            
            SetChartNowOrLoad(chartNow);

            ResetOrderView();

            itemData.isChartShowing = true;
            //100, 250, 500
            itemData.hoSub = socketClient.FuturesUsdt.SubscribeToPartialOrderBookUpdates(itemData.Name, (int)hoChart.Tag, 500, data2 => { BeginInvoke(hoUpdates, data2, FUItemDataList[data2.Symbol.ToUpper()]); }).Data;
        }
        void ChartScroll(Chart chart, ScrollType scrollType)
        {
            if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
            {
                if (scrollType == ScrollType.SmallDecrement && chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum == 0)
                    LoadMore(chart, 300, false);

                chart.ChartAreas[0].AxisX.ScaleView.Scroll(scrollType);
                AdjustChart(chart);
            }
        }
        void SetChartNowOrLoad(Chart chart)
        {
            if (itemDataShowing == null)
                return;

            secChart.Visible = false;
            minChart.Visible = false;
            hourChart.Visible = false;
            dayChart.Visible = false;
            secButton.BackColor = buttonColor;
            minButton.BackColor = buttonColor;
            hourButton.BackColor = buttonColor;
            dayButton.BackColor = buttonColor;

            chart.Visible = true;

            gridItvTextBox.Text = ((double)chart.Tag * 100) + "%";

            if (chart.TabIndex == minChart.TabIndex)
            {
                LoadMore(minChart, 300, true);
                minButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == hourChart.TabIndex)
            {
                LoadMore(hourChart, 300, true);
                hourButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == dayChart.TabIndex)
            {
                LoadMore(dayChart, 300, true);
                dayButton.BackColor = buttonSelectedColor;
            }
            else
            {
                if (!chart.ChartAreas[0].AxisX.ScaleView.IsZoomed && chart.Series[0].Points.Count > baseChartViewSticksSize)
                {
                    chart.ChartAreas[0].AxisX.ScaleView.Zoom(chart.Series[0].Points.Count - baseChartViewSticksSize, chart.Series[0].Points.Count);
                    chart.ChartAreas[0].RecalculateAxesScale();
                    chart.ChartAreas[1].RecalculateAxesScale();
                    AdjustChart(chart);
                }

                secButton.BackColor = buttonSelectedColor;
            }

            chartNow = chart;
        }
        void LoadMore(Chart chart, int limit, bool loadNew)
        {
            if (chart.TabIndex == secChart.TabIndex && itemDataShowing.oldSecStickList.Count == 0)
                return;

            KlineInterval interval = KlineInterval.OneMinute;
            ref List<Stick> list = ref itemDataShowing.secStickList;
            ref Stick stick = ref itemDataShowing.secStick;

            if (chart.TabIndex == minChart.TabIndex)
            {
                list = ref itemDataShowing.minStickList;
                stick = ref itemDataShowing.minStick;
            }
            else if (chart.TabIndex == hourChart.TabIndex)
            {
                interval = KlineInterval.OneHour;
                list = ref itemDataShowing.hourStickList;
                stick = ref itemDataShowing.hourStick;
            }
            else if (chart.TabIndex == dayChart.TabIndex)
            {
                interval = KlineInterval.OneDay;
                list = ref itemDataShowing.dayStickList;
                stick = ref itemDataShowing.dayStick;
            }

            var start = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            if (loadNew)
            {
                foreach (var se in chart.Series)
                    se.Points.Clear();
                list.Clear();

                start = -baseChartViewSticksSize + 2;
                end = 0;
            }

            DateTime? endTime = null;
            if (loadNew)
                endTime = DateTime.UtcNow;
            else if (chart.TabIndex != secChart.TabIndex)
            {
                endTime = list[0].Time;
                list.RemoveAt(0);

                foreach (var se in chart.Series)
                    se.Points.RemoveAt(0);
            }

            if (limit > 0)
            {
                if (chart.TabIndex != secChart.TabIndex)
                {
                    var klines = client.FuturesUsdt.Market.GetKlines(itemDataShowing.Name, interval, null, endTime, limit).Data;
                    Stick newStick;

                    var first = true;
                    limit = 0;
                    foreach (var kline in klines.Reverse())
                    {
                        limit++;

                        newStick = new Stick();
                        newStick.Price[0] = kline.High;
                        newStick.Price[1] = kline.Low;
                        newStick.Price[2] = kline.Open;
                        newStick.Price[3] = kline.Close;
                        newStick.Ms = kline.TakerBuyBaseVolume;
                        newStick.Md = kline.BaseVolume - kline.TakerBuyBaseVolume;
                        newStick.Time = kline.OpenTime;
                        newStick.TCount = kline.TradeCount;
                        InsertFullChartPoint(0, chart, newStick);
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
                        InsertFullChartPoint(0, chart, itemDataShowing.oldSecStickList[i]);
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
            chart.ChartAreas[1].RecalculateAxesScale();
            AdjustChart(chart);
        }
        void AdjustChart(Chart chart)
        {
            var priceLow = double.MaxValue;
            var priceHigh = double.MinValue;
            var msHigh = double.MinValue;
            var mdLow = double.MaxValue;

            for (int i = (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum - 1; i < (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum; i++)
            {
                if (i >= chart.Series[0].Points.Count || i < 0)
                    continue;

                if (chart.Series[0].Points[i].YValues[1] < priceLow)
                    priceLow = chart.Series[0].Points[i].YValues[1];

                if (chart.Series[0].Points[i].YValues[0] > priceHigh)
                    priceHigh = chart.Series[0].Points[i].YValues[0];

                if (chart.Series[3].Points[i].YValues[0] + chart.Series[4].Points[i].YValues[0] > msHigh)
                    msHigh = chart.Series[3].Points[i].YValues[0] + chart.Series[4].Points[i].YValues[0];
                if (chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0] < mdLow)
                    mdLow = chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0];
            }

            if (priceLow == double.MaxValue || priceHigh == double.MinValue || msHigh == double.MinValue || mdLow == double.MinValue)
                return;

            chart.ChartAreas[0].AxisY2.ScaleView.Zoom(priceLow, priceHigh);
            chart.ChartAreas[1].AxisY2.ScaleView.Zoom(mdLow + mdLow / 10, msHigh);

            chart.ChartAreas[0].AxisY2.MajorGrid.Interval = (priceLow + priceHigh) / 2 * (double)chart.Tag;

            chart.ChartAreas[0].RecalculateAxesScale();

            if (itemDataShowing.position && chart.ChartAreas[0].AxisY2.StripLines.Count == 1)
            {
                if (itemDataShowing.EntryPrice > itemDataShowing.minStick.Price[3])
                    chart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemDataShowing.minStick.Price[3] - chart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                else
                    chart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemDataShowing.EntryPrice - chart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
            }
        }
        void AddFullChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { (double)stick.Price[0], (double)stick.Price[1], (double)stick.Price[2], (double)stick.Price[3] });
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
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, -stick.Md);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
                chart.Series[3].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Md);
                chart.Series[4].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Ms - stick.Md);
            }
            else
            {
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, -stick.Ms);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Ms - stick.Md);
                chart.Series[3].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Ms);
                chart.Series[4].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            }
        }
        void InsertFullChartPoint(int index, Chart chart, Stick stick)
        {
            chart.Series[0].Points.InsertXY(index, stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { (double)stick.Price[0], (double)stick.Price[1], (double)stick.Price[2], (double)stick.Price[3] });
            if (stick.Price[2] > stick.Price[3] || (stick.Price[2] == stick.Price[3] && stick.Price[2] - stick.Price[1] >= stick.Price[0] - stick.Price[3]))
            {
                chart.Series[0].Points[index].Color = mnsPrcColor;
                chart.Series[0].Points[index].BackSecondaryColor = mnsPrcColor;
            }
            else
            {
                chart.Series[0].Points[index].Color = plsPrcColor;
                chart.Series[0].Points[index].BackSecondaryColor = plsPrcColor;
            }


            if (stick.Ms > stick.Md)
            {
                chart.Series[1].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, -stick.Md);
                chart.Series[2].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, 0);
                chart.Series[3].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Md);
                chart.Series[4].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Ms - stick.Md);
            }
            else
            {
                chart.Series[1].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, -stick.Ms);
                chart.Series[2].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Ms - stick.Md);
                chart.Series[3].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Ms);
                chart.Series[4].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, 0);
            }
        }
        void AddStartChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { (double)stick.Price[2], (double)stick.Price[2], (double)stick.Price[2], (double)stick.Price[2] });
            chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            chart.Series[3].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            chart.Series[4].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);

            if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed)
            {
                if ((int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= chart.Series[0].Points.Count)
                {
                    chart.ChartAreas[0].RecalculateAxesScale();
                    chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallIncrement);
                }
                else if (chart.TabIndex == secChart.TabIndex && chart.Series[0].Points.Count >= maxSecListCount)
                {
                    chart.ChartAreas[0].RecalculateAxesScale();
                    chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallDecrement);
                }
            }

            AdjustChart(chart);
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
                chart.Series[1].Points.Last().YValues[0] = (double)-stick.Md;
                chart.Series[2].Points.Last().YValues[0] = 0;
                chart.Series[3].Points.Last().YValues[0] = (double)stick.Md;
                chart.Series[4].Points.Last().YValues[0] = (double)(stick.Ms - stick.Md);
            }
            else
            {
                chart.Series[1].Points.Last().YValues[0] = (double)-stick.Ms;
                chart.Series[2].Points.Last().YValues[0] = (double)(stick.Ms - stick.Md);
                chart.Series[3].Points.Last().YValues[0] = (double)stick.Ms;
                chart.Series[4].Points.Last().YValues[0] = 0;
            }
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

            socketClient.FuturesUsdt.SubscribeToKlineUpdates(FUSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, FUItemDataList[data.Symbol]); });
            FUKlineRcvTextBox.Text = "0";
            FUKlineReqTextBox.Text = "/" + FUSymbolList.Count + "(K)";
            FUAggRcvTextBox.Text = "0";
            FUAggReqTextBox.Text = "/0(A)";
        }
        void SetItemDataList()
        {
            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
                if (s.Name == "BTCSTUSDT")
                    continue;

                var itemData = new ItemData(s);
                FUItemDataList.Add(itemData.Name, itemData);
                FUSymbolList.Add(itemData.Name);
                FUListView.AddObject(itemData);

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
                        data => { BeginInvoke(accountUpdates, data);},
                        data => { BeginInvoke(orderUpdates, data, FUItemDataList[data.UpdateData.Symbol]); },
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
                        data => { BeginInvoke(orderUpdates, data, FUItemDataList[data.UpdateData.Symbol]); },
                        data => {
                            var a = data;
                        });
        }
        void GetAccountInfo()
        {
            assetDic.Add("Margin Ratio", new Asset { AssetName = "Margin Ratio" });
            assetsListView.AddObject(assetDic["Margin Ratio"]);
            assetDic.Add("Maintenance Margin", new Asset { AssetName = "Maintenance Margin" });
            assetsListView.AddObject(assetDic["Maintenance Margin"]);
            assetDic.Add("Margin Balance", new Asset { AssetName = "Margin Balance" });
            assetsListView.AddObject(assetDic["Margin Balance"]);
            assetDic.Add("Available Balance", new Asset { AssetName = "Available Balance" });
            assetsListView.AddObject(assetDic["Available Balance"]);
            assetDic.Add("Wallet Balance", new Asset { AssetName = "Wallet Balance" });
            assetsListView.AddObject(assetDic["Wallet Balance"]);

            var ga = client.FuturesUsdt.Account.GetAccountInfo();
            foreach (var s in ga.Data.Assets)
            {
                if (s.Asset == "USDT")
                {
                    assetDic["Margin Balance"].Amount = s.MarginBalance;
                    assetDic["Available Balance"].Amount = s.AvailableBalance;
                    assetDic["Wallet Balance"].Amount = s.WalletBalance;
                }
            }

            #region GetTradeHistory
            var conn = new SQLiteConnection(DBHelper.DBHistoryPath);
            conn.Open();

            var command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS 'Trade_History' " +
                "('Time' TEXT, 'Symbol' TEXT, 'Profit' TEXT)", conn);
            command.ExecuteNonQuery();
            
            command = new SQLiteCommand("SELECT * FROM 'Trade_History'", conn);
            var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                totalTrades++;
                var rate = decimal.Parse(reader["Profit"].ToString());
                totalRateSum += rate;
                if (rate > commisionRate)
                    totalWin++;
            }

            if (totalTrades != 0)
                totalWinRateTextBox.Text = Math.Round((double)totalWin / totalTrades, 2) + "(" + totalTrades + ") " + Math.Round((double)totalRateSum / totalTrades, 2);

            command = new SQLiteCommand("CREATE TABLE IF NOT EXISTS 'Simulation_Trade_History' " +
                "('Time' TEXT, 'Symbol' TEXT, 'Profit' TEXT)", conn);
            command.ExecuteNonQuery();
            command = new SQLiteCommand("SELECT * FROM 'Simulation_Trade_History'", conn);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                simulTotalTrades++;
                var rate = decimal.Parse(reader["Profit"].ToString());
                simulTotalRateSum += rate;
                if (rate > commisionRate)
                    simulTotalWin++;
            }

            if (simulTotalTrades != 0)
                simulTotalWinRateTextBox.Text = Math.Round((double)simulTotalWin / simulTotalTrades, 2) + "(" + simulTotalTrades + ") " + Math.Round((double)simulTotalRateSum / simulTotalTrades, 2);

            conn.Close();
            #endregion

            dbHelper = new DBHelper();
        }

        void OnKlineUpdates(IBinanceStreamKlineData data, ItemData itemData)
        {
            if (itemData.klineFirst)
            {
                itemData.klineFirst = false;

                FUKlineRcv++;
                FUKlineRcvTextBox.Text = FUKlineRcv.ToString();
            }

            if (data.Data.Final)
            {
                if (data.Data.High / data.Data.Low > 1.02m)
                {
                    if (itemData.Real == 0)
                    {
                        SetAgg(itemData, true);
                        dbHelper.SaveData1("Detect_History", "Time", DateTime.UtcNow.ToString(secTimeFormat), "Symbol", itemData.Name, "Data", "Real 1 in : 전분폭 > 2%");
                    }

                    itemData.Real1Condition = 0;
                }
                else if (itemData.Real1Condition < 15)
                    itemData.Real1Condition++;
                else if (itemData.Real == 1 && !itemData.isChartShowing)
                {
                    SetAgg(itemData, false);
                    dbHelper.SaveData1("Detect_History", "Time", DateTime.UtcNow.ToString(secTimeFormat), "Symbol", itemData.Name, "Data", "Real out : 15연속 전분폭 < 2%");
                }

                if (itemData.Real == 2)
                {
                    if (itemData.Real2Condition > 5)
                    {
                        itemData.Real = 1;
                        FUListView.RefreshObject(itemData);
                        dbHelper.SaveData1("Detect_History", "Time", DateTime.UtcNow.ToString(secTimeFormat), "Symbol", itemData.Name
                            , "Data", "Real 2 out : 5분연속 기준 미달");

                        List<Stick> secList = new List<Stick>();
                        secList.AddRange(itemData.oldSecStickList);
                        secList.AddRange(itemData.secStickList);
                        dbHelper.SaveSticksCSVData(itemData.Name + "_" + DateTime.UtcNow.ToString(secTimeFormatSCV), secList);
                    }
                    else
                        itemData.Real2Condition++;
                }

                if (itemData.isChartShowing && itemData.Real == 0 && chartNow.TabIndex == minChart.TabIndex)
                {
                    itemData.minStick = new Stick();
                    itemData.minStick.Price[0] = data.Data.High;
                    itemData.minStick.Price[1] = data.Data.Low;
                    itemData.minStick.Price[2] = data.Data.Open;
                    itemData.minStick.Price[3] = data.Data.Close;
                    itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                    itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                    itemData.minStick.Time = data.Data.OpenTime;
                    itemData.minStick.TCount = data.Data.TradeCount;
                    itemData.minStickList.Add(itemData.minStick);

                    UpdateChartPoint(minChart, itemData.minStick);
                }
            }
            else if (itemData.isChartShowing && itemData.Real == 0 && chartNow.TabIndex == minChart.TabIndex)
            {
                if (itemData.minStick.Time != data.Data.OpenTime)
                {
                    itemData.minStick = new Stick();
                    itemData.minStick.Price[2] = data.Data.Open;
                    itemData.minStick.Time = data.Data.OpenTime;
                    AddStartChartPoint(minChart, itemData.minStick);
                }   

                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Md = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;

                UpdateChartPoint(minChart, itemData.minStick);
            }
        }
        void OnAggregatedTradeUpdates(BinanceStreamAggregatedTrade data, ItemData itemData)
        {
            if (itemData.Real == 0)
                return;

            if (itemData.AggFirst)
            {
                itemData.AggFirst = false;

                FUAggRcv++;
                FUAggRcvTextBox.Text = FUAggRcv.ToString();
            }

            if (data.TradeTime.Subtract(itemData.secStick.Time).TotalSeconds >= 1)
            {
                if (itemData.secStick.Time != default)
                {
                    if (itemData.timeDiffMax != double.MinValue)
                        timeDiffTextBox.Text = Math.Round(itemData.timeDiffMax, 1).ToString();
                    itemData.timeDiffMax = double.MinValue;

                    itemData.secStickList.Add(itemData.secStick);

                    itemData.secLastIndex = itemData.secStickList.Count - 1;

                    if (itemData.secLastIndex > 20)
                    {
                        itemData.price10secHighest = itemData.secStickList[itemData.secLastIndex].Price[0];
                        itemData.price10secLowest = itemData.secStickList[itemData.secLastIndex].Price[1];

                        itemData.ms5secTot = itemData.ms5secTot + itemData.secStickList[itemData.secLastIndex].Ms - itemData.secStickList[itemData.secLastIndex - 5].Ms;
                        itemData.md5secTot = itemData.md5secTot + itemData.secStickList[itemData.secLastIndex].Md - itemData.secStickList[itemData.secLastIndex - 5].Md;
                        itemData.ms5secAvg = itemData.ms5secTot / 5;
                        itemData.md5secAvg = itemData.md5secTot / 5;

                        itemData.ms10secTot = itemData.ms10secTot + (double)(itemData.secStickList[itemData.secLastIndex].Ms - itemData.secStickList[itemData.secLastIndex - 10].Ms);
                        itemData.md10secTot = itemData.md10secTot + (double)(itemData.secStickList[itemData.secLastIndex].Md - itemData.secStickList[itemData.secLastIndex - 10].Md);
                        itemData.ms10secAvg = itemData.ms10secTot / 10;
                        itemData.md10secAvg = itemData.md10secTot / 10;

                        itemData.ms20secTot = itemData.ms20secTot + itemData.secStickList[itemData.secLastIndex].Ms - itemData.secStickList[itemData.secLastIndex - 20].Ms;
                        itemData.md20secTot = itemData.md20secTot + itemData.secStickList[itemData.secLastIndex].Md - itemData.secStickList[itemData.secLastIndex - 20].Md;

                        itemData.ms20secAvg = itemData.ms20secTot / 20;
                        itemData.md20secAvg = itemData.md20secTot / 20;

                        itemData.ms10secDev = Math.Pow(itemData.ms10secAvg - (double)itemData.secStickList[itemData.secLastIndex].Ms, 2);
                        itemData.md10secDev = Math.Pow(itemData.md10secAvg - (double)itemData.secStickList[itemData.secLastIndex].Md, 2);

                        itemData.count10sec = itemData.secStickList[itemData.secLastIndex].TCount;

                        for (itemData.index = itemData.secLastIndex - 9; itemData.index < itemData.secLastIndex; itemData.index++)
                        {
                            if (itemData.secStickList[itemData.index].Price[0] > itemData.price10secHighest)
                                itemData.price10secHighest = itemData.secStickList[itemData.index].Price[0];
                            if (itemData.secStickList[itemData.index].Price[1] < itemData.price10secLowest)
                                itemData.price10secLowest = itemData.secStickList[itemData.index].Price[1];

                            itemData.ms10secDev += Math.Pow(itemData.ms10secAvg - (double)itemData.secStickList[itemData.index].Ms, 2);
                            itemData.md10secDev += Math.Pow(itemData.md10secAvg - (double)itemData.secStickList[itemData.index].Md, 2);

                            itemData.count10sec += itemData.secStickList[itemData.index].TCount;
                        }

                        itemData.ms10secSDev = Math.Pow(itemData.ms10secDev, 0.5);
                        itemData.md10secSDev = Math.Pow(itemData.md10secDev, 0.5);

                        itemData.SDevRatioPrice = itemData.price10secHighest / itemData.price10secLowest;
                        itemData.msSDevRatio = itemData.ms10secSDev / itemData.ms10secAvg;
                        itemData.mdSDevRatio = itemData.md10secSDev / itemData.md10secAvg;

                        if (itemData.SDevRatioPrice > 1.007m
                            && (itemData.msSDevRatio < 2.0 || itemData.mdSDevRatio < 2.0)
                            && itemData.count10sec > 300)
                        {
                            if (itemData.Real != 2)
                            {
                                itemData.Real = 2;
                                Task.Run(() =>
                                {
                                    var ok = false;
                                    Task.Run(() =>
                                    {
                                        MessageBox.Show("Real 2 감지", "Real", MessageBoxButtons.OK);
                                        ok = true;
                                        return;
                                    });
                                    while (true)
                                    {
                                        if (ok)
                                            return;
                                        soundEngine.Play2D(failSound, false, false, false);
                                        Thread.Sleep((int)failSound.PlayLength + 500);
                                    }
                                });
                                FUListView.RefreshObject(itemData);
                                dbHelper.SaveData1("Detect_History", "Time", itemData.secStick.Time.ToString(secTimeFormat), "Symbol", itemData.Name
                                    , "Data", "Real 2 in : 전10초폭: " + Math.Round(itemData.SDevRatioPrice, 4)
                                    + " 표준편차비: 매수" + Math.Round(itemData.msSDevRatio, 2) + " 매도" + Math.Round(itemData.mdSDevRatio, 2) + " 체결수: " + itemData.count10sec);
                            }
                            itemData.Real2Condition = 0;
                            itemData.real2Ms10secAvg = itemData.ms10secAvg;
                            itemData.real2Md10secAvg = itemData.md10secAvg;
                            if (itemData.real2Ms10secAvg > itemData.real2Md10secAvg)
                            {
                                itemData.real2BigAmt10secAvg = itemData.real2Ms10secAvg;
                                itemData.real2SmallAmt10secAvg = itemData.real2Md10secAvg;
                            }
                            else
                            {
                                itemData.real2BigAmt10secAvg = itemData.real2Md10secAvg;
                                itemData.real2SmallAmt10secAvg = itemData.real2Ms10secAvg;
                            }
                            if (itemData.isChartShowing)
                            {
                                if (secChart.ChartAreas[1].AxisY2.StripLines.Count() == 0)
                                {
                                    var strip = new StripLine();
                                    strip.ForeColor = ForeColor;
                                    strip.TextLineAlignment = StringAlignment.Center;
                                    strip.TextAlignment = StringAlignment.Center;
                                    strip.StripWidth = itemData.real2SmallAmt10secAvg * 2;
                                    strip.BackColor = realSmallAmtColor;
                                    strip.Text = strip.StripWidth.ToString();
                                    strip.IntervalOffset = -itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);

                                    strip = new StripLine();
                                    strip.ForeColor = ForeColor;
                                    strip.TextLineAlignment = StringAlignment.Center;
                                    strip.TextAlignment = StringAlignment.Center;
                                    strip.StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                                    strip.BackColor = realBigAmtColor;
                                    strip.Text = strip.StripWidth.ToString();
                                    strip.IntervalOffset = itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);

                                    strip = new StripLine();
                                    strip.ForeColor = ForeColor;
                                    strip.TextLineAlignment = StringAlignment.Center;
                                    strip.TextAlignment = StringAlignment.Center;
                                    strip.StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                                    strip.BackColor = realBigAmtColor;
                                    strip.Text = strip.StripWidth.ToString();
                                    strip.IntervalOffset = -itemData.real2BigAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines.Add(strip);
                                }
                                else
                                {
                                    secChart.ChartAreas[1].AxisY2.StripLines[0].StripWidth = itemData.real2SmallAmt10secAvg * 2;
                                    secChart.ChartAreas[1].AxisY2.StripLines[0].IntervalOffset = -itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines[0].Text = secChart.ChartAreas[1].AxisY2.StripLines[0].StripWidth.ToString();

                                    secChart.ChartAreas[1].AxisY2.StripLines[1].StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines[1].IntervalOffset = itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines[1].Text = secChart.ChartAreas[1].AxisY2.StripLines[1].StripWidth.ToString();

                                    secChart.ChartAreas[1].AxisY2.StripLines[2].StripWidth = itemData.real2BigAmt10secAvg - itemData.real2SmallAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines[2].IntervalOffset = -itemData.real2BigAmt10secAvg;
                                    secChart.ChartAreas[1].AxisY2.StripLines[2].Text = secChart.ChartAreas[1].AxisY2.StripLines[2].StripWidth.ToString();
                                }
                            }
                        }

                        if (itemData.msSDevRatio < itemData.lowestSDevRatio || itemData.mdSDevRatio < itemData.lowestSDevRatio)
                        {
                            itemData.lowestSDevRatioPrice = itemData.SDevRatioPrice;
                            itemData.lowestSDevRatio = itemData.msSDevRatio;
                            if (itemData.mdSDevRatio < itemData.msSDevRatio)
                                itemData.lowestSDevRatio = itemData.mdSDevRatio;
                            itemData.lowestSDevCount10sec = itemData.count10sec;
                        }

                        if (itemData.isChartShowing)
                            autoTextBox.Text =
                                Math.Round((decimal)(itemData.ms10secAvg + itemData.md10secAvg) / 2 / 100 * itemData.price10secLowest, 0).ToString();
                    }
                    else
                    {
                        itemData.ms5secTot += itemData.secStickList[itemData.secLastIndex].Ms;
                        itemData.md5secTot += itemData.secStickList[itemData.secLastIndex].Md;

                        itemData.ms10secTot += (double)itemData.secStickList[itemData.secLastIndex].Ms;
                        itemData.md10secTot += (double)itemData.secStickList[itemData.secLastIndex].Md;

                        itemData.ms20secTot += itemData.secStickList[itemData.secLastIndex].Ms;
                        itemData.md20secTot += itemData.secStickList[itemData.secLastIndex].Md;

                        if (itemData.isChartShowing)
                            autoTextBox.Text = "0000";
                    }

                    if (itemData.secStickList[itemData.secLastIndex].Ms > itemData.secStickList[itemData.secLastIndex].Md)
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

                itemData.secStick = new Stick();

                itemData.secStick.Price[2] = data.Price;
                itemData.secStick.Price[0] = data.Price;
                itemData.secStick.Price[1] = data.Price;
                itemData.secStick.Price[3] = data.Price;

                itemData.secStick.Time = DateTime.Parse(data.TradeTime.ToString(secTimeFormat));

                if (itemData.isChartShowing)
                    AddStartChartPoint(secChart, itemData.secStick);

                if (itemData.secStickList.Count > maxSecListCount)
                {
                    itemData.oldSecStickList.Add(itemData.secStickList[0]);
                    itemData.secStickList.RemoveAt(0);
                    if (itemData.isChartShowing)
                    {
                        secChart.Series[0].Points.RemoveAt(0);
                        secChart.Series[1].Points.RemoveAt(0);
                        secChart.Series[2].Points.RemoveAt(0);
                        secChart.Series[3].Points.RemoveAt(0);
                        secChart.Series[4].Points.RemoveAt(0);
                    }
                }

                if (data.TradeTime.Subtract(itemData.minStick.Time).TotalMinutes >= 1)
                {
                    if (itemData.minStick.Time != default)
                    {
                        itemData.minStickList.Add(itemData.minStick);

                        if (itemData.lowestSDevRatioPrice != 0 && itemData.lowestSDevRatio != double.MaxValue)
                        {
                            itemData.minLowestSDevRatioPrice = Math.Round((itemData.lowestSDevRatioPrice - 1) * 100, 2);
                            itemData.minLowestSDevRatio = Math.Round(itemData.lowestSDevRatio, 2);
                            itemData.minLowestSDevCount10sec = itemData.lowestSDevCount10sec;
                        }
                        itemData.lowestSDevRatioPrice = itemData.SDevRatioPrice;
                        itemData.lowestSDevRatio = itemData.msSDevRatio;

                        FUListView.RefreshObject(itemData);
                    }

                    itemData.minStick = new Stick();

                    itemData.minStick.Price[2] = data.Price;
                    itemData.minStick.Price[0] = data.Price;
                    itemData.minStick.Price[1] = data.Price;
                    itemData.minStick.Price[3] = data.Price;

                    itemData.minStick.Time = DateTime.Parse(data.TradeTime.ToString(minTimeFormat));

                    if (itemData.isChartShowing && chartNow.TabIndex == minChart.TabIndex)
                        AddStartChartPoint(minChart, itemData.minStick);
                }
            }

            if (data.Price > itemData.secStick.Price[0])
                itemData.secStick.Price[0] = data.Price;
            else if (data.Price < itemData.secStick.Price[1])
                itemData.secStick.Price[1] = data.Price;
            if (data.Price > itemData.minStick.Price[0])
                itemData.minStick.Price[0] = data.Price;
            else if (data.Price < itemData.minStick.Price[1])
                itemData.minStick.Price[1] = data.Price;
            
            itemData.secStick.Price[3] = data.Price;
            itemData.minStick.Price[3] = data.Price;

            if (data.BuyerIsMaker)
            {
                itemData.secStick.Md += data.Quantity;
                itemData.minStick.Md += data.Quantity;
            }
            else
            {
                itemData.secStick.Ms += data.Quantity;
                itemData.minStick.Ms += data.Quantity;
            }

            itemData.secStick.TCount++;

            if (itemData.isChartShowing)
            {
                itemData.timeDiff = (DateTime.UtcNow - data.TradeTime).TotalSeconds;
                if (itemData.timeDiff > itemData.timeDiffMax)
                    itemData.timeDiffMax = itemData.timeDiff;

                UpdateChartPoint(secChart, itemData.secStick);
                if (chartNow.TabIndex == minChart.TabIndex)
                    UpdateChartPoint(minChart, itemData.minStick);

                if (itemData.position && secChart.ChartAreas[0].AxisY2.StripLines.Count == 1)
                {
                    if (data.Price > itemData.EntryPrice)
                    {
                        secChart.ChartAreas[0].AxisY2.StripLines[0].StripWidth = (double)(data.Price - itemData.EntryPrice);
                        secChart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemData.EntryPrice - secChart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        minChart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemData.EntryPrice - minChart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        if (itemData.LorS)
                        {
                            secChart.ChartAreas[0].AxisY2.StripLines[0].BackColor = earningColor;
                            secChart.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((data.Price / itemData.EntryPrice - 1) * 100, 2).ToString();
                        }    
                        else
                        {
                            secChart.ChartAreas[0].AxisY2.StripLines[0].BackColor = losingColor;
                            secChart.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((itemData.EntryPrice / data.Price - 1) * 100, 2).ToString();
                        }
                    }
                    else
                    {
                        secChart.ChartAreas[0].AxisY2.StripLines[0].StripWidth = (double)(itemData.EntryPrice - data.Price);
                        secChart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)data.Price - secChart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        minChart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)data.Price - minChart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        if (itemData.LorS)
                        {
                            secChart.ChartAreas[0].AxisY2.StripLines[0].BackColor = losingColor;
                            secChart.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((data.Price / itemData.EntryPrice - 1) * 100, 2).ToString();
                        }
                        else
                        {
                            secChart.ChartAreas[0].AxisY2.StripLines[0].BackColor = earningColor;
                            secChart.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((itemData.EntryPrice / data.Price - 1) * 100, 2).ToString();
                        }
                    }
                    minChart.ChartAreas[0].AxisY2.StripLines[0].StripWidth = secChart.ChartAreas[0].AxisY2.StripLines[0].StripWidth;
                    minChart.ChartAreas[0].AxisY2.StripLines[0].BackColor = secChart.ChartAreas[0].AxisY2.StripLines[0].BackColor;
                    minChart.ChartAreas[0].AxisY2.StripLines[0].Text = secChart.ChartAreas[0].AxisY2.StripLines[0].Text;
                }
            }

            {       // 시뮬레이션
                if (!itemData.simulEnterOrder && itemData.Real == 2 && EnterCondition(itemData))
                {
                    itemData.simulEnterOrder = true;

                    itemData.simulBhtPrc = itemData.secStick.Price[3];
                    itemData.simulBhtTime = itemData.secStick.Time;
                }
                else if (itemData.simulEnterOrder)
                {
                    if (!itemData.tooManyAmt
                        &&
                        (
                            (itemData.simulLorS 
                                && itemData.secStick.Ms - itemData.secStick.Md > itemData.ms20secAvg * 12
                                && itemData.secStick.Ms - itemData.secStick.Md > itemData.md20secAvg * 10
                                && itemData.secStick.Ms / (itemData.secStick.Ms + itemData.secStick.Md) > 0.9m)
                            ||
                            (!itemData.simulLorS 
                                && itemData.secStick.Md - itemData.secStick.Ms > itemData.md20secAvg * 12
                                && itemData.secStick.Md - itemData.secStick.Ms > itemData.ms20secAvg * 10
                                && itemData.secStick.Md / (itemData.secStick.Ms + itemData.secStick.Md) > 0.9m)
                        )
                    )
                    {
                        itemData.tooManyAmt = true;
                        itemData.tooManyAmtTime = itemData.secStick.Time;
                    }

                    if (data.TradeTime.Subtract(itemData.simulBhtTime).TotalSeconds >= 1
                        &&
                        (
                            ExitCondition(itemData)
                            ||
                            (
                                itemData.tooManyAmt && data.TradeTime.Subtract(itemData.tooManyAmtTime).TotalSeconds >= 1
                                && 
                                (
                                    (itemData.simulLorS && itemData.secStickList.Last().Ms < itemData.ms20secAvg)
                                    || 
                                    (!itemData.simulLorS && itemData.secStickList.Last().Md < itemData.md20secAvg)
                                )
                            )
                        )
                    )
                    {
                        if (itemData.simulLorS)
                            itemData.simulProfitRate =  Math.Round((data.Price / itemData.simulBhtPrc - 1) * 100, 2);
                        else
                            itemData.simulProfitRate =  Math.Round((itemData.simulBhtPrc / data.Price - 1) * 100, 2);

                        simulTotalTrades++;
                        simulThisTrades++;
                        if (itemData.simulProfitRate > commisionRate)
                        {
                            simulTotalWin++;
                            simulThisWin++;
                        }
                        simulTotalRateSum += itemData.simulProfitRate;
                        simulThisRateSum += itemData.simulProfitRate;

                        simulTotalWinRateTextBox.Text = Math.Round((double)simulTotalWin / simulTotalTrades, 2) + "(" + simulTotalTrades + ") " + Math.Round((double)simulTotalRateSum / simulTotalTrades, 2);
                        simulTodayWinRateTextBox.Text = Math.Round((double)simulThisWin / simulThisTrades, 2) + "(" + simulThisTrades + ") " + Math.Round((double)simulThisRateSum / simulThisTrades, 2);

                        var simulResultData = new SimulResultData {
                            Name = itemData.Name,
                            ProfitRate = itemData.simulProfitRate,
                            Time = itemData.simulBhtTime.ToString(secTimeFormat) + "+" + Math.Round(data.TradeTime.Subtract(itemData.simulBhtTime).TotalSeconds, 1)
                        };

                        dbHelper.SaveData1("Simulation_Trade_History", "Time", simulResultData.Time, "Symbol", simulResultData.Name, "Profit", simulResultData.ProfitRate.ToString());

                        simulResultListView.InsertObjects(0, new List<SimulResultData> { simulResultData });

                        itemData.simulEnterOrder = false;
                    }
                }
            }
        }
        void OnMarkPriceUpdates(BinanceFuturesUsdtStreamMarkPrice data, ItemData itemData)
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

                var walletBalance = assetDic["Wallet Balance"].Amount;
                var MaintMargin = assetDic["Maintenance Margin"];
                MaintMargin.Amount = Math.Round(itemData.maintMargin, 2);
                var MarginBalance = assetDic["Margin Balance"];
                MarginBalance.Amount = Math.Round(walletBalance + itemData.PNL, 2);
                var asset = assetDic["Available Balance"];
                asset.Amount = Math.Round(walletBalance + itemData.PNL - itemData.InitialMargin, 2);
                asset = assetDic["Margin Ratio"];
                asset.Amount = Math.Round(MaintMargin.Amount / MarginBalance.Amount * 100, 2);
                assetsListView.Refresh();
            }
        }
        void OnAccountUpdates(BinanceFuturesStreamAccountUpdate data)
        {
            foreach (var balance in data.UpdateData.Balances)
                if (balance.Asset == "USDT")
                {
                    var asset = assetDic["Wallet Balance"];
                    asset.Amount = balance.WalletBalance;
                    assetsListView.RefreshObject(asset);
                }

            if (data.UpdateData.Reason == AccountUpdateReason.Order)
                foreach (var position in data.UpdateData.Positions)
                {
                    var itemData = FUItemDataList[position.Symbol];

                    if (position.PositionAmount == 0)
                    {
                        socketClient.Unsubscribe(itemData.markSub);

                        itemData.position = false;

                        ResetOrderView();

                        var asset = assetDic["Margin Ratio"];
                        asset.Amount = 0;
                        asset = assetDic["Maintenance Margin"];
                        asset.Amount = 0;
                        asset = assetDic["Available Balance"];
                        asset.Amount = assetDic["Wallet Balance"].Amount;
                        asset = assetDic["Margin Balance"];
                        asset.Amount = assetDic["Wallet Balance"].Amount;
                        assetsListView.Refresh();

                        secChart.ChartAreas[0].AxisY2.StripLines.Clear();
                        minChart.ChartAreas[0].AxisY2.StripLines.Clear();
                    }
                    else if (itemData.position)
                        itemData.Size = position.PositionAmount;
                    else
                    {
                        itemData.position = true;
                        itemData.EntryPrice = position.EntryPrice;
                        itemData.Size = position.PositionAmount;
                        itemData.PNL = position.UnrealizedPnl;

                        ResetOrderView();

                        itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Name, 3000,
                            data2 => { BeginInvoke(markUpdates, data2, itemData); }).Data;

                        var strip = new StripLine();
                        strip.Interval = double.NaN;
                        strip.ForeColor = ForeColor;
                        strip.TextLineAlignment = StringAlignment.Center;
                        secChart.ChartAreas[0].AxisY2.StripLines.Add(strip);

                        strip = new StripLine();
                        strip.Interval = double.NaN;
                        strip.ForeColor = ForeColor;
                        strip.TextLineAlignment = StringAlignment.Center;
                        minChart.ChartAreas[0].AxisY2.StripLines.Add(strip);
                    }
                }
        }
        void OnOrderUpdates(BinanceFuturesStreamOrderUpdate data, ItemData itemData)
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
                itemData.orderID = data.UpdateData.OrderId;

                if (!itemData.position)
                {
                    var asset = assetDic["Available Balance"];
                    asset.Amount -= (itemData.OrderPrice * itemData.OrderAmount) / itemData.Leverage;
                    assetsListView.RefreshObject(asset);
                }
            }
            else if (data.UpdateData.Status == OrderStatus.PartiallyFilled)
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;
            else if (data.UpdateData.Status == OrderStatus.Filled || data.UpdateData.Status == OrderStatus.Canceled 
                || data.UpdateData.Status == OrderStatus.Rejected || data.UpdateData.Status == OrderStatus.Expired)
            {
                if (data.UpdateData.Status == OrderStatus.Filled && !itemData.position)
                {
                    ResultData resultData;
                    if (resultListView.Items.Count == 0)
                    {
                        resultData = new ResultData { Symbol = itemData.Name };
                        resultListView.InsertObjects(0, new List<ResultData> { resultData });
                    }
                    else
                        resultData = resultListView.GetModelObject(0) as ResultData;
                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.LastGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                        resultData.ProfitRate = Math.Round((data.UpdateData.AveragePrice / itemData.EntryPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.LastGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                        resultData.ProfitRate = Math.Round((itemData.EntryPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }
                    resultData.ProfitRateAndValue = resultData.ProfitRate + ", " + data.UpdateData.RealizedProfit.ToString(ForDecimalString);
                    resultData.LastGapAndTimeAndSuccessAmount = resultData.LastGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.LastTime).TotalSeconds, 1)
                        + ", " + resultData.LastGapAndTimeAndSuccessAmount;

                    totalTrades++;
                    thisTrades++;
                    totalRateSum += resultData.ProfitRate;
                    thisRateSum += resultData.ProfitRate;
                    if (resultData.ProfitRate > commisionRate)
                    {
                        totalWin++;
                        thisWin++;
                    }
                    totalWinRateTextBox.Text = Math.Round((double)totalWin / totalTrades, 2) + "(" + totalTrades + ") " + Math.Round((double)totalRateSum / totalTrades, 2);
                    todayWinRateTextBox.Text = Math.Round((double)thisWin / thisTrades, 2) + "(" + thisTrades + ") " + Math.Round((double)thisRateSum / thisTrades, 2);

                    dbHelper.SaveData1("Trade_History", "Time", resultData.EntryTime.ToString(secTimeFormat) + ", " + resultData.LastTime.ToString(secTimeFormat), "Symbol", resultData.Symbol, "Profit", resultData.ProfitRate.ToString());

                    resultListView.RefreshObject(resultData);

                    LoadResultEffect(resultData.ProfitRate);
                }
                else if ((data.UpdateData.Status == OrderStatus.Filled && itemData.position) || data.UpdateData.Status == OrderStatus.Canceled)
                {
                    var resultData = resultListView.GetModelObject(0) as ResultData;

                    if (data.UpdateData.Status == OrderStatus.Canceled && itemData.positionWhenOrder)
                    {
                        resultData.LastGapAndTimeAndSuccessAmount = Math.Round((itemData.OrderAmount - itemData.Size) / itemData.OrderAmount, 2).ToString();

                        if (itemData.LorS)
                            PlaceOrder(OrderSide.Sell, true);
                        else
                            PlaceOrder(OrderSide.Buy, true);
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
                            + ", " + Math.Round(itemData.Size / itemData.OrderAmount, 2);
                    }

                    resultListView.RefreshObject(resultData);
                }

                if (!itemData.position && !itemData.positionWhenOrder)
                {
                    var asset = assetDic["Available Balance"];
                    asset.Amount += itemData.OrderPrice * itemData.OrderAmount / itemData.Leverage;
                    assetsListView.RefreshObject(asset);
                }
            }
        }
        void OnHoUpdates(IBinanceFuturesEventOrderBook data, ItemData itemData)
        {
            if (!itemData.isChartShowing)
                return;

            itemData.hoHighQuan = 0;

            itemData.hoIndex = 19;
            foreach (var bid in data.Bids)
            {
                itemData.hoLog = (int)Math.Log10((double)(bid.Price * 1.005m - bid.Price));
                if (itemData.hoLog >= 0) itemData.hoLog++;
                itemData.hoLog = (decimal)Math.Pow(10, (double)itemData.hoLog);
                itemData.hoPosition = (bid.Price / ((int)(bid.Price / itemData.hoLog) * itemData.hoLog) - 1) * 100 / 0.5m;
                itemData.hoInt = (int)itemData.hoPosition;
                itemData.hoDecimal = itemData.hoPosition - itemData.hoInt;
                if (itemData.hoDecimal <= 0.2m)
                {
                    hoChart.Series[0].Points[itemData.hoIndex].AxisLabel = bid.Price + "_" + itemData.hoInt;
                    hoChart.Series[1].Points[itemData.hoIndex].AxisLabel = bid.Price + "_" + itemData.hoInt;
                    hoChart.Series[0].Points[itemData.hoIndex].Color = pureMsAmtColor;
                }
                else
                {
                    hoChart.Series[0].Points[itemData.hoIndex].AxisLabel = bid.Price.ToString();
                    hoChart.Series[1].Points[itemData.hoIndex].AxisLabel = bid.Price.ToString();
                }

                hoChart.Series[0].Points[itemData.hoIndex].YValues[0] = (double)bid.Quantity;
                hoChart.Series[1].Points[itemData.hoIndex].YValues[0] = 0;

                itemData.hoIndex--;

                if (bid.Quantity > itemData.hoHighQuan)
                {
                    itemData.hoHighQuan = bid.Quantity;
                    itemData.hoPrice = bid.Price;
                }
            }

            itemData.hoIndex = 20;
            foreach (var ask in data.Asks)
            {
                itemData.hoLog = (int)Math.Log10((double)(ask.Price * 1.005m - ask.Price));
                if (itemData.hoLog >= 0) itemData.hoLog++;
                itemData.hoLog = (decimal)Math.Pow(10, (double)itemData.hoLog);
                itemData.hoPosition = (ask.Price / ((int)(ask.Price / itemData.hoLog) * itemData.hoLog) - 1) * 100 / 0.5m;
                itemData.hoInt = (int)itemData.hoPosition;
                itemData.hoDecimal = itemData.hoPosition - itemData.hoInt;
                if (itemData.hoDecimal <= 0.2m)
                {
                    hoChart.Series[0].Points[itemData.hoIndex].AxisLabel = ask.Price + "_" + itemData.hoInt;
                    hoChart.Series[1].Points[itemData.hoIndex].AxisLabel = ask.Price + "_" + itemData.hoInt;
                    hoChart.Series[1].Points[itemData.hoIndex].Color = pureMdAmtColor;
                }
                else
                {
                    hoChart.Series[0].Points[itemData.hoIndex].AxisLabel = ask.Price.ToString();
                    hoChart.Series[1].Points[itemData.hoIndex].AxisLabel = ask.Price.ToString();
                }

                hoChart.Series[0].Points[itemData.hoIndex].YValues[0] = 0;
                hoChart.Series[1].Points[itemData.hoIndex].YValues[0] = (double)ask.Quantity;

                itemData.hoIndex++;

                if (ask.Quantity > itemData.hoHighQuan)
                {
                    itemData.hoHighQuan = ask.Quantity;
                    itemData.hoPrice = ask.Price;
                }
            }

            hoChart.ChartAreas[0].AxisY.ScaleView.Zoom(0, (double)itemData.hoHighQuan);
            hoChart.ChartAreas[0].AxisY.ScaleView.Position = 0;
            hoHighPriceTextBox.Text = ((int)(itemData.hoHighQuan * itemData.hoPrice / 1000)).ToString();
        }

        void SetAgg(ItemData itemData, bool on)
        {
            if (on)
            {
                itemData.Real = 1;

                itemData.secStickList.Clear();
                itemData.secStick = new Stick();
                itemData.ms5secTot = 0;
                itemData.md5secTot = 0;
                itemData.ms10secTot = 0;
                itemData.md10secTot = 0;
                itemData.ms20secTot = 0;
                itemData.md20secTot = 0;

                itemData.aggSub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, FUItemDataList[data2.Symbol]); }).Data;

                FUAggReq++;
            }
            else
            {
                itemData.Real = 0;

                socketClient.Unsubscribe(itemData.aggSub);

                itemData.AggFirst = true;

                FUAggRcv--;
                FUAggReq--;
                FUAggRcvTextBox.Text = FUAggRcv.ToString();

                itemData.minLowestSDevRatioPrice = 0;
                itemData.minLowestSDevRatio = 0;
                itemData.minLowestSDevCount10sec = 0;
            }
            FUAggReqTextBox.Text = "/" + FUAggReq + "(A)";
            FUListView.RefreshObject(itemData);
        }

        bool EnterCondition(ItemData itemData)
        {
            if (itemData.secStickList.Last().Ms > (decimal)itemData.real2BigAmt10secAvg * 0.8m
                && itemData.secStick.Ms > (decimal)itemData.real2BigAmt10secAvg * 0.8m
                && itemData.secStickList.Last().Md < (decimal)itemData.real2SmallAmt10secAvg * 1.2m
                && itemData.secStick.Md < (decimal)itemData.real2SmallAmt10secAvg * 1.2m
                && itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.ms20secAvg * 2
                && itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.md20secAvg * 1.5m
                && itemData.secStick.Ms - itemData.secStick.Md > itemData.ms20secAvg * 2
                && itemData.secStick.Ms - itemData.secStick.Md > itemData.md20secAvg * 1.5m
                &&
                (
                    itemData.pureSecCountQAvg < 0.8m
                    ||
                    (
                        itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.ms5secAvg * 2
                        && itemData.secStickList.Last().Ms - itemData.secStickList.Last().Md > itemData.md5secAvg * 1.5m
                        && itemData.secStick.Ms - itemData.secStick.Md > itemData.ms5secAvg * 2
                        && itemData.secStick.Ms - itemData.secStick.Md > itemData.md5secAvg * 1.5m
                    )
                )
            )
            {
                itemData.tooManyAmt = false;
                itemData.simulLorS = true;
                return true;
            }
            else if(itemData.secStickList.Last().Md > (decimal)itemData.real2BigAmt10secAvg * 0.8m
                && itemData.secStick.Md > (decimal)itemData.real2BigAmt10secAvg * 0.8m
                && itemData.secStickList.Last().Ms < (decimal)itemData.real2SmallAmt10secAvg * 1.2m
                && itemData.secStick.Ms < (decimal)itemData.real2SmallAmt10secAvg * 1.2m
                && itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.md20secAvg * 2
                && itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.ms20secAvg * 1.5m
                && itemData.secStick.Md - itemData.secStick.Ms > itemData.md20secAvg * 2
                && itemData.secStick.Md - itemData.secStick.Ms > itemData.ms20secAvg * 1.5m
                &&
                (
                    itemData.pureSecCountQAvg > 0.2m
                    ||
                    (
                        itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.md5secAvg * 2
                        && itemData.secStickList.Last().Md - itemData.secStickList.Last().Ms > itemData.ms5secAvg * 1.5m
                        && itemData.secStick.Md - itemData.secStick.Ms > itemData.md5secAvg * 2
                        && itemData.secStick.Md - itemData.secStick.Ms > itemData.ms5secAvg * 1.5m
                    )
                )
            )
            {
                itemData.tooManyAmt = false;
                itemData.simulLorS = false;
                return true;
            }
            else
                return false;
        }
        bool ExitCondition(ItemData itemData)
        {
            if ((itemData.simulLorS
                && itemData.secStickList.Last().Md > itemData.ms5secAvg
                && itemData.secStickList.Last().Md > itemData.ms20secAvg * 2
                && itemData.secStickList.Last().Md > itemData.secStickList.Last().Ms)
                ||
                (!itemData.simulLorS
                && itemData.secStickList.Last().Ms > itemData.md5secAvg
                && itemData.secStickList.Last().Ms > itemData.md20secAvg * 2
                && itemData.secStickList.Last().Ms > itemData.secStickList.Last().Md))
                return true;
            else
                return false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    if (itemDataShowing != null)
                        PlaceOrder(OrderSide.Buy, false);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.C:
                    if (itemDataShowing != null)
                        PlaceOrder(OrderSide.Sell, false);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.A:
                    if (itemDataShowing != null)
                        ChartScroll(chartNow, ScrollType.SmallDecrement);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.S:
                    if (itemDataShowing != null)
                        ChartScroll(chartNow, ScrollType.Last);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.D:
                    if (itemDataShowing != null)
                        ChartScroll(chartNow, ScrollType.SmallIncrement);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.Q:
                    if (itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMinimum - 9, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.W:
                    if (itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - baseChartViewSticksSize, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.E:
                    if (itemDataShowing != null)
                    {
                        if (chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                            chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 11, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        else
                            chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(0, chartNow.Series[0].Points.Count);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.D1:
                    SetChartNowOrLoad(secChart);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.D2:
                    SetChartNowOrLoad(minChart);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.D3:
                    SetChartNowOrLoad(hourChart);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.D4:
                    SetChartNowOrLoad(dayChart);
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.Oemcomma:    //,
                    FUListView.Sort(FUListView.GetColumn("Name"), SortOrder.Ascending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.OemPeriod:    //.
                    FUListView.Sort(FUListView.GetColumn("R"), SortOrder.Descending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.Oem2:     // /
                    FUListView.Sort(FUListView.GetColumn("MsSD"), SortOrder.Descending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                default:
                    break;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            base.OnClosed(e);

            if (tokenSource != null)
                tokenSource.Cancel();

            socketClient.UnsubscribeAll();
            dbHelper.Close();
        }
    }

    class ResultData
    {
        public decimal EntryGap;
        public DateTime EntryTime;
        public string EntryGapAndTimeAndSuccessAmount;
        public decimal LastGap;
        public DateTime LastTime;
        public string LastGapAndTimeAndSuccessAmount;
        public decimal ProfitRate;
        public decimal RealizedProfit = 0;
        public string ProfitRateAndValue;
        public string Symbol;
    }

    class SimulResultData
    {
        public string Name;
        public string Time;
        public decimal ProfitRate;
    }

    class Asset
    {
        public string AssetName;
        public decimal Amount;
    }
}
