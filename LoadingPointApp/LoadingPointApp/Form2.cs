using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoadingPointApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            label1.Hide();
            button1.Hide();
            label2.Hide();
            button2.Hide();
            button4.Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            long hashedPassword = 1156371652;
            string enteredPassword = textBox2.Text;
            long hashedValue = enteredPassword.GetHashCode(); // gets the hashed value of the password
            
            if (textBox1.Text.Equals("admin") && hashedPassword==hashedValue)
            {
                label1.Show();
                button1.Show();
                label2.Show();
                button2.Show();
                button4.Show();
            }
            else
            {
                string message = "Invalid Username or Password!";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            label1.Hide();
            button1.Hide();
            label2.Hide();
            button2.Hide();
            textBox1.Text = "";
            textBox2.Text = "";
            button4.Hide();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}
