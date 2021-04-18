
namespace BinanceHand
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.spotListView = new BrightIdeasSoftware.FastDataListView();
            this.futureUListView = new BrightIdeasSoftware.FastDataListView();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.spotTab = new System.Windows.Forms.TabPage();
            this.futuresUTab = new System.Windows.Forms.TabPage();
            this.FUBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUAvailBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.FUAvailBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMaintMarginTextBox1 = new System.Windows.Forms.TextBox();
            this.FUMarginBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUMaintMarginTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMarginBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.futuresUTabControl = new System.Windows.Forms.TabControl();
            this.FUPositionTab = new System.Windows.Forms.TabPage();
            this.FUPositionListView = new BrightIdeasSoftware.FastDataListView();
            this.FUOpenOrdersTab = new System.Windows.Forms.TabPage();
            this.FUOpenOrdersListView = new BrightIdeasSoftware.FastDataListView();
            this.FUOrderHistoryTab = new System.Windows.Forms.TabPage();
            this.FUOrderHistoryListView = new BrightIdeasSoftware.FastDataListView();
            this.FUTradeHistoryTab = new System.Windows.Forms.TabPage();
            this.FUTradeHistoryListView = new BrightIdeasSoftware.FastDataListView();
            this.FUTransactionHistoryTab = new System.Windows.Forms.TabPage();
            this.FUTransHistoryListView = new BrightIdeasSoftware.FastDataListView();
            this.FUAssetsTab = new System.Windows.Forms.TabPage();
            this.FUAssetsListView = new BrightIdeasSoftware.FastDataListView();
            this.futuresCTab = new System.Windows.Forms.TabPage();
            this.logTab = new System.Windows.Forms.TabPage();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.timeDiffTextBox = new System.Windows.Forms.TextBox();
            this.priceTextBox = new System.Windows.Forms.TextBox();
            this.amtTextBox = new System.Windows.Forms.TextBox();
            this.marketComboBox = new System.Windows.Forms.ComboBox();
            this.futureCListView = new BrightIdeasSoftware.FastDataListView();
            this.spotKlineRcvTextBox = new System.Windows.Forms.TextBox();
            this.spotKlineReqTextBox = new System.Windows.Forms.TextBox();
            this.spotAggRcvTextBox = new System.Windows.Forms.TextBox();
            this.spotAggReqTextBox = new System.Windows.Forms.TextBox();
            this.futureUKlineRcvTextBox = new System.Windows.Forms.TextBox();
            this.futureUKlineReqTextBox = new System.Windows.Forms.TextBox();
            this.futureUAggRcvTextBox = new System.Windows.Forms.TextBox();
            this.futureUAggReqTextBox = new System.Windows.Forms.TextBox();
            this.futureCKlineRcvTextBox = new System.Windows.Forms.TextBox();
            this.futureCKlineReqTextBox = new System.Windows.Forms.TextBox();
            this.futureCAggRcvTextBox = new System.Windows.Forms.TextBox();
            this.futureCAggReqTextBox = new System.Windows.Forms.TextBox();
            this.chartTabControl = new System.Windows.Forms.TabControl();
            this.secChartTab = new System.Windows.Forms.TabPage();
            this.minChartTab = new System.Windows.Forms.TabPage();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.FUMarginRatioTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMarginRatioTextBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureUListView)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.futuresUTab.SuspendLayout();
            this.futuresUTabControl.SuspendLayout();
            this.FUPositionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUPositionListView)).BeginInit();
            this.FUOpenOrdersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUOpenOrdersListView)).BeginInit();
            this.FUOrderHistoryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUOrderHistoryListView)).BeginInit();
            this.FUTradeHistoryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUTradeHistoryListView)).BeginInit();
            this.FUTransactionHistoryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUTransHistoryListView)).BeginInit();
            this.FUAssetsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUAssetsListView)).BeginInit();
            this.logTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).BeginInit();
            this.chartTabControl.SuspendLayout();
            this.secChartTab.SuspendLayout();
            this.minChartTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(44, 23);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(718, 310);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // spotListView
            // 
            this.spotListView.CellEditUseWholeCell = false;
            this.spotListView.DataSource = null;
            this.spotListView.HideSelection = false;
            this.spotListView.Location = new System.Drawing.Point(822, 484);
            this.spotListView.Name = "spotListView";
            this.spotListView.ShowGroups = false;
            this.spotListView.Size = new System.Drawing.Size(189, 146);
            this.spotListView.TabIndex = 4;
            this.spotListView.UseCompatibleStateImageBehavior = false;
            this.spotListView.View = System.Windows.Forms.View.Details;
            this.spotListView.VirtualMode = true;
            this.spotListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            this.spotListView.SelectionChanged += new System.EventHandler(this.ListView_SelectionChanged);
            // 
            // futureUListView
            // 
            this.futureUListView.CellEditUseWholeCell = false;
            this.futureUListView.DataSource = null;
            this.futureUListView.HideSelection = false;
            this.futureUListView.Location = new System.Drawing.Point(1017, 484);
            this.futureUListView.Name = "futureUListView";
            this.futureUListView.ShowGroups = false;
            this.futureUListView.Size = new System.Drawing.Size(189, 146);
            this.futureUListView.TabIndex = 4;
            this.futureUListView.UseCompatibleStateImageBehavior = false;
            this.futureUListView.View = System.Windows.Forms.View.Details;
            this.futureUListView.VirtualMode = true;
            this.futureUListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            this.futureUListView.SelectionChanged += new System.EventHandler(this.ListView_SelectionChanged);
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.spotTab);
            this.mainTabControl.Controls.Add(this.futuresUTab);
            this.mainTabControl.Controls.Add(this.futuresCTab);
            this.mainTabControl.Controls.Add(this.logTab);
            this.mainTabControl.Location = new System.Drawing.Point(12, 474);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(796, 188);
            this.mainTabControl.TabIndex = 5;
            // 
            // spotTab
            // 
            this.spotTab.Location = new System.Drawing.Point(4, 22);
            this.spotTab.Name = "spotTab";
            this.spotTab.Padding = new System.Windows.Forms.Padding(3);
            this.spotTab.Size = new System.Drawing.Size(788, 162);
            this.spotTab.TabIndex = 1;
            this.spotTab.Text = "Spot";
            this.spotTab.UseVisualStyleBackColor = true;
            // 
            // futuresUTab
            // 
            this.futuresUTab.Controls.Add(this.FUBalanceTextBox1);
            this.futuresUTab.Controls.Add(this.FUAvailBalanceTextBox1);
            this.futuresUTab.Controls.Add(this.FUBalanceTextBox0);
            this.futuresUTab.Controls.Add(this.FUAvailBalanceTextBox0);
            this.futuresUTab.Controls.Add(this.FUMarginRatioTextBox1);
            this.futuresUTab.Controls.Add(this.FUMaintMarginTextBox1);
            this.futuresUTab.Controls.Add(this.FUMarginBalanceTextBox1);
            this.futuresUTab.Controls.Add(this.FUMarginRatioTextBox0);
            this.futuresUTab.Controls.Add(this.FUMaintMarginTextBox0);
            this.futuresUTab.Controls.Add(this.FUMarginBalanceTextBox0);
            this.futuresUTab.Controls.Add(this.futuresUTabControl);
            this.futuresUTab.Location = new System.Drawing.Point(4, 22);
            this.futuresUTab.Name = "futuresUTab";
            this.futuresUTab.Padding = new System.Windows.Forms.Padding(3);
            this.futuresUTab.Size = new System.Drawing.Size(788, 162);
            this.futuresUTab.TabIndex = 2;
            this.futuresUTab.Text = "Futures-U";
            this.futuresUTab.UseVisualStyleBackColor = true;
            // 
            // FUBalanceTextBox1
            // 
            this.FUBalanceTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUBalanceTextBox1.Location = new System.Drawing.Point(654, 138);
            this.FUBalanceTextBox1.Name = "FUBalanceTextBox1";
            this.FUBalanceTextBox1.ReadOnly = true;
            this.FUBalanceTextBox1.Size = new System.Drawing.Size(70, 14);
            this.FUBalanceTextBox1.TabIndex = 1;
            this.FUBalanceTextBox1.Text = "0000.00000";
            // 
            // FUAvailBalanceTextBox1
            // 
            this.FUAvailBalanceTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUAvailBalanceTextBox1.Location = new System.Drawing.Point(654, 118);
            this.FUAvailBalanceTextBox1.Name = "FUAvailBalanceTextBox1";
            this.FUAvailBalanceTextBox1.ReadOnly = true;
            this.FUAvailBalanceTextBox1.Size = new System.Drawing.Size(70, 14);
            this.FUAvailBalanceTextBox1.TabIndex = 1;
            this.FUAvailBalanceTextBox1.Text = "0000.00000";
            // 
            // FUBalanceTextBox0
            // 
            this.FUBalanceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUBalanceTextBox0.Location = new System.Drawing.Point(629, 139);
            this.FUBalanceTextBox0.Name = "FUBalanceTextBox0";
            this.FUBalanceTextBox0.ReadOnly = true;
            this.FUBalanceTextBox0.Size = new System.Drawing.Size(19, 14);
            this.FUBalanceTextBox0.TabIndex = 1;
            this.FUBalanceTextBox0.Text = "B :";
            // 
            // FUAvailBalanceTextBox0
            // 
            this.FUAvailBalanceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUAvailBalanceTextBox0.Location = new System.Drawing.Point(620, 118);
            this.FUAvailBalanceTextBox0.Name = "FUAvailBalanceTextBox0";
            this.FUAvailBalanceTextBox0.ReadOnly = true;
            this.FUAvailBalanceTextBox0.Size = new System.Drawing.Size(28, 14);
            this.FUAvailBalanceTextBox0.TabIndex = 1;
            this.FUAvailBalanceTextBox0.Text = "AV :";
            // 
            // FUMaintMarginTextBox1
            // 
            this.FUMaintMarginTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMaintMarginTextBox1.Location = new System.Drawing.Point(654, 78);
            this.FUMaintMarginTextBox1.Name = "FUMaintMarginTextBox1";
            this.FUMaintMarginTextBox1.ReadOnly = true;
            this.FUMaintMarginTextBox1.Size = new System.Drawing.Size(50, 14);
            this.FUMaintMarginTextBox1.TabIndex = 1;
            this.FUMaintMarginTextBox1.Text = "0000.00";
            // 
            // FUMarginBalanceTextBox1
            // 
            this.FUMarginBalanceTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMarginBalanceTextBox1.Location = new System.Drawing.Point(654, 98);
            this.FUMarginBalanceTextBox1.Name = "FUMarginBalanceTextBox1";
            this.FUMarginBalanceTextBox1.ReadOnly = true;
            this.FUMarginBalanceTextBox1.Size = new System.Drawing.Size(70, 14);
            this.FUMarginBalanceTextBox1.TabIndex = 1;
            this.FUMarginBalanceTextBox1.Text = "0000.00000";
            // 
            // FUMaintMarginTextBox0
            // 
            this.FUMaintMarginTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMaintMarginTextBox0.Location = new System.Drawing.Point(618, 78);
            this.FUMaintMarginTextBox0.Name = "FUMaintMarginTextBox0";
            this.FUMaintMarginTextBox0.ReadOnly = true;
            this.FUMaintMarginTextBox0.Size = new System.Drawing.Size(30, 14);
            this.FUMaintMarginTextBox0.TabIndex = 1;
            this.FUMaintMarginTextBox0.Text = "MM :";
            // 
            // FUMarginBalanceTextBox0
            // 
            this.FUMarginBalanceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMarginBalanceTextBox0.Location = new System.Drawing.Point(618, 98);
            this.FUMarginBalanceTextBox0.Name = "FUMarginBalanceTextBox0";
            this.FUMarginBalanceTextBox0.ReadOnly = true;
            this.FUMarginBalanceTextBox0.Size = new System.Drawing.Size(30, 14);
            this.FUMarginBalanceTextBox0.TabIndex = 1;
            this.FUMarginBalanceTextBox0.Text = "MB :";
            // 
            // futuresUTabControl
            // 
            this.futuresUTabControl.Controls.Add(this.FUPositionTab);
            this.futuresUTabControl.Controls.Add(this.FUOpenOrdersTab);
            this.futuresUTabControl.Controls.Add(this.FUOrderHistoryTab);
            this.futuresUTabControl.Controls.Add(this.FUTradeHistoryTab);
            this.futuresUTabControl.Controls.Add(this.FUTransactionHistoryTab);
            this.futuresUTabControl.Controls.Add(this.FUAssetsTab);
            this.futuresUTabControl.Location = new System.Drawing.Point(3, 3);
            this.futuresUTabControl.Name = "futuresUTabControl";
            this.futuresUTabControl.SelectedIndex = 0;
            this.futuresUTabControl.Size = new System.Drawing.Size(604, 154);
            this.futuresUTabControl.TabIndex = 1;
            // 
            // FUPositionTab
            // 
            this.FUPositionTab.Controls.Add(this.FUPositionListView);
            this.FUPositionTab.Location = new System.Drawing.Point(4, 22);
            this.FUPositionTab.Name = "FUPositionTab";
            this.FUPositionTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUPositionTab.Size = new System.Drawing.Size(596, 128);
            this.FUPositionTab.TabIndex = 0;
            this.FUPositionTab.Text = "Position";
            this.FUPositionTab.UseVisualStyleBackColor = true;
            // 
            // FUPositionListView
            // 
            this.FUPositionListView.CellEditUseWholeCell = false;
            this.FUPositionListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.FUPositionListView.DataSource = null;
            this.FUPositionListView.HideSelection = false;
            this.FUPositionListView.Location = new System.Drawing.Point(93, 12);
            this.FUPositionListView.Name = "FUPositionListView";
            this.FUPositionListView.ShowGroups = false;
            this.FUPositionListView.Size = new System.Drawing.Size(121, 97);
            this.FUPositionListView.TabIndex = 0;
            this.FUPositionListView.UseCompatibleStateImageBehavior = false;
            this.FUPositionListView.View = System.Windows.Forms.View.Details;
            this.FUPositionListView.VirtualMode = true;
            // 
            // FUOpenOrdersTab
            // 
            this.FUOpenOrdersTab.Controls.Add(this.FUOpenOrdersListView);
            this.FUOpenOrdersTab.Location = new System.Drawing.Point(4, 22);
            this.FUOpenOrdersTab.Name = "FUOpenOrdersTab";
            this.FUOpenOrdersTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUOpenOrdersTab.Size = new System.Drawing.Size(596, 128);
            this.FUOpenOrdersTab.TabIndex = 1;
            this.FUOpenOrdersTab.Text = "Open Orders";
            this.FUOpenOrdersTab.UseVisualStyleBackColor = true;
            // 
            // FUOpenOrdersListView
            // 
            this.FUOpenOrdersListView.CellEditUseWholeCell = false;
            this.FUOpenOrdersListView.DataSource = null;
            this.FUOpenOrdersListView.HideSelection = false;
            this.FUOpenOrdersListView.Location = new System.Drawing.Point(304, 16);
            this.FUOpenOrdersListView.Name = "FUOpenOrdersListView";
            this.FUOpenOrdersListView.ShowGroups = false;
            this.FUOpenOrdersListView.Size = new System.Drawing.Size(121, 97);
            this.FUOpenOrdersListView.TabIndex = 1;
            this.FUOpenOrdersListView.UseCompatibleStateImageBehavior = false;
            this.FUOpenOrdersListView.View = System.Windows.Forms.View.Details;
            this.FUOpenOrdersListView.VirtualMode = true;
            // 
            // FUOrderHistoryTab
            // 
            this.FUOrderHistoryTab.Controls.Add(this.FUOrderHistoryListView);
            this.FUOrderHistoryTab.Location = new System.Drawing.Point(4, 22);
            this.FUOrderHistoryTab.Name = "FUOrderHistoryTab";
            this.FUOrderHistoryTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUOrderHistoryTab.Size = new System.Drawing.Size(596, 128);
            this.FUOrderHistoryTab.TabIndex = 2;
            this.FUOrderHistoryTab.Text = "Order History";
            this.FUOrderHistoryTab.UseVisualStyleBackColor = true;
            // 
            // FUOrderHistoryListView
            // 
            this.FUOrderHistoryListView.CellEditUseWholeCell = false;
            this.FUOrderHistoryListView.DataSource = null;
            this.FUOrderHistoryListView.HideSelection = false;
            this.FUOrderHistoryListView.Location = new System.Drawing.Point(304, 16);
            this.FUOrderHistoryListView.Name = "FUOrderHistoryListView";
            this.FUOrderHistoryListView.ShowGroups = false;
            this.FUOrderHistoryListView.Size = new System.Drawing.Size(121, 97);
            this.FUOrderHistoryListView.TabIndex = 1;
            this.FUOrderHistoryListView.UseCompatibleStateImageBehavior = false;
            this.FUOrderHistoryListView.View = System.Windows.Forms.View.Details;
            this.FUOrderHistoryListView.VirtualMode = true;
            // 
            // FUTradeHistoryTab
            // 
            this.FUTradeHistoryTab.Controls.Add(this.FUTradeHistoryListView);
            this.FUTradeHistoryTab.Location = new System.Drawing.Point(4, 22);
            this.FUTradeHistoryTab.Name = "FUTradeHistoryTab";
            this.FUTradeHistoryTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUTradeHistoryTab.Size = new System.Drawing.Size(596, 128);
            this.FUTradeHistoryTab.TabIndex = 3;
            this.FUTradeHistoryTab.Text = "Trade History";
            this.FUTradeHistoryTab.UseVisualStyleBackColor = true;
            // 
            // FUTradeHistoryListView
            // 
            this.FUTradeHistoryListView.CellEditUseWholeCell = false;
            this.FUTradeHistoryListView.DataSource = null;
            this.FUTradeHistoryListView.HideSelection = false;
            this.FUTradeHistoryListView.Location = new System.Drawing.Point(304, 16);
            this.FUTradeHistoryListView.Name = "FUTradeHistoryListView";
            this.FUTradeHistoryListView.ShowGroups = false;
            this.FUTradeHistoryListView.Size = new System.Drawing.Size(121, 97);
            this.FUTradeHistoryListView.TabIndex = 1;
            this.FUTradeHistoryListView.UseCompatibleStateImageBehavior = false;
            this.FUTradeHistoryListView.View = System.Windows.Forms.View.Details;
            this.FUTradeHistoryListView.VirtualMode = true;
            // 
            // FUTransactionHistoryTab
            // 
            this.FUTransactionHistoryTab.Controls.Add(this.FUTransHistoryListView);
            this.FUTransactionHistoryTab.Location = new System.Drawing.Point(4, 22);
            this.FUTransactionHistoryTab.Name = "FUTransactionHistoryTab";
            this.FUTransactionHistoryTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUTransactionHistoryTab.Size = new System.Drawing.Size(596, 128);
            this.FUTransactionHistoryTab.TabIndex = 4;
            this.FUTransactionHistoryTab.Text = "Transaction History";
            this.FUTransactionHistoryTab.UseVisualStyleBackColor = true;
            // 
            // FUTransHistoryListView
            // 
            this.FUTransHistoryListView.CellEditUseWholeCell = false;
            this.FUTransHistoryListView.DataSource = null;
            this.FUTransHistoryListView.HideSelection = false;
            this.FUTransHistoryListView.Location = new System.Drawing.Point(304, 16);
            this.FUTransHistoryListView.Name = "FUTransHistoryListView";
            this.FUTransHistoryListView.ShowGroups = false;
            this.FUTransHistoryListView.Size = new System.Drawing.Size(121, 97);
            this.FUTransHistoryListView.TabIndex = 1;
            this.FUTransHistoryListView.UseCompatibleStateImageBehavior = false;
            this.FUTransHistoryListView.View = System.Windows.Forms.View.Details;
            this.FUTransHistoryListView.VirtualMode = true;
            // 
            // FUAssetsTab
            // 
            this.FUAssetsTab.Controls.Add(this.FUAssetsListView);
            this.FUAssetsTab.Location = new System.Drawing.Point(4, 22);
            this.FUAssetsTab.Name = "FUAssetsTab";
            this.FUAssetsTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUAssetsTab.Size = new System.Drawing.Size(596, 128);
            this.FUAssetsTab.TabIndex = 5;
            this.FUAssetsTab.Text = "Assets";
            this.FUAssetsTab.UseVisualStyleBackColor = true;
            // 
            // FUAssetsListView
            // 
            this.FUAssetsListView.CellEditUseWholeCell = false;
            this.FUAssetsListView.DataSource = null;
            this.FUAssetsListView.HideSelection = false;
            this.FUAssetsListView.Location = new System.Drawing.Point(304, 16);
            this.FUAssetsListView.Name = "FUAssetsListView";
            this.FUAssetsListView.ShowGroups = false;
            this.FUAssetsListView.Size = new System.Drawing.Size(121, 97);
            this.FUAssetsListView.TabIndex = 1;
            this.FUAssetsListView.UseCompatibleStateImageBehavior = false;
            this.FUAssetsListView.View = System.Windows.Forms.View.Details;
            this.FUAssetsListView.VirtualMode = true;
            // 
            // futuresCTab
            // 
            this.futuresCTab.Location = new System.Drawing.Point(4, 22);
            this.futuresCTab.Name = "futuresCTab";
            this.futuresCTab.Padding = new System.Windows.Forms.Padding(3);
            this.futuresCTab.Size = new System.Drawing.Size(788, 162);
            this.futuresCTab.TabIndex = 3;
            this.futuresCTab.Text = "Futures-C";
            this.futuresCTab.UseVisualStyleBackColor = true;
            // 
            // logTab
            // 
            this.logTab.Controls.Add(this.logListBox);
            this.logTab.Location = new System.Drawing.Point(4, 22);
            this.logTab.Name = "logTab";
            this.logTab.Padding = new System.Windows.Forms.Padding(3);
            this.logTab.Size = new System.Drawing.Size(788, 162);
            this.logTab.TabIndex = 4;
            this.logTab.Text = "Log";
            this.logTab.UseVisualStyleBackColor = true;
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 12;
            this.logListBox.Location = new System.Drawing.Point(115, 22);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(233, 112);
            this.logListBox.TabIndex = 1;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(86, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(90, 21);
            this.nameTextBox.TabIndex = 6;
            this.nameTextBox.Text = "ABCDEF";
            this.nameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.nameTextBox_KeyPress);
            // 
            // timeDiffTextBox
            // 
            this.timeDiffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeDiffTextBox.Location = new System.Drawing.Point(1175, 450);
            this.timeDiffTextBox.Name = "timeDiffTextBox";
            this.timeDiffTextBox.ReadOnly = true;
            this.timeDiffTextBox.Size = new System.Drawing.Size(38, 14);
            this.timeDiffTextBox.TabIndex = 6;
            this.timeDiffTextBox.Text = "-10.5";
            // 
            // priceTextBox
            // 
            this.priceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.priceTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.priceTextBox.Location = new System.Drawing.Point(1038, 450);
            this.priceTextBox.Name = "priceTextBox";
            this.priceTextBox.ReadOnly = true;
            this.priceTextBox.Size = new System.Drawing.Size(63, 14);
            this.priceTextBox.TabIndex = 6;
            this.priceTextBox.Text = "0.0000000";
            // 
            // amtTextBox
            // 
            this.amtTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.amtTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.amtTextBox.Location = new System.Drawing.Point(1106, 450);
            this.amtTextBox.Name = "amtTextBox";
            this.amtTextBox.ReadOnly = true;
            this.amtTextBox.Size = new System.Drawing.Size(63, 14);
            this.amtTextBox.TabIndex = 6;
            this.amtTextBox.Text = "0.0000000";
            // 
            // marketComboBox
            // 
            this.marketComboBox.FormattingEnabled = true;
            this.marketComboBox.Items.AddRange(new object[] {
            "S",
            "F_U",
            "F_C"});
            this.marketComboBox.Location = new System.Drawing.Point(35, 12);
            this.marketComboBox.Name = "marketComboBox";
            this.marketComboBox.Size = new System.Drawing.Size(50, 20);
            this.marketComboBox.TabIndex = 7;
            // 
            // futureCListView
            // 
            this.futureCListView.CellEditUseWholeCell = false;
            this.futureCListView.DataSource = null;
            this.futureCListView.HideSelection = false;
            this.futureCListView.Location = new System.Drawing.Point(1211, 484);
            this.futureCListView.Name = "futureCListView";
            this.futureCListView.ShowGroups = false;
            this.futureCListView.Size = new System.Drawing.Size(189, 146);
            this.futureCListView.TabIndex = 4;
            this.futureCListView.UseCompatibleStateImageBehavior = false;
            this.futureCListView.View = System.Windows.Forms.View.Details;
            this.futureCListView.VirtualMode = true;
            this.futureCListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            this.futureCListView.SelectionChanged += new System.EventHandler(this.ListView_SelectionChanged);
            // 
            // spotKlineRcvTextBox
            // 
            this.spotKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotKlineRcvTextBox.Location = new System.Drawing.Point(822, 634);
            this.spotKlineRcvTextBox.Name = "spotKlineRcvTextBox";
            this.spotKlineRcvTextBox.ReadOnly = true;
            this.spotKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.spotKlineRcvTextBox.TabIndex = 6;
            this.spotKlineRcvTextBox.Text = "0000";
            // 
            // spotKlineReqTextBox
            // 
            this.spotKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotKlineReqTextBox.Location = new System.Drawing.Point(851, 634);
            this.spotKlineReqTextBox.Name = "spotKlineReqTextBox";
            this.spotKlineReqTextBox.ReadOnly = true;
            this.spotKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotKlineReqTextBox.TabIndex = 6;
            this.spotKlineReqTextBox.Text = "/0000(K)";
            // 
            // spotAggRcvTextBox
            // 
            this.spotAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggRcvTextBox.Location = new System.Drawing.Point(927, 634);
            this.spotAggRcvTextBox.Name = "spotAggRcvTextBox";
            this.spotAggRcvTextBox.ReadOnly = true;
            this.spotAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.spotAggRcvTextBox.TabIndex = 6;
            this.spotAggRcvTextBox.Text = "0";
            // 
            // spotAggReqTextBox
            // 
            this.spotAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggReqTextBox.Location = new System.Drawing.Point(939, 634);
            this.spotAggReqTextBox.Name = "spotAggReqTextBox";
            this.spotAggReqTextBox.ReadOnly = true;
            this.spotAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotAggReqTextBox.TabIndex = 6;
            this.spotAggReqTextBox.Text = "/0(A)";
            // 
            // futureUKlineRcvTextBox
            // 
            this.futureUKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineRcvTextBox.Location = new System.Drawing.Point(1017, 634);
            this.futureUKlineRcvTextBox.Name = "futureUKlineRcvTextBox";
            this.futureUKlineRcvTextBox.ReadOnly = true;
            this.futureUKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureUKlineRcvTextBox.TabIndex = 6;
            this.futureUKlineRcvTextBox.Text = "000";
            // 
            // futureUKlineReqTextBox
            // 
            this.futureUKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineReqTextBox.Location = new System.Drawing.Point(1046, 634);
            this.futureUKlineReqTextBox.Name = "futureUKlineReqTextBox";
            this.futureUKlineReqTextBox.ReadOnly = true;
            this.futureUKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUKlineReqTextBox.TabIndex = 6;
            this.futureUKlineReqTextBox.Text = "/000(K)";
            // 
            // futureUAggRcvTextBox
            // 
            this.futureUAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggRcvTextBox.Location = new System.Drawing.Point(1122, 634);
            this.futureUAggRcvTextBox.Name = "futureUAggRcvTextBox";
            this.futureUAggRcvTextBox.ReadOnly = true;
            this.futureUAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureUAggRcvTextBox.TabIndex = 6;
            this.futureUAggRcvTextBox.Text = "0";
            // 
            // futureUAggReqTextBox
            // 
            this.futureUAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggReqTextBox.Location = new System.Drawing.Point(1134, 634);
            this.futureUAggReqTextBox.Name = "futureUAggReqTextBox";
            this.futureUAggReqTextBox.ReadOnly = true;
            this.futureUAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUAggReqTextBox.TabIndex = 6;
            this.futureUAggReqTextBox.Text = "/0(A)";
            // 
            // futureCKlineRcvTextBox
            // 
            this.futureCKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineRcvTextBox.Location = new System.Drawing.Point(1211, 634);
            this.futureCKlineRcvTextBox.Name = "futureCKlineRcvTextBox";
            this.futureCKlineRcvTextBox.ReadOnly = true;
            this.futureCKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureCKlineRcvTextBox.TabIndex = 6;
            this.futureCKlineRcvTextBox.Text = "000";
            // 
            // futureCKlineReqTextBox
            // 
            this.futureCKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineReqTextBox.Location = new System.Drawing.Point(1240, 634);
            this.futureCKlineReqTextBox.Name = "futureCKlineReqTextBox";
            this.futureCKlineReqTextBox.ReadOnly = true;
            this.futureCKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureCKlineReqTextBox.TabIndex = 6;
            this.futureCKlineReqTextBox.Text = "/000(K)";
            // 
            // futureCAggRcvTextBox
            // 
            this.futureCAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggRcvTextBox.Location = new System.Drawing.Point(1316, 634);
            this.futureCAggRcvTextBox.Name = "futureCAggRcvTextBox";
            this.futureCAggRcvTextBox.ReadOnly = true;
            this.futureCAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureCAggRcvTextBox.TabIndex = 6;
            this.futureCAggRcvTextBox.Text = "0";
            // 
            // futureCAggReqTextBox
            // 
            this.futureCAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggReqTextBox.Location = new System.Drawing.Point(1328, 634);
            this.futureCAggReqTextBox.Name = "futureCAggReqTextBox";
            this.futureCAggReqTextBox.ReadOnly = true;
            this.futureCAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureCAggReqTextBox.TabIndex = 6;
            this.futureCAggReqTextBox.Text = "/0(A)";
            // 
            // chartTabControl
            // 
            this.chartTabControl.Controls.Add(this.secChartTab);
            this.chartTabControl.Controls.Add(this.minChartTab);
            this.chartTabControl.Location = new System.Drawing.Point(132, 28);
            this.chartTabControl.Multiline = true;
            this.chartTabControl.Name = "chartTabControl";
            this.chartTabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chartTabControl.RightToLeftLayout = true;
            this.chartTabControl.SelectedIndex = 0;
            this.chartTabControl.Size = new System.Drawing.Size(766, 393);
            this.chartTabControl.TabIndex = 8;
            // 
            // secChartTab
            // 
            this.secChartTab.Controls.Add(this.chart1);
            this.secChartTab.Location = new System.Drawing.Point(4, 22);
            this.secChartTab.Name = "secChartTab";
            this.secChartTab.Padding = new System.Windows.Forms.Padding(30);
            this.secChartTab.Size = new System.Drawing.Size(758, 367);
            this.secChartTab.TabIndex = 0;
            this.secChartTab.Text = "Seconds";
            this.secChartTab.UseVisualStyleBackColor = true;
            // 
            // minChartTab
            // 
            this.minChartTab.Controls.Add(this.chart2);
            this.minChartTab.Location = new System.Drawing.Point(4, 22);
            this.minChartTab.Name = "minChartTab";
            this.minChartTab.Padding = new System.Windows.Forms.Padding(3);
            this.minChartTab.Size = new System.Drawing.Size(758, 367);
            this.minChartTab.TabIndex = 1;
            this.minChartTab.Text = "Minutes";
            this.minChartTab.UseVisualStyleBackColor = true;
            // 
            // chart2
            // 
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(45, 21);
            this.chart2.Name = "chart2";
            this.chart2.Size = new System.Drawing.Size(743, 373);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // FUMarginRatioTextBox0
            // 
            this.FUMarginRatioTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMarginRatioTextBox0.Location = new System.Drawing.Point(618, 58);
            this.FUMarginRatioTextBox0.Name = "FUMarginRatioTextBox0";
            this.FUMarginRatioTextBox0.ReadOnly = true;
            this.FUMarginRatioTextBox0.Size = new System.Drawing.Size(30, 14);
            this.FUMarginRatioTextBox0.TabIndex = 1;
            this.FUMarginRatioTextBox0.Text = "MR :";
            // 
            // FUMarginRatioTextBox1
            // 
            this.FUMarginRatioTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUMarginRatioTextBox1.Location = new System.Drawing.Point(654, 58);
            this.FUMarginRatioTextBox1.Name = "FUMarginRatioTextBox1";
            this.FUMarginRatioTextBox1.ReadOnly = true;
            this.FUMarginRatioTextBox1.Size = new System.Drawing.Size(50, 14);
            this.FUMarginRatioTextBox1.TabIndex = 1;
            this.FUMarginRatioTextBox1.Text = "00.00";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1417, 682);
            this.Controls.Add(this.marketComboBox);
            this.Controls.Add(this.amtTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.timeDiffTextBox);
            this.Controls.Add(this.chartTabControl);
            this.Controls.Add(this.priceTextBox);
            this.Controls.Add(this.futureCAggReqTextBox);
            this.Controls.Add(this.futureUAggReqTextBox);
            this.Controls.Add(this.spotAggReqTextBox);
            this.Controls.Add(this.futureCAggRcvTextBox);
            this.Controls.Add(this.futureUAggRcvTextBox);
            this.Controls.Add(this.spotAggRcvTextBox);
            this.Controls.Add(this.futureCKlineReqTextBox);
            this.Controls.Add(this.futureCKlineRcvTextBox);
            this.Controls.Add(this.futureUKlineReqTextBox);
            this.Controls.Add(this.futureUKlineRcvTextBox);
            this.Controls.Add(this.spotKlineReqTextBox);
            this.Controls.Add(this.spotKlineRcvTextBox);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.futureCListView);
            this.Controls.Add(this.futureUListView);
            this.Controls.Add(this.spotListView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureUListView)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.futuresUTab.ResumeLayout(false);
            this.futuresUTab.PerformLayout();
            this.futuresUTabControl.ResumeLayout(false);
            this.FUPositionTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUPositionListView)).EndInit();
            this.FUOpenOrdersTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUOpenOrdersListView)).EndInit();
            this.FUOrderHistoryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUOrderHistoryListView)).EndInit();
            this.FUTradeHistoryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUTradeHistoryListView)).EndInit();
            this.FUTransactionHistoryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUTransHistoryListView)).EndInit();
            this.FUAssetsTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUAssetsListView)).EndInit();
            this.logTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).EndInit();
            this.chartTabControl.ResumeLayout(false);
            this.secChartTab.ResumeLayout(false);
            this.minChartTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private BrightIdeasSoftware.FastDataListView spotListView;
        private BrightIdeasSoftware.FastDataListView futureUListView;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox timeDiffTextBox;
        private System.Windows.Forms.TextBox priceTextBox;
        private System.Windows.Forms.TextBox amtTextBox;
        private System.Windows.Forms.ComboBox marketComboBox;
        private BrightIdeasSoftware.FastDataListView futureCListView;
        private System.Windows.Forms.TextBox spotKlineRcvTextBox;
        private System.Windows.Forms.TextBox spotKlineReqTextBox;
        private System.Windows.Forms.TextBox spotAggRcvTextBox;
        private System.Windows.Forms.TextBox spotAggReqTextBox;
        private System.Windows.Forms.TextBox futureUKlineRcvTextBox;
        private System.Windows.Forms.TextBox futureUKlineReqTextBox;
        private System.Windows.Forms.TextBox futureUAggRcvTextBox;
        private System.Windows.Forms.TextBox futureUAggReqTextBox;
        private System.Windows.Forms.TextBox futureCKlineRcvTextBox;
        private System.Windows.Forms.TextBox futureCKlineReqTextBox;
        private System.Windows.Forms.TextBox futureCAggRcvTextBox;
        private System.Windows.Forms.TextBox futureCAggReqTextBox;
        private System.Windows.Forms.TabControl chartTabControl;
        private System.Windows.Forms.TabPage secChartTab;
        private System.Windows.Forms.TabPage minChartTab;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.TabPage spotTab;
        private System.Windows.Forms.TabPage futuresUTab;
        private System.Windows.Forms.TabControl futuresUTabControl;
        private System.Windows.Forms.TabPage FUPositionTab;
        private System.Windows.Forms.TabPage FUOpenOrdersTab;
        private System.Windows.Forms.TabPage futuresCTab;
        private System.Windows.Forms.TabPage logTab;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.TabPage FUOrderHistoryTab;
        private System.Windows.Forms.TabPage FUTradeHistoryTab;
        private System.Windows.Forms.TabPage FUTransactionHistoryTab;
        private System.Windows.Forms.TabPage FUAssetsTab;
        private BrightIdeasSoftware.FastDataListView FUPositionListView;
        private BrightIdeasSoftware.FastDataListView FUOpenOrdersListView;
        private BrightIdeasSoftware.FastDataListView FUOrderHistoryListView;
        private BrightIdeasSoftware.FastDataListView FUTradeHistoryListView;
        private BrightIdeasSoftware.FastDataListView FUTransHistoryListView;
        private BrightIdeasSoftware.FastDataListView FUAssetsListView;
        private System.Windows.Forms.TextBox FUAvailBalanceTextBox1;
        private System.Windows.Forms.TextBox FUAvailBalanceTextBox0;
        private System.Windows.Forms.TextBox FUMarginBalanceTextBox1;
        private System.Windows.Forms.TextBox FUMarginBalanceTextBox0;
        private System.Windows.Forms.TextBox FUBalanceTextBox1;
        private System.Windows.Forms.TextBox FUBalanceTextBox0;
        private System.Windows.Forms.TextBox FUMaintMarginTextBox1;
        private System.Windows.Forms.TextBox FUMaintMarginTextBox0;
        private System.Windows.Forms.TextBox FUMarginRatioTextBox1;
        private System.Windows.Forms.TextBox FUMarginRatioTextBox0;
    }
}

