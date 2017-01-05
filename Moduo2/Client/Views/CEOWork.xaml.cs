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
            if(textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "")
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
            if (textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "")
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
            if (textBoxLeaderName.Text == "" || textBoxLeaderSurname.Text == "" || textBoxLeaderEmail.Text == "")
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
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "")
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
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "")
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
            if (textBoxName.Text == "" || textBoxSurname.Text == "" || textBoxEmail.Text == "")
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
            EmployeeType emt = (EmployeeType)comboBoxType.SelectedItem;

            if (emt == EmployeeType.DEVELOPER)
            {

            }
        }

        private void buttonAddTeam2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonAddTeam3_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxTeamLeader.SelectedItem != null)
            {
                Team newTeam = new Team() { Name = textBoxTeamName.Text, TeamLeader = (Employee)comboBoxTeamLeader.SelectedItem };
            }
        }
    }
}
