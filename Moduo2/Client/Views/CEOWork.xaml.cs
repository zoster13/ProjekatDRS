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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientCommon.Data;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for CEOWork.xaml
    /// </summary>
    public partial class CEOWork : UserControl
    {

        public LocalClientDatabase database = null;

        public CEOWork()
        {
            InitializeComponent();

            //database = new LocalClientDatabase();
            DataContext = LocalClientDatabase.Instance;

            foreach(var type in Enum.GetValues(typeof(EmployeeType)))
            {
                comboBoxNewType.Items.Add(type);
                comboBoxNewType.SelectedIndex = 0;
                comboBoxType.Items.Add(type);
                comboBoxType.SelectedIndex = 0;
            }
        }

        private void textBoxTeamName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTeamName.Text != "")
            {
                buttonTeamNext1.IsEnabled = true;
            }
            else
            {
                buttonTeamNext1.IsEnabled = false;
            }
        }

        private void buttonTeamNext1_Click(object sender, RoutedEventArgs e)
        {
            if(radioButton1.IsChecked == true)
            {
                tabControlNewTeam.SelectedIndex = 2;
            }
            else
            {
                tabControlNewTeam.SelectedIndex = 1;
            }
        }

        private void buttonTeamBack2_Click(object sender, RoutedEventArgs e)
        {
            tabControlNewTeam.SelectedIndex = 0;
        }

        private void buttonTeamBack3_Click(object sender, RoutedEventArgs e)
        {
            tabControlNewTeam.SelectedIndex = 0;
        }

        private void textBoxLeaderName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "" || passwordBoxLeader.Password == "")
            {
                buttonAddTeam2.IsEnabled = false;
            }
            else
            {
                buttonAddTeam2.IsEnabled = true;
            }
        }

        private void textBoxLeaderEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "" || passwordBoxLeader.Password == "")
            {
                buttonAddTeam2.IsEnabled = false;
            }
            else
            {
                buttonAddTeam2.IsEnabled = true;
            }
        }

        private void textBoxLeaderSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "" || passwordBoxLeader.Password == "")
            {
                buttonAddTeam2.IsEnabled = false;
            }
            else
            {
                buttonAddTeam2.IsEnabled = true;
            }
        }

        private void passwordBoxLeader_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "" || passwordBoxLeader.Password == "")
            {
                buttonAddTeam2.IsEnabled = false;
            }
            else
            {
                buttonAddTeam2.IsEnabled = true;
            }
        }

        private void textBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "" || passwordBoxNew.Password == "")
            {
                buttonNext1.IsEnabled = false;
            }
            else
            {
                buttonNext1.IsEnabled = true;
            }
        }

        private void textBoxSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "" || passwordBoxNew.Password == "")
            {
                buttonNext1.IsEnabled = false;
            }
            else
            {
                buttonNext1.IsEnabled = true;
            }
        }

        private void textBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "" || passwordBoxNew.Password == "")
            {
                buttonNext1.IsEnabled = false;
            }
            else
            {
                buttonNext1.IsEnabled = true;
            }
        }


        private void passwordBoxNew_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "" || passwordBoxNew.Password == "")
            {
                buttonNext1.IsEnabled = false;
            }
            else
            {
                buttonNext1.IsEnabled = true;
            }
        }


        private void buttonNext1_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxType.SelectedItem != null)
            {
                EmployeeType emt = (EmployeeType)comboBoxType.SelectedItem;

                if (emt == EmployeeType.DEVELOPER)
                {
                    addEmployeeTabControl.SelectedIndex = 1;
                }
                else if (emt == EmployeeType.SCRUMMASTER)
                {
                    addEmployeeTabControl.SelectedIndex = 2;
                }
                else if (emt == EmployeeType.TEAMLEADER)
                {
                    addEmployeeTabControl.SelectedIndex = 3;
                }
                else if (emt == EmployeeType.HR)
                {
                    addEmployeeTabControl.SelectedIndex = 0;
                    Employee employee = new Employee();
                    
                    employee.Name = textBoxName.Text;
                    employee.Surname = textBoxSurname.Text;
                    employee.Email = textBoxEmail.Text;
                    employee.Password = passwordBoxNew.Password;
                    employee.Type = EmployeeType.HR;
                    employee.PasswordTimeStamp = DateTime.Now;

                    LocalClientDatabase.Instance.proxy.AddEmployee(employee);

                    NewEmployeeMessage();
                }
            }
        }

        private void buttonAddTeam2_Click(object sender, RoutedEventArgs e)
        {
            Employee em = new Employee() { Name = textBoxLeaderName.Text, Surname = textBoxLeaderSurname.Text, Email = textBoxLeaderEmail.Text, Password = passwordBoxLeader.Password };
            Team newTeam = new Team() { Name = textBoxTeamName.Text, TeamLeader = em };
            em.Team = newTeam;

            LocalClientDatabase.Instance.proxy.AddTeam(newTeam);

            textBoxTeamName.Text = "";

            textBoxLeaderName.Text = "";
            textBoxLeaderSurname.Text = "";
            textBoxLeaderEmail.Text = "";
            textBoxTeamName.Text = "";
            passwordBoxLeader.Password = "";

            tabControlNewTeam.SelectedIndex = 0;
        }

        private void buttonAddTeam3_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxTeamLeader.SelectedItem != null)
            {
                Team newTeam = new Team() { Name = textBoxTeamName.Text, TeamLeader = (Employee)comboBoxTeamLeader.SelectedItem, };

                LocalClientDatabase.Instance.proxy.AddTeam(newTeam);
            }

            textBoxTeamName.Text = "";

            tabControlNewTeam.SelectedIndex = 0;
        }

        private void buttonProjectAssign_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxProjects.SelectedItem != null && comboBoxTeams.SelectedItem != null)
            {
                Project project = comboBoxProjects.SelectedItem as Project;
                Team team = comboBoxTeam.SelectedItem as Team;

                if(project.AssignStatus == AssignStatus.UNASSIGNED && team.ScrumMaster.Email != "")
                {
                    foreach(var proj in LocalClientDatabase.Instance.AllProjects)
                    {
                        if(proj.Name == project.Name)
                        {
                            proj.TeamName = team.Name;
                            proj.AssignStatus = AssignStatus.ASSIGNED;
                        }
                    }

                    LocalClientDatabase.Instance.proxy.ProjectTeamAssign(project.Name, team.Name);
                }
            }
        }

        private void buttonBack2_Click(object sender, RoutedEventArgs e)
        {
            addEmployeeTabControl.SelectedIndex = 0;
        }

        private void buttonBack3_Click(object sender, RoutedEventArgs e)
        {
            addEmployeeTabControl.SelectedIndex = 0;
        }

        private void buttonBack4_Click(object sender, RoutedEventArgs e)
        {
            addEmployeeTabControl.SelectedIndex = 0;
        }

        private void buttonAddEmployee2_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();
            Team team = new Team();

            if(comboBoxTeamDeveloper.SelectedItem != null)
            {
                team = comboBoxTeamDeveloper.SelectedItem as Team;

                //employee.Team.Add(team);

                employee.Name = textBoxName.Text;
                employee.Surname = textBoxSurname.Text;
                employee.Email = textBoxEmail.Text;
                employee.Password = passwordBoxNew.Password;
                employee.Type = EmployeeType.DEVELOPER;
                employee.Team = team;
                employee.PasswordTimeStamp = DateTime.Now;

                LocalClientDatabase.Instance.proxy.AddEmployee(employee);

                NewEmployeeMessage();
            }
        }

        private void buttonAddEmployee3_Click(object sender, RoutedEventArgs e)
        {
            Employee employee = new Employee();
            Team team = new Team();

            if (comboBoxTeamScrum.SelectedItem != null)
            {

                team = comboBoxTeamDeveloper.SelectedItem as Team;

                foreach (var team1 in LocalClientDatabase.Instance.Teams)
                {
                    if (team1.Name == team.Name && team1.ScrumMaster.Email == null)
                    {

                        employee.Name = textBoxName.Text;
                        employee.Surname = textBoxSurname.Text;
                        employee.Email = textBoxEmail.Text;
                        employee.Password = passwordBoxNew.Password;
                        employee.Type = EmployeeType.SCRUMMASTER;
                        employee.Team = team;
                        employee.PasswordTimeStamp = DateTime.Now;

                        LocalClientDatabase.Instance.proxy.AddEmployee(employee);

                        NewEmployeeMessage();

                    }
                }
            }  
        }

        private void NewEmployeeMessage()
        {
            textBoxName.Text = "";
            textBoxSurname.Text = "";
            textBoxEmail.Text = "";
            passwordBoxNew.Password = "";

            MessageBox.Show("A new employee has been added!", "New employee added", MessageBoxButton.OK);
        }
    }
}
