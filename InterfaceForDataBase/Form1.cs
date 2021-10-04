using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;


namespace InterfaceForDataBase
{
    
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
        private SqlCommand command;
        private SqlDataReader reader;
        private DataTable table;

        private bool newRowAdding = false;

        private string NameTable = "";
        private string NameHeaderColumn = "";
        int countColumns = 0;


        public Form1()
        {
            InitializeComponent();
            // Connection to the database
            string connectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
            Connection(connectionString);
            

            table = new DataTable();

            UpdateComboBox();
        }

        private void Connection(string connectionString)
        {
            sqlConnection = new SqlConnection(connectionString);

            // Open the connection
            sqlConnection.Open();
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
        // Output all names of tables
        private void UpdateComboBox()
        {
            command = new SqlCommand("Select * From sys.tables", sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            comboBox1.Items.Clear();
            try
            {
                while (reader.Read())
                {
                    comboBox1.Items.Add(reader.GetString(0));
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
            reader = command.ExecuteReader();

            countColumns = reader.FieldCount;
            int countRows = 0;

            dataGridView1.ColumnCount = countColumns+1;

            // Set the column header names
            for (int i = 0; i < countColumns; i++)
                dataGridView1.Columns[i].Name = reader.GetName(i);
            dataGridView1.Columns[countColumns].Name = "";

            object[] ValueItems = new object[countColumns];
            while(reader.Read())
            {
                for (int i = 0; i < countColumns; i++)
                    ValueItems[i] = reader.GetValue(i);
                dataGridView1.Rows.Add(ValueItems);

                DataGridViewLinkCell link = new DataGridViewLinkCell();
                link.Value = "delete";
                dataGridView1[countColumns, countRows] = link;
                countRows++;
            }

            reader.Close();
        }

        /* 
         
         */
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = comboBox1.SelectedIndex;
            if (select != -1)
            {
                NameTable = (string)comboBox1.Items[select];
                GetDataOfTable(NameTable);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new AddTable().Show();
        }

        
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(!newRowAdding)
            {
                DataGridViewLinkCell link = new DataGridViewLinkCell();
                link.Value = "update";
                dataGridView1[countColumns, e.RowIndex] = link;
            }

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (!newRowAdding)
            {
                newRowAdding = true;
                DataGridViewLinkCell link = new DataGridViewLinkCell
                {
                    Value = "insert"
                };
                dataGridView1[countColumns, e.Row.Index - 1] = link;
            }
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == countColumns)
            {
                object value = dataGridView1[e.ColumnIndex, e.RowIndex].Value;
                if (value is "delete")
                {
                    MessageBox.Show("1");
                }
                if (value is "update")
                {
                    MessageBox.Show("2");
                }
                if (value is "insert")
                {
                    MessageBox.Show("3");
                }
            }
        }
    }
}
