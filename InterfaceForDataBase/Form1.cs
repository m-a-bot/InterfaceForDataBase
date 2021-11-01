using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;

namespace InterfaceForDataBase
{
    
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection;
        private SqlCommand command;
        private SqlDataReader reader;
        
        private int IndexNewRow = -1;

        private string NameTable = "";
        private List<string> NamesHeaders = new List<string>();
        int countColumns = 0;
        int countRows;



        public Form1()
        {
            InitializeComponent();
            // Connection to the database
            string connectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
            Connection(connectionString);
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
                MessageBox.Show("Подключение не установлено!");  
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

        private void UpdateTable(string name)
        {
            if (name != "")
            {
                dataGridView1.Columns.Clear();
                NamesHeaders.Clear();

                command = new SqlCommand("Select * from " + name, sqlConnection);
                reader = command.ExecuteReader();

                countColumns = reader.FieldCount;
                countRows = 0;
                dataGridView1.ColumnCount = countColumns + 1;


                // Set the column header names
                for (int i = 0; i < countColumns; i++)
                {
                    string header = reader.GetName(i);
                    dataGridView1.Columns[i].Name = header;
                    NamesHeaders.Add(header);
                }

                dataGridView1.Columns[countColumns].Name = "";

                // Set values of table
                object[] ValueItems = new object[countColumns];
                while (reader.Read())
                {
                    for (int i = 0; i < countColumns; i++)
                        ValueItems[i] = reader.GetValue(i);
                    dataGridView1.Rows.Add(ValueItems);

                    dataGridView1[countColumns, countRows] = new DataGridViewLinkCell() { Value = "delete" };
                    countRows++;
                }
                reader.Close();
            }
        }

         
        //selecting a certain table
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = comboBox1.SelectedIndex;
            if (select != -1)
            {
                NameTable = (string)comboBox1.Items[select];
                UpdateTable(NameTable);
            }
        }

       
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex != IndexNewRow)
            {
                dataGridView1[countColumns, e.RowIndex] = new DataGridViewLinkCell() { Value = "update" };
            }

        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.Index - 2 >= 0)
                dataGridView1[0, e.Row.Index - 1].Value = (int)(dataGridView1[0, e.Row.Index - 2].Value) + 1;
            else
                dataGridView1[0, e.Row.Index - 1].Value = 1;
            dataGridView1[countColumns, IndexNewRow = e.Row.Index - 1] = new DataGridViewLinkCell{ Value = "insert" };
            dataGridView1.AllowUserToAddRows = false;
        }

        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == countColumns)
            {
                string value = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                int id = (int)dataGridView1[0, e.RowIndex].Value;
                
                if (value is "delete")
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    command = new SqlCommand($"delete from {NameTable} where ID = {id}", sqlConnection);
                    command.ExecuteNonQuery();
                }
                
                List<string> v = new List<string>();
                string num = "0123456789";
                for (int i = 1; i < dataGridView1.Rows[e.RowIndex].Cells.Count; i++)
                {
                    string r = (string)dataGridView1.Rows[e.RowIndex].Cells[i].Value;
                    v.Add(num.Contains(r[0]) ? r : r = "'" + r + "'");
                }
                string values = string.Join(", ", v);
                string h = string.Join(", ", NamesHeaders);
                if (value is "update")
                {
                    //dataGridView1[countColumns, e.RowIndex] = new DataGridViewLinkCell() { Value = "delete" };
                    //string couples = "";
                    //for (int i=0; i<dataGridView1.Rows[e.RowIndex].Cells.Count; i++)
                    //{

                    //}
                    //command = new SqlCommand($"update {NameTable} where ID = {id}", sqlConnection);
                    //command.ExecuteNonQuery();
                    /*
                     update Persons1
                        set LastName='rrrrsss', Type='sd'
                        where ID = 3
                     */
                }
                if (value is "insert")
                {
                    //dataGridView1[e.ColumnIndex, e.RowIndex].Value = "delete";
                    dataGridView1.AllowUserToAddRows = true;
                    IndexNewRow = -1;
                    command = new SqlCommand($"insert into {NameTable} ({h}) values ({values})", sqlConnection);
                    command.ExecuteNonQuery();

                    // insert into Persons1 values ('p1', 1223, 'r1rr') 
                }
                UpdateTable(NameTable);
            }
        }  
    }
}
