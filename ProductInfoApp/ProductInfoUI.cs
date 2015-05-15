using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductInfoApp
{
    public partial class ProductInfoUI : Form
    {
        public ProductInfoUI()
        {
            InitializeComponent();

           
        }

        Product product=new Product();
        private string connectionString =
            ConfigurationManager.ConnectionStrings["ProductInfoConString"].ConnectionString;

        private int id;
        private int totalquantity;

        private void saveProductInfoButton_Click(object sender, EventArgs e)
        {
            product.productcode = productCodeTextBox.Text;
            product.description = descriptionTextBox.Text;
            product.quantity = int.Parse(quantityTextBox.Text);


            bool result = AlreadyProductCodeExits(product.productcode);

            if (result)
            {
                 SqlConnection connection = new SqlConnection(connectionString);

                int totalquantity = GetTotalQuantity(product.productcode)+product.quantity;

                    string query = "UPDATE  product SET  p_description='" + product.description +
                                   "', p_quantity='" + totalquantity + "' WHERE p_code='"+product.productcode+"'";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowseffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowseffected > 0)
                    {
                        MessageBox.Show(" Product Information is updated");
                        GetTotalDataInListView();
                        totalQuantityTextBox.Text = GetSumOfQuantity().ToString();
                        GetClearTextBoxes();



                    }
                    else
                    {
                        MessageBox.Show("Error!!");
                    }

            }

            else
            {


                if (product.productcode.Length >= 3)
                {
                    SqlConnection connection = new SqlConnection(connectionString);

                    string query = "INSERT INTO product VALUES('" + product.productcode + "','" + product.description +
                                   "','" + product.quantity + "')";

                    SqlCommand command = new SqlCommand(query, connection);

                    connection.Open();
                    int rowseffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowseffected > 0)
                    {
                        MessageBox.Show(" Product Information is saved");
                        //GetTotalDataInListView();
                        totalQuantityTextBox.Text = GetSumOfQuantity().ToString();
                        GetClearTextBoxes();


                    }
                    else
                    {
                        MessageBox.Show("Error!!");
                    }

                }
                else
                {
                    MessageBox.Show("Product Code must be three characters long");
                }

            }
        }

        private void GetClearTextBoxes()
        {
            productCodeTextBox.Text = "";
            descriptionTextBox.Text = "";
            quantityTextBox.Text = "";
        }

        private void showProductInfoButton_Click(object sender, EventArgs e)
        {
            GetTotalDataInListView();
            totalQuantityTextBox.Text = GetSumOfQuantity().ToString();
           
        }



        private bool  AlreadyProductCodeExits(string code)
    {

           bool productcodeexits = false;
        
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT p_code FROM product WHERE p_code='"+code+"'";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                productcodeexits = true;
               
            }
            connection.Close();

           return productcodeexits;

    }


        private int GetTotalQuantity(string code)
        {
            int totalquantity = 0;

            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT p_quantity FROM product WHERE p_code='"+code+"'";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int quantity = int.Parse(reader["p_quantity"].ToString());
                totalquantity += quantity;

            }
            connection.Close();

            return totalquantity;


        }



        private void GetTotalDataInListView()
        {
            productInfoListView.Items.Clear();

            List<Product> products = new List<Product>();



            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT p_code,p_description,p_quantity FROM product";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {

                Product aProduct = new Product();

                aProduct.productcode = reader["p_code"].ToString();
                aProduct.description = reader["p_description"].ToString();
                aProduct.quantity = int.Parse(reader["p_quantity"].ToString());

                totalquantity = totalquantity + aProduct.quantity;

                products.Add(aProduct);


            }
            connection.Close();

            foreach (var product in products)
            {
                ListViewItem item = new ListViewItem();
                item.Text = product.productcode;
                item.SubItems.Add(product.description);
                item.SubItems.Add(product.quantity.ToString());
                productInfoListView.Items.Add(item);
            }


            

        }


        private int GetSumOfQuantity()
        {

            int sumquantity = 0;

            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT SUM(p_quantity) FROM product";

            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            sumquantity = (int)command.ExecuteScalar();
           
            connection.Close();

            return sumquantity;


        }
    }
}
