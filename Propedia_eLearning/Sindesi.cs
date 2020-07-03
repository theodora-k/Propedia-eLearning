using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Propedia_eLearning
{
    public partial class Sindesi : Form
    {
        public Sindesi()
        {
            InitializeComponent();
        }

        private void sinbtn_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-KKBOP61F\\SQLEXPRESS01;Initial Catalog=PropediaDB;Integrated Security=True";
            using (SqlConnection connection =
                       new SqlConnection(connectionString))
            {
                connection.Open();
                SqlDataAdapter da = new SqlDataAdapter();

                da.TableMappings.Add("Table", "UserInfo");
                SqlCommand command = new SqlCommand("SELECT * FROM dbo.UserInfo;", connection);
                command.CommandType = CommandType.Text;
                da.SelectCommand = command;
                DataSet dataSet = new DataSet("UserInfo");
                da.Fill(dataSet);

                DataTable dt = new DataTable();
                dt = dataSet.Tables["UserInfo"];

                // Once a table has been created, use the
                // NewRow to create a DataRow.

                int user_id = 0;

                // Then add the new row to the collection.
                if (!(sinmail.Text.Equals("") || sinpwd.Text.Equals("")))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["email"].ToString() == sinmail.Text.ToString() && dr["password"].ToString() == sinpwd.Text.ToString())
                        {
                            Int32.TryParse(dr["UserID"].ToString(), out user_id);
                            break;
                        }
                    }

                    if (user_id == 2)
                    {
                        this.Hide();
                        var form2 = new Admin();
                        form2.Closed += (s, args) => this.Close();
                        form2.Show();
                    }
                    else if (user_id != 0)
                    {
                        this.Hide();
                        var form2 = new Homepage(user_id);
                        form2.Closed += (s, args) => this.Close();
                        form2.Show();
                    }
                    else
                    {
                        MessageBox.Show("Ο κωδικός και το email δεν ταιριάζουν. " +
                            "Παρακαλώ προασπαθήστε ξανά","Αποτυχία Σύνδεσης", MessageBoxButtons.OK);

                    }

                }
                else
                {
                    MessageBox.Show("Παρακαλώ συμπλήρωσε email και κωδικό","Αποτυχία Σύνδεσης", MessageBoxButtons.OK);
                }

                connection.Close();

            }

        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new WelcomeScreen();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void forgotpwd_Click_1(object sender, EventArgs e)
        {
            this.Hide();
            var anaktisi = new Anaktisi_Pwd();
            anaktisi.Closed += (s, args) => this.Close();
            anaktisi.Show();
        }

        private void Sindesi_Load(object sender, EventArgs e)
        {

        }
    }
}
