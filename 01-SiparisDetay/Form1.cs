using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _01_SiparisDetay
{
    public partial class Form1 : Form
    {
        //1.Soru - ComboBoxtan çalışan seçip, o çalışanın siparişlerini listelediğimiz. ve sonrasında da o siparişin detay bilgilerine ulaşabildiğimiz bir form geliştiriniz. Çalışanların comboBox'a listelenmesinde öncede oluşturduğunuz bir view'ı kullanınız. Seçili çalışanın siparişlerinin listelenmesinde ise önceden yazılmış bir Stored Procedure'ü kullanınız. 

        private string _connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Northwind;Trusted_Connection=True";
        private SqlConnection _connection;
        private SqlCommand _command;
        private SqlDataAdapter _adapter;
        private string _query;

        public Form1()
        {
            InitializeComponent();
            _connection = new SqlConnection(_connectionString);
            _command = _connection.CreateCommand();
            _adapter = new SqlDataAdapter(_command);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            ComboBoxPersonels();

            ComboBoxCustomers();

        }

        private void ComboBoxCustomers()
        {
            _query = "SELECT * FROM Customers";

            DataTable table = new DataTable();
            _command.CommandText = _query;

            _adapter.Fill(table);


            cmbCustomers.DisplayMember = "CompanyName";
            cmbCustomers.ValueMember = "CustomerID";

            cmbCustomers.DataSource = null;
            cmbCustomers.DataSource = table;
        }

        /// <summary>
        /// EmployeeFullNames VIEW inden combobox a personel isimleri çekiliyor
        /// </summary>
        private void ComboBoxPersonels()
        {
            _query = "SELECT * FROM EmployeeFullNames";

            DataTable table = new DataTable();
            _command.CommandText = _query;

            _adapter.Fill(table);

            cmbEmployee.DisplayMember = "fullName";
            cmbEmployee.ValueMember = "EmployeeID";

            cmbEmployee.DataSource = null;
            cmbEmployee.DataSource = table;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            _query = "EXEC EmployeeOrderList @EmployeeID";

            DataTable table = new DataTable();

            _command.CommandText = _query;

            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@EmployeeID", (int)cmbEmployee.SelectedValue);

            #region Deneme Failed!
            //_connection.Open();
            //dataReader = _command.ExecuteReader();

            //List<EmployeeOrder> employeeOrders = new List<EmployeeOrder>();

            //while (dataReader.Read())
            //{
            //    EmployeeOrder employee = new EmployeeOrder()
            //    {
            //        OrderID = dataReader["O.OrderID"].ToString(),
            //        OrderDate = dataReader.GetFieldValue<DateTime>("O.OrderDate"),
            //        ShippedDate = dataReader.GetFieldValue<DateTime>("O.ShippedDate"),
            //        TotalPrice = dataReader.GetFieldValue<string>("TotalPrice")

            //    };

            //    employeeOrders.Add(employee);
            //}

            //_connection.Close();

            //lbOrderNo.DataSource = null;

            //lbOrderNo.DisplayMember = "OrderID";
            //lbOrderNo.ValueMember = "OrderID";

            //lbOrderNo.DataSource = employeeOrders; 
            #endregion


            #region adapter kullanımı

            _adapter.Fill(table);

            lbOrderNo.DataSource = null;

            lbOrderNo.DisplayMember = "OrderID";
            lbOrderNo.ValueMember = "OrderID";

            lbOrderNo.DataSource = table;

            lblCount.Text = lbOrderNo.Items.Count.ToString();

            #endregion


        }

        private void btnOrderDetails_Click(object sender, EventArgs e)
        {
            DataTable table = new DataTable();

            if (lbOrderNo.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen listeden bir sipariş numarası seçiniz.");
                return;
            }

            string query1 = "SELECT O.OrderDate FROM Orders O INNER JOIN [Order Details] OD ON O.OrderID=OD.OrderID WHERE O.OrderID=@OrderID";
            string query2 = "SELECT O.ShippedDate FROM Orders O INNER JOIN [Order Details] OD ON O.OrderID=OD.OrderID WHERE O.OrderID=@OrderID";
            string query3 = "SELECT (UnitPrice*Quantity*(1-Discount)) AS TotalPrice FROM Orders O INNER JOIN [Order Details] OD ON O.OrderID=OD.OrderID WHERE O.OrderID=@OrderID";

            lblOrderDate.Text = GetScalarValue<DateTime>(query1).ToShortDateString();
            lblShippingDate.Text = GetScalarValue<DateTime>(query2).ToShortDateString();
            lblTotalPrice.Text = GetScalarValue<object>(query3).ToString();

            #region TABLE COLUMNS FAILED!!!
            //table.Columns.Add("O.OrderDate",typeof(DateTime));
            //table.Columns.Add("O.ShippedDate", typeof(DateTime));
            //table.Columns.Add("TotalPrice", typeof(int)); 
            #endregion


        }

        private T GetScalarValue<T>(string query)
        {
            _command.CommandText = query;
            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@OrderID", lbOrderNo.SelectedValue);

            _connection.Open();

            object obj = _command.ExecuteScalar();

            _connection.Close();

            if (obj != null)
            {
                return (T)obj;
            }

            return default(T);

        }


        //2.Soru -ComboBox 'ta müşterileri listeleyin. Seçilen müşteri bu zamana kadar benden kaç defa sipariş vermiş çıktı olarak verin.Bir label'ın textine atayabilir veya mbox ile gösterebilirsiniz. --tek hücre datası okunacak

        private void cmbCustomers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCustomers.SelectedIndex == -1)
            {
                MessageBox.Show("Lütfen listeden bir müşteri seçiniz");
                return;
            }

            _query = "SELECT COUNT(*) FROM Orders WHERE CustomerID=@CustomerID";
            _command.CommandText = _query;
            _command.Parameters.Clear();
            _command.Parameters.AddWithValue("@CustomerID", cmbCustomers.SelectedValue);

            _connection.Open();
            lblOrderCount.Text = _command.ExecuteScalar().ToString();
            _connection.Close();

        }
    }
}
