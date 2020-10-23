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

        public double StepRebarCoverLayer;

        public CIT_04_5_StairFlightReinforcementForm(List<RebarBarType> stepRebarType)
        {
            InitializeComponent();

            List<RebarBarType> stepRebarTypeListForComboBox = stepRebarType;
            comboBox_stepRebarType.DataSource = stepRebarTypeListForComboBox;
            comboBox_stepRebarType.DisplayMember = "Name";
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

        private void textBox_StepRebarCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StepRebarCoverLayer.Text, out StepRebarCoverLayer);
        }
    }
}
