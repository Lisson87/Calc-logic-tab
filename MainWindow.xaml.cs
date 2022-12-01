using System;
using System.Collections;
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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tbox.Focus();
        }

        /// <summary>
        /// запись в текст бокс символа соответствующего нажатой кнопке
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button selectedButton = (Button)e.Source;
            tbox.Text = tbox.Text + selectedButton.Content.ToString();
        }

        /// <summary>
        /// Очищение текстового бокса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAc_Click(object sender, RoutedEventArgs e)
        {
            tbox.Text = "";
        }

        /// <summary>
        /// Удаление последнего символа в текстовом боксе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBackspace_Click(object sender, RoutedEventArgs e)
        {
            tbox.Text = tbox.Text.Substring(0, tbox.Text.Length - 1);
        }

        private void tbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string s = tbox.Text.Length.ToString();
            lbl1.Content = s;
            lbl1.ToolTip = s;
        }

        private void tbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnOk_Click(sender, e);
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (MathLogic.checking_input(tbox.Text) == 0)
            {
                MathLogic mathLogic = new MathLogic(tbox.Text);

                // открытие окна результатов
                Window_result wresult = new Window_result();
                wresult.Owner = this;
                wresult.Title = "Результат для " + tbox.Text;
                wresult.Show();

                wresult.Text_print.Text += mathLogic.getTable();        //ВЫВОД результата в Первое 1 окно
                wresult.Text_print2.Text += mathLogic.getResult();    //ВЫВОД Результата во Второе 2 окно.

            }
        }
    }
}
