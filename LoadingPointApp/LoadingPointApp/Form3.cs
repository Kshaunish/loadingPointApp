using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace LoadingPointApp
{
    public partial class Form3 : Form
    {
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=C:\\Users\\kshau\\source\\repos\\LoadingPointApp\\LoadingPointApp\\Database1.mdf;Integrated Security = True");

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
        private bool CheckVendorNumberInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Vendors where Vendor_Number='" + textBox1.Text + "'", con);
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Close();
                con.Close();
                return true;
            }
            else
            {
                rd.Close();
                con.Close();
                return false;
            }
        }
        private bool CheckWordOrderInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Vendors where Work_Order='" + textBox2.Text + "'", con);
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Close();
                con.Close();
                return true;
            }
            else
            {
                rd.Close();
                con.Close();
                return false;
            }
        }
        private bool CheckVendorIdAndWordOrderInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Vendors where Vendor_Number=@ID and Work_Order='" + textBox2.Text + "'", con);
            cmd.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Close();
                con.Close();
                return true;
            }
            else
            {
                rd.Close();
                con.Close();
                return false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (CheckVendorIdAndWordOrderInDB())
            {
                string message = "Please enter a different Vendor number or Work Order ";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);

            }
            else
            {
                //string message = "Ok";
                //string title = "Warning";
                //MessageBoxButtons buttons = MessageBoxButtons.OK;
                //MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
                con.Open();
                SqlCommand cmd1 = new SqlCommand("insert into Vendors values(@ID,'"+textBox2.Text+"')", con);
                cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd1 = cmd1.ExecuteReader();
                string message = "New Vendor Number and Work Order added";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox2.Text = "";
                textBox1.Text = "";
                con.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (CheckVendorIdAndWordOrderInInvoice())
            {
                string message = "Cannot delete the records because they are curently in use";
                string title = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
                textBox2.Text = "";
                textBox1.Text = "";
            }
            else
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("delete from Vendors where Vendor_Number=@ID and Work_Order='" + textBox2.Text + "'", con);
                cmd2.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd2.ExecuteReader();
                string message = "Deleted the record";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox2.Text = "";
                textBox1.Text = "";
                con.Close();
            }
        }

        private bool CheckVendorIdAndWordOrderInInvoice()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Invoice where v_no=@ID and w_o='" + textBox2.Text + "'", con);
            cmd.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
            SqlDataReader rd = cmd.ExecuteReader();
            if (rd.HasRows)
            {
                rd.Close();
                con.Close();
                return true;
            }
            else
            {
                rd.Close();
                con.Close();
                return false;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (CheckVendorIdAndWordOrderInInvoice())
            {
                string message = "Cannot update the records because they are curently in use";
                string title = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
                textBox2.Text = "";
                textBox1.Text = "";
            }
            else
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("update Vendors set Work_Order='" + textBox2.Text + "' where Vendor_Number=@ID", con);
                cmd2.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd2.ExecuteReader();
                string message = "Updated the record";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox2.Text = "";
                textBox1.Text = "";
                con.Close();
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                string message = "Please enter vendor number";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
             
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select Work_Order from Vendors where Vendor_Number=@ID", con);
                cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd1.ExecuteReader();
                string workOrders = String.Empty;
                if (rd2.HasRows)
                {
                    while (rd2.Read())
                    {
                        workOrders = workOrders + "\n" + rd2.GetValue(0).ToString();
                    }

                    string title = "Work Orders under Vendor Number " + textBox1.Text;
                    MessageBox.Show(workOrders, title);

                }
                else
                {
                    //return false;
                    string message = "No Work Order found";
                    string title = "Error";
                    MessageBoxButtons buttons = MessageBoxButtons.OK;
                    MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
                    //HideButtonsAndLabels();


                }
                con.Close();
            }
            
        }
    }
}
