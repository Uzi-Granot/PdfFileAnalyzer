namespace TestPdfFileAnalyzer
	{
	partial class Password
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
			this.label1 = new System.Windows.Forms.Label();
			this.MessageLabel = new System.Windows.Forms.Label();
			this.PasswordTextBox = new System.Windows.Forms.TextBox();
			this.OK_Button = new System.Windows.Forms.Button();
			this.Cancel_Button = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(202, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "PDF File is protected (Encrypted)";
			// 
			// MessageLabel
			// 
			this.MessageLabel.AutoSize = true;
			this.MessageLabel.Location = new System.Drawing.Point(26, 58);
			this.MessageLabel.Name = "MessageLabel";
			this.MessageLabel.Size = new System.Drawing.Size(216, 16);
			this.MessageLabel.TabIndex = 1;
			this.MessageLabel.Text = "Enter either user or owner password";
			// 
			// PasswordTextBox
			// 
			this.PasswordTextBox.Location = new System.Drawing.Point(29, 90);
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.Size = new System.Drawing.Size(282, 22);
			this.PasswordTextBox.TabIndex = 3;
			this.PasswordTextBox.TextChanged += new System.EventHandler(this.OnTextChanged);
			// 
			// OK_Button
			// 
			this.OK_Button.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.OK_Button.Location = new System.Drawing.Point(64, 135);
			this.OK_Button.Name = "OK_Button";
			this.OK_Button.Size = new System.Drawing.Size(89, 35);
			this.OK_Button.TabIndex = 4;
			this.OK_Button.Text = "OK";
			this.OK_Button.UseVisualStyleBackColor = true;
			// 
			// Cancel_Button
			// 
			this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Cancel_Button.Location = new System.Drawing.Point(184, 135);
			this.Cancel_Button.Name = "Cancel_Button";
			this.Cancel_Button.Size = new System.Drawing.Size(89, 35);
			this.Cancel_Button.TabIndex = 5;
			this.Cancel_Button.Text = "Cancel";
			this.Cancel_Button.UseVisualStyleBackColor = true;
			// 
			// Password
			// 
			this.AcceptButton = this.OK_Button;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.Cancel_Button;
			this.ClientSize = new System.Drawing.Size(337, 189);
			this.Controls.Add(this.Cancel_Button);
			this.Controls.Add(this.OK_Button);
			this.Controls.Add(this.PasswordTextBox);
			this.Controls.Add(this.MessageLabel);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Password";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Encryption Password";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClosing);
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label MessageLabel;
		private System.Windows.Forms.TextBox PasswordTextBox;
		private System.Windows.Forms.Button OK_Button;
		private System.Windows.Forms.Button Cancel_Button;
		}
	}