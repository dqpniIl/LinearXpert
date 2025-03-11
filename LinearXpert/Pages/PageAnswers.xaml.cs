using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAnswers.xaml
    /// </summary>
    public partial class PageAnswers : Page
    {
        private Answers answer = new Answers();
        public PageAnswers()
        {
            InitializeComponent();
            dGridAnswers.ItemsSource = Entities.GetContext().Answers.ToList();
            DataContext = answer;
            comboQuestion.Items.Add(GlobalUser.global_question);
            comboQuestion.SelectedItem = GlobalUser.global_question;
            UpdateDataGrid();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                if (GlobalUser.global_answer != null)
                    foreach (var item in dGridAnswers.Items)
                        if (item == GlobalUser.global_answer)
                        {
                            dGridAnswers.SelectedItem = item;
                            break;
                        }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageQuestions());
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddAnswer(null));
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddAnswer((sender as Button).DataContext as Answers));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dGridAnswers.SelectedItem == null)
                MessageBox.Show("Выберите сначало ответ для удаления!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (MessageBox.Show("Вы действительно хотите удалить этот ответ(ы)?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities context = Entities.GetContext();
                        foreach (var answer in dGridAnswers.SelectedItems.Cast<Answers>().ToList())
                        {
                            var answer_remove = context.Answers.Find(answer.Id_Answer);
                            if (answer_remove != null)
                            {
                                context.Answers.Remove(answer_remove);
                            }
                        }
                        context.SaveChanges();
                        dGridAnswers.ItemsSource = context.Answers.ToList();
                        UpdateDataGrid();
                        MessageBox.Show("Данные удалены!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString(), "Удаление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearch.Text.ToLower();
            var answerList = Entities.GetContext().Answers.Where(a => a.name_Answer.ToLower().Contains(searchText) && a.id_Question == GlobalUser.global_question.Id_Question).ToList();
            dGridAnswers.ItemsSource = answerList;
        }
        private void UpdateDataGrid()
        {
            var answersList = Entities.GetContext().Answers.ToList();
            answersList = answersList.Where(a => a.id_Question == GlobalUser.global_question.Id_Question).ToList();
            dGridAnswers.ItemsSource = answersList;
        }
    }
}
