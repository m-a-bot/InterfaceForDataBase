using System;
using System.IO;
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
        private SqlDataAdapter adapter;
        private SqlCommandBuilder builder;
        private SqlDataReader reader;
        private DataTable table;

        private int IndexNewRow = -1;

        private string NameTable = "";
        int countColumns = 0;
        int countRows;



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
                MessageBox.Show("Подключение не установлено!");  
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

            adapter = new SqlDataAdapter("Select * from " + name, sqlConnection);
            builder = new SqlCommandBuilder(adapter);

            builder.GetInsertCommand();
            builder.GetUpdateCommand();
            builder.GetDeleteCommand();

            command = new SqlCommand("Select * from " + name, sqlConnection);
            reader = command.ExecuteReader();

            countColumns = reader.FieldCount;
            countRows = 0;
            dataGridView1.ColumnCount = countColumns+1;
            

            // Set the column header names
            for (int i = 0; i < countColumns; i++)
            {
                string header = reader.GetName(i);
                dataGridView1.Columns[i].Name = header;
                table.Columns.Add(header);
            }
                
            dataGridView1.Columns[countColumns].Name = "";

            // Set values of table
            object[] ValueItems = new object[countColumns];
            while(reader.Read())
            {
                for (int i = 0; i < countColumns; i++)
                    ValueItems[i] = reader.GetValue(i);
                dataGridView1.Rows.Add(ValueItems);
                table.Rows.Add(ValueItems);

                dataGridView1[countColumns, countRows] = new DataGridViewLinkCell() { Value="delete"};
                countRows++;
            }
            reader.Close();
        }

         
        //selecting a certain table
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int select = comboBox1.SelectedIndex;
            if (select != -1)
            {
                NameTable = (string)comboBox1.Items[select];
                GetDataOfTable(NameTable);
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
            dataGridView1[countColumns, IndexNewRow = e.Row.Index - 1] = new DataGridViewLinkCell{ Value = "insert" };
            dataGridView1.AllowUserToAddRows = false;
        }

        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == countColumns)
            {
                string value = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
                if (value is "delete")
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    table.Rows.RemoveAt(e.RowIndex);
                    
                    
                }
                if (value is "update")
                {
                    DataGridViewCellCollection collectionRow = dataGridView1.Rows[e.RowIndex].Cells;
                    //collectionRow[e.ColumnIndex].Value = "delete";
                    dataGridView1[countColumns, e.RowIndex] = new DataGridViewLinkCell() { Value = "delete" };
                    SetRow(table.Rows[e.RowIndex], collectionRow, e.RowIndex); 
                }
                if (value is "insert")
                {
                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = "delete";
                    dataGridView1.AllowUserToAddRows = true;
                    IndexNewRow = -1;
                    DataRow dataRow = table.NewRow();
                    DataGridViewCellCollection collectionRow = dataGridView1.Rows[e.RowIndex].Cells;
                    table.Rows.Add(SetRow(dataRow,collectionRow, e.RowIndex));

                }

            }
        }

        private DataRow SetRow(DataRow dataRow, DataGridViewCellCollection collectionRow, int indexRow)
        {
            for (int i=0; i< collectionRow.Count - 1; i++)
            {
                dataRow[i] = collectionRow[i].Value;
            }
            return dataRow;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StreamWriter writer = new StreamWriter("res.txt");
            foreach(DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                    writer.Write(row[i] + "\t");
                writer.WriteLine();
            }
            writer.Close();
            
        }
        
    }
}
