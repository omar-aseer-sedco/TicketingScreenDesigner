using DataAccessLayer.DataClasses;
using DataAccessLayer.Constants;
using LogUtils;
using ExceptionUtils;
using BusinessLogicLayer.Controllers;

namespace TicketingScreenDesigner {
	public partial class LoginForm : Form {
		public LoginForm() {
			try {
				InitializeComponent();
				Show();

				var status = LoginController.Initialize();

				switch (status) {
					case InitializationStatus.FAILED_TO_CONNECT:
						LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Error establishing database connection. The database may have been configured incorrectly, or you may not have access to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(0);
						break;
					case InitializationStatus.FILE_CORRUPTED:
						LogsHelper.Log("Configuration file corrupted.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file is corrupted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(0);
						break;
					case InitializationStatus.FILE_NOT_FOUND:
						LogsHelper.Log("Configration file not found", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("The configuration file was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(0);
						break;
					case InitializationStatus.UNDEFINED_ERROR:
						LogsHelper.Log("Error establishing database connection - Login.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Failed to establish database connection due to an unexpected error.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						Environment.Exit(0);
						break;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private async void LogInButton_Click(object sender, EventArgs e) {
			try {
				string bankName = loginBankNameTextBox.Text.Trim();
				string password = loginPasswordTextBox.Text.Trim();

				if (string.IsNullOrEmpty(bankName) || string.IsNullOrEmpty(password)) {
					MessageBox.Show("Please enter a bank name and a password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool? passwordCorrect = LoginController.VerifyPassword(bankName, password);

				if (passwordCorrect is null) {
					LogsHelper.Log("Log In error.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("An error occurred. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if ((bool) passwordCorrect) {
					loginPasswordTextBox.Text = string.Empty;
					var screens = await BankController.GetScreensAsync(bankName);

					if (screens is null) {
						LogsHelper.Log("Error retrieving bank information.", DateTime.Now, EventSeverity.Error);
						MessageBox.Show("Error retrieving bank information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					var bankForm = new BankForm(bankName, screens);
					if (!bankForm.IsDisposed)
						bankForm.ShowDialog();
				}
				else {
					MessageBox.Show("Your bank name or your password is incorrect. Please try again.", "Invalid credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void registerButton_Click(object sender, EventArgs e) {
			try {
				string bankName = registerBankNameTextBox.Text.Trim();
				string password = registerPasswordTextBox.Text.Trim();
				string confirm = confirmPasswordTextBox.Text.Trim();

				if (string.IsNullOrEmpty(bankName) || string.IsNullOrEmpty(password)) {
					MessageBox.Show("Please enter a bank name and a password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool? bankExists = LoginController.CheckIfBankExists(bankName);

				if (bankExists is null) {
					LogsHelper.Log("Registration error.", DateTime.Now, EventSeverity.Error);
					MessageBox.Show("An error occurred. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if ((bool) bankExists) {
					MessageBox.Show("A bank with this name already exists. You can log in.", "Bank already exists", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				if (password != confirm) {
					MessageBox.Show("Passwords must match.", "Passwords do not match", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (LoginController.AddBank(new Bank(bankName, password)))
					MessageBox.Show("Registration successful. You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

				registerBankNameTextBox.Text = string.Empty;
				registerPasswordTextBox.Text = string.Empty;
				confirmPasswordTextBox.Text = string.Empty;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void loginShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			try {
				loginPasswordTextBox.UseSystemPasswordChar = !loginShowPasswordCheckBox.Checked;
				LogsHelper.Log("beep boop", DateTime.Now, EventSeverity.Info);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void registerShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			try {
				registerPasswordTextBox.UseSystemPasswordChar = confirmPasswordTextBox.UseSystemPasswordChar = !registerShowPasswordCheckBox.Checked;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void LoginForm_Resize(object sender, EventArgs e) {
			try {
				int horizontalCorrection = 16;
				int sideMargin = 12;
				int gap = 6;

				logInGroupBox.Size = new Size(((Width - horizontalCorrection) / 2) - sideMargin - (gap / 2), logInGroupBox.Height);
				registerGroupBox.Size = logInGroupBox.Size;
				registerGroupBox.Location = new Point(logInGroupBox.Location.X + logInGroupBox.Width + gap, logInGroupBox.Location.Y);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
