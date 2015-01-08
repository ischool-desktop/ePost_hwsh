namespace SH_SemesterScoreReport_hwsh
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
            this.listViewEx1 = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.labelX3 = new DevComponents.DotNetBar.LabelX();
            this.cboRankRilter = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.listViewEx2 = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cboExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX4 = new DevComponents.DotNetBar.LabelX();
            this.cboTagRank2 = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX5 = new DevComponents.DotNetBar.LabelX();
            this.listViewEx3 = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.labelX7 = new DevComponents.DotNetBar.LabelX();
            this.cboRefExam = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX8 = new DevComponents.DotNetBar.LabelX();
            this.cboSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.cboSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cboConfigure = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.linkLabel5 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.labelX9 = new DevComponents.DotNetBar.LabelX();
            this.labelX11 = new DevComponents.DotNetBar.LabelX();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelX6 = new DevComponents.DotNetBar.LabelX();
            this.panel3 = new System.Windows.Forms.Panel();
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
            this.labelX2.Location = new System.Drawing.Point(13, 205);
            this.labelX2.Name = "labelX2";
            this.labelX2.Size = new System.Drawing.Size(74, 21);
            this.labelX2.TabIndex = 5;
            this.labelX2.Text = "列印科目：";
            // 
            // listViewEx1
            // 
            this.listViewEx1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.listViewEx1.Border.Class = "ListViewBorder";
            this.listViewEx1.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.listViewEx1.CheckBoxes = true;
            this.listViewEx1.Location = new System.Drawing.Point(13, 232);
            this.listViewEx1.Name = "listViewEx1";
            this.listViewEx1.Size = new System.Drawing.Size(366, 248);
            this.listViewEx1.TabIndex = 8;
            this.listViewEx1.UseCompatibleStateImageBehavior = false;
            this.listViewEx1.View = System.Windows.Forms.View.List;
            // 
            // labelX3
            // 
            this.labelX3.AutoSize = true;
            this.labelX3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX3.BackgroundStyle.Class = "";
            this.labelX3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX3.Location = new System.Drawing.Point(13, 174);
            this.labelX3.Name = "labelX3";
            this.labelX3.Size = new System.Drawing.Size(114, 21);
            this.labelX3.TabIndex = 5;
            this.labelX3.Text = "不排名學生類別：";
            // 
            // cboRankRilter
            // 
            this.cboRankRilter.DisplayMember = "Text";
            this.cboRankRilter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboRankRilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRankRilter.FormattingEnabled = true;
            this.cboRankRilter.ItemHeight = 19;
            this.cboRankRilter.Location = new System.Drawing.Point(134, 172);
            this.cboRankRilter.Name = "cboRankRilter";
            this.cboRankRilter.Size = new System.Drawing.Size(160, 25);
            this.cboRankRilter.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboRankRilter.TabIndex = 7;
            // 
            // listViewEx2
            // 
            this.listViewEx2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.listViewEx2.Border.Class = "ListViewBorder";
            this.listViewEx2.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.listViewEx2.CheckBoxes = true;
            this.listViewEx2.Location = new System.Drawing.Point(13, 79);
            this.listViewEx2.Name = "listViewEx2";
            this.listViewEx2.Size = new System.Drawing.Size(370, 147);
            this.listViewEx2.TabIndex = 1;
            this.listViewEx2.UseCompatibleStateImageBehavior = false;
            this.listViewEx2.View = System.Windows.Forms.View.List;
            // 
            // cboExam
            // 
            this.cboExam.DisplayMember = "Name";
            this.cboExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboExam.FormattingEnabled = true;
            this.cboExam.ItemHeight = 19;
            this.cboExam.Location = new System.Drawing.Point(120, 110);
            this.cboExam.Name = "cboExam";
            this.cboExam.Size = new System.Drawing.Size(160, 25);
            this.cboExam.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboExam.TabIndex = 5;
            this.cboExam.ValueMember = "ID";
            this.cboExam.SelectedIndexChanged += new System.EventHandler(this.ExamChanged);
            // 
            // labelX4
            // 
            this.labelX4.AutoSize = true;
            this.labelX4.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX4.BackgroundStyle.Class = "";
            this.labelX4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX4.Location = new System.Drawing.Point(13, 112);
            this.labelX4.Name = "labelX4";
            this.labelX4.Size = new System.Drawing.Size(101, 21);
            this.labelX4.TabIndex = 5;
            this.labelX4.Text = "列印成績試別：";
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
            // listViewEx3
            // 
            this.listViewEx3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.listViewEx3.Border.Class = "ListViewBorder";
            this.listViewEx3.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.listViewEx3.CheckBoxes = true;
            this.listViewEx3.Location = new System.Drawing.Point(13, 79);
            this.listViewEx3.Name = "listViewEx3";
            this.listViewEx3.Size = new System.Drawing.Size(370, 147);
            this.listViewEx3.TabIndex = 1;
            this.listViewEx3.UseCompatibleStateImageBehavior = false;
            this.listViewEx3.View = System.Windows.Forms.View.List;
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
            this.labelX7.Location = new System.Drawing.Point(13, 81);
            this.labelX7.Name = "labelX7";
            this.labelX7.Size = new System.Drawing.Size(60, 21);
            this.labelX7.TabIndex = 5;
            this.labelX7.Text = "學年度：";
            // 
            // cboRefExam
            // 
            this.cboRefExam.DisplayMember = "Name";
            this.cboRefExam.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboRefExam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboRefExam.FormattingEnabled = true;
            this.cboRefExam.ItemHeight = 19;
            this.cboRefExam.Location = new System.Drawing.Point(120, 141);
            this.cboRefExam.Name = "cboRefExam";
            this.cboRefExam.Size = new System.Drawing.Size(160, 25);
            this.cboRefExam.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboRefExam.TabIndex = 6;
            this.cboRefExam.ValueMember = "ID";
            // 
            // labelX8
            // 
            this.labelX8.AutoSize = true;
            this.labelX8.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX8.BackgroundStyle.Class = "";
            this.labelX8.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX8.Location = new System.Drawing.Point(13, 143);
            this.labelX8.Name = "labelX8";
            this.labelX8.Size = new System.Drawing.Size(101, 21);
            this.labelX8.TabIndex = 5;
            this.labelX8.Text = "參考成績試別：";
            // 
            // cboSchoolYear
            // 
            this.cboSchoolYear.DisplayMember = "Text";
            this.cboSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSchoolYear.FormattingEnabled = true;
            this.cboSchoolYear.ItemHeight = 19;
            this.cboSchoolYear.Location = new System.Drawing.Point(73, 79);
            this.cboSchoolYear.Name = "cboSchoolYear";
            this.cboSchoolYear.Size = new System.Drawing.Size(54, 25);
            this.cboSchoolYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSchoolYear.TabIndex = 3;
            this.cboSchoolYear.TextChanged += new System.EventHandler(this.ExamChanged);
            // 
            // cboSemester
            // 
            this.cboSemester.DisplayMember = "Text";
            this.cboSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cboSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboSemester.FormattingEnabled = true;
            this.cboSemester.ItemHeight = 19;
            this.cboSemester.Location = new System.Drawing.Point(174, 79);
            this.cboSemester.Name = "cboSemester";
            this.cboSemester.Size = new System.Drawing.Size(54, 25);
            this.cboSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cboSemester.TabIndex = 4;
            this.cboSemester.TextChanged += new System.EventHandler(this.ExamChanged);
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
            this.panel1.Controls.Add(this.cboConfigure);
            this.panel1.Controls.Add(this.linkLabel4);
            this.panel1.Controls.Add(this.linkLabel3);
            this.panel1.Controls.Add(this.linkLabel5);
            this.panel1.Controls.Add(this.linkLabel2);
            this.panel1.Controls.Add(this.linkLabel1);
            this.panel1.Controls.Add(this.listViewEx1);
            this.panel1.Controls.Add(this.labelX2);
            this.panel1.Controls.Add(this.cboRankRilter);
            this.panel1.Controls.Add(this.labelX3);
            this.panel1.Controls.Add(this.cboSemester);
            this.panel1.Controls.Add(this.labelX9);
            this.panel1.Controls.Add(this.labelX11);
            this.panel1.Controls.Add(this.labelX7);
            this.panel1.Controls.Add(this.cboSchoolYear);
            this.panel1.Controls.Add(this.labelX8);
            this.panel1.Controls.Add(this.cboRefExam);
            this.panel1.Controls.Add(this.labelX4);
            this.panel1.Controls.Add(this.cboExam);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(392, 509);
            this.panel1.TabIndex = 0;
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
            this.linkLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
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
            this.linkLabel5.Location = new System.Drawing.Point(267, 483);
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
            this.linkLabel2.Location = new System.Drawing.Point(102, 483);
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
            this.linkLabel1.Location = new System.Drawing.Point(10, 483);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(86, 17);
            this.linkLabel1.TabIndex = 9;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "檢視套印樣板";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // labelX9
            // 
            this.labelX9.AutoSize = true;
            this.labelX9.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX9.BackgroundStyle.Class = "";
            this.labelX9.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX9.Location = new System.Drawing.Point(127, 81);
            this.labelX9.Name = "labelX9";
            this.labelX9.Size = new System.Drawing.Size(47, 21);
            this.labelX9.TabIndex = 5;
            this.labelX9.Text = "學期：";
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
            this.panel2.Controls.Add(this.labelX6);
            this.panel2.Controls.Add(this.labelX1);
            this.panel2.Controls.Add(this.cboTagRank1);
            this.panel2.Controls.Add(this.listViewEx2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(392, 234);
            this.panel2.TabIndex = 0;
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
            this.panel3.Controls.Add(this.listViewEx3);
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
            this.Text = "期末成績單";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private DevComponents.DotNetBar.Controls.ListViewEx listViewEx1;
        private DevComponents.DotNetBar.LabelX labelX3;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboRankRilter;
        private DevComponents.DotNetBar.Controls.ListViewEx listViewEx2;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboExam;
        private DevComponents.DotNetBar.LabelX labelX4;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboTagRank2;
        private DevComponents.DotNetBar.LabelX labelX5;
        private DevComponents.DotNetBar.Controls.ListViewEx listViewEx3;
        private DevComponents.DotNetBar.LabelX labelX7;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboRefExam;
        private DevComponents.DotNetBar.LabelX labelX8;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboSemester;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cboConfigure;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.LinkLabel linkLabel3;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private DevComponents.DotNetBar.LabelX labelX9;
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
    }
}

