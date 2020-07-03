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
    public partial class Admin : Form
    {
        public Admin()
        {
            InitializeComponent();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;
            recordspanel.Visible = false;
            demoPanel.Visible = false;

        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }

        private void allRecs_Click(object sender, EventArgs e)
        {
            demoPanel.Visible = false;
            recordspanel.Dock = DockStyle.Fill;
            recordspanel.Show();
            recordspanel.Visible = true;
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

                //now setting up stats
                string querystring = "Select * from UserStats";
                SqlDataAdapter adapter = new SqlDataAdapter(querystring, connectionString);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "UserStats");

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["UserID"].ToString() != "2")//only if this is not the teacher
                    {
                        ListViewItem item1 = new ListViewItem(dr["Name"].ToString());
                        ListViewItem item2 = new ListViewItem(dr["Surname"].ToString());
                        ListViewItem item3 = new ListViewItem(dr["email"].ToString());
                        ListViewItem item4 = new ListViewItem(dr["birthdate"].ToString());
                        //count individual progress 
                        int count_done = 0;
                        string scor;
                        for (int i = 1; i <= 10; i++)
                        {
                            scor = ds.Tables["UserStats"].Rows[Int32.Parse(dr["UserID"].ToString()) - 1]["test" + i.ToString()].ToString();
                            if (!(scor.Equals("")) && Int32.Parse(scor) == 10) //count only completed ones
                            {
                                count_done++;
                            }
                        }
                        ListViewItem item5 = new ListViewItem((count_done * 10).ToString() + "%");

                        listView1.Items.AddRange(new ListViewItem[] { item1, item2, item3, item4, item5 });
                    }
                }
                connection.Close();
            }
            
        }

        private void label10_Click(object sender, EventArgs e)
        {
            demoPanel.Visible = false;
            recordspanel.Visible = false;
            this.Hide();
            var form2 = new Admin();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void homebtn_Click(object sender, EventArgs e)
        {
            demoPanel.Visible = false;
            recordspanel.Visible = false;
        }

        private void logoutbtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new WelcomeScreen();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void backhomebtn_Click(object sender, EventArgs e)
        {
            demoPanel.Visible = false;
            recordspanel.Visible = false;
        }

        private void allStats_Click_1(object sender, EventArgs e)
        {
            demoPanel.Dock = DockStyle.Fill;
            demoPanel.Visible = true;
            recordspanel.Visible = false;

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

                //now setting up stats
                string querystring = "Select * from UserStats";
                SqlDataAdapter adapter = new SqlDataAdapter(querystring, connectionString);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "UserStats");

                DataTable dt2 = new DataTable();
                dt2 = ds.Tables["UserStats"];
                int total_studs = 0, total_sum = 0, count_done = 0, count_complete = 0;
                List<int> mo = new List<int>();
                int[] zeros = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                mo.AddRange(zeros);
                foreach (DataRow dr in dt2.Rows)
                {
                    if (dr["UserID"].ToString() != "2")//only if this is not the teacher
                    {
                        //count individual progress 
                        total_studs++;
                        int single_sum = 0, single_done = 0;
                        string scor;
                        for (int i = 1; i <= 10; i++)
                        {
                            scor = ds.Tables["UserStats"].Rows[Int32.Parse(dr["UserID"].ToString()) - 1]["test" + i.ToString()].ToString();
                            if (!(scor.Equals(""))) //count only completed ones
                            {
                                single_sum += Int32.Parse(scor);
                                single_done++;
                            }                            
                        }
                        count_done += single_done;
                        if (single_done == 10)
                            count_complete++;
                        total_sum += single_sum;
                    }
                }
                totalmo.Text = ((total_sum / total_studs) * 10).ToString() + "%";
                mocomplete.Text = ((count_done / total_studs) * 10).ToString() + "%";
                progperc.Text = ((count_complete / total_studs) * 10).ToString() + "%";

                connection.Close();

            }

        }
    }
}
