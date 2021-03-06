﻿using ClientCommon.Data;
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
using ICommon;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for DevAndLeaderWork.xaml
    /// </summary>
    public partial class LeaderWork : UserControl
    {
        public LeaderWork()
        {
            InitializeComponent();
        }

        private void TextBoxUserStoryTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "" && textBoxUserStoryAccCrit.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void TextBoxUserStoryContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "" && textBoxUserStoryAccCrit.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void TextBoxUserStoryDifficulty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (buttonUserStoryAdd == null)
            {
                return;
            }  

            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "" && textBoxUserStoryAccCrit.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void TextBoxUserStoryAccCrit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "" && textBoxUserStoryAccCrit.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void ButtonUserStoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxProjects.SelectedItem != null)
            {
                Project project = comboBoxProjects.SelectedItem as Project;

                if (project.ProgressStatus == ProgressStatus.INPREP)
                {
                    UserStory userStory = new UserStory();
                    userStory.Title = textBoxUserStoryTitle.Text;
                    userStory.Description = textBoxUserStoryContent.Text;
                    userStory.Difficulty = int.Parse(textBoxUserStoryDifficulty.Text);
                    //userStory.Tasks = new List<ClientCommon.Data.Task>();

                    userStory.AcceptStatus = AcceptStatus.PENDING;
                    userStory.ProgressStatus = ProgressStatus.INPREP;
                    userStory.Project = project;
                    
                    comboBoxProjects.Items.Refresh();
                    comboBoxProjects.SelectedItem = null;
                    textBoxUserStoryTitle.Text = "";
                    textBoxUserStoryContent.Text = "";
                    textBoxUserStoryAccCrit.Text = "";
                    textBoxUserStoryDifficulty.Text = "2";

                    if (project.UserStories == null)
                    {
                        project.UserStories = new List<UserStory>();
                    }

                    project.UserStories.Add(userStory);

                    LocalClientDatabase.Instance.UserStories.Add(userStory);

                    LocalClientDatabase.Instance.Proxy.AddUserStory(userStory, project.Name);

                    MessageBox.Show("User story has been added!");
                }
                else
                {
                    MessageBox.Show("You cannot add user stories to this project!");
                }
            }
            else
            {
                MessageBox.Show("You must select a project!");
            }
        }

        private void ButtonUserStorySend_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxProjectsSend.SelectedItem != null)
            {
                Project project = comboBoxProjectsSend.SelectedItem as Project;

                if (project.ProgressStatus == ProgressStatus.INPREP)
                {
                    List<UserStoryCommon> storiesToSend = new List<UserStoryCommon>();

                    project.ProgressStatus = ProgressStatus.PENDING;

                    dataGridProjects.Items.Refresh();
                    dataGridProjects1.Items.Refresh();

                    comboBoxProjectsSend.Items.Refresh();

                    foreach (var userStory in project.UserStories)
                    {
                        UserStoryCommon commStory = new UserStoryCommon();
                        commStory.Title = userStory.Title;
                        commStory.Description = userStory.Description;
                        commStory.AcceptanceCriteria = "acceptance criteria";

                        storiesToSend.Add(commStory);
                    }

                    LocalClientDatabase.Instance.Proxy.SendUserStories(storiesToSend, project.Name);
                }
                else
                {
                    MessageBox.Show("Action is not allowed for this project!");
                }
            }
        }

        private void TextBoxTaskTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTaskTitle.Text != "" && textBoxTaskContent.Text != "")
            {
                buttonTaskAdd.IsEnabled = true;
            }
            else
            {
                buttonTaskAdd.IsEnabled = false;
            }
        }

        private void TextBoxTaskContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxTaskTitle.Text != "" && textBoxTaskContent.Text != "")
            {
                buttonTaskAdd.IsEnabled = true;
            }
            else
            {
                buttonTaskAdd.IsEnabled = false;
            }
        }

        private void ButtonTaskAdd_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxStories.SelectedItem != null)
            {
                UserStory userStory = comboBoxStories.SelectedItem as UserStory;

                if (userStory.AcceptStatus == AcceptStatus.ACCEPTED && userStory.ProgressStatus == ProgressStatus.INPREP)
                {
                    ClientCommon.Data.Task task = new ClientCommon.Data.Task();

                    task.Title = textBoxTaskTitle.Text;
                    task.Description = textBoxTaskContent.Text;
                    task.UserStory = userStory;
                    task.AssignStatus = AssignStatus.UNASSIGNED;
                    task.ProgressStatus = ProgressStatus.PENDING;

                    if (userStory.Tasks == null)
                    {
                        userStory.Tasks = new List<ClientCommon.Data.Task>();
                    }

                    userStory.Tasks.Add(task);

                    LocalClientDatabase.Instance.AllTasks.Add(task);

                    LocalClientDatabase.Instance.Proxy.AddTask(task);

                    textBoxTaskTitle.Text = "";
                    textBoxTaskContent.Text = "";
                    comboBoxStories.SelectedItem = null;

                    MessageBox.Show("A task has been added!");
                }
                else
                {
                    MessageBox.Show("The selected user story has not been accepted!");
                }
            }
        }

        private void ButtonFinishStory_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxStories1.SelectedItem != null)
            {
                UserStory userStory = comboBoxStories1.SelectedItem as UserStory;

                if (userStory.AcceptStatus == AcceptStatus.ACCEPTED && userStory.ProgressStatus == ProgressStatus.INPREP)
                {
                    userStory.ProgressStatus = ProgressStatus.STARTED;
                    userStory.Deadline = DateTime.Now;
                    userStory.Deadline = userStory.Deadline.AddDays(3);

                    foreach (var task in userStory.Tasks)
                    {
                        task.ProgressStatus = ProgressStatus.STARTED;
                    }

                    dataGridUserStories.Items.Refresh();
                    dataGridUserStories1.Items.Refresh();
                    dataGridTasks.Items.Refresh();

                    LocalClientDatabase.Instance.Proxy.ReleaseUserStory(userStory);

                    MessageBox.Show("User story has been sent to team developers!");
                }
                else
                {
                    MessageBox.Show("The selected user story has not been accepted\n or it is closed for change!");
                }
            }
        }

        private void ButtonTaskClaim_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxAllTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxAllTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.AssignStatus == AssignStatus.UNASSIGNED && task.ProgressStatus == ProgressStatus.STARTED)
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
                    MessageBox.Show("This task is not ready for claiming yet\n or it has already been claimed! Chack status.");
                }
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
    }
}
