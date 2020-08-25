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

namespace CITRUS.CIT_04_2_RectangularColumnsReinforcement
{
    public partial class CIT_04_2_RectangularColumnsReinforcementForm : Form
    {
        public RebarBarType mySelectionMainBarTape;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public CIT_04_2_RectangularColumnsReinforcementForm(List<RebarBarType> mainBarTapes, List<RebarBarType> stirrupBarTapes, List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();

            List<RebarBarType> mainBarTapesListForComboBox = mainBarTapes;
            comboBox_MainBarTapes.DataSource = mainBarTapesListForComboBox;
            comboBox_MainBarTapes.DisplayMember = "Name";

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
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
        private void comboBox_MainBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionMainBarTape = comboBox_MainBarTapes.SelectedItem as RebarBarType;
        }
        private void comboBox_StirrupBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
        }
        private void comboBox_RebarCoverTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;
        }
    }
}
