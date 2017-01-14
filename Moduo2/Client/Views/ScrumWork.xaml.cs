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
    public partial class ScrumWork : UserControl
    {
        public ScrumWork()
        {
            InitializeComponent();
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
