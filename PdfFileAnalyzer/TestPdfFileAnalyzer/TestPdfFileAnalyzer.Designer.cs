namespace TestPdfFileAnalyzer
	{
	partial class TestPdfFileAnalyzer
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
			this.AnalyzeButton = new System.Windows.Forms.Button();
			this.CopyrightTextBox = new System.Windows.Forms.RichTextBox();
			this.RecentFilesButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// AnalyzeButton
			// 
			this.AnalyzeButton.Location = new System.Drawing.Point(152, 375);
			this.AnalyzeButton.Name = "AnalyzeButton";
			this.AnalyzeButton.Size = new System.Drawing.Size(149, 45);
			this.AnalyzeButton.TabIndex = 1;
			this.AnalyzeButton.Text = "Open PDF File";
			this.AnalyzeButton.UseVisualStyleBackColor = true;
			this.AnalyzeButton.Click += new System.EventHandler(this.OnOpenPdfFile);
			// 
			// CopyrightTextBox
			// 
			this.CopyrightTextBox.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.CopyrightTextBox.Location = new System.Drawing.Point(37, 47);
			this.CopyrightTextBox.MaxLength = 2048;
			this.CopyrightTextBox.Name = "CopyrightTextBox";
			this.CopyrightTextBox.ReadOnly = true;
			this.CopyrightTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.CopyrightTextBox.Size = new System.Drawing.Size(548, 276);
			this.CopyrightTextBox.TabIndex = 0;
			this.CopyrightTextBox.Text = "";
			// 
			// RecentFilesButton
			// 
			this.RecentFilesButton.Location = new System.Drawing.Point(321, 375);
			this.RecentFilesButton.Name = "RecentFilesButton";
			this.RecentFilesButton.Size = new System.Drawing.Size(149, 45);
			this.RecentFilesButton.TabIndex = 2;
			this.RecentFilesButton.Text = "Recent Files";
			this.RecentFilesButton.UseVisualStyleBackColor = true;
			this.RecentFilesButton.Click += new System.EventHandler(this.OnRecentFiles);
			// 
			// TestPdfFileAnalyzer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(622, 442);
			this.Controls.Add(this.RecentFilesButton);
			this.Controls.Add(this.AnalyzeButton);
			this.Controls.Add(this.CopyrightTextBox);
			this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.Name = "TestPdfFileAnalyzer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PDF File Analyzer";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);

			}

		#endregion

		private System.Windows.Forms.Button AnalyzeButton;
		private System.Windows.Forms.RichTextBox CopyrightTextBox;
		private System.Windows.Forms.Button RecentFilesButton;
		}
	}

