
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
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.spotListView = new BrightIdeasSoftware.FastDataListView();
            this.futureListView = new BrightIdeasSoftware.FastDataListView();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.logListBox = new System.Windows.Forms.ListBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.realTimeTextBox = new System.Windows.Forms.TextBox();
            this.timeDiffTextBox = new System.Windows.Forms.TextBox();
            this.priceTextBox = new System.Windows.Forms.TextBox();
            this.amtTextBox = new System.Windows.Forms.TextBox();
            this.rcvTimeTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureListView)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(0, 0);
            this.chart1.Name = "chart1";
            this.chart1.Size = new System.Drawing.Size(1608, 813);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // spotListView
            // 
            this.spotListView.CellEditUseWholeCell = false;
            this.spotListView.DataSource = null;
            this.spotListView.HideSelection = false;
            this.spotListView.Location = new System.Drawing.Point(1420, 844);
            this.spotListView.Name = "spotListView";
            this.spotListView.ShowGroups = false;
            this.spotListView.Size = new System.Drawing.Size(189, 164);
            this.spotListView.TabIndex = 4;
            this.spotListView.UseCompatibleStateImageBehavior = false;
            this.spotListView.View = System.Windows.Forms.View.Details;
            this.spotListView.VirtualMode = true;
            // 
            // futureListView
            // 
            this.futureListView.CellEditUseWholeCell = false;
            this.futureListView.DataSource = null;
            this.futureListView.HideSelection = false;
            this.futureListView.Location = new System.Drawing.Point(1225, 844);
            this.futureListView.Name = "futureListView";
            this.futureListView.ShowGroups = false;
            this.futureListView.Size = new System.Drawing.Size(189, 164);
            this.futureListView.TabIndex = 4;
            this.futureListView.UseCompatibleStateImageBehavior = false;
            this.futureListView.View = System.Windows.Forms.View.Details;
            this.futureListView.VirtualMode = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(1, 820);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1218, 188);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.logListBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1210, 162);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Log";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // logListBox
            // 
            this.logListBox.FormattingEnabled = true;
            this.logListBox.ItemHeight = 12;
            this.logListBox.Location = new System.Drawing.Point(0, 0);
            this.logListBox.Name = "logListBox";
            this.logListBox.Size = new System.Drawing.Size(1210, 160);
            this.logListBox.TabIndex = 0;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(1305, 814);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(90, 21);
            this.nameTextBox.TabIndex = 6;
            this.nameTextBox.Text = "ABCDEF";
            // 
            // realTimeTextBox
            // 
            this.realTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.realTimeTextBox.Location = new System.Drawing.Point(1474, 821);
            this.realTimeTextBox.Name = "realTimeTextBox";
            this.realTimeTextBox.ReadOnly = true;
            this.realTimeTextBox.Size = new System.Drawing.Size(51, 14);
            this.realTimeTextBox.TabIndex = 6;
            this.realTimeTextBox.Text = "00:00:00";
            // 
            // timeDiffTextBox
            // 
            this.timeDiffTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeDiffTextBox.Location = new System.Drawing.Point(1531, 817);
            this.timeDiffTextBox.Name = "timeDiffTextBox";
            this.timeDiffTextBox.ReadOnly = true;
            this.timeDiffTextBox.Size = new System.Drawing.Size(38, 14);
            this.timeDiffTextBox.TabIndex = 6;
            this.timeDiffTextBox.Text = "-10.5";
            // 
            // priceTextBox
            // 
            this.priceTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.priceTextBox.Location = new System.Drawing.Point(1405, 808);
            this.priceTextBox.Name = "priceTextBox";
            this.priceTextBox.ReadOnly = true;
            this.priceTextBox.Size = new System.Drawing.Size(63, 14);
            this.priceTextBox.TabIndex = 6;
            this.priceTextBox.Text = "0.0000000";
            // 
            // amtTextBox
            // 
            this.amtTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.amtTextBox.Location = new System.Drawing.Point(1405, 824);
            this.amtTextBox.Name = "amtTextBox";
            this.amtTextBox.ReadOnly = true;
            this.amtTextBox.Size = new System.Drawing.Size(63, 14);
            this.amtTextBox.TabIndex = 6;
            this.amtTextBox.Text = "0.0000000";
            // 
            // rcvTimeTextBox
            // 
            this.rcvTimeTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rcvTimeTextBox.Location = new System.Drawing.Point(1474, 808);
            this.rcvTimeTextBox.Name = "rcvTimeTextBox";
            this.rcvTimeTextBox.ReadOnly = true;
            this.rcvTimeTextBox.Size = new System.Drawing.Size(51, 14);
            this.rcvTimeTextBox.TabIndex = 6;
            this.rcvTimeTextBox.Text = "00:00:00";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1011);
            this.Controls.Add(this.realTimeTextBox);
            this.Controls.Add(this.amtTextBox);
            this.Controls.Add(this.priceTextBox);
            this.Controls.Add(this.timeDiffTextBox);
            this.Controls.Add(this.rcvTimeTextBox);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.futureListView);
            this.Controls.Add(this.spotListView);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spotListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.futureListView)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private BrightIdeasSoftware.FastDataListView spotListView;
        private BrightIdeasSoftware.FastDataListView futureListView;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListBox logListBox;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.TextBox realTimeTextBox;
        private System.Windows.Forms.TextBox timeDiffTextBox;
        private System.Windows.Forms.TextBox priceTextBox;
        private System.Windows.Forms.TextBox amtTextBox;
        private System.Windows.Forms.TextBox rcvTimeTextBox;
    }
}

