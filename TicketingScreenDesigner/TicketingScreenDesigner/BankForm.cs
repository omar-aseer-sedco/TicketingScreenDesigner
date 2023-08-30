using System.Data;
using System.Text;
using System.Data.SqlClient;

namespace TicketingScreenDesigner {
	public partial class BankForm : Form {
		private const string titleText = "Ticketing Screen Designer";
		private readonly string bankName;
		private readonly SqlConnection connection;
		private readonly ActiveScreenController activeScreenController;

		private BankForm() {
			InitializeComponent();
			bankName = "";
			connection = Utils.CreateConnection();
			activeScreenController = new ActiveScreenController(this);
		}

		public BankForm(string bankName) : this() {
			this.bankName = bankName;
			Text = titleText + " - " + this.bankName;
			UpdateTitleLabel();
			UpdateListView();
		}

		private void UpdateTitleLabel() {
			titleLabel.Text = titleText + " - " + bankName;
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
				ExceptionHelper.HandleSqlException(ex, "screenId");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}
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
				ExceptionHelper.HandleSqlException(ex, "screenId");
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

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			int selectedCount = screensListView.SelectedIndices.Count;
			if (selectedCount == 0) {
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = false;
				setActiveButton.Enabled = false;
			}
			else if (selectedCount == 1) {
				editScreenButton.Enabled = true;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = true;
			}
			else {
				editScreenButton.Enabled = false;
				deleteScreenButton.Enabled = true;
				setActiveButton.Enabled = false;
			}
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
					ExceptionHelper.HandleSqlException(ex, "screenId");
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
					ExceptionHelper.HandleSqlException(ex, "screenId");
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
			if (screensListView.SelectedItems.Count == 0) {
				MessageBox.Show("Select a screen to activate.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
	}
}
