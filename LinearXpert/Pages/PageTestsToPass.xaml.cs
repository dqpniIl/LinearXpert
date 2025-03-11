using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageTestsToPass.xaml
    /// </summary>
    public partial class PageTestsToPass : Page
    {
        public PageTestsToPass()
        {
            InitializeComponent();
            var context = Entities.GetContext();
            var testsWithQuestions = context.Tests.Where(test => context.QuestionsOfTest.Any(question => question.id_Test == test.Id_Test)).ToList();
            dGridTests.ItemsSource = testsWithQuestions;
        }
        private void BtnBegin_Click(object sender, RoutedEventArgs e)
        {
            GlobalUser.global_test = dGridTests.SelectedItem as Tests;
            Navigation.MainFrame.Navigate(new PageTesting());
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearch.Text.ToLower();
            var testList = Entities.GetContext().Tests.ToList();
            if (!string.IsNullOrEmpty(searchText))
                testList = testList.Where(t => t.name_Test.ToLower().Contains(searchText)).ToList();
            dGridTests.ItemsSource = testList;
        }
    }
}
