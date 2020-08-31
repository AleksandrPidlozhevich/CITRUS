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
        public RebarBarType mySelectionMainBarTapeOne;
        public RebarBarType mySelectionMainBarTapeTwo;
        public RebarBarType mySelectionMainBarTapeThree;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarBarType mySelectionPinBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public string CheckedRebarOutletsButtonName = "";
        public int NumberOfBarsLRFaces = 0;
        public int NumberOfBarsTBFaces = 0;
        public double RebarOutletsLengthLong = 0;
        public double RebarOutletsLengthShort = 0;
        public double FloorThicknessAboveColumn = 0;
        public double StandardStirrupStep = 0;
        public double IncreasedStirrupStep = 0;
        public double FirstStirrupOffset = 0;
        public double StirrupIncreasedPlacementHeight = 0;


        public CIT_04_2_RectangularColumnsReinforcementForm(List<RebarBarType> mainBarTapesOne
            , List<RebarBarType> mainBarTapesTwo
            , List<RebarBarType> mainBarTapesThree
            , List<RebarBarType> stirrupBarTapes
            , List<RebarBarType> pinBarTapes
            , List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();

            List<RebarBarType> mainBarTapesOneListForComboBox = mainBarTapesOne;
            comboBox_MainBarTapesOne.DataSource = mainBarTapesOneListForComboBox;
            comboBox_MainBarTapesOne.DisplayMember = "Name";

            List<RebarBarType> mainBarTapesTwoListForComboBox = mainBarTapesTwo;
            comboBox_MainBarTapesTwo.DataSource = mainBarTapesTwoListForComboBox;
            comboBox_MainBarTapesTwo.DisplayMember = "Name";

            List<RebarBarType> mainBarTapesThreeListForComboBox = mainBarTapesThree;
            comboBox_MainBarTapesThree.DataSource = mainBarTapesThreeListForComboBox;
            comboBox_MainBarTapesThree.DisplayMember = "Name";

            List<RebarBarType> stirrupBarTapesForComboBox = stirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = stirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";

            List<RebarBarType> pinBarTapesForComboBox = pinBarTapes;
            comboBox_PinBarTapes.DataSource = pinBarTapesForComboBox;
            comboBox_PinBarTapes.DisplayMember = "Name";

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void comboBox_MainBarTapesOne_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionMainBarTapeOne = comboBox_MainBarTapesOne.SelectedItem as RebarBarType;
        }
        private void comboBox_MainBarTapesTwo_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionMainBarTapeTwo = comboBox_MainBarTapesTwo.SelectedItem as RebarBarType;
        }
        private void comboBox_MainBarTapesThree_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionMainBarTapeThree = comboBox_MainBarTapesThree.SelectedItem as RebarBarType;
        }

        private void comboBox_StirrupBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
        }
        private void comboBox_PinBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionPinBarTape = comboBox_PinBarTapes.SelectedItem as RebarBarType;
        }
        private void comboBox_RebarCoverTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;
        }
        private void textBox_NumberOfBarsLRFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsLRFaces.Text, out NumberOfBarsLRFaces);
        }
        private void textBox_NumberOfBarsTBFaces_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(textBox_NumberOfBarsTBFaces.Text, out NumberOfBarsTBFaces);
        }
        private void textBox_RebarOutletsLengthLong_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLengthLong.Text, out RebarOutletsLengthLong);
        }
        private void textBox_RebarOutletsLengthShort_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLengthShort.Text, out RebarOutletsLengthShort);
        }
        private void textBox_FloorThicknessAboveColumn_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThicknessAboveColumn);
        }
        private void textBox_StandardStirrupStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StandardStirrupStep.Text, out StandardStirrupStep);
        }
        private void textBox_IncreasedStirrupStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_IncreasedStirrupStep.Text, out IncreasedStirrupStep);
        }
        private void textBox_FirstStirrupOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FirstStirrupOffset.Text, out FirstStirrupOffset);
        }
        private void textBox_StirrupIncreasedPlacementHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupIncreasedPlacementHeight.Text, out StirrupIncreasedPlacementHeight);
        }
    }
}
