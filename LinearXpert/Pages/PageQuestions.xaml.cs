using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageQuestions.xaml
    /// </summary>
    public partial class PageQuestions : Page
    {
        private Questions question = new Questions();
        Entities entities = new Entities();
        public PageQuestions()
        {
            InitializeComponent();
            dGridQuestionsOfTest.ItemsSource = Entities.GetContext().Questions.ToList();
            dGridQuestions.ItemsSource = Entities.GetContext().Questions.ToList();
            UpdateDataGrid();
            DataContext = question;
            comboTest.Items.Add(GlobalUser.global_test);
            comboTest.SelectedItem = GlobalUser.global_test;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddQuestion(null));
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddQuestion((sender as Button).DataContext as Questions));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dGridQuestionsOfTest.SelectedItem == null && dGridQuestions.SelectedItem == null)
                MessageBox.Show("Выберите вопрос для удаления!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (MessageBox.Show("Вы действительно хотите удалить этот вопрос?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Questions remove_question = null;
                        if (dGridQuestionsOfTest.SelectedItem != null)
                            remove_question = dGridQuestionsOfTest.SelectedItem as Questions;
                        else if (dGridQuestions.SelectedItem != null)
                            remove_question = dGridQuestions.SelectedItem as Questions;
                        if (remove_question != null)
                        {
                            var answersToRemove = entities.Answers.Where(a => a.id_Question == remove_question.Id_Question).ToList();
                            foreach (var answer in answersToRemove)
                                entities.Answers.Remove(answer);
                            var questionsOfTestToRemove = entities.QuestionsOfTest.Where(q => q.id_Question == remove_question.Id_Question).ToList();
                            foreach (var questionOfTest in questionsOfTestToRemove)
                                entities.QuestionsOfTest.Remove(questionOfTest);
                            entities.Questions.Remove(remove_question);
                            entities.SaveChanges();
                            UpdateDataGrid();
                            MessageBox.Show("Данные удалены!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Удаляйте только по одному вопросу! / {ex.Message}", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageTests());
        }
        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            var minusQuestion = dGridQuestionsOfTest.SelectedItem as Questions;
            entities.QuestionsOfTest.Remove(entities.QuestionsOfTest.Where(qt => qt.id_Test == GlobalUser.global_test.Id_Test && qt.id_Question == minusQuestion.Id_Question).FirstOrDefault());
            entities.SaveChanges();
            UpdateDataGrid();
        }
        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            var plusQuestion = dGridQuestions.SelectedItem as Questions;
            QuestionsOfTest questionOfTest = new QuestionsOfTest()
            {
                id_Test = GlobalUser.global_test.Id_Test,
                id_Question = plusQuestion.Id_Question
            };
            entities.QuestionsOfTest.Add(questionOfTest);
            entities.SaveChanges();
            UpdateDataGrid();
        }
        private void tbSearchQuestionsOfTest_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearchQuestionsOfTest.Text.ToLower();
            var selectedTest = GlobalUser.global_test;
            var questionsOfTestList = Entities.GetContext().QuestionsOfTest.Where(a => a.id_Test == selectedTest.Id_Test).Select(a => a.Questions).ToList();
            var filteredQuestions = questionsOfTestList.Where(q => q.name_Question.ToLower().Contains(searchText)).ToList();
            dGridQuestionsOfTest.ItemsSource = filteredQuestions;
        }
        private void tbSearchQuestions_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearchQuestions.Text.ToLower();
            var selectedTest = GlobalUser.global_test;
            // Получение списка вопросов, связанных с выбранным тестом
            var questionsOfTestList = Entities.GetContext().QuestionsOfTest.Where(a => a.id_Test == selectedTest.Id_Test).Select(a => a.Questions).ToList();
            // Фильтрация списка всех вопросов по поисковому запросу
            var allQuestionsList = Entities.GetContext().Questions.ToList();
            var filteredAllQuestions = allQuestionsList.Where(q => q.name_Question.ToLower().Contains(searchText)).ToList();
            // Удаление из списка всех вопросов тех, которые связаны с выбранным тестом
            var filteredQuestions = filteredAllQuestions.Except(questionsOfTestList).ToList();
            // Отображение результата в соответствующем DataGrid
            dGridQuestions.ItemsSource = filteredQuestions;
        }
        private void UpdateDataGrid()
        {
            var questionsOfTestList = Entities.GetContext().QuestionsOfTest.ToList();
            var selected_test = GlobalUser.global_test;
            questionsOfTestList = questionsOfTestList.Where(a => a.id_Test == selected_test.Id_Test).ToList();
            List<Questions> listQuestionsOfTest = new List<Questions>();
            List<Questions> listQuestions = new List<Questions>();
            foreach (var question in questionsOfTestList)
                foreach (var q in entities.Questions)
                    if (question.id_Question == q.Id_Question)
                        listQuestionsOfTest.Add(q);
            bool check = false;
            foreach (var zapis in entities.Questions)
            {
                foreach (var zapis2 in listQuestionsOfTest)
                    if (zapis2 == zapis)
                        check = true;
                if (!check)
                    listQuestions.Add(zapis);
                check = false;
            }
            dGridQuestionsOfTest.ItemsSource = listQuestionsOfTest;
            dGridQuestions.ItemsSource = listQuestions;
        }
        private void btnAnswersQuestionsOfTest_Click(object sender, RoutedEventArgs e)
        {
            GlobalUser.global_question = dGridQuestionsOfTest.SelectedItem as Questions;
            Navigation.MainFrame.Navigate(new PageAnswers());
        }
        private void btnAnswersQuestions_Click(object sender, RoutedEventArgs e)
        {
            GlobalUser.global_question = dGridQuestions.SelectedItem as Questions;
            Navigation.MainFrame.Navigate(new PageAnswers());
        }
    }
}
