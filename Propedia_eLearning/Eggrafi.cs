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
using System.Text.RegularExpressions;


namespace Propedia_eLearning
{
    public partial class Eggrafi : Form
    {
        public Eggrafi()
        {
            InitializeComponent();
        }

        private void egbtn_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=LAPTOP-KKBOP61F\\SQLEXPRESS01;Initial Catalog=PropediaDB;Integrated Security=True";
            using (SqlConnection connection =
                       new SqlConnection(connectionString))
            {

                //check if pass
                var regex = @".{8,}";

                // Then add the new row to the collection.
                if (!(egname.Text.Equals("") || egsurname.Text.Equals("") || egmail.Text.Equals("") || egpwd.Text.Equals("")))
                {
                    if (!(Regex.Match(egpwd.Text.ToString(), regex, RegexOptions.IgnoreCase).Success))
                    {
                        MessageBox.Show("Ο κωδικός πρέπει να διαθέτει πάνω από 8 χαρακτήρες", "Αποτυχία Εγγραφής", MessageBoxButtons.OK);
                    }
                    else
                    {
                        string birthdate = "";
                        if (!egage.Text.Equals(""))
                        {
                            birthdate = egage.Text.ToString();
                        }
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
                        int new_id = dt.Select().Length + 1;
                        connection.Close();

                        SqlCommand command2 = new SqlCommand("INSERT UserInfo (UserID, Name, Surname, email, password, birthdate ) VALUES " +
                            "('" + (new_id).ToString() + "', '" + egname.Text.ToString() +"', '"+ egsurname.Text.ToString() +
                            "', '" + egmail.Text.ToString() + "', '" + egpwd.Text.ToString() + "', '" + birthdate + "');");
                        command2.CommandType = CommandType.Text;
                        command2.Connection = connection;

                        connection.Open();
                        command2.ExecuteNonQuery();
                        connection.Close();

                        //now create an empty userstats row

                        SqlCommand command3 = new SqlCommand("INSERT UserStats (UserID) VALUES " +
                            "(" + (new_id).ToString() + ");");
                        command3.CommandType = CommandType.Text;
                        command3.Connection = connection;

                        connection.Open();
                        command3.ExecuteNonQuery();
                        connection.Close();

                        this.Hide();
                        var form2 = new Homepage(new_id);
                         //var form2 = new Homepage(egmail.Text.ToString());
                        form2.Closed += (s, args) => this.Close();
                        form2.Show();                      

                    }                   
                }           
                else
                {
                    MessageBox.Show("Πρέπει να συμπληρώσετε όλα τα απαραίτητα πεδία", "Αποτυχία Εγγραφής", MessageBoxButtons.OK);
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

        private void Eggrafi_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;

        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }
    }
}
