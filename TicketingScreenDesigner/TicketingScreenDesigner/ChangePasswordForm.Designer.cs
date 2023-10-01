namespace TicketingScreenDesigner {
	partial class ChangePasswordForm {
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
			changePasswordButton = new Button();
			showPasswordTextBox = new CheckBox();
			confirmPasswordTextBox = new TextBox();
			label4 = new Label();
			newPasswordTextBox = new TextBox();
			oldPasswordTextBox = new TextBox();
			label2 = new Label();
			oldPasswordLabel = new Label();
			SuspendLayout();
			// 
			// changePasswordButton
			// 
			changePasswordButton.Anchor = AnchorStyles.Bottom;
			changePasswordButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			changePasswordButton.Location = new Point(145, 207);
			changePasswordButton.Name = "changePasswordButton";
			changePasswordButton.Size = new Size(160, 34);
			changePasswordButton.TabIndex = 24;
			changePasswordButton.Text = "Change password";
			changePasswordButton.UseVisualStyleBackColor = true;
			changePasswordButton.Click += changePasswordButton_Click;
			// 
			// showPasswordTextBox
			// 
			showPasswordTextBox.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			showPasswordTextBox.AutoSize = true;
			showPasswordTextBox.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
			showPasswordTextBox.Location = new Point(316, 171);
			showPasswordTextBox.Name = "showPasswordTextBox";
			showPasswordTextBox.Size = new Size(123, 23);
			showPasswordTextBox.TabIndex = 23;
			showPasswordTextBox.Text = "Show password";
			showPasswordTextBox.UseVisualStyleBackColor = true;
			showPasswordTextBox.CheckedChanged += registerShowPasswordCheckBox_CheckedChanged;
			// 
			// confirmPasswordTextBox
			// 
			confirmPasswordTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			confirmPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			confirmPasswordTextBox.Location = new Point(132, 120);
			confirmPasswordTextBox.Name = "confirmPasswordTextBox";
			confirmPasswordTextBox.Size = new Size(307, 29);
			confirmPasswordTextBox.TabIndex = 21;
			confirmPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// label4
			// 
			label4.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label4.Location = new Point(10, 123);
			label4.Name = "label4";
			label4.Size = new Size(116, 21);
			label4.TabIndex = 26;
			label4.Text = "Confirm:";
			label4.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// newPasswordTextBox
			// 
			newPasswordTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			newPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			newPasswordTextBox.Location = new Point(132, 70);
			newPasswordTextBox.Name = "newPasswordTextBox";
			newPasswordTextBox.Size = new Size(307, 29);
			newPasswordTextBox.TabIndex = 20;
			newPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// oldPasswordTextBox
			// 
			oldPasswordTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			oldPasswordTextBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			oldPasswordTextBox.Location = new Point(132, 20);
			oldPasswordTextBox.Name = "oldPasswordTextBox";
			oldPasswordTextBox.Size = new Size(307, 29);
			oldPasswordTextBox.TabIndex = 19;
			oldPasswordTextBox.UseSystemPasswordChar = true;
			// 
			// label2
			// 
			label2.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			label2.Location = new Point(10, 73);
			label2.Name = "label2";
			label2.Size = new Size(116, 21);
			label2.TabIndex = 25;
			label2.Text = "New password:";
			label2.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// oldPasswordLabel
			// 
			oldPasswordLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
			oldPasswordLabel.Location = new Point(10, 23);
			oldPasswordLabel.Name = "oldPasswordLabel";
			oldPasswordLabel.Size = new Size(116, 21);
			oldPasswordLabel.TabIndex = 22;
			oldPasswordLabel.Text = "Old password:";
			oldPasswordLabel.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// ChangePasswordForm
			// 
			AcceptButton = changePasswordButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(451, 260);
			Controls.Add(changePasswordButton);
			Controls.Add(showPasswordTextBox);
			Controls.Add(confirmPasswordTextBox);
			Controls.Add(label4);
			Controls.Add(newPasswordTextBox);
			Controls.Add(oldPasswordTextBox);
			Controls.Add(label2);
			Controls.Add(oldPasswordLabel);
			FormBorderStyle = FormBorderStyle.Fixed3D;
			MaximizeBox = false;
			MinimumSize = new Size(467, 299);
			Name = "ChangePasswordForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Change Password";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button changePasswordButton;
		private CheckBox showPasswordTextBox;
		private TextBox confirmPasswordTextBox;
		private Label label4;
		private TextBox newPasswordTextBox;
		private TextBox oldPasswordTextBox;
		private Label label2;
		private Label oldPasswordLabel;
	}
}