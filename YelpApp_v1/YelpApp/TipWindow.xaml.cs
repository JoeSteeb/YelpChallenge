using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QueryHandler_;

namespace YelpApp
{
    /// <summary>
    /// Interaction logic for TipWindow.xaml
    /// </summary>
    public partial class TipWindow : Window
    {
        Query qH;
        User currentUser;
        Business businessSelection;
        public TipWindow()
        {
            InitializeComponent();
        }

        public TipWindow(Query qH, User user, Business businessSelection)
        {
            InitializeComponent();

            this.qH = qH;
            this.currentUser = user;
            this.businessSelection = businessSelection;

            SetUpDataGrid();
            PromptLogin();
            DisplayBusinessData();
        }

        private void SetUpDataGrid()
        {
            DataGridTextColumn col1 = new DataGridTextColumn();
            DataGridTextColumn col2 = new DataGridTextColumn();
            DataGridTextColumn col3 = new DataGridTextColumn();
            DataGridTextColumn col4 = new DataGridTextColumn();
            DataGridTextColumn col5 = new DataGridTextColumn();

            col1.Header = "User ID";
            col1.Binding = new Binding("userID");
            col1.Width = 150;
            tipDataGrid.Columns.Add(col1);

            col2.Header = "Date Posted";
            col2.Binding = new Binding("datePosted");
            col2.Width = 125;
            tipDataGrid.Columns.Add(col2);

            col3.Header = "Body";
            col3.Binding = new Binding("body");
            col3.Width = 300;
            tipDataGrid.Columns.Add(col3);

            col4.Header = "Likes";
            col4.Binding = new Binding("likes");
            col4.Width = 50;
            tipDataGrid.Columns.Add(col4);

        }


        private void ShowTips(IEnumerable<(string, string, string, string)> e)
        {
            foreach (var (id, dp, body, likes) in e)
            {
                tipDataGrid.Items.Add(new Tips { userID = id, datePosted = dp, body = body, likes = likes });
            }
        }

        private void DisplayBusinessData()
        {
            tipDataGrid.Items.Clear();

            ShowTips(qH.GetTipsInBusiness(businessSelection.bID, b =>
                                                    (
                                                        (string)b["user_id"],
                                                        (string)b["date_posted"],
                                                        (string)b["body"],
                                                        (string)b["likes"]
                                                    )));
        }

        private void PromptLogin()
        {
            if (IsLoggedIn())
            {
                nameTBLabel.Visibility = Visibility.Visible;
                nameLabel.Content = currentUser.userName;
                nameLabel.Visibility = Visibility.Visible;

                nameTB.Visibility = Visibility.Hidden;
                useridLabel.Visibility = Visibility.Hidden;
                useridTB.Visibility = Visibility.Hidden;
                loginButton.Visibility = Visibility.Hidden;
            }
            else
            {
                nameTBLabel.Visibility = Visibility.Visible;
                nameLabel.Visibility = Visibility.Hidden;

                nameTB.Visibility = Visibility.Visible;
                useridLabel.Visibility = Visibility.Visible;
                useridTB.Visibility = Visibility.Visible;
                loginButton.Visibility = Visibility.Visible;
            }
        }

        private bool IsLoggedIn()
        {
            return currentUser != null;
        }

        private void useridTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void Login()
        {
            bool found = false;
            string name = nameTB.Text.ToString(), userid = useridTB.Text.ToString();
            IEnumerable<string> login = qH.Login(name, userid);
            foreach (string n in login)
            {
                if (n == name)
                {
                    currentUser = new User { userName = name, userId = userid };

                    nameTB.Clear();
                    useridTB.Clear();
                    nameLabel.Content = name;
                    nameTBLabel.Visibility = Visibility.Visible;
                    nameLabel.Visibility = Visibility.Visible;
                    nameTB.Visibility = Visibility.Hidden;
                    useridTB.Visibility = Visibility.Hidden;
                    useridLabel.Visibility = Visibility.Hidden;
                    loginButton.Visibility = Visibility.Hidden;
                    found = true;
                    break;
                }
            }

            // check if user was found
            if (!found)
            {
                System.Windows.MessageBox.Show($"Login - Error: Could not find Name ({name}) and ID ({userid}) combination. Please try again...");
            }
            else
            {
                System.Windows.MessageBox.Show($"Login - Success: Welcome!");
            }
        }

        private void tipTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(!IsLoggedIn())
                {
                    System.Windows.MessageBox.Show("Please login begore entering a tip!");
                }
                else
                {
                    Post();
                }
            }
        }

        private void postButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoggedIn())
            {
                System.Windows.MessageBox.Show("Please login begore entering a tip!");
            }
            else
            {
                Post();
            }
        }

        private void Post()
        {
            //System.Windows.MessageBox.Show(timestamp);
            qH.InsertTip(currentUser.userId, businessSelection.bID, tipTB.Text.ToString(), DateTime.Now);
            tipTB.Clear();
            DisplayBusinessData();
        }

    }




    public class Tips
    {
        public string userID { get; set; }
        public string datePosted { get; set; }
        public string body { get; set; }
        public string likes { get; set; }

    }



}
