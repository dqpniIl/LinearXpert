using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageCalculator.xaml
    /// </summary>
    public partial class PageCalculator : Page
    {
        private ObservableCollection<ObservableCollection<string>> _matrixData;
        public PageCalculator()
        {
            InitializeComponent();
            lblResult.Visibility = Visibility.Hidden;
            txtbResult.Visibility = Visibility.Hidden;
            dGridData.Visibility = Visibility.Hidden;
            btnCalculate.Visibility = Visibility.Hidden;
            btnExportTXT.Visibility = Visibility.Hidden;
            tbRows.PreviewTextInput += Numbers_Text;
            tbColumns.PreviewTextInput += Numbers_Text;
            dGridData.PreviewTextInput += NumbersWithMinus_Text;
        }
        private void Numbers_Text(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9]");
            if (regex.IsMatch(e.Text))
                e.Handled = true;
        }
        private void NumbersWithMinus_Text(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9-]");
            if (regex.IsMatch(e.Text))
                e.Handled = true;
        }
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            tbRows.Clear();
            tbColumns.Clear();
            tbRows.IsEnabled = true;
            tbColumns.IsEnabled = true;
            btnCreateTable.IsEnabled = true;
            dGridData.ItemsSource = null;
            btnCalculate.Visibility = Visibility.Hidden;
            btnExportTXT.Visibility = Visibility.Hidden;
            dGridData.Visibility = Visibility.Hidden;
            lblResult.Visibility = Visibility.Hidden;
            txtbResult.Text = "";
            txtbResult.Visibility = Visibility.Hidden;
        }
        private void btnCreateTable_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbRows.Text, out int rows) && int.TryParse(tbColumns.Text, out int columns))
            {
                if (rows > 7 || columns > 7)
                {
                    MessageBox.Show("Ограничение (Можно вводить не больше 7 строк или не больше 7 столбцов)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                dGridData.Columns.Clear();
                _matrixData = new ObservableCollection<ObservableCollection<string>>();
                for (int i = 0; i < columns; i++)
                {
                    dGridData.Columns.Add(new DataGridTextColumn
                    {
                        Header = $"С{i + 1}",
                        Binding = new System.Windows.Data.Binding($"[{i}]")
                    });
                }
                for (int i = 0; i < rows; i++)
                {
                    var row = new ObservableCollection<string>();
                    for (int j = 0; j < columns; j++)
                    {
                        row.Add(string.Empty);
                    }
                    _matrixData.Add(row);
                }
                dGridData.Visibility = Visibility.Visible;
                btnCalculate.Visibility = Visibility.Visible;
                tbRows.IsEnabled = false;
                tbColumns.IsEnabled = false;
                btnCreateTable.IsEnabled = false;
                dGridData.ItemsSource = _matrixData;
            }
            else
            {
                MessageBox.Show("Пожалуйста введите количество строк и столбцов правильно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (_matrixData != null && _matrixData.Count > 0)
            {
                bool check = false;
                int rows = _matrixData.Count;
                int columns = _matrixData[0].Count;
                double[,] table = new double[rows, columns];
                for (int i = 0; i < rows; i++)
                    for (int j = 0; j < columns; j++)
                    {
                        if (double.TryParse(_matrixData[i][j], out double matrixElement))
                            table[i, j] = matrixElement;
                        else
                        {
                            check = true;
                            break;
                        }
                    }
                if (check)
                {
                    MessageBox.Show("Неправильные данные в исходной таблице. Исправьте таблицу и данные в ней, и попробуйте снова рассчитать результат!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                double[] result = new double[columns - 1];
                double[,] table_result;
                Simplex S = new Simplex(table);
                table_result = S.Calculate(result);
                dGridData.Visibility = Visibility.Collapsed;
                btnCalculate.Visibility = Visibility.Collapsed;
                lblResult.Visibility = Visibility.Visible;
                txtbResult.Visibility = Visibility.Visible;
                btnExportTXT.Visibility = Visibility.Visible;
                txtbResult.Text += "Исходная таблица:\n\n";
                for (int i = 0; i < table.GetLength(0); i++)
                {
                    for (int j = 0; j < table.GetLength(1); j++)
                        txtbResult.Text += "\t" + table[i, j].ToString() + " ";
                    txtbResult.Text += "\n";
                }
                txtbResult.Text += "\nРешенная симплекс-таблица:\n\n";
                for (int i = 0; i < table_result.GetLength(0); i++)
                {
                    for (int j = 0; j < table_result.GetLength(1); j++)
                        txtbResult.Text += "\t" + table_result[i, j].ToString() + " ";
                    txtbResult.Text += "\n";
                }
                txtbResult.Text += "\nРешение:";
                int v = 0;
                foreach (int r in result)
                {
                    v++;
                    txtbResult.Text += $" X[{v}] = " + r.ToString() + ";";
                }
            }
        }
        private void btnExportTXT_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Report");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = $"ResultCalculate_{timestamp}.txt";
            string filePath = Path.Combine(folderPath, fileName);
            string textToExport = txtbResult.Text;
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.Write(textToExport);
                }
                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
                MessageBox.Show("Результат успешно сохранен!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при сохранении результата: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private class Simplex
        {
            //source - симплекс таблица без базисных переменных
            double[,] table; //симплекс таблица

            int m, n;

            List<int> basis; //список базисных переменных

            public Simplex(double[,] source)
            {
                m = source.GetLength(0);
                n = source.GetLength(1);
                table = new double[m, n + m - 1];
                basis = new List<int>();

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < table.GetLength(1); j++)
                    {
                        if (j < n)
                            table[i, j] = source[i, j];
                        else
                            table[i, j] = 0;
                    }
                    //выставляем коэффициент 1 перед базисной переменной в строке
                    if ((n + i) < table.GetLength(1))
                    {
                        table[i, n + i] = 1;
                        basis.Add(n + i);
                    }
                }

                n = table.GetLength(1);
            }

            //result - в этот массив будут записаны полученные значения X
            public double[,] Calculate(double[] result)
            {
                int mainCol, mainRow; //ведущие столбец и строка

                while (!IsItEnd())
                {
                    mainCol = findMainCol();
                    mainRow = findMainRow(mainCol);
                    basis[mainRow] = mainCol;

                    double[,] new_table = new double[m, n];

                    for (int j = 0; j < n; j++)
                        new_table[mainRow, j] = table[mainRow, j] / table[mainRow, mainCol];

                    for (int i = 0; i < m; i++)
                    {
                        if (i == mainRow)
                            continue;

                        for (int j = 0; j < n; j++)
                            new_table[i, j] = table[i, j] - table[i, mainCol] * new_table[mainRow, j];
                    }
                    table = new_table;
                }

                //заносим в result найденные значения X
                for (int i = 0; i < result.Length; i++)
                {
                    int k = basis.IndexOf(i + 1);
                    if (k != -1)
                        result[i] = table[k, 0];
                    else
                        result[i] = 0;
                }

                return table;
            }

            private bool IsItEnd()
            {
                bool flag = true;

                for (int j = 1; j < n; j++)
                {
                    if (table[m - 1, j] < 0)
                    {
                        flag = false;
                        break;
                    }
                }

                return flag;
            }

            private int findMainCol()
            {
                int mainCol = 1;

                for (int j = 2; j < n; j++)
                    if (table[m - 1, j] < table[m - 1, mainCol])
                        mainCol = j;

                return mainCol;
            }

            private int findMainRow(int mainCol)
            {
                int mainRow = 0;

                for (int i = 0; i < m - 1; i++)
                    if (table[i, mainCol] > 0)
                    {
                        mainRow = i;
                        break;
                    }

                for (int i = mainRow + 1; i < m - 1; i++)
                    if ((table[i, mainCol] > 0) && ((table[i, 0] / table[i, mainCol]) < (table[mainRow, 0] / table[mainRow, mainCol])))
                        mainRow = i;

                return mainRow;
            }
        }
    }
}