
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend5 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.spotListView = new BrightIdeasSoftware.FastDataListView();
            this.futureUListView = new BrightIdeasSoftware.FastDataListView();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.spotTab = new System.Windows.Forms.TabPage();
            this.futuresUTab = new System.Windows.Forms.TabPage();
            this.FUWalletBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUAvailBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUWalletBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.FUAvailBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMarginRatioTextBox1 = new System.Windows.Forms.TextBox();
            this.FUMaintMarginTextBox1 = new System.Windows.Forms.TextBox();
            this.FUMarginBalanceTextBox1 = new System.Windows.Forms.TextBox();
            this.FUMarginRatioTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMaintMarginTextBox0 = new System.Windows.Forms.TextBox();
            this.FUMarginBalanceTextBox0 = new System.Windows.Forms.TextBox();
            this.futuresUTabControl = new System.Windows.Forms.TabControl();
            this.FUPositionTab = new System.Windows.Forms.TabPage();
            this.FUPositionListView = new BrightIdeasSoftware.FastDataListView();
            this.FUOpenOrdersTab = new System.Windows.Forms.TabPage();
            this.FUOpenOrdersListView = new BrightIdeasSoftware.FastDataListView();
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
            this.spotGroupBox = new System.Windows.Forms.GroupBox();
            this.FUGroupBox = new System.Windows.Forms.GroupBox();
            this.FCGroupBox = new System.Windows.Forms.GroupBox();
            this.orderGroupBox = new System.Windows.Forms.GroupBox();
            this.leverageTextBox0 = new System.Windows.Forms.TextBox();
            this.orderPriceTextBox1 = new System.Windows.Forms.TextBox();
            this.autoSizeTextBox1 = new System.Windows.Forms.TextBox();
            this.autoSizeTextBox0 = new System.Windows.Forms.TextBox();
            this.orderSizeTextBox1 = new System.Windows.Forms.TextBox();
            this.orderPriceTextBox2 = new System.Windows.Forms.TextBox();
            this.orderSizeTextBox0 = new System.Windows.Forms.TextBox();
            this.leverageTextBox1 = new System.Windows.Forms.TextBox();
            this.orderPriceTextBox0 = new System.Windows.Forms.TextBox();
            this.autoSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.miniSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.ROCheckBox = new System.Windows.Forms.CheckBox();
            this.PORadioButton = new System.Windows.Forms.RadioButton();
            this.marketRadioButton = new System.Windows.Forms.RadioButton();
            this.IOCRadioButton = new System.Windows.Forms.RadioButton();
            this.GTCRadioButton = new System.Windows.Forms.RadioButton();
            this.sellButton = new System.Windows.Forms.Button();
            this.buyButton = new System.Windows.Forms.Button();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.resultListView = new BrightIdeasSoftware.FastDataListView();
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
            this.logTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).BeginInit();
            this.chartTabControl.SuspendLayout();
            this.secChartTab.SuspendLayout();
            this.minChartTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.spotGroupBox.SuspendLayout();
            this.FUGroupBox.SuspendLayout();
            this.FCGroupBox.SuspendLayout();
            this.orderGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            legend5.Enabled = false;
            legend5.Name = "Legend1";
            this.chart1.Legends.Add(legend5);
            this.chart1.Location = new System.Drawing.Point(19, 24);
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
            this.spotListView.Location = new System.Drawing.Point(6, 20);
            this.spotListView.Name = "spotListView";
            this.spotListView.ShowGroups = false;
            this.spotListView.Size = new System.Drawing.Size(189, 79);
            this.spotListView.TabIndex = 4;
            this.spotListView.UseCompatibleStateImageBehavior = false;
            this.spotListView.View = System.Windows.Forms.View.Details;
            this.spotListView.VirtualMode = true;
            this.spotListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            // 
            // futureUListView
            // 
            this.futureUListView.CellEditUseWholeCell = false;
            this.futureUListView.DataSource = null;
            this.futureUListView.HideSelection = false;
            this.futureUListView.Location = new System.Drawing.Point(9, 20);
            this.futureUListView.Name = "futureUListView";
            this.futureUListView.ShowGroups = false;
            this.futureUListView.Size = new System.Drawing.Size(121, 49);
            this.futureUListView.TabIndex = 4;
            this.futureUListView.UseCompatibleStateImageBehavior = false;
            this.futureUListView.View = System.Windows.Forms.View.Details;
            this.futureUListView.VirtualMode = true;
            this.futureUListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
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
            this.mainTabControl.Size = new System.Drawing.Size(728, 188);
            this.mainTabControl.TabIndex = 5;
            // 
            // spotTab
            // 
            this.spotTab.Location = new System.Drawing.Point(4, 22);
            this.spotTab.Name = "spotTab";
            this.spotTab.Padding = new System.Windows.Forms.Padding(3);
            this.spotTab.Size = new System.Drawing.Size(720, 162);
            this.spotTab.TabIndex = 1;
            this.spotTab.Text = "Spot";
            this.spotTab.UseVisualStyleBackColor = true;
            // 
            // futuresUTab
            // 
            this.futuresUTab.Controls.Add(this.FUWalletBalanceTextBox1);
            this.futuresUTab.Controls.Add(this.FUAvailBalanceTextBox1);
            this.futuresUTab.Controls.Add(this.FUWalletBalanceTextBox0);
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
            this.futuresUTab.Size = new System.Drawing.Size(720, 162);
            this.futuresUTab.TabIndex = 2;
            this.futuresUTab.Text = "Futures-U";
            this.futuresUTab.UseVisualStyleBackColor = true;
            // 
            // FUWalletBalanceTextBox1
            // 
            this.FUWalletBalanceTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUWalletBalanceTextBox1.Location = new System.Drawing.Point(654, 138);
            this.FUWalletBalanceTextBox1.Name = "FUWalletBalanceTextBox1";
            this.FUWalletBalanceTextBox1.ReadOnly = true;
            this.FUWalletBalanceTextBox1.Size = new System.Drawing.Size(70, 14);
            this.FUWalletBalanceTextBox1.TabIndex = 1;
            this.FUWalletBalanceTextBox1.Text = "0000.00000";
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
            // FUWalletBalanceTextBox0
            // 
            this.FUWalletBalanceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUWalletBalanceTextBox0.Location = new System.Drawing.Point(620, 133);
            this.FUWalletBalanceTextBox0.Name = "FUWalletBalanceTextBox0";
            this.FUWalletBalanceTextBox0.ReadOnly = true;
            this.FUWalletBalanceTextBox0.Size = new System.Drawing.Size(30, 14);
            this.FUWalletBalanceTextBox0.TabIndex = 1;
            this.FUWalletBalanceTextBox0.Text = "WB :";
            // 
            // FUAvailBalanceTextBox0
            // 
            this.FUAvailBalanceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUAvailBalanceTextBox0.Location = new System.Drawing.Point(620, 118);
            this.FUAvailBalanceTextBox0.Name = "FUAvailBalanceTextBox0";
            this.FUAvailBalanceTextBox0.ReadOnly = true;
            this.FUAvailBalanceTextBox0.Size = new System.Drawing.Size(30, 14);
            this.FUAvailBalanceTextBox0.TabIndex = 1;
            this.FUAvailBalanceTextBox0.Text = "AB :";
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
            this.FUPositionTab.Text = "Position(0)";
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
            this.FUOpenOrdersTab.Text = "Open Orders(0)";
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
            // futuresCTab
            // 
            this.futuresCTab.Location = new System.Drawing.Point(4, 22);
            this.futuresCTab.Name = "futuresCTab";
            this.futuresCTab.Padding = new System.Windows.Forms.Padding(3);
            this.futuresCTab.Size = new System.Drawing.Size(720, 162);
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
            this.logTab.Size = new System.Drawing.Size(720, 162);
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
            // 
            // timeDiffTextBox
            // 
            this.timeDiffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeDiffTextBox.Location = new System.Drawing.Point(814, 440);
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
            this.priceTextBox.Location = new System.Drawing.Point(677, 440);
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
            this.amtTextBox.Location = new System.Drawing.Point(745, 440);
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
            this.futureCListView.Location = new System.Drawing.Point(6, 20);
            this.futureCListView.Name = "futureCListView";
            this.futureCListView.ShowGroups = false;
            this.futureCListView.Size = new System.Drawing.Size(133, 71);
            this.futureCListView.TabIndex = 4;
            this.futureCListView.UseCompatibleStateImageBehavior = false;
            this.futureCListView.View = System.Windows.Forms.View.Details;
            this.futureCListView.VirtualMode = true;
            this.futureCListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            // 
            // spotKlineRcvTextBox
            // 
            this.spotKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotKlineRcvTextBox.Location = new System.Drawing.Point(15, 105);
            this.spotKlineRcvTextBox.Name = "spotKlineRcvTextBox";
            this.spotKlineRcvTextBox.ReadOnly = true;
            this.spotKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.spotKlineRcvTextBox.TabIndex = 6;
            this.spotKlineRcvTextBox.Text = "0000";
            // 
            // spotKlineReqTextBox
            // 
            this.spotKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotKlineReqTextBox.Location = new System.Drawing.Point(44, 105);
            this.spotKlineReqTextBox.Name = "spotKlineReqTextBox";
            this.spotKlineReqTextBox.ReadOnly = true;
            this.spotKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotKlineReqTextBox.TabIndex = 6;
            this.spotKlineReqTextBox.Text = "/0000(K)";
            // 
            // spotAggRcvTextBox
            // 
            this.spotAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggRcvTextBox.Location = new System.Drawing.Point(120, 105);
            this.spotAggRcvTextBox.Name = "spotAggRcvTextBox";
            this.spotAggRcvTextBox.ReadOnly = true;
            this.spotAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.spotAggRcvTextBox.TabIndex = 6;
            this.spotAggRcvTextBox.Text = "0";
            // 
            // spotAggReqTextBox
            // 
            this.spotAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggReqTextBox.Location = new System.Drawing.Point(132, 105);
            this.spotAggReqTextBox.Name = "spotAggReqTextBox";
            this.spotAggReqTextBox.ReadOnly = true;
            this.spotAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotAggReqTextBox.TabIndex = 6;
            this.spotAggReqTextBox.Text = "/0(A)";
            // 
            // futureUKlineRcvTextBox
            // 
            this.futureUKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineRcvTextBox.Location = new System.Drawing.Point(9, 75);
            this.futureUKlineRcvTextBox.Name = "futureUKlineRcvTextBox";
            this.futureUKlineRcvTextBox.ReadOnly = true;
            this.futureUKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureUKlineRcvTextBox.TabIndex = 6;
            this.futureUKlineRcvTextBox.Text = "000";
            // 
            // futureUKlineReqTextBox
            // 
            this.futureUKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineReqTextBox.Location = new System.Drawing.Point(38, 75);
            this.futureUKlineReqTextBox.Name = "futureUKlineReqTextBox";
            this.futureUKlineReqTextBox.ReadOnly = true;
            this.futureUKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUKlineReqTextBox.TabIndex = 6;
            this.futureUKlineReqTextBox.Text = "/000(K)";
            // 
            // futureUAggRcvTextBox
            // 
            this.futureUAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggRcvTextBox.Location = new System.Drawing.Point(114, 75);
            this.futureUAggRcvTextBox.Name = "futureUAggRcvTextBox";
            this.futureUAggRcvTextBox.ReadOnly = true;
            this.futureUAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureUAggRcvTextBox.TabIndex = 6;
            this.futureUAggRcvTextBox.Text = "0";
            // 
            // futureUAggReqTextBox
            // 
            this.futureUAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggReqTextBox.Location = new System.Drawing.Point(126, 75);
            this.futureUAggReqTextBox.Name = "futureUAggReqTextBox";
            this.futureUAggReqTextBox.ReadOnly = true;
            this.futureUAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUAggReqTextBox.TabIndex = 6;
            this.futureUAggReqTextBox.Text = "/0(A)";
            // 
            // futureCKlineRcvTextBox
            // 
            this.futureCKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineRcvTextBox.Location = new System.Drawing.Point(12, 98);
            this.futureCKlineRcvTextBox.Name = "futureCKlineRcvTextBox";
            this.futureCKlineRcvTextBox.ReadOnly = true;
            this.futureCKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureCKlineRcvTextBox.TabIndex = 6;
            this.futureCKlineRcvTextBox.Text = "000";
            // 
            // futureCKlineReqTextBox
            // 
            this.futureCKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineReqTextBox.Location = new System.Drawing.Point(41, 98);
            this.futureCKlineReqTextBox.Name = "futureCKlineReqTextBox";
            this.futureCKlineReqTextBox.ReadOnly = true;
            this.futureCKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureCKlineReqTextBox.TabIndex = 6;
            this.futureCKlineReqTextBox.Text = "/000(K)";
            // 
            // futureCAggRcvTextBox
            // 
            this.futureCAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggRcvTextBox.Location = new System.Drawing.Point(117, 98);
            this.futureCAggRcvTextBox.Name = "futureCAggRcvTextBox";
            this.futureCAggRcvTextBox.ReadOnly = true;
            this.futureCAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureCAggRcvTextBox.TabIndex = 6;
            this.futureCAggRcvTextBox.Text = "0";
            // 
            // futureCAggReqTextBox
            // 
            this.futureCAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggReqTextBox.Location = new System.Drawing.Point(129, 98);
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
            this.chartTabControl.Location = new System.Drawing.Point(29, 41);
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
            legend6.Enabled = false;
            legend6.Name = "Legend1";
            this.chart2.Legends.Add(legend6);
            this.chart2.Location = new System.Drawing.Point(45, 21);
            this.chart2.Name = "chart2";
            this.chart2.Size = new System.Drawing.Size(743, 373);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // spotGroupBox
            // 
            this.spotGroupBox.Controls.Add(this.spotListView);
            this.spotGroupBox.Controls.Add(this.spotKlineRcvTextBox);
            this.spotGroupBox.Controls.Add(this.spotKlineReqTextBox);
            this.spotGroupBox.Controls.Add(this.spotAggRcvTextBox);
            this.spotGroupBox.Controls.Add(this.spotAggReqTextBox);
            this.spotGroupBox.Location = new System.Drawing.Point(1027, 33);
            this.spotGroupBox.Name = "spotGroupBox";
            this.spotGroupBox.Size = new System.Drawing.Size(210, 126);
            this.spotGroupBox.TabIndex = 2;
            this.spotGroupBox.TabStop = false;
            this.spotGroupBox.Text = "Spot";
            // 
            // FUGroupBox
            // 
            this.FUGroupBox.Controls.Add(this.futureUListView);
            this.FUGroupBox.Controls.Add(this.futureUKlineRcvTextBox);
            this.FUGroupBox.Controls.Add(this.futureUKlineReqTextBox);
            this.FUGroupBox.Controls.Add(this.futureUAggRcvTextBox);
            this.FUGroupBox.Controls.Add(this.futureUAggReqTextBox);
            this.FUGroupBox.Location = new System.Drawing.Point(1027, 165);
            this.FUGroupBox.Name = "FUGroupBox";
            this.FUGroupBox.Size = new System.Drawing.Size(209, 98);
            this.FUGroupBox.TabIndex = 9;
            this.FUGroupBox.TabStop = false;
            this.FUGroupBox.Text = "Futures-U";
            // 
            // FCGroupBox
            // 
            this.FCGroupBox.Controls.Add(this.futureCListView);
            this.FCGroupBox.Controls.Add(this.futureCKlineRcvTextBox);
            this.FCGroupBox.Controls.Add(this.futureCKlineReqTextBox);
            this.FCGroupBox.Controls.Add(this.futureCAggRcvTextBox);
            this.FCGroupBox.Controls.Add(this.futureCAggReqTextBox);
            this.FCGroupBox.Location = new System.Drawing.Point(1022, 278);
            this.FCGroupBox.Name = "FCGroupBox";
            this.FCGroupBox.Size = new System.Drawing.Size(200, 141);
            this.FCGroupBox.TabIndex = 9;
            this.FCGroupBox.TabStop = false;
            this.FCGroupBox.Text = "Futures-C";
            // 
            // orderGroupBox
            // 
            this.orderGroupBox.Controls.Add(this.leverageTextBox0);
            this.orderGroupBox.Controls.Add(this.orderPriceTextBox1);
            this.orderGroupBox.Controls.Add(this.autoSizeTextBox1);
            this.orderGroupBox.Controls.Add(this.autoSizeTextBox0);
            this.orderGroupBox.Controls.Add(this.orderSizeTextBox1);
            this.orderGroupBox.Controls.Add(this.orderPriceTextBox2);
            this.orderGroupBox.Controls.Add(this.orderSizeTextBox0);
            this.orderGroupBox.Controls.Add(this.leverageTextBox1);
            this.orderGroupBox.Controls.Add(this.orderPriceTextBox0);
            this.orderGroupBox.Controls.Add(this.autoSizeCheckBox);
            this.orderGroupBox.Controls.Add(this.miniSizeCheckBox);
            this.orderGroupBox.Controls.Add(this.ROCheckBox);
            this.orderGroupBox.Controls.Add(this.PORadioButton);
            this.orderGroupBox.Controls.Add(this.marketRadioButton);
            this.orderGroupBox.Controls.Add(this.IOCRadioButton);
            this.orderGroupBox.Controls.Add(this.GTCRadioButton);
            this.orderGroupBox.Controls.Add(this.sellButton);
            this.orderGroupBox.Controls.Add(this.buyButton);
            this.orderGroupBox.Location = new System.Drawing.Point(746, 469);
            this.orderGroupBox.Name = "orderGroupBox";
            this.orderGroupBox.Size = new System.Drawing.Size(351, 189);
            this.orderGroupBox.TabIndex = 9;
            this.orderGroupBox.TabStop = false;
            this.orderGroupBox.Text = "Order";
            // 
            // leverageTextBox0
            // 
            this.leverageTextBox0.Location = new System.Drawing.Point(263, 41);
            this.leverageTextBox0.Name = "leverageTextBox0";
            this.leverageTextBox0.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.leverageTextBox0.Size = new System.Drawing.Size(41, 21);
            this.leverageTextBox0.TabIndex = 9;
            // 
            // orderPriceTextBox1
            // 
            this.orderPriceTextBox1.Location = new System.Drawing.Point(65, 39);
            this.orderPriceTextBox1.Name = "orderPriceTextBox1";
            this.orderPriceTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.orderPriceTextBox1.Size = new System.Drawing.Size(41, 21);
            this.orderPriceTextBox1.TabIndex = 9;
            // 
            // autoSizeTextBox1
            // 
            this.autoSizeTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.autoSizeTextBox1.Location = new System.Drawing.Point(148, 97);
            this.autoSizeTextBox1.Name = "autoSizeTextBox1";
            this.autoSizeTextBox1.ReadOnly = true;
            this.autoSizeTextBox1.Size = new System.Drawing.Size(82, 14);
            this.autoSizeTextBox1.TabIndex = 8;
            this.autoSizeTextBox1.Text = "% of Balance";
            // 
            // autoSizeTextBox0
            // 
            this.autoSizeTextBox0.Location = new System.Drawing.Point(102, 97);
            this.autoSizeTextBox0.Name = "autoSizeTextBox0";
            this.autoSizeTextBox0.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.autoSizeTextBox0.Size = new System.Drawing.Size(30, 21);
            this.autoSizeTextBox0.TabIndex = 8;
            this.autoSizeTextBox0.Text = "1";
            // 
            // orderSizeTextBox1
            // 
            this.orderSizeTextBox1.Location = new System.Drawing.Point(65, 70);
            this.orderSizeTextBox1.Name = "orderSizeTextBox1";
            this.orderSizeTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.orderSizeTextBox1.Size = new System.Drawing.Size(62, 21);
            this.orderSizeTextBox1.TabIndex = 7;
            // 
            // orderPriceTextBox2
            // 
            this.orderPriceTextBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.orderPriceTextBox2.Location = new System.Drawing.Point(112, 42);
            this.orderPriceTextBox2.Name = "orderPriceTextBox2";
            this.orderPriceTextBox2.ReadOnly = true;
            this.orderPriceTextBox2.Size = new System.Drawing.Size(15, 14);
            this.orderPriceTextBox2.TabIndex = 6;
            this.orderPriceTextBox2.Text = "%";
            // 
            // orderSizeTextBox0
            // 
            this.orderSizeTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.orderSizeTextBox0.Location = new System.Drawing.Point(21, 78);
            this.orderSizeTextBox0.Name = "orderSizeTextBox0";
            this.orderSizeTextBox0.ReadOnly = true;
            this.orderSizeTextBox0.Size = new System.Drawing.Size(30, 14);
            this.orderSizeTextBox0.TabIndex = 6;
            this.orderSizeTextBox0.Text = "Size";
            // 
            // leverageTextBox1
            // 
            this.leverageTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.leverageTextBox1.Location = new System.Drawing.Point(310, 46);
            this.leverageTextBox1.Name = "leverageTextBox1";
            this.leverageTextBox1.ReadOnly = true;
            this.leverageTextBox1.Size = new System.Drawing.Size(33, 14);
            this.leverageTextBox1.TabIndex = 6;
            this.leverageTextBox1.Text = "/ 125";
            // 
            // orderPriceTextBox0
            // 
            this.orderPriceTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.orderPriceTextBox0.Location = new System.Drawing.Point(21, 42);
            this.orderPriceTextBox0.Name = "orderPriceTextBox0";
            this.orderPriceTextBox0.ReadOnly = true;
            this.orderPriceTextBox0.Size = new System.Drawing.Size(30, 14);
            this.orderPriceTextBox0.TabIndex = 6;
            this.orderPriceTextBox0.Text = "Price";
            // 
            // autoSizeCheckBox
            // 
            this.autoSizeCheckBox.AutoSize = true;
            this.autoSizeCheckBox.Location = new System.Drawing.Point(47, 97);
            this.autoSizeCheckBox.Name = "autoSizeCheckBox";
            this.autoSizeCheckBox.Size = new System.Drawing.Size(49, 16);
            this.autoSizeCheckBox.TabIndex = 4;
            this.autoSizeCheckBox.Text = "Auto";
            this.autoSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // miniSizeCheckBox
            // 
            this.miniSizeCheckBox.AutoSize = true;
            this.miniSizeCheckBox.Location = new System.Drawing.Point(133, 72);
            this.miniSizeCheckBox.Name = "miniSizeCheckBox";
            this.miniSizeCheckBox.Size = new System.Drawing.Size(106, 16);
            this.miniSizeCheckBox.TabIndex = 4;
            this.miniSizeCheckBox.Text = "Minimum Size";
            this.miniSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // ROCheckBox
            // 
            this.ROCheckBox.AutoSize = true;
            this.ROCheckBox.Checked = true;
            this.ROCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ROCheckBox.Location = new System.Drawing.Point(21, 132);
            this.ROCheckBox.Name = "ROCheckBox";
            this.ROCheckBox.Size = new System.Drawing.Size(97, 16);
            this.ROCheckBox.TabIndex = 4;
            this.ROCheckBox.Text = "Reduce Only";
            this.ROCheckBox.UseVisualStyleBackColor = true;
            // 
            // PORadioButton
            // 
            this.PORadioButton.AutoSize = true;
            this.PORadioButton.Location = new System.Drawing.Point(140, 20);
            this.PORadioButton.Name = "PORadioButton";
            this.PORadioButton.Size = new System.Drawing.Size(78, 16);
            this.PORadioButton.TabIndex = 3;
            this.PORadioButton.TabStop = true;
            this.PORadioButton.Text = "Post Only";
            this.PORadioButton.UseVisualStyleBackColor = true;
            // 
            // marketRadioButton
            // 
            this.marketRadioButton.AutoSize = true;
            this.marketRadioButton.Location = new System.Drawing.Point(133, 42);
            this.marketRadioButton.Name = "marketRadioButton";
            this.marketRadioButton.Size = new System.Drawing.Size(61, 16);
            this.marketRadioButton.TabIndex = 3;
            this.marketRadioButton.TabStop = true;
            this.marketRadioButton.Text = "Market";
            this.marketRadioButton.UseVisualStyleBackColor = true;
            // 
            // IOCRadioButton
            // 
            this.IOCRadioButton.AutoSize = true;
            this.IOCRadioButton.Location = new System.Drawing.Point(76, 20);
            this.IOCRadioButton.Name = "IOCRadioButton";
            this.IOCRadioButton.Size = new System.Drawing.Size(44, 16);
            this.IOCRadioButton.TabIndex = 3;
            this.IOCRadioButton.TabStop = true;
            this.IOCRadioButton.Text = "IOC";
            this.IOCRadioButton.UseVisualStyleBackColor = true;
            // 
            // GTCRadioButton
            // 
            this.GTCRadioButton.AutoSize = true;
            this.GTCRadioButton.Checked = true;
            this.GTCRadioButton.Location = new System.Drawing.Point(21, 20);
            this.GTCRadioButton.Name = "GTCRadioButton";
            this.GTCRadioButton.Size = new System.Drawing.Size(49, 16);
            this.GTCRadioButton.TabIndex = 3;
            this.GTCRadioButton.TabStop = true;
            this.GTCRadioButton.Text = "GTC";
            this.GTCRadioButton.UseVisualStyleBackColor = true;
            // 
            // sellButton
            // 
            this.sellButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.sellButton.Location = new System.Drawing.Point(133, 154);
            this.sellButton.Name = "sellButton";
            this.sellButton.Size = new System.Drawing.Size(117, 29);
            this.sellButton.TabIndex = 2;
            this.sellButton.Text = "Sell/Short";
            this.sellButton.UseVisualStyleBackColor = false;
            // 
            // buyButton
            // 
            this.buyButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.buyButton.Location = new System.Drawing.Point(10, 154);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(117, 29);
            this.buyButton.TabIndex = 2;
            this.buyButton.Text = "Buy/Long";
            this.buyButton.UseVisualStyleBackColor = false;
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(1274, 528);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(172, 142);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox.TabIndex = 10;
            this.pictureBox.TabStop = false;
            // 
            // resultListView
            // 
            this.resultListView.CellEditUseWholeCell = false;
            this.resultListView.DataSource = null;
            this.resultListView.HideSelection = false;
            this.resultListView.Location = new System.Drawing.Point(1115, 515);
            this.resultListView.Name = "resultListView";
            this.resultListView.ShowGroups = false;
            this.resultListView.Size = new System.Drawing.Size(121, 97);
            this.resultListView.TabIndex = 11;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.VirtualMode = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1458, 682);
            this.Controls.Add(this.resultListView);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.orderGroupBox);
            this.Controls.Add(this.FCGroupBox);
            this.Controls.Add(this.FUGroupBox);
            this.Controls.Add(this.spotGroupBox);
            this.Controls.Add(this.marketComboBox);
            this.Controls.Add(this.amtTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.timeDiffTextBox);
            this.Controls.Add(this.chartTabControl);
            this.Controls.Add(this.priceTextBox);
            this.Controls.Add(this.mainTabControl);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
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
            this.logTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).EndInit();
            this.chartTabControl.ResumeLayout(false);
            this.secChartTab.ResumeLayout(false);
            this.minChartTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.spotGroupBox.ResumeLayout(false);
            this.spotGroupBox.PerformLayout();
            this.FUGroupBox.ResumeLayout(false);
            this.FUGroupBox.PerformLayout();
            this.FCGroupBox.ResumeLayout(false);
            this.FCGroupBox.PerformLayout();
            this.orderGroupBox.ResumeLayout(false);
            this.orderGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).EndInit();
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
        private BrightIdeasSoftware.FastDataListView FUPositionListView;
        private BrightIdeasSoftware.FastDataListView FUOpenOrdersListView;
        private System.Windows.Forms.TextBox FUAvailBalanceTextBox1;
        private System.Windows.Forms.TextBox FUAvailBalanceTextBox0;
        private System.Windows.Forms.TextBox FUMarginBalanceTextBox1;
        private System.Windows.Forms.TextBox FUMarginBalanceTextBox0;
        private System.Windows.Forms.TextBox FUWalletBalanceTextBox1;
        private System.Windows.Forms.TextBox FUWalletBalanceTextBox0;
        private System.Windows.Forms.TextBox FUMaintMarginTextBox1;
        private System.Windows.Forms.TextBox FUMaintMarginTextBox0;
        private System.Windows.Forms.TextBox FUMarginRatioTextBox1;
        private System.Windows.Forms.TextBox FUMarginRatioTextBox0;
        private System.Windows.Forms.GroupBox spotGroupBox;
        private System.Windows.Forms.GroupBox FUGroupBox;
        private System.Windows.Forms.GroupBox FCGroupBox;
        private System.Windows.Forms.GroupBox orderGroupBox;
        private System.Windows.Forms.Button sellButton;
        private System.Windows.Forms.Button buyButton;
        private System.Windows.Forms.CheckBox ROCheckBox;
        private System.Windows.Forms.RadioButton PORadioButton;
        private System.Windows.Forms.RadioButton marketRadioButton;
        private System.Windows.Forms.RadioButton IOCRadioButton;
        private System.Windows.Forms.RadioButton GTCRadioButton;
        private System.Windows.Forms.TextBox orderPriceTextBox2;
        private System.Windows.Forms.TextBox orderPriceTextBox0;
        private System.Windows.Forms.TextBox orderSizeTextBox0;
        private System.Windows.Forms.CheckBox miniSizeCheckBox;
        private System.Windows.Forms.CheckBox autoSizeCheckBox;
        private System.Windows.Forms.TextBox orderSizeTextBox1;
        private System.Windows.Forms.TextBox autoSizeTextBox1;
        private System.Windows.Forms.TextBox autoSizeTextBox0;
        private System.Windows.Forms.TextBox orderPriceTextBox1;
        private System.Windows.Forms.TextBox leverageTextBox0;
        private System.Windows.Forms.TextBox leverageTextBox1;
        private System.Windows.Forms.PictureBox pictureBox;
        private BrightIdeasSoftware.FastDataListView resultListView;
    }
}

