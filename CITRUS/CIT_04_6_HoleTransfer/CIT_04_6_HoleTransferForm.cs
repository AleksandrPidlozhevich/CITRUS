using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS.CIT_04_6_HoleTransfer
{
    public partial class CIT_04_6_HoleTransferForm : Form
    {
        public string CheckedButtonNameTransferOption = "";
        public CIT_04_6_HoleTransferForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            CheckedButtonNameTransferOption = groupBox_TransferOption.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
