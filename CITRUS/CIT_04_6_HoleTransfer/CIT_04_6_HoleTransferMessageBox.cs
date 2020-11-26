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
    public partial class CIT_04_6_HoleTransferMessageBox : Form
    {
        public CIT_04_6_HoleTransferMessageBox(string str)
        {
            InitializeComponent();
            richTextBox_Message.Text = "Не удалось найти основу для элементов с ID:\n" + str;
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
