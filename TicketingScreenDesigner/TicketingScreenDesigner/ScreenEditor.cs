using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		private const string TITLE_TEXT = "Screen Editor";
		private readonly string bankName;
		private readonly string screenId;
		private readonly SqlConnection connection;
		private readonly List<TicketingButton> pendingAdds;
		private readonly Dictionary<string, TicketingButton> pendingUpdates;
		private readonly List<string> deleteList;
		private readonly BankForm callingForm;
		private readonly bool newScreen;

		private ScreenEditor() {
			InitializeComponent();
			bankName = string.Empty;
			screenId = string.Empty;
			connection = DBUtils.CreateConnection();
			pendingAdds = new List<TicketingButton>();
			pendingUpdates = new Dictionary<string, TicketingButton>();
			deleteList = new List<string>();
			callingForm = new BankForm(bankName);
		}

		public ScreenEditor(BankForm callingForm, string bankName) : this() {
			this.callingForm = callingForm;
			this.bankName = bankName;
			Text = TITLE_TEXT + " - New Screen";
			newScreen = true;
		}

		public ScreenEditor(BankForm callingForm, string bankName, string screenId, string screenTitle, bool isActive) : this(callingForm, bankName) {
			this.screenId = screenId;
			screenIdTextBox.Text = screenId;
			screenTitleTextBox.Text = screenTitle;
			activeCheckBox.Checked = isActive;
			Text = TITLE_TEXT + " - " + screenId;
			newScreen = false;
			UpdateListView();
		}

		public void UpdateListView() {
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
					string buttonId = reader[ButtonsConstants.BUTTON_ID].ToString() ?? $"Error getting {ButtonsConstants.BUTTON_ID}";
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
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
			finally {
				connection.Close();
			}

			foreach (var button in pendingAdds) {
				AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
			}
			foreach (var button in pendingUpdates.Values) {
				AddToListView(button.ButtonId, button.NameEn, button.Type, button.Service, button.MessageEn);
			}
		}

		private void AddToListView(string buttonId, string nameEn, string type, string? service, string? messageEn) {
			if (deleteList.Contains(buttonId)) {
				return;
			}

			ListViewItem row = new() {
				Name = ButtonsConstants.BUTTON_ID,
				Text = buttonId
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

		private void addButton_Click(object sender, EventArgs e) {
			TrimInput();
			var buttonEditor = new ButtonEditor(this, bankName, screenIdTextBox.Text);
			buttonEditor.Show();
		}

		private bool AddScreen() {
			TrimInput();

			bool success = false;

			string query = $"INSERT INTO {ScreensConstants.TABLE_NAME} ({ScreensConstants.BANK_NAME}, {ScreensConstants.SCREEN_ID}, {ScreensConstants.IS_ACTIVE}, {ScreensConstants.SCREEN_TITLE}) VALUES (@bankName, @screenId, @isActive, @screenTitle);";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenIdTextBox.Text);
			command.Parameters.AddWithValue("@isActive", 0);
			command.Parameters.AddWithValue("@screenTitle", screenTitleTextBox.Text != string.Empty ? screenTitleTextBox.Text : DBNull.Value);

			try {
				connection.Open();

				success = command.ExecuteNonQuery() == 1;
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

			return success;
		}

		private bool AddNewScreen() {
			bool addScreenSuccess = AddScreen();
			bool activationSuccess = addScreenSuccess && UpdateScreenActivation();
			bool addButtonsSuccess = addScreenSuccess && AddPending();
			bool updateButtonsSuccess = addScreenSuccess && UpdatePending();

			return addScreenSuccess && activationSuccess && addButtonsSuccess && updateButtonsSuccess;
		}

		private bool UpdateScreenInformation() {
			TrimInput();

			bool success = false;

			string updateScreenQuery = $"UPDATE {ScreensConstants.TABLE_NAME} SET {ScreensConstants.SCREEN_ID} = @newScreenId, {ScreensConstants.SCREEN_TITLE} = @screenTitle " +
				$"WHERE {ScreensConstants.SCREEN_ID} = @screenId AND {ScreensConstants.BANK_NAME} = @bankName";
			var updateScreenCommand = new SqlCommand(updateScreenQuery, connection);
			updateScreenCommand.Parameters.AddWithValue("@newScreenId", screenIdTextBox.Text);
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

			return success;
		}

		private bool UpdateScreenActivation() {
			TrimInput();

			bool success;

			if (activeCheckBox.Checked) {
				success = callingForm.ActivateScreen(screenIdTextBox.Text);
			}
			else {
				success = callingForm.DeactivateScreen(screenIdTextBox.Text);
			}

			return success;
		}

		public void AddButton(TicketingButton button) {
			pendingAdds.Add(button);
		}

		public void UpdateButton(string buttonId, TicketingButton updatedButton) {
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
			}

			if (!isPendingButton) {
				pendingUpdates.Add(buttonId, updatedButton);
			}
		}

		private string? GetButtonFieldByName(TicketingButton button, string fieldName) {
			switch (fieldName) {
				case "@newButtonId":
					return button.ButtonId;
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

		private SqlCommand? CreateUpdateButtonsCommand() {
			if (pendingUpdates.Count == 0)
				return null;

			var query = new StringBuilder($"UPDATE {ButtonsConstants.TABLE_NAME} SET ");
			var command = new SqlCommand();

			var fields = new Dictionary<string, StringBuilder>()
			{
				{"@newButtonId", new StringBuilder($"{ButtonsConstants.BUTTON_ID} = CASE {ButtonsConstants.BUTTON_ID} ")},
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

		private bool UpdatePending() {
			bool success = true;
			var command = CreateUpdateButtonsCommand();

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

			return success;
		}

		private void UnifyScreenId() {
			foreach (var button in pendingAdds) {
				button.ScreenId = screenIdTextBox.Text;
			}
			foreach (var button in pendingUpdates) {
				button.Value.ScreenId = screenIdTextBox.Text;
			}
		}

		private SqlCommand? CreateAddButtonsCommand() {
			if (pendingAdds.Count == 0)
				return null;

			var query = new StringBuilder($"INSERT INTO {ButtonsConstants.TABLE_NAME} ({ButtonsConstants.BANK_NAME}, {ButtonsConstants.SCREEN_ID}, {ButtonsConstants.BUTTON_ID}, {ButtonsConstants.TYPE}, {ButtonsConstants.NAME_EN}, " +
				$"{ButtonsConstants.NAME_AR}, {ButtonsConstants.SERVICE}, {ButtonsConstants.MESSAGE_EN}, {ButtonsConstants.MESSAGE_AR}) VALUES ");
			var command = new SqlCommand();

			int i = 0;
			foreach (var b in pendingAdds) {
				query.Append('(');
				query.Append("@bankName").Append(i).Append(',');
				query.Append("@screenId").Append(i).Append(',');
				query.Append("@buttonId").Append(i).Append(',');
				query.Append("@type").Append(i).Append(',');
				query.Append("@nameEn").Append(i).Append(',');
				query.Append("@nameAr").Append(i).Append(',');
				query.Append("@service").Append(i).Append(',');
				query.Append("@messageEn").Append(i).Append(',');
				query.Append("@messageAr").Append(i);
				query.Append("),");

				command.Parameters.Add("@bankName" + i, SqlDbType.VarChar).Value = b.BankName;
				command.Parameters.Add("@screenId" + i, SqlDbType.VarChar).Value = b.ScreenId;
				command.Parameters.Add("@buttonId" + i, SqlDbType.VarChar).Value = b.ButtonId;
				command.Parameters.Add("@type" + i, SqlDbType.VarChar).Value = b.Type;
				command.Parameters.Add("@nameEn" + i, SqlDbType.VarChar).Value = b.NameEn;
				command.Parameters.Add("@nameAr" + i, SqlDbType.NVarChar).Value = b.NameAr;
				if (b.Service is not null) {
					command.Parameters.Add("@service" + i, SqlDbType.VarChar).Value = b.Service;
				}
				else {
					command.Parameters.Add("@service" + i, SqlDbType.VarChar).Value = DBNull.Value;
				}
				if (b.MessageEn is not null) {
					command.Parameters.Add("@messageEn" + i, SqlDbType.VarChar).Value = b.MessageEn;
				}
				else {
					command.Parameters.Add("@messageEn" + i, SqlDbType.VarChar).Value = DBNull.Value;
				}
				if (b.MessageAr is not null) {
					command.Parameters.Add("@messageAr" + i, SqlDbType.NVarChar).Value = b.MessageAr;
				}
				else {
					command.Parameters.Add("@messageAr" + i, SqlDbType.VarChar).Value = DBNull.Value;
				}

				++i;
			}

			query.Length--; // remove last

			command.Connection = connection;
			command.CommandText = query.ToString();

			return command;
		}

		private bool AddPending() {
			bool success = true;
			var command = CreateAddButtonsCommand();

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

			return success;
		}

		private bool UpdateCurrentScreen() {
			bool informationUpdateSuccess = UpdateScreenInformation();
			bool activationUpdateSuccess = UpdateScreenActivation();
			bool addButtonsSuccess = AddPending();
			bool updateButtonsSuccess = UpdatePending();

			return informationUpdateSuccess & activationUpdateSuccess & addButtonsSuccess & updateButtonsSuccess;
		}

		private SqlCommand? CreateDeleteButtonsCommand() {
			if (deleteList.Count == 0)
				return null;

			var query = new StringBuilder($"DELETE FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} IN (");
			var command = new SqlCommand();
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenId);

			int i = 0;
			foreach (var b in deleteList) {
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

		private bool DeletePending() {
			bool success = true;
			var command = CreateDeleteButtonsCommand();

			if (command is not null) {
				success = false;

				try {
					connection.Open();
					success = command.ExecuteNonQuery() == deleteList.Count;
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
				deleteList.Clear();

			return success;
		}

		private void TrimInput() {
			screenIdTextBox.Text = screenIdTextBox.Text.Trim();
		}

		private void saveButton_Click(object sender, EventArgs e) {
			TrimInput();

			if (!IsInformationComplete()) {
				MessageBox.Show("Please fill in the screen ID and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			bool success;

			UnifyScreenId();

			if (newScreen) {
				success = AddNewScreen();
			}
			else {
				success = UpdateCurrentScreen();
			}

			success &= DeletePending();

			if (success) {
				callingForm.UpdateListView();
				Close();
			}
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			if (pendingAdds.Count > 0) {
				var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

				if (confirmationResult == DialogResult.No) {
					return;
				}
			}

			pendingAdds.Clear();
			Close();
		}

		private void deleteButton_Click(object sender, EventArgs e) {
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
				foreach (var pendingButton in pendingAdds) {
					if (pendingButton.ButtonId == button.Text) {
						pendingAdds.Remove(pendingButton);
						inPendingList = true;
						break;
					}
				}

				if (!inPendingList) {
					deleteList.Add(button.Text);
				}
			}

			UpdateListView();
		}

		private void buttonsListView_SelectedIndexChanged(object sender, EventArgs e) {
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

		private void editButton_Click(object sender, EventArgs e) {
			if (buttonsListView.SelectedItems.Count != 1) {
				MessageBox.Show("Select one button to edit.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			TrimInput();

			var buttonEditor = new ButtonEditor(this, bankName, screenIdTextBox.Text, buttonsListView.SelectedItems[0].Text);
			buttonEditor.Show();
		}

		public TicketingButton? GetPendingButtonById(string buttonId) {
			foreach (var button in pendingAdds) {
				if (buttonId == button.ButtonId) {
					return button;
				}
			}
			if (pendingUpdates.ContainsKey(buttonId))
				return pendingUpdates[buttonId];

			return null;
		}

		private bool HasButtons() {
			return buttonsListView.Items.Count > 0;
		}

		private bool IsInformationComplete() {
			TrimInput();

			return HasButtons() && screenIdTextBox.Text != string.Empty;
		}

		private void previewButton_Click(object sender, EventArgs e) {
			MessageBox.Show("This doesn't do anything yet.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void autoFillIdButton_Click(object sender, EventArgs e) {
			string query = $"SELECT {ScreensConstants.SCREEN_ID} FROM {ScreensConstants.TABLE_NAME} WHERE {ScreensConstants.BANK_NAME} = @bankName AND {ScreensConstants.SCREEN_ID} LIKE @screenIdPattern";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenIdPattern", "Screen %");

			try {
				connection.Open();

				int mx = 0;
				var reader = command.ExecuteReader();

				while (reader.Read()) {
					string id = reader.GetString(0);

					if (int.TryParse(id[7..], out int number)) {
						mx = Math.Max(mx, number);
					}
				}

				screenIdTextBox.Text = $"Screen {mx + 1}";
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

		public int GetLastAutoFilledButtonIndex() {
			int mx = 0;
			Regex pattern = new Regex(@"Button \d+");

			foreach (var button in pendingAdds) {
				if (pattern.IsMatch(button.ButtonId) && int.TryParse(button.ButtonId[7..], out int number)) {
					mx = Math.Max(mx, number);
				}
			}
			foreach (var button in pendingUpdates.Values) {
				if (pattern.IsMatch(button.ButtonId) && int.TryParse(button.ButtonId[7..], out int number)) {
					mx = Math.Max(mx, number);
				}
			}

			return mx;
		}

		public bool CheckIfExists(string buttonId) {
			string query = $"SELECT {ButtonsConstants.BUTTON_ID} FROM {ButtonsConstants.TABLE_NAME} WHERE {ButtonsConstants.BANK_NAME} = @bankName AND {ButtonsConstants.SCREEN_ID} = @screenId AND {ButtonsConstants.BUTTON_ID} = @buttonId;";
			var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@bankName", bankName);
			command.Parameters.AddWithValue("@screenId", screenIdTextBox.Text);
			command.Parameters.AddWithValue("@buttonId", buttonId);

			bool ret = false;

			try {
				connection.Open();

				ret = command.ExecuteReader().HasRows;
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

			foreach (var button in pendingAdds) {
				ret |= button.ButtonId == buttonId;
			}
			foreach (var button in pendingUpdates.Values) {
				ret |= button.ButtonId == buttonId;
			}

			return ret;
		}
	}
}
