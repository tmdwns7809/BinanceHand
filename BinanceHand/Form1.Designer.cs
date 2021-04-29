
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
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            this.secChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.FUListView = new BrightIdeasSoftware.FastDataListView();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.FUTab = new System.Windows.Forms.TabPage();
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
            this.FUTabControl = new System.Windows.Forms.TabControl();
            this.FUPositionTab = new System.Windows.Forms.TabPage();
            this.FUPositionListView = new BrightIdeasSoftware.FastDataListView();
            this.FUOpenOrdersTab = new System.Windows.Forms.TabPage();
            this.FUOpenOrdersListView = new BrightIdeasSoftware.FastDataListView();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.timeDiffTextBox = new System.Windows.Forms.TextBox();
            this.priceTextBox = new System.Windows.Forms.TextBox();
            this.amtTextBox = new System.Windows.Forms.TextBox();
            this.marketComboBox = new System.Windows.Forms.ComboBox();
            this.FUKlineRcvTextBox = new System.Windows.Forms.TextBox();
            this.FUKlineReqTextBox = new System.Windows.Forms.TextBox();
            this.FUAggRcvTextBox = new System.Windows.Forms.TextBox();
            this.FUAggReqTextBox = new System.Windows.Forms.TextBox();
            this.minChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.FUGroupBox = new System.Windows.Forms.GroupBox();
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
            this.hourChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.dayChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.secButton = new System.Windows.Forms.Button();
            this.minButton = new System.Windows.Forms.Button();
            this.hourButton = new System.Windows.Forms.Button();
            this.dayButton = new System.Windows.Forms.Button();
            this.gridItvTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.secChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FUListView)).BeginInit();
            this.mainTabControl.SuspendLayout();
            this.FUTab.SuspendLayout();
            this.FUTabControl.SuspendLayout();
            this.FUPositionTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUPositionListView)).BeginInit();
            this.FUOpenOrdersTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FUOpenOrdersListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minChart)).BeginInit();
            this.FUGroupBox.SuspendLayout();
            this.orderGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayChart)).BeginInit();
            this.SuspendLayout();
            // 
            // secChart
            // 
            this.secChart.BackColor = System.Drawing.Color.Black;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.secChart.Legends.Add(legend1);
            this.secChart.Location = new System.Drawing.Point(512, 109);
            this.secChart.Name = "secChart";
            this.secChart.Size = new System.Drawing.Size(224, 200);
            this.secChart.TabIndex = 0;
            this.secChart.Text = "chart1";
            // 
            // FUListView
            // 
            this.FUListView.CellEditUseWholeCell = false;
            this.FUListView.DataSource = null;
            this.FUListView.HideSelection = false;
            this.FUListView.Location = new System.Drawing.Point(9, 20);
            this.FUListView.Name = "FUListView";
            this.FUListView.ShowGroups = false;
            this.FUListView.Size = new System.Drawing.Size(121, 49);
            this.FUListView.TabIndex = 4;
            this.FUListView.UseCompatibleStateImageBehavior = false;
            this.FUListView.View = System.Windows.Forms.View.Details;
            this.FUListView.VirtualMode = true;
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.FUTab);
            this.mainTabControl.Location = new System.Drawing.Point(12, 474);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(728, 188);
            this.mainTabControl.TabIndex = 5;
            // 
            // FUTab
            // 
            this.FUTab.Controls.Add(this.FUWalletBalanceTextBox1);
            this.FUTab.Controls.Add(this.FUAvailBalanceTextBox1);
            this.FUTab.Controls.Add(this.FUWalletBalanceTextBox0);
            this.FUTab.Controls.Add(this.FUAvailBalanceTextBox0);
            this.FUTab.Controls.Add(this.FUMarginRatioTextBox1);
            this.FUTab.Controls.Add(this.FUMaintMarginTextBox1);
            this.FUTab.Controls.Add(this.FUMarginBalanceTextBox1);
            this.FUTab.Controls.Add(this.FUMarginRatioTextBox0);
            this.FUTab.Controls.Add(this.FUMaintMarginTextBox0);
            this.FUTab.Controls.Add(this.FUMarginBalanceTextBox0);
            this.FUTab.Controls.Add(this.FUTabControl);
            this.FUTab.Location = new System.Drawing.Point(4, 22);
            this.FUTab.Name = "FUTab";
            this.FUTab.Padding = new System.Windows.Forms.Padding(3);
            this.FUTab.Size = new System.Drawing.Size(720, 162);
            this.FUTab.TabIndex = 2;
            this.FUTab.Text = "Futures-USD";
            this.FUTab.UseVisualStyleBackColor = true;
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
            // FUTabControl
            // 
            this.FUTabControl.Controls.Add(this.FUPositionTab);
            this.FUTabControl.Controls.Add(this.FUOpenOrdersTab);
            this.FUTabControl.Location = new System.Drawing.Point(3, 3);
            this.FUTabControl.Name = "FUTabControl";
            this.FUTabControl.SelectedIndex = 0;
            this.FUTabControl.Size = new System.Drawing.Size(604, 154);
            this.FUTabControl.TabIndex = 1;
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
            // nameTextBox
            // 
            this.nameTextBox.BackColor = System.Drawing.Color.White;
            this.nameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.nameTextBox.Location = new System.Drawing.Point(86, 12);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(90, 21);
            this.nameTextBox.TabIndex = 6;
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
            this.amtTextBox.Location = new System.Drawing.Point(742, 440);
            this.amtTextBox.Name = "amtTextBox";
            this.amtTextBox.ReadOnly = true;
            this.amtTextBox.Size = new System.Drawing.Size(63, 14);
            this.amtTextBox.TabIndex = 6;
            this.amtTextBox.Text = "0.0000000";
            // 
            // marketComboBox
            // 
            this.marketComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.marketComboBox.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.marketComboBox.FormattingEnabled = true;
            this.marketComboBox.Items.AddRange(new object[] {
            "F_U"});
            this.marketComboBox.Location = new System.Drawing.Point(35, 12);
            this.marketComboBox.Name = "marketComboBox";
            this.marketComboBox.Size = new System.Drawing.Size(50, 20);
            this.marketComboBox.TabIndex = 7;
            // 
            // FUKlineRcvTextBox
            // 
            this.FUKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUKlineRcvTextBox.Location = new System.Drawing.Point(9, 75);
            this.FUKlineRcvTextBox.Name = "FUKlineRcvTextBox";
            this.FUKlineRcvTextBox.ReadOnly = true;
            this.FUKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.FUKlineRcvTextBox.TabIndex = 6;
            this.FUKlineRcvTextBox.Text = "000";
            // 
            // FUKlineReqTextBox
            // 
            this.FUKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUKlineReqTextBox.Location = new System.Drawing.Point(38, 75);
            this.FUKlineReqTextBox.Name = "FUKlineReqTextBox";
            this.FUKlineReqTextBox.ReadOnly = true;
            this.FUKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.FUKlineReqTextBox.TabIndex = 6;
            this.FUKlineReqTextBox.Text = "/000(K)";
            // 
            // FUAggRcvTextBox
            // 
            this.FUAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUAggRcvTextBox.Location = new System.Drawing.Point(100, 75);
            this.FUAggRcvTextBox.Name = "FUAggRcvTextBox";
            this.FUAggRcvTextBox.ReadOnly = true;
            this.FUAggRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.FUAggRcvTextBox.TabIndex = 6;
            this.FUAggRcvTextBox.Text = "0";
            // 
            // FUAggReqTextBox
            // 
            this.FUAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.FUAggReqTextBox.Location = new System.Drawing.Point(132, 75);
            this.FUAggReqTextBox.Name = "FUAggReqTextBox";
            this.FUAggReqTextBox.ReadOnly = true;
            this.FUAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.FUAggReqTextBox.TabIndex = 6;
            this.FUAggReqTextBox.Text = "/0(A)";
            // 
            // minChart
            // 
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.minChart.Legends.Add(legend2);
            this.minChart.Location = new System.Drawing.Point(562, 126);
            this.minChart.Name = "minChart";
            this.minChart.Size = new System.Drawing.Size(214, 247);
            this.minChart.TabIndex = 1;
            this.minChart.Text = "chart2";
            // 
            // FUGroupBox
            // 
            this.FUGroupBox.Controls.Add(this.FUListView);
            this.FUGroupBox.Controls.Add(this.FUKlineRcvTextBox);
            this.FUGroupBox.Controls.Add(this.FUKlineReqTextBox);
            this.FUGroupBox.Controls.Add(this.FUAggRcvTextBox);
            this.FUGroupBox.Controls.Add(this.FUAggReqTextBox);
            this.FUGroupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FUGroupBox.Location = new System.Drawing.Point(1205, 97);
            this.FUGroupBox.Name = "FUGroupBox";
            this.FUGroupBox.Size = new System.Drawing.Size(209, 98);
            this.FUGroupBox.TabIndex = 9;
            this.FUGroupBox.TabStop = false;
            this.FUGroupBox.Text = "Futures-USD";
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
            this.leverageTextBox0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leverageTextBox0.Location = new System.Drawing.Point(263, 41);
            this.leverageTextBox0.Name = "leverageTextBox0";
            this.leverageTextBox0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.leverageTextBox0.Size = new System.Drawing.Size(41, 21);
            this.leverageTextBox0.TabIndex = 9;
            // 
            // orderPriceTextBox1
            // 
            this.orderPriceTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.orderPriceTextBox1.Location = new System.Drawing.Point(65, 39);
            this.orderPriceTextBox1.Name = "orderPriceTextBox1";
            this.orderPriceTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
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
            this.autoSizeTextBox0.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.autoSizeTextBox0.Size = new System.Drawing.Size(30, 21);
            this.autoSizeTextBox0.TabIndex = 8;
            this.autoSizeTextBox0.Text = "1";
            // 
            // orderSizeTextBox1
            // 
            this.orderSizeTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.orderSizeTextBox1.Location = new System.Drawing.Point(65, 70);
            this.orderSizeTextBox1.Name = "orderSizeTextBox1";
            this.orderSizeTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
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
            this.miniSizeCheckBox.BackColor = System.Drawing.Color.Transparent;
            this.miniSizeCheckBox.Location = new System.Drawing.Point(133, 72);
            this.miniSizeCheckBox.Name = "miniSizeCheckBox";
            this.miniSizeCheckBox.Size = new System.Drawing.Size(106, 16);
            this.miniSizeCheckBox.TabIndex = 4;
            this.miniSizeCheckBox.Text = "Minimum Size";
            this.miniSizeCheckBox.UseVisualStyleBackColor = false;
            // 
            // ROCheckBox
            // 
            this.ROCheckBox.AutoSize = true;
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
            this.PORadioButton.BackColor = System.Drawing.Color.Transparent;
            this.PORadioButton.Location = new System.Drawing.Point(140, 20);
            this.PORadioButton.Name = "PORadioButton";
            this.PORadioButton.Size = new System.Drawing.Size(78, 16);
            this.PORadioButton.TabIndex = 3;
            this.PORadioButton.TabStop = true;
            this.PORadioButton.Text = "Post Only";
            this.PORadioButton.UseVisualStyleBackColor = false;
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
            this.sellButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
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
            this.buyButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buyButton.Location = new System.Drawing.Point(10, 154);
            this.buyButton.Name = "buyButton";
            this.buyButton.Size = new System.Drawing.Size(117, 29);
            this.buyButton.TabIndex = 2;
            this.buyButton.Text = "Buy/Long";
            this.buyButton.UseVisualStyleBackColor = false;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            this.resultListView.Location = new System.Drawing.Point(1139, 539);
            this.resultListView.Name = "resultListView";
            this.resultListView.ShowGroups = false;
            this.resultListView.Size = new System.Drawing.Size(121, 97);
            this.resultListView.TabIndex = 11;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.VirtualMode = true;
            // 
            // hourChart
            // 
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.hourChart.Legends.Add(legend3);
            this.hourChart.Location = new System.Drawing.Point(659, 126);
            this.hourChart.Name = "hourChart";
            this.hourChart.Size = new System.Drawing.Size(214, 247);
            this.hourChart.TabIndex = 2;
            this.hourChart.Text = "chart3";
            // 
            // dayChart
            // 
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.dayChart.Legends.Add(legend4);
            this.dayChart.Location = new System.Drawing.Point(742, 126);
            this.dayChart.Name = "dayChart";
            this.dayChart.Size = new System.Drawing.Size(214, 247);
            this.dayChart.TabIndex = 3;
            this.dayChart.Text = "chart3";
            // 
            // secButton
            // 
            this.secButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.secButton.Location = new System.Drawing.Point(323, 66);
            this.secButton.Name = "secButton";
            this.secButton.Size = new System.Drawing.Size(20, 20);
            this.secButton.TabIndex = 12;
            this.secButton.Text = "S";
            this.secButton.UseVisualStyleBackColor = true;
            // 
            // minButton
            // 
            this.minButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.minButton.Location = new System.Drawing.Point(349, 66);
            this.minButton.Name = "minButton";
            this.minButton.Size = new System.Drawing.Size(20, 20);
            this.minButton.TabIndex = 12;
            this.minButton.Text = "M";
            this.minButton.UseVisualStyleBackColor = true;
            // 
            // hourButton
            // 
            this.hourButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.hourButton.Location = new System.Drawing.Point(375, 66);
            this.hourButton.Name = "hourButton";
            this.hourButton.Size = new System.Drawing.Size(20, 20);
            this.hourButton.TabIndex = 12;
            this.hourButton.Text = "H";
            this.hourButton.UseVisualStyleBackColor = true;
            // 
            // dayButton
            // 
            this.dayButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dayButton.Location = new System.Drawing.Point(401, 66);
            this.dayButton.Name = "dayButton";
            this.dayButton.Size = new System.Drawing.Size(20, 20);
            this.dayButton.TabIndex = 12;
            this.dayButton.Text = "D";
            this.dayButton.UseVisualStyleBackColor = true;
            // 
            // gridItvTextBox
            // 
            this.gridItvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridItvTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.gridItvTextBox.Location = new System.Drawing.Point(618, 440);
            this.gridItvTextBox.Name = "gridItvTextBox";
            this.gridItvTextBox.ReadOnly = true;
            this.gridItvTextBox.Size = new System.Drawing.Size(46, 14);
            this.gridItvTextBox.TabIndex = 6;
            this.gridItvTextBox.Text = "0.0%";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1458, 682);
            this.Controls.Add(this.dayButton);
            this.Controls.Add(this.hourButton);
            this.Controls.Add(this.minButton);
            this.Controls.Add(this.secButton);
            this.Controls.Add(this.dayChart);
            this.Controls.Add(this.hourChart);
            this.Controls.Add(this.minChart);
            this.Controls.Add(this.secChart);
            this.Controls.Add(this.resultListView);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.orderGroupBox);
            this.Controls.Add(this.FUGroupBox);
            this.Controls.Add(this.marketComboBox);
            this.Controls.Add(this.gridItvTextBox);
            this.Controls.Add(this.amtTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.timeDiffTextBox);
            this.Controls.Add(this.priceTextBox);
            this.Controls.Add(this.mainTabControl);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.secChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FUListView)).EndInit();
            this.mainTabControl.ResumeLayout(false);
            this.FUTab.ResumeLayout(false);
            this.FUTab.PerformLayout();
            this.FUTabControl.ResumeLayout(false);
            this.FUPositionTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUPositionListView)).EndInit();
            this.FUOpenOrdersTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FUOpenOrdersListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minChart)).EndInit();
            this.FUGroupBox.ResumeLayout(false);
            this.FUGroupBox.PerformLayout();
            this.orderGroupBox.ResumeLayout(false);
            this.orderGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart secChart;
        private BrightIdeasSoftware.FastDataListView FUListView;
        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox timeDiffTextBox;
        private System.Windows.Forms.TextBox priceTextBox;
        private System.Windows.Forms.TextBox amtTextBox;
        private System.Windows.Forms.ComboBox marketComboBox;
        private System.Windows.Forms.TextBox FUKlineRcvTextBox;
        private System.Windows.Forms.TextBox FUKlineReqTextBox;
        private System.Windows.Forms.TextBox FUAggRcvTextBox;
        private System.Windows.Forms.TextBox FUAggReqTextBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart minChart;
        private System.Windows.Forms.TabPage FUTab;
        private System.Windows.Forms.TabControl FUTabControl;
        private System.Windows.Forms.TabPage FUPositionTab;
        private System.Windows.Forms.TabPage FUOpenOrdersTab;
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
        private System.Windows.Forms.GroupBox FUGroupBox;
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
        private System.Windows.Forms.DataVisualization.Charting.Chart hourChart;
        private System.Windows.Forms.DataVisualization.Charting.Chart dayChart;
        private System.Windows.Forms.Button secButton;
        private System.Windows.Forms.Button minButton;
        private System.Windows.Forms.Button hourButton;
        private System.Windows.Forms.Button dayButton;
        private System.Windows.Forms.TextBox gridItvTextBox;
    }
}

