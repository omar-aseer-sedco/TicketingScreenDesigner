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

				if (string.IsNullOrEmpty(bankName)) {
					MessageBox.Show("Please enter the name of your bank.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				var bankForm = new BankForm(bankName, BankController.AccessBank(new Bank(bankName)));
				bankForm.ShowDialog();
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
	}
}
