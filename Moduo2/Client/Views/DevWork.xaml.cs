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

        private void ButtonTaskClaim_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxAllTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxAllTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.AssignStatus == AssignStatus.UNASSIGNED)
                {
                    task.AssignStatus = AssignStatus.ASSIGNED;
                    task.ProgressStatus = ProgressStatus.STARTED;
                    task.EmployeeName = LocalClientDatabase.Instance.CurrentEmployee.Name;

                    LocalClientDatabase.Instance.MyTasks.Add(task);

                    LocalClientDatabase.Instance.Proxy.TaskClaimed(task);

                    MessageBox.Show("You have successfully claimed a task!");
                }
                else
                {
                    MessageBox.Show("This task has already been claimed!");
                }
            }
        }

        private void ButtonProjectDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridProjects.SelectedItem != null)
            {
                Project proj = dataGridProjects.SelectedItem as Project;

                textProjectDescription.Text = proj.Description;
            }
        }

        private void ButtonUserStoryDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridUserStories.SelectedItem != null)
            {
                UserStory userStory = dataGridUserStories.SelectedItem as UserStory;

                textUserStoryDescription.Text = userStory.Description;
            }
        }

        private void ButtonMyTaskDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridMyTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = dataGridMyTasks.SelectedItem as ClientCommon.Data.Task;

                textMyTaskDescription.Text = task.Description;
            }
        }

        private void ButtonTaskDescription_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = dataGridTasks.SelectedItem as ClientCommon.Data.Task;

                textTaskDescription.Text = task.Description;
            }
        }

        private void ButtonTaskComplete_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxMyTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxMyTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.ProgressStatus != ProgressStatus.COMPLETED)
                {
                    task.ProgressStatus = ProgressStatus.COMPLETED;

                    LocalClientDatabase.Instance.Proxy.TaskCompleted(task);

                    MessageBox.Show("Task completed!");
                }
                else
                {
                    MessageBox.Show("This task has already been marked as completed!");
                }
            }
        }
    }
}
