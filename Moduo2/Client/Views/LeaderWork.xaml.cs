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

        private void textBoxUserStoryTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void textBoxUserStoryContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void textBoxUserStoryDifficulty_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (buttonUserStoryAdd == null)
                return;

            if (textBoxUserStoryTitle.Text != "" && textBoxUserStoryContent.Text != "" && textBoxUserStoryDifficulty.Text != "")
            {
                buttonUserStoryAdd.IsEnabled = true;
            }
            else
            {
                buttonUserStoryAdd.IsEnabled = false;
            }
        }

        private void buttonUserStoryAdd_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxProjects.SelectedItem != null)
            {
                Project project = comboBoxProjects.SelectedItem as Project;

                if(project.ProgressStatus == ProgressStatus.INPREP)
                {
                    UserStory userStory = new UserStory();
                    userStory.Title = textBoxUserStoryTitle.Text;
                    userStory.Description = textBoxUserStoryContent.Text;
                    userStory.Difficulty = int.Parse(textBoxUserStoryDifficulty.Text);

                    userStory.AcceptStatus = AcceptStatus.PENDING;
                    userStory.ProgressStatus = ProgressStatus.INPREP; 

                    userStory.Project = project;

                    comboBoxProjects.Items.Refresh();
                    comboBoxProjects.SelectedItem = null;
                    textBoxUserStoryTitle.Text = "";
                    textBoxUserStoryContent.Text = "";
                    textBoxUserStoryDifficulty.Text = "2";

                    //project.UserStories.Add(userStory);

                    LocalClientDatabase.Instance.UserStories.Add(userStory);

                    //LocalClientDatabase.Instance.proxy.AddUserStory(userStory);

                    MessageBox.Show("User story has been added");
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

        private void buttonUserStorySend_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxProjectsSend.SelectedItem != null)
            {
                Project project = comboBoxProjectsSend.SelectedItem as Project;

                if(project.ProgressStatus == ProgressStatus.INPREP)
                {
                    List<UserStoryCommon> storiesToSend = new List<UserStoryCommon>();

                    project.ProgressStatus = ProgressStatus.PENDING;

                    comboBoxProjectsSend.Items.Refresh();

                    foreach (var userStory in project.UserStories)
                    {
                        UserStoryCommon commStory = new UserStoryCommon();
                        commStory.Title = userStory.Title;
                        commStory.Description = userStory.Description;

                        storiesToSend.Add(commStory);
                    }

                    LocalClientDatabase.Instance.proxy.SendUserStories(storiesToSend, project.Name);
                }
                else
                {
                    MessageBox.Show("Action is not allowed for this project!");
                }
            }
        }

        private void textBoxTaskTitle_TextChanged(object sender, TextChangedEventArgs e)
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

        private void textBoxTaskContent_TextChanged(object sender, TextChangedEventArgs e)
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

        private void buttonTaskAdd_Click(object sender, RoutedEventArgs e)
        {
            if( comboBoxStories.SelectedItem != null)
            {
                UserStory userStory = comboBoxStories.SelectedItem as UserStory;

                if(userStory.AcceptStatus != AcceptStatus.ACCEPTED && userStory.ProgressStatus == ProgressStatus.INPREP)
                {
                    ClientCommon.Data.Task task = new ClientCommon.Data.Task();

                    task.Title = textBoxTaskTitle.Text;
                    task.Description = textBoxTaskContent.Text;
                    task.UserStory = userStory;
                    task.AssignStatus = AssignStatus.UNASSIGNED;
                    task.ProgressStatus = ProgressStatus.PENDING;

                    LocalClientDatabase.Instance.AllTasks.Add(task);

                    LocalClientDatabase.Instance.proxy.AddTask(task);

                }
                else
                {
                    MessageBox.Show("The selected user story i=has not been accepted!");
                }
            }
        }

        private void buttonFinishStory_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxStories1.SelectedItem != null)
            {
                UserStory userStory = comboBoxStories1.SelectedItem as UserStory;

                if (userStory.AcceptStatus != AcceptStatus.ACCEPTED && userStory.ProgressStatus == ProgressStatus.INPREP)
                {
                    userStory.ProgressStatus = ProgressStatus.STARTED;
                    userStory.Deadline = DateTime.Now;
                    userStory.Deadline.AddDays(3);

                    LocalClientDatabase.Instance.proxy.ReleaseUserStory(userStory);
                }
                else
                {
                    MessageBox.Show("The selected user story i=has not been accepted!");
                }
            }
        }

        private void buttonTaskClaim_Click(object sender, RoutedEventArgs e)
        {
            if(comboBoxAllTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxAllTasks.SelectedItem as ClientCommon.Data.Task;

                if(task.AssignStatus == AssignStatus.UNASSIGNED)
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

        private void buttonTaskComplete_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxMyTasks.SelectedItem != null)
            {
                ClientCommon.Data.Task task = comboBoxMyTasks.SelectedItem as ClientCommon.Data.Task;

                if (task.ProgressStatus == ProgressStatus.STARTED)
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