
namespace CITRUS
{
    partial class RefreshDuctFittingsStartForm
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
            this.btn_Not = new System.Windows.Forms.Button();
            this.btn_Yes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox_RefreshOption = new System.Windows.Forms.GroupBox();
            this.radioButton_WholeProject = new System.Windows.Forms.RadioButton();
            this.radioButton_VisibleInView = new System.Windows.Forms.RadioButton();
            this.radioButton_Selected = new System.Windows.Forms.RadioButton();
            this.groupBox_RefreshOption.SuspendLayout();
            this.SuspendLayout();
            // 
            // btn_Not
            // 
            this.btn_Not.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Not.Location = new System.Drawing.Point(272, 124);
            this.btn_Not.Name = "btn_Not";
            this.btn_Not.Size = new System.Drawing.Size(100, 25);
            this.btn_Not.TabIndex = 6;
            this.btn_Not.Text = "Нет";
            this.btn_Not.UseVisualStyleBackColor = true;
            this.btn_Not.Click += new System.EventHandler(this.btn_Cancel_Click);
            this.btn_Not.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RefreshDuctFittingsStartForm_KeyDown);
            // 
            // btn_Yes
            // 
            this.btn_Yes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Yes.Location = new System.Drawing.Point(143, 124);
            this.btn_Yes.Name = "btn_Yes";
            this.btn_Yes.Size = new System.Drawing.Size(100, 25);
            this.btn_Yes.TabIndex = 5;
            this.btn_Yes.Text = "Да";
            this.btn_Yes.UseVisualStyleBackColor = true;
            this.btn_Yes.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(364, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Обновление фитингов внесет изменение в модель с учетом настроек";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "трассировки воздуховодов по умолчанию. Продолжить?";
            // 
            // groupBox_RefreshOption
            // 
            this.groupBox_RefreshOption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_RefreshOption.Controls.Add(this.radioButton_WholeProject);
            this.groupBox_RefreshOption.Controls.Add(this.radioButton_VisibleInView);
            this.groupBox_RefreshOption.Controls.Add(this.radioButton_Selected);
            this.groupBox_RefreshOption.Location = new System.Drawing.Point(13, 13);
            this.groupBox_RefreshOption.Name = "groupBox_RefreshOption";
            this.groupBox_RefreshOption.Size = new System.Drawing.Size(359, 50);
            this.groupBox_RefreshOption.TabIndex = 11;
            this.groupBox_RefreshOption.TabStop = false;
            this.groupBox_RefreshOption.Text = "Вариант обновления:";
            // 
            // radioButton_WholeProject
            // 
            this.radioButton_WholeProject.AutoSize = true;
            this.radioButton_WholeProject.Location = new System.Drawing.Point(242, 19);
            this.radioButton_WholeProject.Name = "radioButton_WholeProject";
            this.radioButton_WholeProject.Size = new System.Drawing.Size(111, 17);
            this.radioButton_WholeProject.TabIndex = 2;
            this.radioButton_WholeProject.Text = "Во всем проекте";
            this.radioButton_WholeProject.UseVisualStyleBackColor = true;
            // 
            // radioButton_VisibleInView
            // 
            this.radioButton_VisibleInView.AutoSize = true;
            this.radioButton_VisibleInView.Location = new System.Drawing.Point(110, 19);
            this.radioButton_VisibleInView.Name = "radioButton_VisibleInView";
            this.radioButton_VisibleInView.Size = new System.Drawing.Size(114, 17);
            this.radioButton_VisibleInView.TabIndex = 1;
            this.radioButton_VisibleInView.Text = "Видимые на виде";
            this.radioButton_VisibleInView.UseVisualStyleBackColor = true;
            // 
            // radioButton_Selected
            // 
            this.radioButton_Selected.AutoSize = true;
            this.radioButton_Selected.Checked = true;
            this.radioButton_Selected.Location = new System.Drawing.Point(6, 19);
            this.radioButton_Selected.Name = "radioButton_Selected";
            this.radioButton_Selected.Size = new System.Drawing.Size(84, 17);
            this.radioButton_Selected.TabIndex = 0;
            this.radioButton_Selected.TabStop = true;
            this.radioButton_Selected.Text = "Выбранные";
            this.radioButton_Selected.UseVisualStyleBackColor = true;
            // 
            // RefreshDuctFittingsStartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 161);
            this.Controls.Add(this.groupBox_RefreshOption);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_Not);
            this.Controls.Add(this.btn_Yes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximumSize = new System.Drawing.Size(400, 200);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "RefreshDuctFittingsStartForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Внимание!!!";
            this.groupBox_RefreshOption.ResumeLayout(false);
            this.groupBox_RefreshOption.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Not;
        private System.Windows.Forms.Button btn_Yes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox_RefreshOption;
        private System.Windows.Forms.RadioButton radioButton_WholeProject;
        private System.Windows.Forms.RadioButton radioButton_VisibleInView;
        private System.Windows.Forms.RadioButton radioButton_Selected;
    }
}