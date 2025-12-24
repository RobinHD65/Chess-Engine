using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Schachengine
{
    public partial class Form2 : Form
    {
        Form1 form1;
        public Form2(Form1 _form1)
        {
            form1 = _form1;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int min))
            {
                form1.InitalizeMatch(min, radioButton1.Checked);
                
                Close();
            } else
            {
                MessageBox.Show("Bitte gebe die Minutenzahl in einer natürlichen Zahl an!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
