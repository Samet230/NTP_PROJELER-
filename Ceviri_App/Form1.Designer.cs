namespace Ceviri_App;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // UI Kontrolleri
    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Panel pnlControls;
    private System.Windows.Forms.Label lblFrom;
    private System.Windows.Forms.ComboBox cmbFromLang;
    private System.Windows.Forms.Label lblTo;
    private System.Windows.Forms.ComboBox cmbToLang;
    private System.Windows.Forms.Button btnSwitch; // Dilleri değiştir butonu (Opsiyonel, şimdilik yer tutucu veya sadece görsel)
    private System.Windows.Forms.TextBox txtInput;
    private System.Windows.Forms.TextBox txtOutput;
    private System.Windows.Forms.Button btnTranslate;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Panel pnlInput;
    private System.Windows.Forms.Panel pnlOutput;
    private System.Windows.Forms.Label lblInputHeader;
    private System.Windows.Forms.Label lblOutputHeader;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.pnlHeader = new System.Windows.Forms.Panel();
        this.lblTitle = new System.Windows.Forms.Label();
        this.pnlControls = new System.Windows.Forms.Panel();
        this.cmbToLang = new System.Windows.Forms.ComboBox();
        this.lblTo = new System.Windows.Forms.Label();
        this.cmbFromLang = new System.Windows.Forms.ComboBox();
        this.lblFrom = new System.Windows.Forms.Label();
        this.txtInput = new System.Windows.Forms.TextBox();
        this.txtOutput = new System.Windows.Forms.TextBox();
        this.btnTranslate = new System.Windows.Forms.Button();
        this.btnClear = new System.Windows.Forms.Button();
        this.pnlInput = new System.Windows.Forms.Panel();
        this.lblInputHeader = new System.Windows.Forms.Label();
        this.pnlOutput = new System.Windows.Forms.Panel();
        this.lblOutputHeader = new System.Windows.Forms.Label();
        this.pnlHeader.SuspendLayout();
        this.pnlControls.SuspendLayout();
        this.pnlInput.SuspendLayout();
        this.pnlOutput.SuspendLayout();
        this.SuspendLayout();

        // 
        // pnlHeader
        // 
        this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(226))))); // Modern Blue
        this.pnlHeader.Controls.Add(this.lblTitle);
        this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlHeader.Location = new System.Drawing.Point(0, 0);
        this.pnlHeader.Name = "pnlHeader";
        this.pnlHeader.Size = new System.Drawing.Size(900, 60);
        this.pnlHeader.TabIndex = 0;
        // 
        // lblTitle
        // 
        this.lblTitle.AutoSize = true;
        this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblTitle.ForeColor = System.Drawing.Color.White;
        this.lblTitle.Location = new System.Drawing.Point(20, 13);
        this.lblTitle.Name = "lblTitle";
        this.lblTitle.Size = new System.Drawing.Size(250, 32);
        this.lblTitle.TabIndex = 0;
        this.lblTitle.Text = "Çeviri Uygulaması";
        // 
        // pnlControls
        // 
        this.pnlControls.BackColor = System.Drawing.Color.White;
        this.pnlControls.Controls.Add(this.btnClear);
        this.pnlControls.Controls.Add(this.btnTranslate);
        this.pnlControls.Controls.Add(this.cmbToLang);
        this.pnlControls.Controls.Add(this.lblTo);
        this.pnlControls.Controls.Add(this.cmbFromLang);
        this.pnlControls.Controls.Add(this.lblFrom);
        this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
        this.pnlControls.Location = new System.Drawing.Point(0, 60);
        this.pnlControls.Name = "pnlControls";
        this.pnlControls.Size = new System.Drawing.Size(900, 80);
        this.pnlControls.TabIndex = 1;
        // 
        // lblFrom
        // 
        this.lblFrom.AutoSize = true;
        this.lblFrom.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblFrom.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        this.lblFrom.Location = new System.Drawing.Point(30, 15);
        this.lblFrom.Name = "lblFrom";
        this.lblFrom.Size = new System.Drawing.Size(76, 19);
        this.lblFrom.TabIndex = 0;
        this.lblFrom.Text = "Kaynak Dil";
        // 
        // cmbFromLang
        // 
        this.cmbFromLang.BackColor = System.Drawing.Color.WhiteSmoke;
        this.cmbFromLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbFromLang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.cmbFromLang.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.cmbFromLang.FormattingEnabled = true;
        this.cmbFromLang.Location = new System.Drawing.Point(30, 37);
        this.cmbFromLang.Name = "cmbFromLang";
        this.cmbFromLang.Size = new System.Drawing.Size(200, 28);
        this.cmbFromLang.TabIndex = 1;
        // 
        // lblTo
        // 
        this.lblTo.AutoSize = true;
        this.lblTo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblTo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        this.lblTo.Location = new System.Drawing.Point(260, 15);
        this.lblTo.Name = "lblTo";
        this.lblTo.Size = new System.Drawing.Size(69, 19);
        this.lblTo.TabIndex = 2;
        this.lblTo.Text = "Hedef Dil";
        // 
        // cmbToLang
        // 
        this.cmbToLang.BackColor = System.Drawing.Color.WhiteSmoke;
        this.cmbToLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbToLang.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.cmbToLang.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.cmbToLang.FormattingEnabled = true;
        this.cmbToLang.Location = new System.Drawing.Point(260, 37);
        this.cmbToLang.Name = "cmbToLang";
        this.cmbToLang.Size = new System.Drawing.Size(200, 28);
        this.cmbToLang.TabIndex = 3;
        // 
        // btnTranslate
        // 
        this.btnTranslate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(144)))), ((int)(((byte)(226)))));
        this.btnTranslate.FlatAppearance.BorderSize = 0;
        this.btnTranslate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnTranslate.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnTranslate.ForeColor = System.Drawing.Color.White;
        this.btnTranslate.Location = new System.Drawing.Point(500, 25);
        this.btnTranslate.Name = "btnTranslate";
        this.btnTranslate.Size = new System.Drawing.Size(140, 40);
        this.btnTranslate.TabIndex = 4;
        this.btnTranslate.Text = "Çevir";
        this.btnTranslate.UseVisualStyleBackColor = false;
        this.btnTranslate.Click += new System.EventHandler(this.btnTranslate_Click);
        this.btnTranslate.Cursor = System.Windows.Forms.Cursors.Hand;
        // 
        // btnClear
        // 
        this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
        this.btnClear.FlatAppearance.BorderSize = 0;
        this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        this.btnClear.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        this.btnClear.Location = new System.Drawing.Point(660, 25);
        this.btnClear.Name = "btnClear";
        this.btnClear.Size = new System.Drawing.Size(120, 40);
        this.btnClear.TabIndex = 5;
        this.btnClear.Text = "Temizle";
        this.btnClear.UseVisualStyleBackColor = false;
        this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
        this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
        // 
        // pnlInput
        // 
        this.pnlInput.Controls.Add(this.txtInput);
        this.pnlInput.Controls.Add(this.lblInputHeader);
        this.pnlInput.Dock = System.Windows.Forms.DockStyle.Left;
        this.pnlInput.Padding = new System.Windows.Forms.Padding(30, 20, 15, 30);
        this.pnlInput.Size = new System.Drawing.Size(450, 460);
        this.pnlInput.TabIndex = 2;
        // 
        // lblInputHeader
        // 
        this.lblInputHeader.AutoSize = true;
        this.lblInputHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblInputHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblInputHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        this.lblInputHeader.Location = new System.Drawing.Point(30, 20);
        this.lblInputHeader.Name = "lblInputHeader";
        this.lblInputHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
        this.lblInputHeader.Size = new System.Drawing.Size(95, 31);
        this.lblInputHeader.TabIndex = 0;
        this.lblInputHeader.Text = "Giriş Metni";
        // 
        // txtInput
        // 
        this.txtInput.BackColor = System.Drawing.Color.White;
        this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtInput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtInput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtInput.Location = new System.Drawing.Point(30, 51);
        this.txtInput.Multiline = true;
        this.txtInput.Name = "txtInput";
        this.txtInput.PlaceholderText = "Çevrilecek metni buraya yazın...";
        this.txtInput.Size = new System.Drawing.Size(405, 379);
        this.txtInput.TabIndex = 1;
        // 
        // pnlOutput
        // 
        this.pnlOutput.Controls.Add(this.txtOutput);
        this.pnlOutput.Controls.Add(this.lblOutputHeader);
        this.pnlOutput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pnlOutput.Padding = new System.Windows.Forms.Padding(15, 20, 30, 30);
        this.pnlOutput.Size = new System.Drawing.Size(450, 460);
        this.pnlOutput.TabIndex = 3;
        // 
        // lblOutputHeader
        // 
        this.lblOutputHeader.AutoSize = true;
        this.lblOutputHeader.Dock = System.Windows.Forms.DockStyle.Top;
        this.lblOutputHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblOutputHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        this.lblOutputHeader.Location = new System.Drawing.Point(15, 20);
        this.lblOutputHeader.Name = "lblOutputHeader";
        this.lblOutputHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
        this.lblOutputHeader.Size = new System.Drawing.Size(107, 31);
        this.lblOutputHeader.TabIndex = 0;
        this.lblOutputHeader.Text = "Çeviri Sonucu";
        // 
        // txtOutput
        // 
        this.txtOutput.BackColor = System.Drawing.Color.WhiteSmoke;
        this.txtOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtOutput.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.txtOutput.Location = new System.Drawing.Point(15, 51);
        this.txtOutput.Multiline = true;
        this.txtOutput.Name = "txtOutput";
        this.txtOutput.ReadOnly = true;
        this.txtOutput.PlaceholderText = "Çeviri burada görünecek...";
        this.txtOutput.Size = new System.Drawing.Size(405, 379);
        this.txtOutput.TabIndex = 1;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
        this.ClientSize = new System.Drawing.Size(900, 600);
        this.Controls.Add(this.pnlOutput);
        this.Controls.Add(this.pnlInput);
        this.Controls.Add(this.pnlControls);
        this.Controls.Add(this.pnlHeader);
        this.Name = "Form1";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Text = "Çeviri Uygulaması";
        this.pnlHeader.ResumeLayout(false);
        this.pnlHeader.PerformLayout();
        this.pnlControls.ResumeLayout(false);
        this.pnlControls.PerformLayout();
        this.pnlInput.ResumeLayout(false);
        this.pnlInput.PerformLayout();
        this.pnlOutput.ResumeLayout(false);
        this.pnlOutput.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion
}
