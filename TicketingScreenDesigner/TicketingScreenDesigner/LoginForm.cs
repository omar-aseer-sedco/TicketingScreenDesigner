using BusinessLogicLayer;
using DataAccessLayer.DataClasses;
using LogUtils;

namespace TicketingScreenDesigner {
	public partial class LoginForm : Form {
		public LoginForm() {
			InitializeComponent();
			LogsHelper.InitializeLogsDirectory();
		}

		private void LogInButton_Click(object sender, EventArgs e) {
			try {
				string bankName = loginBankNameTextBox.Text.Trim();
				string password = loginPasswordTextBox.Text.Trim();

				if (string.IsNullOrEmpty(bankName) || string.IsNullOrEmpty(password)) {
					MessageBox.Show("Please enter a bank name and a password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (BankController.VerifyPassword(bankName, password)) {
					loginPasswordTextBox.Text = string.Empty;
					var screens = BankController.GetScreens(bankName);
					var bankForm = new BankForm(bankName, screens);
					bankForm.ShowDialog();
				}
				else {
					MessageBox.Show("Your bank name or your password is incorrect. Please try again.", "Invalid credentials", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void registerButton_Click(object sender, EventArgs e) {
			try {
				string bankName = registerBankNameTextBox.Text.Trim();
				string password = registerPasswordTextBox.Text.Trim();
				string confirm = confirmPasswordTextBox.Text.Trim();

				if (BankController.CheckIfBankExists(bankName)) {
					MessageBox.Show("A bank with this name already exists. You can log in.", "Bank already exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				if (password != confirm) {
					MessageBox.Show("Passwords must match.", "Passwords do not match", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				BankController.AddBank(new Bank(bankName, password));
				MessageBox.Show("Registration successful. You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

				registerBankNameTextBox.Text = string.Empty;
				registerPasswordTextBox.Text = string.Empty;
				confirmPasswordTextBox.Text = string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void loginShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			loginPasswordTextBox.UseSystemPasswordChar = !loginShowPasswordCheckBox.Checked;
		}

		private void registerShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			registerPasswordTextBox.UseSystemPasswordChar = confirmPasswordTextBox.UseSystemPasswordChar = !registerShowPasswordCheckBox.Checked;
		}

		private void LoginForm_Resize(object sender, EventArgs e) {
			int horizontalCorrection = 16;
			int sideMargin = 12;
			int gap = 6;

			logInGroupBox.Size = new Size(((Width - horizontalCorrection) / 2) - sideMargin - (gap / 2), logInGroupBox.Height);
			registerGroupBox.Size = logInGroupBox.Size;
			registerGroupBox.Location = new Point(logInGroupBox.Location.X + logInGroupBox.Width + gap, logInGroupBox.Location.Y);
		}
	}
}
