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

            database = new LocalClientDatabase();
            DataContext = database;

            foreach(var type in Enum.GetValues(typeof(EmployeeType)))
            {
                comboBoxNewType.Items.Add(type);
                comboBoxNewType.SelectedIndex = 0;
                comboBoxType.Items.Add(type);
                comboBoxType.SelectedIndex = 0;
            }
        }
    }
}
