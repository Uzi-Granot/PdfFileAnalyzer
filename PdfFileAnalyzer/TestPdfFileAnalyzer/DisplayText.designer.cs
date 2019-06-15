namespace TestPdfFileAnalyzer
	{
	partial class DisplayText
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
			this.DisplayBox = new System.Windows.Forms.TextBox();
			this.Close_Button = new System.Windows.Forms.Button();
			this.SaveToFileButton = new System.Windows.Forms.Button();
			this.ButtonsBox = new System.Windows.Forms.GroupBox();
			this.ViewStreamButton = new System.Windows.Forms.Button();
			this.RotateLeftButton = new System.Windows.Forms.Button();
			this.RotateRightButton = new System.Windows.Forms.Button();
			this.ViewContentsButton = new System.Windows.Forms.Button();
			this.CopyToClipboardButton = new System.Windows.Forms.Button();
			this.HexDisplayButton = new System.Windows.Forms.Button();
			this.TextDisplayButton = new System.Windows.Forms.Button();
			this.ButtonsBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// DisplayBox
			// 
			this.DisplayBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.DisplayBox.Location = new System.Drawing.Point(5, 2);
			this.DisplayBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.DisplayBox.Multiline = true;
			this.DisplayBox.Name = "DisplayBox";
			this.DisplayBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.DisplayBox.Size = new System.Drawing.Size(1024, 460);
			this.DisplayBox.TabIndex = 0;
			this.DisplayBox.WordWrap = false;
			// 
			// Close_Button
			// 
			this.Close_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close_Button.Location = new System.Drawing.Point(772, 18);
			this.Close_Button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Close_Button.Name = "Close_Button";
			this.Close_Button.Size = new System.Drawing.Size(75, 44);
			this.Close_Button.TabIndex = 8;
			this.Close_Button.Text = "Close";
			this.Close_Button.UseVisualStyleBackColor = true;
			// 
			// SaveToFileButton
			// 
			this.SaveToFileButton.Location = new System.Drawing.Point(514, 18);
			this.SaveToFileButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.SaveToFileButton.Name = "SaveToFileButton";
			this.SaveToFileButton.Size = new System.Drawing.Size(110, 44);
			this.SaveToFileButton.TabIndex = 6;
			this.SaveToFileButton.Text = "Save to File";
			this.SaveToFileButton.UseVisualStyleBackColor = true;
			this.SaveToFileButton.Click += new System.EventHandler(this.OnSaveToFile);
			// 
			// ButtonsBox
			// 
			this.ButtonsBox.Controls.Add(this.HexDisplayButton);
			this.ButtonsBox.Controls.Add(this.TextDisplayButton);
			this.ButtonsBox.Controls.Add(this.ViewStreamButton);
			this.ButtonsBox.Controls.Add(this.RotateLeftButton);
			this.ButtonsBox.Controls.Add(this.RotateRightButton);
			this.ButtonsBox.Controls.Add(this.ViewContentsButton);
			this.ButtonsBox.Controls.Add(this.CopyToClipboardButton);
			this.ButtonsBox.Controls.Add(this.SaveToFileButton);
			this.ButtonsBox.Controls.Add(this.Close_Button);
			this.ButtonsBox.Location = new System.Drawing.Point(98, 469);
			this.ButtonsBox.Name = "ButtonsBox";
			this.ButtonsBox.Size = new System.Drawing.Size(864, 71);
			this.ButtonsBox.TabIndex = 1;
			this.ButtonsBox.TabStop = false;
			// 
			// ViewStreamButton
			// 
			this.ViewStreamButton.Location = new System.Drawing.Point(18, 18);
			this.ViewStreamButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.ViewStreamButton.Name = "ViewStreamButton";
			this.ViewStreamButton.Size = new System.Drawing.Size(106, 44);
			this.ViewStreamButton.TabIndex = 0;
			this.ViewStreamButton.Text = "View Stream";
			this.ViewStreamButton.UseVisualStyleBackColor = true;
			this.ViewStreamButton.Click += new System.EventHandler(this.OnViewStream);
			// 
			// RotateLeftButton
			// 
			this.RotateLeftButton.Font = new System.Drawing.Font("Wingdings 3", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.RotateLeftButton.Location = new System.Drawing.Point(450, 18);
			this.RotateLeftButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.RotateLeftButton.Name = "RotateLeftButton";
			this.RotateLeftButton.Size = new System.Drawing.Size(52, 44);
			this.RotateLeftButton.TabIndex = 5;
			this.RotateLeftButton.Text = "Q";
			this.RotateLeftButton.UseVisualStyleBackColor = true;
			this.RotateLeftButton.Click += new System.EventHandler(this.OnRotateLeft);
			// 
			// RotateRightButton
			// 
			this.RotateRightButton.Font = new System.Drawing.Font("Wingdings 3", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
			this.RotateRightButton.Location = new System.Drawing.Point(386, 18);
			this.RotateRightButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.RotateRightButton.Name = "RotateRightButton";
			this.RotateRightButton.Size = new System.Drawing.Size(52, 44);
			this.RotateRightButton.TabIndex = 4;
			this.RotateRightButton.Text = "P";
			this.RotateRightButton.UseVisualStyleBackColor = true;
			this.RotateRightButton.Click += new System.EventHandler(this.OnRotateRight);
			// 
			// ViewContentsButton
			// 
			this.ViewContentsButton.Location = new System.Drawing.Point(136, 18);
			this.ViewContentsButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.ViewContentsButton.Name = "ViewContentsButton";
			this.ViewContentsButton.Size = new System.Drawing.Size(110, 44);
			this.ViewContentsButton.TabIndex = 1;
			this.ViewContentsButton.Text = "View Contents";
			this.ViewContentsButton.UseVisualStyleBackColor = true;
			this.ViewContentsButton.Click += new System.EventHandler(this.OnViewContents);
			// 
			// CopyToClipboardButton
			// 
			this.CopyToClipboardButton.Location = new System.Drawing.Point(636, 18);
			this.CopyToClipboardButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.CopyToClipboardButton.Name = "CopyToClipboardButton";
			this.CopyToClipboardButton.Size = new System.Drawing.Size(124, 44);
			this.CopyToClipboardButton.TabIndex = 7;
			this.CopyToClipboardButton.Text = "Copy to Clipboard";
			this.CopyToClipboardButton.UseVisualStyleBackColor = true;
			this.CopyToClipboardButton.Click += new System.EventHandler(this.OnCopyToClipboard);
			// 
			// HexDisplayButton
			// 
			this.HexDisplayButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.HexDisplayButton.Location = new System.Drawing.Point(322, 18);
			this.HexDisplayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.HexDisplayButton.Name = "HexDisplayButton";
			this.HexDisplayButton.Size = new System.Drawing.Size(52, 44);
			this.HexDisplayButton.TabIndex = 3;
			this.HexDisplayButton.Text = "Hex";
			this.HexDisplayButton.UseVisualStyleBackColor = true;
			this.HexDisplayButton.Click += new System.EventHandler(this.OnHexDisplay);
			// 
			// TextDisplayButton
			// 
			this.TextDisplayButton.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.TextDisplayButton.Location = new System.Drawing.Point(258, 18);
			this.TextDisplayButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.TextDisplayButton.Name = "TextDisplayButton";
			this.TextDisplayButton.Size = new System.Drawing.Size(52, 44);
			this.TextDisplayButton.TabIndex = 2;
			this.TextDisplayButton.Text = "Text";
			this.TextDisplayButton.UseVisualStyleBackColor = true;
			this.TextDisplayButton.Click += new System.EventHandler(this.OnTextDisplay);
			// 
			// DisplayText
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Close_Button;
			this.ClientSize = new System.Drawing.Size(1031, 552);
			this.Controls.Add(this.ButtonsBox);
			this.Controls.Add(this.DisplayBox);
			this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.Name = "DisplayText";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Display Text";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResizeBegin += new System.EventHandler(this.OnResizeBegin);
			this.ResizeEnd += new System.EventHandler(this.OnResizeEnd);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.Resize += new System.EventHandler(this.OnResize);
			this.ButtonsBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.TextBox DisplayBox;
		private System.Windows.Forms.Button Close_Button;
		private System.Windows.Forms.Button SaveToFileButton;
		private System.Windows.Forms.GroupBox ButtonsBox;
		private System.Windows.Forms.Button CopyToClipboardButton;
		private System.Windows.Forms.Button ViewContentsButton;
		private System.Windows.Forms.Button RotateLeftButton;
		private System.Windows.Forms.Button RotateRightButton;
		private System.Windows.Forms.Button ViewStreamButton;
		private System.Windows.Forms.Button HexDisplayButton;
		private System.Windows.Forms.Button TextDisplayButton;
		}
	}