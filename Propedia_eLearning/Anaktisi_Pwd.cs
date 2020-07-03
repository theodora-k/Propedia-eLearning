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
using System.Net;
using System.Net.Mail;



namespace Propedia_eLearning
{
    public partial class Anaktisi_Pwd : Form
    {
        String new_pwd = "";
       
        public Anaktisi_Pwd()
        {
            InitializeComponent();
        }

        private string genNewPwd()
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
                if (!(anakname.Text.Equals("") || anakemail.Text.Equals("")))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["email"].ToString() == anakemail.Text.ToString() && (dr["Name"].ToString() + " " + dr["Surname"].ToString() == anakname.Text.ToString()))
                        {
                            Int32.TryParse(dr["UserID"].ToString(), out user_id);
                            break;
                        }
                    }

                    if (user_id != 0)
                    {
                        Random generator = new Random();
                        String r = generator.Next(0, 999999).ToString("D6");
                        SqlCommand command2 = new SqlCommand("UPDATE UserInfo SET password  = " + r + " WHERE UserID = " + (user_id).ToString() + ";");
                        command2.CommandType = CommandType.Text;
                        command2.Connection = connection;

                        command2.ExecuteNonQuery();
                        connection.Close();
                        new_pwd = r;
                        return r;
                    }
                    else
                    {
                        connection.Close();
                        return null;
                    }

                }
                else
                {
                    connection.Close();
                    return null;
                }

            }
        }

        private void sendemail(object sender, EventArgs e)
        {
            if (!genNewPwd().Equals(null))
            {
                anakname.Text = "";
                anakemail.Text = "";
                label2.Text = "Ο νέος σου κωδικός είναι " + new_pwd + "!";
                antigrafi.Visible = true;
            }
            else
            {
                MessageBox.Show("Το email δεν ταιριάζει με την εγγραφή κάποιου χρήστη. " +
                            "Παρακαλώ προασπαθήστε ξανά", "Αποτυχία Ανάκτησης κωδικού", MessageBoxButtons.OK);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Sindesi();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void Anaktisi_Pwd_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;
            antigrafi.Visible = false;
        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }

        private void antigrafi_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(new_pwd);
        }
    }
}
