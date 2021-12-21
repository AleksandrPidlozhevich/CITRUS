
namespace CITRUS
{
    partial class GloryHoleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GloryHoleForm));
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_Ok = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox_DuctSideClearance = new System.Windows.Forms.TextBox();
            this.textBox_DuctTopBottomClearance = new System.Windows.Forms.TextBox();
            this.textBox_PipeSideClearance = new System.Windows.Forms.TextBox();
            this.textBox_PipeTopBottomClearance = new System.Windows.Forms.TextBox();
            this.checkBox_MergeHoles = new System.Windows.Forms.CheckBox();
            this.textBox_RoundUpIncrement = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Cancel.Location = new System.Drawing.Point(472, 324);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(100, 25);
            this.btn_Cancel.TabIndex = 9;
            this.btn_Cancel.Text = "Отмена";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_Ok
            // 
            this.btn_Ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Ok.Location = new System.Drawing.Point(343, 324);
            this.btn_Ok.Name = "btn_Ok";
            this.btn_Ok.Size = new System.Drawing.Size(100, 25);
            this.btn_Ok.TabIndex = 8;
            this.btn_Ok.Text = "Ок";
            this.btn_Ok.UseVisualStyleBackColor = true;
            this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(560, 306);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // textBox_DuctSideClearance
            // 
            this.textBox_DuctSideClearance.Location = new System.Drawing.Point(309, 249);
            this.textBox_DuctSideClearance.Name = "textBox_DuctSideClearance";
            this.textBox_DuctSideClearance.Size = new System.Drawing.Size(50, 20);
            this.textBox_DuctSideClearance.TabIndex = 17;
            this.textBox_DuctSideClearance.Text = "75";
            this.textBox_DuctSideClearance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_DuctTopBottomClearance
            // 
            this.textBox_DuctTopBottomClearance.Location = new System.Drawing.Point(415, 67);
            this.textBox_DuctTopBottomClearance.Name = "textBox_DuctTopBottomClearance";
            this.textBox_DuctTopBottomClearance.Size = new System.Drawing.Size(50, 20);
            this.textBox_DuctTopBottomClearance.TabIndex = 18;
            this.textBox_DuctTopBottomClearance.Text = "75";
            this.textBox_DuctTopBottomClearance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_PipeSideClearance
            // 
            this.textBox_PipeSideClearance.Location = new System.Drawing.Point(134, 209);
            this.textBox_PipeSideClearance.Name = "textBox_PipeSideClearance";
            this.textBox_PipeSideClearance.Size = new System.Drawing.Size(50, 20);
            this.textBox_PipeSideClearance.TabIndex = 19;
            this.textBox_PipeSideClearance.Text = "50";
            this.textBox_PipeSideClearance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_PipeTopBottomClearance
            // 
            this.textBox_PipeTopBottomClearance.Location = new System.Drawing.Point(148, 104);
            this.textBox_PipeTopBottomClearance.Name = "textBox_PipeTopBottomClearance";
            this.textBox_PipeTopBottomClearance.Size = new System.Drawing.Size(50, 20);
            this.textBox_PipeTopBottomClearance.TabIndex = 20;
            this.textBox_PipeTopBottomClearance.Text = "50";
            this.textBox_PipeTopBottomClearance.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkBox_MergeHoles
            // 
            this.checkBox_MergeHoles.AutoSize = true;
            this.checkBox_MergeHoles.Checked = true;
            this.checkBox_MergeHoles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_MergeHoles.Location = new System.Drawing.Point(400, 22);
            this.checkBox_MergeHoles.Name = "checkBox_MergeHoles";
            this.checkBox_MergeHoles.Size = new System.Drawing.Size(143, 17);
            this.checkBox_MergeHoles.TabIndex = 21;
            this.checkBox_MergeHoles.Text = "Объединить отверстия";
            this.checkBox_MergeHoles.UseVisualStyleBackColor = true;
            // 
            // textBox_RoundUpIncrement
            // 
            this.textBox_RoundUpIncrement.Location = new System.Drawing.Point(221, 295);
            this.textBox_RoundUpIncrement.Name = "textBox_RoundUpIncrement";
            this.textBox_RoundUpIncrement.Size = new System.Drawing.Size(50, 20);
            this.textBox_RoundUpIncrement.TabIndex = 22;
            this.textBox_RoundUpIncrement.Text = "50";
            this.textBox_RoundUpIncrement.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(37, 298);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Размеры отверстий округлять до";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(277, 298);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "мм";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GloryHoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_RoundUpIncrement);
            this.Controls.Add(this.checkBox_MergeHoles);
            this.Controls.Add(this.textBox_PipeTopBottomClearance);
            this.Controls.Add(this.textBox_PipeSideClearance);
            this.Controls.Add(this.textBox_DuctTopBottomClearance);
            this.Controls.Add(this.textBox_DuctSideClearance);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_Ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.MaximumSize = new System.Drawing.Size(600, 400);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.Name = "GloryHoleForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Параметры отверстий";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GloryHoleForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_Ok;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox_DuctSideClearance;
        private System.Windows.Forms.TextBox textBox_DuctTopBottomClearance;
        private System.Windows.Forms.TextBox textBox_PipeSideClearance;
        private System.Windows.Forms.TextBox textBox_PipeTopBottomClearance;
        private System.Windows.Forms.CheckBox checkBox_MergeHoles;
        private System.Windows.Forms.TextBox textBox_RoundUpIncrement;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}