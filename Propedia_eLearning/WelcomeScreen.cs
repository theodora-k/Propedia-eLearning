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
    public partial class WelcomeScreen : Form
    {
        public WelcomeScreen()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;
            /*string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\Database1.mdf;Integrated Security=True";
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

                //testlabel.Text = dt.Rows[0]["email"].ToString();

                /*foreach (DataRow dr in dt.Rows)
                {
                    if (dr["Name"].ToString() == "Theodoraa")
                    {
                        lol = dr["Surname"].ToString();
                    } 
                    //MessageBox.Show(dr["email"].ToString());
                }

                connection.Close();

            }*/
        }

        private void eggrafibtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var eggrafi = new Eggrafi();
            eggrafi.Closed += (s, args) => this.Close();
            eggrafi.Show();
        }

        private void sindesibtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var sindesi = new Sindesi();
            sindesi.Closed += (s, args) => this.Close();
            sindesi.Show();
        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }

    }
}
