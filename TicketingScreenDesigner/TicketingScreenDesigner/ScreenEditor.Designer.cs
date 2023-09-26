namespace TicketingScreenDesigner {
	partial class ScreenEditor {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			components = new System.ComponentModel.Container();
			saveButton = new Button();
			buttonsListView = new ListView();
			nameColumn = new ColumnHeader();
			typeColumn = new ColumnHeader();
			serviceColumn = new ColumnHeader();
			messageColumn = new ColumnHeader();
			cancelButton = new Button();
			addButton = new Button();
			deleteButton = new Button();
			editButton = new Button();
			activeCheckBox = new CheckBox();
			screenTitleLabel = new Label();
			screenTitleTextBox = new TextBox();
			refreshButton = new Button();
			toolTip1 = new ToolTip(components);
			statusLabel = new Label();
			SuspendLayout();
			// 
			// saveButton
			// 
			saveButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			saveButton.Location = new Point(597, 330);
			saveButton.Name = "saveButton";
			saveButton.Size = new Size(100, 23);
			saveButton.TabIndex = 0;
			saveButton.Text = "Save";
			toolTip1.SetToolTip(saveButton, "Save (Enter):\r\nSave your changes and close the window.");
			saveButton.UseVisualStyleBackColor = true;
			saveButton.Click += saveButton_Click;
			// 
			// buttonsListView
			// 
			buttonsListView.AllowColumnReorder = true;
			buttonsListView.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			buttonsListView.Columns.AddRange(new ColumnHeader[] { nameColumn, typeColumn, serviceColumn, messageColumn });
			buttonsListView.FullRowSelect = true;
			buttonsListView.Location = new Point(12, 39);
			buttonsListView.Name = "buttonsListView";
			buttonsListView.Size = new Size(579, 286);
			buttonsListView.TabIndex = 1;
			buttonsListView.UseCompatibleStateImageBehavior = false;
			buttonsListView.View = View.Details;
			buttonsListView.SelectedIndexChanged += buttonsListView_SelectedIndexChanged;
			buttonsListView.KeyDown += buttonsListView_KeyDown;
			// 
			// nameColumn
			// 
			nameColumn.Text = "Name";
			nameColumn.Width = 120;
			// 
			// typeColumn
			// 
			typeColumn.Text = "Type";
			typeColumn.Width = 120;
			// 
			// serviceColumn
			// 
			serviceColumn.Text = "Service";
			serviceColumn.Width = 140;
			// 
			// messageColumn
			// 
			messageColumn.Text = "Message";
			messageColumn.Width = 140;
			// 
			// cancelButton
			// 
			cancelButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			cancelButton.Location = new Point(492, 330);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(100, 23);
			cancelButton.TabIndex = 2;
			cancelButton.Text = "Cancel";
			toolTip1.SetToolTip(cancelButton, "Cancel (Esc):\r\nDiscard your changes and close the window.");
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// addButton
			// 
			addButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			addButton.Location = new Point(597, 38);
			addButton.Name = "addButton";
			addButton.Size = new Size(100, 23);
			addButton.TabIndex = 3;
			addButton.Text = "Add Button";
			toolTip1.SetToolTip(addButton, "Add (A):\r\nAdd a new button to the screen.");
			addButton.UseVisualStyleBackColor = true;
			addButton.Click += addButton_Click;
			// 
			// deleteButton
			// 
			deleteButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			deleteButton.Enabled = false;
			deleteButton.Location = new Point(597, 92);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new Size(100, 23);
			deleteButton.TabIndex = 4;
			deleteButton.Text = "Delete Button";
			toolTip1.SetToolTip(deleteButton, "Delete (D/Del):\r\nDelete the currently selected button(s).");
			deleteButton.UseVisualStyleBackColor = true;
			deleteButton.Click += deleteButton_Click;
			// 
			// editButton
			// 
			editButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			editButton.Enabled = false;
			editButton.Location = new Point(597, 65);
			editButton.Name = "editButton";
			editButton.Size = new Size(100, 23);
			editButton.TabIndex = 5;
			editButton.Text = "Edit Button";
			toolTip1.SetToolTip(editButton, "Edit (E):\r\nEdit the currently selected button.");
			editButton.UseVisualStyleBackColor = true;
			editButton.Click += editButton_Click;
			// 
			// activeCheckBox
			// 
			activeCheckBox.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			activeCheckBox.AutoSize = true;
			activeCheckBox.Location = new Point(598, 310);
			activeCheckBox.Name = "activeCheckBox";
			activeCheckBox.Size = new Size(59, 19);
			activeCheckBox.TabIndex = 6;
			activeCheckBox.Text = "Active";
			toolTip1.SetToolTip(activeCheckBox, "Active (S):\r\nSelect to set the screen as active when you save.");
			activeCheckBox.UseVisualStyleBackColor = true;
			// 
			// screenTitleLabel
			// 
			screenTitleLabel.AutoSize = true;
			screenTitleLabel.Location = new Point(12, 14);
			screenTitleLabel.Name = "screenTitleLabel";
			screenTitleLabel.Size = new Size(70, 15);
			screenTitleLabel.TabIndex = 10;
			screenTitleLabel.Text = "Screen Title:";
			toolTip1.SetToolTip(screenTitleLabel, "Screen Title:\r\nThis is the title that will be shown to your customers\r\non the screen. If left blank, the screen ID will be shown\r\ninstead.");
			// 
			// screenTitleTextBox
			// 
			screenTitleTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			screenTitleTextBox.Location = new Point(88, 10);
			screenTitleTextBox.Name = "screenTitleTextBox";
			screenTitleTextBox.Size = new Size(609, 23);
			screenTitleTextBox.TabIndex = 9;
			// 
			// refreshButton
			// 
			refreshButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			refreshButton.Location = new Point(597, 119);
			refreshButton.Name = "refreshButton";
			refreshButton.Size = new Size(100, 23);
			refreshButton.TabIndex = 13;
			refreshButton.Text = "Refresh List";
			toolTip1.SetToolTip(refreshButton, "Refresh (R):\r\nRefresh the button list view.");
			refreshButton.UseVisualStyleBackColor = true;
			refreshButton.Click += refreshButton_Click;
			// 
			// statusLabel
			// 
			statusLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
			statusLabel.AutoSize = true;
			statusLabel.Location = new Point(9, 334);
			statusLabel.Name = "statusLabel";
			statusLabel.Size = new Size(0, 15);
			statusLabel.TabIndex = 14;
			// 
			// ScreenEditor
			// 
			AcceptButton = saveButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(708, 362);
			ControlBox = false;
			Controls.Add(statusLabel);
			Controls.Add(refreshButton);
			Controls.Add(screenTitleLabel);
			Controls.Add(screenTitleTextBox);
			Controls.Add(activeCheckBox);
			Controls.Add(editButton);
			Controls.Add(deleteButton);
			Controls.Add(addButton);
			Controls.Add(cancelButton);
			Controls.Add(buttonsListView);
			Controls.Add(saveButton);
			MinimumSize = new Size(724, 378);
			Name = "ScreenEditor";
			StartPosition = FormStartPosition.CenterParent;
			Text = "Screen Editor";
			FormClosed += ScreenEditor_FormClosed;
			KeyDown += ScreenEditor_KeyDown;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private Button saveButton;
		private ListView buttonsListView;
		private Button cancelButton;
		private Button addButton;
		private Button deleteButton;
		private Button editButton;
		private CheckBox activeCheckBox;
		private Label screenTitleLabel;
		private TextBox screenTitleTextBox;
		private ColumnHeader nameColumn;
		private ColumnHeader typeColumn;
		private ColumnHeader serviceColumn;
		private ColumnHeader messageColumn;
		private Button refreshButton;
		private ToolTip toolTip1;
		private Label statusLabel;
	}
}