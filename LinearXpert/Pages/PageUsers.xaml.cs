using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageUsers.xaml
    /// </summary>
    public partial class PageUsers : Page
    {
        private Users user = new Users();
        public PageUsers()
        {
            InitializeComponent();
            var listWithoutCurrentUser = Entities.GetContext().Users.Where(u => u.Id_User != 11&& u.Id_User != GlobalUser.User.Id_User).ToList();
            dGridUsers.ItemsSource = listWithoutCurrentUser;
            DataContext = user;
            comboSearch.ItemsSource = Entities.GetContext().Roles.ToList();
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                if (GlobalUser.global_user != null)
                    foreach (var item in dGridUsers.Items)
                        if (item == GlobalUser.global_user)
                        {
                            dGridUsers.SelectedItem = item;
                            break;
                        }
            }
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddUser(null));
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddUser((sender as Button).DataContext as Users));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы точно хотите удалить следующие {dGridUsers.SelectedItems.Count} пользователей(я)?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Entities context = Entities.GetContext();
                foreach (var user in dGridUsers.SelectedItems.Cast<Users>().ToList())
                {
                    var profile_results = context.Results.Where(r => r.id_Profile == user.Id_User).ToList();
                    if (profile_results != null)
                    {
                        foreach (var result in profile_results)
                            context.Results.Remove(result);
                    }
                    var profiles = context.Profiles.Where(r => r.Id_Profile == user.Id_User).ToList();
                    if (profiles != null)
                    {
                        foreach (var profile in profiles)
                            context.Profiles.Remove(profile);
                    }
                    var teachersForGroups = context.GroupsOfTeacher.Where(t => t.id_UserTeacher == user.Id_User).ToList();
                    if (teachersForGroups != null)
                    {
                        foreach (var zapis in teachersForGroups)
                            context.GroupsOfTeacher.Remove(zapis);
                    }
                    var user_remove = context.Users.Find(user.Id_User);
                    if (user_remove != null)
                    {
                        context.Users.Remove(user_remove);
                    }
                }
                context.SaveChanges();
                MessageBox.Show("Данные удалены!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                var listWithoutCurrentUser = Entities.GetContext().Users.Where(u => u.Id_User != GlobalUser.User.Id_User).ToList();
                dGridUsers.ItemsSource = listWithoutCurrentUser;
            }
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();
            var collectionView = CollectionViewSource.GetDefaultView(dGridUsers.ItemsSource);
            if (collectionView != null)
            {
                if (string.IsNullOrEmpty(searchText))
                {
                    collectionView.Filter = null;
                }
                else
                {
                    collectionView.Filter = item =>
                    {
                        Users zapis_User = item as Users;
                        if (zapis_User != null)
                        {
                            return zapis_User.login_User.ToLower().Contains(searchText)
                            || zapis_User.Roles.name_Role.ToLower().Contains(searchText)
                            || zapis_User.Profiles.surname_Profile.ToLower().Contains(searchText)
                            || zapis_User.Profiles.name_Profile.ToLower().Contains(searchText)
                            || zapis_User.Profiles.patronymic_Profile.ToLower().Contains(searchText)
                            || zapis_User.Profiles.Groups.name_Group.ToLower().Contains(searchText);
                        }
                        return false;
                    };
                }
            }
        }
        private void comboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var rolesList = Entities.GetContext().Users.ToList();
            if (comboSearch.SelectedIndex > -1)
            {
                var selected_item = comboSearch.SelectedItem as Roles;
                rolesList = rolesList.Where(r => r.Roles.Id_Role == selected_item.Id_Role).OrderBy(r => r.Profiles.surname_Profile).ToList();
            }
            if (comboSearch.SelectedIndex == 0)
            {
                rolesList = rolesList.Where(r => r.Id_User !=11 && r.Id_User != GlobalUser.User.Id_User).OrderBy(r => r.Profiles.surname_Profile).ToList();
            }
            dGridUsers.ItemsSource = rolesList;
        }
        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            tbSearch.Clear();
            comboSearch.SelectedIndex = -1;
            var listWithoutCurrentUser = Entities.GetContext().Users.Where(u => u.Id_User != GlobalUser.User.Id_User).ToList();
            dGridUsers.ItemsSource = listWithoutCurrentUser;
        }
        private void BtnSetGroups_Click(object sender, RoutedEventArgs e)
        {
            GlobalUser.global_user = dGridUsers.SelectedItem as Users;
            if (GlobalUser.global_user.id_Role == 2)
                Navigation.MainFrame.Navigate(new PageGroups());
            else
                MessageBox.Show("Страница настройки групп нужна для связывания преподавателя и групп. На нее нельзя перейти, так как вы выбрали не преподавателя. " +
                    "Пожалуйста выберите преподавателя из списка и повторите попытку перехода!", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
