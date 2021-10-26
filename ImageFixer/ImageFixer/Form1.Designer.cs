namespace ImageFixer
{
	partial class Form1
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
			this.pathTextBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.browseButton = new System.Windows.Forms.Button();
			this.processButton = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.label2 = new System.Windows.Forms.Label();
			this.ignoreTextBox = new System.Windows.Forms.TextBox();
			this.browseFixedButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.fixedTextBox = new System.Windows.Forms.TextBox();
			this.extendedBottomCheckBox = new System.Windows.Forms.CheckBox();
			this.doubleExceptionCheckBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// pathTextBox
			// 
			this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.pathTextBox.Location = new System.Drawing.Point(12, 25);
			this.pathTextBox.Name = "pathTextBox";
			this.pathTextBox.Size = new System.Drawing.Size(480, 20);
			this.pathTextBox.TabIndex = 0;
			this.pathTextBox.Text = "C:\\Projects\\Champions Images\\Powerhouse";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(73, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Images folder:";
			// 
			// browseButton
			// 
			this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseButton.Location = new System.Drawing.Point(498, 25);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(39, 23);
			this.browseButton.TabIndex = 2;
			this.browseButton.Text = "...";
			this.browseButton.UseVisualStyleBackColor = true;
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// processButton
			// 
			this.processButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.processButton.Location = new System.Drawing.Point(9, 172);
			this.processButton.Name = "processButton";
			this.processButton.Size = new System.Drawing.Size(75, 23);
			this.processButton.TabIndex = 3;
			this.processButton.Text = "Process";
			this.processButton.UseVisualStyleBackColor = true;
			this.processButton.Click += new System.EventHandler(this.processButton_Click);
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 87);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(127, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Ignore files that start with:";
			// 
			// ignoreTextBox
			// 
			this.ignoreTextBox.Location = new System.Drawing.Point(12, 103);
			this.ignoreTextBox.Name = "ignoreTextBox";
			this.ignoreTextBox.Size = new System.Drawing.Size(480, 20);
			this.ignoreTextBox.TabIndex = 5;
			this.ignoreTextBox.Text = "Powerhouse";
			// 
			// browseFixedButton
			// 
			this.browseFixedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.browseFixedButton.Location = new System.Drawing.Point(498, 64);
			this.browseFixedButton.Name = "browseFixedButton";
			this.browseFixedButton.Size = new System.Drawing.Size(39, 23);
			this.browseFixedButton.TabIndex = 8;
			this.browseFixedButton.Text = "...";
			this.browseFixedButton.UseVisualStyleBackColor = true;
			this.browseFixedButton.Click += new System.EventHandler(this.browseFixedButton_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 7;
			this.label3.Text = "Fixed folder:";
			// 
			// fixedTextBox
			// 
			this.fixedTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fixedTextBox.Location = new System.Drawing.Point(12, 64);
			this.fixedTextBox.Name = "fixedTextBox";
			this.fixedTextBox.Size = new System.Drawing.Size(480, 20);
			this.fixedTextBox.TabIndex = 6;
			this.fixedTextBox.Text = "C:\\Projects\\Champions Images\\Powerhouse\\Fixed";
			// 
			// extendedBottomCheckBox
			// 
			this.extendedBottomCheckBox.AutoSize = true;
			this.extendedBottomCheckBox.Location = new System.Drawing.Point(15, 139);
			this.extendedBottomCheckBox.Name = "extendedBottomCheckBox";
			this.extendedBottomCheckBox.Size = new System.Drawing.Size(106, 17);
			this.extendedBottomCheckBox.TabIndex = 9;
			this.extendedBottomCheckBox.Text = "Extended bottom";
			this.extendedBottomCheckBox.UseVisualStyleBackColor = true;
			// 
			// doubleExceptionCheckBox
			// 
			this.doubleExceptionCheckBox.AutoSize = true;
			this.doubleExceptionCheckBox.Location = new System.Drawing.Point(233, 139);
			this.doubleExceptionCheckBox.Name = "doubleExceptionCheckBox";
			this.doubleExceptionCheckBox.Size = new System.Drawing.Size(77, 17);
			this.doubleExceptionCheckBox.TabIndex = 10;
			this.doubleExceptionCheckBox.Text = "2 thinggies";
			this.doubleExceptionCheckBox.UseVisualStyleBackColor = true;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(549, 207);
			this.Controls.Add(this.doubleExceptionCheckBox);
			this.Controls.Add(this.extendedBottomCheckBox);
			this.Controls.Add(this.browseFixedButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.fixedTextBox);
			this.Controls.Add(this.ignoreTextBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.processButton);
			this.Controls.Add(this.browseButton);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pathTextBox);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox pathTextBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button processButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox ignoreTextBox;
		private System.Windows.Forms.Button browseFixedButton;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox fixedTextBox;
		private System.Windows.Forms.CheckBox extendedBottomCheckBox;
		private System.Windows.Forms.CheckBox doubleExceptionCheckBox;
	}
}

