using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Data.SqlClient;


namespace Propedia_eLearning
{
    public partial class Testpage : Form
    {
        int logged_user_id;
        int test_id;
        bool isRep;

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        public Random a = new Random();
        List<Tuple<int, int>> randomList = new List<Tuple<int, int>>();
        Tuple<int, int> intarray;
        int CurrentNumber = 0, extraNumber = 0;

        int curr_score = 0 ;
        int desiredQuesTotal;
        string path_chi = "";
        string connectionString = "Data Source=LAPTOP-KKBOP61F\\SQLEXPRESS01;Initial Catalog=PropediaDB;Integrated Security=True";

        public Testpage(int logged_usr, int test_number, bool rep)
        {
            InitializeComponent();
            logged_user_id = logged_usr;
            test_id = test_number; // the normal number
            isRep = rep;
        }

        //generate a new unique number on the desired span every time we need it
        private void NewNumber()
        {
            CurrentNumber = a.Next(1, 11);
            if (isRep)
            {
                if (test_id == 1)
                {
                    extraNumber = a.Next(1, 4);
                }
                else if (test_id == 2)
                {
                    extraNumber = a.Next(4, 8);
                }
                else if (test_id == 3)
                {
                    extraNumber = a.Next(7, 10);
                }
                else
                {
                    extraNumber = a.Next(1, 11);
                }
            }
            else
            {
                extraNumber = test_id;
            }
            //if this is not a repetitive extranumber is 0
            intarray = new Tuple<int, int>(CurrentNumber, extraNumber);
            if (!randomList.Contains(new Tuple<int, int>(CurrentNumber, extraNumber)))
            {
                randomList.Add(intarray);
                randnumber.Text = CurrentNumber.ToString();
            }
            else
            {
                NewNumber();
            }
        }

        private void Testpage_Load_1(object sender, EventArgs e)
        {
            this.HelpButtonClicked += Click_HelpButtonClicked;

            path_chi = "..//..//Resources//Pics//With_bg//x" + (test_id) + ".png";
            randomList.Clear();
            if (!isRep)//TODO Complete panel rep code
            {
                desiredQuesTotal = 5;
                testpanel.Visible = false;
                testpanel.Hide();
                testpanel.Dock = DockStyle.None;
                //set up labels
                prevques.Visible = true;
                chapterno.Text = test_id.ToString();
                chapterno2.Text = test_id.ToString();
                prevpanel.Visible = true;
                label15.Visible = true;

                //set up fact box
                string path_csv = "..//..//Resources//chifacts.csv";
                string[] values = File.ReadAllLines(path_csv);
                factbox.Text = values[test_id - 1];

                //set up images
                Image im = Image.FromFile(path_chi);
                xipic.BackgroundImage = im;

                string path_table = "..//..//Resources//Pics//tables//table" + test_id + ".png";
                Image im2 = Image.FromFile(path_table);
                tablebox.BackgroundImage = im2;

                //undo visibility if user had previously visited a repetative test


            }
            else//TODO Complete panel rep code
            {
                desiredQuesTotal = 10;

                testpanel.Show();
                testpanel.Visible = true;
                testpanel.Dock = DockStyle.Fill;

                prevques.Visible = false;
                prevpanel.Visible = false;
                label19.Text = "Επαναληπτικό Τεστ " + test_id.ToString();
                chapternoo.Visible = false;
                chapternoo2.Visible = false;
                label17.Visible = false;
                label15.Visible = false;
                label14.Visible = false;
                label16.Text = "Εξάσκησε την προπαίδεια των αριθμών " + ((test_id * 3) - 2).ToString() + " εώς " + (test_id * 3).ToString() + " λύνοντας 10 τυχαίες πράξεις!";

                questno.Text = "1";
                //load first random number
                NewNumber();
                randnumber.Text = CurrentNumber.ToString();
                questno2.Text = extraNumber.ToString();

                Image im3 = Image.FromFile("..//..//Resources//Pics//With_bg//x1.png");
                xi2pic.BackgroundImage = im3;

            }

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
                loggedusr2.Text = dt.Rows[(logged_user_id - 1)]["Name"].ToString() + " " + dt.Rows[(logged_user_id - 1)]["Surname"].ToString();
                loggersur3.Text = loggedusr2.Text;

                //now check if this is the second time the user has played the game
                string querystring = "Select * from UserStats";
                SqlDataAdapter adapter = new SqlDataAdapter(querystring, connectionString);
                DataSet ds = new DataSet();
                adapter.Fill(ds, "UserStats");
                string quiz_score = ds.Tables["UserStats"].Rows[(logged_user_id - 1)]["test" + test_id.ToString()].ToString();
                if (!quiz_score.Equals(""))
                {
                    if(Int32.Parse(quiz_score) < 10)
                    {
                        string msg = "Ο Κύριος Χι χαίρεται πολύ που επαναλαμβάνεις την προπαίδεια του " + test_id + " ώστε να γίνεις καλύτερος! " +
                            "Πάτησε ΟΚ για να δεις ένα βιντεάκι με τον φίλο του το Σοφό Καγκουρό και να εμπεδώσεις την ύλη καλύτερα. " +
                            "Εάν θες να συνεχίσεις την επανάληψη, πάτησε ακύρωση. ";
                        DialogResult dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
                        if (dialogResult == DialogResult.OK)
                        {
                            System.Diagnostics.Process.Start("https://www.youtube.com/watch?v=vs5cXRIUl2A");
                        }
                    }
                }
                connection.Close();
            }

        }

        private void showtestpanel_Click(object sender, EventArgs e)
        {
            testpanel.Dock = DockStyle.Fill;
            testpanel.Show();
            testpanel.Visible = true;
            chapternoo.Text = test_id.ToString();
            chapternoo2.Text = test_id.ToString();

            questno.Text = "1";
            //load first random number
            NewNumber();
            randnumber.Text = CurrentNumber.ToString();
            questno2.Text = test_id.ToString();

            Image im3 = Image.FromFile(path_chi);
            xi2pic.BackgroundImage = im3;

        }

        private void nextques_Click(object sender, EventArgs e)
        {
            if (randomList.Count() < desiredQuesTotal)
            {
                //TODO Check if input is max 2 chars and digits
                if (Int32.Parse(qinput.Text) == Int32.Parse(randnumber.Text) * Int32.Parse(questno2.Text))
                {
                    curr_score += 1;
                    yesnobox.Visible = true;
                    yesnobox.BackgroundImage = Image.FromFile("..//..//Resources//Pics//without_bg//sosto.png");
                    timer.Interval = 3000;
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Start();
                }
                else
                {
                    yesnobox.Visible = true;
                    yesnobox.BackgroundImage = Image.FromFile("..//..//Resources//Pics//without_bg//lathos.png");
                    timer.Interval = 3000;
                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Start();
                }
                NewNumber(); //Then Roll the dice
                questno.Text = (Int32.Parse(questno.Text) + 1).ToString();
                if (isRep)
                    questno2.Text = extraNumber.ToString();
                qinput.Clear();
            }
            else { //if capacity is reached
                //adding final score but not doing any other actions for moving forward to next question
                if (Int32.Parse(qinput.Text) == Int32.Parse(randnumber.Text) * Int32.Parse(questno2.Text))
                    curr_score += 1;
                int final_perc = Convert.ToInt32(((double)curr_score / (double)desiredQuesTotal) * 10); 
                using (SqlConnection connection =
                           new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command2;
                    if (!isRep)
                    {
                       command2 = new SqlCommand("UPDATE UserStats SET test" + test_id + " = " + final_perc.ToString() + " WHERE UserID = " + (logged_user_id).ToString() + ";");
                    }
                    else
                    {
                        command2 = new SqlCommand("UPDATE UserStats SET test" + test_id + "rep = " + final_perc.ToString() + " WHERE UserID = " + (logged_user_id).ToString() + ";");
                    }
                    command2.CommandType = CommandType.Text;
                    command2.Connection = connection;

                    command2.ExecuteNonQuery();
                    connection.Close();

                    string msg = "";
                    DialogResult dialogResult;
                    if (!isRep)
                    {
                        if (final_perc == 100)
                        {
                            msg = "Μόλις ολοκλήρωσες την δοκιμασία της προπαίδειας του " + test_id + " με άριστα! Πάτησε ΟΚ για να επιστρέψεις πίσω στην αρχική.";
                            dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OK);
                            if (dialogResult == DialogResult.OK)
                            {
                                this.Hide();
                                var form2 = new Homepage(logged_user_id);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                        }
                        else
                        {
                            msg = "Μόλις ολοκλήρωσες την δοκιμασία της προπαίδειας του " + test_id + " με σκορ " + (final_perc * 10).ToString() + "% !"
                                + " Σε περίπτωση που επιθυμείς να ξαναπροσπαθήσεις, μπορείς να πατήσεις ακύρωση (Cancel), " +
                                "ενώ εάν θέλεις να επιστρέψεις στην αρχική πάτησε ΟΚ!";
                            dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.OK)
                            {
                                testpanel.Visible = false;
                                this.Hide();
                                var form2 = new Homepage(logged_user_id);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                            else if (dialogResult == DialogResult.Cancel)
                            {
                                this.Hide();
                                testpanel.Visible = false;
                                var form2 = new Testpage(logged_user_id, test_id, isRep);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                        }
                    }
                    else
                    {
                        if (test_id == 4)
                        {
                            msg = "Μόλις ολοκλήρωσες το τελικό επαναληπτικό τεστ με σκορ " + (final_perc * 10).ToString() + "% !" +
                                "Εάν επιθυμείς να βελτιώσεις την επίδοση σου, στην αρχική σελίδα έχουν επισημανθεί τα " +
                                "κεφάλαια στα οποία ο Κύριος Χι διέκρινε ότι έχεις αδυναμία.";
                            dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OK);
                            if (dialogResult == DialogResult.OK)
                            {
                                this.Hide();
                                var form2 = new Homepage(logged_user_id);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                        }
                        else
                        {
                            if (test_id == 1)
                            {
                                msg = "Μόλις ολοκλήρωσες το επαναληπτικό τεστ των αριθμών 1 εώς 3 με σκορ " + (final_perc * 10).ToString() + "% !"
                                + " Σε περίπτωση που επιθυμείς να ξαναπροσπαθήσεις, μπορείς να πατήσεις ακύρωση (Cancel), " +
                                "ενώ εάν θέλεις να επιστρέψεις στην αρχική πάτησε ΟΚ!";
                                ;
                            }
                            else if (test_id == 2)
                            {
                                msg = "Μόλις ολοκλήρωσες το επαναληπτικό τεστ των αριθμών 4 εώς 6 με σκορ " + (final_perc * 10).ToString() + "% !"
                                + " Σε περίπτωση που επιθυμείς να ξαναπροσπαθήσεις, μπορείς να πατήσεις ακύρωση (Cancel), " +
                                "ενώ εάν θέλεις να επιστρέψεις στην αρχική πάτησε ΟΚ!";
                                ;
                            }
                            else if (test_id == 3)
                            {
                                msg = "Μόλις ολοκλήρωσες το επαναληπτικό τεστ των αριθμών 7 εώς 9 με σκορ " + (final_perc * 10).ToString() + "% !"
                                + " Σε περίπτωση που επιθυμείς να ξαναπροσπαθήσεις, μπορείς να πατήσεις ακύρωση (Cancel), " +
                                "ενώ εάν θέλεις να επιστρέψεις στην αρχική πάτησε ΟΚ!";
                                ;
                            }

                            dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
                            if (dialogResult == DialogResult.OK)
                            {
                                this.Hide();
                                var form2 = new Homepage(logged_user_id);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                            else if (dialogResult == DialogResult.Cancel)
                            {
                                this.Hide();
                                var form2 = new Testpage(logged_user_id, test_id, isRep);
                                form2.Closed += (s, args) => this.Close();
                                form2.Show();
                            }
                        }
                    }                   
                }
                //reseting current score
                curr_score = 0;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            yesnobox.Visible = false;
        }

        private void prevques_Click(object sender, EventArgs e)
        {
            if (randomList.Count() > 1)
            {
                randnumber.Text = randomList[randomList.Count() - 1].ToString();
                qinput.Clear();
            }
            else
            {
                MessageBox.Show("Δεν μπορείς να μεταβείς πριν από την πρώτη ερώτηση!", "Προειδοποίηση", MessageBoxButtons.OK);
            }
        }

        private void homebtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            var form2 = new Homepage(logged_user_id);
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void prevpanel_Click(object sender, EventArgs e)
        {
            string msg = "Προσοχή! Εάν επιστρέψεις στην προηγούμενη σελίδα, θα χαθεί όλη η πρόοδος του τεστ." +
                " Εάν δεν είσαι σίγουρος, κλίκαρε Cancel (Ακύρωση)";
            DialogResult dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                curr_score = 0;
                this.Hide();
                var form2 = new Testpage(logged_user_id, test_id, isRep);
                form2.Closed += (s, args) => this.Close();
                form2.Show();
            }
        }

        private void backhomebtn_Click(object sender, EventArgs e)
        {
            string msg = "Προσοχή! Εάν επιστρέψεις στην αρχική, θα χαθεί όλη η πρόοδος του τεστ." +
                " Εάν δεν είσαι σίγουρος, κλίκαρε Cancel (Ακύρωση)";
            DialogResult dialogResult = MessageBox.Show(msg, "Προειδοποίηση", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                curr_score = 0;
                this.Hide();
                var form2 = new Homepage(logged_user_id);
                form2.Closed += (s, args) => this.Close();
                form2.Show();
            }

        }

        private void Click_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            Help.ShowHelp(this, @"..\..\Resources\Help\doc.html");
        }
    }
}
