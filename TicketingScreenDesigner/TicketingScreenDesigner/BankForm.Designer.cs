namespace TicketingScreenDesigner
{
	partial class BankForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			screensListView = new ListView();
			screenIdColumn = new ColumnHeader();
			isActiveColumn = new ColumnHeader();
			screenTitleColumn = new ColumnHeader();
			addScreenButton = new Button();
			editScreenButton = new Button();
			deleteScreenButton = new Button();
			titleLabel = new Label();
			setActiveButton = new Button();
			SuspendLayout();
			// 
			// screensListView
			// 
			screensListView.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			screensListView.Columns.AddRange(new ColumnHeader[] { screenIdColumn, isActiveColumn, screenTitleColumn });
			screensListView.FullRowSelect = true;
			screensListView.Location = new Point(12, 68);
			screensListView.Name = "screensListView";
			screensListView.Size = new Size(515, 356);
			screensListView.TabIndex = 0;
			screensListView.UseCompatibleStateImageBehavior = false;
			screensListView.View = View.Details;
			screensListView.SelectedIndexChanged += screensListView_SelectedIndexChanged;
			// 
			// screenIdColumn
			// 
			screenIdColumn.Text = "Screen ID";
			screenIdColumn.Width = 100;
			// 
			// isActiveColumn
			// 
			isActiveColumn.Text = "Active?";
			isActiveColumn.Width = 65;
			// 
			// screenTitleColumn
			// 
			screenTitleColumn.Text = "Screen Title";
			screenTitleColumn.Width = 346;
			// 
			// addScreenButton
			// 
			addScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			addScreenButton.Location = new Point(534, 68);
			addScreenButton.Name = "addScreenButton";
			addScreenButton.Size = new Size(108, 23);
			addScreenButton.TabIndex = 1;
			addScreenButton.Text = "Add";
			addScreenButton.UseVisualStyleBackColor = true;
			addScreenButton.Click += addScreenButton_Click;
			// 
			// editScreenButton
			// 
			editScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			editScreenButton.Enabled = false;
			editScreenButton.Location = new Point(534, 97);
			editScreenButton.Name = "editScreenButton";
			editScreenButton.Size = new Size(108, 23);
			editScreenButton.TabIndex = 2;
			editScreenButton.Text = "Edit";
			editScreenButton.UseVisualStyleBackColor = true;
			editScreenButton.Click += editScreenButton_Click;
			// 
			// deleteScreenButton
			// 
			deleteScreenButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			deleteScreenButton.Enabled = false;
			deleteScreenButton.Location = new Point(534, 126);
			deleteScreenButton.Name = "deleteScreenButton";
			deleteScreenButton.Size = new Size(108, 23);
			deleteScreenButton.TabIndex = 3;
			deleteScreenButton.Text = "Delete";
			deleteScreenButton.UseVisualStyleBackColor = true;
			deleteScreenButton.Click += deleteScreenButton_Click;
			// 
			// titleLabel
			// 
			titleLabel.Anchor = AnchorStyles.Top;
			titleLabel.AutoSize = true;
			titleLabel.Font = new Font("Segoe UI", 21.75F, FontStyle.Regular, GraphicsUnit.Point);
			titleLabel.Location = new Point(48, 14);
			titleLabel.Name = "titleLabel";
			titleLabel.Size = new Size(558, 40);
			titleLabel.TabIndex = 4;
			titleLabel.Text = "Ticketing Screen Designer - <Bank Name>";
			// 
			// setActiveButton
			// 
			setActiveButton.Anchor =  AnchorStyles.Top | AnchorStyles.Right;
			setActiveButton.Enabled = false;
			setActiveButton.Location = new Point(534, 155);
			setActiveButton.Name = "setActiveButton";
			setActiveButton.Size = new Size(108, 23);
			setActiveButton.TabIndex = 5;
			setActiveButton.Text = "Set Active Screen";
			setActiveButton.UseVisualStyleBackColor = true;
			setActiveButton.Click += setActiveButton_Click;
			// 
			// BankForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(654, 436);
			Controls.Add(setActiveButton);
			Controls.Add(titleLabel);
			Controls.Add(deleteScreenButton);
			Controls.Add(editScreenButton);
			Controls.Add(addScreenButton);
			Controls.Add(screensListView);
			Name = "BankForm";
			StartPosition = FormStartPosition.CenterScreen;
			Text = "- Ticketing Screen Designer";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private ListView screensListView;
		private Button addScreenButton;
		private Button editScreenButton;
		private Button deleteScreenButton;
		private Label titleLabel;
		private ColumnHeader screenIdColumn;
		private ColumnHeader isActiveColumn;
		private ColumnHeader screenTitleColumn;
		private Button setActiveButton;
	}
}