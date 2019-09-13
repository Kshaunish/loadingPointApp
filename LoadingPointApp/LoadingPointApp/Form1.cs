using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.IO.Ports; // for serial communication
using System.Threading;

namespace LoadingPointApp
{
    public partial class Form1 : Form
    {
        SqlConnection con = new SqlConnection("Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=C:\\Users\\kshau\\source\\repos\\LoadingPointApp\\LoadingPointApp\\Database1.mdf;Integrated Security = True");

        static string curDate = DateTime.Now.ToString("dd/MM/yyyy"); // today's date in dd-mm-yyyy format string
        string currentDate = curDate.Substring(0, 2) + curDate.Substring(3, 2) + curDate.Substring(6, 4);

        int trip_limit = 99;

        //int initialChalaanNumber = 1000;

        public Form1()
        {

            InitializeComponent();
            textBox6.Text = "";
            textBox6.Hide();
            label7.Hide();
            button4.Hide();
            label8.Hide();
            label1.Text = "RFID Number";
            label15.Hide();
            textBox89.Hide();
            textBox99.Hide();

            button5.Hide(); //clear button


            label13.Text = "Disconnected";

            button8.Hide();

            foreach (String s in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(s);
            }


        }

        // serial port communication
        public System.IO.Ports.SerialPort sport;
        public void serialport_connect(String port, int baudrate, Parity parity, int databits, StopBits stopbits)
        {

            sport = new System.IO.Ports.SerialPort(
            port, baudrate, parity, databits, stopbits);
            try
            {
                label13.Text = "";
                sport.Open();
                button6.Enabled = false;
                label13.Text = "Connected";
                sport.DataReceived += new SerialDataReceivedEventHandler(sport_DataReceived);

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString(), "\n Error"); }
        }
        ////uncomment for default
        private void sport_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            //textBox1.Text = sport.ReadExisting();
            string rfid_data = sport.ReadExisting();
            char[] asciiBytes = rfid_data.ToCharArray();
            string rfid_no = "";


            //textBox1.Text = "";// comment this


            foreach (char ch in asciiBytes)
            {
                int i = (int)ch;
                string myString = i.ToString();
                rfid_no = rfid_no + i;
            }
            textBox1.Text = rfid_no;

            //sport.Close();// comment
            //label13.Text = "Disconnected"; //comment
            //if (!sport.IsOpen) // comment this
            //{
            //    button7.Enabled = false;
            //    button6.Enabled = true;
            //}
        }
        
        private void Button6_Click(object sender, EventArgs e) // Connect button
        {
            button6.Enabled = false;
            button7.Enabled = true;
            string port = comboBox1.Text.ToString();
            //string port = "COM6";                                                    //  PORT == COM6
            int baudrate = Convert.ToInt32(9600);                                   // BAUD RATE
            Parity parity = (Parity)Enum.Parse(typeof(Parity), "None");            // PARITY
            int databits = Convert.ToInt32(8);                                    // DATA BITS
            StopBits stopbits = (StopBits)Enum.Parse(typeof(StopBits), "One");   // STOP BITS
            serialport_connect(port, baudrate, parity, databits, stopbits);
        }
        private void Button7_Click(object sender, EventArgs e) // Disconnect button
        {
            if (sport.IsOpen)
            {
                label13.Text = "Disconnected";
                sport.Close();
                button6.Enabled = true;
                button7.Enabled = false;
            }
        }
        // end of serial port communication

        private void Button1_Click(object sender, EventArgs e)
        {
            string message = "Please fill all input fields!";

            string title = "Warning";
            MessageBoxButtons buttons = MessageBoxButtons.OK;

            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "" || textBox8.Text == "" || textBox9.Text == "" || textBox10.Text == "" || textBox11.Text == "")
            {
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }

            if (CheckDeliveryOrderNumberInDB() == false && textBox2.Text != "")
            {
                MessageBox.Show("Check Delivery Order Number", title, buttons, MessageBoxIcon.Warning);
            }

            if (CheckVendorNumberInDB() == false && textBox3.Text != "")
            {
                MessageBox.Show("Check Vendor Number", title, buttons, MessageBoxIcon.Warning);
            }

            if (CheckWordOrderInDB() == false && textBox4.Text != "")
            {
                MessageBox.Show("Check Work Order", title, buttons, MessageBoxIcon.Warning);
            }
            if (CheckDeliveryOrderNumberInDB() == true && CheckVendorNumberInDB() == true && CheckWordOrderInDB() == true)
            {
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "" || textBox5.Text == "")
                {
                    MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
                }
                else
                {
                    bool presenceOfRfidNumber = CheckRFIDinDB();
                    string cat = presenceOfRfidNumber.ToString();

                    // calculation of net weight
                    string gross = textBox10.Text;
                    string tare = textBox11.Text;
                    int gross_wt, tare_wt, net_wt;
                    int.TryParse(gross, out gross_wt);
                    int.TryParse(tare, out tare_wt);
                    net_wt = gross_wt - tare_wt; // net weight

                    // material number
                    string material_no = textBox8.Text;
                    string vehicle_no = textBox9.Text;

                    string chalaan = GenerateChalaan();


                    if (presenceOfRfidNumber == false)
                    {
                        con.Open();
                        SqlCommand cmd1 = new SqlCommand("insert into RFID_Records values(" + textBox1.Text + ",1)", con);
                        SqlDataReader rd1 = cmd1.ExecuteReader();
                        con.Close();

                        con.Open();
                        
                        SqlCommand cmd = new SqlCommand("insert into Invoice values(1,'" + chalaan + "'," + textBox1.Text + ",'" + currentDate + "'," + textBox2.Text + "," + textBox3.Text + ",'" + textBox4.Text + "'," + textBox5.Text + ",'" + material_no + "','" + vehicle_no + "'," + textBox10.Text + "," + textBox11.Text + "," + "@NW)", con);
                        cmd.Parameters.AddWithValue("@NW", net_wt);
                        SqlDataReader rd = cmd.ExecuteReader();
                        con.Close();
                        MessageBox.Show("New RFID registered and added to transaction list", "Success", buttons, MessageBoxIcon.Information);

                    }
                    else // if rfid is present in rfid_records
                    {
                        // get the existing trip_number from invoice table
                        con.Open();
                        SqlCommand cmd1 = new SqlCommand("select max(trip_no) from Invoice where c_no=@ID", con);
                        cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                        string max_trip_no = "";
                        SqlDataReader rd1 = cmd1.ExecuteReader();
                        while (rd1.Read())
                        {
                            max_trip_no = rd1.GetValue(0).ToString();
                        }
                        con.Close();

                        int updated_trip_no;
                        int.TryParse(max_trip_no, out updated_trip_no);

                        if (updated_trip_no == trip_limit)
                        {

                            // get issue_no from RFID_Records table
                            con.Open();
                            SqlCommand cmd6 = new SqlCommand("select Issue_Number from RFID_Records where Chalaan_Number=@ID", con);
                            cmd6.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                            string issue_no = "";
                            SqlDataReader rd6 = cmd6.ExecuteReader();
                            while (rd6.Read())
                            {
                                issue_no = rd6.GetValue(0).ToString();
                            }
                            con.Close();
                            int updated_issue_no;
                            int.TryParse(issue_no, out updated_issue_no);

                            updated_issue_no = updated_issue_no + 1;

                            // update issue number in rfid_records
                            con.Open();
                            SqlCommand cmdUpdate = new SqlCommand("update RFID_Records set Issue_Number=@IN where Chalaan_Number=@ID", con);
                            cmdUpdate.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
                            cmdUpdate.Parameters.AddWithValue("@IN", updated_issue_no);
                            SqlDataReader rdUpdate = cmdUpdate.ExecuteReader();
                            con.Close();

                            // insert existing rfid with trip_number = 1 and current_date
                            con.Open();
                            
                            SqlCommand cmdNewTripNumber = new SqlCommand("insert into Invoice values(1,'" + chalaan + "'," + textBox1.Text + ",'" + currentDate + "'," + textBox2.Text + "," + textBox3.Text + ",'" + textBox4.Text + "'," + textBox5.Text + ",'" + material_no + "','" + vehicle_no + "'," + textBox10.Text + "," + textBox11.Text + "," + "@NW)", con);
                            cmdNewTripNumber.Parameters.AddWithValue("@NW", net_wt);
                            SqlDataReader rdNewTripNumber = cmdNewTripNumber.ExecuteReader();
                            con.Close();
                            MessageBox.Show("Issue Number updated for RFID and added to transaction list", "Success", buttons, MessageBoxIcon.Information);

                        }
                        else
                        {
                            updated_trip_no = updated_trip_no + 1;
                            con.Open();
                            
                            SqlCommand cmdExisting = new SqlCommand("insert into Invoice values(@ID,'" + chalaan + "'," + textBox1.Text + ",'" + currentDate + "'," + textBox2.Text + "," + textBox3.Text + ",'" + textBox4.Text + "'," + textBox5.Text + ",'" + material_no + "','" + vehicle_no + "'," + textBox10.Text + "," + textBox11.Text + "," + "@NW)", con);
                            
                            cmdExisting.Parameters.AddWithValue("@NW", net_wt);
                            cmdExisting.Parameters.AddWithValue("@ID", updated_trip_no);
                            SqlDataReader rdExisting = cmdExisting.ExecuteReader();
                            con.Close();
                            MessageBox.Show("Trip number updated for existing RFID", "Success", buttons, MessageBoxIcon.Information);
                        }

                    }
                }
            }
        }
        private bool CheckRFIDinDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from RFID_Records where Chalaan_Number='" + textBox1.Text + "'", con);
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

        private bool CheckDeliveryOrderNumberInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Delivery_Orders where Delivery_Order_Number='" + textBox2.Text + "'", con);
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

        private string GenerateChalaan()
        {
            string curDate = DateTime.Now.ToString("dd/MM/yyyy"); // today's date in dd-mm-yyyy format string
            string currentDate = curDate.Substring(0, 2) + curDate.Substring(3, 2) + curDate.Substring(6, 4);
            //string curTime = DateTime.Now.ToShortTimeString(); // do not uncomment

            string curTime = DateTime.Now.ToString("HH:mm:ss"); //"h:mm:ss"
            string currentTime = curTime.Substring(0, 2) + curTime.Substring(3, 2) + curTime.Substring(6, 2);

            // string chalaan_no = currentDate + currentTime; // uncomment this first

            string chalaan_no;
            string[] strArr = { curDate.Substring(0, 2), curDate.Substring(3, 2), curDate.Substring(6, 4), curTime.Substring(0, 2), curTime.Substring(3, 2), curTime.Substring(6, 2) };
            int chal_no = 0;
            foreach( string str in strArr)
            {
                int StringToInt = 0;
                int.TryParse(str, out StringToInt);
                chal_no = chal_no + StringToInt;
            }
            chalaan_no = chal_no.ToString() + curDate.Substring(0, 2) + curDate.Substring(3, 2);


            return chalaan_no;
        }

        private bool CheckVendorNumberInDB()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Vendors where Vendor_Number='" + textBox3.Text + "'", con);
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
            SqlCommand cmd = new SqlCommand("select * from Vendors where Work_Order='" + textBox4.Text + "'", con);
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
        private void Button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
           
        }
        private void Button3_Click(object sender, EventArgs e)
        {
            label2.Text = "Search Transaction List";
            HideButtonsAndLabels();
            button3.Hide();
            button4.Show();
            button8.Show(); //Go button
            label15.Hide();
            textBox89.Hide();
            textBox99.Hide();

            button5.Show(); //clear

        }
        private void HideButtonsAndLabels()
        {
 
            label1.Text = "RFID Number"; // change this to RFID Number, previously Chalaan Number

            textBox6.Hide();
            label7.Hide();

            label8.Hide();


            button1.Hide();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();
            textBox5.Hide();
            label3.Hide();
            label4.Hide();
            label5.Hide();
            label6.Hide();

            //textBox7.Hide(); // date
            textBox8.Hide();
            textBox9.Hide();
            textBox10.Hide();
            textBox11.Hide();

            //label8.Hide(); // date
            label9.Hide();
            label10.Hide();
            label11.Hide();
            label12.Hide();

        }
        private void GetDetails()
        {


            // get the maximum trip_number from invoice table
            con.Open();
            SqlCommand cmd = new SqlCommand("select max(trip_no) from Invoice where c_no=@ID", con);
            cmd.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
            string max_trip_no = "";
            SqlDataReader rd = cmd.ExecuteReader();
            while (rd.Read())
            {
                max_trip_no = rd.GetValue(0).ToString();
            }
            con.Close();
            int maximum_trip_no;
            int.TryParse(max_trip_no, out maximum_trip_no);



            con.Open();
           
            //SqlCommand cmd1 = new SqlCommand("select chal_no,c_no,del_no,v_no,w_o,p_no,material_no,vehicle_no,gross_wt,tare_wt,net_wt,t_date from Invoice where chal_no=@ID", con);
            SqlCommand cmd1 = new SqlCommand("select chal_no,c_no,del_no,v_no,w_o,p_no,material_no,vehicle_no,gross_wt,tare_wt,net_wt,t_date from Invoice where c_no=@ID and trip_no=@TN", con);


            cmd1.Parameters.AddWithValue("@ID", int.Parse(textBox1.Text));
            cmd1.Parameters.AddWithValue("@TN", maximum_trip_no);

            //cmd1.Parameters.AddWithValue("@ID", textBox1.Text);
            ////cmd1.Parameters.AddWithValue("@TN", int.Parse(textBox6.Text));

            SqlDataReader rd2 = cmd1.ExecuteReader();
            if (rd2.HasRows)
            {
                while (rd2.Read())
                {
                    textBox1.Text = rd2.GetValue(1).ToString(); ////change to getValue(1), previously 0
                    //label1.Show();


                    textBox6.Text = rd2.GetValue(0).ToString(); //change to getValue(0), previously 1
                    textBox6.Show();
                    label7.Text = "Chalaan Number"; // change to Chalaan Number, previously RFID
                    label7.Show();

                    textBox2.Text = rd2.GetValue(2).ToString();
                    label3.Show();
                    textBox2.Show();
                    textBox3.Text = rd2.GetValue(3).ToString();
                    label4.Show();
                    textBox3.Show();
                    textBox4.Text = rd2.GetValue(4).ToString();
                    label5.Show();
                    textBox4.Show();
                    textBox5.Text = rd2.GetValue(5).ToString();
                    label6.Show();
                    textBox5.Show();

                    textBox8.Text = rd2.GetValue(6).ToString();
                    label9.Show();
                    textBox8.Show();

                    textBox9.Text = rd2.GetValue(7).ToString();
                    label10.Show();
                    textBox9.Show();

                    textBox10.Text = rd2.GetValue(8).ToString();
                    label11.Show();
                    textBox10.Show();

                    textBox11.Text = rd2.GetValue(9).ToString();
                    label12.Show();
                    textBox11.Show();

                    label15.Show();
                    textBox89.Text = rd2.GetValue(10).ToString();
                    textBox89.Show();

                    label8.Show();
                    textBox99.Text = rd2.GetValue(11).ToString().Substring(0,2)+"-"+ rd2.GetValue(11).ToString().Substring(2, 2)+"-"+ rd2.GetValue(11).ToString().Substring(4, 4);
                    textBox99.Show();

                }
            }
            else
            {
                //return false;
                string message = "Transaction not found";
                string title = "Error";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Error);
                HideButtonsAndLabels();


            }

            
            con.Close();
        }
        private void TextBox6_TextChanged(object sender, EventArgs e)
        {
            
            if (textBox6.Text == "")
            {
                HideButtonsAndLabels();
            }
        }
        private void Button4_Click(object sender, EventArgs e)
        {
            label1.Text = "RFID Number";

            textBox6.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Hide();
            label7.Hide();
            button1.Show();
            textBox2.Show();
            textBox3.Show();
            textBox4.Show();
            textBox5.Show();
            label3.Show();
            label4.Show();
            label5.Show();
            label6.Show();
            button3.Show();
            button4.Hide();
            label2.Text = "Log Transaction";

            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";


            //textBox7.Show(); // date
            textBox8.Show();
            textBox9.Show();
            textBox10.Show();
            textBox11.Show();

            //label8.Show(); // date
            label9.Show();
            label10.Show();
            label11.Show();
            label12.Show();

            label8.Hide();
            

            button8.Hide();

            label1.Text = "RFID Number";
            label15.Hide();
            textBox89.Hide();
            textBox99.Hide();


            button5.Hide();


        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        
        private void Label8_Click(object sender, EventArgs e)
        {

        }

        private void Button8_Click(object sender, EventArgs e)
        {
            label1.Text = "RFID Number"; // change this to RFID Number, Previously Chalaan Number
            //if (textBox1.Text != "" && textBox6.Text != "" && comboBox1.Text != "" && comboBox2.Text != "" && comboBox3.Text != "")
            if (textBox1.Text != "")
            {
                GetDetails();
            }
            else
            {
                string message = "Please fill the RFID Number!";
                string title = "Warning";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, title, buttons, MessageBoxIcon.Warning);
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Button9_Click(object sender, EventArgs e) // Launches Admin Mode in a separate window
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";

            textBox8.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";

            textBox89.Text = "";
            textBox89.Hide();
            textBox99.Hide();
            textBox99.Text = "";
            label15.Hide();

            HideButtonsAndLabels();

        }
    }
}
