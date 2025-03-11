using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageReferenceBook.xaml
    /// </summary>
    public partial class PageReferenceBook : Page
    {
        private ReferenceBook refbook = new ReferenceBook();
        public PageReferenceBook()
        {
            InitializeComponent();
            dGridReferenceBook.ItemsSource = Entities.GetContext().ReferenceBook.ToList();
            DataContext = refbook;
            if (GlobalUser.User.id_Role == 3)
            {
                btnAdd.Visibility = Visibility.Collapsed;
                btnDel.Visibility = Visibility.Collapsed;
            }
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
                dGridReferenceBook.ItemsSource = Entities.GetContext().ReferenceBook.ToList();
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddEntrie(null));
        }
        private void BtnLook_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddEntrie((sender as Button).DataContext as ReferenceBook));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dGridReferenceBook.SelectedItem == null)
                MessageBox.Show("Выберите запись для удаления!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (MessageBox.Show("Вы действительно хотите удалить запись(и)?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Entities context = Entities.GetContext();
                        foreach (var entrie in dGridReferenceBook.SelectedItems.Cast<ReferenceBook>().ToList())
                        {
                            var entrie_remove = context.ReferenceBook.Find(entrie.Id_Entrie);
                            if (entrie_remove != null)
                                context.ReferenceBook.Remove(entrie_remove);
                        }
                        context.SaveChanges();
                        dGridReferenceBook.ItemsSource = context.ReferenceBook.ToList();
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
            var entriesList = Entities.GetContext().ReferenceBook.ToList();
            if (!string.IsNullOrEmpty(searchText))
                entriesList = entriesList.Where(ent => ent.topic_Entrie.ToLower().Contains(searchText)).ToList();
            dGridReferenceBook.ItemsSource = entriesList;
        }
    }
}
