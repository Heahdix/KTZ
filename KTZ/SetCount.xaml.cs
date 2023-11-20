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

namespace KTZ
{
    /// <summary>
    /// Логика взаимодействия для SetCount.xaml
    /// </summary>
    public partial class SetCount : Window
    {
        public SetCount()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int columns = Convert.ToInt32(Columns.Text);
                int rows = Convert.ToInt32(Rows.Text);

                if (columns < 2 || rows < 2)
                {
                    MessageBox.Show("Значение должно быть больше 2");
                }
                else
                {
                    MainWindow mainWindow = new MainWindow(rows, columns);
                    mainWindow.Show();
                    this.Close();
                }
            }
            catch
            {
                MessageBox.Show("Введите числовые значения");
            }
        }
    }
}
