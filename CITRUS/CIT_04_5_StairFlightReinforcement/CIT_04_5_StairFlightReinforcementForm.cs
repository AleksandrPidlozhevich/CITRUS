using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CITRUS.Properties;

namespace CITRUS.CIT_04_5_StairFlightReinforcement
{
    public partial class CIT_04_5_StairFlightReinforcementForm : Form
    {
        public RebarBarType mySelectionStepRebarType;
        public RebarBarType mySelectionStaircaseRebarType;

        public double StepRebarCoverLayer;
        public double StepLength;
        public double StepHeight;
        public double StaircaseSlabThickness;
        public double StairCoverLayer;
        public double StepRebarStep;
        public double StaircaseRebarStep;
        public double TopExtensionStaircase;
        public double TopExtensionHeightStaircase;
        public double BottomExtensionHeightStaircase;
        public double BottomExtensionHeightStaircaseNodeA2;
        public string FirstBarMeshName = "";
        public string AdditionalBarMeshName_1 = "";
        public string AdditionalBarMeshName_2 = "";

        public string CheckedBottomConnectionNodeName;
        public string CheckedTopConnectionNodeName;

        public CIT_04_5_StairFlightReinforcementForm(List<RebarBarType> stepRebarType, List<RebarBarType> staircaseRebarType)
        {
            InitializeComponent();
            textBox_StepRebarCoverLayer.Text = Settings.Default["SFRF_StepRebarCoverLayer"].ToString();
            textBox_StepLength.Text = Settings.Default["SFRF_StepLength"].ToString();
            textBox_StepHeight.Text = Settings.Default["SFRF_StepHeight"].ToString();
            textBox_StaircaseSlabThickness.Text = Settings.Default["SFRF_StaircaseSlabThickness"].ToString();
            textBox_StairCoverLayer.Text = Settings.Default["SFRF_StairCoverLayer"].ToString();
            textBox_StepRebarStep.Text = Settings.Default["SFRF_StepRebarStep"].ToString();
            textBox_StaircaseRebarStep.Text = Settings.Default["SFRF_StaircaseRebarStep"].ToString();
            textBox_TopExtensionStaircase.Text = Settings.Default["SFRF_TopExtensionStaircase"].ToString();
            textBox_TopExtensionHeightStaircase.Text = Settings.Default["SFRF_TopExtensionHeightStaircase"].ToString();
            textBox_BottomExtensionHeightStaircase.Text = Settings.Default["SFRF_BottomExtensionHeightStaircase"].ToString();
            textBox_BottomExtensionHeightStaircaseNodeA2.Text = Settings.Default["SFRF_BottomExtensionHeightStaircaseNodeA2"].ToString();
            textBox_FirstBarMeshName.Text = Settings.Default["SFRF_FirstBarMeshName"].ToString();
            textBox_AdditionalBarMeshName_1.Text = Settings.Default["SFRF_AdditionalBarMeshName_1"].ToString();
            textBox_AdditionalBarMeshName_2.Text = Settings.Default["SFRF_AdditionalBarMeshName_2"].ToString();

            List<RebarBarType> stepRebarTypeListForComboBox = stepRebarType;
            comboBox_stepRebarType.DataSource = stepRebarTypeListForComboBox;
            comboBox_stepRebarType.DisplayMember = "Name";

            List<RebarBarType> staircaseRebarTypeListForComboBox = staircaseRebarType;
            comboBox_staircaseRebarType.DataSource = staircaseRebarTypeListForComboBox;
            comboBox_staircaseRebarType.DisplayMember = "Name";
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            Settings.Default["SFRF_StepRebarCoverLayer"] = textBox_StepRebarCoverLayer.Text;
            Settings.Default["SFRF_StepLength"] = textBox_StepLength.Text;
            Settings.Default["SFRF_StepHeight"] = textBox_StepHeight.Text;
            Settings.Default["SFRF_StaircaseSlabThickness"] = textBox_StaircaseSlabThickness.Text;
            Settings.Default["SFRF_StairCoverLayer"] = textBox_StairCoverLayer.Text;
            Settings.Default["SFRF_StepRebarStep"] = textBox_StepRebarStep.Text;
            Settings.Default["SFRF_StaircaseRebarStep"] = textBox_StaircaseRebarStep.Text;
            Settings.Default["SFRF_TopExtensionStaircase"] = textBox_TopExtensionStaircase.Text;
            Settings.Default["SFRF_TopExtensionHeightStaircase"] = textBox_TopExtensionHeightStaircase.Text;
            Settings.Default["SFRF_BottomExtensionHeightStaircase"] = textBox_BottomExtensionHeightStaircase.Text;
            Settings.Default["SFRF_BottomExtensionHeightStaircaseNodeA2"] = textBox_BottomExtensionHeightStaircaseNodeA2.Text;
            Settings.Default["SFRF_FirstBarMeshName"] = textBox_FirstBarMeshName.Text;
            Settings.Default["SFRF_AdditionalBarMeshName_1"] = textBox_AdditionalBarMeshName_1.Text;
            Settings.Default["SFRF_AdditionalBarMeshName_2"] = textBox_AdditionalBarMeshName_2.Text;
            Settings.Default.Save();

            CheckedBottomConnectionNodeName = groupBox_BottomConnectionNode.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            CheckedTopConnectionNodeName = groupBox_TopConnectionNode.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox_stepRebarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionStepRebarType = comboBox_stepRebarType.SelectedItem as RebarBarType;
        }
        private void comboBox_staircaseRebarType_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionStaircaseRebarType = comboBox_staircaseRebarType.SelectedItem as RebarBarType;
        }

        private void textBox_StepRebarCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepRebarCoverLayer.Text, out StepRebarCoverLayer);
        }
        private void textBox_StepLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepLength.Text, out StepLength);
        }
        private void textBox_StepHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepHeight.Text, out StepHeight);
        }
        private void textBox_StaircaseSlabThickness_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StaircaseSlabThickness.Text, out StaircaseSlabThickness);
        }
        private void textBox_StairCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StairCoverLayer.Text, out StairCoverLayer);
        }
        private void textBox_StepRebarStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepRebarStep.Text, out StepRebarStep);
        }
        private void textBox_StaircaseRebarStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StaircaseRebarStep.Text, out StaircaseRebarStep);
        }
        private void textBox_TopExtensionStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopExtensionStaircase.Text, out TopExtensionStaircase);
        }
        private void textBox_TopExtensionHeightStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopExtensionHeightStaircase.Text, out TopExtensionHeightStaircase);
        }
        private void textBox_BottomExtensionHeightStaircase_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomExtensionHeightStaircase.Text, out BottomExtensionHeightStaircase);
        }
        private void textBox_BottomExtensionHeightStaircaseNodeA2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomExtensionHeightStaircaseNodeA2.Text, out BottomExtensionHeightStaircaseNodeA2);
        }
        private void textBox_FirstBarMeshName_TextChanged(object sender, EventArgs e)
        {
            FirstBarMeshName = textBox_FirstBarMeshName.Text;
        }
        private void textBox_AdditionalBarMeshName_1_TextChanged(object sender, EventArgs e)
        {
            AdditionalBarMeshName_1 = textBox_AdditionalBarMeshName_1.Text;
        }
        private void textBox_AdditionalBarMeshName_2_TextChanged(object sender, EventArgs e)
        {
            AdditionalBarMeshName_2 = textBox_AdditionalBarMeshName_2.Text;
        }
    }
}
