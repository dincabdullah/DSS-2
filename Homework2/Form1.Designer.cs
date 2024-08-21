namespace Homework2
{
    partial class Form1
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
            this.BrowseButton = new System.Windows.Forms.Button();
            this.SOMButton = new System.Windows.Forms.Button();
            this.FilePathTextBox = new System.Windows.Forms.TextBox();
            this.DimensionTextBox = new System.Windows.Forms.TextBox();
            this.clusterPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.DatasetFileLabel = new System.Windows.Forms.Label();
            this.DimensionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // BrowseButton
            // 
            this.BrowseButton.Location = new System.Drawing.Point(275, 10);
            this.BrowseButton.Name = "BrowseButton";
            this.BrowseButton.Size = new System.Drawing.Size(75, 23);
            this.BrowseButton.TabIndex = 0;
            this.BrowseButton.Text = "Browse";
            this.BrowseButton.UseVisualStyleBackColor = true;
            this.BrowseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            // 
            // SOMButton
            // 
            this.SOMButton.Location = new System.Drawing.Point(160, 45);
            this.SOMButton.Name = "SOMButton";
            this.SOMButton.Size = new System.Drawing.Size(75, 23);
            this.SOMButton.TabIndex = 1;
            this.SOMButton.Text = "SOM";
            this.SOMButton.UseVisualStyleBackColor = true;
            this.SOMButton.Click += new System.EventHandler(this.SOMButton_Click);
            // 
            // DatasetFileLabel
            // 
            this.DatasetFileLabel.AutoSize = true;
            this.DatasetFileLabel.Location = new System.Drawing.Point(12, 15);
            this.DatasetFileLabel.Name = "DatasetFileLabel";
            this.DatasetFileLabel.Size = new System.Drawing.Size(68, 13);
            this.DatasetFileLabel.TabIndex = 2;
            this.DatasetFileLabel.Text = "Dataset file:";
            // 
            // FilePathTextBox
            // 
            this.FilePathTextBox.Location = new System.Drawing.Point(86, 12);
            this.FilePathTextBox.Name = "FilePathTextBox";
            this.FilePathTextBox.ReadOnly = true;
            this.FilePathTextBox.Size = new System.Drawing.Size(183, 20);
            this.FilePathTextBox.TabIndex = 3;
            // 
            // DimensionLabel
            // 
            this.DimensionLabel.AutoSize = true;
            this.DimensionLabel.Location = new System.Drawing.Point(12, 50);
            this.DimensionLabel.Name = "DimensionLabel";
            this.DimensionLabel.Size = new System.Drawing.Size(60, 13);
            this.DimensionLabel.TabIndex = 4;
            this.DimensionLabel.Text = "Dimension:";
            // 
            // DimensionTextBox
            // 
            this.DimensionTextBox.Location = new System.Drawing.Point(86, 47);
            this.DimensionTextBox.Name = "DimensionTextBox";
            this.DimensionTextBox.Size = new System.Drawing.Size(50, 20);
            this.DimensionTextBox.TabIndex = 5;
            // 
            // clusterPanel
            // 
            this.clusterPanel.AutoScroll = true;
            this.clusterPanel.BackColor = System.Drawing.Color.LightBlue;
            this.clusterPanel.Location = new System.Drawing.Point(12, 90);
            this.clusterPanel.Name = "clusterPanel";
            this.clusterPanel.Size = new System.Drawing.Size(370, 370);
            this.clusterPanel.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(394, 472);
            this.Controls.Add(this.clusterPanel);
            this.Controls.Add(this.DimensionTextBox);
            this.Controls.Add(this.DimensionLabel);
            this.Controls.Add(this.FilePathTextBox);
            this.Controls.Add(this.DatasetFileLabel);
            this.Controls.Add(this.SOMButton);
            this.Controls.Add(this.BrowseButton);
            this.Name = "Form1";
            this.Text = "MainForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BrowseButton;
        private System.Windows.Forms.Button SOMButton;
        private System.Windows.Forms.TextBox FilePathTextBox;
        private System.Windows.Forms.TextBox DimensionTextBox;
        private System.Windows.Forms.FlowLayoutPanel clusterPanel;
        private System.Windows.Forms.Label DatasetFileLabel;
        private System.Windows.Forms.Label DimensionLabel;
    }
}
