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

namespace CITRUS.CIT_04_1_SquareColumnsReinforcement
{
    public partial class CIT_04_1_1FormSquareColumnsReinforcementType4 : Form
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

        public double SecondLowerRebarOffset;
        public double SecondTopRebarOffset;
        public double SecondLeftRebarOffset;
        public double SecondRightRebarOffset;
        public double DeepeningBarsSize;

        public string CheckedRebarOutletsButtonName;

        public bool СhangeColumnSection;
        public bool TransitionToOverlap;
        public bool DeepeningBars;
        public bool BendIntoASlab;

        public CIT_04_1_1FormSquareColumnsReinforcementType4(List<RebarBarType> firstMainBarTapes
            , List<RebarBarType> secondMainBarTapes
            , List<RebarBarType> firstStirrupBarTapes
            , List<RebarBarType> secondStirrupBarTapes
            , List<RebarCoverType> rebarCoverTypes)
        {
            InitializeComponent();
            textBox_FloorThicknessAboveColumn.Text = Settings.Default["FSCRT4_FloorThickness"].ToString();
            textBox_RebarOutletsLength.Text = Settings.Default["FSCRT4_RebarOutlets"].ToString();
            textBox_RebarSecondOutletsLength.Text = Settings.Default["FSCRT4_RebarSecondOutlets"].ToString();
            textBox_FirstStirrupOffset.Text = Settings.Default["FSCRT4_FirstStirrupOffset"].ToString();
            textBox_IncreasedStirrupSpacing.Text = Settings.Default["FSCRT4_IncreasedStirrupSpacing"].ToString();
            textBox_StandardStirrupSpacing.Text = Settings.Default["FSCRT4_StandardStirrupSpacing"].ToString();
            textBox_StirrupIncreasedPlacementHeight.Text = Settings.Default["FSCRT4_StirrupIncreasedPlacementHeight"].ToString();
            textBox_ColumnSectionOffset.Text = Settings.Default["FSCRT4_ColumnSectionOffset"].ToString();
            textBox_DeepeningBars.Text = Settings.Default["FSCRT4_DeepeningBarsSize"].ToString();
            textBox_SecondLowerRebarOffset.Text = Settings.Default["FSCRT4_SecondLowerRebarOffset"].ToString();
            textBox_SecondTopRebarOffset.Text = Settings.Default["FSCRT4_SecondTopRebarOffset"].ToString();
            textBox_SecondLeftRebarOffset.Text = Settings.Default["FSCRT4_SecondLeftRebarOffset"].ToString();
            textBox_SecondRightRebarOffset.Text = Settings.Default["FSCRT4_SecondRightRebarOffset"].ToString();

            List <RebarBarType> firstMainBarTapesListForComboBox = firstMainBarTapes;
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
            BendIntoASlab = checkBox_BendIntoASlab.Checked;

            Settings.Default["FSCRT4_FloorThickness"] = textBox_FloorThicknessAboveColumn.Text;
            Settings.Default["FSCRT4_RebarOutlets"] = textBox_RebarOutletsLength.Text;
            Settings.Default["FSCRT4_RebarSecondOutlets"] = textBox_RebarSecondOutletsLength.Text;
            Settings.Default["FSCRT4_FirstStirrupOffset"] = textBox_FirstStirrupOffset.Text;
            Settings.Default["FSCRT4_IncreasedStirrupSpacing"] = textBox_IncreasedStirrupSpacing.Text;
            Settings.Default["FSCRT4_StandardStirrupSpacing"] = textBox_StandardStirrupSpacing.Text;
            Settings.Default["FSCRT4_StirrupIncreasedPlacementHeight"] = textBox_StirrupIncreasedPlacementHeight.Text;
            Settings.Default["FSCRT4_ColumnSectionOffset"] = textBox_ColumnSectionOffset.Text;
            Settings.Default["FSCRT4_DeepeningBarsSize"] = textBox_DeepeningBars.Text;
            Settings.Default["FSCRT4_SecondLowerRebarOffset"] = textBox_SecondLowerRebarOffset.Text;
            Settings.Default["FSCRT4_SecondTopRebarOffset"] = textBox_SecondTopRebarOffset.Text;
            Settings.Default["FSCRT4_SecondLeftRebarOffset"] = textBox_SecondLeftRebarOffset.Text;
            Settings.Default["FSCRT4_SecondRightRebarOffset"] = textBox_SecondRightRebarOffset.Text;
            Settings.Default.Save();

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
        private void textBox_ColumnSectionOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_ColumnSectionOffset.Text, out ColumnSectionOffset);
        }
        private void textBox_SecondLowerRebarOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLowerRebarOffset.Text, out SecondLowerRebarOffset);
        }
        private void textBox_SecondTopRebarOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondTopRebarOffset.Text, out SecondTopRebarOffset);
        }
        private void textBox_SecondLeftRebarOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondLeftRebarOffset.Text, out SecondLeftRebarOffset);
        }
        private void textBox_SecondRightRebarOffset_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_SecondRightRebarOffset.Text, out SecondRightRebarOffset);
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
