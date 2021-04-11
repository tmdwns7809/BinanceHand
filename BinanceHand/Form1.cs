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
        Dictionary<string, ItemData> spotItemDataList = new Dictionary<string, ItemData>();
        Dictionary<string, ItemData> futureItemDataList = new Dictionary<string, ItemData>();

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

            SetItemDataList();

            SetComponents();
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

        delegate void FormUpdate(BinanceStreamAggregatedTrade data, bool isSpot);
        FormUpdate formUpdate;
        delegate void ListUpdate(IBinanceStreamKlineData data, bool isSpot);
        ListUpdate listUpdate;
        string startTime;
        void SetComponents()
        {
            startTime = DateTime.UtcNow.ToString();
            Text = "Binance       UTC-" + startTime;

            ChartSetting();

            var nameColumn = new OLVColumn("현물", "Name");
            var flucColumn = new OLVColumn("변동성", "Rate");
            nameColumn.FillsFreeSpace = true;
            flucColumn.FillsFreeSpace = true;
            spotListView.AllColumns.Add(nameColumn);
            spotListView.AllColumns.Add(flucColumn);
            //spotListView.HeaderStyle = ColumnHeaderStyle.None;
            spotListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn });
            spotListView.SelectionChanged += ListView_SelectionChanged;

            nameColumn = new OLVColumn("선물", "Name");
            flucColumn = new OLVColumn("변동성", "Rate");
            nameColumn.FillsFreeSpace = true;
            flucColumn.FillsFreeSpace = true;
            futureListView.AllColumns.Add(nameColumn);
            futureListView.AllColumns.Add(flucColumn);
            futureListView.Columns.AddRange(new ColumnHeader[] { nameColumn, flucColumn });
            futureListView.SelectionChanged += ListView_SelectionChanged;

            formUpdate += new FormUpdate(OnAggregatedTradeUpdates);
            listUpdate += new ListUpdate(OnSymbolKlineUpdates);
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
        }
        void Chart1_AxisViewChanged(object sender, ViewEventArgs e)
        {
            AdjustChart();
        }
        void AdjustChart()
        {
            var priceLow = double.MaxValue;
            var priceHigh = double.MinValue;
            var msOrMdHigh = double.MinValue;

            for (int i = (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMinimum - 1; i < (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum; i++)
            {
                if (i >= chart1.Series[0].Points.Count || i < 0)
                    continue;

                if (chart1.Series[0].Points[i].YValues[1] < priceLow)
                    priceLow = chart1.Series[0].Points[i].YValues[1];
                else if (chart1.Series[0].Points[i].YValues[0] > priceHigh)
                    priceHigh = chart1.Series[0].Points[i].YValues[0];

                if (chart1.Series[1].Points[i].YValues[0] + chart1.Series[2].Points[i].YValues[0] > msOrMdHigh)
                    msOrMdHigh = chart1.Series[1].Points[i].YValues[0] + chart1.Series[2].Points[i].YValues[0];
            }

            chart1.ChartAreas[0].AxisY.ScaleView.Zoom(2 * priceLow - priceHigh, priceHigh);
            chart1.ChartAreas[1].AxisY.ScaleView.Zoom(0, msOrMdHigh);
            chart1.ChartAreas[1].AxisY2.ScaleView.Zoom(0, msOrMdHigh);
        }

        ItemData itemDataShowing;
        private void ListView_SelectionChanged(object sender, EventArgs e)
        {
            if (((FastDataListView)sender).SelectedIndices.Count != 1)
                return;

            ItemData itemData = ((FastDataListView)sender).SelectedObject as ItemData;

            if (itemData.isChartShowing)
                return;

            if (itemDataShowing != null && itemDataShowing.isChartShowing)
                itemDataShowing.isChartShowing = false;

            itemData.isChartShowing = true;

            if (itemData.isSpot)
                Text = itemData.Name + "-Spot      Binance       UTC-" + startTime;
            else
                Text = itemData.Name + "-Future      Binance       UTC-" + startTime;

            foreach (var s in chart1.Series)
                s.Points.Clear();

            if (itemData.isAggOn)
            {
                Stick s;
                for (int i = 0; i < itemData.secStickList.Count; i++)
                {
                    s = itemData.secStickList[i];

                    chart1.Series[0].Points.AddXY(s.Time.ToString("HH:mm:ss"), new object[] { s.Price[0], s.Price[1], s.Price[2], s.Price[3] });
                    if (s.Price[2] > s.Price[3] || (s.Price[2] == s.Price[3] && s.Price[2] - s.Price[1] >= s.Price[0] - s.Price[3]))
                    {
                        chart1.Series[0].Points.Last().Color = mnsPrcColor;
                        chart1.Series[0].Points.Last().BackSecondaryColor = mnsPrcColor;
                    }
                    else
                    {
                        chart1.Series[0].Points.Last().Color = plsPrcColor;
                        chart1.Series[0].Points.Last().BackSecondaryColor = plsPrcColor;
                    }

                    if (s.Ms > s.Md)
                    {
                        chart1.Series[1].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, s.Md);
                        chart1.Series[2].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, s.Ms - s.Md);
                        chart1.Series[2].Points.Last().Color = msAmtColor;
                    }
                    else
                    {
                        chart1.Series[1].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, s.Ms);
                        chart1.Series[2].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, s.Md - s.Ms);
                        chart1.Series[2].Points.Last().Color = mdAmtColor;
                    }
                }

                if (chart1.Series[0].Points.Count >= 121)
                {
                    chart1.ChartAreas[0].RecalculateAxesScale();
                    chart1.ChartAreas[0].AxisX.ScaleView.Zoom(chart1.Series[0].Points.Count - 120, chart1.Series[0].Points.Count);
                    AdjustChart();
                }

                chart1.ChartAreas[0].AxisY.MajorGrid.Interval = chart1.Series[0].Points.Last().YValues[0] * 0.005;
            }
            else
            {
                itemData.isAggOn = true;
                itemData.secStickList.Clear();
                itemData.secStick = new Stick();

                if (itemData.isSpot)
                {
                    itemData.sub = socketClient.Spot.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(formUpdate, data2, true); }).Data;
                    spotAggOnNow++;
                    spotListView.AddObject(itemData);
                }
                else
                {
                    itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(formUpdate, data2, false); }).Data;
                    futureAggOnNow++;
                    futureListView.AddObject(itemData);
                }
            }

            itemDataShowing = itemData;
        }

        List<string>[] spotSymbolList = Enumerable.Range(0, 5).Select(i => new List<string>()).ToArray();
        List<string> futureSymbolList = new List<string>();
        void SetItemDataList()
        {
            var exchangeInfo = client.Spot.System.GetExchangeInfo();
            short n = 0;
            foreach (var ex in exchangeInfo.Data.Symbols)
            {
                var symbol = ex.Name.Trim().ToUpper();

                var itemData = new ItemData(symbol, true);
                spotItemDataList.Add(symbol, itemData);

                spotSymbolList[n / 300].Add(symbol);

                n++;
            }

            var exchangeInfo2 = client.FuturesUsdt.System.GetExchangeInfo();
            n = 0;
            foreach (var ex in exchangeInfo2.Data.Symbols)
            {
                var symbol = ex.Name.Trim().ToUpper();

                var itemData = new ItemData(symbol, false);
                futureItemDataList.Add(symbol, itemData);

                futureSymbolList.Add(symbol);

                n++;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
                socketClient.Spot.SubscribeToKlineUpdates(spotSymbolList[i], KlineInterval.OneMinute, data => { BeginInvoke(listUpdate, data, true); });
            
            socketClient.FuturesUsdt.SubscribeToKlineUpdates(futureSymbolList, KlineInterval.OneMinute, data => { BeginInvoke(listUpdate, data, false); });
        }

        short spotAggOnNow = 0;
        short futureAggOnNow = 0;
        private void OnSymbolKlineUpdates(IBinanceStreamKlineData data, bool isSpot)
        {
            if (data.Data.Final)
            {
                ItemData itemData;
                if (isSpot)
                    itemData = spotItemDataList[data.Symbol];
                else
                    itemData = futureItemDataList[data.Symbol];

                if (itemData.isSpot)
                {
                    if (data.Data.High + data.Data.Low < data.Data.Close * 2)
                        itemData.Rate = Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                    else
                        itemData.Rate = -Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                }
                else
                {
                    if (data.Data.High + data.Data.Low < data.Data.Close * 2)
                        itemData.Rate = Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                    else
                        itemData.Rate = -Math.Round((data.Data.High / data.Data.Low - 1) * 100, 1);
                }

                if (Math.Abs(itemData.Rate) > 1)
                {
                    if (!itemData.isAggOn && itemData.Rate > 1 && ((itemData.isSpot && spotAggOnNow < 5) || (!itemData.isSpot && futureAggOnNow < 5)))
                    {
                        itemData.isAggOn = true;
                        itemData.secStickList.Clear();
                        itemData.secStick = new Stick();

                        if (itemData.isSpot)
                        {
                            itemData.sub = socketClient.Spot.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(formUpdate, data2, true); }).Data;
                            spotAggOnNow++;
                            spotListView.AddObject(itemData);
                        }
                        else
                        {
                            itemData.sub = socketClient.FuturesUsdt.SubscribeToAggregatedTradeUpdates(itemData.Name, data2 => { BeginInvoke(formUpdate, data2, false); }).Data;
                            futureAggOnNow++;
                            futureListView.AddObject(itemData);
                        }
                    }

                    itemData.FlatMinRow = 0;
                }
                else if (itemData.isAggOn && itemData.FlatMinRow < 5)
                    itemData.FlatMinRow++;
                else if (itemData.isAggOn && !itemData.isChartShowing)
                {
                    socketClient.Unsubscribe(itemData.sub);
                    itemData.isAggOn = false;

                    if (itemData.isSpot)
                    {
                        spotAggOnNow--;
                        spotListView.RemoveObject(itemData);
                    }
                    else
                    {
                        futureAggOnNow--;
                        futureListView.RemoveObject(itemData);
                    }
                }

                itemData.minStick.Price[0] = data.Data.High;
                itemData.minStick.Price[1] = data.Data.Low;
                itemData.minStick.Price[2] = data.Data.Open;
                itemData.minStick.Price[3] = data.Data.Close;

                itemData.minStick.Ms = data.Data.TakerBuyBaseVolume;
                itemData.minStick.Ms = data.Data.BaseVolume - data.Data.TakerBuyBaseVolume;

                itemData.minStick.Time = data.Data.OpenTime;

                itemData.minStickList.Add(itemData.minStick);
                itemData.minStick = new Stick();
            }
        }

        private void OnAggregatedTradeUpdates(BinanceStreamAggregatedTrade data, bool isSpot)
        {
            ItemData itemData;
            if (isSpot)
                itemData = spotItemDataList[data.Symbol];
            else
                itemData = futureItemDataList[data.Symbol];

            if (!itemData.isAggOn)
                return;

            if (data.TradeTime.Second != itemData.secStick.Time.Second || data.TradeTime.Minute != itemData.secStick.Time.Minute || data.TradeTime.Hour != itemData.secStick.Time.Hour)
            {
                if (itemData.secStick.Price[0] != decimal.Zero)
                    itemData.secStickList.Add(itemData.secStick);
                else if (itemData.isChartShowing)
                    chart1.ChartAreas[0].AxisY.MajorGrid.Interval = (double)data.Price * 0.005;

                itemData.secStick = new Stick();

                itemData.secStick.Price[2] = data.Price;
                itemData.secStick.Price[0] = data.Price;
                itemData.secStick.Price[1] = data.Price;

                itemData.secStick.Time = data.TradeTime;

                if (itemData.isChartShowing)
                {
                    chart1.Series[0].Points.AddXY(itemData.secStick.Time.ToString("HH:mm:ss"), new object[] { data.Price, data.Price, data.Price, data.Price });
                    chart1.Series[1].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, 0);
                    chart1.Series[2].Points.AddXY(chart1.Series[0].Points.Last().AxisLabel, 0);

                    if (chart1.Series[0].Points.Count == 121)
                    {
                        chart1.ChartAreas[0].AxisX.ScaleView.Zoom(1, 121);
                        chart1.ChartAreas[0].RecalculateAxesScale();
                        AdjustChart();
                    }

                    if (chart1.ChartAreas[0].AxisX.ScaleView.IsZoomed && (int)chart1.ChartAreas[0].AxisX.ScaleView.ViewMaximum >= chart1.Series[0].Points.Count - 1)
                    {
                        chart1.ChartAreas[0].RecalculateAxesScale();
                        chart1.ChartAreas[0].AxisX.ScaleView.Scroll(ScrollType.SmallIncrement);
                        AdjustChart();
                    }
                }

                if (itemData.secStickList.Count > 2000)
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
            {
                itemData.secStick.Price[0] = data.Price;
                if (itemData.isChartShowing)
                    chart1.Series[0].Points.Last().YValues[0] = (double)itemData.secStick.Price[0];
            }
            else if (data.Price < itemData.secStick.Price[1])
            {
                itemData.secStick.Price[1] = data.Price;
                if (itemData.isChartShowing)
                    chart1.Series[0].Points.Last().YValues[1] = (double)itemData.secStick.Price[1];
            }
            itemData.secStick.Price[3] = data.Price;

            if (data.BuyerIsMaker)
                itemData.secStick.Md += data.Quantity;
            else
                itemData.secStick.Ms += data.Quantity;

            if (itemData.isChartShowing)
            {
                if (itemData.secStick.Ms > itemData.secStick.Md)
                {
                    chart1.Series[1].Points.Last().YValues[0] = (double)itemData.secStick.Md;
                    chart1.Series[2].Points.Last().YValues[0] = (double)(itemData.secStick.Ms - itemData.secStick.Md);
                    chart1.Series[2].Points.Last().Color = msAmtColor;
                }
                else
                {
                    chart1.Series[1].Points.Last().YValues[0] = (double)itemData.secStick.Ms;
                    chart1.Series[2].Points.Last().YValues[0] = (double)(itemData.secStick.Md - itemData.secStick.Ms);
                    chart1.Series[2].Points.Last().Color = mdAmtColor;
                }

                amtTextBox.Text = data.Quantity.ToString();
                priceTextBox.Text = data.Price.ToString();
                rcvTimeTextBox.Text = data.TradeTime.ToString("HH:mm:ss");
                realTimeTextBox.Text = DateTime.UtcNow.ToString("HH:mm:ss");
                if (data.BuyerIsMaker)
                {
                    amtTextBox.ForeColor = mdAmtColor;
                    priceTextBox.ForeColor = mdAmtColor;
                    rcvTimeTextBox.ForeColor = mdAmtColor;
                    realTimeTextBox.ForeColor = mdAmtColor;
                }
                else
                {
                    amtTextBox.ForeColor = msAmtColor;
                    priceTextBox.ForeColor = msAmtColor;
                    rcvTimeTextBox.ForeColor = msAmtColor;
                    realTimeTextBox.ForeColor = msAmtColor;
                }

                chart1.Series[0].Points.Last().YValues[3] = (double)itemData.secStick.Price[3];

                if (itemData.secStick.Price[2] > itemData.secStick.Price[3] || 
                    (itemData.secStick.Price[2] == itemData.secStick.Price[3] && itemData.secStick.Price[2] - itemData.secStick.Price[1] >= itemData.secStick.Price[0] - itemData.secStick.Price[3]))
                {
                    chart1.Series[0].Points.Last().Color = mnsPrcColor;
                    chart1.Series[0].Points.Last().BackSecondaryColor = mnsPrcColor;
                }
                else
                {
                    chart1.Series[0].Points.Last().Color = plsPrcColor;
                    chart1.Series[0].Points.Last().BackSecondaryColor = plsPrcColor;
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (listenThread != null)
                listenThread.Abort();
            socketClient.UnsubscribeAll();
        }
    }
}
