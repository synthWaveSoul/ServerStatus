using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerStatus
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            string path = @"logs.txt";                        
            string s = "";
            int i = 1;

            StreamReader sr = File.OpenText(path);
            while ((s = sr.ReadLine()) != null)
            {
                richTextBox1.AppendText((i++ + ". " + s) + Environment.NewLine);
            }
            sr.Close();         
        }
    }
}
