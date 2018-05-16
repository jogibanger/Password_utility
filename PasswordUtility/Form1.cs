using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CryptoLib;

namespace PasswordUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnProceed_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(txtPassword.Text))
            {
                try
                {
                    if (rbEncode.Checked)
                    {
                        result = CryptoLib.Crypto.EncryptString(txtPassword.Text);
                    }
                    else if (rbDecode.Checked) 
                    {
                        result = CryptoLib.Crypto.DecryptString(txtPassword.Text);
                    }
                    txtResult.Text = result;
                }
                catch (Exception ex)
                {
                    lblErrorMessage.Text = "Error: " + ex.Message;
                }
            }
            else {
                MessageBox.Show("Please input text to encrypt / decrypt");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtPassword.Text = string.Empty;
            txtResult.Text = string.Empty;
            rbDecode.Checked = false;
            rbEncode.Checked = false;
        }

    }
}
