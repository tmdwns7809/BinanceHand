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

namespace BinanceHand
{
    public partial class Form1 : Form
    {
        //DBHelper dbHelper = DBHelper.GetInstance();

        Dictionary<string, ItemData> spotItemDataList = new Dictionary<string, ItemData>();
        Dictionary<string, ItemData> futureUItemDataList = new Dictionary<string, ItemData>();
        Dictionary<string, ItemData> futureCItemDataList = new Dictionary<string, ItemData>();

        static int amtAnPrcAlpha = 130;
        Color msAmtColor = Color.FromArgb(amtAnPrcAlpha, Color.Orange);
        Color mdAmtColor = Color.FromArgb(amtAnPrcAlpha, Color.ForestGreen);
        Color msAndMdAmtColor = Color.FromArgb(amtAnPrcAlpha, 145, 151, 17);
        Color plsPrcColor = Color.FromArgb(amtAnPrcAlpha, Color.Red);
        Color mnsPrcColor = Color.FromArgb(amtAnPrcAlpha, Color.Blue);

        static int gridAlpha = 90;
        Color gridColor = Color.FromArgb(gridAlpha, Color.Gray);

        public Form1()
        {
            InitializeComponent();

            SetClientAndKey();

            SetComponents();

            SetItemDataList();
        }

        BinanceClient client;
        BinanceSocketClient socketClient;
        Thread listenThread;
        void SetClientAndKey()
        {
            client = new BinanceClient(new BinanceClientOptions
            {
                ApiCredentials = new ApiCredentials("DpLZEH9lDIuoYvs5L9YcDshSXW1rN9Rlw6OMj7zVQwR7wuBGt9vtyntEDNUfUb50", "xXhj8yvUrLgVxlO4BioadC4RQSoydDLR2UFIaMcqBCdX1WM2LjVsM3cxC1Y9lgVJ"),
                //LogVerbosity = LogVerbosity.Debug,
                //LogWriters = new List<TextWriter> { Console.Out }
            });
            var spotStartResult = client.Spot.UserStream.StartUserStream();
            if (!spotStartResult.Success)
                throw new Exception($"Failed to start user stream: {spotStartResult.Error}");
            var listenKey = spotStartResult.Data;
            listenThread = new Thread(delegate ()
            {
                while (true)
                {
                    Thread.Sleep(55 * 60 * 1000);
                    client.Spot.UserStream.KeepAliveUserStream(listenKey);
                }
            });
            listenThread.Start();

            socketClient = new BinanceSocketClient(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials("DpLZEH9lDIuoYvs5L9YcDshSXW1rN9Rlw6OMj7zVQwR7wuBGt9vtyntEDNUfUb50", "xXhj8yvUrLgVxlO4BioadC4RQSoydDLR2UFIaMcqBCdX1WM2LjVsM3cxC1Y9lgVJ"),
                //LogVerbosity = LogVerbosity.Debug,
                //LogWriters = new List<TextWriter> { Console.Out }
            });
        }

        string startTime;
        void SetComponents()
        {
            startTime = DateTime.UtcNow.ToString();
            Text = "Binance       UTC-" + startTime;

            SetComponentsLocationAndSize();

            ChartSetting();

            var nameColumn = new OLVColumn("현물", "Name");
            var flucColumn = new OLVColumn("전분봉", "RateBfor");
            var durColumn = new OLVColumn("지속", "AggReadyRow");
            var winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            var proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = 5;
            flucColumn.FreeSpaceProportion = 2;
            durColumn.FreeSpaceProportion = 1;
            winColumn.FreeSpaceProportion = 3;
            proColumn.FreeSpaceProportion = 3;
            spotListView.AllColumns.Add(nameColumn);
            spotListView.AllColumns.Add(flucColumn);
            spotListView.AllColumns.Add(durColumn);
            spotListView.AllColumns.Add(winColumn);
            spotListView.AllColumns.Add(proColumn);
            spotListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });

            nameColumn = new OLVColumn("선물U", "Name");
            flucColumn = new OLVColumn("전분봉", "RateBfor");
            durColumn = new OLVColumn("지속", "AggReadyRow");
            winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = 5;
            flucColumn.FreeSpaceProportion = 2;
            durColumn.FreeSpaceProportion = 1;
            winColumn.FreeSpaceProportion = 3;
            proColumn.FreeSpaceProportion = 3;
            futureUListView.AllColumns.Add(nameColumn);
            futureUListView.AllColumns.Add(flucColumn);
            futureUListView.AllColumns.Add(durColumn);
            futureUListView.AllColumns.Add(winColumn);
            futureUListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });

            nameColumn = new OLVColumn("선물C", "Name");
            flucColumn = new OLVColumn("전분봉", "RateBfor");
            durColumn = new OLVColumn("지속", "AggReadyRow");
            winColumn = new OLVColumn("승률(횟수)", "WinPrecantage");
            proColumn = new OLVColumn("산술(기하)", "ProfitRateSumAndMul");
            nameColumn.FreeSpaceProportion = 5;
            flucColumn.FreeSpaceProportion = 2;
            durColumn.FreeSpaceProportion = 1;
            winColumn.FreeSpaceProportion = 3;
            proColumn.FreeSpaceProportion = 3;
            futureCListView.AllColumns.Add(nameColumn);
            futureCListView.AllColumns.Add(flucColumn);
            futureCListView.AllColumns.Add(durColumn);
            futureCListView.AllColumns.Add(winColumn);
            futureCListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn, durColumn, winColumn, proColumn });
        }
        void SetComponentsLocationAndSize()
        {
            WindowState = FormWindowState.Maximized;

            chartTabControl.Margin = new Padding(0);
            chartTabControl.Location = new Point(0, 0);
            chartTabControl.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.8), (int)(Screen.GetWorkingArea(this).Size.Height * 0.8));
            chartTabPageSec.Margin = chartTabControl.Margin;
            chartTabPageSec.Padding = chartTabControl.Margin;
            chartTabPageSec.Location = new Point(0, 0);
            chartTabPageSec.Size = new Size(chartTabControl.Size.Width, chartTabControl.Size.Height - chartTabControl.ItemSize.Height);
            chartTabPageMin.Margin = chartTabPageSec.Margin;
            chartTabPageMin.Padding = chartTabPageSec.Padding;
            chartTabPageMin.Location = chartTabPageSec.Location;
            chartTabPageMin.Size = chartTabPageSec.Size;

            chart1.Margin = chartTabControl.Margin;
            chart1.Padding = chartTabControl.Margin;
            chart1.Location = new Point(0, 0);
            chart1.Size = new Size(chartTabPageSec.Size.Width, chartTabPageSec.Size.Height);
            chart2.Margin = chart1.Margin;
            chart2.Padding = chart1.Padding;
            chart2.Location = chart1.Location;
            chart2.Size = chart1.Size;

            rcvTimeTextBox.Margin = chartTabControl.Margin;
            rcvTimeTextBox.Location = new Point(chartTabControl.Location.X + chartTabControl.Size.Width - 50, chartTabControl.Location.Y + chartTabControl.Size.Height - 50);
            realTimeTextBox.Margin = chartTabControl.Margin;
            realTimeTextBox.Location = new Point(rcvTimeTextBox.Location.X, rcvTimeTextBox.Location.Y + rcvTimeTextBox.Size.Height);
            timeDiffTextBox.Margin = chartTabControl.Margin;
            timeDiffTextBox.Location = new Point(realTimeTextBox.Location.X + realTimeTextBox.Size.Width, realTimeTextBox.Location.Y - timeDiffTextBox.Size.Height / 2);
            priceTextBox.Margin = chartTabControl.Margin;
            priceTextBox.Location = new Point(rcvTimeTextBox.Location.X - priceTextBox.Size.Width, rcvTimeTextBox.Location.Y);
            amtTextBox.Margin = chartTabControl.Margin;
            amtTextBox.Location = new Point(priceTextBox.Location.X, priceTextBox.Location.Y + priceTextBox.Size.Height);
            nameTextBox.Margin = chartTabControl.Margin;
            nameTextBox.Location = new Point(amtTextBox.Location.X - nameTextBox.Size.Width, amtTextBox.Location.Y - nameTextBox.Size.Height / 2);
            marketComboBox.Margin = chartTabControl.Margin;
            marketComboBox.Size = new Size(50, nameTextBox.Size.Height);
            marketComboBox.Location = new Point(nameTextBox.Location.X - marketComboBox.Size.Width, nameTextBox.Location.Y);

            logTabControl.Margin = chartTabControl.Margin;
            logTabControl.Location = new Point(chartTabControl.Location.X, chartTabControl.Location.Y + chartTabControl.Size.Height);
            logTabControl.Size = new Size((int)(Screen.GetWorkingArea(this).Size.Width * 0.5), Screen.GetWorkingArea(this).Size.Height - chartTabControl.Size.Height);
            logTabPage.Margin = chartTabControl.Margin;
            logTabPage.Padding = chartTabControl.Margin;
            logTabPage.Location = new Point(0, 0);
            logTabPage.Size = new Size(logTabControl.Size.Width, logTabControl.Size.Height - logTabControl.ItemSize.Height);
            logListBox.Margin = chartTabControl.Margin;
            logListBox.Padding = chartTabControl.Margin;
            logListBox.Location = new Point(0, 0);
            logListBox.Size = new Size(logTabPage.Size.Width, logTabPage.Size.Height);

            spotListView.Margin = chartTabControl.Margin;
            spotListView.Location = new Point(logTabControl.Location.X + logTabControl.Size.Width, logTabControl.Location.Y);
            spotListView.Size = new Size((Screen.GetWorkingArea(this).Size.Width - logTabControl.Size.Width) / 3, logTabControl.Size.Height - spotKlineRcvTextBox.Size.Height);
            spotKlineRcvTextBox.Margin = chartTabControl.Margin;
            spotKlineRcvTextBox.Location = new Point(spotListView.Location.X, spotListView.Location.Y + spotListView.Size.Height);
            spotKlineReqTextBox.Margin = chartTabControl.Margin;
            spotKlineReqTextBox.Location = new Point(spotKlineRcvTextBox.Location.X + spotKlineRcvTextBox.Size.Width, spotKlineRcvTextBox.Location.Y);
            spotAggRcvTextBox.Margin = chartTabControl.Margin;
            spotAggRcvTextBox.Location = new Point(spotKlineReqTextBox.Location.X + spotKlineReqTextBox.Size.Width + spotKlineRcvTextBox.Size.Width, spotKlineReqTextBox.Location.Y);
            spotAggReqTextBox.Margin = chartTabControl.Margin;
            spotAggReqTextBox.Location = new Point(spotAggRcvTextBox.Location.X + spotAggRcvTextBox.Size.Width, spotAggRcvTextBox.Location.Y);

            futureUListView.Margin = chartTabControl.Margin;
            futureUListView.Location = new Point(spotListView.Location.X + spotListView.Size.Width, spotListView.Location.Y);
            futureUListView.Size = new Size(spotListView.Size.Width, spotListView.Size.Height);
            futureUKlineRcvTextBox.Margin = chartTabControl.Margin;
            futureUKlineRcvTextBox.Location = new Point(futureUListView.Location.X, futureUListView.Location.Y + futureUListView.Size.Height);
            futureUKlineReqTextBox.Margin = chartTabControl.Margin;
            futureUKlineReqTextBox.Location = new Point(futureUKlineRcvTextBox.Location.X + futureUKlineRcvTextBox.Size.Width, futureUKlineRcvTextBox.Location.Y);
            futureUAggRcvTextBox.Margin = chartTabControl.Margin;
            futureUAggRcvTextBox.Location = new Point(futureUKlineReqTextBox.Location.X + futureUKlineReqTextBox.Size.Width + futureUKlineRcvTextBox.Size.Width, futureUKlineReqTextBox.Location.Y);
            futureUAggReqTextBox.Margin = chartTabControl.Margin;
            futureUAggReqTextBox.Location = new Point(futureUAggRcvTextBox.Location.X + futureUAggRcvTextBox.Size.Width, futureUAggRcvTextBox.Location.Y);

            futureCListView.Margin = chartTabControl.Margin;
            futureCListView.Location = new Point(futureUListView.Location.X + futureUListView.Size.Width, futureUListView.Location.Y);
            futureCListView.Size = new Size(spotListView.Size.Width, spotListView.Size.Height);
            futureCKlineRcvTextBox.Margin = chartTabControl.Margin;
            futureCKlineRcvTextBox.Location = new Point(futureCListView.Location.X, futureCListView.Location.Y + futureCListView.Size.Height);
            futureCKlineReqTextBox.Margin = chartTabControl.Margin;
            futureCKlineReqTextBox.Location = new Point(futureCKlineRcvTextBox.Location.X + futureCKlineRcvTextBox.Size.Width, futureCKlineRcvTextBox.Location.Y);
            futureCAggRcvTextBox.Margin = chartTabControl.Margin;
            futureCAggRcvTextBox.Location = new Point(futureCKlineReqTextBox.Location.X + futureCKlineReqTextBox.Size.Width + futureCKlineRcvTextBox.Size.Width, futureCKlineReqTextBox.Location.Y);
            futureCAggReqTextBox.Margin = chartTabControl.Margin;
            futureCAggReqTextBox.Location = new Point(futureCAggRcvTextBox.Location.X + futureCAggRcvTextBox.Size.Width, futureCAggRcvTextBox.Location.Y);
        }
        private void nameTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (marketComboBox.SelectedItem.ToString())
            {
                case "S":
                    if (!spotItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(spotItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                case "F_U":
                    if (!futureUItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(futureUItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                case "F_C":
                    if (!futureCItemDataList.ContainsKey(nameTextBox.Text.Trim().ToUpper()))
                    {
                        MessageBox.Show("No symbol");
                        return;
                    }
                    ShowChart(futureCItemDataList[nameTextBox.Text.Trim().ToUpper()]);
                    break;

                default:
                    break;
            }
        }
        ItemData itemDataShowing;
        void ListView_SelectionChanged(object sender, EventArgs e)
        {
            if (((FastDataListView)sender).SelectedIndices.Count != 1)
                return;

            ShowChart(((FastDataListView)sender).SelectedObject as ItemData);
        }
        int baseChartViewSticksSize = 120;
        void ShowChart(ItemData itemData)
        {
            if (itemData.isChartShowing)
                return;

            if (itemDataShowing != null && itemDataShowing.isChartShowing)
                itemDataShowing.isChartShowing = false;
            itemDataShowing = itemData;

            itemData.isChartShowing = true;

            if (itemData.isSpot)
                Text = itemData.Name + "-Spot      Binance       UTC-" + startTime;
            else if (itemData.isFutureUsdt)
                Text = itemData.Name + "-Future(Usdt)      Binance       UTC-" + startTime;
            else
                Text = itemData.Name + "-Future(Coin)      Binance       UTC-" + startTime;

            nameTextBox.Text = itemData.Name;

            foreach (var se in chart1.Series)
                se.Points.Clear();
            foreach (var se in chart2.Series)
                se.Points.Clear();

            Stick stick;
            for (int i = 0; i < itemData.minStickList.Count; i++)
            {
                stick = itemData.minStickList[i];
                AddFullChartPoint(chart2, ItemData.minChartLabel, stick);
            }
            if (itemData.minStickList.Count <= baseChartViewSticksSize)
                LoadMinutesMore(itemData);

            if (itemData.isAggOn)
            {
                for (int i = 0; i < itemData.secStickList.Count; i++)
                {
                    stick = itemData.secStickList[i];
                    AddFullChartPoint(chart1, ItemData.secChartLabel, stick);
                }

                AddFullChartPoint(chart1, ItemData.secChartLabel, itemData.secStick);

                if (chart1.Series[0].Points.Count > baseChartViewSticksSize)
                {
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count - baseChartViewSticksSize + 1, chart1.Series[0].Points.Count);
                    chart1.ChartAreas[0].RecalculateAxesScale();
                    AdjustChart(chart1);
                }

                chart1.ChartAreas[0].AxisY.MajorGrid.Interval = chart1.Series[0].Points.Last().YValues[0] * 0.005;
            }
            else
                SetAggOn(itemData, true);
        }
        void ChartSetting()
        {
            chart1.AxisViewChanged += Chart1_AxisViewChanged;

            var chartAreaMain = chart1.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMain.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisX.MajorGrid.Interval = 10;
            chartAreaMain.AxisX.MajorTickMark.Size = 0.4f;
            chartAreaMain.AxisX.LabelStyle.Interval = 60;

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


            chart2.AxisViewChanged += Chart2_AxisViewChanged;

            chartAreaMain = chart2.ChartAreas.Add("ChartAreaMain");
            chartAreaMain.AxisX.ScaleView.SizeType = DateTimeIntervalType.Seconds;
            chartAreaMain.AxisX.ScaleView.Small​Scroll​Size = 1;
            chartAreaMain.AxisX.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisX.MajorGrid.Interval = 10;
            chartAreaMain.AxisX.MajorTickMark.Size = 0.4f;
            chartAreaMain.AxisX.LabelStyle.Interval = 60;

            chartAreaMain.AxisY.MajorGrid.LineColor = gridColor;
            chartAreaMain.AxisY.ScrollBar.Enabled = false;
            chartAreaMain.AxisY.MajorTickMark.Enabled = false;
            chartAreaMain.AxisY.IsStartedFromZero = false;
            chartAreaMain.AxisY.LabelStyle.Enabled = false;

            chartAreaMain.Position = new ElementPosition(0, 0, 100, 100);


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
        void Chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            AdjustChart(chart1);
        }
        void Chart2_AxisViewChanged(object sender, ViewEventArgs e)
        {
            if ((int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum == 0)
                LoadMinutesMore(itemDataShowing);
            AdjustChart(chart2);
        }
        void LoadMinutesMore(ItemData itemData)
        {
            var start = (int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMinimum + 1;
            var end = (int)chart2.ChartAreas[0].AxisX.ScaleView.ViewMaximum - 1;

            if (end - start + 1 <= baseChartViewSticksSize)
                start = end - baseChartViewSticksSize + 1;

            DateTime endTime;
            if (itemData.minStickList.Count == 0)
                endTime = DateTime.UtcNow;
            else
                endTime = itemData.minStickList[0].Time.AddMinutes(-1);

            IEnumerable<IBinanceKline> klines;
            if (itemData.isSpot)
                klines = client.Spot.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 600).Data;
            else if (itemData.isFutureUsdt)
                klines = client.FuturesUsdt.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 600).Data;
            else
                klines = client.FuturesCoin.Market.GetKlines(itemData.Name, KlineInterval.OneMinute, null, endTime, 600).Data;

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
                stick.Ms = kline.BaseVolume - kline.TakerBuyBaseVolume;
                stick.Time = kline.OpenTime;
                stick.TCount = kline.TradeCount;
                AddFullChartPoint(chart2, ItemData.minChartLabel, stick);
                newMinStickList.Add(stick);
            }

            start += newMinStickList.Count;
            end += newMinStickList.Count;

            for (int i = 0; i < itemData.minStickList.Count; i++)
            {
                stick = itemData.minStickList[i];
                AddFullChartPoint(chart2, ItemData.minChartLabel, stick);
                newMinStickList.Add(stick);
            }

            itemData.minStickList = newMinStickList;

            chart2.ChartAreas[0].AxisX.ScaleView.Zoom(start, end);
            chart2.ChartAreas[0].RecalculateAxesScale();
            AdjustChart(chart2);
        }
        void AddFullChartPoint(Chart chart, string xType, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(xType), new object[] { stick.Price[0], stick.Price[1], stick.Price[2], stick.Price[3] });
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
        void AddStartChartPoint(Chart chart, string xType, Stick stick)
        {
            chart.Series[0].Points.AddXY(stick.Time.ToString(xType), new object[] { stick.Price[2], stick.Price[2], stick.Price[2], stick.Price[2] });
            chart.Series[1].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);
            chart.Series[2].Points.AddXY(chart.Series[0].Points.Last().AxisLabel, 0);

            if (chart.Series[0].Points.Count == baseChartViewSticksSize + 1)
            {
                chart.ChartAreas[0].AxisX.ScaleView.Zoom(chart.Series[0].Points.Count - baseChartViewSticksSize + 1, chart.Series[0].Points.Count);
                chart.ChartAreas[0].RecalculateAxesScale();
                AdjustChart(chart);
            }

            if (chart.ChartAreas[0].AxisX.ScaleView.IsZoomed && (int)chart.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= chart.Series[0].Points.Count - 1)
            {
                chart.ChartAreas[0].RecalculateAxesScale();
                chart.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallIncrement);
                AdjustChart(chart);
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
                else if (chart.Series[0].Points[i].YValues[0] > priceHigh)
                    priceHigh = chart.Series[0].Points[i].YValues[0];

                if (chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0] > msOrMdHigh)
                    msOrMdHigh = chart.Series[1].Points[i].YValues[0] + chart.Series[2].Points[i].YValues[0];
            }

            chart.ChartAreas[0].AxisY.ScaleView.Zoom(2 * priceLow - priceHigh, priceHigh);
            chart.ChartAreas[1].AxisY.ScaleView.Zoom(0, msOrMdHigh);
            chart.ChartAreas[1].AxisY2.ScaleView.Zoom(0, msOrMdHigh);

            chart.ChartAreas[0].AxisY.MajorGrid.Interval = (double)priceHigh * 0.005;
        }

        List<string>[] spotSymbolList = Enumerable.Range(0, 5).Select(i => new List<string>()).ToArray();
        List<string> futureUSymbolList = new List<string>();
        List<string> futureCSymbolList = new List<string>();
        void SetItemDataList()
        {
            var exchangeInfo = client.Spot.System.GetExchangeInfo();
            short n = 0;
            foreach (var s in exchangeInfo.Data.Symbols)
            {
                var itemData = new ItemData(s, null, null);
                spotItemDataList.Add(itemData.Name, itemData);
                spotSymbolList[n / 300].Add(itemData.Name);

                n++;
            }

            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            n = 0;
            foreach (var s in exchangeInfo2.Data.Symbols)
            {
                var itemData = new ItemData(null, s, null);
                futureUItemDataList.Add(itemData.Name, itemData);
                futureUSymbolList.Add(itemData.Name);

                n++;
            }

            var exchangeInfo3 = client.FuturesCoin.System.GetExchangeInfo();
            n = 0;
            foreach (var s in exchangeInfo3.Data.Symbols)
            {
                var itemData = new ItemData(null, null, s);
                futureCItemDataList.Add(itemData.Name, itemData);
                futureCSymbolList.Add(itemData.Name);

                n++;
            }
        }

        delegate void AggUpdates(BinanceStreamAggregatedTrade data, ItemData itemData);
        AggUpdates aggUpdates;
        delegate void KlineUpdates(IBinanceStreamKlineData data, ItemData itemData);
        KlineUpdates klineUpdates;
        void Form1_Load(object sender, EventArgs e)
        {
            aggUpdates += new AggUpdates(OnAggregatedTradeUpdates);
            klineUpdates += new KlineUpdates(OnKlineUpdates);

            var symbols = 0;
            for (int i = 0; i < 5; i++)
            {
                socketClient.Spot.SubscribeToKlineUpdates(spotSymbolList[i], KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, spotItemDataList[data.Symbol]); });
                symbols += spotSymbolList[i].Count;
            }
            spotAggReqTextBox.Text = "/" + symbols + "(K)";
            
            socketClient.FuturesUsdt.SubscribeToKlineUpdates(futureUSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, futureUItemDataList[data.Symbol]); });
            futureUAggReqTextBox.Text = "/" + futureUSymbolList.Count + "(K)";

            socketClient.FuturesCoin.SubscribeToKlineUpdates(futureCSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(klineUpdates, data, futureCItemDataList[data.Symbol]); });
            futureCAggReqTextBox.Text = "/" + futureCSymbolList.Count + "(K)";
        }

        short spotAggOnNow = 0;
        short futureUAggOnNow = 0;
        short futureCAggOnNow = 0;
        short spotKlineRcv = 0;
        short futureUKlineRcv = 0;
        short futureCKlineRcv = 0;
        void OnKlineUpdates(IBinanceStreamKlineData data, ItemData itemData)
        {
            if (data.Data.Final)
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

                if (data.Data.High + data.Data.Low < data.Data.Close * 2)
                    itemData.RateBfor = Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                else
                    itemData.RateBfor = -Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);

                if (Math.Abs(itemData.RateBfor) > 1)
                {
                    if (itemData.isSpot && !itemData.isAggReady && itemData.RateBfor > 1 && data.Data.TradeCount > 180)
                    {
                        itemData.isAggReady = true;
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
                        futureCAggRcvTextBox.Text = futureUAggRcv.ToString();
                        futureCAggReqTextBox.Text = futureUAggReq.ToString();
                    }
                }

                if (itemData.isSpot)
                    spotListView.RefreshObject(itemData);
                else if (itemData.isFutureUsdt)
                    futureUListView.RefreshObject(itemData);
                else
                    futureCListView.RefreshObject(itemData);

                if (itemData.isAggReady && !itemData.isAggOn &&
                    ((itemData.isSpot && spotAggOnNow < 5) || (!itemData.isSpot && itemData.isFutureUsdt && futureUAggOnNow < 5) || (!itemData.isSpot && !itemData.isFutureUsdt && futureCAggOnNow < 5)))
                    SetAggOn(itemData, false);

                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Ms = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;
                itemData.minStick.TCount = data.Data.TradeCount;
                itemData.minStickList.Add(itemData.minStick);

                itemData.minStick = new Stick();

                if (itemData.isChartShowing)
                {
                    itemData.minStick.Price[2] = data.Data.Close;
                    itemData.minStick.Time = data.Data.OpenTime.AddMinutes(1);
                    AddStartChartPoint(chart2, ItemData.minChartLabel, itemData.minStick);
                }
            }
            else if (itemData.isChartShowing)
            {
                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;
                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Ms = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;
                itemData.minStick.Time = data.Data.OpenTime;
                itemData.minStick.TCount = data.Data.TradeCount;
                UpdateChartPoint(chart2, itemData.minStick);
            }
        }
        short spotAggRcv = 0;
        short futureUAggRcv = 0;
        short futureCAggRcv = 0;
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
                            itemData.secMsList0Tot += itemData.secStickList[itemData.secStickList.Count - 1].Ms;
                            itemData.secMdList0Tot += itemData.secStickList[itemData.secStickList.Count - 1].Md;
                        }
                        else
                        {
                            itemData.secMsList0Tot = itemData.secMsList0Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt0].Ms + itemData.secStickList[itemData.secStickList.Count - 1].Ms;
                            itemData.secMsList0Avg = itemData.secMsList0Tot / ItemData.amt0;
                            itemData.secMdList0Tot = itemData.secMdList0Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt0].Md + itemData.secStickList[itemData.secStickList.Count - 1].Md;
                            itemData.secMdList0Avg = itemData.secMdList0Tot / ItemData.amt0;
                        }
                        if (itemData.secStickList.Count <= ItemData.amt1)
                        {
                            itemData.secMsList1Tot += itemData.secStickList[itemData.secStickList.Count - 1].Ms;
                            itemData.secMdList1Tot += itemData.secStickList[itemData.secStickList.Count - 1].Md;
                        }
                        else
                        {
                            itemData.secMsList1Tot = itemData.secMsList1Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt1].Ms + itemData.secStickList[itemData.secStickList.Count - 1].Ms;
                            itemData.secMsList1Avg = itemData.secMsList1Tot / ItemData.amt1;
                            itemData.secMdList1Tot = itemData.secMdList1Tot - itemData.secStickList[itemData.secStickList.Count - 1 - ItemData.amt1].Md + itemData.secStickList[itemData.secStickList.Count - 1].Md;
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
                    AddStartChartPoint(chart1, ItemData.secChartLabel, itemData.secStick);

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

            if (itemData.isChartShowing)
            {
                timeDiffTextBox.Text = Math.Round((DateTime.UtcNow - data.TradeTime).TotalSeconds, 1).ToString();
                amtTextBox.Text = data.Quantity.ToString();
                priceTextBox.Text = data.Price.ToString();
                rcvTimeTextBox.Text = itemData.newTime;
                realTimeTextBox.Text = DateTime.UtcNow.ToString(ItemData.secChartLabel);
                if (data.BuyerIsMaker)
                {
                    amtTextBox.ForeColor = mdAmtColor;
                    priceTextBox.ForeColor = mdAmtColor;
                    rcvTimeTextBox.ForeColor = mdAmtColor;
                    realTimeTextBox.ForeColor = mdAmtColor;
                    timeDiffTextBox.ForeColor = mdAmtColor;
                }
                else
                {
                    amtTextBox.ForeColor = msAmtColor;
                    priceTextBox.ForeColor = msAmtColor;
                    rcvTimeTextBox.ForeColor = msAmtColor;
                    realTimeTextBox.ForeColor = msAmtColor;
                    timeDiffTextBox.ForeColor = msAmtColor;
                }

                UpdateChartPoint(chart1, itemData.secStick);
            }
        }
        short spotAggReq = 0;
        short futureUAggReq = 0;
        short futureCAggReq = 0;
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
            itemData.WinPrecantage = "0(0)";
            itemData.profitRateSum = 0;
            itemData.profitRateMul = 0;
            itemData.ProfitRateSumAndMul = "0(0)";

            itemData.secStickList.Clear();
            itemData.secStick = new Stick();
            if (itemData.isSpot)
            {
                itemData.sub = socketClient.Spot.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, spotItemDataList[data2.Symbol]); }).Data;
                itemData.item = spotListView.ModelToItem(itemData);
                itemData.item.ForeColor = plsPrcColor;
                if (addObject)
                    spotListView.AddObject(itemData);
                spotListView.MoveObjects(0, new List<ItemData> { itemData });
                spotListView.RefreshObject(itemData);
                spotAggOnNow++;
                spotAggReq++;
                spotAggReqTextBox.Text = "/" + spotAggReq + "(A)";
            }
            else
            {
                if (itemData.LorS)
                    itemData.item.ForeColor = plsPrcColor;
                else
                    itemData.item.ForeColor = mnsPrcColor;
                if (itemData.isFutureUsdt)
                {
                    itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, futureUItemDataList[data2.Symbol]); }).Data;
                    itemData.item = futureUListView.ModelToItem(itemData);
                    if (addObject)
                        futureUListView.AddObject(itemData);
                    futureUListView.MoveObjects(0, new List<ItemData> { itemData });
                    futureUListView.RefreshObject(itemData);
                    futureUAggOnNow++;
                    futureUAggReq++;
                    futureUAggReqTextBox.Text = "/" + futureUAggReq + "(A)";
                }
                else
                {
                    itemData.sub = socketClient.FuturesCoin.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(aggUpdates, data2, futureCItemDataList[data2.Symbol]); }).Data;
                    itemData.item = futureCListView.ModelToItem(itemData);
                    if (addObject)
                        futureCListView.AddObject(itemData);
                    futureCListView.MoveObjects(0, new List<ItemData> { itemData });
                    futureCListView.RefreshObject(itemData);
                    futureCAggOnNow++;
                    futureCAggReq++;
                    futureCAggReqTextBox.Text = "/" + futureCAggReq + "(A)";
                }
            }
        }
        bool BuyConditionMs(ItemData itemData)
        {
            if (
                (
                    itemData.secStickList[itemData.secStickList.Count - 1].Ms - itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMsList1Avg * 2
                    && itemData.secStickList[itemData.secStickList.Count - 1].Ms - itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMdList1Avg * 1.5m
                    && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMsList1Avg * 2
                    && itemData.secStick.Ms - itemData.secStick.Md > itemData.secMdList1Avg * 1.5m
                    &&
                    (
                        itemData.pureSecCountQAvg < 0.8m
                        ||
                        (
                            itemData.secStickList[itemData.secStickList.Count - 1].Ms - itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMsList0Avg * 2
                            && itemData.secStickList[itemData.secStickList.Count - 1].Ms - itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMdList0Avg * 1.5m
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
                    itemData.secStickList[itemData.secStickList.Count - 1].Md - itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMdList1Avg * 2
                    && itemData.secStickList[itemData.secStickList.Count - 1].Md - itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMsList1Avg * 1.5m
                    && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMdList1Avg * 2
                    && itemData.secStick.Md - itemData.secStick.Ms > itemData.secMsList1Avg * 1.5m
                    &&
                    (
                        itemData.pureSecCountQAvg > 0.2m
                        ||
                        (
                            itemData.secStickList[itemData.secStickList.Count - 1].Md - itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMdList0Avg * 2
                            && itemData.secStickList[itemData.secStickList.Count - 1].Md - itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMsList0Avg * 1.5m
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
            if (itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMsList0Avg
                && itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secMsList1Avg * 2
                && itemData.secStickList[itemData.secStickList.Count - 1].Md > itemData.secStickList[itemData.secStickList.Count - 1].Ms)
                return true;
            else
                return false;
        }
        bool SellConditionMd(ItemData itemData)
        {
            if (itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMdList0Avg
                && itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secMdList1Avg * 2
                && itemData.secStickList[itemData.secStickList.Count - 1].Ms > itemData.secStickList[itemData.secStickList.Count - 1].Md)
                return true;
            else
                return false;
        }
        bool SellCondition2ndMs(ItemData itemData)
        {
            if (itemData.secStickList[itemData.secStickList.Count - 1].Md < itemData.secMsList1Avg)
                return true;
            else
                return false;
        }
        bool SellCondition2ndMd(ItemData itemData)
        {
            if (itemData.secStickList[itemData.secStickList.Count - 1].Ms < itemData.secMdList1Avg)
                return true;
            else
                return false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (listenThread != null)
                listenThread.Abort();
            socketClient.UnsubscribeAll();
            //dbHelper.Close();
        }
    }
}
