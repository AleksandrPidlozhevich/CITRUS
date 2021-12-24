using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CITRUS
{
    public partial class RefreshDuctFittingsStartForm : Form
    {
        public string RefreshOptionCheckedButtonName;
        public RefreshDuctFittingsStartForm()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            RefreshOptionCheckedButtonName = groupBox_RefreshOption.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void RefreshDuctFittingsStartForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Enter || e.KeyValue == (char)Keys.Space)
            {
                RefreshOptionCheckedButtonName = groupBox_RefreshOption.Controls.OfType<RadioButton>().FirstOrDefault(rb => rb.Checked).Name;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }

            else if (e.KeyValue == (char)Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }
    }
}
