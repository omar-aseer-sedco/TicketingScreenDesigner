using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.ComponentModel.Design;

namespace TicketingScreenDesigner {
	public partial class BankForm : Form {
		private const string TITLE_TEXT = "Ticketing Screen Designer";
		private readonly string bankName;
		private readonly SqlConnection connection;
		private readonly ActiveScreenController activeScreenController;

		private BankForm() {
			InitializeComponent();
			bankName = "";
			connection = DBUtils.CreateConnection();
			activeScreenController = new ActiveScreenController(this);
		}

		public BankForm(string bankName) : this() {
			this.bankName = bankName;
			Text = TITLE_TEXT + " - " + this.bankName;
			UpdateTitleLabel();
			UpdateListView();
		}

		private void UpdateTitleLabel() {
			titleLabel.Text = TITLE_TEXT + " - " + bankName;
			int sizeDifference = ClientSize.Width - titleLabel.Width;
			titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
		}

		private void addScreenButton_Click(object sender, EventArgs e) {
			var screenEditor = new ScreenEditor(this, bankName);
			screenEditor.Show();
		}

		public void UpdateListView() {
			screensListView.Items.Clear();

			string query = $"SELECT {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName;";
			SqlCommand command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);

			try {
				connection.Open();

				SqlDataReader reader = command.ExecuteReader();

				while (reader.Read()) {
					ListViewItem row = new() {
						Name = ScreensConstants.SCREEN_ID,
						Text = reader[ScreensConstants.SCREEN_ID].ToString()
					};

					ListViewItem.ListViewSubItem isActive = new() {
						Name = ScreensConstants.IS_ACTIVE,
						Text = (bool) reader[ScreensConstants.IS_ACTIVE] ? "Yes" : "No"
					};
					row.SubItems.Add(isActive);

					ListViewItem.ListViewSubItem screenTitle = new() {
						Name = ScreensConstants.SCREEN_TITLE,
						Text = reader[ScreensConstants.SCREEN_TITLE].ToString()
					};
					row.SubItems.Add(screenTitle);

					screensListView.Items.Add(row);
				}

				reader.Close();
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			CheckButtonActivation();
		}

		private void editScreenButton_Click(object sender, EventArgs e) {
			if (screensListView.SelectedItems.Count == 0) {
				MessageBox.Show("Select a screen to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			string screenId = screensListView.SelectedItems[0].Text;
			string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
			bool isActive = screensListView.SelectedItems[0].SubItems[ScreensConstants.IS_ACTIVE].Text == "Yes";
			var screenEditor = new ScreenEditor(this, bankName, screenId, screenTitle, isActive);
			screenEditor.Show();
		}

		private bool DeleteSelected() {
			int selectedCount = screensListView.SelectedItems.Count;

			if (selectedCount == 0)
				return false;

			var query = new StringBuilder($"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} IN (");
			var command = new SqlCommand();
			command.Parameters.AddWithValue("@bankName", bankName);

			int i = 0;
			foreach (ListViewItem row in screensListView.SelectedItems) {
				query.Append("@P").Append(i).Append(',');
				command.Parameters.Add("@P" + i, SqlDbType.VarChar).Value = row.SubItems[ScreensConstants.SCREEN_ID].Text;
				++i;
			}

			query.Length--; // remove last ,
			query.Append(");");

			command.Connection = connection;
			command.CommandText = query.ToString();

			bool success = false;

			try {
				connection.Open();
				success = command.ExecuteNonQuery() == selectedCount;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "screen ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return success;
		}

		private void deleteScreenButton_Click(object sender, EventArgs e) {
			if (screensListView.SelectedItems.Count == 0) {
				MessageBox.Show("Select a screen to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected screen(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

			if (confirmationResult == DialogResult.No) {
				return;
			}

			if (!DeleteSelected()) {
				MessageBox.Show("Screen deletion failed.", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			UpdateListView();
		}

		private void CheckButtonActivation() {
			int selectedCount = screensListView.SelectedIndices.Count;
			if (selectedCount == 0) {
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = false;
				setActiveButton.Enabled = false;
				previewButton.Enabled = false;
			}
			else if (selectedCount == 1) {
				editScreenButton.Enabled = true;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = true;
				previewButton.Enabled = true;
			}
			else {
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = false;
				previewButton.Enabled = false;
			}
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			CheckButtonActivation();
		}

		private class ActiveScreenController {
			BankForm parentForm;

			public ActiveScreenController(BankForm parentForm) {
				this.parentForm = parentForm;
			}

			public string? GetActiveScreenId() {
				string query = $"SELECT {ScreensConstants.SCREEN_ID} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.IS_ACTIVE} = 1;";
				var command = new SqlCommand(query, parentForm.connection);
				command.Parameters.AddWithValue("@bankName", parentForm.bankName);

				string? ret = null;

				try {
					parentForm.connection.Open();

					var reader = command.ExecuteReader();

					if (reader.Read()) {
						ret = reader.GetString(0);
					}
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Screen ID");
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}
				finally {
					parentForm.connection.Close();
				}

				return ret;
			}

			private bool SetIsActive(string screenId, bool active) {
				string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId";
				var command = new SqlCommand(query, parentForm.connection);
				command.Parameters.AddWithValue("@isActive", active ? 1 : 0);
				command.Parameters.AddWithValue("@bankName", parentForm.bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				bool success = false;

				try {
					parentForm.connection.Open();

					success = command.ExecuteNonQuery() == 1;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Screen ID");
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}
				finally {
					parentForm.connection.Close();
				}

				return success;
			}

			public bool DeactivateScreen(string screenId) {
				return SetIsActive(screenId, false);
			}

			public bool ActivateScreen(string screenId) {
				string? currentlyActiveScreenId = GetActiveScreenId();

				if (currentlyActiveScreenId is not null && currentlyActiveScreenId != screenId) {
					DeactivateScreen(currentlyActiveScreenId);
				}

				return SetIsActive(screenId, true);
			}
		}

		public string? GetActiveScreenId() {
			return activeScreenController.GetActiveScreenId();
		}

		public bool ActivateScreen(string screenId) {
			return activeScreenController.ActivateScreen(screenId);
		}

		public bool DeactivateScreen(string screenId) {
			return activeScreenController.DeactivateScreen(screenId);
		}

		private void setActiveButton_Click(object sender, EventArgs e) {
			if (screensListView.SelectedItems.Count != 1) {
				MessageBox.Show("Select one screen to activate.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			string selectedScreenId = screensListView.SelectedItems[0].Text;
			string? currentlyActiveScreenId = GetActiveScreenId();

			if (currentlyActiveScreenId is not null) {
				if (currentlyActiveScreenId == selectedScreenId) {
					MessageBox.Show("The selected screen is already active.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}
				else {
					var confirmationResult = MessageBox.Show("Setting this screen as active will deactivate the currently active screen. Do you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}

					DeactivateScreen(currentlyActiveScreenId);
				}
			}

			ActivateScreen(selectedScreenId);
			UpdateListView();
		}

		private void previewButton_Click(object sender, EventArgs e) {
			MessageBox.Show("This doesn't do anything yet.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
			return;

			//if (screensListView.SelectedItems.Count != 1) {
			//	MessageBox.Show("Select one screen to preview.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//	return;
			//}

			//PreviewScreen(screensListView.SelectedItems[0].Text, screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text);
		}
		
		private List<TicketingButton> GetButtons(string screenId) {
			string query = $"SELECT * FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId;";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenId);

			var ret = new List<TicketingButton>();
			try {
				connection.Open();

				string buttonBankName, buttonScreenId, buttonId, type, nameEn, nameAr;
				string? service, messageEn, messageAr;
				var reader = command.ExecuteReader();

				while (reader.Read()) {
					buttonBankName = reader[ButtonsConstants.BANK_NAME].ToString() ?? $"Error getting {ButtonsConstants.BANK_NAME}";
					buttonScreenId = reader[ButtonsConstants.SCREEN_ID].ToString() ?? $"Error getting {ButtonsConstants.SCREEN_ID}";
					buttonId = reader[ButtonsConstants.BUTTON_ID].ToString() ?? $"Error getting {ButtonsConstants.BUTTON_ID}";
					type = reader[ButtonsConstants.TYPE].ToString() ?? $"Error getting {ButtonsConstants.TYPE}";
					nameEn = reader[ButtonsConstants.NAME_EN].ToString() ?? $"Error getting {ButtonsConstants.NAME_EN}";
					nameAr = reader[ButtonsConstants.NAME_AR].ToString() ?? $"Error getting {ButtonsConstants.NAME_AR}";
					service = reader[ButtonsConstants.SERVICE].ToString();
					messageEn = reader[ButtonsConstants.MESSAGE_EN].ToString();
					messageAr = reader[ButtonsConstants.MESSAGE_AR].ToString();

					ret.Add(new TicketingButton(buttonBankName, buttonScreenId, buttonId, type, nameEn, nameAr, service, messageEn, messageAr));
				}
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "button ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return ret;
		}

		public void PreviewScreen(string screenId, string screenTitle) {
			var previewForm = new PreviewFormTest(screenTitle, GetButtons(screenId));
			previewForm.Show();
		}
	}
}
