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

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType6 : Form
    {
        public RebarBarType mySelectionFirstMainBarTape;
        public RebarBarType mySelectionSecondMainBarTape;
        public RebarBarType mySelectionFirstStirrupBarTape;
        public RebarBarType mySelectionSecondStirrupBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public double FloorThickness;
        public double RebarOutlets;
        public double RebarSecondOutlets;
        public double FirstStirrupOffset;
        public double IncreasedStirrupSpacing;
        public double StandardStirrupSpacing;
        public double StirrupIncreasedPlacementHeight;
        public double ColumnSectionOffset;

        public double SecondLowerRebarOffset1;
        public double SecondTopRebarOffset1;
        public double SecondLeftRebarOffset1;
        public double SecondRightRebarOffset1;

        public double SecondLowerRebarOffset2;
        public double SecondTopRebarOffset2;
        public double SecondLeftRebarOffset2;
        public double SecondRightRebarOffset2;
        public double DeepeningBarsSize;

        public string CheckedRebarOutletsButtonName;

        public bool СhangeColumnSection;
        public bool TransitionToOverlap;
        public bool DeepeningBars;

        public CIT_04_1_1FormSquareColumnsReinforcementType6(List<RebarBarType> firstMainBarTapes
            , List<RebarBarType> secondMainBarTapes
            , List<RebarBarType> firstStirrupBarTapes
            , List<RebarBarType> secondStirrupBarTapes
            , List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();

            List<RebarBarType> firstMainBarTapesListForComboBox = firstMainBarTapes;
            comboBox_FirstMainBarTapes.DataSource = firstMainBarTapesListForComboBox;
            comboBox_FirstMainBarTapes.DisplayMember = "Name";

            List<RebarBarType> secondMainBarTapesListForComboBox = secondMainBarTapes;
            comboBox_SecondMainBarTapes.DataSource = secondMainBarTapesListForComboBox;
            comboBox_SecondMainBarTapes.DisplayMember = "Name";

            List<RebarBarType> firstStirrupBarTapesForComboBox = firstStirrupBarTapes;
            comboBox_StirrupBarTapes.DataSource = firstStirrupBarTapesForComboBox;
            comboBox_StirrupBarTapes.DisplayMember = "Name";

            List<RebarBarType> secondStirrupBarTapesForComboBox = secondStirrupBarTapes;
            comboBox_SecondStirrupBarTapes.DataSource = secondStirrupBarTapesForComboBox;
            comboBox_SecondStirrupBarTapes.DisplayMember = "Name";

            List<RebarCoverType> rebarCoverTypesListForComboBox = rebarCoverTypes;
            comboBox_RebarCoverTypes.DataSource = rebarCoverTypesListForComboBox;
            comboBox_RebarCoverTypes.DisplayMember = "Name";
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox_FirstMainBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionFirstMainBarTape = comboBox_FirstMainBarTapes.SelectedItem as RebarBarType;
        }
        private void comboBox_SecondMainBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionSecondMainBarTape = comboBox_SecondMainBarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_StirrupBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionFirstStirrupBarTape = comboBox_StirrupBarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_SecondStirrupBarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionSecondStirrupBarTape = comboBox_SecondStirrupBarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_RebarCoverTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionRebarCoverType = comboBox_RebarCoverTypes.SelectedItem as RebarCoverType;
        }

        private void textBox_FloorThicknessAboveColumn_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThickness);

        }

        private void textBox_RebarOutletsLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLength.Text, out RebarOutlets);
        }

        private void textBox_RebarSecondOutletsLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarSecondOutletsLength.Text, out RebarSecondOutlets);
        }

        private void textBox_FirstStirrupOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FirstStirrupOffset.Text, out FirstStirrupOffset);
        }

        private void textBox_IncreasedStirrupSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_IncreasedStirrupSpacing.Text, out IncreasedStirrupSpacing);
        }

        private void textBox_StandardStirrupSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StandardStirrupSpacing.Text, out StandardStirrupSpacing);
        }

        private void textBox_StirrupIncreasedPlacementHeight_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_StirrupIncreasedPlacementHeight.Text, out StirrupIncreasedPlacementHeight);
        }

        private void checkBox_СhangeSection_CheckedChanged(object sender, EventArgs e)
        {
            СhangeColumnSection = checkBox_СhangeSection.Checked;
        }

        private void textBox_ColumnSectionOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ColumnSectionOffset.Text, out ColumnSectionOffset);
        }


        private void CIT_04_1_1FormSquareColumnsReinforcementType4_Load(object sender, EventArgs e)
        {

        }

        private void textBox_SecondLowerRebarOffset1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLowerRebarOffset1.Text, out SecondLowerRebarOffset1);
        }

        private void textBox_SecondTopRebarOffset1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondTopRebarOffset1.Text, out SecondTopRebarOffset1);
        }

        private void textBox_SecondLeftRebarOffset1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLeftRebarOffset1.Text, out SecondLeftRebarOffset1);
        }

        private void textBox_SecondRightRebarOffset1_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondRightRebarOffset1.Text, out SecondRightRebarOffset1);
        }

        private void textBox_SecondLowerRebarOffset2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLowerRebarOffset2.Text, out SecondLowerRebarOffset2);
        }

        private void textBox_SecondTopRebarOffset2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondTopRebarOffset2.Text, out SecondTopRebarOffset2);
        }

        private void textBox_SecondLeftRebarOffset2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLeftRebarOffset2.Text, out SecondLeftRebarOffset2);
        }

        private void textBox_SecondRightRebarOffset2_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondRightRebarOffset2.Text, out SecondRightRebarOffset2);
        }

        private void textBox_DeepeningBars_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningBars.Text, out DeepeningBarsSize);
        }
    }
}
