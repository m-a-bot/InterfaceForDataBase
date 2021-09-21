using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace InterfaceForDataBase
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
        private SqlCommand command;

        public Form1()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
            sqlConnection.Open();

            UpdateComboBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            if (sqlConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Подключение не установлено!");  
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand command = new SqlCommand("CREATE TABLE P1 (ID int, LastName varchar(255))", sqlConnection);
            command.ExecuteNonQuery();
        }

        private void UpdateComboBox()
        {
            command = new SqlCommand("Select * From sys.tables", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            comboBox1.Items.Clear();
            try
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetValue(0));
                }
            } catch
            {

            }
            reader.Close();
        }

        private void GetDataOfTable(string name)
        {
            command = new SqlCommand("Select * from " + name, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            MessageBox.Show(reader.HasRows.ToString());
            reader.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = comboBox1.SelectedIndex;
            if (select != -1)
            {
                string a = (string)comboBox1.Items[select];
                GetDataOfTable(a);
            }
        }
    }
}
