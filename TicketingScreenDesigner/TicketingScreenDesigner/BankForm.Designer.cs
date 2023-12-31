﻿namespace TicketingScreenDesigner {
	partial class BankForm {
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
			screensListView = new ListView();
			screenTitleColumn = new ColumnHeader();
			isActiveColumn = new ColumnHeader();
			addScreenButton = new Button();
			editScreenButton = new Button();
			deleteScreenButton = new Button();
			titleLabel = new Label();
			setActiveButton = new Button();
			previewButton = new Button();
			refreshButton = new Button();
			menuStrip1 = new MenuStrip();
			accountToolStripMenuItem = new ToolStripMenuItem();
			changePasswordToolStripMenuItem = new ToolStripMenuItem();
			logOutToolStripMenuItem = new ToolStripMenuItem();
			helpToolStripMenuItem = new ToolStripMenuItem();
			keyboardShortcutsToolStripMenuItem = new ToolStripMenuItem();
			contextMenuStrip1 = new ContextMenuStrip(components);
			toolTip1 = new ToolTip(components);
			statusLabel = new Label();
			menuStrip1.SuspendLayout();
			SuspendLayout();
			// 
			// screensListView
			// 
			screensListView.AllowColumnReorder = true;
			screensListView.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			screensListView.Columns.AddRange(new ColumnHeader[] { screenTitleColumn, isActiveColumn });
			screensListView.FullRowSelect = true;
			screensListView.Location = new Point(12, 77);
			screensListView.Name = "screensListView";
			screensListView.Size = new Size(515, 338);
			screensListView.TabIndex = 0;
			screensListView.UseCompatibleStateImageBehavior = false;
			screensListView.View = View.Details;
			screensListView.SelectedIndexChanged += screensListView_SelectedIndexChanged;
			screensListView.KeyDown += screensListView_KeyDown;
			// 
			// screenTitleColumn
			// 
			screenTitleColumn.Text = "Screen Title";
			screenTitleColumn.Width = 430;
			// 
			// isActiveColumn
			// 
			isActiveColumn.Text = "Active?";
			isActiveColumn.Width = 80;
			// 
			// addScreenButton
			// 
			addScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			addScreenButton.Location = new Point(534, 77);
			addScreenButton.Name = "addScreenButton";
			addScreenButton.Size = new Size(108, 23);
			addScreenButton.TabIndex = 1;
			addScreenButton.Text = "Add Screen";
			toolTip1.SetToolTip(addScreenButton, "Add (A)\r\nAdd a new screen.");
			addScreenButton.UseVisualStyleBackColor = true;
			addScreenButton.Click += addScreenButton_Click;
			// 
			// editScreenButton
			// 
			editScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			editScreenButton.Enabled = false;
			editScreenButton.Location = new Point(534, 104);
			editScreenButton.Name = "editScreenButton";
			editScreenButton.Size = new Size(108, 23);
			editScreenButton.TabIndex = 2;
			editScreenButton.Text = "Edit Screen";
			toolTip1.SetToolTip(editScreenButton, "Edit (E)\r\nEdit the currently selected screen.");
			editScreenButton.UseVisualStyleBackColor = true;
			editScreenButton.Click += editScreenButton_Click;
			// 
			// deleteScreenButton
			// 
			deleteScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			deleteScreenButton.Enabled = false;
			deleteScreenButton.Location = new Point(534, 131);
			deleteScreenButton.Name = "deleteScreenButton";
			deleteScreenButton.Size = new Size(108, 23);
			deleteScreenButton.TabIndex = 3;
			deleteScreenButton.Text = "Delete Screen";
			toolTip1.SetToolTip(deleteScreenButton, "Delete (D/Del):\r\nDelete the currently selected screen(s).");
			deleteScreenButton.UseVisualStyleBackColor = true;
			deleteScreenButton.Click += deleteScreenButton_Click;
			// 
			// titleLabel
			// 
			titleLabel.Anchor = AnchorStyles.Top;
			titleLabel.AutoSize = true;
			titleLabel.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point);
			titleLabel.Location = new Point(48, 27);
			titleLabel.Name = "titleLabel";
			titleLabel.Size = new Size(558, 40);
			titleLabel.TabIndex = 4;
			titleLabel.Text = "Ticketing Screen Designer - <Bank Name>";
			// 
			// setActiveButton
			// 
			setActiveButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			setActiveButton.Enabled = false;
			setActiveButton.Location = new Point(534, 158);
			setActiveButton.Name = "setActiveButton";
			setActiveButton.Size = new Size(108, 23);
			setActiveButton.TabIndex = 5;
			setActiveButton.Text = "Set Active Screen";
			toolTip1.SetToolTip(setActiveButton, "Set Active Screen (S):\r\nSet the currently selected screen to active. \r\nThis will deactivate the currently active screen.");
			setActiveButton.UseVisualStyleBackColor = true;
			setActiveButton.Click += setActiveButton_Click;
			// 
			// previewButton
			// 
			previewButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			previewButton.Enabled = false;
			previewButton.Location = new Point(534, 185);
			previewButton.Name = "previewButton";
			previewButton.Size = new Size(108, 23);
			previewButton.TabIndex = 6;
			previewButton.Text = "Preview Screen";
			toolTip1.SetToolTip(previewButton, "Preview (P):\r\nPreview how the currently selected screen would look.");
			previewButton.UseVisualStyleBackColor = true;
			previewButton.Click += previewButton_Click;
			// 
			// refreshButton
			// 
			refreshButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			refreshButton.Location = new Point(533, 212);
			refreshButton.Name = "refreshButton";
			refreshButton.Size = new Size(108, 23);
			refreshButton.TabIndex = 7;
			refreshButton.Text = "Refresh List";
			toolTip1.SetToolTip(refreshButton, "Refresh (R):\r\nRefresh the screen list view.");
			refreshButton.UseVisualStyleBackColor = true;
			refreshButton.Click += refreshButton_Click;
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new ToolStripItem[] { accountToolStripMenuItem, helpToolStripMenuItem });
			menuStrip1.Location = new Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new Size(654, 24);
			menuStrip1.TabIndex = 8;
			menuStrip1.Text = "menuStrip1";
			// 
			// accountToolStripMenuItem
			// 
			accountToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changePasswordToolStripMenuItem, logOutToolStripMenuItem });
			accountToolStripMenuItem.Name = "accountToolStripMenuItem";
			accountToolStripMenuItem.Size = new Size(64, 20);
			accountToolStripMenuItem.Text = "Account";
			// 
			// changePasswordToolStripMenuItem
			// 
			changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
			changePasswordToolStripMenuItem.Size = new Size(168, 22);
			changePasswordToolStripMenuItem.Text = "Change password";
			changePasswordToolStripMenuItem.Click += changePasswordToolStripMenuItem_Click;
			// 
			// logOutToolStripMenuItem
			// 
			logOutToolStripMenuItem.Name = "logOutToolStripMenuItem";
			logOutToolStripMenuItem.Size = new Size(168, 22);
			logOutToolStripMenuItem.Text = "Log out";
			logOutToolStripMenuItem.Click += logOutToolStripMenuItem_Click;
			// 
			// helpToolStripMenuItem
			// 
			helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { keyboardShortcutsToolStripMenuItem });
			helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			helpToolStripMenuItem.Size = new Size(44, 20);
			helpToolStripMenuItem.Text = "Help";
			// 
			// keyboardShortcutsToolStripMenuItem
			// 
			keyboardShortcutsToolStripMenuItem.Name = "keyboardShortcutsToolStripMenuItem";
			keyboardShortcutsToolStripMenuItem.Size = new Size(176, 22);
			keyboardShortcutsToolStripMenuItem.Text = "Keyboard shortcuts";
			keyboardShortcutsToolStripMenuItem.Click += keyboardShortcutsToolStripMenuItem_Click;
			// 
			// contextMenuStrip1
			// 
			contextMenuStrip1.Name = "contextMenuStrip1";
			contextMenuStrip1.Size = new Size(61, 4);
			// 
			// statusLabel
			// 
			statusLabel.Anchor =  AnchorStyles.Bottom | AnchorStyles.Left;
			statusLabel.AutoSize = true;
			statusLabel.Location = new Point(9, 426);
			statusLabel.Name = "statusLabel";
			statusLabel.Size = new Size(0, 15);
			statusLabel.TabIndex = 9;
			// 
			// BankForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(654, 454);
			Controls.Add(statusLabel);
			Controls.Add(refreshButton);
			Controls.Add(previewButton);
			Controls.Add(setActiveButton);
			Controls.Add(titleLabel);
			Controls.Add(deleteScreenButton);
			Controls.Add(editScreenButton);
			Controls.Add(addScreenButton);
			Controls.Add(screensListView);
			Controls.Add(menuStrip1);
			MainMenuStrip = menuStrip1;
			MinimumSize = new Size(670, 475);
			Name = "BankForm";
			StartPosition = FormStartPosition.CenterParent;
			Text = "- Ticketing Screen Designer";
			FormClosed += BankForm_FormClosed;
			KeyDown += BankForm_KeyDown;
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListView screensListView;
		private Button addScreenButton;
		private Button editScreenButton;
		private Button deleteScreenButton;
		private Label titleLabel;
		private ColumnHeader isActiveColumn;
		private ColumnHeader screenTitleColumn;
		private Button setActiveButton;
		private Button previewButton;
		private Button refreshButton;
		private MenuStrip menuStrip1;
		private ContextMenuStrip contextMenuStrip1;
		private ToolStripMenuItem helpToolStripMenuItem;
		private ToolStripMenuItem keyboardShortcutsToolStripMenuItem;
		private ToolTip toolTip1;
		private Label statusLabel;
		private ToolStripMenuItem accountToolStripMenuItem;
		private ToolStripMenuItem changePasswordToolStripMenuItem;
		private ToolStripMenuItem logOutToolStripMenuItem;
	}
}