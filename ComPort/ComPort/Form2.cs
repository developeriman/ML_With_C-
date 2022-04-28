using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ComPort
{
    public partial class Form2 : Form
    {
        MySqlConnection myConnection;
        MySqlCommand myCommand;
        MySqlDataAdapter myDataAdapter;
        DataSet myDataSet;
        Form1 objForm1; 

        public void RefreshAndShowOnDataGridView()
        {
            try
            {
                myConnection = new MySqlConnection("server=localhost;username=root; password=; port=3306; database=database01");
                myConnection.Open();
                myCommand = new MySqlCommand("SELECT * FROM `table01`", myConnection);
                myDataAdapter = new MySqlDataAdapter(myCommand);
                myDataSet = new DataSet();
               myDataAdapter.Fill(myDataSet, "Serial Data");
           
                dataGridView1.DataSource = myDataSet;
                dataGridView1.DataMember = "Serial Data";
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; 

           
                myConnection.Close();

            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }


        private void EventFromForm1(object sender, Form1.UpdateDataEventArgs args)
        {
            RefreshAndShowOnDataGridView();
        }
        public Form2(Form1 obj)
        {
            InitializeComponent();
            objForm1 = obj; 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            RefreshAndShowOnDataGridView(); 
            objForm1.UpdateDataEventHandler += EventFromForm1;
        }
    }
}
