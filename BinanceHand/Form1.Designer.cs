
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.secChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.FUListView = new BrightIdeasSoftware.FastDataListView();
            this.FUKlineRcvTextBox = new System.Windows.Forms.TextBox();
            this.FUKlineReqTextBox = new System.Windows.Forms.TextBox();
            this.FUAggRcvTextBox = new System.Windows.Forms.TextBox();
            this.FUAggReqTextBox = new System.Windows.Forms.TextBox();
            this.minChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.FUGroupBox = new System.Windows.Forms.GroupBox();
            this.realButton = new System.Windows.Forms.Button();
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
            this.autoTextBox = new System.Windows.Forms.TextBox();
            this.timeDiffTextBox = new System.Windows.Forms.TextBox();
            this.todayWinRateTextBox = new System.Windows.Forms.TextBox();
            this.totalWinRateTextBox = new System.Windows.Forms.TextBox();
            this.simulTodayWinRateTextBox = new System.Windows.Forms.TextBox();
            this.simulTotalWinRateTextBox = new System.Windows.Forms.TextBox();
            this.simulResultListView = new BrightIdeasSoftware.FastDataListView();
            this.assetsListView = new BrightIdeasSoftware.FastDataListView();
            this.mainResultGroupBox = new System.Windows.Forms.GroupBox();
            this.simulResultGroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.secChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FUListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minChart)).BeginInit();
            this.FUGroupBox.SuspendLayout();
            this.orderGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.simulResultListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.assetsListView)).BeginInit();
            this.mainResultGroupBox.SuspendLayout();
            this.simulResultGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // secChart
            // 
            this.secChart.BackColor = System.Drawing.Color.Black;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.secChart.Legends.Add(legend1);
            this.secChart.Location = new System.Drawing.Point(75, 43);
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
            this.minChart.BackColor = System.Drawing.Color.DimGray;
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.minChart.Legends.Add(legend2);
            this.minChart.Location = new System.Drawing.Point(125, 60);
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
            this.FUGroupBox.Controls.Add(this.realButton);
            this.FUGroupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FUGroupBox.Location = new System.Drawing.Point(658, 60);
            this.FUGroupBox.Name = "FUGroupBox";
            this.FUGroupBox.Size = new System.Drawing.Size(209, 98);
            this.FUGroupBox.TabIndex = 9;
            this.FUGroupBox.TabStop = false;
            this.FUGroupBox.Text = "Futures-USD";
            // 
            // realButton
            // 
            this.realButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.realButton.Location = new System.Drawing.Point(147, 29);
            this.realButton.Name = "realButton";
            this.realButton.Size = new System.Drawing.Size(43, 20);
            this.realButton.TabIndex = 12;
            this.realButton.Text = "Real";
            this.realButton.UseVisualStyleBackColor = true;
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
            this.orderGroupBox.Location = new System.Drawing.Point(305, 458);
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
            this.pictureBox.Location = new System.Drawing.Point(68, 66);
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
            this.resultListView.Location = new System.Drawing.Point(40, 44);
            this.resultListView.Name = "resultListView";
            this.resultListView.ShowGroups = false;
            this.resultListView.Size = new System.Drawing.Size(216, 189);
            this.resultListView.TabIndex = 11;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.VirtualMode = true;
            // 
            // hourChart
            // 
            this.hourChart.BackColor = System.Drawing.Color.Gray;
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.hourChart.Legends.Add(legend3);
            this.hourChart.Location = new System.Drawing.Point(178, 76);
            this.hourChart.Name = "hourChart";
            this.hourChart.Size = new System.Drawing.Size(214, 247);
            this.hourChart.TabIndex = 2;
            this.hourChart.Text = "chart3";
            // 
            // dayChart
            // 
            this.dayChart.BackColor = System.Drawing.Color.Silver;
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.dayChart.Legends.Add(legend4);
            this.dayChart.Location = new System.Drawing.Point(305, 76);
            this.dayChart.Name = "dayChart";
            this.dayChart.Size = new System.Drawing.Size(214, 247);
            this.dayChart.TabIndex = 3;
            this.dayChart.Text = "chart3";
            // 
            // secButton
            // 
            this.secButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.secButton.Location = new System.Drawing.Point(32, 12);
            this.secButton.Name = "secButton";
            this.secButton.Size = new System.Drawing.Size(20, 20);
            this.secButton.TabIndex = 12;
            this.secButton.Text = "S";
            this.secButton.UseVisualStyleBackColor = true;
            // 
            // minButton
            // 
            this.minButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.minButton.Location = new System.Drawing.Point(58, 12);
            this.minButton.Name = "minButton";
            this.minButton.Size = new System.Drawing.Size(20, 20);
            this.minButton.TabIndex = 12;
            this.minButton.Text = "M";
            this.minButton.UseVisualStyleBackColor = true;
            // 
            // hourButton
            // 
            this.hourButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.hourButton.Location = new System.Drawing.Point(84, 12);
            this.hourButton.Name = "hourButton";
            this.hourButton.Size = new System.Drawing.Size(20, 20);
            this.hourButton.TabIndex = 12;
            this.hourButton.Text = "H";
            this.hourButton.UseVisualStyleBackColor = true;
            // 
            // dayButton
            // 
            this.dayButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dayButton.Location = new System.Drawing.Point(110, 12);
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
            this.gridItvTextBox.Location = new System.Drawing.Point(438, 423);
            this.gridItvTextBox.Name = "gridItvTextBox";
            this.gridItvTextBox.ReadOnly = true;
            this.gridItvTextBox.Size = new System.Drawing.Size(46, 14);
            this.gridItvTextBox.TabIndex = 6;
            this.gridItvTextBox.Text = "0.0%";
            // 
            // autoTextBox
            // 
            this.autoTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.autoTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.autoTextBox.Location = new System.Drawing.Point(512, 423);
            this.autoTextBox.Name = "autoTextBox";
            this.autoTextBox.ReadOnly = true;
            this.autoTextBox.Size = new System.Drawing.Size(56, 14);
            this.autoTextBox.TabIndex = 6;
            this.autoTextBox.Text = "0000";
            // 
            // timeDiffTextBox
            // 
            this.timeDiffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeDiffTextBox.Location = new System.Drawing.Point(339, 423);
            this.timeDiffTextBox.Name = "timeDiffTextBox";
            this.timeDiffTextBox.ReadOnly = true;
            this.timeDiffTextBox.Size = new System.Drawing.Size(52, 14);
            this.timeDiffTextBox.TabIndex = 6;
            this.timeDiffTextBox.Text = "0.0";
            // 
            // todayWinRateTextBox
            // 
            this.todayWinRateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.todayWinRateTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.todayWinRateTextBox.Location = new System.Drawing.Point(135, 20);
            this.todayWinRateTextBox.Name = "todayWinRateTextBox";
            this.todayWinRateTextBox.ReadOnly = true;
            this.todayWinRateTextBox.Size = new System.Drawing.Size(89, 14);
            this.todayWinRateTextBox.TabIndex = 6;
            this.todayWinRateTextBox.Text = "0.00(000) 0.00";
            // 
            // totalWinRateTextBox
            // 
            this.totalWinRateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.totalWinRateTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.totalWinRateTextBox.Location = new System.Drawing.Point(40, 20);
            this.totalWinRateTextBox.Name = "totalWinRateTextBox";
            this.totalWinRateTextBox.ReadOnly = true;
            this.totalWinRateTextBox.Size = new System.Drawing.Size(89, 14);
            this.totalWinRateTextBox.TabIndex = 6;
            this.totalWinRateTextBox.Text = "0.00(000) 0.00";
            // 
            // simulTodayWinRateTextBox
            // 
            this.simulTodayWinRateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.simulTodayWinRateTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.simulTodayWinRateTextBox.Location = new System.Drawing.Point(154, 47);
            this.simulTodayWinRateTextBox.Name = "simulTodayWinRateTextBox";
            this.simulTodayWinRateTextBox.ReadOnly = true;
            this.simulTodayWinRateTextBox.Size = new System.Drawing.Size(87, 14);
            this.simulTodayWinRateTextBox.TabIndex = 6;
            this.simulTodayWinRateTextBox.Text = "0.00(000) 0.00";
            // 
            // simulTotalWinRateTextBox
            // 
            this.simulTotalWinRateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.simulTotalWinRateTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.simulTotalWinRateTextBox.Location = new System.Drawing.Point(60, 47);
            this.simulTotalWinRateTextBox.Name = "simulTotalWinRateTextBox";
            this.simulTotalWinRateTextBox.ReadOnly = true;
            this.simulTotalWinRateTextBox.Size = new System.Drawing.Size(88, 14);
            this.simulTotalWinRateTextBox.TabIndex = 6;
            this.simulTotalWinRateTextBox.Text = "0.00(000) 0.00";
            // 
            // simulResultListView
            // 
            this.simulResultListView.CellEditUseWholeCell = false;
            this.simulResultListView.DataSource = null;
            this.simulResultListView.HideSelection = false;
            this.simulResultListView.Location = new System.Drawing.Point(25, 62);
            this.simulResultListView.Name = "simulResultListView";
            this.simulResultListView.ShowGroups = false;
            this.simulResultListView.Size = new System.Drawing.Size(216, 189);
            this.simulResultListView.TabIndex = 11;
            this.simulResultListView.UseCompatibleStateImageBehavior = false;
            this.simulResultListView.View = System.Windows.Forms.View.Details;
            this.simulResultListView.VirtualMode = true;
            // 
            // assetsListView
            // 
            this.assetsListView.CellEditUseWholeCell = false;
            this.assetsListView.DataSource = null;
            this.assetsListView.HideSelection = false;
            this.assetsListView.Location = new System.Drawing.Point(51, 458);
            this.assetsListView.Name = "assetsListView";
            this.assetsListView.ShowGroups = false;
            this.assetsListView.Size = new System.Drawing.Size(216, 189);
            this.assetsListView.TabIndex = 11;
            this.assetsListView.UseCompatibleStateImageBehavior = false;
            this.assetsListView.View = System.Windows.Forms.View.Details;
            this.assetsListView.VirtualMode = true;
            // 
            // mainResultGroupBox
            // 
            this.mainResultGroupBox.Controls.Add(this.totalWinRateTextBox);
            this.mainResultGroupBox.Controls.Add(this.todayWinRateTextBox);
            this.mainResultGroupBox.Controls.Add(this.pictureBox);
            this.mainResultGroupBox.Controls.Add(this.resultListView);
            this.mainResultGroupBox.Location = new System.Drawing.Point(685, 409);
            this.mainResultGroupBox.Name = "mainResultGroupBox";
            this.mainResultGroupBox.Size = new System.Drawing.Size(302, 261);
            this.mainResultGroupBox.TabIndex = 13;
            this.mainResultGroupBox.TabStop = false;
            this.mainResultGroupBox.Text = "Main";
            // 
            // simulResultGroupBox
            // 
            this.simulResultGroupBox.Controls.Add(this.simulResultListView);
            this.simulResultGroupBox.Controls.Add(this.simulTodayWinRateTextBox);
            this.simulResultGroupBox.Controls.Add(this.simulTotalWinRateTextBox);
            this.simulResultGroupBox.Location = new System.Drawing.Point(1022, 367);
            this.simulResultGroupBox.Name = "simulResultGroupBox";
            this.simulResultGroupBox.Size = new System.Drawing.Size(306, 280);
            this.simulResultGroupBox.TabIndex = 13;
            this.simulResultGroupBox.TabStop = false;
            this.simulResultGroupBox.Text = "Simulation";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1458, 682);
            this.Controls.Add(this.simulResultGroupBox);
            this.Controls.Add(this.mainResultGroupBox);
            this.Controls.Add(this.assetsListView);
            this.Controls.Add(this.dayButton);
            this.Controls.Add(this.hourButton);
            this.Controls.Add(this.minButton);
            this.Controls.Add(this.secButton);
            this.Controls.Add(this.dayChart);
            this.Controls.Add(this.hourChart);
            this.Controls.Add(this.minChart);
            this.Controls.Add(this.secChart);
            this.Controls.Add(this.orderGroupBox);
            this.Controls.Add(this.FUGroupBox);
            this.Controls.Add(this.autoTextBox);
            this.Controls.Add(this.gridItvTextBox);
            this.Controls.Add(this.timeDiffTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.secChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FUListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minChart)).EndInit();
            this.FUGroupBox.ResumeLayout(false);
            this.FUGroupBox.PerformLayout();
            this.orderGroupBox.ResumeLayout(false);
            this.orderGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hourChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dayChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.simulResultListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.assetsListView)).EndInit();
            this.mainResultGroupBox.ResumeLayout(false);
            this.mainResultGroupBox.PerformLayout();
            this.simulResultGroupBox.ResumeLayout(false);
            this.simulResultGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart secChart;
        private BrightIdeasSoftware.FastDataListView FUListView;
        private System.Windows.Forms.TextBox FUKlineRcvTextBox;
        private System.Windows.Forms.TextBox FUKlineReqTextBox;
        private System.Windows.Forms.TextBox FUAggRcvTextBox;
        private System.Windows.Forms.TextBox FUAggReqTextBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart minChart;
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
        private System.Windows.Forms.TextBox autoTextBox;
        private System.Windows.Forms.Button realButton;
        private System.Windows.Forms.TextBox timeDiffTextBox;
        private System.Windows.Forms.TextBox todayWinRateTextBox;
        private System.Windows.Forms.TextBox totalWinRateTextBox;
        private System.Windows.Forms.TextBox simulTodayWinRateTextBox;
        private System.Windows.Forms.TextBox simulTotalWinRateTextBox;
        private BrightIdeasSoftware.FastDataListView simulResultListView;
        private BrightIdeasSoftware.FastDataListView assetsListView;
        private System.Windows.Forms.GroupBox mainResultGroupBox;
        private System.Windows.Forms.GroupBox simulResultGroupBox;
    }
}

