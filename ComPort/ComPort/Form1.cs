using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace ComPort
{
    public partial class Form1 : Form
    {

        string sendWith;
        string dataIN;
        int dataInLength;
        int[] dataInDec;


        StreamWriter objStremWriter;

        string pathFile;

        bool state_AppendText = true;

        MySqlConnection myConnection;
        MySqlCommand myCommand;


        #region My Own Method 
        private void SaveDataToTxtFile()
        {
            if (saveToTxtFileToolStripMenuItem.Checked)
            {
                try
                {
                    objStremWriter = new StreamWriter(pathFile, state_AppendText);
                    if (toolStripComboBox_writeLineOrwrite.Text == "WriteLine")
                    {
                        objStremWriter.WriteLine(dataIN);

                    }
                    else if (toolStripComboBox_writeLineOrwrite.Text == "Write")
                    {
                        objStremWriter.Write(dataIN + " ");
                    }
                    objStremWriter.Close();

                }
                catch (Exception err)
                {
                    MessageBox.Show(err.Message);
                }
            }


        }

        private void SaveDataToMySqlDatabase()
        {
            if (savetoMySQLDatabaseToolStripMenuItem.Checked)
            {
                try
                {
                    myConnection = new MySqlConnection("server=localhost;username=root; password=; port=3306; database=database01");
                    myConnection.Open();
                    myCommand = new MySqlCommand(string.Format("INSERT INTO `table01` VALUES('{0}')", dataIN), myConnection);
                    myCommand.ExecuteNonQuery();
                    myConnection.Close();
                    RefreshDataGridviewForm2();
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        #region Custom EventHandler
        #region RX Data Format
        private string RxDataFormat(int[] dataInput)
        {
            string strOut = "";
            if (toolStripComboBox4.Text == "Hex")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element, 16) + "\t";
                }
            }
            else if (toolStripComboBox4.Text == "Decimal")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element) + "\t";
                }
            }

            else if (toolStripComboBox4.Text == "Binary")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToString(element, 2) + "\t";
                }
            }

            else if (toolStripComboBox4.Text == "Char")
            {
                foreach (int element in dataInput)
                {
                    strOut += Convert.ToChar(element) + "\t";
                }
            }
            return strOut;
        }


        #endregion


        public delegate void UpdateDelegate(object sender, UpdateDataEventArgs args);
        public event UpdateDelegate UpdateDataEventHandler;
        public class UpdateDataEventArgs : EventArgs
        {

        }

        protected void RefreshDataGridviewForm2()
        {
            UpdateDataEventArgs args = new UpdateDataEventArgs();
            UpdateDataEventHandler.Invoke(this, args);
        }
        #endregion
        #endregion

        #region GUI Method

        public Form1()
        {
            InitializeComponent();
        }



        private void Form1_Load(object sender, EventArgs e)
        {

            chBoxDtrEnable.Checked = false;
            serialPort1.DtrEnable = false;
            chBoxRTSEnable.Checked = false;
            serialPort1.RtsEnable = false;
            btnSendData.Enabled = true;
            sendWith = @"Both (\r\n)";
            toolStripComboBox3.Text = "BOTTOM";

            toolStripComboBox1.Text = "Add to Old Data";
            toolStripComboBox2.Text = @"Both (\r\n)";

            toolStripComboBox_appendOrOverwriteText.Text = "Append Text";
            toolStripComboBox_writeLineOrwrite.Text = "WriteLine";
            toolStripComboBox_TxDataFormat.Text = "Char";


            pathFile = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

            pathFile += @"\_My Source File\SerialData.txt";

            saveToTxtFileToolStripMenuItem.Checked = false;

            savetoMySQLDatabaseToolStripMenuItem.Checked = false;

            toolStripComboBox4.Text = "Char";
           toolStripComboBox_TxDataFormat.Text = "Char";

            this.toolStripComboBox_TxDataFormat.ComboBox.SelectionChangeCommitted += new System.EventHandler(this.toolStripComboBox_TxDataFormat_SelectionChangeCommitted);

            //C:\Users\bdnhi\source\repos\ComPort\ComPort\_My Source File


        }



        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void oPENToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = Convert.ToInt32(cBoxBaudRate.Text);
                serialPort1.DataBits = Convert.ToInt32(cBoxDataBits.Text);
                serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), cBoxStopBits.Text);
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), cBoxParityBits.Text);
                serialPort1.Open();
                progressBar1.Value = 100;


            }
            catch (Exception err)
            {

                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);


            }
        }


        private void cLOSEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;


            }
        }



        private void btnSendData_Click(object sender, EventArgs e)
        {
            TxSendData(); 
        }

        private void chBoxDtrEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxDtrEnable.Checked)
            {
                serialPort1.DtrEnable = true;
                MessageBox.Show("DTR Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                serialPort1.DtrEnable = false;
            }
        }

        private void chBoxRTSEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxRTSEnable.Checked)
            {
                serialPort1.RtsEnable = true;
                MessageBox.Show("RTS Enable", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);


            }
            else
            {
                serialPort1.RtsEnable = false;
            }
        }




        #region TX Data Format

        private void TxtDataFormat()
        {
            if (toolStripComboBox_TxDataFormat.Text == "Char")
            {
                //Send the data in the textbox via serial port
                serialPort1.Write(tBoxDataOut.Text);
                //Calculate the length of the data sent and then show it
                int dataOUTLength = tBoxDataOut.TextLength;
                lblDataOutLength.Text = String.Format("{0:00}", dataOUTLength);
            }
            else
            {
                //Declare Local Variable 
                string dataOutBuffer;
                int countComma = 0;
                string[] dataPrepareToSend;
                byte[] dataToSend;
                try
                {
                    //Move the data package in the textbox into a variable 
                    dataOutBuffer = tBoxDataOut.Text;

                    //Cont how much comma(,) punctutation in the data variable
                    foreach (char c in dataOutBuffer) if (c == ',') { { countComma++; } }

                    //Create one-dimensional arry (string data type with the lenght based on the countComma)
                    dataPrepareToSend = new string[countComma];

                    //Parsing the data in dataOutBuffer and save it into an array dataPrepareToSend based on comma punctuation
                    countComma = 0; //Reset Value to 0
                    foreach (char c in dataOutBuffer)
                    {
                        if (c != ',')
                        {
                            //Append the data to array of dataPrepareToSend 
                            dataPrepareToSend[countComma] += c;
                        }
                        else
                        {
                            //If a comma finds in the data package, then increase the countComma vaiable. CountComma is using to determine the index of dataPrepareToSend array 
                            countComma++;
                            //Stop foreach process if numbers of countComma equal to the size of dataPrepareToSend
                            if (countComma == dataPrepareToSend.GetLength(0)) { break; }
                        }
                    }
                    //Create one-dimensional array (byte data type )with the length based on the size of dataPrepareToSend
                    dataToSend = new byte[dataPrepareToSend.Length];
                    if (toolStripComboBox_TxDataFormat.Text == "Hex")
                    {
                        //Convert data in string array (dataPrepareToSend) into byte array(dataToSend)
                        for (int a = 0; a < dataPrepareToSend.Length; a++)
                        {
                            dataToSend[a] = Convert.ToByte(dataPrepareToSend[a], 16);
                            //Convert string to an 8-bit unsigned integer with the specified base number
                            //Value 16 mean Hexa
                        }
                    }
                    else if (toolStripComboBox_TxDataFormat.Text == "Binary")
                    {
                        //Convert data in string array (dataPrepareToSend) into byte array(dataToSend)
                        for (int a = 0; a < dataPrepareToSend.Length; a++)
                        {
                            dataToSend[a] = Convert.ToByte(dataPrepareToSend[a], 2);
                            //Convert string to an 8-bit unsigned integer with the specified base number
                            //Value 2 mean Binary
                        }
                    }
                    else if (toolStripComboBox_TxDataFormat.Text == "Decimal")
                    {
                        //Convert data in string array (dataPrepareToSend) into byte array(dataToSend)
                        for (int a = 0; a < dataPrepareToSend.Length; a++)
                        {
                            dataToSend[a] = Convert.ToByte(dataPrepareToSend[a], 10);
                            //Convert string to an 8-bit unsigned integer with the specified base number
                            //Value 10 mean Decimal
                        }
                    }
                    //Send a specified number of bytes to the serial port
                    serialPort1.Write(dataToSend, 0, dataToSend.Length);
                    //Calculate the lenght of data sent and then show it
                    lblDataOutLength.Text = String.Format("{0:00}", dataToSend.Length);
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }
        }

        private void TxSendData()
        {
            if (serialPort1.IsOpen)
            {
                // dataOUT = tBoxDataOut.Text;
                if (sendWith == "None")
                {
                    //serialPort1.Write(dataOUT);
                    TxtDataFormat();
                }
                else if (sendWith == @"Both (\r\n)")
                {
                    //serialPort1.Write(dataOUT + "\r\n");
                    TxtDataFormat();
                    serialPort1.Write("\r\n");
                }

                else if (sendWith == @"New Line (\n)")
                {
                    // serialPort1.Write(dataOUT + "\n");
                    TxtDataFormat();
                    serialPort1.Write("\n");
                }

                else if (sendWith == @"Carriage Return (\r)")
                {
                    TxtDataFormat();
                    serialPort1.Write("\r");
                    //serialPort1.Write(dataOUT + "\r");
                }

            }
            if (toolStripMenuItem.Checked)
            {
                if (tBoxDataOut.Text != "")
                {
                    tBoxDataOut.Text = "";
                }
            }
        }


        private void tBoxDataOut_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                this.doSomething();
                e.Handled = true;
                e.SuppressKeyPress = true;

            }
        }

        private void doSomething()
        {
           TxSendData();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //dataIn = serialPort1.ReadExisting();

            List<int> dataBuffer = new List<int>();
            while (serialPort1.BytesToRead > 0)
            {
                try
                {
                    dataBuffer.Add(serialPort1.ReadByte());
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            }

            dataInLength = dataBuffer.Count();
            dataInDec = new int[dataInLength];
            dataInDec = dataBuffer.ToArray();

            this.Invoke(new EventHandler(ShowData));
        }

        private void ShowData(object sender, EventArgs e)
        {
            // int dataInLength = dataIn.Length;
            dataIN = RxDataFormat(dataInDec);
            lblDataInLength.Text = String.Format("{0:00}", dataInLength);
            if (toolStripComboBox1.Text == "Always Update")
            {
                tBoxDataIN.Text = dataIN;

            }
            else if (toolStripComboBox1.Text == "Add to Old Data")
            {

                if (toolStripComboBox3.Text == "TOP")
                {
                    //tBoxDataIN.Text += dataIN; 
                    tBoxDataIN.Text = tBoxDataIN.Text.Insert(0, dataIN);

                }
                else if (toolStripComboBox3.Text == "BUTTON")
                {

                    tBoxDataIN.Text += dataIN;
                }

            }

            SaveDataToTxtFile();

            SaveDataToMySqlDatabase();


        }


        private void receivToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cBoxBaudRate_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Created by Imam hossain Iman", "Creator", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripComboBox2_DropDownClose(object sender, EventArgs e)
        {
            if (toolStripComboBox2.Text == "None")
            {
                sendWith = "None";
            }
            else if (toolStripComboBox2.Text == @"Both (\r\n)")
            {

                sendWith = @"Both (\r\n)";
            }
            else if (toolStripComboBox2.Text == @"New Line (\n)")
            {
                sendWith = @"New Line (\n)";
            }
            else if (toolStripComboBox2.Text == @"Carrige Return (\r)")
            {
                sendWith = @"Carrige Return (\r)";
            }
        }

        private void toolStripComboBox2_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBox2.Text == "None")
            {
                sendWith = "None";
            }
            else if (toolStripComboBox2.Text == @"Both(\r\n)")
            {

                sendWith = @"Both (\r\n)";
            }
            else if (toolStripComboBox2.Text == @"New Line (\n)")
            {
                sendWith = @"New Line (\n)";
            }
            else if (toolStripComboBox2.Text == @"Carrige Return (\r)")
            {
                sendWith = @"Carrige Return (\r)";
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            groupBox12.Width = panel1.Width - 285;
            groupBox12.Height = panel1.Height - 76;
            tBoxDataIN.Height = panel1.Height - 105;
            tBoxDataIN.Height = panel1.Height - 130;

        }

        private void toolStripComboBox_appendOrOverwriteText_DropDownClosed(object sender, EventArgs e)
        {
            if (toolStripComboBox_appendOrOverwriteText.Text == "Append Text")
            {
                state_AppendText = true;
            }
            else
            {
                state_AppendText = false;
            }

        }


        private void showDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 objForm2 = new Form2(this);
            objForm2.Show();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form3 objForm3 = new Form3(this);
            objForm3.Show();
            this.Hide();
        }

        private void toolStripComboBox_TxDataFormat_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //Every time selected different Tx data format, then delete all contents in ther textbox data out 
            tBoxDataOut.Clear();

            string message = "If you are not using char data format, append the comma (,) after each byte data. Otherwise, the byte data will ingnore. \n" +
                "Example :\t255,-> One byte data \n" +
                "\t255, 128, 140, ->Two or more byte data \n" +
                "\t120, 144, 189 -> The 189 will ignore cause has no comma (,)";
            MessageBox.Show(message, "Warning", MessageBoxButtons.OK);

        }


        private void multiplePortsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void cBoxCOMPORT_DropDown(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.Clear();

            cBoxCOMPORT.Items.AddRange(ports);
        }

        #endregion
        #endregion
    }
}