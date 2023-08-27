namespace TicketingScreenDesigner
{
	partial class LoginForm
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
			bankNameTextBox = new TextBox();
			label1 = new Label();
			logInButton = new Button();
			welcomeLabel = new Label();
			SuspendLayout();
			// 
			// bankNameTextBox
			// 
			bankNameTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			bankNameTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			bankNameTextBox.Location = new Point(114, 94);
			bankNameTextBox.Name = "bankNameTextBox";
			bankNameTextBox.Size = new Size(258, 29);
			bankNameTextBox.TabIndex = 0;
			// 
			// label1
			// 
			label1.Anchor = AnchorStyles.Left;
			label1.AutoSize = true;
			label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label1.Location = new Point(12, 97);
			label1.Name = "label1";
			label1.Size = new Size(93, 21);
			label1.TabIndex = 1;
			label1.Text = "Bank Name:";
			// 
			// logInButton
			// 
			logInButton.Anchor = AnchorStyles.Bottom;
			logInButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			logInButton.Location = new Point(142, 174);
			logInButton.Name = "logInButton";
			logInButton.Size = new Size(100, 34);
			logInButton.TabIndex = 2;
			logInButton.Text = "Log In";
			logInButton.UseVisualStyleBackColor = true;
			logInButton.Click += LogInButton_Click;
			// 
			// welcomeLabel
			// 
			welcomeLabel.Anchor = AnchorStyles.Top;
			welcomeLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
			welcomeLabel.Location = new Point(11, 28);
			welcomeLabel.Name = "welcomeLabel";
			welcomeLabel.Size = new Size(362, 51);
			welcomeLabel.TabIndex = 3;
			welcomeLabel.Text = "Welcome to [Name pending]. Enter the name of your bank to get started.";
			welcomeLabel.TextAlign = ContentAlignment.TopCenter;
			// 
			// LoginForm
			// 
			AcceptButton = logInButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(384, 231);
			Controls.Add(welcomeLabel);
			Controls.Add(logInButton);
			Controls.Add(label1);
			Controls.Add(bankNameTextBox);
			Name = "LoginForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "Log In";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox bankNameTextBox;
		private Label label1;
		private Button logInButton;
		private Label welcomeLabel;
	}
}