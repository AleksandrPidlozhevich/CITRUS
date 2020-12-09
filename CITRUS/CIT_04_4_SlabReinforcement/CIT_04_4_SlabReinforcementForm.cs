using Autodesk.Revit.DB.Structure;
using CITRUS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_04_4_SlabReinforcement
{
    public partial class CIT_04_4_SlabReinforcementForm : Form
    {
        public RebarBarType mySelectionBottomXDirectionRebarTape;
        public RebarBarType mySelectionBottomYDirectionRebarTape;
        public RebarBarType mySelectionTopXDirectionRebarTape;
        public RebarBarType mySelectionTopYDirectionRebarTape;

        public double BottomXDirectionRebarSpacing;
        public double BottomYDirectionRebarSpacing;
        public double TopXDirectionRebarSpacing;
        public double TopYDirectionRebarSpacing;

        public RebarCoverType mySelectionRebarCoverTypeForTop;
        public RebarCoverType mySelectionRebarCoverTypeForBottom;

        public double PerimeterFramingDiam;
        public double PerimeterFramingOverlaping;
        public double PerimeterFramingEndCoverLayer;
        public double PerimeterFramingStep;

        public CIT_04_4_SlabReinforcementForm(List<RebarBarType> BottomXDirectionRebarTapesList,
            List<RebarBarType> BottomYDirectionRebarTapesList
            , List<RebarBarType> TopXDirectionRebarTapesList
            , List<RebarBarType> TopYDirectionRebarTapesList
            , List<RebarCoverType> rebarCoverTypesForTop
            , List<RebarCoverType> rebarCoverTypesForBottom)
        {
            InitializeComponent();

            textBox_BottomXDirectionRebarSpacing.Text = Settings.Default["SR_BottomXDirectionRebarSpacing"].ToString();
            textBox_BottomYDirectionRebarSpacing.Text = Settings.Default["SR_BottomYDirectionRebarSpacing"].ToString();
            textBox_TopXDirectionRebarSpacing.Text = Settings.Default["SR_TopXDirectionRebarSpacing"].ToString();
            textBox_TopYDirectionRebarSpacing.Text = Settings.Default["SR_TopYDirectionRebarSpacing"].ToString();
            textBox_PerimeterFramingDiam.Text = Settings.Default["SR_PerimeterFramingDiam"].ToString();
            textBox_PerimeterFramingOverlaping.Text = Settings.Default["SR_PerimeterFramingOverlaping"].ToString();
            textBox_PerimeterFramingEndCoverLayer.Text = Settings.Default["SR_PerimeterFramingEndCoverLayer"].ToString();
            textBox_PerimeterFramingStep.Text = Settings.Default["SR_PerimeterFramingStep"].ToString();



            List<RebarBarType> bottomXDirectionRebarTapesListForComboBox = BottomXDirectionRebarTapesList;
            comboBox_BottomXDirectionRebarTapes.DataSource = bottomXDirectionRebarTapesListForComboBox;
            comboBox_BottomXDirectionRebarTapes.DisplayMember = "Name";

            List<RebarBarType> bottomYDirectionRebarTapesListForComboBox = BottomYDirectionRebarTapesList;
            comboBox_BottomYDirectionRebarTapes.DataSource = bottomYDirectionRebarTapesListForComboBox;
            comboBox_BottomYDirectionRebarTapes.DisplayMember = "Name";

            List<RebarBarType> topXDirectionRebarTapesListForComboBox = TopXDirectionRebarTapesList;
            comboBox_TopXDirectionRebarTapes.DataSource = topXDirectionRebarTapesListForComboBox;
            comboBox_TopXDirectionRebarTapes.DisplayMember = "Name";

            List<RebarBarType> topYDirectionRebarTapesListForComboBox = TopYDirectionRebarTapesList;
            comboBox_TopYDirectionRebarTapes.DataSource = topYDirectionRebarTapesListForComboBox;
            comboBox_TopYDirectionRebarTapes.DisplayMember = "Name";

            List<RebarCoverType> rebarCoverTypesForTopForComboBox = rebarCoverTypesForTop;
            comboBox_RebarCoverTypeForTop.DataSource = rebarCoverTypesForTopForComboBox;
            comboBox_RebarCoverTypeForTop.DisplayMember = "Name";

            List<RebarCoverType> rebarCoverTypesForBottomForComboBox = rebarCoverTypesForBottom;
            comboBox_RebarCoverTypeForBottom.DataSource = rebarCoverTypesForBottomForComboBox;
            comboBox_RebarCoverTypeForBottom.DisplayMember = "Name";
        }

        private void button1_Ok_Click(object sender, EventArgs e)
        {
            Settings.Default["SR_BottomXDirectionRebarSpacing"] = textBox_BottomXDirectionRebarSpacing.Text;
            Settings.Default["SR_BottomYDirectionRebarSpacing"] = textBox_BottomYDirectionRebarSpacing.Text;
            Settings.Default["SR_TopXDirectionRebarSpacing"] = textBox_TopXDirectionRebarSpacing.Text;
            Settings.Default["SR_TopYDirectionRebarSpacing"] = textBox_TopYDirectionRebarSpacing.Text;
            Settings.Default["SR_PerimeterFramingDiam"] = textBox_PerimeterFramingDiam.Text;
            Settings.Default["SR_PerimeterFramingOverlaping"] = textBox_PerimeterFramingOverlaping.Text;
            Settings.Default["SR_PerimeterFramingEndCoverLayer"] = textBox_PerimeterFramingEndCoverLayer.Text;
            Settings.Default["SR_PerimeterFramingStep"] = textBox_PerimeterFramingStep.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void comboBox_BottomXDirectionRebarTapesListForComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionBottomXDirectionRebarTape = comboBox_BottomXDirectionRebarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_BottomYDirectionRebarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionBottomYDirectionRebarTape = comboBox_BottomYDirectionRebarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_TopXDirectionRebarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionTopXDirectionRebarTape = comboBox_TopXDirectionRebarTapes.SelectedItem as RebarBarType;
        }

        private void comboBox_TopYDirectionRebarTapes_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionTopYDirectionRebarTape = comboBox_TopYDirectionRebarTapes.SelectedItem as RebarBarType;
        }

        private void textBox_BottomXDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomXDirectionRebarSpacing.Text, out BottomXDirectionRebarSpacing);
        }

        private void textBox_BottomYDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_BottomYDirectionRebarSpacing.Text, out BottomYDirectionRebarSpacing);
        }

        private void textBox_TopXDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopXDirectionRebarSpacing.Text, out TopXDirectionRebarSpacing);
        }

        private void textBox_TopYDirectionRebarSpacing_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_TopYDirectionRebarSpacing.Text, out TopYDirectionRebarSpacing);
        }

        private void comboBox_RebarCoverTypeForTop_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionRebarCoverTypeForTop = comboBox_RebarCoverTypeForTop.SelectedItem as RebarCoverType;
        }

        private void comboBox_RebarCoverTypeForBottom_SelectedIndexChanged(object sender, EventArgs e)
        {
            mySelectionRebarCoverTypeForBottom = comboBox_RebarCoverTypeForBottom.SelectedItem as RebarCoverType;
        }

        private void textBox_PerimeterFramingDiam_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingDiam.Text, out PerimeterFramingDiam);
        }

        private void textBox_PerimeterFramingOverlaping_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingOverlaping.Text, out PerimeterFramingOverlaping);
        }

        private void textBox_PerimeterFramingEndCoverLayer_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingEndCoverLayer.Text, out PerimeterFramingEndCoverLayer);
        }

        private void textBox_PerimeterFramingStep_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(textBox_PerimeterFramingStep.Text, out PerimeterFramingStep);
        }
    }
}
