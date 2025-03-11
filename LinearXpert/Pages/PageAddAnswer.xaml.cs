using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddAnswer.xaml
    /// </summary>
    public partial class PageAddAnswer : Page
    {
        private Answers answer = new Answers();
        public PageAddAnswer(Answers selectAnswer)
        {
            InitializeComponent();
            if (selectAnswer != null)
            {
                answer = selectAnswer;
            }
            DataContext = answer;
            if (answer.status_Answer == "Правильный")
                chkBoxStatus.IsChecked = true;
            else
                chkBoxStatus.IsChecked = false;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(answer.name_Answer))
                errors.AppendLine("Ответ не введен");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (chkBoxStatus.IsChecked == true)
                answer.status_Answer = "Правильный";
            else
                answer.status_Answer = "";
            if (answer.Id_Answer == 0)
            {
                Entities.GetContext().Answers.Add(answer);
                answer.id_Question = GlobalUser.global_question.Id_Question;
            }
            try
            {
                Entities.GetContext().SaveChanges();
                GlobalUser.global_answer = answer;
                Navigation.MainFrame.Navigate(new PageAnswers());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
