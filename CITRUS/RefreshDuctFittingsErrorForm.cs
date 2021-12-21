using Autodesk.Revit.DB;
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
    public partial class RefreshDuctFittingsErrorForm : System.Windows.Forms.Form
    {
        public RefreshDuctFittingsErrorForm(string elementIdsListStr)
        {
            InitializeComponent();
            richTextBox_Errors.Text = elementIdsListStr;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
