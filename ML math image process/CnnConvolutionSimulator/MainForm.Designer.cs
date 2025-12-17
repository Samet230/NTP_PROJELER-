namespace CnnConvolutionSimulator
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelControls = new System.Windows.Forms.Panel();
            this.lblFilter = new System.Windows.Forms.Label();
            this.cmbFilters = new System.Windows.Forms.ComboBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpCustom = new System.Windows.Forms.GroupBox();
            this.btnApplyCustom = new System.Windows.Forms.Button();
            this.txt00 = new System.Windows.Forms.TextBox();
            this.txt01 = new System.Windows.Forms.TextBox();
            this.txt02 = new System.Windows.Forms.TextBox();
            this.txt10 = new System.Windows.Forms.TextBox();
            this.txt11 = new System.Windows.Forms.TextBox();
            this.txt12 = new System.Windows.Forms.TextBox();
            this.txt20 = new System.Windows.Forms.TextBox();
            this.txt21 = new System.Windows.Forms.TextBox();
            this.txt22 = new System.Windows.Forms.TextBox();
            this.panelImages = new System.Windows.Forms.TableLayoutPanel();
            this.panelSource = new System.Windows.Forms.Panel();
            this.pbSource = new System.Windows.Forms.PictureBox();
            this.lblSource = new System.Windows.Forms.Label();
            this.panelResult = new System.Windows.Forms.Panel();
            this.pbResult = new System.Windows.Forms.PictureBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelImages.SuspendLayout();
            this.panelSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSource)).BeginInit();
            this.panelResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panelControls, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panelImages, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 75F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1000, 600);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelControls.Controls.Add(this.grpCustom);
            this.panelControls.Controls.Add(this.lblFilter);
            this.panelControls.Controls.Add(this.cmbFilters);
            this.panelControls.Controls.Add(this.btnProcess);
            this.panelControls.Controls.Add(this.btnLoad);
            this.panelControls.Controls.Add(this.btnSave);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControls.Location = new System.Drawing.Point(3, 453);
            this.panelControls.Name = "panelControls";
            this.panelControls.Size = new System.Drawing.Size(994, 144);
            this.panelControls.TabIndex = 0;
            // 
            // lblFilter
            // 
            this.lblFilter.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblFilter.AutoSize = true;
            this.lblFilter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFilter.ForeColor = System.Drawing.Color.White;
            this.lblFilter.Location = new System.Drawing.Point(340, 32);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(69, 19);
            this.lblFilter.TabIndex = 7;
            this.lblFilter.Text = "Filtre Seç:";
            // 
            // cmbFilters
            // 
            this.cmbFilters.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbFilters.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.cmbFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilters.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbFilters.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cmbFilters.ForeColor = System.Drawing.Color.White;
            this.cmbFilters.FormattingEnabled = true;
            this.cmbFilters.Items.AddRange(new object[] {
            "Kenar Algılama (Edge Detection)",
            "Keskinleştirme (Sharpen)",
            "Gaussian Bulanıklaştırma (Blur)",
            "Kutu Bulanıklaştırma (Box Blur)",
            "Kabartma (Emboss)",
            "Sobel Yatay (Horizontal)",
            "Sobel Dikey (Vertical)"});
            this.cmbFilters.Location = new System.Drawing.Point(415, 29);
            this.cmbFilters.Name = "cmbFilters";
            this.cmbFilters.Size = new System.Drawing.Size(200, 25);
            this.cmbFilters.TabIndex = 4;
            // 
            // btnProcess
            // 
            this.btnProcess.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnProcess.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnProcess.FlatAppearance.BorderSize = 0;
            this.btnProcess.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcess.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnProcess.ForeColor = System.Drawing.Color.White;
            this.btnProcess.Location = new System.Drawing.Point(630, 22);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(90, 40);
            this.btnProcess.TabIndex = 3;
            this.btnProcess.Text = "İŞLE";
            this.btnProcess.UseVisualStyleBackColor = false;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnLoad.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.btnLoad.FlatAppearance.BorderSize = 0;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.Location = new System.Drawing.Point(20, 22);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(130, 40);
            this.btnLoad.TabIndex = 2;
            this.btnLoad.Text = "RESİM YÜKLE";
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(160, 22);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(130, 40);
            this.btnSave.TabIndex = 8;
            this.btnSave.Text = "KAYDET";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpCustom
            // 
            this.grpCustom.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.grpCustom.Controls.Add(this.btnApplyCustom);
            this.grpCustom.Controls.Add(this.txt00);
            this.grpCustom.Controls.Add(this.txt01);
            this.grpCustom.Controls.Add(this.txt02);
            this.grpCustom.Controls.Add(this.txt10);
            this.grpCustom.Controls.Add(this.txt11);
            this.grpCustom.Controls.Add(this.txt12);
            this.grpCustom.Controls.Add(this.txt20);
            this.grpCustom.Controls.Add(this.txt21);
            this.grpCustom.Controls.Add(this.txt22);
            this.grpCustom.ForeColor = System.Drawing.Color.White;
            this.grpCustom.Location = new System.Drawing.Point(730, 10);
            this.grpCustom.Name = "grpCustom";
            this.grpCustom.Size = new System.Drawing.Size(250, 125);
            this.grpCustom.TabIndex = 9;
            this.grpCustom.TabStop = false;
            this.grpCustom.Text = "Özel Filtre (3x3)";
            // 
            // btnApplyCustom
            // 
            this.btnApplyCustom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnApplyCustom.FlatAppearance.BorderSize = 0;
            this.btnApplyCustom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyCustom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnApplyCustom.ForeColor = System.Drawing.Color.White;
            this.btnApplyCustom.Location = new System.Drawing.Point(160, 45);
            this.btnApplyCustom.Name = "btnApplyCustom";
            this.btnApplyCustom.Size = new System.Drawing.Size(90, 40);
            this.btnApplyCustom.TabIndex = 10;
            this.btnApplyCustom.Text = "UYGULA";
            this.btnApplyCustom.UseVisualStyleBackColor = false;
            this.btnApplyCustom.Click += new System.EventHandler(this.btnApplyCustom_Click);
            // 
            // txt00
            // 
            this.txt00.Location = new System.Drawing.Point(15, 25);
            this.txt00.Name = "txt00";
            this.txt00.Size = new System.Drawing.Size(40, 23);
            this.txt00.TabIndex = 0;
            this.txt00.Text = "0";
            this.txt00.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt01
            // 
            this.txt01.Location = new System.Drawing.Point(60, 25);
            this.txt01.Name = "txt01";
            this.txt01.Size = new System.Drawing.Size(40, 23);
            this.txt01.TabIndex = 1;
            this.txt01.Text = "0";
            this.txt01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt02
            // 
            this.txt02.Location = new System.Drawing.Point(105, 25);
            this.txt02.Name = "txt02";
            this.txt02.Size = new System.Drawing.Size(40, 23);
            this.txt02.TabIndex = 2;
            this.txt02.Text = "0";
            this.txt02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt10
            // 
            this.txt10.Location = new System.Drawing.Point(15, 55);
            this.txt10.Name = "txt10";
            this.txt10.Size = new System.Drawing.Size(40, 23);
            this.txt10.TabIndex = 3;
            this.txt10.Text = "0";
            this.txt10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt11
            // 
            this.txt11.Location = new System.Drawing.Point(60, 55);
            this.txt11.Name = "txt11";
            this.txt11.Size = new System.Drawing.Size(40, 23);
            this.txt11.TabIndex = 4;
            this.txt11.Text = "1";
            this.txt11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt12
            // 
            this.txt12.Location = new System.Drawing.Point(105, 55);
            this.txt12.Name = "txt12";
            this.txt12.Size = new System.Drawing.Size(40, 23);
            this.txt12.TabIndex = 5;
            this.txt12.Text = "0";
            this.txt12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt20
            // 
            this.txt20.Location = new System.Drawing.Point(15, 85);
            this.txt20.Name = "txt20";
            this.txt20.Size = new System.Drawing.Size(40, 23);
            this.txt20.TabIndex = 6;
            this.txt20.Text = "0";
            this.txt20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt21
            // 
            this.txt21.Location = new System.Drawing.Point(60, 85);
            this.txt21.Name = "txt21";
            this.txt21.Size = new System.Drawing.Size(40, 23);
            this.txt21.TabIndex = 7;
            this.txt21.Text = "0";
            this.txt21.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txt22
            // 
            this.txt22.Location = new System.Drawing.Point(105, 85);
            this.txt22.Name = "txt22";
            this.txt22.Size = new System.Drawing.Size(40, 23);
            this.txt22.TabIndex = 8;
            this.txt22.Text = "0";
            this.txt22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelImages
            // 
            this.panelImages.ColumnCount = 2;
            this.panelImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelImages.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelImages.Controls.Add(this.panelSource, 0, 0);
            this.panelImages.Controls.Add(this.panelResult, 1, 0);
            this.panelImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelImages.Location = new System.Drawing.Point(3, 3);
            this.panelImages.Name = "panelImages";
            this.panelImages.RowCount = 1;
            this.panelImages.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelImages.Size = new System.Drawing.Size(994, 444);
            this.panelImages.TabIndex = 1;
            // 
            // panelSource
            // 
            this.panelSource.Controls.Add(this.pbSource);
            this.panelSource.Controls.Add(this.lblSource);
            this.panelSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSource.Location = new System.Drawing.Point(3, 3);
            this.panelSource.Name = "panelSource";
            this.panelSource.Padding = new System.Windows.Forms.Padding(10);
            this.panelSource.Size = new System.Drawing.Size(491, 498);
            this.panelSource.TabIndex = 0;
            // 
            // pbSource
            // 
            this.pbSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pbSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbSource.Location = new System.Drawing.Point(10, 31);
            this.pbSource.Name = "pbSource";
            this.pbSource.Size = new System.Drawing.Size(471, 457);
            this.pbSource.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSource.TabIndex = 0;
            this.pbSource.TabStop = false;
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSource.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSource.ForeColor = System.Drawing.Color.White;
            this.lblSource.Location = new System.Drawing.Point(10, 10);
            this.lblSource.Name = "lblSource";
            this.lblSource.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblSource.Size = new System.Drawing.Size(116, 31);
            this.lblSource.TabIndex = 5;
            this.lblSource.Text = "Orijinal Resim";
            // 
            // panelResult
            // 
            this.panelResult.Controls.Add(this.pbResult);
            this.panelResult.Controls.Add(this.lblResult);
            this.panelResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelResult.Location = new System.Drawing.Point(497, 3);
            this.panelResult.Name = "panelResult";
            this.panelResult.Padding = new System.Windows.Forms.Padding(10);
            this.panelResult.Size = new System.Drawing.Size(494, 498);
            this.panelResult.TabIndex = 1;
            // 
            // pbResult
            // 
            this.pbResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbResult.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbResult.Location = new System.Drawing.Point(10, 31);
            this.pbResult.Name = "pbResult";
            this.pbResult.Size = new System.Drawing.Size(474, 457);
            this.pbResult.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbResult.TabIndex = 1;
            this.pbResult.TabStop = false;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblResult.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblResult.ForeColor = System.Drawing.Color.White;
            this.lblResult.Location = new System.Drawing.Point(10, 10);
            this.lblResult.Name = "lblResult";
            this.lblResult.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.lblResult.Size = new System.Drawing.Size(109, 31);
            this.lblResult.TabIndex = 6;
            this.lblResult.Text = "İşlenmiş Hali";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "MainForm";
            this.Text = "CNN Konvolüsyon Simülatörü";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelImages.ResumeLayout(false);
            this.panelSource.ResumeLayout(false);
            this.panelSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSource)).EndInit();
            this.panelResult.ResumeLayout(false);
            this.panelResult.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResult)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panelControls;
        private System.Windows.Forms.TableLayoutPanel panelImages;
        private System.Windows.Forms.Panel panelSource;
        private System.Windows.Forms.PictureBox pbSource;
        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.Panel panelResult;
        private System.Windows.Forms.PictureBox pbResult;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbFilters;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.GroupBox grpCustom;
        private System.Windows.Forms.Button btnApplyCustom;
        private System.Windows.Forms.TextBox txt00;
        private System.Windows.Forms.TextBox txt01;
        private System.Windows.Forms.TextBox txt02;
        private System.Windows.Forms.TextBox txt10;
        private System.Windows.Forms.TextBox txt11;
        private System.Windows.Forms.TextBox txt12;
        private System.Windows.Forms.TextBox txt20;
        private System.Windows.Forms.TextBox txt21;
        private System.Windows.Forms.TextBox txt22;
    }
}
