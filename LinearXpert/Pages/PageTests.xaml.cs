using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageTests.xaml
    /// </summary>
    public partial class PageTests : Page
    {
        private Tests test = new Tests();
        Entities entities = new Entities();
        public PageTests()
        {
            InitializeComponent();
            dGridTests.ItemsSource = Entities.GetContext().Tests.ToList();
            DataContext = test;
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                if (GlobalUser.global_test != null)
                    foreach (var item in dGridTests.Items)
                        if (item == GlobalUser.global_test)
                        {
                            dGridTests.SelectedItem = item;
                            break;
                        }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddTest(null));
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddTest((sender as Button).DataContext as Tests));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dGridTests.SelectedItem == null)
                MessageBox.Show("Выберите сначало тест для удаления!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                var remove_test = dGridTests.SelectedItem as Tests;
                if (remove_test != null)
                {
                    if (MessageBox.Show("Вы действительно хотите удалить этот тест(ы)?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        try
                        {
                            foreach (var test in dGridTests.SelectedItems.Cast<Tests>().ToList())
                            {
                                var results = entities.Results.Where(r => r.id_Test == test.Id_Test).ToList();
                                if (results != null)
                                {
                                    foreach (var result in results)
                                        entities.Results.Remove(result);
                                }
                                var questions = entities.QuestionsOfTest.Where(q => q.id_Test == test.Id_Test).ToList();
                                if (questions != null)
                                {
                                    foreach (var question in questions)
                                        entities.QuestionsOfTest.Remove(question);
                                }
                                var test_remove = entities.Tests.Find(test.Id_Test);
                                if (test_remove != null)
                                {
                                    entities.Tests.Remove(test_remove);
                                }
                            }
                            entities.SaveChanges();
                            dGridTests.ItemsSource = Entities.GetContext().Tests.ToList();
                            MessageBox.Show("Данные удалены!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка при удалении теста! {ex.Message}", "Удаление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearch.Text.ToLower();
            var testList = Entities.GetContext().Tests.ToList();
            if (!string.IsNullOrEmpty(searchText))
                testList = testList.Where(t => t.name_Test.ToLower().Contains(searchText)).ToList();
            dGridTests.ItemsSource = testList;
        }
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            GlobalUser.global_test = dGridTests.SelectedItem as Tests;
            Navigation.MainFrame.Navigate(new PageQuestions());
        }
    }
}
