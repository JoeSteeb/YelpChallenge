using System;
using Npgsql;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using QueryHandler_;

namespace YelpApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ListBox current;
        Query qH;
        Query qU;
        User currentUser;

        public MainWindow()
        {
            InitializeComponent();
            SetUpDataGrid(new List<(string, string, int)> {
                ("ID", "bID", 50), 
                ("Name", "bName", 100), 
                ("Address", "bAddress", 150),
                ("Longitude", "longitude", 100),
                ("Latitude", "latitude", 100),
                ("Stars", "stars", 50),
                ("Is Open", "is_open", 50),
                ("Tips", "tip_count", 50),
                ("Checkins", "numCheckins", 50)
            },businessDataGrid);

            SetUpDataGrid(new List<(string, string, int)> {
                ("Name", "userName", 100),
                ("Average Stars", "stars", 90),
                ("Yelping Since", "yelpingSince", 120),
                ("Total Likes", "tipLikes", 100),
            }, FriendTable);

            SetUpDataGrid(new List<(string, string, int)> {
                ("User Name", "uName", 100),
                ("Business", "bName", 100),
                ("City", "city", 100),
                ("Text", "text", 300),
                ("Date", "date", 100)
            }, LatestTipsGrid);
        }

        private void SetUpDataGrid(List<(string, string, int)> names, DataGrid dest)
        {
            foreach ((string, string, int) s in names)
            {
                dest.Columns.Add(new DataGridTextColumn { Header = s.Item1,Binding = new Binding(s.Item2), Width = s.Item3});
            }
        }

        private void ShowList(IEnumerable<string> e)
        {
           foreach (string item in e)
           {
                current.Items.Add(item);
           }
        }

        private void PutList(IEnumerable<string> e, ListBox dest)
        {
            dest.Items.Clear();
            foreach (string item in e)
            {
                dest.Items.Add(item);
            }
        }

        private void ShowUsers(IEnumerable<(object, object, object, object, object, object, object, object, object, object, object)> e, TextBox dest)
        {
            foreach (var (uname, ysince, tips, fans, stars, funny, useful, cool, lon, lat, likes) in e)
            {
                dest.Text = 
                    "Name: " + uname + 
                    "\nStars: " + stars+ " fans: " + fans+ 
                    "\nYelping since: " + ysince + 
                    "\nVotes:" +
                    "\nFunny:" + funny + "Cool: " + cool + "Useful: " + useful +
                    "\nTip Count: " + tips +
                    "\nTotal Tip Likes: " + likes +
                    "\n Location:" +
                    "\nLat: " + lat +
                    "\nLon: " + lon;
            }

        }

        private void ShowFriends(IEnumerable<(object, object, object, object)> e, DataGrid dest)
        {
            dest.Items.Clear();
            foreach (var (uname, ysince, stars, likes) in e)
            {
                dest.Items.Add(new pUser { userName = uname+"", stars = stars+"", yelpingSince = ysince+"", tipLikes = likes+""});               
            }
        }

        private void ShowTips(IEnumerable<(object, object, object, object, object)> e, DataGrid dest)
        {
            dest.Items.Clear();
            foreach (var (uname, bname, city, text, date) in e)
            {
                dest.Items.Add(new pTip { uName = uname + "", bName = bname + "", city = city + "",  text = text + "", date = date + "" });
            }
        }

        //var(id, dp, body, likes)


        private void ShowData(IEnumerable<(string, string, string, string, string, string, string, string, string)> e)
        {
            foreach (var (id, name, address, lon, lat, stars, is_open, tip_count, numCheckins) in e)
            {
                businessDataGrid.Items.Add(new Business { bID = id, bName = name, bAddress = address, longitude = lon, latitude = lat, stars = stars, is_open = is_open, tip_count = tip_count, numCheckins = numCheckins});
            }
        }


        private void stateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cityListBox.Items.Clear();
            zipListBox.Items.Clear();
            categoriesListBox.Items.Clear();
            businessDataGrid.Items.Clear();

            if (!stateListBox.Items.IsEmpty && qH != null)
            {
                current = cityListBox;
                ShowList(qH.GetCitiesInState(stateListBox.SelectedItem.ToString()));
            }
        }

        private void cityListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            zipListBox.Items.Clear();
            categoriesListBox.Items.Clear();
            businessDataGrid.Items.Clear();

            if (!cityListBox.Items.IsEmpty && qH != null)
            {
                current = zipListBox;
                ShowList(qH.GetZipsInCity(stateListBox.SelectedItem.ToString(), cityListBox.SelectedItem.ToString()));
            }
        }

        private void zipListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            categoriesListBox.Items.Clear();
            businessDataGrid.Items.Clear();
            if (!zipListBox.Items.IsEmpty && qH != null)
            {
                current = categoriesListBox;
                ShowList(qH.GetCategoriesInZip(Convert.ToInt32(zipListBox.SelectedItem.ToString())));
                ShowData(qH.GetBusinessesInZip(Convert.ToInt32(zipListBox.SelectedItem.ToString()), b =>
                                                    (
                                                        (string)b["business_id"],
                                                        (string)b["business_name"],
                                                        (string)b["business_address"],
                                                        (string)b["longitude"],
                                                        (string)b["latitude"],
                                                        (string)b["stars"],
                                                        (string)b["is_open"],
                                                        (string)b["tip_count"],
                                                        (string)b["numCheckins"]
                                                    )));
            }
        }

        private void categoriesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessDataGrid.Items.Clear();

            if (!categoriesListBox.Items.IsEmpty && qH != null)
            {
                ShowData(qH.GetBusinessesInZip(Convert.ToInt32(zipListBox.SelectedItem.ToString()), categoriesListBox.SelectedItems, b =>
                                                    (
                                                        (string)b["business_id"],
                                                        (string)b["business_name"],
                                                        (string)b["business_address"],
                                                        (string)b["longitude"],
                                                        (string)b["latitude"],
                                                        (string)b["stars"],
                                                        (string)b["is_open"],
                                                        (string)b["tip_count"],
                                                        (string)b["numCheckins"]
                                                    )));
            }
        }

        private void passwordTB_KeyDown(object sender, KeyEventArgs e)
        {
            Connect(e.Key);
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            Connect(Key.Enter);
        }

        private void disconnectButton_Click(object sender, RoutedEventArgs e)
        {
            // app
            appLabel.Content = "Sign In:";
            appLabel.Visibility = Visibility.Hidden;
            nameTBLabel.Visibility = Visibility.Hidden;
            nameLabel.Visibility = Visibility.Hidden;
            useridLabel.Visibility = Visibility.Hidden;
            nameTB.Visibility = Visibility.Hidden;
            useridTB.Visibility = Visibility.Hidden;
            loginButton.Visibility = Visibility.Hidden;
            logoutButton.Visibility = Visibility.Hidden;
            
            // hide and unhide the right things
            postgresLabel.Visibility = Visibility.Visible;
            dbLabel.Visibility = Visibility.Visible;
            usernameLabel.Visibility = Visibility.Visible;
            passwordLabel.Visibility = Visibility.Visible;
            passwordTB.Visibility = Visibility.Visible;
            connectButton.Visibility = Visibility.Visible;
            disconnectButton.Visibility = Visibility.Hidden;

            // clear everything and nullify Query
            stateListBox.Items.Clear();
            cityListBox.Items.Clear();
            zipListBox.Items.Clear();
            categoriesListBox.Items.Clear();
            businessDataGrid.Items.Clear();
            qH = null;
        }
    
        private void Connect(Key key)
        {
            if (key == Key.Enter)
            {
                qH = new Query("postgres", passwordTB.Text.ToString(), "yelpdb");
                qU = new Query(qH);

                // authenticate connection
                if (!qH.CheckConnection())
                {
                    System.Windows.MessageBox.Show("Error - PSQL: Failed to login. Please try again...");
                }
                else
                {
                    current = stateListBox;


                    // hide and unhide the right things
                    // database 
                    dbLabel.Visibility = Visibility.Hidden;
                    postgresLabel.Visibility = Visibility.Hidden;
                    usernameLabel.Visibility = Visibility.Hidden;
                    passwordLabel.Visibility = Visibility.Hidden;
                    passwordTB.Visibility = Visibility.Hidden;
                    connectButton.Visibility = Visibility.Hidden;
                    LoginCanvas.Visibility = Visibility.Hidden;
                    disconnectButton.Visibility = Visibility.Visible;

                    // app
                    appLabel.Visibility = Visibility.Visible;
                    nameTBLabel.Visibility = Visibility.Visible;
                    useridLabel.Visibility = Visibility.Visible;
                    nameTB.Visibility = Visibility.Visible;
                    useridTB.Visibility = Visibility.Visible;
                    loginButton.Visibility = Visibility.Visible;

                    // clear database password
                    passwordTB.Clear();

                    // open connection
                    qH.Open();
                    //qU.Open();
                    ShowList(qH.GetAllStates());
                }
            }
        }

        private void businessDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!businessDataGrid.Items.IsEmpty)
            {
                TipWindow tipForm = new TipWindow(qH, currentUser, (Business)businessDataGrid.SelectedItem);
                tipForm.Show();
            }
        }

        private void useridTB_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Login();
            }
        }

        //private void nameTB_KeyDown(object sender, KeyEventArgs e)
        //{

        //}

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            currentUser = null;

            // hide and unhide the right things
            // app
            appLabel.Content = "Sign In:";
            nameLabel.Visibility = Visibility.Hidden;
            useridLabel.Visibility = Visibility.Visible;
            nameTB.Visibility = Visibility.Visible;
            useridTB.Visibility = Visibility.Visible;
            loginButton.Visibility = Visibility.Visible;
            logoutButton.Visibility = Visibility.Hidden;

        }

        private void Login()
        {
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
                    appLabel.Content = "Signed In:";
                    nameTBLabel.Visibility = Visibility.Visible;
                    nameLabel.Visibility = Visibility.Visible;
                    useridLabel.Visibility = Visibility.Hidden;
                    nameTB.Visibility = Visibility.Hidden;
                    useridTB.Visibility = Visibility.Hidden;
                    loginButton.Visibility = Visibility.Hidden;
                    logoutButton.Visibility = Visibility.Visible;

                    System.Windows.MessageBox.Show($"Login - Success: Welcome!");
                    break;
                }
                else
                {
                    System.Windows.MessageBox.Show($"Login - Error: Could not find Name ({name}) and ID ({userid}) combination. Please try again...");
                }
            }
            
        }


        private void userTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (qH != null)
            {
                PutList(qH.GetUser(userTextBox.Text), userListBox);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentUser == null)
            {
                currentUser = new User();
                currentUser.userName = "";
                currentUser.userId = "";
            }
            if (userListBox.SelectedValue != null)
            {
                FriendTable.Items.Clear();
                currentUser.userId = userListBox.SelectedValue.ToString();
                userNameBox.Text = currentUser.userId;
                ShowUsers(qH.FindUserById(currentUser.userId), userNameBox);
                ShowFriends(qH.FindFriendsByID(currentUser.userId), FriendTable);
                ShowTips(qH.FindRecentTips(currentUser.userId), LatestTipsGrid);
            }
        }

        private void userNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
