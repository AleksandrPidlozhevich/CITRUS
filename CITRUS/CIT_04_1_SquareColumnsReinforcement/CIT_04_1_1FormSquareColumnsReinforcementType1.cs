using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using CITRUS.Properties;

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType1 : Form
    {
        public RebarBarType mySelectionMainBarTape;
        public RebarBarType mySelectionStirrupBarTape;
        public RebarCoverType mySelectionRebarCoverType;

        public double FloorThickness;
        public double RebarOutlets;
        public double FirstStirrupOffset;
        public double IncreasedStirrupSpacing;
        public double StandardStirrupSpacing;
        public double StirrupIncreasedPlacementHeight;
        public double ColumnSectionOffset;
        public double DeepeningBarsSize;
        public string CheckedRebarOutletsButtonName;

        public bool СhangeColumnSection;
        public bool TransitionToOverlap;
        public bool DeepeningBars;
        public bool BendIntoASlab;

        public CIT_04_1_1FormSquareColumnsReinforcementType1(List<RebarBarType> mainBarTapes, List<RebarBarType> stirrupBarTapes, List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            textBox_FloorThicknessAboveColumn.Text = Settings.Default["FSCRT1_FloorThickness"].ToString();
            textBox_RebarOutletsLength.Text = Settings.Default["FSCRT1_RebarOutlets"].ToString();
            textBox_FirstStirrupOffset.Text = Settings.Default["FSCRT1_FirstStirrupOffset"].ToString();
            textBox_IncreasedStirrupSpacing.Text = Settings.Default["FSCRT1_IncreasedStirrupSpacing"].ToString();
            textBox_StandardStirrupSpacing.Text = Settings.Default["FSCRT1_StandardStirrupSpacing"].ToString();
            textBox_StirrupIncreasedPlacementHeight.Text = Settings.Default["FSCRT1_StirrupIncreasedPlacementHeight"].ToString();
            textBox_ColumnSectionOffset.Text = Settings.Default["FSCRT1_ColumnSectionOffset"].ToString();
            textBox_DeepeningBars.Text = Settings.Default["FSCRT1_DeepeningBarsSize"].ToString();

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

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            CheckedRebarOutletsButtonName = groupBox_RebarOutlets.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            TransitionToOverlap = checkBox_TransitionToOverlap.Checked;
            DeepeningBars = checkBox_DeepeningBars.Checked;
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            Settings.Default["FSCRT1_FloorThickness"] = textBox_FloorThicknessAboveColumn.Text;
            Settings.Default["FSCRT1_RebarOutlets"] = textBox_RebarOutletsLength.Text;
            Settings.Default["FSCRT1_FirstStirrupOffset"] = textBox_FirstStirrupOffset.Text;
            Settings.Default["FSCRT1_IncreasedStirrupSpacing"] = textBox_IncreasedStirrupSpacing.Text;
            Settings.Default["FSCRT1_StandardStirrupSpacing"] = textBox_StandardStirrupSpacing.Text;
            Settings.Default["FSCRT1_StirrupIncreasedPlacementHeight"] = textBox_StirrupIncreasedPlacementHeight.Text;
            Settings.Default["FSCRT1_ColumnSectionOffset"] = textBox_ColumnSectionOffset.Text;
            Settings.Default["FSCRT1_DeepeningBarsSize"] = textBox_DeepeningBars.Text;
            Settings.Default.Save();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
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

        private void textBox_FloorThicknessAboveColumn_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_FloorThicknessAboveColumn.Text, out FloorThickness);

        }
        private void textBox_RebarOutletsLength_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_RebarOutletsLength.Text, out RebarOutlets);
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
        private void textBox_ColumnSectionOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ColumnSectionOffset.Text, out ColumnSectionOffset);
        }
        private void textBox_DeepeningBars_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_DeepeningBars.Text, out DeepeningBarsSize);
        }

        private void checkBox_СhangeSection_CheckedChanged(object sender, EventArgs e)
        {
            СhangeColumnSection = checkBox_СhangeSection.Checked;
        }


    }
}
