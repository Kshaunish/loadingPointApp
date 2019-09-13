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
    public partial class Form4 : Form
    {
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=C:\\Users\\kshau\\source\\repos\\LoadingPointApp\\LoadingPointApp\\Database1.mdf;Integrated Security = True");
        static string curDate = DateTime.Now.ToString("dd/MM/yyyy"); // today's date in dd-mm-yyyy format string
        string currentDate = curDate.Substring(0, 2) + curDate.Substring(3, 2) + curDate.Substring(6, 4);
        public Form4()
        {
            InitializeComponent();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();
            label6.Hide();
            textBox5.Hide();
        }

        private void Label4_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox2.Hide();
                textBox3.Hide();
                textBox4.Hide();
                label6.Hide();
                textBox5.Hide();
            }
            else
            {
                textBox2.Show();
                textBox3.Show();
                textBox4.Show();
            }
        }
        private bool CheckDeliveryOrderNumberInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Delivery_Orders where Delivery_Order_Number='" + textBox1.Text + "'", con);
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
        private bool CheckDeliveryOrderNumberInInvoice()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Invoice where del_no='" + textBox1.Text + "'", con);
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
            if (CheckDeliveryOrderNumberInDB())
            {
                string message = "Delivery Order already exists";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("insert into Delivery_Orders values(@ID,'" + textBox2.Text + "','" + currentDate + "','" + textBox3.Text + "','" + textBox4.Text + "')", con);
                cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd1 = cmd1.ExecuteReader();
                string message = "New Delivery Order added";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";

                con.Close();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (CheckDeliveryOrderNumberInInvoice())
            {
                string message = "Delivery Order is Active! Cannot delete";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
            else
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("delete from Delivery_Orders where Delivery_Order_Number=@ID", con);
                cmd2.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd2.ExecuteReader();
                string message = "Deleted the record";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                con.Close();
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (CheckDeliveryOrderNumberInInvoice())
            {
                string message = "Delivery Order is Active! Cannot update";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
            else if(CheckDeliveryOrderNumberInDB() && CheckDeliveryOrderNumberInInvoice()==false)
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("update Delivery_Orders set Order_Name='" + textBox2.Text + "', Source='" + textBox3.Text + "', Destination='"+ textBox4.Text +"' where Delivery_Order_Number=@ID", con);
                cmd2.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd2.ExecuteReader();
                string message = "Updated the record";
                string title = "Success";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Information);
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
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

        private void Form4_Load(object sender, EventArgs e)
        {
            
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                string message = "Enter Delivery Number";
                string title = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }

            else
            {
                label6.Show();
                textBox5.Show();
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select Order_Name,Order_Date,Source,Destination from Delivery_Orders where Delivery_Order_Number=@ID", con);
                cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                SqlDataReader rd2 = cmd1.ExecuteReader();
                if (rd2.HasRows)
                {
                    while (rd2.Read())
                    {
                        textBox2.Text = rd2.GetValue(0).ToString();
                        //label3.Show();
                        textBox2.Show();
                        textBox3.Text = rd2.GetValue(2).ToString();
                        //label4.Show();
                        textBox3.Show();
                        textBox4.Text = rd2.GetValue(3).ToString();
                        //label5.Show();
                        textBox4.Show();


                        string dateFromDB = rd2.GetValue(1).ToString();
                        //MessageBox.Show(dateFromDB);
                        if (dateFromDB.Length == 8)
                        {
                            string FinalDate = dateFromDB.Substring(0, 1) + "-" + dateFromDB.Substring(1, 2) + "-" + dateFromDB.Substring(3, 4);
                            textBox5.Text = dateFromDB;
                        }
                        else
                        {
                            string FinalDate = dateFromDB.Substring(0, 2) + "-" + dateFromDB.Substring(2, 2) + "-" + dateFromDB.Substring(4, 4);
                            textBox5.Text = FinalDate;
                        }

                        textBox5.Show();

                    }
                }
                else
                {
                    //return false;
                    string message = "Delivery Order not found";
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
