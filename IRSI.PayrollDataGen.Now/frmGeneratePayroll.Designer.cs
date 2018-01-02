namespace IRSI.PayrollDataGen.Now
{
  partial class frmGeneratePayroll
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.cmdGenerate = new System.Windows.Forms.Button();
      this.cmdExit = new System.Windows.Forms.Button();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.lvDatedFolders = new System.Windows.Forms.ListView();
      this.tbLog = new System.Windows.Forms.TextBox();
      this.tableLayoutPanel1.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // cmdGenerate
      // 
      this.cmdGenerate.Enabled = false;
      this.cmdGenerate.Location = new System.Drawing.Point(482, 263);
      this.cmdGenerate.Name = "cmdGenerate";
      this.cmdGenerate.Size = new System.Drawing.Size(75, 23);
      this.cmdGenerate.TabIndex = 2;
      this.cmdGenerate.Text = "Generate";
      this.cmdGenerate.UseVisualStyleBackColor = true;
      this.cmdGenerate.Click += new System.EventHandler(this.cmdGenerate_Click);
      // 
      // cmdExit
      // 
      this.cmdExit.Location = new System.Drawing.Point(401, 263);
      this.cmdExit.Name = "cmdExit";
      this.cmdExit.Size = new System.Drawing.Size(75, 23);
      this.cmdExit.TabIndex = 3;
      this.cmdExit.Text = "Exit";
      this.cmdExit.UseVisualStyleBackColor = true;
      this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 3;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.cmdGenerate, 2, 1);
      this.tableLayoutPanel1.Controls.Add(this.cmdExit, 1, 1);
      this.tableLayoutPanel1.Controls.Add(this.tbLog, 0, 2);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 3;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 80F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(560, 354);
      this.tableLayoutPanel1.TabIndex = 4;
      // 
      // groupBox1
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 3);
      this.groupBox1.Controls.Add(this.lvDatedFolders);
      this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.groupBox1.Location = new System.Drawing.Point(3, 3);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Padding = new System.Windows.Forms.Padding(5);
      this.groupBox1.Size = new System.Drawing.Size(554, 254);
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Dated Folders";
      // 
      // lvDatedFolders
      // 
      this.lvDatedFolders.CheckBoxes = true;
      this.lvDatedFolders.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvDatedFolders.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.lvDatedFolders.Location = new System.Drawing.Point(5, 18);
      this.lvDatedFolders.Name = "lvDatedFolders";
      this.lvDatedFolders.Size = new System.Drawing.Size(544, 231);
      this.lvDatedFolders.TabIndex = 0;
      this.lvDatedFolders.UseCompatibleStateImageBehavior = false;
      this.lvDatedFolders.View = System.Windows.Forms.View.SmallIcon;
      this.lvDatedFolders.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvDatedFolders_ItemChecked);
      // 
      // tbLog
      // 
      this.tableLayoutPanel1.SetColumnSpan(this.tbLog, 3);
      this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tbLog.Location = new System.Drawing.Point(3, 292);
      this.tbLog.Multiline = true;
      this.tbLog.Name = "tbLog";
      this.tbLog.Size = new System.Drawing.Size(554, 59);
      this.tbLog.TabIndex = 6;
      // 
      // frmGeneratePayroll
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(560, 354);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Name = "frmGeneratePayroll";
      this.Text = "Generate Payroll";
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tableLayoutPanel1.PerformLayout();
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button cmdGenerate;
    private System.Windows.Forms.Button cmdExit;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox tbLog;
    private System.Windows.Forms.ListView lvDatedFolders;
  }
}

