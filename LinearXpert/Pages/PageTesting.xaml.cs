using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageTesting.xaml
    /// </summary>
    public partial class PageTesting : Page
    {
        Entities entities = new Entities();
        Questions[] massivQuestions;
        int currentQuestionIndex = 0; // Индекс текущего вопроса
        int tekballs = 0, allballs = 0, percent = 0, grade = 0, countRightAnswers = 0;
        Answers selectAnswer = null;
        public PageTesting()
        {
            InitializeComponent();
            dGridAnswers.Visibility = Visibility.Hidden;
            LoadQuestionAndAnswers();
        }
        private void LoadQuestionAndAnswers()
        {
            try
            {
                var arrayQuestionsOfTest = entities.QuestionsOfTest.Where(q => q.id_Test == GlobalUser.global_test.Id_Test).Select(q => q.id_Question).ToArray();
                var arrayQuestions = entities.Questions.Where(q => arrayQuestionsOfTest.Contains(q.Id_Question)).OrderBy(x => Guid.NewGuid()).ToArray();
                massivQuestions = arrayQuestions;
                if (currentQuestionIndex == 0)
                    allballs = massivQuestions.Length;
                tbTest.Text = $"Тест: {GlobalUser.global_test.name_Test}";
                tbQuestion.Text = $"Вопрос: {massivQuestions[currentQuestionIndex].name_Question}";
                int parentIdAnswer = massivQuestions[currentQuestionIndex].Id_Question;
                var Answers = entities.Answers.Where(a => a.id_Question == parentIdAnswer).OrderBy(x => Guid.NewGuid()).ToArray();
                if (massivQuestions[currentQuestionIndex].id_TypeOfAnswer == 1)
                {
                    foreach (var answer in Answers)
                        listboxAnswers.Items.Add(answer);
                }
                else
                {
                    listboxAnswers.Visibility = Visibility.Hidden;
                    dGridAnswers.Visibility = Visibility.Visible;
                    foreach (var answer in Answers)
                    {
                        dGridAnswers.Items.Add(answer);
                        if (answer.status_Answer == "Правильный")
                            countRightAnswers += 1;
                    }
                }
            }
            catch
            {
                MessageBox.Show("В этом тесте нет вопросов!", "Ошибка перехода", MessageBoxButton.OK, MessageBoxImage.Error);
                btnAnswer.IsEnabled = false;
                return;
            }
        }
        private void btnAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (massivQuestions[currentQuestionIndex].id_TypeOfAnswer == 1)
            {
                if (selectAnswer == null)
                {
                    MessageBox.Show("Выберите ответ!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if (selectAnswer.status_Answer == "Правильный")
                        tekballs += 1;
                }
            }
            else
            {
                if (dGridAnswers.SelectedItem == null)
                {
                    MessageBox.Show("Выберите ответ!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    List<Answers> selectedItems = new List<Answers>();
                    foreach (var item in dGridAnswers.Items) // Проходимся по элементам в DataGrid
                    {
                        // Получаем ячейку с чекбоксом
                        DataGridCell cell = GetCell(dGridAnswers, item, 0); // Предполагается, что чекбокс находится в первой колонке
                        CheckBox checkBox = FindChild<CheckBox>(cell); // Получаем чекбокс из содержимого ячейки
                        if (checkBox.IsChecked == true) // Проверяем, был ли чекбокс выбран
                        {
                            selectedItems.Add(item as Answers); // Если да, добавляем элемент в список выбранных элементов
                        }
                    }
                    if (selectedItems.Count == 0)
                    {
                        MessageBox.Show("Выберите ответ!", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    if (countRightAnswers == selectedItems.Count)
                    {
                        bool check = false;
                        foreach (var answer in selectedItems)
                        {
                            if (answer.status_Answer == "Правильный")
                                check = true;
                            if (check == false)
                                break;
                        }
                        if (check)
                            tekballs += 1;
                    }
                }
            }
            if (currentQuestionIndex < massivQuestions.Length - 1) // Выполняем следующие действия только если есть еще вопросы
            {
                if (massivQuestions[currentQuestionIndex].id_TypeOfAnswer == 1)
                {
                    listboxAnswers.Items.Clear(); // Очищаем текущие ответы в листбоксе
                }
                else
                {
                    dGridAnswers.Items.Clear();
                    countRightAnswers = 0;
                }
                currentQuestionIndex++; // Увеличиваем индекс текущего вопроса
                tbQuestion.Text = $"Вопрос: {massivQuestions[currentQuestionIndex].name_Question}"; // Загружаем следующий вопрос и ответы
                int parentIdAnswer = massivQuestions[currentQuestionIndex].Id_Question;
                var Answers = entities.Answers.Where(a => a.id_Question == parentIdAnswer).OrderBy(x => Guid.NewGuid()).ToArray();
                if (massivQuestions[currentQuestionIndex].id_TypeOfAnswer == 1)
                {
                    listboxAnswers.Visibility = Visibility.Visible;
                    dGridAnswers.Visibility = Visibility.Hidden;
                    foreach (var answer in Answers)
                        listboxAnswers.Items.Add(answer);
                }
                else
                {
                    listboxAnswers.Visibility = Visibility.Hidden;
                    dGridAnswers.Visibility = Visibility.Visible;
                    foreach (var answer in Answers)
                    {
                        dGridAnswers.Items.Add(answer);
                        if (answer.status_Answer == "Правильный")
                            countRightAnswers += 1;
                    }
                }
            }
            else
            {
                percent = (tekballs * 100) / allballs;
                if (percent < 50)
                    grade = 2;
                if (percent >= 50)
                    grade = 3;
                if (percent >= 75)
                    grade = 4;
                if (percent >= 90)
                    grade = 5;
                Results result = new Results()
                {
                    percent_Result = percent,
                    countRightQuestions_Result = $"{tekballs}/{allballs}",
                    grade_Result = grade,
                    date_Result = DateTime.Now.ToString(),
                    id_Profile = GlobalUser.User.Id_User,
                    id_Test = GlobalUser.global_test.Id_Test
                };
                entities.Results.Add(result);
                entities.SaveChanges();
                MessageBox.Show("Тест выполнен!", "Тестирование", MessageBoxButton.OK, MessageBoxImage.Information);
                Navigation.MainFrame.Navigate(new PageResults());
            }
        }
        private void listboxAnswers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected_answer = listboxAnswers.SelectedItem as Answers;
            if (selected_answer != null)
                selectAnswer = selected_answer;
        }
        private DataGridCell GetCell(DataGrid dataGrid, object item, int columnIndex)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromItem(item);
            if (row != null)
            {
                DataGridCellsPresenter presenter = FindChild<DataGridCellsPresenter>(row);
                if (presenter != null)
                {
                    DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    if (cell == null)
                    {
                        dataGrid.ScrollIntoView(row, dataGrid.Columns[columnIndex]);
                        cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
                    }
                    return cell;
                }
            }
            return null;
        }
        private T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindChild<T>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }
    }
}
