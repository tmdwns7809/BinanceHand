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

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        #region Global vars
        string startTime;

        Dictionary<string, ItemData> FUItemDataList = new Dictionary<string, ItemData>();

        ItemData itemDataShowing;

        static int PrcAlpha = 200;
        static int amtAlpha = 200;
        static int stripAlpha = 20;
        Color msAmtColor = Color.FromArgb(amtAlpha, 0, 89, 64);
        Color mdAmtColor = Color.FromArgb(amtAlpha, 130, 17, 43);
        Color msAndMdAmtColor = Color.FromArgb(amtAlpha, 65, 53, 53);
        Color plsPrcColor = Color.FromArgb(PrcAlpha, 14, 203, 129);
        Color mnsPrcColor = Color.FromArgb(PrcAlpha, 207, 48, 74);
        Color earningColor = Color.FromArgb(stripAlpha, Color.Gold);
        Color losingColor = Color.FromArgb(stripAlpha, Color.LightGray);

        int baseChartViewSticksSize = 120;

        static int gridAlpha = 20;
        Color gridColor = Color.FromArgb(gridAlpha, Color.Gray);

        Color controlBackColor = Color.FromArgb(24, 26, 32);
        Color buttonSelectedColor = Color.DarkGray;
        Color buttonColor = Color.Gray;

        decimal FUMarginBalance;
        decimal FUMaintMargin;
        decimal FUAvailableBalance;
        decimal FUWalletBalance;
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

        List<string> FUSymbolList = new List<string>();

        bool positionExitPriceMarket = true;
        bool miniSizeDefault = true;
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

            SetMainTab();

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

            var chartAreaMain = secChart.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMain.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisX.MajorGrid.Interval = 5;
            chartAreaMain.AxisX.MajorTickMark.Size = 0.4f;
            chartAreaMain.AxisX.LabelStyle.Interval = 30;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.secChartLabel;
            chartAreaMain.AxisX.LabelStyle.ForeColor = Color.Transparent;
            chartAreaMain.AxisX.ScrollBar.BackColor = Color.Transparent;
            chartAreaMain.AxisX.ScrollBar.ButtonColor = Color.Transparent;
            chartAreaMain.AxisX.ScrollBar.LineColor = Color.Transparent;
            chartAreaMain.AxisX.LineColor = gridColor;

            chartAreaMain.AxisY.Enabled = AxisEnabled.False;
            chartAreaMain.AxisY2.Enabled = AxisEnabled.True;
            chartAreaMain.AxisY2.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisY2.ScrollBar.Enabled = false;
            chartAreaMain.AxisY2.MajorTickMark.Enabled = false;
            chartAreaMain.AxisY2.IsStartedFromZero = false;
            chartAreaMain.AxisY2.LabelStyle.ForeColor = ForeColor;
            chartAreaMain.AxisY2.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartAreaMain.AxisY2.LineColor = gridColor;

            chartAreaMain.Position = new ElementPosition(0, 0, 100, 60);
            chartAreaMain.BackColor = controlBackColor;
            chartAreaMain.BorderColor = ForeColor;

            var chartAreaMsMd = secChart.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = chartAreaMain.BackColor;
            chartAreaMsMd.BorderColor = chartAreaMain.BorderColor;
            chartAreaMsMd.Position = new ElementPosition(chartAreaMain.Position.X, chartAreaMain.Position.Height - 5, chartAreaMain.Position.Width, 100 - chartAreaMain.Position.Height + 6);

            chartAreaMsMd.AxisX.ScaleView.SizeType = chartAreaMain.AxisX.ScaleView.SizeType;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = chartAreaMain.AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMsMd.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMsMd.AxisX.MajorGrid.Interval = chartAreaMain.AxisX.MajorGrid.Interval;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = gridColor;
            chartAreaMsMd.AxisX.MajorTickMark.Size = chartAreaMain.AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.Interval = chartAreaMain.AxisX.LabelStyle.Interval;
            chartAreaMsMd.AxisX.LabelStyle.Format = chartAreaMain.AxisX.LabelStyle.Format;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = ForeColor;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = BackColor;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = Color.FromArgb(18, 22, 28);
            chartAreaMsMd.AxisX.ScrollBar.LineColor = Color.FromArgb(150, Color.LightGray);
            chartAreaMsMd.AxisX.LineColor = gridColor;

            chartAreaMsMd.AxisY.Enabled = AxisEnabled.False;
            chartAreaMsMd.AxisY2.Enabled = AxisEnabled.True;
            chartAreaMsMd.AxisY2.MajorGrid.Enabled = false;
            chartAreaMsMd.AxisY2.ScrollBar.Enabled = false;
            chartAreaMsMd.AxisY2.MajorTickMark.Enabled = false;
            chartAreaMsMd.AxisY2.LabelStyle.ForeColor = ForeColor;
            chartAreaMsMd.AxisY2.LineColor = gridColor;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;

            var seriesPrice = secChart.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = plsPrcColor;
            seriesPrice.YAxisType = AxisType.Secondary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            var seriesMsAndMdAmt = secChart.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = AxisType.Secondary;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            var seriesMsOrMdAmt = secChart.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = AxisType.Secondary;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;


            minChart.BackColor = BackColor;
            minChart.ForeColor = ForeColor;
            minChart.Location = secChart.Location;
            minChart.Size = secChart.Size;

            minChart.Tag = 0.02;
            minChart.AxisViewChanged += (sender, e) => { AdjustChart(minChart); };
            minChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            minChart.MouseWheel += OnChartMouseWheel;

            chartAreaMain = minChart.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMain.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartAreaMain.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartAreaMain.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartAreaMain.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.minChartLabel;
            chartAreaMain.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartAreaMain.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartAreaMain.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartAreaMain.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartAreaMain.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartAreaMain.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartAreaMain.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartAreaMain.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartAreaMain.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartAreaMain.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartAreaMain.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartAreaMain.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartAreaMain.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartAreaMain.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartAreaMain.Position = secChart.ChartAreas[0].Position;
            chartAreaMain.BackColor = secChart.ChartAreas[0].BackColor;
            chartAreaMain.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartAreaMsMd = minChart.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = secChart.ChartAreas[1].BackColor;
            chartAreaMsMd.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartAreaMsMd.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartAreaMsMd.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMsMd.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartAreaMsMd.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartAreaMsMd.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartAreaMsMd.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartAreaMsMd.AxisX.LabelStyle.Format = chartAreaMain.AxisX.LabelStyle.Format;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartAreaMsMd.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartAreaMsMd.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartAreaMsMd.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartAreaMsMd.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartAreaMsMd.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartAreaMsMd.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartAreaMsMd.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartAreaMsMd.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartAreaMsMd.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;

            seriesPrice = minChart.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = secChart.Series[0].Color;
            seriesPrice.YAxisType = AxisType.Secondary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            seriesMsAndMdAmt = minChart.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = secChart.Series["매수and매도량"].YAxisType;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            seriesMsOrMdAmt = minChart.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = secChart.Series["매수or매도량"].YAxisType;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;


            hourChart.BackColor = BackColor;
            hourChart.ForeColor = ForeColor;
            hourChart.Location = secChart.Location;
            hourChart.Size = secChart.Size;

            hourChart.Tag = 0.05;
            hourChart.AxisViewChanged += (sender, e) => { AdjustChart(minChart); };
            hourChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            hourChart.MouseWheel += OnChartMouseWheel;

            chartAreaMain = hourChart.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMain.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartAreaMain.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartAreaMain.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartAreaMain.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.hourChartLabel;
            chartAreaMain.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartAreaMain.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartAreaMain.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartAreaMain.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartAreaMain.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartAreaMain.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartAreaMain.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartAreaMain.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartAreaMain.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartAreaMain.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartAreaMain.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartAreaMain.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartAreaMain.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartAreaMain.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartAreaMain.Position = secChart.ChartAreas[0].Position;
            chartAreaMain.BackColor = secChart.ChartAreas[0].BackColor;
            chartAreaMain.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartAreaMsMd = hourChart.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = secChart.ChartAreas[1].BackColor;
            chartAreaMsMd.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartAreaMsMd.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartAreaMsMd.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMsMd.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartAreaMsMd.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartAreaMsMd.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartAreaMsMd.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartAreaMsMd.AxisX.LabelStyle.Format = chartAreaMain.AxisX.LabelStyle.Format;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartAreaMsMd.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartAreaMsMd.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartAreaMsMd.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartAreaMsMd.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartAreaMsMd.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartAreaMsMd.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartAreaMsMd.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartAreaMsMd.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartAreaMsMd.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;

            seriesPrice = hourChart.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = secChart.Series[0].Color;
            seriesPrice.YAxisType = AxisType.Secondary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            seriesMsAndMdAmt = hourChart.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = secChart.Series["매수and매도량"].YAxisType;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            seriesMsOrMdAmt = hourChart.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = secChart.Series["매수or매도량"].YAxisType;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;


            dayChart.BackColor = BackColor;
            dayChart.ForeColor = ForeColor;
            dayChart.Location = secChart.Location;
            dayChart.Size = secChart.Size;

            dayChart.Tag = 0.1;
            dayChart.AxisViewChanged += (sender, e) => { AdjustChart(minChart); };
            dayChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            dayChart.MouseWheel += OnChartMouseWheel;

            chartAreaMain = dayChart.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = secChart.ChartAreas[0].AxisX.ScaleView.SizeType;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[0].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMain.AxisX.MajorGrid.LineColor = secChart.ChartAreas[0].AxisX.MajorGrid.LineColor;
            chartAreaMain.AxisX.MajorGrid.Interval = secChart.ChartAreas[0].AxisX.MajorGrid.Interval;
            chartAreaMain.AxisX.MajorTickMark.Size = secChart.ChartAreas[0].AxisX.MajorTickMark.Size;
            chartAreaMain.AxisX.LabelStyle.Interval = secChart.ChartAreas[0].AxisX.LabelStyle.Interval;
            chartAreaMain.AxisX.LabelStyle.Format = ItemData.dayChartLabel;
            chartAreaMain.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisX.LabelStyle.ForeColor;
            chartAreaMain.AxisX.ScrollBar.BackColor = secChart.ChartAreas[0].AxisX.ScrollBar.BackColor;
            chartAreaMain.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[0].AxisX.ScrollBar.ButtonColor;
            chartAreaMain.AxisX.ScrollBar.LineColor = secChart.ChartAreas[0].AxisX.ScrollBar.LineColor;
            chartAreaMain.AxisX.LineColor = secChart.ChartAreas[0].AxisX.LineColor;

            chartAreaMain.AxisY.Enabled = secChart.ChartAreas[0].AxisY.Enabled;
            chartAreaMain.AxisY2.Enabled = secChart.ChartAreas[0].AxisY2.Enabled;
            chartAreaMain.AxisY2.MajorGrid.LineColor = secChart.ChartAreas[0].AxisY2.MajorGrid.LineColor;
            chartAreaMain.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[0].AxisY2.ScrollBar.Enabled;
            chartAreaMain.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[0].AxisY2.MajorTickMark.Enabled;
            chartAreaMain.AxisY2.IsStartedFromZero = secChart.ChartAreas[0].AxisY2.IsStartedFromZero;
            chartAreaMain.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[0].AxisY2.LabelStyle.ForeColor;
            chartAreaMain.AxisY2.IntervalAutoMode = secChart.ChartAreas[0].AxisY2.IntervalAutoMode;
            chartAreaMain.AxisY2.LineColor = secChart.ChartAreas[0].AxisY2.LineColor;

            chartAreaMain.Position = secChart.ChartAreas[0].Position;
            chartAreaMain.BackColor = secChart.ChartAreas[0].BackColor;
            chartAreaMain.BorderColor = secChart.ChartAreas[0].BorderColor;

            chartAreaMsMd = dayChart.ChartAreas.Add("ChartAreaMsMd");
            chartAreaMsMd.BackColor = secChart.ChartAreas[1].BackColor;
            chartAreaMsMd.BorderColor = secChart.ChartAreas[1].BorderColor;
            chartAreaMsMd.Position.FromRectangleF(secChart.ChartAreas[1].Position.ToRectangleF());

            chartAreaMsMd.AxisX.ScaleView.SizeType = secChart.ChartAreas[1].AxisX.ScaleView.SizeType;
            chartAreaMsMd.AxisX.ScaleView.Small​Scroll​Size = secChart.ChartAreas[1].AxisX.ScaleView.Small​Scroll​Size;
            chartAreaMsMd.AxisX.MajorGrid.Enabled = secChart.ChartAreas[1].AxisX.MajorGrid.Enabled;
            chartAreaMsMd.AxisX.MajorGrid.LineColor = secChart.ChartAreas[1].AxisX.MajorGrid.LineColor;
            chartAreaMsMd.AxisX.MajorGrid.Interval = secChart.ChartAreas[1].AxisX.MajorGrid.Interval;
            chartAreaMsMd.AxisX.MajorTickMark.LineColor = secChart.ChartAreas[1].AxisX.MajorTickMark.LineColor;
            chartAreaMsMd.AxisX.MajorTickMark.Size = secChart.ChartAreas[1].AxisX.MajorTickMark.Size;
            chartAreaMsMd.AxisX.LabelStyle.Interval = secChart.ChartAreas[1].AxisX.LabelStyle.Interval;
            chartAreaMsMd.AxisX.LabelStyle.Format = chartAreaMain.AxisX.LabelStyle.Format;
            chartAreaMsMd.AxisX.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisX.LabelStyle.ForeColor;
            chartAreaMsMd.AxisX.ScrollBar.BackColor = secChart.ChartAreas[1].AxisX.ScrollBar.BackColor;
            chartAreaMsMd.AxisX.ScrollBar.ButtonColor = secChart.ChartAreas[1].AxisX.ScrollBar.ButtonColor;
            chartAreaMsMd.AxisX.ScrollBar.LineColor = secChart.ChartAreas[1].AxisX.ScrollBar.LineColor;
            chartAreaMsMd.AxisX.LineColor = secChart.ChartAreas[1].AxisX.LineColor;

            chartAreaMsMd.AxisY.Enabled = secChart.ChartAreas[1].AxisY.Enabled;
            chartAreaMsMd.AxisY2.Enabled = secChart.ChartAreas[1].AxisY2.Enabled;
            chartAreaMsMd.AxisY2.MajorGrid.Enabled = secChart.ChartAreas[1].AxisY2.MajorGrid.Enabled;
            chartAreaMsMd.AxisY2.ScrollBar.Enabled = secChart.ChartAreas[1].AxisY2.ScrollBar.Enabled;
            chartAreaMsMd.AxisY2.MajorTickMark.Enabled = secChart.ChartAreas[1].AxisY2.MajorTickMark.Enabled;
            chartAreaMsMd.AxisY2.LabelStyle.ForeColor = secChart.ChartAreas[1].AxisY2.LabelStyle.ForeColor;
            chartAreaMsMd.AxisY2.LineColor = secChart.ChartAreas[1].AxisY2.LineColor;

            chartAreaMsMd.AlignWithChartArea = chartAreaMain.Name;

            seriesPrice = dayChart.Series.Add("가격");
            seriesPrice.ChartType = SeriesChartType.Candlestick;
            seriesPrice.XValueMember = "Time";
            seriesPrice.YValueMembers = "High,Low,Open,Close";
            seriesPrice.XValueType = ChartValueType.Time;
            seriesPrice.Color = secChart.Series[0].Color;
            seriesPrice.YAxisType = AxisType.Secondary;
            seriesPrice.ChartArea = chartAreaMain.Name;

            seriesMsAndMdAmt = dayChart.Series.Add("매수and매도량");
            seriesMsAndMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsAndMdAmt.XValueType = ChartValueType.Time;
            seriesMsAndMdAmt.Color = msAndMdAmtColor;
            seriesMsAndMdAmt.YAxisType = secChart.Series["매수and매도량"].YAxisType;
            seriesMsAndMdAmt.ChartArea = chartAreaMsMd.Name;

            seriesMsOrMdAmt = dayChart.Series.Add("매수or매도량");
            seriesMsOrMdAmt.ChartType = SeriesChartType.StackedColumn;
            seriesMsOrMdAmt.XValueType = ChartValueType.Time;
            seriesMsOrMdAmt.Color = msAmtColor;
            seriesMsOrMdAmt.YAxisType = secChart.Series["매수or매도량"].YAxisType;
            seriesMsOrMdAmt.ChartArea = chartAreaMsMd.Name;
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
        void SetMainTab()
        {
            mainTabControl.Location = new Point(secChart.Location.X + 10, secChart.Location.Y + secChart.Size.Height + 10);
            mainTabControl.Size = new Size((int)(secChart.Size.Width * 0.5) - 10, Screen.GetWorkingArea(this).Size.Height - secChart.Size.Height - 50);
            mainTabControl.SelectedTab = FUTab;

            FUTab.BackColor = controlBackColor;
            FUTab.ForeColor = ForeColor;
            FUTab.Size = new Size(mainTabControl.Size.Width - 17, mainTabControl.Size.Height - 37);

            FUTabControl.Location = new Point(5, 5);
            FUTabControl.Size = FUTab.Size;
            FUTabControl.SelectedTab = FUPositionTab;

            FUPositionTab.BackColor = controlBackColor;
            FUPositionTab.ForeColor = ForeColor;
            FUPositionTab.Size = new Size(FUTabControl.Size.Width - 10, FUTabControl.Size.Height - 10);

            FUOpenOrdersTab.BackColor = controlBackColor;
            FUOpenOrdersTab.ForeColor = ForeColor;
            FUOpenOrdersTab.Size = FUPositionTab.Size;

            FUPositionListView.BackColor = controlBackColor;
            FUPositionListView.ForeColor = ForeColor;
            FUPositionListView.Location = new Point(0, 0);
            FUPositionListView.Size = FUPositionTab.Size;
            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var nameColumn = new OLVColumn("Symbol", "Name");
            nameColumn.FreeSpaceProportion = 3;
            nameColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(nameColumn);
            var leverageColumn = new OLVColumn("Leverage", "Leverage");
            leverageColumn.FreeSpaceProportion = 2;
            leverageColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(leverageColumn);
            var sizeColumn = new OLVColumn("Size", "Size");
            sizeColumn.FreeSpaceProportion = 2;
            sizeColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(sizeColumn);
            var entryPriceColumn = new OLVColumn("Entry Price", "EntryPrice");
            entryPriceColumn.FreeSpaceProportion = 2;
            entryPriceColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(entryPriceColumn);
            var markPriceColumn = new OLVColumn("Mark Price", "MarkPrice");
            markPriceColumn.FreeSpaceProportion = 2;
            markPriceColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(markPriceColumn);
            var initialmarginColumn = new OLVColumn("Initial Margin", "InitialMargin");
            initialmarginColumn.FreeSpaceProportion = 2;
            initialmarginColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(initialmarginColumn);
            var PNLColumn = new OLVColumn("PNL", "PNL");
            PNLColumn.FreeSpaceProportion = 2;
            PNLColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(PNLColumn);
            var ROEColumn = new OLVColumn("ROE(%)", "ROE");
            ROEColumn.FreeSpaceProportion = 2;
            ROEColumn.HeaderFormatStyle = headerstyle;
            FUPositionListView.AllColumns.Add(ROEColumn);
            FUPositionListView.Columns.AddRange(new ColumnHeader[]
                { nameColumn, leverageColumn, sizeColumn, entryPriceColumn, markPriceColumn,
                    initialmarginColumn, PNLColumn, ROEColumn });
            FUPositionListView.SelectionChanged += (sender, e) => { if (FUPositionListView.SelectedIndices.Count == 1) ShowChart(FUPositionListView.SelectedObject as ItemData); };

            FUOpenOrdersListView.BackColor = controlBackColor;
            FUOpenOrdersListView.ForeColor = ForeColor;
            FUOpenOrdersListView.Location = FUPositionListView.Location;
            FUOpenOrdersListView.Size = FUPositionListView.Size;

            var timeColumn = new OLVColumn("Time", "OrderTime");
            timeColumn.FreeSpaceProportion = 6;
            timeColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(timeColumn);
            nameColumn = new OLVColumn("Symbol", "Name");
            nameColumn.FreeSpaceProportion = 3;
            nameColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(nameColumn);
            var typeColumn = new OLVColumn("Type", "OrderType");
            typeColumn.FreeSpaceProportion = 3;
            typeColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(typeColumn);
            var sideColumn = new OLVColumn("Side", "OrderSide");
            sideColumn.FreeSpaceProportion = 3;
            sideColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(sideColumn);
            var priceColumn = new OLVColumn("Price", "OrderPrice");
            priceColumn.FreeSpaceProportion = 3;
            priceColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(priceColumn);
            var amountColumn = new OLVColumn("Amount", "OrderAmount");
            amountColumn.FreeSpaceProportion = 3;
            amountColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(amountColumn);
            var filledColumn = new OLVColumn("Filled", "OrderFilled");
            filledColumn.FreeSpaceProportion = 3;
            filledColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(filledColumn);
            var ROColumn = new OLVColumn("Reduce Only", "ReduceOnly");
            ROColumn.FreeSpaceProportion = 3;
            ROColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(ROColumn);
            var conditionColumn = new OLVColumn("Condition", "Condition");
            conditionColumn.FreeSpaceProportion = 3;
            conditionColumn.HeaderFormatStyle = headerstyle;
            FUOpenOrdersListView.AllColumns.Add(conditionColumn);
            FUOpenOrdersListView.Columns.AddRange(new ColumnHeader[]
                { timeColumn, nameColumn, typeColumn, sideColumn, priceColumn, amountColumn,
                    filledColumn, ROColumn, conditionColumn });
            FUOpenOrdersListView.SelectionChanged += (sender, e) => { if (FUOpenOrdersListView.SelectedIndices.Count == 1) ShowChart(FUOpenOrdersListView.SelectedObject as ItemData); };

            dayButton.BackColor = buttonColor;
            dayButton.ForeColor = ForeColor;
            dayButton.Location = new Point(mainTabControl.Location.X + mainTabControl.Size.Width - 10 - dayButton.Size.Width, mainTabControl.Location.Y - 5);
            dayButton.Click += (sebder, e) => { if (!dayChart.Visible) SetChartNowOrLoad(dayChart); };
            dayButton.BringToFront();

            hourButton.BackColor = buttonColor;
            hourButton.ForeColor = ForeColor;
            hourButton.Location = new Point(dayButton.Location.X - 1 - dayButton.Size.Width, dayButton.Location.Y);
            hourButton.Click += (sebder, e) => { if (!hourChart.Visible) SetChartNowOrLoad(hourChart); };
            hourButton.BringToFront();

            minButton.BackColor = buttonSelectedColor;
            minButton.ForeColor = ForeColor;
            minButton.Location = new Point(hourButton.Location.X - 1 - dayButton.Size.Width, dayButton.Location.Y);
            minButton.Click += (sebder, e) => { if (!minChart.Visible) SetChartNowOrLoad(minChart); };
            minButton.BringToFront();

            secButton.BackColor = buttonColor;
            secButton.ForeColor = ForeColor;
            secButton.Location = new Point(minButton.Location.X - 1 - dayButton.Size.Width, dayButton.Location.Y);
            secButton.Click += (sebder, e) => { if (!secChart.Visible) SetChartNowOrLoad(secChart); };
            secButton.BringToFront();

            FUMarginRatioTextBox0.BackColor = FUTab.BackColor;
            FUMarginRatioTextBox0.ForeColor = ForeColor;
            FUMarginRatioTextBox0.Location =
                new Point(FUTabControl.Location.X + FUTabControl.Size.Width - FUMarginRatioTextBox0.Size.Width - FUMarginRatioTextBox1.Size.Width
                    - FUMaintMarginTextBox0.Size.Width - FUMaintMarginTextBox1.Size.Width - FUMarginBalanceTextBox0.Size.Width - FUMarginBalanceTextBox1.Size.Width
                    - FUAvailBalanceTextBox0.Size.Width - FUAvailBalanceTextBox1.Size.Width - FUWalletBalanceTextBox0.Size.Width - FUWalletBalanceTextBox1.Size.Width, FUTabControl.Location.Y);
            FUMarginRatioTextBox0.BringToFront();

            FUMarginRatioTextBox1.BackColor = FUTab.BackColor;
            FUMarginRatioTextBox1.ForeColor = ForeColor;
            FUMarginRatioTextBox1.Location =
                new Point(FUMarginRatioTextBox0.Location.X + FUMarginRatioTextBox0.Size.Width, FUTabControl.Location.Y);
            FUMarginRatioTextBox1.BringToFront();

            FUMaintMarginTextBox0.BackColor = FUTab.BackColor;
            FUMaintMarginTextBox0.ForeColor = ForeColor;
            FUMaintMarginTextBox0.Location =
                new Point(FUMarginRatioTextBox1.Location.X + FUMarginRatioTextBox1.Size.Width, FUTabControl.Location.Y);
            FUMaintMarginTextBox0.BringToFront();

            FUMaintMarginTextBox1.BackColor = FUTab.BackColor;
            FUMaintMarginTextBox1.ForeColor = ForeColor;
            FUMaintMarginTextBox1.Location =
                new Point(FUMaintMarginTextBox0.Location.X + FUMaintMarginTextBox0.Size.Width, FUTabControl.Location.Y);
            FUMaintMarginTextBox1.BringToFront();

            FUMarginBalanceTextBox0.BackColor = FUTab.BackColor;
            FUMarginBalanceTextBox0.ForeColor = ForeColor;
            FUMarginBalanceTextBox0.Location =
                new Point(FUMaintMarginTextBox1.Location.X + FUMaintMarginTextBox1.Size.Width, FUTabControl.Location.Y);
            FUMarginBalanceTextBox0.BringToFront();

            FUMarginBalanceTextBox1.BackColor = FUTab.BackColor;
            FUMarginBalanceTextBox1.ForeColor = ForeColor;
            FUMarginBalanceTextBox1.Location =
                new Point(FUMarginBalanceTextBox0.Location.X + FUMarginBalanceTextBox0.Size.Width, FUTabControl.Location.Y);
            FUMarginBalanceTextBox1.BringToFront();

            FUAvailBalanceTextBox0.BackColor = FUTab.BackColor;
            FUAvailBalanceTextBox0.ForeColor = ForeColor;
            FUAvailBalanceTextBox0.Location =
                new Point(FUMarginBalanceTextBox1.Location.X + FUMarginBalanceTextBox1.Size.Width, FUTabControl.Location.Y);
            FUAvailBalanceTextBox0.BringToFront();

            FUAvailBalanceTextBox1.BackColor = FUTab.BackColor;
            FUAvailBalanceTextBox1.ForeColor = ForeColor;
            FUAvailBalanceTextBox1.Location =
                new Point(FUAvailBalanceTextBox0.Location.X + FUAvailBalanceTextBox0.Size.Width, FUTabControl.Location.Y);
            FUAvailBalanceTextBox1.BringToFront();

            FUWalletBalanceTextBox0.BackColor = FUTab.BackColor;
            FUWalletBalanceTextBox0.ForeColor = ForeColor;
            FUWalletBalanceTextBox0.Location =
                new Point(FUAvailBalanceTextBox1.Location.X + FUAvailBalanceTextBox1.Size.Width, FUTabControl.Location.Y);
            FUWalletBalanceTextBox0.BringToFront();

            FUWalletBalanceTextBox1.BackColor = FUTab.BackColor;
            FUWalletBalanceTextBox1.ForeColor = ForeColor;
            FUWalletBalanceTextBox1.Location =
                new Point(FUWalletBalanceTextBox0.Location.X + FUWalletBalanceTextBox0.Size.Width, FUTabControl.Location.Y);
            FUWalletBalanceTextBox1.BringToFront();
        }
        void SetOrderView()
        {
            orderGroupBox.BackColor = BackColor;
            orderGroupBox.ForeColor = ForeColor;
            orderGroupBox.Location = new Point(mainTabControl.Location.X + mainTabControl.Size.Width + 10, mainTabControl.Location.Y);
            orderGroupBox.Size = new Size(350, mainTabControl.Size.Height);

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

                mainTabControl.Focus();
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
            timeDiffTextBox.Location = new Point(orderGroupBox.Location.X + 50, orderGroupBox.Location.Y - 20);
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
            resultListView.BackColor = controlBackColor;
            resultListView.ForeColor = ForeColor;
            resultListView.Size = new Size(Screen.GetWorkingArea(this).Size.Width - orderGroupBox.Location.X - orderGroupBox.Size.Width - 20, mainTabControl.Size.Height);
            resultListView.Location = new Point(orderGroupBox.Location.X + orderGroupBox.Size.Width + 10, mainTabControl.Location.Y);
            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var column0 = new OLVColumn("No.", "Number");
            column0.FreeSpaceProportion = 1;
            column0.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(column0);
            var entryColumn = new OLVColumn("EG(%), TD(s), SA", "EntryGapAndTimeAndSuccessAmount");
            entryColumn.FreeSpaceProportion = 3;
            entryColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(entryColumn);
            var lastColumn = new OLVColumn("LG(%), TD(s), SA", "LastGapAndTimeAndSuccessAmount");
            lastColumn.FreeSpaceProportion = 3;
            lastColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(lastColumn);
            var profitColumn = new OLVColumn("PR(%), RP($)", "ProfitRateAndValue");
            profitColumn.FreeSpaceProportion = 3;
            profitColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(profitColumn);
            resultListView.Columns.AddRange(new ColumnHeader[] { column0, entryColumn, lastColumn, profitColumn });
            resultListView.FormatRow += (sender, e) =>
            {
                if (((ResultData)e.Model).Profit > 0.1m)
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
        }
        void SetSymbolsListView()
        {
            FUGroupBox.BackColor = BackColor;
            FUGroupBox.ForeColor = ForeColor;
            FUGroupBox.Location = new Point(secChart.Location.X + secChart.Size.Width, secChart.Location.Y + 10);
            FUGroupBox.Size = new Size(Screen.GetWorkingArea(this).Size.Width - secChart.Width - 10, secChart.Size.Height - 20);

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
            realButton.Location = new Point(FUAggReqTextBox.Location.X + FUAggReqTextBox.Width, FUAggReqTextBox.Location.Y);
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
            if (profit <= 0.1m)
            {
                pictureBox.Image = fail;
                soundEngine.Play2D(failSound, false, false, false);
            }
            else if (profit <= 0.4m)
            {
                pictureBox.Image = win0;
                soundEngine.Play2D(win0Sound, false, false, false);
            }
            else if (profit <= 0.7m)
            {
                pictureBox.Image = win1;
                soundEngine.Play2D(win1Sound, false, false, false);
            }
            else if (profit <= 1.0m)
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

                    var limitAmount = (int)(FUAvailableBalance * limitPercent / 100 / price / itemDataShowing.minSize) * itemDataShowing.minSize;

                    if (limitAmount < quantity)
                        quantity = limitAmount;
                }
            }

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
                    resultData = new ResultData(1);
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

                var resultData = new ResultData(resultListView.Items.Count + 1);
                resultData.EntryTime = DateTime.UtcNow;
                resultListView.InsertObjects(0, new List<ResultData> { resultData });

                itemDataShowing.Size = 0;
            }
        }
        void CancelOrder(ItemData itemData)
        {
            client.FuturesUsdt.Order.CancelAllOrders(itemData.Name);
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
                itemDataShowing.isChartShowing = false;
            itemDataShowing = itemData;

            Text = itemData.Name + "-Future(Usdt)      Binance       UTC-" + startTime;

            foreach (var se in secChart.Series)
                se.Points.Clear();

            if (itemData.Real >= 1)
            {
                for (int i = 0; i < itemData.secStickList.Count; i++)
                    AddFullChartPoint(secChart, itemData.secStickList[i]);
                if (itemData.secStick.Time != default)
                    AddFullChartPoint(secChart, itemData.secStick);
                FirstZoomIfNeeded(secChart);
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
        }
        void FirstZoomIfNeeded(Chart chart)
        {
            chart.ChartAreas[0].RecalculateAxesScale();
            if (chart.Series[0].Points.Count > baseChartViewSticksSize)
            {
                chart.ChartAreas[0].AxisX.ScaleView.Zoom(chart.Series[0].Points.Count - baseChartViewSticksSize + 1, chart.Series[0].Points.Count);
                chart.ChartAreas[0].RecalculateAxesScale();
            }
            else
            {
                chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                chart.ChartAreas[0].RecalculateAxesScale();
            }
            chart.ChartAreas[1].RecalculateAxesScale();
            AdjustChart(chart);
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

            chart.ChartAreas[0].AxisY2.ScaleView.Zoom(priceLow, priceHigh);
            chart.ChartAreas[1].AxisY2.ScaleView.Zoom(0, msOrMdHigh);

            chart.ChartAreas[0].AxisY2.MajorGrid.Interval = (priceLow + priceHigh) / 2 * (double)chart.Tag;

            chart.ChartAreas[0].RecalculateAxesScale();

            if (itemDataShowing.position && chartNow.ChartAreas[0].AxisY2.StripLines.Count == 1)
            {
                if (itemDataShowing.EntryPrice > itemDataShowing.minStick.Price[3])
                    chartNow.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemDataShowing.minStick.Price[3] - chartNow.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                else
                    chartNow.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemDataShowing.EntryPrice - chartNow.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
            }
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

            chartNow = chart;

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
                LoadMore(chartNow, 300, true);
                minButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == hourChart.TabIndex)
            {
                LoadMore(chartNow, 300, true);
                hourButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == dayChart.TabIndex)
            {
                LoadMore(chartNow, 300, true);
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

                    for (int i = limit - 1; i >= 0; i--)
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
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Md);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Ms - stick.Md);
                chart.Series[2].Points.Last().Color = msAmtColor;
            }
            else
            {
                chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Ms);
                chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, stick.Md - stick.Ms);
                chart.Series[2].Points.Last().Color = mdAmtColor;
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
                chart.Series[1].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Md);
                chart.Series[2].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Ms - stick.Md);
                chart.Series[2].Points[index].Color = msAmtColor;
            }
            else
            {
                chart.Series[1].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Ms);
                chart.Series[2].Points.InsertXY(index, chart.Series[0].Points[index].AxisLabel, stick.Md - stick.Ms);
                chart.Series[2].Points[index].Color = mdAmtColor;
            }
        }
        void AddStartChartPoint(Chart chart, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(chart.ChartAreas[0].AxisX.LabelStyle.Format), new object[] { (double)stick.Price[2], (double)stick.Price[2], (double)stick.Price[2], (double)stick.Price[2] });
            chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);

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
            var go = client.FuturesUsdt.Order.GetOpenOrders().Data;
            foreach (var order in go)
            {
                var itemData = FUItemDataList[order.Symbol];

                itemData.order = true;
                itemData.OrderTime = order.CreatedTime;
                itemData.OrderType = order.OriginalType;
                itemData.OrderSide = order.Side;
                itemData.OrderPrice = order.Price;
                itemData.OrderAmount = order.OriginalQuantity;
                itemData.OrderFilled = order.ExecutedQuantity;
                itemData.ReduceOnly = order.ReduceOnly;
                itemData.Condition = order.TimeInForce;
                itemData.clientOrderID = order.ClientOrderId;
                itemData.orderID = order.OrderId;

                FUOpenOrdersListView.AddObject(itemData);
                FUOpenOrdersListView.RefreshObject(itemData);
                FUOpenOrdersTab.Text = "Open Orders(" + FUOpenOrdersListView.Items.Count + ")";
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
                    FUWalletBalance = s.WalletBalance;
                    FUWalletBalanceTextBox1.Text = Math.Round(FUWalletBalance, 2).ToString();
                }
            }
            foreach (var s in ga.Data.Positions)
            {
                if (s.EntryPrice != 0m)
                {
                    var itemData = FUItemDataList[s.Symbol];

                    itemData.position = true;

                    itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Name, 3000,
                        data => { BeginInvoke(markUpdates, data, itemData); }).Data;

                    itemData.InitialMargin = Math.Round(s.InitialMargin, 2);
                    itemData.maintMargin = s.MaintMargin;

                    FUPositionListView.AddObject(itemData);
                    FUPositionListView.RefreshObject(itemData);
                    FUPositionTab.Text = "Position(" + FUPositionListView.Items.Count + ")";
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

                    itemData.Leverage = s.Leverage;
                    itemData.MarkPrice = Math.Round(s.MarkPrice, 2);
                    itemData.Size = s.PositionAmount;
                    itemData.notianalValue = s.MarkPrice * Math.Abs(itemData.Size);
                    itemData.EntryPrice = Math.Round(s.EntryPrice, 2);
                    itemData.PNL = Math.Round(s.UnrealizedProfit, 2);
                    itemData.ROE = Math.Round(itemData.PNL / itemData.InitialMargin * 100, 2);

                    foreach (var brackets in client.FuturesUsdt.GetBrackets(s.Symbol).Data)
                        itemData.brackets = brackets.Brackets.ToList();

                    itemData.maxNotionalValue = s.MaxNotionalValue;
                    itemData.maxLeverage = itemData.brackets[0].InitialLeverage;

                    FUPositionListView.UpdateObject(itemData);
                    FUPositionListView.RefreshObject(itemData);
                }
            }
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
                        SetAgg(itemData, true);

                    itemData.FlatMinRow = 0;
                }
                else if (itemData.FlatMinRow < 15)
                    itemData.FlatMinRow++;
                else if (itemData.Real >= 1 && !itemData.isChartShowing)
                    SetAgg(itemData, false);

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
                if (itemData.secStick.Price[1] != decimal.Zero)
                {
                    itemData.secStickList.Add(itemData.secStick);

                    itemData.secLastIndex = itemData.secStickList.Count - 1;

                    if (itemData.secLastIndex > 9)
                    {
                        itemData.price10secHighest = itemData.secStickList[itemData.secLastIndex].Price[0];
                        itemData.price10secLowest = itemData.secStickList[itemData.secLastIndex].Price[1];

                        itemData.ms10secTot = itemData.ms10secTot + (double)(itemData.secStickList[itemData.secLastIndex].Ms - itemData.secStickList[itemData.secLastIndex - 10].Ms);
                        itemData.md10secTot = itemData.md10secTot + (double)(itemData.secStickList[itemData.secLastIndex].Md - itemData.secStickList[itemData.secLastIndex - 10].Md);

                        itemData.ms10secAvg = itemData.ms10secTot / 10;
                        itemData.md10secAvg = itemData.md10secTot / 10;

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

                        if (itemData.Real != 2 && itemData.SDevRatioPrice > 1.005m
                            && (itemData.msSDevRatio < 1.5 || itemData.mdSDevRatio < 1.5))
                        {
                            itemData.Real = 2;
                            soundEngine.Play2D(win3Sound, false, false, false);
                            FUListView.RefreshObject(itemData);
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
                        itemData.ms10secTot += (double)itemData.secStickList[itemData.secLastIndex].Ms;
                        itemData.md10secTot += (double)itemData.secStickList[itemData.secLastIndex].Md;

                        if (itemData.isChartShowing)
                            autoTextBox.Text = "0000";
                    }
                }

                itemData.secStick = new Stick();

                itemData.secStick.Price[2] = data.Price;
                itemData.secStick.Price[0] = data.Price;
                itemData.secStick.Price[1] = data.Price;
                itemData.secStick.Price[3] = data.Price;

                itemData.secStick.Time = DateTime.Parse(data.TradeTime.ToString("yyyy-MM-dd HH:mm:ss"));

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
                    }
                }

                if (data.TradeTime.Subtract(itemData.minStick.Time).TotalMinutes >= 1)
                {
                    if (itemData.minStick.Price[1] != decimal.Zero)
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

                    itemData.minStick.Time = DateTime.Parse(data.TradeTime.ToString("yyyy-MM-dd HH:mm"));

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
                timeDiffTextBox.Text = Math.Round((DateTime.UtcNow - data.TradeTime).TotalSeconds, 1).ToString();

                UpdateChartPoint(secChart, itemData.secStick);
                if (chartNow.TabIndex == minChart.TabIndex)
                    UpdateChartPoint(minChart, itemData.minStick);

                if (itemData.position && secChart.ChartAreas[0].AxisY2.StripLines.Count == 1)
                {
                    if (data.Price > itemData.EntryPrice)
                    {
                        secChart.ChartAreas[0].AxisY2.StripLines[0].StripWidth = (double)(data.Price - itemData.EntryPrice);
                        secChart.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemData.EntryPrice - secChart.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
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

                FUMaintMargin = 0;
                FUMarginBalance = FUWalletBalance;
                FUAvailableBalance = FUWalletBalance;
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
                FUPositionListView.RefreshObject(itemData);
            }
        }
        void OnAccountUpdates(BinanceFuturesStreamAccountUpdate data)
        {
            foreach (var balance in data.UpdateData.Balances)
                if (balance.Asset == "USDT")
                {
                    FUWalletBalance = balance.WalletBalance;
                    FUWalletBalanceTextBox1.Text = Math.Round(FUWalletBalance, 2).ToString();
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

                        FUPositionListView.RemoveObject(itemData);
                        FUPositionListView.RefreshObject(itemData);
                        FUPositionTab.Text = "Position(" + FUPositionListView.Items.Count + ")";

                        if (FUPositionListView.Items.Count == 0)
                        {
                            FUMarginRatioTextBox1.Text = "0";
                            FUMaintMargin = 0;
                            FUMaintMarginTextBox1.Text = "0";
                            FUMarginBalance = 0;
                            FUMarginBalanceTextBox1.Text = "0";
                            FUAvailableBalance = FUWalletBalance;
                            FUAvailBalanceTextBox1.Text = FUWalletBalanceTextBox1.Text;
                        }

                        chartNow.ChartAreas[0].AxisY2.StripLines.Clear();
                    }
                    else if (itemData.position)
                    {
                        itemData.Size = position.PositionAmount;

                        FUPositionListView.UpdateObject(itemData);
                        FUPositionListView.RefreshObject(itemData);
                    }
                    else
                    {
                        itemData.position = true;
                        itemData.EntryPrice = position.EntryPrice;
                        itemData.Size = position.PositionAmount;
                        itemData.PNL = position.UnrealizedPnl;

                        ResetOrderView();

                        itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Name, 3000,
                            data2 => { BeginInvoke(markUpdates, data2, itemData); }).Data;

                        FUPositionListView.AddObject(itemData);
                        FUPositionListView.RefreshObject(itemData);
                        FUPositionTab.Text = "Position(" + FUPositionListView.Items.Count + ")";

                        var strip = new StripLine();
                        strip.Interval = double.MaxValue;
                        strip.ForeColor = ForeColor;
                        strip.TextLineAlignment = StringAlignment.Center;
                        chartNow.ChartAreas[0].AxisY2.StripLines.Add(strip);
                    }
                }
        }
        void OnOrderUpdates(BinanceFuturesStreamOrderUpdate data, ItemData itemData)
        {
            if (data.UpdateData.Status == OrderStatus.New)
            {
                itemData.order = true;

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
                    FUAvailableBalance -= (itemData.OrderPrice * itemData.OrderAmount) / itemData.Leverage;
                    FUAvailBalanceTextBox1.Text = Math.Round(FUAvailableBalance, 2).ToString();
                }

                FUOpenOrdersListView.AddObject(itemData);
                FUOpenOrdersListView.RefreshObject(itemData);
                FUOpenOrdersTab.Text = "Open Orders(" + FUOpenOrdersListView.Items.Count + ")";
            }
            else if (data.UpdateData.Status == OrderStatus.PartiallyFilled)
            {
                itemData.OrderFilled = data.UpdateData.AccumulatedQuantityOfFilledTrades;

                FUOpenOrdersListView.UpdateObject(itemData);
                FUOpenOrdersListView.RefreshObject(itemData);
            }
            else if (data.UpdateData.Status == OrderStatus.Filled || data.UpdateData.Status == OrderStatus.Canceled 
                || data.UpdateData.Status == OrderStatus.Rejected || data.UpdateData.Status == OrderStatus.Expired)
            {
                if (data.UpdateData.Status == OrderStatus.Filled && !itemData.position)
                {
                    ResultData resultData;
                    if (resultListView.Items.Count == 0)
                    {
                        resultData = new ResultData(1);
                        resultListView.InsertObjects(0, new List<ResultData> { resultData });
                    }
                    else
                        resultData = resultListView.GetModelObject(0) as ResultData;
                    if (data.UpdateData.Side == OrderSide.Sell)
                    {
                        resultData.LastGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                        resultData.Profit = Math.Round((data.UpdateData.AveragePrice / itemData.EntryPrice - 1) * 100, 2);
                    }
                    else
                    {
                        resultData.LastGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                        resultData.Profit = Math.Round((itemData.EntryPrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                    }
                    resultData.ProfitRateAndValue = resultData.Profit + ", " + data.UpdateData.RealizedProfit.ToString(ForDecimalString);
                    resultData.LastGapAndTimeAndSuccessAmount = resultData.LastGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.LastTime).TotalSeconds, 1)
                        + ", " + resultData.LastGapAndTimeAndSuccessAmount;
                    resultListView.RefreshObject(resultData);

                    LoadResultEffect(resultData.Profit);
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
                        if (data.UpdateData.Side == OrderSide.Buy)
                            resultData.EntryGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                        else
                            resultData.EntryGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);

                        resultData.EntryGapAndTimeAndSuccessAmount = resultData.EntryGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.EntryTime).TotalSeconds, 1)
                            + ", " + Math.Round(itemData.Size / itemData.OrderAmount, 2);
                    }

                    resultListView.RefreshObject(resultData);
                }

                if (!itemData.position && !itemData.positionWhenOrder)
                {
                    FUAvailableBalance += (itemData.OrderPrice * itemData.OrderAmount) / itemData.Leverage;
                    FUAvailBalanceTextBox1.Text = Math.Round(FUAvailableBalance, 2).ToString();
                }

                itemData.order = false;

                FUOpenOrdersListView.RemoveObject(itemData);
                FUOpenOrdersListView.RefreshObject(itemData);
                FUOpenOrdersTab.Text = "Open Orders(" + FUOpenOrdersListView.Items.Count + ")";
            }
        }

        void SetAgg(ItemData itemData, bool on)
        {
            if (on)
            {
                itemData.Real = 1;

                itemData.secStickList.Clear();
                itemData.secStick = new Stick();
                itemData.ms10secTot = 0;
                itemData.md10secTot = 0;

                itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, FUItemDataList[data2.Symbol]); }).Data;

                FUAggReq++;
            }
            else
            {
                itemData.Real = 0;

                socketClient.Unsubscribe(itemData.sub);

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
                    if (itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 11, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
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

                case Keys.Delete:
                    if (itemDataShowing != null && itemDataShowing.order)
                        CancelOrder(itemDataShowing);
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
            //dbHelper.Close();
        }
    }

    class ResultData
    {
        public int Number;
        public decimal EntryGap;
        public DateTime EntryTime;
        public string EntryGapAndTimeAndSuccessAmount;
        public decimal LastGap;
        public DateTime LastTime;
        public string LastGapAndTimeAndSuccessAmount;
        public decimal Profit;
        public string ProfitRateAndValue;

        public ResultData(int n)
        {
            Number = n;
        }
    }
}
