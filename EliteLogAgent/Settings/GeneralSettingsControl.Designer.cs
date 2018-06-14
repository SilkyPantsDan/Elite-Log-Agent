﻿namespace EliteLogAgent.Settings
{
    partial class GeneralSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdrNameLabel = new System.Windows.Forms.Label();
            this.commanderNameTextBox = new System.Windows.Forms.TextBox();
            this.uploadLatestDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdrNameLabel
            // 
            this.cmdrNameLabel.AutoSize = true;
            this.cmdrNameLabel.Location = new System.Drawing.Point(16, 6);
            this.cmdrNameLabel.Name = "cmdrNameLabel";
            this.cmdrNameLabel.Size = new System.Drawing.Size(73, 13);
            this.cmdrNameLabel.TabIndex = 5;
            this.cmdrNameLabel.Text = "CMDR Name:";
            // 
            // commanderNameTextBox
            // 
            this.commanderNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commanderNameTextBox.Location = new System.Drawing.Point(95, 3);
            this.commanderNameTextBox.Name = "commanderNameTextBox";
            this.commanderNameTextBox.Size = new System.Drawing.Size(170, 20);
            this.commanderNameTextBox.TabIndex = 4;
            this.commanderNameTextBox.Text = "Commander Name";
            this.commanderNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // uploadLatestDataButton
            // 
            this.uploadLatestDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uploadLatestDataButton.Location = new System.Drawing.Point(3, 29);
            this.uploadLatestDataButton.Name = "uploadLatestDataButton";
            this.uploadLatestDataButton.Size = new System.Drawing.Size(262, 23);
            this.uploadLatestDataButton.TabIndex = 3;
            this.uploadLatestDataButton.Text = "Upload last 5 files for all providers";
            this.uploadLatestDataButton.UseVisualStyleBackColor = true;
            this.uploadLatestDataButton.Click += new System.EventHandler(this.uploadLatestDataButton_Click);
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdrNameLabel);
            this.Controls.Add(this.commanderNameTextBox);
            this.Controls.Add(this.uploadLatestDataButton);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(268, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cmdrNameLabel;
        private System.Windows.Forms.TextBox commanderNameTextBox;
        private System.Windows.Forms.Button uploadLatestDataButton;
    }
}