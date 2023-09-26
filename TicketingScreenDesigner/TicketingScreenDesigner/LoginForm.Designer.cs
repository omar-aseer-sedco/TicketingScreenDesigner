namespace TicketingScreenDesigner {
	partial class LoginForm {
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			loginBankNameTextBox = new TextBox();
			label1 = new Label();
			logInButton = new Button();
			welcomeLabel = new Label();
			logInGroupBox = new GroupBox();
			loginShowPasswordCheckBox = new CheckBox();
			loginPasswordTextBox = new TextBox();
			loginPasswordLabel = new Label();
			registerGroupBox = new GroupBox();
			registerButton = new Button();
			registerShowPasswordCheckBox = new CheckBox();
			confirmPasswordTextBox = new TextBox();
			label4 = new Label();
			registerPasswordTextBox = new TextBox();
			registerBankNameTextBox = new TextBox();
			label2 = new Label();
			registerBankNameLabel = new Label();
			logInGroupBox.SuspendLayout();
			registerGroupBox.SuspendLayout();
			SuspendLayout();
			// 
			// loginBankNameTextBox
			// 
			loginBankNameTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			loginBankNameTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			loginBankNameTextBox.Location = new Point(105, 50);
			loginBankNameTextBox.Name = "loginBankNameTextBox";
			loginBankNameTextBox.Size = new Size(205, 29);
			loginBankNameTextBox.TabIndex = 0;
			// 
			// label1
			// 
			label1.Anchor = AnchorStyles.Left;
			label1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label1.Location = new Point(6, 53);
			label1.Name = "label1";
			label1.Size = new Size(93, 21);
			label1.TabIndex = 1;
			label1.Text = "Bank Name:";
			label1.TextAlign = ContentAlignment.MiddleRight;
			// 
			// logInButton
			// 
			logInButton.Anchor = AnchorStyles.Bottom;
			logInButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			logInButton.Location = new Point(108, 237);
			logInButton.Name = "logInButton";
			logInButton.Size = new Size(100, 34);
			logInButton.TabIndex = 3;
			logInButton.Text = "Log In";
			logInButton.UseVisualStyleBackColor = true;
			logInButton.Click += LogInButton_Click;
			// 
			// welcomeLabel
			// 
			welcomeLabel.Anchor = AnchorStyles.Top;
			welcomeLabel.Font = new Font("Segoe UI", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
			welcomeLabel.Location = new Point(141, 18);
			welcomeLabel.Name = "welcomeLabel";
			welcomeLabel.Size = new Size(380, 51);
			welcomeLabel.TabIndex = 3;
			welcomeLabel.Text = "Welcome to Omar's Amazing Ticketing Screen Designer. Log In or register to get started.";
			welcomeLabel.TextAlign = ContentAlignment.TopCenter;
			// 
			// logInGroupBox
			// 
			logInGroupBox.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
			logInGroupBox.Controls.Add(loginShowPasswordCheckBox);
			logInGroupBox.Controls.Add(loginPasswordTextBox);
			logInGroupBox.Controls.Add(loginPasswordLabel);
			logInGroupBox.Controls.Add(loginBankNameTextBox);
			logInGroupBox.Controls.Add(label1);
			logInGroupBox.Controls.Add(logInButton);
			logInGroupBox.Location = new Point(12, 71);
			logInGroupBox.Name = "logInGroupBox";
			logInGroupBox.Size = new Size(316, 315);
			logInGroupBox.TabIndex = 4;
			logInGroupBox.TabStop = false;
			logInGroupBox.Text = "Log In";
			// 
			// loginShowPasswordCheckBox
			// 
			loginShowPasswordCheckBox.Anchor = AnchorStyles.Right;
			loginShowPasswordCheckBox.AutoSize = true;
			loginShowPasswordCheckBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			loginShowPasswordCheckBox.Location = new Point(187, 150);
			loginShowPasswordCheckBox.Name = "loginShowPasswordCheckBox";
			loginShowPasswordCheckBox.Size = new Size(123, 23);
			loginShowPasswordCheckBox.TabIndex = 2;
			loginShowPasswordCheckBox.Text = "Show password";
			loginShowPasswordCheckBox.UseVisualStyleBackColor = true;
			loginShowPasswordCheckBox.CheckedChanged += loginShowPasswordCheckBox_CheckedChanged;
			// 
			// loginPasswordTextBox
			// 
			loginPasswordTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			loginPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			loginPasswordTextBox.Location = new Point(105, 100);
			loginPasswordTextBox.Name = "loginPasswordTextBox";
			loginPasswordTextBox.Size = new Size(205, 29);
			loginPasswordTextBox.TabIndex = 1;
			loginPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// loginPasswordLabel
			// 
			loginPasswordLabel.Anchor = AnchorStyles.Left;
			loginPasswordLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			loginPasswordLabel.Location = new Point(6, 103);
			loginPasswordLabel.Name = "loginPasswordLabel";
			loginPasswordLabel.Size = new Size(93, 21);
			loginPasswordLabel.TabIndex = 4;
			loginPasswordLabel.Text = "Password:";
			loginPasswordLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// registerGroupBox
			// 
			registerGroupBox.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
			registerGroupBox.Controls.Add(registerButton);
			registerGroupBox.Controls.Add(registerShowPasswordCheckBox);
			registerGroupBox.Controls.Add(confirmPasswordTextBox);
			registerGroupBox.Controls.Add(label4);
			registerGroupBox.Controls.Add(registerPasswordTextBox);
			registerGroupBox.Controls.Add(registerBankNameTextBox);
			registerGroupBox.Controls.Add(label2);
			registerGroupBox.Controls.Add(registerBankNameLabel);
			registerGroupBox.Location = new Point(334, 71);
			registerGroupBox.Name = "registerGroupBox";
			registerGroupBox.Size = new Size(316, 315);
			registerGroupBox.TabIndex = 5;
			registerGroupBox.TabStop = false;
			registerGroupBox.Text = "Register";
			// 
			// registerButton
			// 
			registerButton.Anchor = AnchorStyles.Bottom;
			registerButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			registerButton.Location = new Point(108, 237);
			registerButton.Name = "registerButton";
			registerButton.Size = new Size(100, 34);
			registerButton.TabIndex = 8;
			registerButton.Text = "Register";
			registerButton.UseVisualStyleBackColor = true;
			registerButton.Click += registerButton_Click;
			// 
			// registerShowPasswordCheckBox
			// 
			registerShowPasswordCheckBox.Anchor = AnchorStyles.Right;
			registerShowPasswordCheckBox.AutoSize = true;
			registerShowPasswordCheckBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			registerShowPasswordCheckBox.Location = new Point(187, 200);
			registerShowPasswordCheckBox.Name = "registerShowPasswordCheckBox";
			registerShowPasswordCheckBox.Size = new Size(123, 23);
			registerShowPasswordCheckBox.TabIndex = 7;
			registerShowPasswordCheckBox.Text = "Show password";
			registerShowPasswordCheckBox.UseVisualStyleBackColor = true;
			registerShowPasswordCheckBox.CheckedChanged += registerShowPasswordCheckBox_CheckedChanged;
			// 
			// confirmPasswordTextBox
			// 
			confirmPasswordTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			confirmPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			confirmPasswordTextBox.Location = new Point(105, 150);
			confirmPasswordTextBox.Name = "confirmPasswordTextBox";
			confirmPasswordTextBox.Size = new Size(205, 29);
			confirmPasswordTextBox.TabIndex = 6;
			confirmPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// label4
			// 
			label4.Anchor = AnchorStyles.Left;
			label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label4.Location = new Point(6, 153);
			label4.Name = "label4";
			label4.Size = new Size(93, 21);
			label4.TabIndex = 10;
			label4.Text = "Confirm:";
			label4.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// registerPasswordTextBox
			// 
			registerPasswordTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			registerPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			registerPasswordTextBox.Location = new Point(105, 100);
			registerPasswordTextBox.Name = "registerPasswordTextBox";
			registerPasswordTextBox.Size = new Size(205, 29);
			registerPasswordTextBox.TabIndex = 5;
			registerPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// registerBankNameTextBox
			// 
			registerBankNameTextBox.Anchor =  AnchorStyles.Left | AnchorStyles.Right;
			registerBankNameTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			registerBankNameTextBox.Location = new Point(105, 50);
			registerBankNameTextBox.Name = "registerBankNameTextBox";
			registerBankNameTextBox.Size = new Size(205, 29);
			registerBankNameTextBox.TabIndex = 4;
			// 
			// label2
			// 
			label2.Anchor = AnchorStyles.Left;
			label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label2.Location = new Point(6, 103);
			label2.Name = "label2";
			label2.Size = new Size(93, 21);
			label2.TabIndex = 8;
			label2.Text = "Password:";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// registerBankNameLabel
			// 
			registerBankNameLabel.Anchor = AnchorStyles.Left;
			registerBankNameLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			registerBankNameLabel.Location = new Point(6, 53);
			registerBankNameLabel.Name = "registerBankNameLabel";
			registerBankNameLabel.Size = new Size(93, 21);
			registerBankNameLabel.TabIndex = 6;
			registerBankNameLabel.Text = "Bank Name:";
			registerBankNameLabel.TextAlign = ContentAlignment.MiddleRight;
			// 
			// LoginForm
			// 
			AcceptButton = logInButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(662, 397);
			Controls.Add(registerGroupBox);
			Controls.Add(logInGroupBox);
			Controls.Add(welcomeLabel);
			MinimumSize = new Size(678, 436);
			Name = "LoginForm";
			Text = "Log In";
			Resize += LoginForm_Resize;
			logInGroupBox.ResumeLayout(false);
			logInGroupBox.PerformLayout();
			registerGroupBox.ResumeLayout(false);
			registerGroupBox.PerformLayout();
			ResumeLayout(false);
		}

		#endregion

		private TextBox loginBankNameTextBox;
		private Label label1;
		private Button logInButton;
		private Label welcomeLabel;
		private GroupBox logInGroupBox;
		private TextBox loginPasswordTextBox;
		private Label loginPasswordLabel;
		private GroupBox registerGroupBox;
		private TextBox confirmPasswordTextBox;
		private Label label4;
		private TextBox registerPasswordTextBox;
		private TextBox registerBankNameTextBox;
		private Label label2;
		private Label registerBankNameLabel;
		private CheckBox registerShowPasswordCheckBox;
		private CheckBox loginShowPasswordCheckBox;
		private Button registerButton;
	}
}