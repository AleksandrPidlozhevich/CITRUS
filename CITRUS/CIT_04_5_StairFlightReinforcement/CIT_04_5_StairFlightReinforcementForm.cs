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
        public string FirstBarMeshName = "";
        public string AdditionalBarMeshName_1 = "";

        public CIT_04_5_StairFlightReinforcementForm(List<RebarBarType> stepRebarType, List<RebarBarType> staircaseRebarType)
        {
            InitializeComponent();

            List<RebarBarType> stepRebarTypeListForComboBox = stepRebarType;
            comboBox_stepRebarType.DataSource = stepRebarTypeListForComboBox;
            comboBox_stepRebarType.DisplayMember = "Name";

            List<RebarBarType> staircaseRebarTypeListForComboBox = staircaseRebarType;
            comboBox_staircaseRebarType.DataSource = staircaseRebarTypeListForComboBox;
            comboBox_staircaseRebarType.DisplayMember = "Name";
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
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

        private void textBox_FirstBarMeshName_TextChanged(object sender, EventArgs e)
        {
            FirstBarMeshName = textBox_FirstBarMeshName.Text;
        }

        private void textBox_AdditionalBarMeshName_1_TextChanged(object sender, EventArgs e)
        {
            AdditionalBarMeshName_1 = textBox_AdditionalBarMeshName_1.Text;
        }
    }
}
