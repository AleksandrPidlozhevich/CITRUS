namespace CITRUS.CIT_04_6_HoleTransfer
{
    partial class CIT_04_6_HoleTransferForm
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
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.groupBox_TransferOption = new System.Windows.Forms.GroupBox();
            this.radioButton_TransferAll = new System.Windows.Forms.RadioButton();
            this.radioButton_TransferSelected = new System.Windows.Forms.RadioButton();
            this.groupBox_TransferOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(172, 94);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 7;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(43, 94);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 6;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // groupBox_TransferOption
            // 
            this.groupBox_TransferOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_TransferOption.Controls.Add(this.radioButton_TransferAll);
            this.groupBox_TransferOption.Controls.Add(this.radioButton_TransferSelected);
            this.groupBox_TransferOption.Location = new System.Drawing.Point(12, 12);
            this.groupBox_TransferOption.Name = "groupBox_TransferOption";
            this.groupBox_TransferOption.Size = new System.Drawing.Size(260, 69);
            this.groupBox_TransferOption.TabIndex = 8;
            this.groupBox_TransferOption.TabStop = false;
            this.groupBox_TransferOption.Text = "Выберите вариант:";
            // 
            // radioButton_TransferAll
            // 
            this.radioButton_TransferAll.AutoSize = true;
            this.radioButton_TransferAll.Location = new System.Drawing.Point(7, 43);
            this.radioButton_TransferAll.Name = "radioButton_TransferAll";
            this.radioButton_TransferAll.Size = new System.Drawing.Size(101, 17);
            this.radioButton_TransferAll.TabIndex = 1;
            this.radioButton_TransferAll.Text = "Перенести все";
            this.radioButton_TransferAll.UseVisualStyleBackColor = true;
            // 
            // radioButton_TransferSelected
            // 
            this.radioButton_TransferSelected.AutoSize = true;
            this.radioButton_TransferSelected.Checked = true;
            this.radioButton_TransferSelected.Location = new System.Drawing.Point(7, 20);
            this.radioButton_TransferSelected.Name = "radioButton_TransferSelected";
            this.radioButton_TransferSelected.Size = new System.Drawing.Size(141, 17);
            this.radioButton_TransferSelected.TabIndex = 0;
            this.radioButton_TransferSelected.TabStop = true;
            this.radioButton_TransferSelected.Text = "Перенести выбранные";
            this.radioButton_TransferSelected.UseVisualStyleBackColor = true;
            // 
            // CIT_04_6_HoleTransferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 131);
            this.Controls.Add(this.groupBox_TransferOption);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.MaximumSize = new System.Drawing.Size(300, 170);
            this.MinimumSize = new System.Drawing.Size(300, 170);
            this.Name = "CIT_04_6_HoleTransferForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Перенос проемов";
            this.groupBox_TransferOption.ResumeLayout(false);
            this.groupBox_TransferOption.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.GroupBox groupBox_TransferOption;
        private System.Windows.Forms.RadioButton radioButton_TransferAll;
        private System.Windows.Forms.RadioButton radioButton_TransferSelected;
    }
}