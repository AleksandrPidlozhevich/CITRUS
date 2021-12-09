
namespace CITRUS
{
    partial class GloryHoleSaveAssignmentVersionForm
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
            this.groupBox_ActionSelection = new System.Windows.Forms.GroupBox();
            this.radioButton_ResetAssignmentVersion = new System.Windows.Forms.RadioButton();
            this.radioButton_SaveAssignmentVersion = new System.Windows.Forms.RadioButton();
            this.groupBox_ActionSelection.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(292, 74);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 4;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(163, 74);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 3;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // groupBox_ActionSelection
            // 
            this.groupBox_ActionSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_ActionSelection.Controls.Add(this.radioButton_ResetAssignmentVersion);
            this.groupBox_ActionSelection.Controls.Add(this.radioButton_SaveAssignmentVersion);
            this.groupBox_ActionSelection.Location = new System.Drawing.Point(12, 12);
            this.groupBox_ActionSelection.Name = "groupBox_ActionSelection";
            this.groupBox_ActionSelection.Size = new System.Drawing.Size(380, 56);
            this.groupBox_ActionSelection.TabIndex = 5;
            this.groupBox_ActionSelection.TabStop = false;
            this.groupBox_ActionSelection.Text = "Выберите действие:";
            // 
            // radioButton_ResetAssignmentVersion
            // 
            this.radioButton_ResetAssignmentVersion.AutoSize = true;
            this.radioButton_ResetAssignmentVersion.Location = new System.Drawing.Point(215, 19);
            this.radioButton_ResetAssignmentVersion.Name = "radioButton_ResetAssignmentVersion";
            this.radioButton_ResetAssignmentVersion.Size = new System.Drawing.Size(159, 17);
            this.radioButton_ResetAssignmentVersion.TabIndex = 1;
            this.radioButton_ResetAssignmentVersion.Text = "Сбросить версию задания";
            this.radioButton_ResetAssignmentVersion.UseVisualStyleBackColor = true;
            // 
            // radioButton_SaveAssignmentVersion
            // 
            this.radioButton_SaveAssignmentVersion.AutoSize = true;
            this.radioButton_SaveAssignmentVersion.Checked = true;
            this.radioButton_SaveAssignmentVersion.Location = new System.Drawing.Point(6, 19);
            this.radioButton_SaveAssignmentVersion.Name = "radioButton_SaveAssignmentVersion";
            this.radioButton_SaveAssignmentVersion.Size = new System.Drawing.Size(164, 17);
            this.radioButton_SaveAssignmentVersion.TabIndex = 0;
            this.radioButton_SaveAssignmentVersion.TabStop = true;
            this.radioButton_SaveAssignmentVersion.Text = "Сохранить версию задания";
            this.radioButton_SaveAssignmentVersion.UseVisualStyleBackColor = true;
            // 
            // GloryHoleSaveAssignmentVersionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(404, 111);
            this.Controls.Add(this.groupBox_ActionSelection);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(420, 150);
            this.MinimumSize = new System.Drawing.Size(420, 150);
            this.Name = "GloryHoleSaveAssignmentVersionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Задание на отверстия";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GloryHoleSaveAssignmentVersionForm_KeyDown);
            this.groupBox_ActionSelection.ResumeLayout(false);
            this.groupBox_ActionSelection.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.GroupBox groupBox_ActionSelection;
        private System.Windows.Forms.RadioButton radioButton_ResetAssignmentVersion;
        private System.Windows.Forms.RadioButton radioButton_SaveAssignmentVersion;
    }
}