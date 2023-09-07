#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Text;
using System.Data.SqlClient;
using DataAccessLayer;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;

namespace TicketingScreenDesigner
{
    public partial class BankForm : Form {
		private const string TITLE_TEXT = "Ticketing Screen Designer";
		private readonly string bankName;
		private readonly SqlConnection connection;
		private readonly ActiveScreenController activeScreenController;
		private readonly List<TicketingScreen> screens;

		private BankForm() {
			InitializeComponent();
			connection = DBUtils.CreateConnection();
			bankName = "";
			activeScreenController = new ActiveScreenController(this);
			screens = new List<TicketingScreen>();
		}

		public BankForm(string bankName) : this() {
			this.bankName = bankName;
			Text = TITLE_TEXT + " - " + this.bankName;
			UpdateTitleLabel();
			UpdateListView();
		}

		public BankForm(string bankName , List<TicketingScreen> screens) : this(bankName) {
			this.screens = screens;
		}

		private void UpdateTitleLabel() {
			titleLabel.Text = TITLE_TEXT + " - " + bankName;
			int sizeDifference = ClientSize.Width - titleLabel.Width;
			titleLabel.Location = new Point(sizeDifference / 2, titleLabel.Location.Y);
		}

		private void Add() {
			var screenEditor = new ScreenEditor(connection, this, bankName);
			screenEditor.ShowDialog();
		}

		private void addScreenButton_Click(object sender, EventArgs e) {
			Add();
		}

		public void UpdateListView() {
			try {
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
				finally {
					connection.Close();
				}

				UpdateFormButtonActivation();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		public bool CheckIfScreenExists(int screenId) {
			bool ret = false;

			string query = $"SELECT COUNT({ScreensConstants.SCREEN_ID}) FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.SCREEN_ID} = @screenId;";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@screenId", screenId);

			try {
				connection.Open();

				ret = (int) command.ExecuteScalar() == 1;
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

			return ret;
		}

		private void Edit() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = int.Parse(screensListView.SelectedItems[0].Text);

				if (!CheckIfScreenExists(screenId)) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				bool isActive = GetActiveScreenId() == screenId;
				var screenEditor = new ScreenEditor(connection, this, bankName, screenId, screenTitle, isActive);
				screenEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void editScreenButton_Click(object sender, EventArgs e) {
			Edit();
		}

		private bool DeleteSelected() {
			bool success = false;

			try {
				int selectedCount = screensListView.SelectedItems.Count;

				if (selectedCount == 0)
					return false;

				var query = new StringBuilder($"DELETE FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} IN (");
				var command = new SqlCommand();
				command.Parameters.AddWithValue("@bankName", bankName);

				int i = 0;
				foreach (ListViewItem row in screensListView.SelectedItems) {
					query.Append("@P").Append(i).Append(',');
					command.Parameters.AddWithValue("@P" + i, row.SubItems[ScreensConstants.SCREEN_ID].Text);

					++i;
				}

				query.Length--;
				query.Append(");");

				command.Connection = connection;
				command.CommandText = query.ToString();


				try {
					connection.Open();
					command.ExecuteNonQuery();
					success = true;
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "screen ID");
				}
				finally {
					connection.Close();
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private void Delete() {
			try {
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
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void deleteScreenButton_Click(object sender, EventArgs e) {
			Delete();
		}

		private void UpdateFormButtonActivation() {
			try {
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
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void screensListView_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateFormButtonActivation();
		}

		private class ActiveScreenController {
			BankForm parentForm;

			public ActiveScreenController(BankForm parentForm) {
				this.parentForm = parentForm;
			}

			public int? GetActiveScreenId() {
				int? ret = null;

				try {
					string query = $"SELECT {ScreensConstants.SCREEN_ID} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.IS_ACTIVE} = 1;";
					var command = new SqlCommand(query, parentForm.connection);
					command.Parameters.AddWithValue("@bankName", parentForm.bankName);

					try {
						parentForm.connection.Open();

						var reader = command.ExecuteReader();

						if (reader.Read()) {
							ret = reader.GetInt32(0);
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
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}

				return ret;
			}

			private bool SetIsActive(int screenId, bool active) {
				bool success = false;

				try {
					string query = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.IS_ACTIVE} = @isActive WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} = @screenId";
					var command = new SqlCommand(query, parentForm.connection);
					command.Parameters.AddWithValue("@isActive", active ? 1 : 0);
					command.Parameters.AddWithValue("@bankName", parentForm.bankName);
					command.Parameters.AddWithValue("@screenId", screenId);

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
				}
				catch (Exception ex) {
					ExceptionHelper.HandleGeneralException(ex);
				}

				return success;
			}

			public bool DeactivateScreen(int screenId) {
				return SetIsActive(screenId, false);
			}

			public bool ActivateScreen(int screenId) {
				int? currentlyActiveScreenId = GetActiveScreenId();

				if (currentlyActiveScreenId is not null && currentlyActiveScreenId != screenId) {
					DeactivateScreen((int) currentlyActiveScreenId);
				}

				return SetIsActive(screenId, true);
			}
		}

		public int? GetActiveScreenId() {
			return activeScreenController.GetActiveScreenId();
		}

		public bool ActivateScreen(int screenId) {
			return activeScreenController.ActivateScreen(screenId);
		}

		public bool DeactivateScreen(int screenId) {
			return activeScreenController.DeactivateScreen(screenId);
		}

		private void SetActive() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to activate.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int selectedScreenId = int.Parse(screensListView.SelectedItems[0].Text);
				int? currentlyActiveScreenId = GetActiveScreenId();

				if (currentlyActiveScreenId is not null) {
					if (currentlyActiveScreenId == selectedScreenId) {
						MessageBox.Show("The selected screen is already active.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
						return;
					}
					else {
						var confirmationResult = MessageBox.Show("Setting this screen as active will deactivate the currently active screen. Do you want to proceed?", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

						if (confirmationResult == DialogResult.No) {
							UpdateListView();
							return;
						}

						DeactivateScreen((int) currentlyActiveScreenId);
					}
				}

				ActivateScreen(selectedScreenId);
				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void setActiveButton_Click(object sender, EventArgs e) {
			SetActive();
		}

		private void Preview() {
			try {
				if (screensListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one screen to preview.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int screenId = int.Parse(screensListView.SelectedItems[0].Text);
				string screenTitle = screensListView.SelectedItems[0].SubItems[ScreensConstants.SCREEN_TITLE].Text;
				PreviewScreenById(screenId, screenTitle);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void previewButton_Click(object sender, EventArgs e) {
			Preview();
		}

		public List<TicketingButtonTMP> GetButtonsByScreenId(int screenId) {
			var ret = new List<TicketingButtonTMP>();

			try {
				string query = $"SELECT * FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId;";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				try {
					connection.Open();

					int buttonScreenId, buttonId;
					string buttonBankName, type, nameEn, nameAr;
					string? service, messageEn, messageAr;
					var reader = command.ExecuteReader();

					while (reader.Read()) {
						buttonBankName = reader[ButtonsConstants.BANK_NAME].ToString() ?? $"Error getting {ButtonsConstants.BANK_NAME}";
						buttonScreenId = int.Parse(reader[ButtonsConstants.SCREEN_ID].ToString() ?? "-1");
						buttonId = int.Parse(reader[ButtonsConstants.BUTTON_ID].ToString() ?? "-1");
						type = reader[ButtonsConstants.TYPE].ToString() ?? $"Error getting {ButtonsConstants.TYPE}";
						nameEn = reader[ButtonsConstants.NAME_EN].ToString() ?? $"Error getting {ButtonsConstants.NAME_EN}";
						nameAr = reader[ButtonsConstants.NAME_AR].ToString() ?? $"Error getting {ButtonsConstants.NAME_AR}";
						service = reader[ButtonsConstants.SERVICE].ToString();
						messageEn = reader[ButtonsConstants.MESSAGE_EN].ToString();
						messageAr = reader[ButtonsConstants.MESSAGE_AR].ToString();

						ret.Add(new TicketingButtonTMP(buttonBankName, buttonScreenId, buttonId, type, nameEn, nameAr, service, messageEn, messageAr));
					}
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "button ID");
				}
				finally {
					connection.Close();
				}

			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return ret;
		}

		private void PreviewScreenById(int screenId, string screenTitle) {
			try {
				if (!CheckIfScreenExists(screenId)) {
					MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var previewForm = new PreviewForm(screenTitle, GetButtonsByScreenId(screenId));
				previewForm.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void refreshButton_Click(object sender, EventArgs e) {
			UpdateListView();
		}

		private void HandleKeyEvent(KeyEventArgs e) {
			switch (e.KeyCode) {
				case Keys.Enter:
				case Keys.E:
					Edit();
					break;
				case Keys.Delete:
				case Keys.Back:
				case Keys.D:
					Delete();
					break;
				case Keys.A:
					Add();
					break;
				case Keys.S:
					SetActive();
					break;
				case Keys.P:
					Preview();
					break;
				case Keys.R:
					UpdateListView();
					break;
			}
		}

		private void BankForm_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void screensListView_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e) {
			string shortcuts = "E: Edit\nD/Del/Backspace: Delete\nA: Add\nS: Set Active\nP: Preview\nR: Refresh";

			MessageBox.Show(shortcuts, "Keyboard shortcuts", MessageBoxButtons.OK, MessageBoxIcon.Question);
		}
	}
}
