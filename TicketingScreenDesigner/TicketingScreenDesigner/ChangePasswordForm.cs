#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using BusinessLogicLayer.Controllers;
using ExceptionUtils;

namespace TicketingScreenDesigner
{
    public partial class ChangePasswordForm : Form {
		private readonly string bankName;

		private ChangePasswordForm() {
			InitializeComponent();
		}

		public ChangePasswordForm(string bankName) : this() {
			this.bankName = bankName;
		}

		private void registerShowPasswordCheckBox_CheckedChanged(object sender, EventArgs e) {
			try {
				oldPasswordTextBox.UseSystemPasswordChar = newPasswordTextBox.UseSystemPasswordChar = confirmPasswordTextBox.UseSystemPasswordChar = !showPasswordTextBox.Checked;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void changePasswordButton_Click(object sender, EventArgs e) {
			try {
				string oldPassword = oldPasswordTextBox.Text.Trim();
				string newPassword = newPasswordTextBox.Text.Trim();
				string confirm = confirmPasswordTextBox.Text.Trim();

				if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword)) {
					MessageBox.Show("Please enter your old password and a new password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (newPassword != confirm) {
					MessageBox.Show("Your new password and the confirmation do not match.", "Passwords do not match", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool? success = LoginController.ChangePassword(bankName, oldPassword, newPassword);

				if (success is null) {
					MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				else if (success == false) {
					MessageBox.Show("Your password is incorrect.", "Invalid credentials", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				else {
					MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
					Close();
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
				MessageBox.Show(ErrorMessages.UNEXPECTED_ERROR_MESSAGE, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
