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
    /// Interaction logic for DevWork.xaml
    /// </summary>
    public partial class DevWork : UserControl
    {
        public DevWork()
        {
            InitializeComponent();
        }

        private void buttonTaskClaim_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxAllTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxAllTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.AssignStatus == AssignStatus.UNASSIGNED)
                {
                    task.AssignStatus = AssignStatus.ASSIGNED;
                    task.ProgressStatus = ProgressStatus.STARTED;
                    task.EmployeeName = LocalClientDatabase.Instance.CurrentEmployee.Name + " " + LocalClientDatabase.Instance.CurrentEmployee.Surname;

                    LocalClientDatabase.Instance.MyTasks.Add(task);

                    LocalClientDatabase.Instance.proxy.TaskClaimed(task);
                }
                else
                {
                    MessageBox.Show("This task has already been claimed!");
                }
            }
        }

        private void buttonProjectDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProjects.SelectedItem != null)
            {
                Project proj = dataGridProjects.SelectedItem as Project;

                textProjectDescription.Text = proj.Description;
            }
        }

        private void buttonUserStoryDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridUserStories.SelectedItem != null)
            {
                UserStory userStory = dataGridUserStories.SelectedItem as UserStory;

                textUserStoryDescription.Text = userStory.Description;
            }
        }

        private void buttonMyTaskDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridMyTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = dataGridMyTasks.SelectedItem as ClientCommon.Data.Task;

                textMyTaskDescription.Text = task.Description;
            }
        }

        private void buttonTaskDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = dataGridTasks.SelectedItem as ClientCommon.Data.Task;

                textMyTaskDescription.Text = task.Description;
            }
        }

        private void buttonTaskComplete_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxMyTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxMyTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.ProgressStatus != ProgressStatus.COMPLETED)
                {
                    task.ProgressStatus = ProgressStatus.COMPLETED;
                }
                else
                {
                    MessageBox.Show("This task has already been marked as completed!");
                }
            }
        }
    }
}
