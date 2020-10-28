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
            this.textBox_StepLength = new System.Windows.Forms.TextBox();
            this.textBox_StepHeight = new System.Windows.Forms.TextBox();
            this.textBox_StaircaseSlabThickness = new System.Windows.Forms.TextBox();
            this.textBox_StairCoverLayer = new System.Windows.Forms.TextBox();
            this.textBox_StepRebarStep = new System.Windows.Forms.TextBox();
            this.textBox_FirstBarMeshName = new System.Windows.Forms.TextBox();
            this.comboBox_staircaseRebarType = new System.Windows.Forms.ComboBox();
            this.textBox_StaircaseRebarStep = new System.Windows.Forms.TextBox();
            this.textBox_TopExtensionStaircase = new System.Windows.Forms.TextBox();
            this.textBox_TopExtensionHeightStaircase = new System.Windows.Forms.TextBox();
            this.textBox_BottomExtensionHeightStaircase = new System.Windows.Forms.TextBox();
            this.textBox_AdditionalBarMeshName_1 = new System.Windows.Forms.TextBox();
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
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
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
            this.comboBox_stepRebarType.Sorted = true;
            this.comboBox_stepRebarType.TabIndex = 4;
            this.comboBox_stepRebarType.SelectedIndexChanged += new System.EventHandler(this.comboBox_stepRebarType_SelectedIndexChanged);
            // 
            // textBox_StepRebarCoverLayer
            // 
            this.textBox_StepRebarCoverLayer.Location = new System.Drawing.Point(30, 265);
            this.textBox_StepRebarCoverLayer.Name = "textBox_StepRebarCoverLayer";
            this.textBox_StepRebarCoverLayer.Size = new System.Drawing.Size(100, 20);
            this.textBox_StepRebarCoverLayer.TabIndex = 5;
            this.textBox_StepRebarCoverLayer.TextChanged += new System.EventHandler(this.textBox_StepRebarCoverLayer_TextChanged);
            // 
            // textBox_StepLength
            // 
            this.textBox_StepLength.Location = new System.Drawing.Point(30, 213);
            this.textBox_StepLength.Name = "textBox_StepLength";
            this.textBox_StepLength.Size = new System.Drawing.Size(100, 20);
            this.textBox_StepLength.TabIndex = 6;
            this.textBox_StepLength.TextChanged += new System.EventHandler(this.textBox_StepLength_TextChanged);
            // 
            // textBox_StepHeight
            // 
            this.textBox_StepHeight.Location = new System.Drawing.Point(30, 239);
            this.textBox_StepHeight.Name = "textBox_StepHeight";
            this.textBox_StepHeight.Size = new System.Drawing.Size(100, 20);
            this.textBox_StepHeight.TabIndex = 7;
            this.textBox_StepHeight.TextChanged += new System.EventHandler(this.textBox_StepHeight_TextChanged);
            // 
            // textBox_StaircaseSlabThickness
            // 
            this.textBox_StaircaseSlabThickness.Location = new System.Drawing.Point(485, 213);
            this.textBox_StaircaseSlabThickness.Name = "textBox_StaircaseSlabThickness";
            this.textBox_StaircaseSlabThickness.Size = new System.Drawing.Size(100, 20);
            this.textBox_StaircaseSlabThickness.TabIndex = 8;
            this.textBox_StaircaseSlabThickness.TextChanged += new System.EventHandler(this.textBox_StaircaseSlabThickness_TextChanged);
            // 
            // textBox_StairCoverLayer
            // 
            this.textBox_StairCoverLayer.Location = new System.Drawing.Point(485, 239);
            this.textBox_StairCoverLayer.Name = "textBox_StairCoverLayer";
            this.textBox_StairCoverLayer.Size = new System.Drawing.Size(100, 20);
            this.textBox_StairCoverLayer.TabIndex = 9;
            this.textBox_StairCoverLayer.TextChanged += new System.EventHandler(this.textBox_StairCoverLayer_TextChanged);
            // 
            // textBox_StepRebarStep
            // 
            this.textBox_StepRebarStep.Location = new System.Drawing.Point(30, 70);
            this.textBox_StepRebarStep.Name = "textBox_StepRebarStep";
            this.textBox_StepRebarStep.Size = new System.Drawing.Size(91, 20);
            this.textBox_StepRebarStep.TabIndex = 10;
            this.textBox_StepRebarStep.TextChanged += new System.EventHandler(this.textBox_StepRebarStep_TextChanged);
            // 
            // textBox_FirstBarMeshName
            // 
            this.textBox_FirstBarMeshName.Location = new System.Drawing.Point(668, 42);
            this.textBox_FirstBarMeshName.Name = "textBox_FirstBarMeshName";
            this.textBox_FirstBarMeshName.Size = new System.Drawing.Size(100, 20);
            this.textBox_FirstBarMeshName.TabIndex = 11;
            this.textBox_FirstBarMeshName.TextChanged += new System.EventHandler(this.textBox_FirstBarMeshName_TextChanged);
            // 
            // comboBox_staircaseRebarType
            // 
            this.comboBox_staircaseRebarType.FormattingEnabled = true;
            this.comboBox_staircaseRebarType.Location = new System.Drawing.Point(485, 129);
            this.comboBox_staircaseRebarType.Name = "comboBox_staircaseRebarType";
            this.comboBox_staircaseRebarType.Size = new System.Drawing.Size(100, 21);
            this.comboBox_staircaseRebarType.Sorted = true;
            this.comboBox_staircaseRebarType.TabIndex = 12;
            this.comboBox_staircaseRebarType.SelectedIndexChanged += new System.EventHandler(this.comboBox_staircaseRebarType_SelectedIndexChanged);
            // 
            // textBox_StaircaseRebarStep
            // 
            this.textBox_StaircaseRebarStep.Location = new System.Drawing.Point(485, 156);
            this.textBox_StaircaseRebarStep.Name = "textBox_StaircaseRebarStep";
            this.textBox_StaircaseRebarStep.Size = new System.Drawing.Size(100, 20);
            this.textBox_StaircaseRebarStep.TabIndex = 13;
            this.textBox_StaircaseRebarStep.TextChanged += new System.EventHandler(this.textBox_StaircaseRebarStep_TextChanged);
            // 
            // textBox_TopExtensionStaircase
            // 
            this.textBox_TopExtensionStaircase.Location = new System.Drawing.Point(327, 337);
            this.textBox_TopExtensionStaircase.Name = "textBox_TopExtensionStaircase";
            this.textBox_TopExtensionStaircase.Size = new System.Drawing.Size(100, 20);
            this.textBox_TopExtensionStaircase.TabIndex = 14;
            this.textBox_TopExtensionStaircase.TextChanged += new System.EventHandler(this.textBox_TopExtensionStaircase_TextChanged);
            // 
            // textBox_TopExtensionHeightStaircase
            // 
            this.textBox_TopExtensionHeightStaircase.Location = new System.Drawing.Point(327, 372);
            this.textBox_TopExtensionHeightStaircase.Name = "textBox_TopExtensionHeightStaircase";
            this.textBox_TopExtensionHeightStaircase.Size = new System.Drawing.Size(100, 20);
            this.textBox_TopExtensionHeightStaircase.TabIndex = 15;
            this.textBox_TopExtensionHeightStaircase.TextChanged += new System.EventHandler(this.textBox_TopExtensionHeightStaircase_TextChanged);
            // 
            // textBox_BottomExtensionHeightStaircase
            // 
            this.textBox_BottomExtensionHeightStaircase.Location = new System.Drawing.Point(181, 372);
            this.textBox_BottomExtensionHeightStaircase.Name = "textBox_BottomExtensionHeightStaircase";
            this.textBox_BottomExtensionHeightStaircase.Size = new System.Drawing.Size(100, 20);
            this.textBox_BottomExtensionHeightStaircase.TabIndex = 16;
            this.textBox_BottomExtensionHeightStaircase.TextChanged += new System.EventHandler(this.textBox_BottomExtensionHeightStaircase_TextChanged);
            // 
            // textBox_AdditionalBarMeshName_1
            // 
            this.textBox_AdditionalBarMeshName_1.Location = new System.Drawing.Point(668, 70);
            this.textBox_AdditionalBarMeshName_1.Name = "textBox_AdditionalBarMeshName_1";
            this.textBox_AdditionalBarMeshName_1.Size = new System.Drawing.Size(100, 20);
            this.textBox_AdditionalBarMeshName_1.TabIndex = 17;
            this.textBox_AdditionalBarMeshName_1.TextChanged += new System.EventHandler(this.textBox_AdditionalBarMeshName_1_TextChanged);
            // 
            // CIT_04_5_StairFlightReinforcementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.textBox_AdditionalBarMeshName_1);
            this.Controls.Add(this.textBox_BottomExtensionHeightStaircase);
            this.Controls.Add(this.textBox_TopExtensionHeightStaircase);
            this.Controls.Add(this.textBox_TopExtensionStaircase);
            this.Controls.Add(this.textBox_StaircaseRebarStep);
            this.Controls.Add(this.comboBox_staircaseRebarType);
            this.Controls.Add(this.textBox_FirstBarMeshName);
            this.Controls.Add(this.textBox_StepRebarStep);
            this.Controls.Add(this.textBox_StairCoverLayer);
            this.Controls.Add(this.textBox_StaircaseSlabThickness);
            this.Controls.Add(this.textBox_StepHeight);
            this.Controls.Add(this.textBox_StepLength);
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
        private System.Windows.Forms.TextBox textBox_StepLength;
        private System.Windows.Forms.TextBox textBox_StepHeight;
        private System.Windows.Forms.TextBox textBox_StaircaseSlabThickness;
        private System.Windows.Forms.TextBox textBox_StairCoverLayer;
        private System.Windows.Forms.TextBox textBox_StepRebarStep;
        private System.Windows.Forms.TextBox textBox_FirstBarMeshName;
        private System.Windows.Forms.ComboBox comboBox_staircaseRebarType;
        private System.Windows.Forms.TextBox textBox_StaircaseRebarStep;
        private System.Windows.Forms.TextBox textBox_TopExtensionStaircase;
        private System.Windows.Forms.TextBox textBox_TopExtensionHeightStaircase;
        private System.Windows.Forms.TextBox textBox_BottomExtensionHeightStaircase;
        private System.Windows.Forms.TextBox textBox_AdditionalBarMeshName_1;
    }
}