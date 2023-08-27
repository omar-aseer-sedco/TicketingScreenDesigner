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
			listView1 = new ListView();
			addScreenButton = new Button();
			editScreenButton = new Button();
			deleteScreenButton = new Button();
			SuspendLayout();
			// 
			// listView1
			// 
			listView1.Location = new Point(12, 68);
			listView1.Name = "listView1";
			listView1.Size = new Size(515, 356);
			listView1.TabIndex = 0;
			listView1.UseCompatibleStateImageBehavior = false;
			// 
			// addScreenButton
			// 
			addScreenButton.Location = new Point(533, 68);
			addScreenButton.Name = "addScreenButton";
			addScreenButton.Size = new Size(108, 23);
			addScreenButton.TabIndex = 1;
			addScreenButton.Text = "Add";
			addScreenButton.UseVisualStyleBackColor = true;
			addScreenButton.Click += addScreenButton_Click;
			// 
			// editScreenButton
			// 
			editScreenButton.Enabled = false;
			editScreenButton.Location = new Point(533, 97);
			editScreenButton.Name = "editScreenButton";
			editScreenButton.Size = new Size(108, 23);
			editScreenButton.TabIndex = 2;
			editScreenButton.Text = "Edit";
			editScreenButton.UseVisualStyleBackColor = true;
			// 
			// deleteScreenButton
			// 
			deleteScreenButton.Enabled = false;
			deleteScreenButton.Location = new Point(533, 126);
			deleteScreenButton.Name = "deleteScreenButton";
			deleteScreenButton.Size = new Size(108, 23);
			deleteScreenButton.TabIndex = 3;
			deleteScreenButton.Text = "Delete";
			deleteScreenButton.UseVisualStyleBackColor = true;
			// 
			// BankForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(653, 436);
			Controls.Add(deleteScreenButton);
			Controls.Add(editScreenButton);
			Controls.Add(addScreenButton);
			Controls.Add(listView1);
			Name = "BankForm";
			Text = "- Ticketing Screen Designer";
			ResumeLayout(false);
		}

		#endregion

		private ListView listView1;
		private Button addScreenButton;
		private Button editScreenButton;
		private Button deleteScreenButton;
	}
}