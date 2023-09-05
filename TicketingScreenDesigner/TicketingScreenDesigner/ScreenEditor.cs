#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using System.Data.SqlClient;
using System.Text;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		private const string TITLE_TEXT = "Screen Editor";
		private readonly string bankName;
		private readonly string screenTitle;
		private readonly SqlConnection connection;
		private readonly List<TicketingButton> pendingAdds;
		private readonly Dictionary<int, TicketingButton> pendingUpdates;
		private readonly List<int> pendingDeletes;
		private readonly BankForm callingForm;
		private readonly bool isNewScreen;
		private int screenId;
		private int pendingButtonId;

		private ScreenEditor() {
			InitializeComponent();
			bankName = string.Empty;
			screenId = -1;
			screenTitle = string.Empty;
			pendingAdds = new List<TicketingButton>();
			pendingUpdates = new Dictionary<int, TicketingButton>();
			pendingDeletes = new List<int>();
			pendingButtonId = -1;
		}

		public ScreenEditor(SqlConnection connection, BankForm callingForm, string bankName) : this() {
			this.connection = connection;
			this.callingForm = callingForm;
			this.bankName = bankName;
			Text = TITLE_TEXT + " - New Screen";
			isNewScreen = true;
		}

		public ScreenEditor(SqlConnection connection, BankForm callingForm, string bankName, int screenId, string screenTitle, bool isActive) : this(connection, callingForm, bankName) {
			this.screenId = screenId;
			this.screenTitle = screenTitle;
			screenTitleTextBox.Text = screenTitle;
			activeCheckBox.Checked = isActive;
			Text = TITLE_TEXT + " - " + screenTitle;
			isNewScreen = false;
			UpdateListView();
		}

		public void UpdateListView() {
			try {
				buttonsListView.Items.Clear();

				string query = $"SELECT {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.NAME_EN}, {ButtonsConstants.TYPE}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN} " +
					$"FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				try {
					connection.Open();

					var reader = command.ExecuteReader();

					while (reader.Read()) {
						int buttonId = int.Parse(reader[ButtonsConstants.BUTTON_ID].ToString() ?? "-1");
						string type = reader[ButtonsConstants.TYPE].ToString() ?? $"Error getting {ButtonsConstants.TYPE}";
						string nameEn = reader[ButtonsConstants.NAME_EN].ToString() ?? $"Error getting {ButtonsConstants.NAME_EN}";
						string? service = reader[ButtonsConstants.SERVICE].ToString();
						string? messageEn = reader[ButtonsConstants.MESSAGE_EN].ToString();

						if (!pendingUpdates.ContainsKey(buttonId))
							AddToListView(buttonId, nameEn, type, service, messageEn);
					}

					reader.Close();
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Button ID");
				}
				finally {
					connection.Close();
				}

				foreach (var button in pendingAdds) {
					AddToListView(button);
				}
				foreach (var button in pendingUpdates.Values) {
					AddToListView(button);
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void AddToListView(int buttonId, string nameEn, string type, string? service, string? messageEn) {
			try {
				if (pendingDeletes.Contains(buttonId)) {
					return;
				}

				ListViewItem row = new() {
					Name = ButtonsConstants.BUTTON_ID,
					Text = buttonId == 0 ? string.Empty : buttonId.ToString(),
				};

				ListViewItem.ListViewSubItem buttonNameEn = new() {
					Name = ButtonsConstants.NAME_EN,
					Text = nameEn
				};
				row.SubItems.Add(buttonNameEn);

				ListViewItem.ListViewSubItem buttonType = new() {
					Name = ButtonsConstants.TYPE,
					Text = type
				};
				row.SubItems.Add(buttonType);

				ListViewItem.ListViewSubItem buttonService = new() {
					Name = ButtonsConstants.SERVICE,
					Text = service ?? string.Empty
				};
				row.SubItems.Add(buttonService);

				ListViewItem.ListViewSubItem buttonMessageEn = new() {
					Name = ButtonsConstants.MESSAGE_EN,
					Text = messageEn ?? string.Empty
				};
				row.SubItems.Add(buttonMessageEn);

				buttonsListView.Items.Add(row);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void AddToListView(TicketingButton button) {
			AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
		}

		private void Add() {
			try {
				ButtonEditor buttonEditor;
				if (isNewScreen) {
					buttonEditor = new ButtonEditor(connection, this, bankName);
				}
				else {
					buttonEditor = new ButtonEditor(connection, this, bankName, screenId);
				}
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			Add();
		}

		private int InsertScreen() {
			int ret = -1;

			try {
				string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE}) VALUES (@bankName, @isActive, @screenTitle);SELECT CAST(scope_identity() AS int);";
				var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@isActive", 0);
				command.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text != string.Empty ? screenTitleTextBox.Text : DBNull.Value);

				try {
					connection.Open();

					ret = (int) command.ExecuteScalar();
				}
				catch (SqlException ex) {
					ExceptionHelper.HandleSqlException(ex, "Screen ID");
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

		private bool AddNewScreen() {
			screenId = InsertScreen();
			bool addScreenSuccess = screenId != -1;
			bool activationSuccess = addScreenSuccess && UpdateScreenActivation();
			bool addButtonsSuccess = addScreenSuccess && AddPending();
			bool updateButtonsSuccess = addScreenSuccess && UpdatePending();

			return addScreenSuccess && activationSuccess && addButtonsSuccess && updateButtonsSuccess;
		}

		private bool UpdateScreenTitle() {
			bool success = false;

			try {
				string updateScreenQuery = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_TITLE} = @screenTitle WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
				var updateScreenCommand = new SqlCommand(updateScreenQuery, connection);
				updateScreenCommand.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text);
				updateScreenCommand.Parameters.AddWithValue("@screenId", screenId);
				updateScreenCommand.Parameters.AddWithValue("@bankName", bankName);

				try {
					connection.Open();
					success = updateScreenCommand.ExecuteNonQuery() == 1;
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
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private bool UpdateScreenActivation() {
			bool success = false;

			try {
				if (activeCheckBox.Checked) {
					success = callingForm.ActivateScreen(screenId);
				}
				else {
					success = callingForm.DeactivateScreen(screenId);
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		public void AddButton(TicketingButton button) {
			try {
				pendingAdds.Add(button);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		public void UpdateButton(int buttonId, TicketingButton updatedButton) {
			try {
				bool isPendingButton = false;

				for (int i = 0; i < pendingAdds.Count; ++i) {
					if (pendingAdds[i].ButtonId == buttonId) {
						pendingAdds[i] = updatedButton;
						isPendingButton = true;
						break;
					}
				}
				if (pendingUpdates.ContainsKey(buttonId)) {
					pendingUpdates[buttonId] = updatedButton;
					isPendingButton = true;
				}

				if (!isPendingButton) {
					pendingUpdates.Add(buttonId, updatedButton);
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private string? GetButtonFieldByName(TicketingButton button, string fieldName) {
			switch (fieldName) {
				case "@type":
					return button.Type;
				case "@nameEn":
					return button.NameEn;
				case "@nameAr":
					return button.NameAr;
				case "@service":
					return button.Service;
				case "@messageEn":
					return button.MessageEn;
				case "@messageAr":
					return button.MessageAr;
				default:
					return null;
			}
		}

		private bool RemoveDeletedButtons() {
			if (pendingUpdates.Count == 0) {
				return false;
			}

			bool ret = false;

			var query = new StringBuilder($"SELECT {ButtonsConstants.BUTTON_ID} FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BUTTON_ID} IN (");
			var command = new SqlCommand();

			int i = 0;
			foreach (var buttonId in pendingUpdates.Keys) {
				query.Append("@buttonId").Append(i).Append(',');
				command.Parameters.AddWithValue("@buttonId" + i, buttonId);

				++i;
			}

			query.Length--;
			query.Append(");");

			command.CommandText = query.ToString();
			command.Connection = connection;

			try {
				connection.Open();

				var undeleted = new HashSet<int>();
				var reader = command.ExecuteReader();
				while (reader.Read()) {
					undeleted.Add(reader.GetInt32(0));
				}

				foreach (var buttonId in pendingUpdates.Keys.ToList()) {
					if (!undeleted.Contains(buttonId)) {
						pendingUpdates.Remove(buttonId);
						ret = true;
					}
				}
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return ret;
		}

		private SqlCommand? CreateUpdateButtonsCommand() {
			if (pendingUpdates.Count == 0)
				return null;

			try {
				var query = new StringBuilder($"UPDATE {ButtonsConstants.TABLE_NAME} SET ");
				var command = new SqlCommand();

				var fields = new Dictionary<string, StringBuilder>()
				{
					{"@type", new StringBuilder($"{ButtonsConstants.TYPE} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@nameEn", new StringBuilder($"{ButtonsConstants.NAME_EN} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@nameAr", new StringBuilder($"{ButtonsConstants.NAME_AR} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@service", new StringBuilder($"{ButtonsConstants.SERVICE} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@messageEn", new StringBuilder($"{ButtonsConstants.MESSAGE_EN} = CASE {ButtonsConstants.BUTTON_ID} ")},
					{"@messageAr", new StringBuilder($"{ButtonsConstants.MESSAGE_AR} = CASE {ButtonsConstants.BUTTON_ID} ")},
				};

				var buttonIds = new StringBuilder("");

				int i = 0;
				foreach (var button in pendingUpdates) {
					foreach (var field in fields) {
						field.Value.Append($" WHEN @buttonId").Append(i).Append(" THEN ").Append(field.Key).Append(i);
						string? buttonField = GetButtonFieldByName(button.Value, field.Key);
						if (buttonField is not null)
							command.Parameters.AddWithValue(field.Key + i, buttonField);
						else
							command.Parameters.AddWithValue(field.Key + i, DBNull.Value);
					}

					command.Parameters.AddWithValue("@buttonId" + i, button.Key);
					buttonIds.Append("@buttonId" + i).Append(',');

					++i;
				}

				foreach (var field in fields) {
					field.Value.Append(" ELSE @default END,");
					query.Append(field.Value);
				}

				buttonIds.Length--;
				query.Length--;

				query.Append($" WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (").Append(buttonIds).Append(");");

				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);
				command.Parameters.AddWithValue("@default", "Update error");

				command.Connection = connection;
				command.CommandText = query.ToString();

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private bool UpdatePending() {
			bool success = false;

			try {
				if (RemoveDeletedButtons()) {
					MessageBox.Show("Some of the buttons you edited were deleted by a different user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}

				var command = CreateUpdateButtonsCommand();

				success = true;

				if (command is not null) {
					success = false;

					try {
						connection.Open();
						success = command.ExecuteNonQuery() == pendingUpdates.Count;
					}
					catch (SqlException ex) {
						ExceptionHelper.HandleSqlException(ex, "Button ID");
					}
					catch (Exception ex) {
						ExceptionHelper.HandleGeneralException(ex);
					}
					finally {
						connection.Close();
					}
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private SqlCommand? CreateAddButtonsCommand() {
			if (pendingAdds.Count == 0)
				return null;

			try {
				var query = new StringBuilder($"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.TYPE}, {ButtonsConstants.NAME_EN}, " +
					$"{ButtonsConstants.NAME_AR}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES ");
				var command = new SqlCommand();

				int i = 0;
				foreach (var b in pendingAdds) {
					query.Append('(');
					query.Append("@bankName").Append(i).Append(',');
					query.Append("@screenId").Append(i).Append(',');
					query.Append("@type").Append(i).Append(',');
					query.Append("@nameEn").Append(i).Append(',');
					query.Append("@nameAr").Append(i).Append(',');
					query.Append("@service").Append(i).Append(',');
					query.Append("@messageEn").Append(i).Append(',');
					query.Append("@messageAr").Append(i);
					query.Append("),");

					command.Parameters.AddWithValue("@bankName" + i, b.BankName);
					command.Parameters.AddWithValue("@screenId" + i, screenId);
					command.Parameters.AddWithValue("@type" + i, b.Type);
					command.Parameters.AddWithValue("@nameEn" + i, b.NameEn);
					command.Parameters.AddWithValue("@nameAr" + i, b.NameAr);
					if (b.Service is not null) {
						command.Parameters.AddWithValue("@service" + i, b.Service);
					}
					else {
						command.Parameters.AddWithValue("@service" + i, DBNull.Value);
					}
					if (b.MessageEn is not null) {
						command.Parameters.AddWithValue("@messageEn" + i, b.MessageEn);
					}
					else {
						command.Parameters.AddWithValue("@messageEn" + i, DBNull.Value);
					}
					if (b.MessageAr is not null) {
						command.Parameters.AddWithValue("@messageAr" + i, b.MessageAr);
					}
					else {
						command.Parameters.AddWithValue("@messageAr" + i, DBNull.Value);
					}

					++i;
				}

				query.Length--;

				command.Connection = connection;
				command.CommandText = query.ToString();

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private bool AddPending() {
			bool success = false;

			try {
				var command = CreateAddButtonsCommand();

				success = true;

				if (command is not null) {
					success = false;

					try {
						connection.Open();
						success = command.ExecuteNonQuery() == pendingAdds.Count;
					}
					catch (SqlException ex) {
						ExceptionHelper.HandleSqlException(ex, "Button ID");
					}
					catch (Exception ex) {
						ExceptionHelper.HandleGeneralException(ex);
					}
					finally {
						connection.Close();
					}
				}

				if (success)
					pendingAdds.Clear();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private bool UpdateCurrentScreen() {
			bool informationUpdateSuccess = UpdateScreenTitle();
			bool activationUpdateSuccess = UpdateScreenActivation();
			bool addButtonsSuccess = AddPending();
			bool updateButtonsSuccess = UpdatePending();

			return informationUpdateSuccess & activationUpdateSuccess & addButtonsSuccess & updateButtonsSuccess;
		}

		private SqlCommand? CreateDeleteButtonsCommand() {
			if (pendingDeletes.Count == 0)
				return null;

			try {
				var query = new StringBuilder($"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (");
				var command = new SqlCommand();
				command.Parameters.AddWithValue("@bankName", bankName);
				command.Parameters.AddWithValue("@screenId", screenId);

				int i = 0;
				foreach (var b in pendingDeletes) {
					query.Append("@buttonId").Append(i).Append(',');
					command.Parameters.AddWithValue("@buttonId" + i, b);
					++i;
				}

				query.Length--; // remove last
				query.Append(");");

				command.Connection = connection;
				command.CommandText = query.ToString();

				return command;
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private bool DeletePending() {
			bool success = false;

			try {
				var command = CreateDeleteButtonsCommand();

				success = true;

				if (command is not null) {
					success = false;

					try {
						connection.Open();
						command.ExecuteNonQuery();
						success = true;
					}
					catch (SqlException ex) {
						ExceptionHelper.HandleSqlException(ex, "ButtonID");
					}
					catch (Exception ex) {
						ExceptionHelper.HandleGeneralException(ex);
					}
					finally {
						connection.Close();
					}
				}

				if (success)
					pendingDeletes.Clear();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return success;
		}

		private void saveButton_Click(object sender, EventArgs e) {
			try {
				screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in the screen title and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				bool success;

				if (isNewScreen) {
					success = AddNewScreen();
				}
				else {
					if (callingForm.CheckIfScreenExists(screenId)) {
						success = UpdateCurrentScreen();
					}
					else {
						MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						callingForm.UpdateListView();
						Close();
						return;
					}
				}

				success &= DeletePending();

				if (success) {
					callingForm.UpdateListView();
				}
				else {
					throw new Exception("Failed to Apply all of the changes due to an unexpected error.");
				}

				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private bool IsDataChanged() {
			return pendingAdds.Count > 0 || pendingUpdates.Count > 0 || pendingDeletes.Count > 0 || (isNewScreen && screenTitleTextBox.Text != string.Empty) || (!isNewScreen && screenTitleTextBox.Text != screenTitle);
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				pendingAdds.Clear();
				pendingUpdates.Clear();
				pendingDeletes.Clear();
				callingForm.UpdateListView();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void Delete() {
			try {
				int selectedCount = buttonsListView.SelectedItems.Count;

				if (selectedCount == 0) {
					MessageBox.Show("Select buttons to delete.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				var confirmationResult = MessageBox.Show("Are you sure you want to delete the selected button(s)? This action cannot be undone.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No) {
					return;
				}

				foreach (ListViewItem button in buttonsListView.SelectedItems) {
					bool inPendingList = false;
					int buttonId = int.Parse(button.Text);
					foreach (var pendingButton in pendingAdds) {
						if (pendingButton.ButtonId == buttonId) {
							pendingAdds.Remove(pendingButton);
							inPendingList = true;
							break;
						}
					}
					if (pendingUpdates.ContainsKey(buttonId)) {
						pendingUpdates.Remove(buttonId);
						inPendingList = true;
					}

					if (!inPendingList) {
						pendingDeletes.Add(int.Parse(button.Text));
					}
				}

				UpdateListView();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void deleteButton_Click(object sender, EventArgs e) {
			Delete();
		}

		private void UpdateFormButtonActivation() {
			try {
				int selectedCount = buttonsListView.SelectedItems.Count;

				if (selectedCount == 0) {
					editButton.Enabled = false;
					deleteButton.Enabled = false;
				}
				else if (selectedCount == 1) {
					editButton.Enabled = true;
					deleteButton.Enabled = true;
				}
				else {
					editButton.Enabled = false;
					deleteButton.Enabled = true;
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e) {
			UpdateFormButtonActivation();
		}

		public bool CheckIfButtonExists(int buttonId) {
			bool ret = false;

			string query = $"SELECT COUNT({ButtonsConstants.BUTTON_ID}) FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BUTTON_ID} = @buttonId;";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@buttonId", buttonId);

			try {
				connection.Open();

				ret = (int) command.ExecuteScalar() == 1;
			}
			catch (SqlException ex) {
				ExceptionHelper.HandleSqlException(ex, "Button ID");
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			return ret;
		}

		public void CheckIfScreenExists() {
			if (!isNewScreen && !callingForm.CheckIfScreenExists(screenId)) {
				MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				pendingAdds.Clear();
				pendingUpdates.Clear();
				pendingDeletes.Clear();
				Close();
				return;
			}
		}

		private void Edit() {
			try {
				if (buttonsListView.SelectedItems.Count != 1) {
					MessageBox.Show("Select one button to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					return;
				}

				int buttonId = int.Parse(buttonsListView.SelectedItems[0].Text);

				if (!CheckIfButtonExists(buttonId)) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var buttonEditor = new ButtonEditor(connection, this, bankName, screenId, buttonId);
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			Edit();
		}

		public TicketingButton? GetPendingButtonById(int buttonId) {
			try {
				foreach (var button in pendingAdds) {
					if (buttonId == button.ButtonId) {
						return button;
					}
				}
				if (pendingUpdates.ContainsKey(buttonId))
					return pendingUpdates[buttonId];
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}

			return null;
		}

		private bool HasButtons() {
			return buttonsListView.Items.Count > 0;
		}

		private bool IsInformationComplete() {
			return HasButtons() && screenTitleTextBox.Text != string.Empty;
		}

		private void refreshButton_Click(object sender, EventArgs e) {
			UpdateListView();
		}

		private void ToggleActive() {
			activeCheckBox.Checked = !activeCheckBox.Checked;
		}

		private void HandleKeyEvent(KeyEventArgs e) {
			switch (e.KeyCode) {
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
				case Keys.R:
					UpdateListView();
					break;
				case Keys.S:
					ToggleActive();
					break;
			}
		}

		private void ScreenEditor_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		private void buttonsListView_KeyDown(object sender, KeyEventArgs e) {
			HandleKeyEvent(e);
		}

		public int GetNextPendingButtonId() {
			--pendingButtonId;
			return pendingButtonId;
		}
	}
}
