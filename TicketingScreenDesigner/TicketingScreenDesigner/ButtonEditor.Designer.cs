namespace TicketingScreenDesigner {
	partial class ButtonEditor {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			buttonIdTextBox = new TextBox();
			buttonIdLabel = new Label();
			typeComboBox = new ComboBox();
			saveButton = new Button();
			nameEnglishLabel = new Label();
			nameArabicLabel = new Label();
			typeLabel = new Label();
			nameEnTextBox = new TextBox();
			nameArTextBox = new TextBox();
			cancelButton = new Button();
			showMessagePanel = new Panel();
			messageArTextBox = new TextBox();
			messageEnTextBox = new TextBox();
			label1 = new Label();
			label2 = new Label();
			issueTicketPanel = new Panel();
			serviceTextBox = new TextBox();
			label4 = new Label();
			autoFillIdButton = new Button();
			showMessagePanel.SuspendLayout();
			issueTicketPanel.SuspendLayout();
			SuspendLayout();
			// 
			// buttonIdTextBox
			// 
			buttonIdTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			buttonIdTextBox.Location = new Point(121, 15);
			buttonIdTextBox.Name = "buttonIdTextBox";
			buttonIdTextBox.Size = new Size(398, 23);
			buttonIdTextBox.TabIndex = 0;
			// 
			// buttonIdLabel
			// 
			buttonIdLabel.AutoSize = true;
			buttonIdLabel.Location = new Point(54, 18);
			buttonIdLabel.Name = "buttonIdLabel";
			buttonIdLabel.Size = new Size(60, 15);
			buttonIdLabel.TabIndex = 1;
			buttonIdLabel.Text = "Button ID:";
			// 
			// typeComboBox
			// 
			typeComboBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			typeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
			typeComboBox.FormattingEnabled = true;
			typeComboBox.Items.AddRange(new object[] { "Issue Ticket", "Show Message" });
			typeComboBox.Location = new Point(121, 102);
			typeComboBox.Name = "typeComboBox";
			typeComboBox.Size = new Size(479, 23);
			typeComboBox.TabIndex = 3;
			typeComboBox.SelectedIndexChanged += typeComboBox_SelectedIndexChanged;
			// 
			// saveButton
			// 
			saveButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			saveButton.Location = new Point(525, 222);
			saveButton.Name = "saveButton";
			saveButton.Size = new Size(75, 23);
			saveButton.TabIndex = 8;
			saveButton.Text = "Save";
			saveButton.UseVisualStyleBackColor = true;
			saveButton.Click += saveButton_Click;
			// 
			// nameEnglishLabel
			// 
			nameEnglishLabel.AutoSize = true;
			nameEnglishLabel.Location = new Point(23, 47);
			nameEnglishLabel.Name = "nameEnglishLabel";
			nameEnglishLabel.Size = new Size(91, 15);
			nameEnglishLabel.TabIndex = 4;
			nameEnglishLabel.Text = "Name (English):";
			// 
			// nameArabicLabel
			// 
			nameArabicLabel.AutoSize = true;
			nameArabicLabel.Location = new Point(27, 76);
			nameArabicLabel.Name = "nameArabicLabel";
			nameArabicLabel.Size = new Size(87, 15);
			nameArabicLabel.TabIndex = 5;
			nameArabicLabel.Text = "Name (Arabic):";
			// 
			// typeLabel
			// 
			typeLabel.AutoSize = true;
			typeLabel.Location = new Point(80, 105);
			typeLabel.Name = "typeLabel";
			typeLabel.Size = new Size(34, 15);
			typeLabel.TabIndex = 6;
			typeLabel.Text = "Type:";
			// 
			// nameEnTextBox
			// 
			nameEnTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			nameEnTextBox.Location = new Point(121, 44);
			nameEnTextBox.Name = "nameEnTextBox";
			nameEnTextBox.Size = new Size(479, 23);
			nameEnTextBox.TabIndex = 1;
			// 
			// nameArTextBox
			// 
			nameArTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			nameArTextBox.Location = new Point(121, 73);
			nameArTextBox.Name = "nameArTextBox";
			nameArTextBox.RightToLeft = RightToLeft.Yes;
			nameArTextBox.Size = new Size(479, 23);
			nameArTextBox.TabIndex = 2;
			// 
			// cancelButton
			// 
			cancelButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.Location = new Point(444, 222);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 23);
			cancelButton.TabIndex = 7;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// showMessagePanel
			// 
			showMessagePanel.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			showMessagePanel.Controls.Add(messageArTextBox);
			showMessagePanel.Controls.Add(messageEnTextBox);
			showMessagePanel.Controls.Add(label1);
			showMessagePanel.Controls.Add(label2);
			showMessagePanel.Location = new Point(0, 131);
			showMessagePanel.Name = "showMessagePanel";
			showMessagePanel.Size = new Size(613, 52);
			showMessagePanel.TabIndex = 10;
			showMessagePanel.Visible = false;
			// 
			// messageArTextBox
			// 
			messageArTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			messageArTextBox.Location = new Point(121, 29);
			messageArTextBox.Name = "messageArTextBox";
			messageArTextBox.RightToLeft = RightToLeft.Yes;
			messageArTextBox.Size = new Size(479, 23);
			messageArTextBox.TabIndex = 5;
			// 
			// messageEnTextBox
			// 
			messageEnTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			messageEnTextBox.Location = new Point(121, 0);
			messageEnTextBox.Name = "messageEnTextBox";
			messageEnTextBox.Size = new Size(479, 23);
			messageEnTextBox.TabIndex = 4;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(13, 32);
			label1.Name = "label1";
			label1.Size = new Size(101, 15);
			label1.TabIndex = 10;
			label1.Text = "Message (Arabic):";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(9, 3);
			label2.Name = "label2";
			label2.Size = new Size(105, 15);
			label2.TabIndex = 9;
			label2.Text = "Message (English):";
			// 
			// issueTicketPanel
			// 
			issueTicketPanel.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			issueTicketPanel.Controls.Add(serviceTextBox);
			issueTicketPanel.Controls.Add(label4);
			issueTicketPanel.Location = new Point(0, 189);
			issueTicketPanel.Name = "issueTicketPanel";
			issueTicketPanel.Size = new Size(613, 23);
			issueTicketPanel.TabIndex = 13;
			issueTicketPanel.Visible = false;
			// 
			// serviceTextBox
			// 
			serviceTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			serviceTextBox.Location = new Point(121, 0);
			serviceTextBox.Name = "serviceTextBox";
			serviceTextBox.Size = new Size(479, 23);
			serviceTextBox.TabIndex = 6;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(67, 3);
			label4.Name = "label4";
			label4.Size = new Size(47, 15);
			label4.TabIndex = 9;
			label4.Text = "Service:";
			// 
			// autoFillIdButton
			// 
			autoFillIdButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			autoFillIdButton.Location = new Point(525, 14);
			autoFillIdButton.Name = "autoFillIdButton";
			autoFillIdButton.Size = new Size(75, 23);
			autoFillIdButton.TabIndex = 14;
			autoFillIdButton.Text = "Autofill";
			autoFillIdButton.UseVisualStyleBackColor = true;
			autoFillIdButton.Click += autoFillIdButton_Click;
			// 
			// ButtonEditor
			// 
			AcceptButton = saveButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cancelButton;
			ClientSize = new Size(612, 255);
			Controls.Add(autoFillIdButton);
			Controls.Add(issueTicketPanel);
			Controls.Add(showMessagePanel);
			Controls.Add(cancelButton);
			Controls.Add(nameArTextBox);
			Controls.Add(nameEnTextBox);
			Controls.Add(typeLabel);
			Controls.Add(nameArabicLabel);
			Controls.Add(nameEnglishLabel);
			Controls.Add(saveButton);
			Controls.Add(typeComboBox);
			Controls.Add(buttonIdLabel);
			Controls.Add(buttonIdTextBox);
			MinimumSize = new Size(628, 233);
			Name = "ButtonEditor";
			Text = "Button Editor";
			showMessagePanel.ResumeLayout(false);
			showMessagePanel.PerformLayout();
			issueTicketPanel.ResumeLayout(false);
			issueTicketPanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox buttonIdTextBox;
		private Label buttonIdLabel;
		private ComboBox typeComboBox;
		private Button saveButton;
		private Label nameEnglishLabel;
		private Label nameArabicLabel;
		private Label typeLabel;
		private TextBox nameEnTextBox;
		private TextBox nameArTextBox;
		private Button cancelButton;
		private Panel showMessagePanel;
		private TextBox messageArTextBox;
		private TextBox messageEnTextBox;
		private Label label1;
		private Label label2;
		private Panel issueTicketPanel;
		private TextBox serviceTextBox;
		private Label label4;
		private Button autoFillIdButton;
	}
}