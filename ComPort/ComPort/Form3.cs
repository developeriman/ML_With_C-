using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports; 

namespace ComPort
{
    public partial class Form3 : Form
    {
        Form1 objForm1;
        string _1stDataIn;
        string _2ndDataIn; 
        public Form3(Form1 obj)
        {
            InitializeComponent();
            objForm1 = obj; 
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            button_1stOpen.Enabled = true;
            button_1stClose.Enabled = false; 
            button_1stButtonSend.Enabled = false;
            comboBox_1stBaudRate.Text = "9600"; //Default Value; 
            comboBox_1stEndLine.Text = "WriteLine"; //Default Value
            string[] portsLists = SerialPort.GetPortNames(); 
            comboBox_1stComPort.Items.AddRange(portsLists);

            button_2ndOpen.Enabled = true;
            button_2ndClose.Enabled = false;
            button_2ndButtonSend.Enabled = false;
            comboBox_2ndBaudRate.Text = "9600";
            comboBox_2ndEndLine.Text = "WriteLine";
            comboBox_2ndComPort.Items.AddRange(portsLists);
        }

        private void Form3_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    button_1stOpen.Enabled = true;
                    button_1stClose.Enabled = false;
                    button_1stButtonSend.Enabled = false;
                    progressBar_1stStatusBar.Value = 0;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
            if (serialPort2.IsOpen)
            {
                try
                {
                    serialPort2.Close();
                    button_2ndOpen.Enabled = true;
                    button_2ndClose.Enabled = false;
                    button_2ndButtonSend.Enabled = false;
                    progressBar_2ndStatusBar.Value = 0;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }

            objForm1.Show();
        }
        #region 1st COM PORT
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox_stTextReceiver.SelectionStart = richTextBox_stTextReceiver.Text.Length;
            richTextBox_stTextReceiver.ScrollToCaret(); 
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button_1stOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox_1stComPort.Text;
                serialPort1.BaudRate = Convert.ToInt32(comboBox_1stBaudRate.Text);
                serialPort1.Open();

                button_1stOpen.Enabled = false;
                button_1stClose.Enabled = true;
                button_1stButtonSend.Enabled = true;
                progressBar_1stStatusBar.Value = 100;

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message); 
            }
        }

        private void button_1stClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.Close();
                    button_1stOpen.Enabled = true;
                    button_1stClose.Enabled = false;
                    button_1stButtonSend.Enabled = false;
                    progressBar_1stStatusBar.Value = 0; 
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message); 
                }
            }
        }

        private void button_1stButtonSend_Click(object sender, EventArgs e)
        {
            if (comboBox_1stEndLine.Text =="WriteLine") {
                serialPort1.WriteLine(textBox_1stTextSent.Text);
            }else if(comboBox_1stEndLine.Text == "Write")
            {
                serialPort1.Write(textBox_1stTextSent.Text); 
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _1stDataIn = serialPort1.ReadExisting();
            this.Invoke(new EventHandler(_1stShowData));
        }

        private void _1stShowData(object sender, EventArgs e)
        {
            richTextBox_stTextReceiver.Text += _1stDataIn; 
        }
        #endregion
        #region 2nd COM PORT
        private void button_2ndOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort2.PortName = comboBox_2ndComPort.Text;
                serialPort2.BaudRate = Convert.ToInt32(comboBox_2ndBaudRate.Text);
                serialPort2.Open();

                button_2ndOpen.Enabled = false;
                button_2ndClose.Enabled = true;
                button_2ndButtonSend.Enabled = true;
                progressBar_2ndStatusBar.Value = 100;

            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void button_2ndClose_Click(object sender, EventArgs e)
        {
            if (serialPort2.IsOpen)
            {
                try
                {
                    serialPort2.Close();
                    button_2ndOpen.Enabled = true;
                    button_2ndClose.Enabled = false;
                    button_2ndButtonSend.Enabled = false;
                    progressBar_2ndStatusBar.Value = 0;
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void button_2ndButtonSend_Click(object sender, EventArgs e)
        {
            if (comboBox_2ndEndLine.Text == "WriteLine")
            {
                serialPort2.WriteLine(textBox_2ndTextSent.Text);
            }
            else if (comboBox_2ndEndLine.Text == "Write")
            {
                serialPort1.Write(textBox_2ndTextSent.Text);
            }
        }

        private void serialPort2_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _2ndDataIn = serialPort2.ReadExisting();
            this.Invoke(new EventHandler(_2ndShowData)); 
        }

        private void _2ndShowData(object sender, EventArgs e)
        {
            richTextBox_2ndTextReceiver.Text += _2ndDataIn; 
        }

        #endregion

        private void richTextBox2ndTextReceiver_TextChanged(object sender, EventArgs e)
        {
            richTextBox_2ndTextReceiver.SelectionStart = richTextBox_2ndTextReceiver.Text.Length;
            richTextBox_2ndTextReceiver.ScrollToCaret();
        }
    }
}
