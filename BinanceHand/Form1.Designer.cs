
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
            this.logTabControl = new System.Windows.Forms.TabControl();
            this.logTabPage = new System.Windows.Forms.TabPage();
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
            this.chartTabPageSec = new System.Windows.Forms.TabPage();
            this.chartTabPageMin = new System.Windows.Forms.TabPage();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureUListView)).BeginInit();
            this.logTabControl.SuspendLayout();
            this.logTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).BeginInit();
            this.chartTabControl.SuspendLayout();
            this.chartTabPageSec.SuspendLayout();
            this.chartTabPageMin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.SuspendLayout();
            // 
            // chart1
            // 
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(131, 66);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(980, 527);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // spotListView
            // 
            this.spotListView.CellEditUseWholeCell = false;
            this.spotListView.DataSource = null;
            this.spotListView.HideSelection = false;
            this.spotListView.Location = new System.Drawing.Point(1030, 844);
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
            this.futureUListView.Location = new System.Drawing.Point(1225, 844);
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
            // logTabControl
            // 
            this.logTabControl.Controls.Add(this.logTabPage);
            this.logTabControl.Location = new System.Drawing.Point(1, 820);
            this.logTabControl.Name = "logTabControl";
            this.logTabControl.SelectedIndex = 0;
            this.logTabControl.Size = new System.Drawing.Size(796, 188);
            this.logTabControl.TabIndex = 5;
            // 
            // logTabPage
            // 
            this.logTabPage.Controls.Add(this.logListBox);
            this.logTabPage.Location = new System.Drawing.Point(4, 22);
            this.logTabPage.Name = "logTabPage";
            this.logTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.logTabPage.Size = new System.Drawing.Size(788, 162);
            this.logTabPage.TabIndex = 0;
            this.logTabPage.Text = "Log";
            this.logTabPage.UseVisualStyleBackColor = true;
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 12;
            this.logListBox.Location = new System.Drawing.Point(0, 0);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(313, 160);
            this.logListBox.TabIndex = 0;
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
            this.timeDiffTextBox.Location = new System.Drawing.Point(1488, 809);
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
            this.priceTextBox.Location = new System.Drawing.Point(1419, 793);
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
            this.amtTextBox.Location = new System.Drawing.Point(1419, 809);
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
            this.futureCListView.Location = new System.Drawing.Point(1419, 844);
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
            this.spotKlineRcvTextBox.Location = new System.Drawing.Point(1030, 994);
            this.spotKlineRcvTextBox.Name = "spotKlineRcvTextBox";
            this.spotKlineRcvTextBox.ReadOnly = true;
            this.spotKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.spotKlineRcvTextBox.TabIndex = 6;
            this.spotKlineRcvTextBox.Text = "0000";
            // 
            // spotKlineReqTextBox
            // 
            this.spotKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotKlineReqTextBox.Location = new System.Drawing.Point(1059, 994);
            this.spotKlineReqTextBox.Name = "spotKlineReqTextBox";
            this.spotKlineReqTextBox.ReadOnly = true;
            this.spotKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotKlineReqTextBox.TabIndex = 6;
            this.spotKlineReqTextBox.Text = "/0000(K)";
            // 
            // spotAggRcvTextBox
            // 
            this.spotAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggRcvTextBox.Location = new System.Drawing.Point(1135, 994);
            this.spotAggRcvTextBox.Name = "spotAggRcvTextBox";
            this.spotAggRcvTextBox.ReadOnly = true;
            this.spotAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.spotAggRcvTextBox.TabIndex = 6;
            this.spotAggRcvTextBox.Text = "0";
            // 
            // spotAggReqTextBox
            // 
            this.spotAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.spotAggReqTextBox.Location = new System.Drawing.Point(1147, 994);
            this.spotAggReqTextBox.Name = "spotAggReqTextBox";
            this.spotAggReqTextBox.ReadOnly = true;
            this.spotAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.spotAggReqTextBox.TabIndex = 6;
            this.spotAggReqTextBox.Text = "/0(A)";
            // 
            // futureUKlineRcvTextBox
            // 
            this.futureUKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineRcvTextBox.Location = new System.Drawing.Point(1225, 994);
            this.futureUKlineRcvTextBox.Name = "futureUKlineRcvTextBox";
            this.futureUKlineRcvTextBox.ReadOnly = true;
            this.futureUKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureUKlineRcvTextBox.TabIndex = 6;
            this.futureUKlineRcvTextBox.Text = "000";
            // 
            // futureUKlineReqTextBox
            // 
            this.futureUKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUKlineReqTextBox.Location = new System.Drawing.Point(1254, 994);
            this.futureUKlineReqTextBox.Name = "futureUKlineReqTextBox";
            this.futureUKlineReqTextBox.ReadOnly = true;
            this.futureUKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUKlineReqTextBox.TabIndex = 6;
            this.futureUKlineReqTextBox.Text = "/000(K)";
            // 
            // futureUAggRcvTextBox
            // 
            this.futureUAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggRcvTextBox.Location = new System.Drawing.Point(1330, 994);
            this.futureUAggRcvTextBox.Name = "futureUAggRcvTextBox";
            this.futureUAggRcvTextBox.ReadOnly = true;
            this.futureUAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureUAggRcvTextBox.TabIndex = 6;
            this.futureUAggRcvTextBox.Text = "0";
            // 
            // futureUAggReqTextBox
            // 
            this.futureUAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureUAggReqTextBox.Location = new System.Drawing.Point(1342, 994);
            this.futureUAggReqTextBox.Name = "futureUAggReqTextBox";
            this.futureUAggReqTextBox.ReadOnly = true;
            this.futureUAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureUAggReqTextBox.TabIndex = 6;
            this.futureUAggReqTextBox.Text = "/0(A)";
            // 
            // futureCKlineRcvTextBox
            // 
            this.futureCKlineRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineRcvTextBox.Location = new System.Drawing.Point(1419, 994);
            this.futureCKlineRcvTextBox.Name = "futureCKlineRcvTextBox";
            this.futureCKlineRcvTextBox.ReadOnly = true;
            this.futureCKlineRcvTextBox.Size = new System.Drawing.Size(26, 14);
            this.futureCKlineRcvTextBox.TabIndex = 6;
            this.futureCKlineRcvTextBox.Text = "000";
            // 
            // futureCKlineReqTextBox
            // 
            this.futureCKlineReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCKlineReqTextBox.Location = new System.Drawing.Point(1448, 994);
            this.futureCKlineReqTextBox.Name = "futureCKlineReqTextBox";
            this.futureCKlineReqTextBox.ReadOnly = true;
            this.futureCKlineReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureCKlineReqTextBox.TabIndex = 6;
            this.futureCKlineReqTextBox.Text = "/000(K)";
            // 
            // futureCAggRcvTextBox
            // 
            this.futureCAggRcvTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggRcvTextBox.Location = new System.Drawing.Point(1524, 994);
            this.futureCAggRcvTextBox.Name = "futureCAggRcvTextBox";
            this.futureCAggRcvTextBox.ReadOnly = true;
            this.futureCAggRcvTextBox.Size = new System.Drawing.Size(10, 14);
            this.futureCAggRcvTextBox.TabIndex = 6;
            this.futureCAggRcvTextBox.Text = "0";
            // 
            // futureCAggReqTextBox
            // 
            this.futureCAggReqTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.futureCAggReqTextBox.Location = new System.Drawing.Point(1536, 994);
            this.futureCAggReqTextBox.Name = "futureCAggReqTextBox";
            this.futureCAggReqTextBox.ReadOnly = true;
            this.futureCAggReqTextBox.Size = new System.Drawing.Size(47, 14);
            this.futureCAggReqTextBox.TabIndex = 6;
            this.futureCAggReqTextBox.Text = "/0(A)";
            // 
            // chartTabControl
            // 
            this.chartTabControl.Controls.Add(this.chartTabPageSec);
            this.chartTabControl.Controls.Add(this.chartTabPageMin);
            this.chartTabControl.Location = new System.Drawing.Point(30, 22);
            this.chartTabControl.Multiline = true;
            this.chartTabControl.Name = "chartTabControl";
            this.chartTabControl.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chartTabControl.RightToLeftLayout = true;
            this.chartTabControl.SelectedIndex = 0;
            this.chartTabControl.Size = new System.Drawing.Size(1519, 765);
            this.chartTabControl.TabIndex = 8;
            // 
            // chartTabPageSec
            // 
            this.chartTabPageSec.Controls.Add(this.chart1);
            this.chartTabPageSec.Location = new System.Drawing.Point(4, 22);
            this.chartTabPageSec.Name = "chartTabPageSec";
            this.chartTabPageSec.Padding = new System.Windows.Forms.Padding(30);
            this.chartTabPageSec.Size = new System.Drawing.Size(1511, 739);
            this.chartTabPageSec.TabIndex = 0;
            this.chartTabPageSec.Text = "Seconds";
            this.chartTabPageSec.UseVisualStyleBackColor = true;
            // 
            // chartTabPageMin
            // 
            this.chartTabPageMin.Controls.Add(this.chart2);
            this.chartTabPageMin.Location = new System.Drawing.Point(4, 22);
            this.chartTabPageMin.Name = "chartTabPageMin";
            this.chartTabPageMin.Padding = new System.Windows.Forms.Padding(3);
            this.chartTabPageMin.Size = new System.Drawing.Size(1511, 739);
            this.chartTabPageMin.TabIndex = 1;
            this.chartTabPageMin.Text = "Minutes";
            this.chartTabPageMin.UseVisualStyleBackColor = true;
            // 
            // chart2
            // 
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.chart2.Legends.Add(legend2);
            this.chart2.Location = new System.Drawing.Point(0, 0);
            this.chart2.Name = "chart2";
            this.chart2.Size = new System.Drawing.Size(980, 527);
            this.chart2.TabIndex = 1;
            this.chart2.Text = "chart2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1011);
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
            this.Controls.Add(this.logTabControl);
            this.Controls.Add(this.futureCListView);
            this.Controls.Add(this.futureUListView);
            this.Controls.Add(this.spotListView);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureUListView)).EndInit();
            this.logTabControl.ResumeLayout(false);
            this.logTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.futureCListView)).EndInit();
            this.chartTabControl.ResumeLayout(false);
            this.chartTabPageSec.ResumeLayout(false);
            this.chartTabPageMin.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private BrightIdeasSoftware.FastDataListView spotListView;
        private BrightIdeasSoftware.FastDataListView futureUListView;
        private System.Windows.Forms.TabControl logTabControl;
        private System.Windows.Forms.TabPage logTabPage;
        private System.Windows.Forms.ListBox logListBox;
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
        private System.Windows.Forms.TabPage chartTabPageSec;
        private System.Windows.Forms.TabPage chartTabPageMin;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
    }
}

