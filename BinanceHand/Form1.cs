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

        //DBHelper dbHelper = DBHelper.GetInstance();

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
        bool onlyAgg = true;

        int maxSecListCount = 600;
        #endregion

        public Form1()
        {
            InitializeComponent();

            SetComponents();

            SetClientAndKey();
        }

        void SetComponents()
        {
            marketComboBox.SelectedItem = marketComboBox.Items[0];

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
        Chart chartNow;
        void SetChart()
        {
            chartNow = minChart;

            secChart.BackColor = BackColor;
            secChart.ForeColor = ForeColor;
            secChart.Location = new Point(0, 0);
            secChart.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.8), (int)(Screen.GetWorkingArea(this).Size.Height * 0.8));

            secChart.Tag = 0.005;
            secChart.AxisViewChanged += OnChartAxisViewChanged;
            secChart.AxisScrollBarClicked += OnChartAxisScrollBarClicked;
            secChart.MouseWheel += OnChartMouseWheel;
            //secChart.PostPaint += OnChartPostPaint;

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
            chartAreaMsMd.Position = new ElementPosition(chartAreaMain.Position.X, chartAreaMain.Position.Height - 6, chartAreaMain.Position.Width, 100 - chartAreaMain.Position.Height + 6);

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
            minChart.AxisViewChanged += OnChartAxisViewChanged;
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
            hourChart.AxisViewChanged += OnChartAxisViewChanged;
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
            dayChart.AxisViewChanged += OnChartAxisViewChanged;
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

            secButton.Click += (sebder, e) => { if (!secChart.Visible) SetChartNowOrLoad(secChart); };
            minButton.Click += (sebder, e) => { if (!minChart.Visible) SetChartNowOrLoad(minChart); };
            hourButton.Click += (sebder, e) => { if (!hourChart.Visible) SetChartNowOrLoad(hourChart); };
            dayButton.Click += (sebder, e) => { if (!dayChart.Visible) SetChartNowOrLoad(dayChart); };
        }
        void OnChartAxisViewChanged(object sender, ViewEventArgs e)
        {
            AdjustChart(chartNow);
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
        void OnChartPostPaint(object sender, ChartPaintEventArgs e)
        {
            // loop only over line annotations:
            List<LineAnnotation> annos =
                chartNow.Annotations.Where(x => x is LineAnnotation)
                                    .Cast<LineAnnotation>().ToList();
            if (!annos.Any()) return;

            // a few short references
            Graphics g = e.ChartGraphics.Graphics;
            ChartArea ca = chartNow.ChartAreas[0];
            Axis ax = ca.AxisX;
            Axis ay = ca.AxisY2;

            // we want to clip the line to the innerplotposition excluding the scrollbar:
            Rectangle r = Rectangle.Round(InnerPlotPositionClientRectangle(chartNow, ca));
            g.SetClip(new Rectangle(r.X, r.Y, r.Width, r.Height - (int)ax.ScrollBar.Size));
            g.InterpolationMode = InterpolationMode.NearestNeighbor;  // pick your mode!
            foreach (LineAnnotation la in annos)
            {
                if (Double.IsNaN(la.Width)) continue;  // *
                                                       // calculate the coordinates
                int x1 = (int)ax.ValueToPixelPosition(la.AnchorX);
                int y1 = (int)ay.ValueToPixelPosition(la.AnchorY);
                int x2 = (int)ax.ValueToPixelPosition(la.AnchorX + la.Width);
                int y2 = (int)ay.ValueToPixelPosition(la.AnchorY + la.Height);

                // now we draw the line if necessary:
                if (x1 < r.X || x1 > r.Right)
                    using (Pen pen = new Pen(la.LineColor, 0.5f)) g.DrawLine(pen, x1, y1, x2, y2);
            }
            // reset the clip to allow the system drawing a scrollbar
            g.ResetClip();
        }
        RectangleF InnerPlotPositionClientRectangle(Chart chart, ChartArea CA)
        {
            RectangleF IPP = CA.InnerPlotPosition.ToRectangleF();
            RectangleF CArp = ChartAreaClientRectangle(chart, CA);

            float pw = CArp.Width / 100f;
            float ph = CArp.Height / 100f;

            return new RectangleF(CArp.X + pw * IPP.X, CArp.Y + ph * IPP.Y,
                                    pw * IPP.Width, ph * IPP.Height);
        }
        RectangleF ChartAreaClientRectangle(Chart chart, ChartArea CA)
        {
            RectangleF CAR = CA.Position.ToRectangleF();
            float pw = chart.ClientSize.Width / 100f;
            float ph = chart.ClientSize.Height / 100f;
            return new RectangleF(pw * CAR.X, ph * CAR.Y, pw * CAR.Width, ph * CAR.Height);
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

            marketComboBox.BackColor = controlBackColor;
            marketComboBox.ForeColor = ForeColor;
            marketComboBox.Size = new Size(60, nameTextBox.Size.Height);
            marketComboBox.Location = new Point(mainTabControl.Location.X + mainTabControl.Size.Width - marketComboBox.Size.Width - nameTextBox.Size.Width - 10
                , mainTabControl.Location.Y - 5);
            marketComboBox.BringToFront();

            nameTextBox.BackColor = controlBackColor;
            nameTextBox.ForeColor = ForeColor;
            nameTextBox.Location = new Point(marketComboBox.Location.X + marketComboBox.Size.Width + 1, marketComboBox.Location.Y);
            nameTextBox.BringToFront();

            dayButton.BackColor = buttonColor;
            dayButton.ForeColor = ForeColor;
            dayButton.Location = new Point(marketComboBox.Location.X - 10 - dayButton.Size.Width, marketComboBox.Location.Y);

            hourButton.BackColor = buttonColor;
            hourButton.ForeColor = ForeColor;
            hourButton.Location = new Point(dayButton.Location.X - 1 - dayButton.Size.Width, marketComboBox.Location.Y);

            minButton.BackColor = buttonSelectedColor;
            minButton.ForeColor = ForeColor;
            minButton.Location = new Point(hourButton.Location.X - 1 - dayButton.Size.Width, marketComboBox.Location.Y);

            secButton.BackColor = buttonColor;
            secButton.ForeColor = ForeColor;
            secButton.Location = new Point(minButton.Location.X - 1 - dayButton.Size.Width, marketComboBox.Location.Y);


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
            buyButton.Click += (sender, e) => { PlaceOrder(OrderSide.Buy); };

            sellButton.BackColor = Color.FromArgb(255, 207, 48, 74);
            sellButton.ForeColor = ForeColor;
            sellButton.Size = buyButton.Size;
            sellButton.Location = new Point(buyButton.Location.X + buyButton.Size.Width + 5, buyButton.Location.Y);
            sellButton.Click += (sender, e) => { PlaceOrder(OrderSide.Sell); };

            ROCheckBox.BackColor = BackColor;
            ROCheckBox.ForeColor = ForeColor;
            ROCheckBox.Location = new Point(sellButton.Location.X + sellButton.Size.Width - ROCheckBox.Size.Width, sellButton.Location.Y - ROCheckBox.Size.Height - 3);

            priceTextBox.BackColor = BackColor;
            priceTextBox.ForeColor = ForeColor;
            priceTextBox.Location = new Point(orderGroupBox.Location.X + orderGroupBox.Size.Width - priceTextBox.Size.Width - amtTextBox.Size.Width - timeDiffTextBox.Size.Width
                , orderGroupBox.Location.Y - 5 - priceTextBox.Size.Height);
            priceTextBox.BringToFront();

            amtTextBox.BackColor = BackColor;
            amtTextBox.ForeColor = ForeColor;
            amtTextBox.Location = new Point(priceTextBox.Location.X + priceTextBox.Size.Width, priceTextBox.Location.Y);
            amtTextBox.BringToFront();

            timeDiffTextBox.BackColor = BackColor;
            timeDiffTextBox.ForeColor = ForeColor;
            timeDiffTextBox.Location = new Point(amtTextBox.Location.X + amtTextBox.Size.Width, amtTextBox.Location.Y);
            timeDiffTextBox.Size = new Size(30, amtTextBox.Size.Height);
            timeDiffTextBox.BringToFront();

            gridItvTextBox.BackColor = BackColor;
            gridItvTextBox.ForeColor = ForeColor;
            gridItvTextBox.Location = new Point(priceTextBox.Location.X - gridItvTextBox.Size.Width, priceTextBox.Location.Y);
            gridItvTextBox.BringToFront();

            autoTextBox.BackColor = BackColor;
            autoTextBox.ForeColor = ForeColor;
            autoTextBox.Location = new Point(gridItvTextBox.Location.X - autoTextBox.Size.Width, gridItvTextBox.Location.Y);
            autoTextBox.BringToFront();
        }
        void SetResultView()
        {
            resultListView.BackColor = controlBackColor;
            resultListView.ForeColor = ForeColor;
            resultListView.Size = new Size(Screen.GetWorkingArea(this).Size.Width - orderGroupBox.Location.X - orderGroupBox.Size.Width - 20, mainTabControl.Size.Height);
            resultListView.Location = new Point(orderGroupBox.Location.X + orderGroupBox.Size.Width + 10, mainTabControl.Location.Y);

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

            var headerstyle = new HeaderFormatStyle();
            headerstyle.SetBackColor(BackColor);
            headerstyle.SetForeColor(ForeColor);
            var numberColumn = new OLVColumn("No.", "Number");
            numberColumn.FreeSpaceProportion = 1;
            numberColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(numberColumn);
            var entryColumn = new OLVColumn("EG(%), TD(s)", "EntryGapAndTime");
            entryColumn.FreeSpaceProportion = 3;
            entryColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(entryColumn);
            var lastColumn = new OLVColumn("LG(%), TD(s)", "LastGapAndTime");
            lastColumn.FreeSpaceProportion = 3;
            lastColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(lastColumn);
            var profitColumn = new OLVColumn("PR(%), RP($)", "ProfitRateAndValue");
            profitColumn.FreeSpaceProportion = 3;
            profitColumn.HeaderFormatStyle = headerstyle;
            resultListView.AllColumns.Add(profitColumn);
            resultListView.Columns.AddRange(new ColumnHeader[] { numberColumn, entryColumn, lastColumn, profitColumn });
            resultListView.FormatRow += (sender, e) =>
            {
                if (((ResultData)e.Model).Profit > 0.1m)
                    e.Item.ForeColor = Color.Gold;
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
            var nameColumnSize = 7;
            var nameColumn = new OLVColumn("Name", "Name");
            nameColumn.FreeSpaceProportion = nameColumnSize;
            nameColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(nameColumn);
            var flucColumnSize = 3;
            var flucColumn = new OLVColumn("Fluc", "FlucBfor");
            flucColumn.FreeSpaceProportion = flucColumnSize;
            flucColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(flucColumn);
            var countColumnSize = 3;
            var countColumn = new OLVColumn("Count", "CountBfor");
            countColumn.FreeSpaceProportion = countColumnSize;
            countColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(countColumn);
            var durColumnSize = 2;
            var durColumn = new OLVColumn("D", "AggReadyRow");
            durColumn.FreeSpaceProportion = durColumnSize;
            durColumn.HeaderFormatStyle = headerstyle;
            FUListView.AllColumns.Add(durColumn);
            FUListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, countColumn, durColumn });
            FUListView.SelectionChanged += (sender, e) => { if (FUListView.SelectedIndices.Count == 1) ShowChart(FUListView.SelectedObject as ItemData); };
            FUListView.FormatRow += (sender, e) =>
            {
                if (((ItemData)e.Model).isAggOn)
                {
                    var itemData = (ItemData)e.Model;

                    if (itemData.LorS)
                        e.Item.ForeColor = plsPrcColor;
                    else
                        e.Item.ForeColor = mnsPrcColor;
                }
            };

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

        #region LoadResultEffect vars
        int numberOfFrames = 0;
        int currentFrame = 0;
        Image fail = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\fail0\fail0.gif");
        Image win0 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win0\win0.gif");
        Image win1 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win1\win1.gif");
        Image win2 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win2\win2.gif");
        Image win3 = Image.FromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win3\win3.gif");

        static ISoundEngine soundEngine = new ISoundEngine() { SoundVolume = 0.2f };
        ISound currentlyPlayingSound;
        ISoundSource failSound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\fail0\fail0.ogg");
        ISoundSource win0Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win0\win0.ogg");
        ISoundSource win1Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win1\win1.ogg");
        ISoundSource win2Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win2\win2.ogg");
        ISoundSource win3Sound = soundEngine.AddSoundSourceFromFile(@"C:\Users\tmdwn\source\repos\BinanceHand\rsc\win3\win3.ogg");
        #endregion
        void LoadResultEffect(decimal profit)
        {
            if (profit <= 0.1m)
            {
                pictureBox.Image = fail;
                currentlyPlayingSound = soundEngine.Play2D(failSound, false, false, false);
            }
            else if (profit <= 0.4m)
            {
                pictureBox.Image = win0;
                currentlyPlayingSound = soundEngine.Play2D(win0Sound, false, false, false);
            }
            else if (profit <= 0.7m)
            {
                pictureBox.Image = win1;
                currentlyPlayingSound = soundEngine.Play2D(win1Sound, false, false, false);
            }
            else if (profit <= 1.0m)
            {
                pictureBox.Image = win2;
                currentlyPlayingSound = soundEngine.Play2D(win2Sound, false, false, false);
            }
            else
            {
                pictureBox.Image = win3;
                currentlyPlayingSound = soundEngine.Play2D(win3Sound, false, false, false);
            }

            var dimension = new FrameDimension(pictureBox.Image.FrameDimensionsList[0]);
            currentFrame = 0;
            numberOfFrames = pictureBox.Image.GetFrameCount(dimension);
            pictureBox.Image.SelectActiveFrame(dimension, 0);
            pictureBox.Visible = true;
            pictureBox.Enabled = true;
        }

        void PlaceOrder(OrderSide orderSide)
        {
            if (!decimal.TryParse(orderPriceTextBox1.Text, out decimal priceRate) 
                || !decimal.TryParse(orderSizeTextBox1.Text, out decimal size) || size <= 0 
                || !int.TryParse(autoSizeTextBox0.Text, out int limitPercent) || limitPercent <= 0)
            {
                MessageBox.Show("input error");
                orderPriceTextBox1.Clear();
                return;
            }

            var stickNow = itemDataShowing.secStick;
            if (stickNow.Time == default)
                stickNow = itemDataShowing.minStick;

            var orderType = OrderType.Limit;
            TimeInForce? timeInForce = null;
            decimal? price = null;
            decimal quantity = 0;

            if (!marketRadioButton.Checked)
            {
                if (orderSide == OrderSide.Buy)
                    price = (int)(stickNow.Price[3] * (1 + priceRate / 100) / itemDataShowing.priceTickSize) * itemDataShowing.priceTickSize;
                else
                    price = (int)(stickNow.Price[3] * (1 - priceRate / 100) / itemDataShowing.priceTickSize) * itemDataShowing.priceTickSize;

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
                    var stick = itemDataShowing.secStickList[itemDataShowing.secStickList.Count - 1];

                    if (stick.Price[0] / stick.Price[1] > 1.001m)
                        quantity = (int)((stick.Ms + stick.Md) / 2 / 10 / (stick.Price[0] / stick.Price[1] - 1) / 1000 / itemDataShowing.minSize) * itemDataShowing.minSize;
                    else
                        quantity = (int)((stick.Ms + stick.Md) / 2 / 10 / itemDataShowing.minSize) * itemDataShowing.minSize;

                    var limitAmount = (int)(FUAvailableBalance * limitPercent / 100 / ((stick.Price[0] + stick.Price[1]) / 2) / itemDataShowing.minSize) * itemDataShowing.minSize;

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
            itemDataShowing.orderStartClosePrice = stickNow.Price[3];
            var a = client.FuturesUsdt.Order.PlaceOrder(
                itemDataShowing.Name
                , orderSide
                , orderType
                , quantity
                , PositionSide.Both
                , timeInForce
                , ROCheckBox.Checked
                , price);

            if (!a.Success)
            {
                MessageBox.Show("order fail" + a.Error);
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
            }
            else
            {
                var resultData = new ResultData(resultListView.Items.Count + 1);
                resultData.EntryTime = DateTime.UtcNow;
                resultListView.InsertObjects(0, new List<ResultData> { resultData });
            }
        }
        void CancelOrder(ItemData itemData)
        {
            client.FuturesUsdt.Order.CancelOrder(itemData.Name, itemData.orderID, itemData.clientOrderID);
        }

        void FirstSetOrderView()
        {
            leverageTextBox0.Text = itemDataShowing.Leverage.ToString();

            TickAutoSizeButton(true);

            if (itemDataShowing.position)
            {
                TickMinSizeButton(false);
                marketRadioButton.Checked = true;
                ROCheckBox.Checked = true;
            }
            else
            {
                TickMinSizeButton(true);
                GTCRadioButton.Checked = true;
                ROCheckBox.Checked = false;
            }

            orderPriceTextBox1.Text = "0.1";
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

            nameTextBox.Text = itemData.Name;

            for (int i = 0; i < secChart.Series.Count; i++)
            {
                secChart.Series[i].Points.Clear();
                minChart.Series[i].Points.Clear();
                hourChart.Series[i].Points.Clear();
                dayChart.Series[i].Points.Clear();
            }

            if (itemData.isAggOn || onlyAgg)
            {
                for (int i = 0; i < itemData.secStickList.Count; i++)
                    AddFullChartPoint(secChart, itemData.secStickList[i]);
                if (itemData.secStick.Time != default)
                    AddFullChartPoint(secChart, itemData.secStick);
                FirstZoomIfNeeded(secChart);
            }
            else
                SetAggOn(itemData, true);

            for (int i = 0; i < itemData.minStickList.Count; i++)
                AddFullChartPoint(minChart, itemData.minStickList[i]);
            if (itemData.minStick.Time != default)
                AddFullChartPoint(minChart, itemData.minStick);
            FirstZoomIfNeeded(minChart);

            for (int i = 0; i < itemData.hourStickList.Count; i++)
                AddFullChartPoint(hourChart, itemData.hourStickList[i]);
            if (itemData.hourStick.Time != default)
                AddFullChartPoint(hourChart, itemData.hourStick);
            FirstZoomIfNeeded(hourChart);

            for (int i = 0; i < itemData.dayStickList.Count; i++)
                AddFullChartPoint(dayChart, itemData.dayStickList[i]);
            if (itemData.dayStick.Time != default)
                AddFullChartPoint(dayChart, itemData.dayStick);
            FirstZoomIfNeeded(dayChart);

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

            FirstSetOrderView();

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
                    LoadMore(chart, 300);
                else if (chart.TabIndex != secChart.TabIndex && chart.TabIndex != minChart.TabIndex
                    && scrollType == ScrollType.SmallIncrement && chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= chart.Series[0].Points.Count)
                    LoadMore(chart, 0);

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
                if (itemDataShowing.minStickList.Count <= baseChartViewSticksSize)
                    LoadMore(chartNow, 300);

                minButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == hourChart.TabIndex)
            {
                if (itemDataShowing.hourStickList.Count <= baseChartViewSticksSize)
                    LoadMore(chartNow, 300);
                else if (DateTime.UtcNow.Subtract(itemDataShowing.hourStickList.Last().Time).TotalHours > 1)
                    LoadMore(chartNow, 0);

                hourButton.BackColor = buttonSelectedColor;
            }
            else if (chart.TabIndex == dayChart.TabIndex)
            {
                if (itemDataShowing.dayStickList.Count <= baseChartViewSticksSize)
                    LoadMore(chartNow, 300);
                else if (DateTime.UtcNow.Subtract(itemDataShowing.dayStickList.Last().Time).TotalDays > 1)
                    LoadMore(chartNow, 0);

                dayButton.BackColor = buttonSelectedColor;
            }
            else
                secButton.BackColor = buttonSelectedColor;

            AdjustChart(chart);
        }
        void LoadMore(Chart chart, int limit, DateTime now = default)
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

            if (double.IsNaN(chart.ChartAreas[0].AxisX.ScaleView.ViewMinimum) || double.IsNaN(chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum))
            {
                start = 1;
                end = list.Count;
            }

            if (end - start + 1 < baseChartViewSticksSize)
                start = end - baseChartViewSticksSize + 1;

            DateTime? endTime = null;
            bool addAll = false;
            if (list.Count == 0)
                endTime = DateTime.UtcNow;
            else if (chart.TabIndex != secChart.TabIndex)
            {
                endTime = list[0].Time;
                list.RemoveAt(0);
                foreach (var se in chart.Series)
                    se.Points.RemoveAt(0);
                addAll = true;
            }

            IEnumerable<IBinanceKline> klines;
            List<Stick> newList = new List<Stick>();
            Stick newStick;

            if (limit > 0)
            {
                if (chart.TabIndex != secChart.TabIndex)
                {
                    klines = client.FuturesUsdt.Market.GetKlines(itemDataShowing.Name, interval, null, endTime, limit).Data;

                    foreach (var kline in klines.Reverse())
                    {
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
                        if (kline.OpenTime == stick.Time || (!addAll && stick.Time == default))
                        {
                            if (stick.Time != default)
                                foreach (var se in chart.Series)
                                    se.Points.RemoveAt(se.Points.Count - 1);
                            stick = newStick;
                        }
                        else
                            newList.Insert(0, newStick);
                    }
                }
                else
                {
                    if (limit > itemDataShowing.oldSecStickList.Count)
                        limit = itemDataShowing.oldSecStickList.Count;

                    for (int i = limit - 1; i >= 0; i--)
                    {
                        InsertFullChartPoint(0, chart, itemDataShowing.oldSecStickList[i]);
                        newList.Insert(0, itemDataShowing.oldSecStickList[i]);
                        itemDataShowing.oldSecStickList.RemoveAt(i);
                    }
                }

                start += newList.Count;
                end += newList.Count;
            }

            if (addAll && chart.TabIndex != secChart.TabIndex)
            {
                for (int i = 0; i < list.Count; i++)
                    newList.Add(list[i]);

                if (now == default || newList[newList.Count - 1].Time >= now)
                    klines = client.FuturesUsdt.Market.GetKlines(itemDataShowing.Name, interval, newList[newList.Count - 1].Time, null, null).Data;
                else
                    klines = client.FuturesUsdt.Market.GetKlines(itemDataShowing.Name, interval, newList[newList.Count - 1].Time, now, null).Data;

                int count = klines.Count();
                short j = 0;
                foreach (var kline in klines)
                {
                    j++;

                    if (newList[newList.Count - 1].Time >= kline.OpenTime)
                        continue;

                    if (kline.OpenTime == stick.Time)
                        foreach (var se in chart.Series)
                            se.Points.RemoveAt(se.Points.Count - 1);

                    newStick = new Stick();
                    newStick.Price[0] = kline.High;
                    newStick.Price[1] = kline.Low;
                    newStick.Price[2] = kline.Open;
                    newStick.Price[3] = kline.Close;
                    newStick.Ms = kline.TakerBuyBaseVolume;
                    newStick.Md = kline.BaseVolume - kline.TakerBuyBaseVolume;
                    newStick.Time = kline.OpenTime;
                    newStick.TCount = kline.TradeCount;
                    AddFullChartPoint(chart, newStick);
                    if (j != count)
                        newList.Add(newStick);
                    else
                        stick = newStick;
                }
            }

            list = newList;

            if (chart.TabIndex == minChart.TabIndex)
            {
                newStick = newList.Last();
                if (newStick.Price[0] != newStick.Price[1])
                    autoTextBox.Text =
                        Math.Round(
                            (newStick.Ms + newStick.Md) / 2
                            / ((newStick.Price[2] + 2 * (newStick.Price[0] - newStick.Price[1]) - newStick.Price[3])
                                / newStick.Price[3] * 1000) * newStick.Price[3]
                            , 0).ToString();
                else
                    autoTextBox.Text = "0000";
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

        #region Form1_Load vars
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
        #endregion
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

            if (!onlyAgg)
            {
                socketClient.FuturesUsdt.SubscribeToKlineUpdates(FUSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, FUItemDataList[data.Symbol]); });
                FUKlineReqTextBox.Text = "/" + FUSymbolList.Count + "(K)";
            }
            else
            {
                socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(FUSymbolList, data => { BeginInvoke(aggUpdates, data, FUItemDataList[data.Symbol]); });
                //socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(FUSymbolList.GetRange(50, FUSymbolList.Count - 50), data => { BeginInvoke(aggUpdates, data, FUItemDataList[data.Symbol]); });
                FUAggReqTextBox.Text = "/" + FUSymbolList.Count + "(A)";
            }
        }
        #region SetItemDataList vars
        List<string> FUSymbolList = new List<string>();
        #endregion
        void SetItemDataList()
        {
            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
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

        #region OnKlineUpdates vars
        short FUAggOnNow = 0;
        short FUKlineRcv = 0;
        #endregion
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
                if (data.Data.High + data.Data.Low < data.Data.Close * 2)
                    itemData.FlucBfor = Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                else
                    itemData.FlucBfor = -Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);

                itemData.CountBfor = itemData.minStick.TCount;

                if (Math.Abs(itemData.FlucBfor) > 1)
                {
                    if (!itemData.isAggReady && data.Data.TradeCount > 180)
                    {
                        if (itemData.FlucBfor > 1)
                            itemData.LorS = true;
                        else
                            itemData.LorS = false;

                        itemData.isAggReady = true;
                        itemData.AggReadyRow = 0;
                        FUListView.AddObject(itemData);
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
                    itemData.AggFirst = true;

                    FUListView.RemoveObject(itemData);
                    FUAggOnNow--;
                    FUAggRcv--;
                    FUAggReq--;
                    FUAggRcvTextBox.Text = FUAggRcv.ToString();
                    FUAggReqTextBox.Text = FUAggReq.ToString();
                }

                if (itemData.isAggReady)
                    FUListView.RefreshObject(itemData);

                if (itemData.isAggReady && !itemData.isAggOn && FUAggOnNow < 5)
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
                    if (itemData.minStickList.Count > 1 && itemData.minStickList[itemData.minStickList.Count - 1].Time.AddMinutes(-1) != itemData.minStickList[itemData.minStickList.Count - 2].Time)
                    {
                        itemData.minStickList.RemoveAt(itemData.minStickList.Count - 1);
                        LoadMore(minChart, 0, data.Data.OpenTime);
                        itemData.minStickList.Add(itemData.minStick);
                    }

                    if (minChart.Series[0].Points.Count != 0)
                        UpdateChartPoint(minChart, itemData.minStick);
                    else
                        AddFullChartPoint(minChart, itemData.minStick);

                    AdjustChart(minChart);
                }

                itemData.minStick = new Stick();
            }
            else if (itemData.isChartShowing)
            {
                if (itemData.minStick.Time != data.Data.OpenTime)
                {
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
                itemData.minStick.TCount = data.Data.TradeCount;

                if (itemData.minStick.Time.ToString("HH:mm") != minChart.Series[0].Points[minChart.Series[0].Points.Count - 1].AxisLabel.Substring(3, 5))
                    AddStartChartPoint(minChart, itemData.minStick);

                UpdateChartPoint(minChart, itemData.minStick);
            }
        }
        #region OnAggregatedTradeUpdates vars
        short FUAggRcv = 0;
        #endregion
        void OnAggregatedTradeUpdates(BinanceStreamAggregatedTrade data, ItemData itemData)
        {
            if (!onlyAgg && (!itemData.isAggReady || !itemData.isAggOn))
                return;

            if (itemData.AggFirst)
            {
                itemData.AggFirst = false;

                FUAggRcv++;
                FUAggRcvTextBox.Text = FUAggRcv.ToString();
            }

            itemData.newTime = data.TradeTime.ToString(ItemData.secChartLabel);

            if (data.TradeTime.Second != itemData.secStick.Time.Second || data.TradeTime.Minute != itemData.secStick.Time.Minute || data.TradeTime.Hour != itemData.secStick.Time.Hour)
            {
                if (itemData.secStick.Price[1] != decimal.Zero)
                    itemData.secStickList.Add(itemData.secStick);

                itemData.secStick = new Stick();

                itemData.secStick.Price[2] = data.Price;
                itemData.secStick.Price[0] = data.Price;
                itemData.secStick.Price[1] = data.Price;
                itemData.secStick.Price[3] = data.Price;

                itemData.secStick.Time = data.TradeTime;

                if (itemData.isChartShowing)
                {
                    AddStartChartPoint(secChart, itemData.secStick);

                    if (secChart.Series[0].Points.Count == baseChartViewSticksSize + 1)
                    {
                        secChart.ChartAreas[0].RecalculateAxesScale();
                        secChart.ChartAreas[0].AxisX.ScaleView.Zoom(secChart.Series[0].Points.Count - baseChartViewSticksSize + 1, secChart.Series[0].Points.Count);
                        secChart.ChartAreas[0].RecalculateAxesScale();
                    }
                    if (secChart.Series[0].Points.Count != 1)
                        AdjustChart(secChart);
                }

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

                if (onlyAgg)
                {
                    if (data.TradeTime.Minute != itemData.minStick.Time.Minute || data.TradeTime.Hour != itemData.minStick.Time.Hour)
                    {
                        if (itemData.minStick.Price[1] != decimal.Zero)
                        {
                            itemData.minStickList.Add(itemData.minStick);
                            itemData.FlucBfor = Math.Round((itemData.minStick.Price[0] / itemData.minStick.Price[1] - 1) * 100, 1);
                            itemData.CountBfor = itemData.minStick.TCount;

                            if (itemData.FlucBfor > 1)
                            {
                                if (!itemData.isAggReady && itemData.minStick.TCount > 180)
                                {
                                    itemData.isAggReady = true;
                                    itemData.AggReadyRow = 0;
                                    if (!onlyAgg)
                                        FUListView.AddObject(itemData);
                                }
                                itemData.FlatMinRow = 0;
                                itemData.AggReadyRow++;
                            }
                            else if (itemData.isAggReady && itemData.FlatMinRow < 5)
                            {
                                itemData.FlatMinRow++;
                                itemData.AggReadyRow++;
                            }

                            FUListView.RefreshObject(itemData);

                            if (itemData.isChartShowing && itemData.minStick.Price[0] != itemData.minStick.Price[1])
                                autoTextBox.Text = 
                                    Math.Round(
                                        (itemData.minStick.Ms + itemData.minStick.Md) / 2 
                                        / ((itemData.minStick.Price[2] + 2 * (itemData.minStick.Price[0] - itemData.minStick.Price[1]) - itemData.minStick.Price[3]) 
                                            / itemData.minStick.Price[3] * 1000) * itemData.minStick.Price[3]
                                        , 0).ToString();
                        }

                        itemData.minStick = new Stick();

                        itemData.minStick.Price[2] = data.Price;
                        itemData.minStick.Price[0] = data.Price;
                        itemData.minStick.Price[1] = data.Price;
                        itemData.minStick.Price[3] = data.Price;

                        itemData.minStick.Time = DateTime.Parse(data.TradeTime.ToString("yyyy-MM-dd HH:mm"));

                        if (itemData.isChartShowing)
                        {
                            AddStartChartPoint(minChart, itemData.minStick);

                            if (minChart.Series[0].Points.Count == baseChartViewSticksSize + 1)
                            {
                                minChart.ChartAreas[0].RecalculateAxesScale();
                                minChart.ChartAreas[0].AxisX.ScaleView.Zoom(minChart.Series[0].Points.Count - baseChartViewSticksSize + 1, minChart.Series[0].Points.Count);
                                minChart.ChartAreas[0].RecalculateAxesScale();
                                minChart.ChartAreas[1].RecalculateAxesScale();
                            }
                            if (minChart.Series[0].Points.Count != 1)
                                AdjustChart(minChart);
                        }
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
            if (onlyAgg)
            {
                if (data.Price > itemData.minStick.Price[0])
                    itemData.minStick.Price[0] = data.Price;
                else if (data.Price < itemData.minStick.Price[1])
                    itemData.minStick.Price[1] = data.Price;
                itemData.minStick.Price[3] = data.Price;
                if (data.BuyerIsMaker)
                    itemData.minStick.Md += data.Quantity;
                else
                    itemData.minStick.Ms += data.Quantity;
                itemData.minStick.TCount++;
            }

            if (itemData.isChartShowing)
            {
                timeDiffTextBox.Text = Math.Round((DateTime.UtcNow - data.TradeTime).TotalSeconds, 1).ToString();
                amtTextBox.Text = data.Quantity.ToString(ForDecimalString);
                priceTextBox.Text = data.Price.ToString(ForDecimalString);
                if (data.BuyerIsMaker)
                    amtTextBox.ForeColor = mnsPrcColor;
                else
                    amtTextBox.ForeColor = plsPrcColor;
                priceTextBox.ForeColor = amtTextBox.ForeColor;
                timeDiffTextBox.ForeColor = amtTextBox.ForeColor;

                UpdateChartPoint(secChart, itemData.secStick);
                UpdateChartPoint(minChart, itemData.minStick);

                if (itemData.position)
                {
                    if (data.Price > itemData.EntryPrice)
                    {
                        chartNow.ChartAreas[0].AxisY2.StripLines[0].StripWidth = (double)(data.Price - itemData.EntryPrice);
                        chartNow.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)itemData.EntryPrice - chartNow.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        if (itemData.LorS)
                        {
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].BackColor = earningColor;
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((data.Price / itemData.EntryPrice - 1) * 100, 2).ToString();
                        }    
                        else
                        {
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].BackColor = losingColor;
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((itemData.EntryPrice / data.Price - 1) * 100, 2).ToString();
                        }
                    }
                    else
                    {
                        chartNow.ChartAreas[0].AxisY2.StripLines[0].StripWidth = (double)(itemData.EntryPrice - data.Price);
                        chartNow.ChartAreas[0].AxisY2.StripLines[0].IntervalOffset = (double)data.Price - chartNow.ChartAreas[0].AxisY2.ScaleView.ViewMinimum;
                        if (itemData.LorS)
                        {
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].BackColor = losingColor;
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((data.Price / itemData.EntryPrice - 1) * 100, 2).ToString();
                        }
                        else
                        {
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].BackColor = earningColor;
                            chartNow.ChartAreas[0].AxisY2.StripLines[0].Text = Math.Round((itemData.EntryPrice / data.Price - 1) * 100, 2).ToString();
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

                        FirstSetOrderView();

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

                        FirstSetOrderView();

                        itemData.markSub = socketClient.FuturesUsdt.SubscribeToMarkPriceUpdates(itemData.Name, 3000,
                            data2 => { BeginInvoke(markUpdates, data2, itemData); }).Data;

                        FUPositionListView.AddObject(itemData);
                        FUPositionListView.RefreshObject(itemData);
                        FUPositionTab.Text = "Position(" + FUPositionListView.Items.Count + ")";
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
                if (data.UpdateData.Status == OrderStatus.Filled)
                {
                    if (!itemData.position)
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
                        resultData.LastGapAndTime = resultData.LastGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.LastTime).TotalSeconds, 1);
                        resultListView.RefreshObject(resultData);

                        LoadResultEffect(resultData.Profit);

                        chartNow.ChartAreas[0].AxisY2.StripLines.Clear();
                    }
                    else
                    {
                        var resultData = resultListView.GetModelObject(0) as ResultData;
                        if (data.UpdateData.Side == OrderSide.Buy)
                            resultData.EntryGap = Math.Round((data.UpdateData.AveragePrice / itemData.orderStartClosePrice - 1) * 100, 2);
                        else
                            resultData.EntryGap = Math.Round((itemData.orderStartClosePrice / data.UpdateData.AveragePrice - 1) * 100, 2);
                        resultData.EntryGapAndTime = resultData.EntryGap + ", " + Math.Round((data.UpdateData.CreateTime - resultData.EntryTime).TotalSeconds, 1);
                        resultListView.RefreshObject(resultData);

                        var strip = new StripLine();
                        strip.Interval = double.MaxValue;
                        strip.ForeColor = ForeColor;
                        strip.TextLineAlignment = StringAlignment.Center;
                        chartNow.ChartAreas[0].AxisY2.StripLines.Add(strip);
                    }
                }
                else
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

        #region SetAggOn vars
        short FUAggReq = 0;
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
            itemData.AMandGM = "0.00(0)";

            itemData.secStickList.Clear();
            itemData.secStick = new Stick();

            itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, FUItemDataList[data2.Symbol]); }).Data;
            if (addObject)
            {
                FUListView.AddObject(itemData);
                FUListView.RefreshObject(itemData);
            }
            FUListView.RemoveObject(itemData);
            FUListView.RefreshObject(itemData);
            FUListView.InsertObjects(0, new List<ItemData> { itemData });
            FUListView.RefreshObject(itemData);
            FUAggOnNow++;
            FUAggReq++;
            FUAggReqTextBox.Text = "/" + FUAggReq + "(A)";
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Z:
                    if (!nameTextBox.Focused && itemDataShowing != null)
                    {
                        PlaceOrder(OrderSide.Buy);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.C:
                    if (!nameTextBox.Focused && itemDataShowing != null)
                    {
                        PlaceOrder(OrderSide.Sell);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.A:
                    if (!nameTextBox.Focused && itemDataShowing != null)
                    {
                        ChartScroll(chartNow, ScrollType.SmallDecrement);
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.S:
                    if (!nameTextBox.Focused && itemDataShowing != null)
                    {
                        ChartScroll(chartNow, ScrollType.Last);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.D:
                    if (!nameTextBox.Focused && itemDataShowing != null)
                    {
                        ChartScroll(chartNow, ScrollType.SmallIncrement);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.Q:
                    if (!nameTextBox.Focused && itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMinimum - 9, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.W:
                    if (!nameTextBox.Focused && itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - baseChartViewSticksSize, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.E:
                    if (!nameTextBox.Focused && itemDataShowing != null && chartNow.ChartAreas[0].AxisX.ScaleView.IsZoomed)
                    {
                        chartNow.ChartAreas[0].AxisX.ScaleView.Zoom(chartNow.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 11, chartNow.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1);
                        AdjustChart(chartNow);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.D1:
                    if (!nameTextBox.Focused)
                    {
                        SetChartNowOrLoad(secChart);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.D2:
                    if (!nameTextBox.Focused)
                    {
                        SetChartNowOrLoad(minChart);
                        e.SuppressKeyPress = true;
                    }
                        e.Handled = true;
                    break;

                case Keys.D3:
                    if (!nameTextBox.Focused)
                    {
                        SetChartNowOrLoad(hourChart);
                        e.SuppressKeyPress = true;
                    }
                        e.Handled = true;
                    break;

                case Keys.D4:
                    if (!nameTextBox.Focused)
                    {
                        SetChartNowOrLoad(dayChart);
                        e.SuppressKeyPress = true;
                    }
                    e.Handled = true;
                    break;

                case Keys.Oemcomma:    //,
                    FUListView.Sort(FUListView.GetColumn("Name"), SortOrder.Ascending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.OemPeriod:    //.
                    FUListView.Sort(FUListView.GetColumn("Fluc"), SortOrder.Descending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.Oem2:     // /
                    FUListView.Sort(FUListView.GetColumn("Count"), SortOrder.Descending);
                    FUListView.Refresh();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    break;

                case Keys.ControlKey:
                    if (!nameTextBox.Focused)
                        nameTextBox.Focus();
                    else
                    {
                        orderGroupBox.Focus();
                    }

                    e.Handled = true;
                    break;

                case Keys.Enter:
                    if (nameTextBox.Focused)
                    {
                        if (!FUItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                        {
                            MessageBox.Show("No symbol");
                            return;
                        }
                        ShowChart(FUItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                        mainTabControl.Focus();
                        e.SuppressKeyPress = true;
                        e.Handled = true;
                    }
                    break;

                case Keys.Delete:
                    if (itemDataShowing != null && itemDataShowing.order)
                        CancelOrder(itemDataShowing);
                    e.Handled = true;
                    break;

                case Keys.D0:
                    if (FUPositionListView.Items.Count != 0)
                        FUPositionListView.SelectedIndex = 0;
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
        public string EntryGapAndTime;
        public decimal LastGap;
        public DateTime LastTime;
        public string LastGapAndTime;
        public decimal Profit;
        public string ProfitRateAndValue;

        public ResultData(int n)
        {
            Number = n;
        }
    }
}
