﻿using System;
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
        private SqlDataReader reader;
        private DataTable table;

        public Form1()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString);
            sqlConnection.Open();

            table = new DataTable();

            UpdateComboBox();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            

            if (sqlConnection.State != ConnectionState.Open)
            {
                MessageBox.Show("Подключение не установлено!");  
            }
            
        }

        private void ExecuteRequest(string request)
        {

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
            dataGridView1.Columns.Clear();
            table.Columns.Clear();

            command = new SqlCommand("Select * from " + name, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();
            //DataTable table = new DataTable();
            table.Load(reader);

            dataGridView1.DataSource = table;

            
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            new AddTable().Show();
        }

        private string oldInfo;
        
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            
        }
    }
}
