namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    partial class CIT_04_5_StairFlightReinforcementForm
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
            this.comboBox_stepRebarType = new System.Windows.Forms.ComboBox();
            this.textBox_StepRebarCoverLayer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(686, 413);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 3;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(557, 413);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 2;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // comboBox_stepRebarType
            // 
            this.comboBox_stepRebarType.FormattingEnabled = true;
            this.comboBox_stepRebarType.Location = new System.Drawing.Point(30, 31);
            this.comboBox_stepRebarType.Name = "comboBox_stepRebarType";
            this.comboBox_stepRebarType.Size = new System.Drawing.Size(91, 21);
            this.comboBox_stepRebarType.TabIndex = 4;
            this.comboBox_stepRebarType.SelectedIndexChanged += new System.EventHandler(this.comboBox_stepRebarType_SelectedIndexChanged);
            // 
            // textBox_StepRebarCoverLayer
            // 
            this.textBox_StepRebarCoverLayer.Location = new System.Drawing.Point(174, 32);
            this.textBox_StepRebarCoverLayer.Name = "textBox_StepRebarCoverLayer";
            this.textBox_StepRebarCoverLayer.Size = new System.Drawing.Size(100, 20);
            this.textBox_StepRebarCoverLayer.TabIndex = 5;
            this.textBox_StepRebarCoverLayer.TextChanged += new System.EventHandler(this.textBox_StepRebarCoverLayer_TextChanged);
            // 
            // CIT_04_5_StairFlightReinforcementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_StepRebarCoverLayer);
            this.Controls.Add(this.comboBox_stepRebarType);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.Name = "CIT_04_5_StairFlightReinforcementForm";
            this.Text = "CIT_04_5_StairFlightReinforcementForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.ComboBox comboBox_stepRebarType;
        private System.Windows.Forms.TextBox textBox_StepRebarCoverLayer;
    }
}