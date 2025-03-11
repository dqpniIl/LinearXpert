using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddQuestion.xaml
    /// </summary>
    public partial class PageAddQuestion : Page
    {
        private Questions question = new Questions();
        Entities entities = new Entities();
        public PageAddQuestion(Questions selectQuestion)
        {
            InitializeComponent();
            if (selectQuestion != null)
            {
                question = selectQuestion;
            }
            DataContext = question;
            if (question.id_TypeOfAnswer == 2)
                chkBoxType.IsChecked = true;
            else
                chkBoxType.IsChecked = false;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(question.name_Question))
                errors.AppendLine("Вопрос не введен");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (chkBoxType.IsChecked == true)
                question.id_TypeOfAnswer = 2;
            else
                question.id_TypeOfAnswer = 1;
            if (question.Id_Question == 0)
            {
                Entities.GetContext().Questions.Add(question);
            }
            else
            {
                try
                {
                    var existingQuestion = entities.Questions.FirstOrDefault(q => q.Id_Question == question.Id_Question);
                    if (existingQuestion != null)
                    {
                        existingQuestion.name_Question = question.name_Question;
                        existingQuestion.id_TypeOfAnswer = question.id_TypeOfAnswer;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            entities.SaveChanges();
            Entities.GetContext().SaveChanges();
            GlobalUser.global_question = question;
            Navigation.MainFrame.Navigate(new PageQuestions());
        }
    }
}
