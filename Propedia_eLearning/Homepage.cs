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
using System.Reflection;
using System.IO;

namespace Propedia_eLearning
{
    public partial class Homepage : Form
    {
        int logged_user_id, curr_item = 0;
        bool isRep = false;

        public Homepage(int LoggedInUsr)
        {
            InitializeComponent();
            logged_user_id = LoggedInUsr;
        }

        private void logoutbtn_Click(object sender, EventArgs e)
        {
            string msg = "Προσοχή! Είσαι σίγουρος ότι θέλεις να αποσυνδεθείς;";
            DialogResult dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                this.Hide();
                var form2 = new WelcomeScreen();
                form2.Closed += (s, args) => this.Close();
                form2.Show();
            }

        }

        private void Homepage_Load(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;

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

                //testlabel.Text = dt.Rows[0]["email"].ToString();
                loggedusr.Text = dt.Rows[(logged_user_id - 1)]["Name"].ToString() + " " + dt.Rows[(logged_user_id - 1)]["Surname"].ToString();

                //now setting up stats
                string querystring = "Select * from UserStats";
                SqlDataAdapter adapter = new SqlDataAdapter(querystring, connectionString);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "UserStats");

                //1. progress bar
                int count_done = 0;
                string scor;
                for (int i = 1; i <= 10; i++)
                {
                    scor = ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test" + i.ToString()].ToString();
                    if (!(scor.Equals("")) && Int32.Parse(scor) == 10) //count only completed ones
                    {
                        count_done++;
                    }
                }

                progressBar1.Value = count_done * 10;

                progress1.Text = count_done.ToString();

                //Get a list of all associated controls

                List<Control> scorelabels = new List<Control>();
                List<Control> testbuttons = new List<Control>();
                List<Control> scorereplabels = new List<Control>();
                List<Control> testrepbuttons = new List<Control>();

                foreach (Control control in this.Controls)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        if (control.GetType() == typeof(Label) && control.Name == "score" + i.ToString())
                            scorelabels.Add(control);
                        else if (control.GetType() == typeof(Button) && control.Name == "btn" + i.ToString())
                            testbuttons.Add(control);                     
                        else if (control.GetType() == typeof(Label) && control.Name == "score" + i.ToString() + "rep")
                            scorereplabels.Add(control);//it will stop in the 4th element
                        else if (control.GetType() == typeof(Button) && control.Name == "btn" + i.ToString() + "rep")
                            testrepbuttons.Add(control);//it will stop in the 4th element
                    }
                }

                //now reversing controls because they were added in the opposite way
                scorelabels.Reverse();
                testbuttons.Reverse();
                scorereplabels.Reverse();

                string curr_score;
                string path_retry = "..//..//Resources//retry.png";
                string path_go = "..//..//Resources//go.jpg";
                string path_check = "..//..//Resources//check.png";
                int count_rep = 0, count_total = 0;

                //2. show score of each test in textboxes
                for (int i = 0; i < 10; i++)
                {
                    testbuttons[i].Click += testclick;
                    
                    curr_score = ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test" + (i + 1).ToString()].ToString();

                    if (!curr_score.Equals(""))
                    {
                        scorelabels[i].Text = (Int32.Parse(curr_score) * 10).ToString() + "%";
                        Image im;
                        if (curr_score == "10")
                        {
                            im = Image.FromFile(path_check);
                            testbuttons[i].Enabled = false;
                        }
                        else
                        {
                            im = Image.FromFile(path_retry);
                        }
                        testbuttons[i].BackgroundImage = im;
                        count_rep++;
                        count_total++;
                    }

                    ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test" + (i + 1).ToString()].ToString();

                    if (i == 2 && count_rep == 3 && (ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test1rep"].ToString()).Equals("")) // means repeat quiz 1,2 or 3 are available
                    {
                        Image im = Image.FromFile(path_go);
                        testrepbuttons[0].BackgroundImage = im;
                        testrepbuttons[0].Enabled = true;
                        count_rep = 0; //take counter to 0 again
                    }
                    else if (i == 5 && count_rep == 3 && (ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test2rep"].ToString()).Equals("")) // means repeat quiz 1,2 or 3 are available
                    {
                        Image im = Image.FromFile(path_go);
                        testrepbuttons[1].BackgroundImage = im;
                        testrepbuttons[1].Enabled = true;
                        count_rep = 0; //take counter to 0 again
                    }
                    if (i == 8 && count_rep == 3 && (ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test3rep"].ToString()).Equals("")) // means repeat quiz 1,2 or 3 are available
                    {
                        Image im = Image.FromFile(path_go);
                        testrepbuttons[2].BackgroundImage = im;
                        testrepbuttons[2].Enabled = true;
                        count_rep = 0; //take counter to 0 again
                    }

                }

                //3. repeat score population for repeat tests
                for (int i = 0; i <= 2; i++)
                {
                    Console.WriteLine(testrepbuttons[i].Name);
                    testrepbuttons[i].Click += testclick;

                    curr_score = ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test" + (i + 1).ToString() + "rep"].ToString();
                    if (!curr_score.Equals(""))
                    {
                        Image im;
                        if (curr_score == "10")
                        {
                            im = Image.FromFile(path_check);
                            testrepbuttons[i].Enabled = false;
                        }
                        else
                        {
                            im = Image.FromFile(path_retry);
                            testrepbuttons[i].Enabled = true;
                        }
                        testrepbuttons[i].BackgroundImage = im;
                        scorereplabels[i].Text = (Int32.Parse(curr_score) * 10).ToString() + "%";
                    }
                }
                
                //4. open final test
                curr_score = ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test4rep"].ToString();
                if (count_total == 10 && curr_score.Equals("")) 
                {
                    Image im = Image.FromFile(path_go);
                    testrepbuttons[3].BackgroundImage = im;
                    testrepbuttons[3].Enabled = true;

                }
                else if (!curr_score.Equals(""))
                {
                    Image im;
                    if (curr_score == "10")
                    {
                        im = Image.FromFile(path_check);
                    }
                    else
                    {
                        im = Image.FromFile(path_retry);
                    }
                    testrepbuttons[3].BackgroundImage = im;
                    scorereplabels[3].Text = curr_score + "/ 10";
                }

                connection.Close();
            }           
        }

        private void testclick(object sender, EventArgs ev)
        {
            var button = (Button)sender;
            if (button.Name.ToString().Contains("rep"))
                isRep = true;
            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            curr_item = Int32.Parse(Regex.Replace(button.Name.ToString(), "[^0-9]", ""));

            this.Hide();         
            var newtest = new Testpage(logged_user_id, curr_item, isRep);
            newtest.Closed += (s, args) => this.Close();
            newtest.Show();
        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }

    }
}
