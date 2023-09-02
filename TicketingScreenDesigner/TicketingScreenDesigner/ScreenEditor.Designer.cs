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
			saveButton = new Button();
			buttonsListView = new ListView();
			idColumn = new ColumnHeader();
			nameColumn = new ColumnHeader();
			typeColumn = new ColumnHeader();
			serviceColumn = new ColumnHeader();
			messageColumn = new ColumnHeader();
			cancelButton = new Button();
			addButton = new Button();
			deleteButton = new Button();
			editButton = new Button();
			activeCheckBox = new CheckBox();
			screenIdTextBox = new TextBox();
			screenIdLabel = new Label();
			screenTitleLabel = new Label();
			screenTitleTextBox = new TextBox();
			autoFillIdButton = new Button();
			refreshButton = new Button();
			SuspendLayout();
			// 
			// saveButton
			// 
			saveButton.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			saveButton.Location = new Point(622, 339);
			saveButton.Name = "saveButton";
			saveButton.Size = new Size(75, 23);
			saveButton.TabIndex = 0;
			saveButton.Text = "&Save";
			saveButton.UseVisualStyleBackColor = true;
			saveButton.Click += saveButton_Click;
			// 
			// buttonsListView
			// 
			buttonsListView.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			buttonsListView.Columns.AddRange(new ColumnHeader[] { idColumn, nameColumn, typeColumn, serviceColumn, messageColumn });
			buttonsListView.FullRowSelect = true;
			buttonsListView.Location = new Point(12, 71);
			buttonsListView.Name = "buttonsListView";
			buttonsListView.Size = new Size(604, 261);
			buttonsListView.TabIndex = 1;
			buttonsListView.UseCompatibleStateImageBehavior = false;
			buttonsListView.View = View.Details;
			buttonsListView.SelectedIndexChanged += buttonsListView_SelectedIndexChanged;
			buttonsListView.KeyDown += buttonsListView_KeyDown;
			// 
			// idColumn
			// 
			idColumn.Text = "Button ID";
			idColumn.Width = 80;
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
			cancelButton.Location = new Point(542, 339);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 23);
			cancelButton.TabIndex = 2;
			cancelButton.Text = "&Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// addButton
			// 
			addButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			addButton.Location = new Point(622, 71);
			addButton.Name = "addButton";
			addButton.Size = new Size(75, 23);
			addButton.TabIndex = 3;
			addButton.Text = "A&dd";
			addButton.UseVisualStyleBackColor = true;
			addButton.Click += addButton_Click;
			// 
			// deleteButton
			// 
			deleteButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			deleteButton.Enabled = false;
			deleteButton.Location = new Point(622, 129);
			deleteButton.Name = "deleteButton";
			deleteButton.Size = new Size(75, 23);
			deleteButton.TabIndex = 4;
			deleteButton.Text = "De&lete";
			deleteButton.UseVisualStyleBackColor = true;
			deleteButton.Click += deleteButton_Click;
			// 
			// editButton
			// 
			editButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			editButton.Enabled = false;
			editButton.Location = new Point(622, 100);
			editButton.Name = "editButton";
			editButton.Size = new Size(75, 23);
			editButton.TabIndex = 5;
			editButton.Text = "&Edit";
			editButton.UseVisualStyleBackColor = true;
			editButton.Click += editButton_Click;
			// 
			// activeCheckBox
			// 
			activeCheckBox.Anchor =  AnchorStyles.Bottom | AnchorStyles.Right;
			activeCheckBox.AutoSize = true;
			activeCheckBox.Location = new Point(623, 317);
			activeCheckBox.Name = "activeCheckBox";
			activeCheckBox.Size = new Size(59, 19);
			activeCheckBox.TabIndex = 6;
			activeCheckBox.Text = "&Active";
			activeCheckBox.UseVisualStyleBackColor = true;
			// 
			// screenIdTextBox
			// 
			screenIdTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			screenIdTextBox.Location = new Point(88, 13);
			screenIdTextBox.Name = "screenIdTextBox";
			screenIdTextBox.Size = new Size(529, 23);
			screenIdTextBox.TabIndex = 7;
			// 
			// screenIdLabel
			// 
			screenIdLabel.AutoSize = true;
			screenIdLabel.Location = new Point(23, 17);
			screenIdLabel.Name = "screenIdLabel";
			screenIdLabel.Size = new Size(59, 15);
			screenIdLabel.TabIndex = 8;
			screenIdLabel.Text = "Screen ID:";
			// 
			// screenTitleLabel
			// 
			screenTitleLabel.AutoSize = true;
			screenTitleLabel.Location = new Point(12, 46);
			screenTitleLabel.Name = "screenTitleLabel";
			screenTitleLabel.Size = new Size(70, 15);
			screenTitleLabel.TabIndex = 10;
			screenTitleLabel.Text = "Screen Title:";
			// 
			// screenTitleTextBox
			// 
			screenTitleTextBox.Anchor =  AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			screenTitleTextBox.Location = new Point(88, 42);
			screenTitleTextBox.Name = "screenTitleTextBox";
			screenTitleTextBox.Size = new Size(609, 23);
			screenTitleTextBox.TabIndex = 9;
			// 
			// autoFillIdButton
			// 
			autoFillIdButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			autoFillIdButton.Location = new Point(623, 13);
			autoFillIdButton.Name = "autoFillIdButton";
			autoFillIdButton.Size = new Size(75, 23);
			autoFillIdButton.TabIndex = 12;
			autoFillIdButton.Text = "Auto&Fill";
			autoFillIdButton.UseVisualStyleBackColor = true;
			autoFillIdButton.Click += autoFillIdButton_Click;
			// 
			// refreshButton
			// 
			refreshButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			refreshButton.Location = new Point(622, 158);
			refreshButton.Name = "refreshButton";
			refreshButton.Size = new Size(75, 23);
			refreshButton.TabIndex = 13;
			refreshButton.Text = "&Refresh";
			refreshButton.UseVisualStyleBackColor = true;
			refreshButton.Click += refreshButton_Click;
			// 
			// ScreenEditor
			// 
			AcceptButton = saveButton;
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			CancelButton = cancelButton;
			ClientSize = new Size(708, 372);
			Controls.Add(refreshButton);
			Controls.Add(autoFillIdButton);
			Controls.Add(screenTitleLabel);
			Controls.Add(screenTitleTextBox);
			Controls.Add(screenIdLabel);
			Controls.Add(screenIdTextBox);
			Controls.Add(activeCheckBox);
			Controls.Add(editButton);
			Controls.Add(deleteButton);
			Controls.Add(addButton);
			Controls.Add(cancelButton);
			Controls.Add(buttonsListView);
			Controls.Add(saveButton);
			MinimumSize = new Size(724, 411);
			Name = "ScreenEditor";
			Text = "Screen Editor";
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
		private TextBox screenIdTextBox;
		private Label screenIdLabel;
		private Label screenTitleLabel;
		private TextBox screenTitleTextBox;
		private ColumnHeader idColumn;
		private ColumnHeader nameColumn;
		private ColumnHeader typeColumn;
		private ColumnHeader serviceColumn;
		private ColumnHeader messageColumn;
		private Button autoFillIdButton;
		private Button refreshButton;
	}
}