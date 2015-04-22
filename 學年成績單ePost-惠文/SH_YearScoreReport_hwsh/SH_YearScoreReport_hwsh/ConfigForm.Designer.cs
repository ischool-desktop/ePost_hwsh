namespace SH_YearScoreReport_hwsh
{
    partial class ConfigForm
    {

        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.cboTagRank1 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.labelX2 = new DevComponents.DotNetBar.LabelX();
            this.lvSubject = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.lvSubjTag1 = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cboTagRank2 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.lvSubjTag2 = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.iptSchoolYear = new DevComponents.Editors.IntegerInput();
            this.cblvSubject = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cboConfigure = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.labelX11 = new DevComponents.DotNetBar.LabelX();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cblvSubjTag1 = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cblvSubjTag2 = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.labelX10 = new DevComponents.DotNetBar.LabelX();
            this.panel5 = new System.Windows.Forms.Panel();
            this.chkExportEPOST = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.btnSaveConfig = new DevComponents.DotNetBar.ButtonX();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.btnCancel = new DevComponents.DotNetBar.ButtonX();
            this.panel4 = new System.Windows.Forms.Panel();
            this.circularProgress1 = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iptSchoolYear)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // cboTagRank1
            // 
            this.cboTagRank1.DisplayMember = "Text";
            this.cboTagRank1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTagRank1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTagRank1.FormattingEnabled = true;
            this.cboTagRank1.ItemHeight = 19;
            this.cboTagRank1.Location = new System.Drawing.Point(95, 14);
            this.cboTagRank1.Name = "cboTagRank1";
            this.cboTagRank1.Size = new System.Drawing.Size(160, 25);
            this.cboTagRank1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboTagRank1.TabIndex = 0;
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(13, 16);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(82, 21);
            this.labelX1.TabIndex = 5;
            this.labelX1.Text = "類別排名1：";
            // 
            // labelX2
            // 
            this.labelX2.AutoSize = true;
            this.labelX2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX2.BackgroundStyle.Class = "";
            this.labelX2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX2.Location = new System.Drawing.Point(12, 114);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(74, 21);
            this.labelX2.TabIndex = 5;
            this.labelX2.Text = "列印科目：";
            // 
            // lvSubject
            // 
            this.lvSubject.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.lvSubject.Border.Class = "ListViewBorder";
            this.lvSubject.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvSubject.CheckBoxes = true;
            this.lvSubject.Location = new System.Drawing.Point(12, 141);
            this.lvSubject.Name = "lvSubject";
            this.lvSubject.Size = new System.Drawing.Size(366, 319);
            this.lvSubject.TabIndex = 8;
            this.lvSubject.UseCompatibleStateImageBehavior = false;
            this.lvSubject.View = System.Windows.Forms.View.List;
            // 
            // lvSubjTag1
            // 
            this.lvSubjTag1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.lvSubjTag1.Border.Class = "ListViewBorder";
            this.lvSubjTag1.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvSubjTag1.CheckBoxes = true;
            this.lvSubjTag1.Location = new System.Drawing.Point(13, 79);
            this.lvSubjTag1.Name = "lvSubjTag1";
            this.lvSubjTag1.Size = new System.Drawing.Size(370, 147);
            this.lvSubjTag1.TabIndex = 1;
            this.lvSubjTag1.UseCompatibleStateImageBehavior = false;
            this.lvSubjTag1.View = System.Windows.Forms.View.List;
            // 
            // cboTagRank2
            // 
            this.cboTagRank2.DisplayMember = "Text";
            this.cboTagRank2.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboTagRank2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTagRank2.FormattingEnabled = true;
            this.cboTagRank2.ItemHeight = 19;
            this.cboTagRank2.Location = new System.Drawing.Point(95, 14);
            this.cboTagRank2.Name = "cboTagRank2";
            this.cboTagRank2.Size = new System.Drawing.Size(160, 25);
            this.cboTagRank2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboTagRank2.TabIndex = 0;
            // 
            // labelX5
            // 
            this.labelX5.AutoSize = true;
            this.labelX5.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX5.BackgroundStyle.Class = "";
            this.labelX5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX5.Location = new System.Drawing.Point(13, 16);
            this.labelX5.Name = "labelX5";
            this.labelX5.Size = new System.Drawing.Size(82, 21);
            this.labelX5.TabIndex = 5;
            this.labelX5.Text = "類別排名2：";
            // 
            // lvSubjTag2
            // 
            this.lvSubjTag2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.lvSubjTag2.Border.Class = "ListViewBorder";
            this.lvSubjTag2.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvSubjTag2.CheckBoxes = true;
            this.lvSubjTag2.Location = new System.Drawing.Point(13, 79);
            this.lvSubjTag2.Name = "lvSubjTag2";
            this.lvSubjTag2.Size = new System.Drawing.Size(370, 147);
            this.lvSubjTag2.TabIndex = 1;
            this.lvSubjTag2.UseCompatibleStateImageBehavior = false;
            this.lvSubjTag2.View = System.Windows.Forms.View.List;
            // 
            // labelX7
            // 
            this.labelX7.AutoSize = true;
            this.labelX7.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX7.BackgroundStyle.Class = "";
            this.labelX7.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX7.Location = new System.Drawing.Point(26, 77);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(60, 21);
            this.labelX7.TabIndex = 5;
            this.labelX7.Text = "學年度：";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 509F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(784, 509);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.iptSchoolYear);
            this.panel1.Controls.Add(this.cblvSubject);
            this.panel1.Controls.Add(this.cboConfigure);
            this.panel1.Controls.Add(this.linkLabel4);
            this.panel1.Controls.Add(this.linkLabel3);
            this.panel1.Controls.Add(this.linkLabel5);
            this.panel1.Controls.Add(this.linkLabel2);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.lvSubject);
            this.panel1.Controls.Add(this.labelX2);
            this.panel1.Controls.Add(this.labelX11);
            this.panel1.Controls.Add(this.labelX7);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 509);
            this.panel1.TabIndex = 0;
            // 
            // iptSchoolYear
            // 
            this.iptSchoolYear.AllowEmptyState = false;
            // 
            // 
            // 
            this.iptSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.iptSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.iptSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.iptSchoolYear.Location = new System.Drawing.Point(93, 75);
            this.iptSchoolYear.MaxValue = 1000;
            this.iptSchoolYear.MinValue = 90;
            this.iptSchoolYear.Name = "iptSchoolYear";
            this.iptSchoolYear.ShowUpDown = true;
            this.iptSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.iptSchoolYear.TabIndex = 14;
            this.iptSchoolYear.Value = 90;
            this.iptSchoolYear.ValueChanged += new System.EventHandler(this.iptSchoolYear_ValueChanged);
            // 
            // cblvSubject
            // 
            this.cblvSubject.AutoSize = true;
            // 
            // 
            // 
            this.cblvSubject.BackgroundStyle.Class = "";
            this.cblvSubject.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cblvSubject.Location = new System.Drawing.Point(88, 114);
            this.cblvSubject.Name = "cblvSubject";
            this.cblvSubject.Size = new System.Drawing.Size(54, 21);
            this.cblvSubject.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cblvSubject.TabIndex = 13;
            this.cblvSubject.Text = "全選";
            this.cblvSubject.CheckedChanged += new System.EventHandler(this.cblvSubject_CheckedChanged);
            // 
            // cboConfigure
            // 
            this.cboConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboConfigure.DisplayMember = "Name";
            this.cboConfigure.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboConfigure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConfigure.FormattingEnabled = true;
            this.cboConfigure.ItemHeight = 19;
            this.cboConfigure.Location = new System.Drawing.Point(106, 14);
            this.cboConfigure.Name = "cboConfigure";
            this.cboConfigure.Size = new System.Drawing.Size(273, 25);
            this.cboConfigure.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboConfigure.TabIndex = 0;
            this.cboConfigure.SelectedIndexChanged += new System.EventHandler(this.cboConfigure_SelectedIndexChanged);
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.Location = new System.Drawing.Point(306, 50);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(73, 17);
            this.linkLabel4.TabIndex = 2;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "刪除設定檔";
            this.linkLabel4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // linkLabel3
            // 
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Location = new System.Drawing.Point(227, 50);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(73, 17);
            this.linkLabel3.TabIndex = 1;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "複製設定檔";
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabel5
            // 
            this.linkLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel5.AutoSize = true;
            this.linkLabel5.Location = new System.Drawing.Point(267, 480);
            this.linkLabel5.Name = "linkLabel5";
            this.linkLabel5.Size = new System.Drawing.Size(112, 17);
            this.linkLabel5.TabIndex = 11;
            this.linkLabel5.TabStop = true;
            this.linkLabel5.Text = "下載合併欄位總表";
            this.linkLabel5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(102, 480);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(86, 17);
            this.linkLabel2.TabIndex = 10;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "變更套印樣板";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(10, 480);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(86, 17);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "檢視套印樣板";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelX11
            // 
            this.labelX11.AutoSize = true;
            this.labelX11.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX11.BackgroundStyle.Class = "";
            this.labelX11.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX11.Location = new System.Drawing.Point(13, 16);
            this.labelX11.Name = "labelX11";
            this.labelX11.Size = new System.Drawing.Size(87, 21);
            this.labelX11.TabIndex = 5;
            this.labelX11.Text = "樣板設定檔：";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.panel5, 0, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(392, 0);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(392, 509);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cblvSubjTag1);
            this.panel2.Controls.Add(this.labelX6);
            this.panel2.Controls.Add(this.labelX1);
            this.panel2.Controls.Add(this.cboTagRank1);
            this.panel2.Controls.Add(this.lvSubjTag1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(392, 234);
            this.panel2.TabIndex = 0;
            // 
            // cblvSubjTag1
            // 
            this.cblvSubjTag1.AutoSize = true;
            // 
            // 
            // 
            this.cblvSubjTag1.BackgroundStyle.Class = "";
            this.cblvSubjTag1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cblvSubjTag1.Location = new System.Drawing.Point(94, 50);
            this.cblvSubjTag1.Name = "cblvSubjTag1";
            this.cblvSubjTag1.Size = new System.Drawing.Size(54, 21);
            this.cblvSubjTag1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cblvSubjTag1.TabIndex = 14;
            this.cblvSubjTag1.Text = "全選";
            this.cblvSubjTag1.CheckedChanged += new System.EventHandler(this.cblvSubjTag1_CheckedChanged);
            // 
            // labelX6
            // 
            this.labelX6.AutoSize = true;
            this.labelX6.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX6.BackgroundStyle.Class = "";
            this.labelX6.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX6.Location = new System.Drawing.Point(13, 50);
            this.labelX6.Name = "labelX6";
            this.labelX6.Size = new System.Drawing.Size(74, 21);
            this.labelX6.TabIndex = 5;
            this.labelX6.Text = "排名科目：";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.cblvSubjTag2);
            this.panel3.Controls.Add(this.lvSubjTag2);
            this.panel3.Controls.Add(this.labelX10);
            this.panel3.Controls.Add(this.labelX5);
            this.panel3.Controls.Add(this.cboTagRank2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 234);
            this.panel3.Margin = new System.Windows.Forms.Padding(0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(392, 234);
            this.panel3.TabIndex = 1;
            // 
            // cblvSubjTag2
            // 
            this.cblvSubjTag2.AutoSize = true;
            // 
            // 
            // 
            this.cblvSubjTag2.BackgroundStyle.Class = "";
            this.cblvSubjTag2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cblvSubjTag2.Location = new System.Drawing.Point(93, 50);
            this.cblvSubjTag2.Name = "cblvSubjTag2";
            this.cblvSubjTag2.Size = new System.Drawing.Size(54, 21);
            this.cblvSubjTag2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cblvSubjTag2.TabIndex = 14;
            this.cblvSubjTag2.Text = "全選";
            this.cblvSubjTag2.CheckedChanged += new System.EventHandler(this.cblvSubjTag2_CheckedChanged);
            // 
            // labelX10
            // 
            this.labelX10.AutoSize = true;
            this.labelX10.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX10.BackgroundStyle.Class = "";
            this.labelX10.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX10.Location = new System.Drawing.Point(13, 50);
            this.labelX10.Name = "labelX10";
            this.labelX10.Size = new System.Drawing.Size(74, 21);
            this.labelX10.TabIndex = 7;
            this.labelX10.Text = "排名科目：";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.chkExportEPOST);
            this.panel5.Controls.Add(this.btnSaveConfig);
            this.panel5.Controls.Add(this.btnPrint);
            this.panel5.Controls.Add(this.btnCancel);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 468);
            this.panel5.Margin = new System.Windows.Forms.Padding(0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(392, 41);
            this.panel5.TabIndex = 1;
            // 
            // chkExportEPOST
            // 
            // 
            // 
            // 
            this.chkExportEPOST.BackgroundStyle.Class = "";
            this.chkExportEPOST.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkExportEPOST.Location = new System.Drawing.Point(13, 9);
            this.chkExportEPOST.Name = "chkExportEPOST";
            this.chkExportEPOST.Size = new System.Drawing.Size(100, 23);
            this.chkExportEPOST.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.chkExportEPOST.TabIndex = 3;
            this.chkExportEPOST.Text = "產生epost檔";
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSaveConfig.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSaveConfig.Enabled = false;
            this.btnSaveConfig.Location = new System.Drawing.Point(146, 9);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(75, 23);
            this.btnSaveConfig.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSaveConfig.TabIndex = 0;
            this.btnSaveConfig.Text = "儲存設定";
            this.btnSaveConfig.Tooltip = "儲存當前的樣板設定。";
            this.btnSaveConfig.Click += new System.EventHandler(this.SaveTemplate);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnPrint.Enabled = false;
            this.btnPrint.Location = new System.Drawing.Point(227, 9);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnPrint.TabIndex = 1;
            this.btnPrint.Text = "確定";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.Transparent;
            this.btnCancel.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(308, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Location = new System.Drawing.Point(917, 659);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 100);
            this.panel4.TabIndex = 12;
            // 
            // circularProgress1
            // 
            this.circularProgress1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.circularProgress1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.circularProgress1.BackgroundStyle.Class = "";
            this.circularProgress1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.circularProgress1.FocusCuesEnabled = false;
            this.circularProgress1.Location = new System.Drawing.Point(355, 217);
            this.circularProgress1.Name = "circularProgress1";
            this.circularProgress1.ProgressBarType = DevComponents.DotNetBar.eCircularProgressType.Dot;
            this.circularProgress1.ProgressColor = System.Drawing.Color.LimeGreen;
            this.circularProgress1.ProgressTextVisible = true;
            this.circularProgress1.Size = new System.Drawing.Size(75, 75);
            this.circularProgress1.Style = DevComponents.DotNetBar.eDotNetBarStyle.Windows7;
            this.circularProgress1.TabIndex = 13;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(784, 509);
            this.Controls.Add(this.circularProgress1);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.tableLayoutPanel1);
            this.DoubleBuffered = true;
            this.MaximizeBox = true;
            this.Name = "ConfigForm";
            this.Text = "學年成績單(固定排名)";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.iptSchoolYear)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.ComboBoxEx cboTagRank1;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX labelX2;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSubject;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSubjTag1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboTagRank2;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ListViewEx lvSubjTag2;
        private DevComponents.DotNetBar.LabelX labelX7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboConfigure;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private DevComponents.DotNetBar.LabelX labelX11;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel panel2;
        private DevComponents.DotNetBar.LabelX labelX6;
        private System.Windows.Forms.Panel panel3;
        private DevComponents.DotNetBar.LabelX labelX10;
        private System.Windows.Forms.Panel panel5;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private DevComponents.DotNetBar.ButtonX btnCancel;
        private System.Windows.Forms.Panel panel4;
        private DevComponents.DotNetBar.Controls.CircularProgress circularProgress1;
        private DevComponents.DotNetBar.ButtonX btnSaveConfig;
        private System.Windows.Forms.LinkLabel linkLabel5;
        private DevComponents.DotNetBar.Controls.CheckBoxX chkExportEPOST;
        private DevComponents.DotNetBar.Controls.CheckBoxX cblvSubject;
        private DevComponents.DotNetBar.Controls.CheckBoxX cblvSubjTag1;
        private DevComponents.DotNetBar.Controls.CheckBoxX cblvSubjTag2;
        private DevComponents.Editors.IntegerInput iptSchoolYear;
    }
}