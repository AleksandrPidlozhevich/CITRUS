namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    partial class CIT_04_2_RectangularColumnsReinforcementForm
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
            this.btn_Ok = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.comboBox_MainBarTapes = new System.Windows.Forms.ComboBox();
            this.comboBox_StirrupBarTapes = new System.Windows.Forms.ComboBox();
            this.comboBox_RebarCoverTypes = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btn_Ok
            // 
            this.btn_Ok.Location = new System.Drawing.Point(184, 229);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 0;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(314, 229);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 1;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // comboBox_MainBarTapes
            // 
            this.comboBox_MainBarTapes.FormattingEnabled = true;
            this.comboBox_MainBarTapes.Location = new System.Drawing.Point(12, 12);
            this.comboBox_MainBarTapes.Name = "comboBox_MainBarTapes";
            this.comboBox_MainBarTapes.Size = new System.Drawing.Size(121, 21);
            this.comboBox_MainBarTapes.Sorted = true;
            this.comboBox_MainBarTapes.TabIndex = 2;
            this.comboBox_MainBarTapes.SelectedIndexChanged += new System.EventHandler(this.comboBox_MainBarTapes_SelectedIndexChanged);
            // 
            // comboBox_StirrupBarTapes
            // 
            this.comboBox_StirrupBarTapes.FormattingEnabled = true;
            this.comboBox_StirrupBarTapes.Location = new System.Drawing.Point(12, 65);
            this.comboBox_StirrupBarTapes.Name = "comboBox_StirrupBarTapes";
            this.comboBox_StirrupBarTapes.Size = new System.Drawing.Size(121, 21);
            this.comboBox_StirrupBarTapes.Sorted = true;
            this.comboBox_StirrupBarTapes.TabIndex = 3;
            this.comboBox_StirrupBarTapes.SelectedIndexChanged += new System.EventHandler(this.comboBox_StirrupBarTapes_SelectedIndexChanged);
            // 
            // comboBox_RebarCoverTypes
            // 
            this.comboBox_RebarCoverTypes.FormattingEnabled = true;
            this.comboBox_RebarCoverTypes.Location = new System.Drawing.Point(12, 117);
            this.comboBox_RebarCoverTypes.Name = "comboBox_RebarCoverTypes";
            this.comboBox_RebarCoverTypes.Size = new System.Drawing.Size(121, 21);
            this.comboBox_RebarCoverTypes.Sorted = true;
            this.comboBox_RebarCoverTypes.TabIndex = 4;
            this.comboBox_RebarCoverTypes.SelectedIndexChanged += new System.EventHandler(this.comboBox_RebarCoverTypes_SelectedIndexChanged);
            // 
            // CIT_04_2_RectangularColumnsReinforcementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(426, 266);
            this.Controls.Add(this.comboBox_RebarCoverTypes);
            this.Controls.Add(this.comboBox_StirrupBarTapes);
            this.Controls.Add(this.comboBox_MainBarTapes);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Name = "CIT_04_2_RectangularColumnsReinforcementForm";
            this.Text = "CIT_04_2_RectangularColumnsReinforcementForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ComboBox comboBox_MainBarTapes;
        private System.Windows.Forms.ComboBox comboBox_StirrupBarTapes;
        private System.Windows.Forms.ComboBox comboBox_RebarCoverTypes;
    }
}