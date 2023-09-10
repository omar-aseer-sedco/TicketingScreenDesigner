#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using BusinessLogicLayer;
using DataAccessLayer.Constants;
using DataAccessLayer.DataClasses;

namespace TicketingScreenDesigner {
	public partial class ScreenEditor : Form {
		private const string TITLE_TEXT = "Screen Editor";

		private readonly bool isNewScreen;
		private readonly BankForm callingForm;
		private readonly BankController bankController;
		private readonly ScreenController screenController;
		private List<TicketingButton> buttons;

		private ScreenEditor() {
			InitializeComponent();
			buttons = new List<TicketingButton>();
		}

		public ScreenEditor(BankForm callingForm, BankController bankController) : this() {
			isNewScreen = true;
			Text = TITLE_TEXT + " - New Screen";
			this.callingForm = callingForm;
			this.bankController = bankController;
			screenController = new ScreenController(bankController.BankName);
		}

		public ScreenEditor(BankForm callingForm, BankController bankController, TicketingScreen screen) : this(callingForm, bankController) {
			isNewScreen = false;
			Text = TITLE_TEXT + " - " + screen.ScreenTitle;
			screenController = new ScreenController(bankController.BankName, screen.ScreenId, screen.ScreenTitle);
			FillInfo(screen);
			UpdateListView();
		}

		private void FillInfo(TicketingScreen screen) {
			screenTitleTextBox.Text = screen.ScreenTitle;
			activeCheckBox.Checked = screen.IsActive;
		}

		private void SyncButtons() {
			buttons = screenController.GetAllButtons();
		}

		public void UpdateListView() {
			try {
				SyncButtons();

				buttonsListView.Items.Clear();

				foreach (var button in buttons) {
					AddToListView(button);
				}
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void AddToListView(TicketingButton button) {
			try {
				ListViewItem row = new() {
					Name = ButtonsConstants.NAME_EN,
					Text = button.NameEn,
				};

				ListViewItem.ListViewSubItem buttonType = new() {
					Name = ButtonsConstants.TYPE,
					Text = button.Type
				};
				row.SubItems.Add(buttonType);

				ListViewItem.ListViewSubItem buttonService = new() {
					Name = ButtonsConstants.SERVICE,
					Text = button is IssueTicketButton issueTicketButton ? issueTicketButton.Service : string.Empty
				};
				row.SubItems.Add(buttonService);

				ListViewItem.ListViewSubItem buttonMessageEn = new() {
					Name = ButtonsConstants.MESSAGE_EN,
					Text = button is ShowMessageButton showMessageButton ? showMessageButton.MessageEn : string.Empty
				};
				row.SubItems.Add(buttonMessageEn);

				row.Tag = button;

				buttonsListView.Items.Add(row);
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void Add() {
			try {
				var buttonEditor = new ButtonEditor(this, screenController);
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void addButton_Click(object sender, EventArgs e) {
			Add();
		}

		private void AddNewScreen() {
			screenController.ScreenId = bankController.AddScreen(new TicketingScreen() {
				BankName = bankController.BankName,
				ScreenTitle = screenTitleTextBox.Text,
				IsActive = activeCheckBox.Checked,
			});
		}

		private void UpdateCurrentScreen() {
			bankController.UpdateScreen(screenController.ScreenId, new TicketingScreen() {
				BankName = bankController.BankName,
				ScreenTitle = screenTitleTextBox.Text,
				IsActive = activeCheckBox.Checked,
			});
		}

		private void saveButton_Click(object sender, EventArgs e) {
			try {
				TrimInput();

				if (!IsInformationComplete()) {
					MessageBox.Show("Please fill in the screen title and add at least one button before saving the screen.", "Incomplete information", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (isNewScreen) {
					AddNewScreen();
				}
				else {
					if (bankController.CheckIfScreenExists(screenController.ScreenId)) {
						UpdateCurrentScreen();
					}
					else {
						MessageBox.Show("This screen no longer exists. It may have been deleted by another user.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
						callingForm.UpdateListView();
						Close();
						return;
					}
				}

				screenController.CommitPendingChanges();
				callingForm.UpdateListView();

				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private bool IsDataChanged() {
			TrimInput();
			return screenController.GetPendingChangeCount() != 0 || (isNewScreen && screenTitleTextBox.Text != string.Empty) || (!isNewScreen && screenTitleTextBox.Text != screenController.ScreenTitle);
		}

		private void cancelButton_Click(object sender, EventArgs e) {
			try {
				if (IsDataChanged()) {
					var confirmationResult = MessageBox.Show("Are you sure you want to quit? You will lose any unsaved changes.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (confirmationResult == DialogResult.No) {
						return;
					}
				}

				MessageBox.Show("tf");

				screenController.CancelPendingChanges();
				callingForm.UpdateListView();
				Close();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private List<int> GetSelectedButtonIds() {
			var selectedButtonIds = new List<int>();
			foreach (ListViewItem button in buttonsListView.SelectedItems)
				selectedButtonIds.Add(((TicketingButton) button.Tag).ButtonId);

			return selectedButtonIds;
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

				screenController.DeleteButtonsCancellable(GetSelectedButtonIds());

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

		public void CheckIfScreenExists() {
			if (!isNewScreen && !bankController.CheckIfScreenExists(screenController.ScreenId)) {
				MessageBox.Show("This screen no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
				screenController.CancelPendingChanges();
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

				TicketingButton button = (TicketingButton) buttonsListView.SelectedItems[0].Tag;

				if (!screenController.CheckIfButtonExists(button.ButtonId)) {
					MessageBox.Show("This button no longer exists. It may have been deleted by a different user.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information);
					UpdateListView();
					return;
				}

				var buttonEditor = new ButtonEditor(this, screenController, button);
				buttonEditor.ShowDialog();
			}
			catch (Exception ex) {
				ExceptionHelper.HandleGeneralException(ex);
			}
		}

		private void editButton_Click(object sender, EventArgs e) {
			Edit();
		}

		private bool IsInformationComplete() {
			TrimInput();
			return buttonsListView.Items.Count > 0 && screenTitleTextBox.Text != string.Empty;
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

		private void TrimInput() {
			screenTitleTextBox.Text = screenTitleTextBox.Text.Trim();
		}
	}
}
