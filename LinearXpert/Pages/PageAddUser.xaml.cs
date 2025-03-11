using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddUser.xaml
    /// </summary>
    public partial class PageAddUser : Page
    {
        private Users user = new Users();
        Entities entities = new Entities();
        public PageAddUser(Users selectUser)
        {
            InitializeComponent();
            if (selectUser != null)
            {
                user = selectUser;
            }
            else
            {
                tbPassword.IsEnabled = true;
                btnNewPassword.Visibility = Visibility.Collapsed;
                btnSavePassword.Visibility = Visibility.Collapsed;
                tbPassword.Clear();
            }
            DataContext = user;
            comboGroups.ItemsSource = Entities.GetContext().Groups.ToList();
            comboRole.ItemsSource = Entities.GetContext().Roles.ToList();
            InputPattern inputPattern = new InputPattern();
            tbSurname.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbName.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbPatronymic.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbLogin.PreviewTextInput += inputPattern.TBLogin_TextInput;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StringBuilder errors = new StringBuilder();
                if (string.IsNullOrWhiteSpace(user.login_User))
                    errors.AppendLine("Логин не введён");
                if (string.IsNullOrWhiteSpace(tbSurname.Text))
                    errors.AppendLine("Фамилия пользователя не введена");
                if (string.IsNullOrWhiteSpace(tbName.Text))
                    errors.AppendLine("Имя пользователя не введено");
                if (string.IsNullOrWhiteSpace(tbPatronymic.Text))
                    errors.AppendLine("Отчество пользователя не введено");
                if (comboGroups.SelectedItem == null)
                    errors.AppendLine("Группа не выбрана");
                if (comboRole.SelectedItem == null)
                    errors.AppendLine("Роль не выбрана");
                if (string.IsNullOrWhiteSpace(tbPassword.Text))
                    errors.AppendLine("Пароль не введён");
                if (errors.Length > 0)
                {
                    MessageBox.Show(errors.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (user.Id_User == 0)
                {
                    var existUser = entities.Users.Where(u => u.login_User == tbLogin.Text).FirstOrDefault();
                    if (existUser != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существуют!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    user.password_User = HashFunction.EncryptPassword(tbPassword.Text);
                    Entities.GetContext().Users.Add(user);
                    Entities.GetContext().SaveChanges();
                    var newProfile = new Profiles
                    {
                        Id_Profile = user.Id_User,
                        surname_Profile = tbSurname.Text,
                        name_Profile = tbName.Text,
                        patronymic_Profile = tbPatronymic.Text,
                        id_Group = (comboGroups.SelectedItem as Groups).Id_Group
                    };
                    Entities.GetContext().Profiles.Add(newProfile);
                }
                Entities.GetContext().SaveChanges();
                if (user.id_Role != 1)
                {
                    var teachersForGroups = entities.GroupsOfTeacher.Where(t => t.id_UserTeacher == user.Id_User).ToList();
                    if (teachersForGroups != null)
                        foreach (var zapis in teachersForGroups)
                            entities.GroupsOfTeacher.Remove(zapis);
                    entities.SaveChanges();
                }
                MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                GlobalUser.global_user = user;
                Navigation.MainFrame.Navigate(new PageUsers());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboRole_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((comboRole.SelectedItem as Roles).Id_Role != 3)
            {
                comboGroups.SelectedIndex = 0;
                comboGroups.IsEnabled = false;
                lblGroup.Visibility=Visibility.Collapsed;
                comboGroups.Visibility = Visibility.Collapsed;
            }
            else
            {
                lblGroup.Visibility = Visibility.Visible;
                comboGroups.Visibility = Visibility.Visible;
                comboGroups.IsEnabled = true;
            }
        }
        private void btnNewPassword_Click(object sender, RoutedEventArgs e)
        {
            btnNewPassword.IsEnabled = false;
            tbPassword.IsEnabled = true;
            tbPassword.Clear();
            tbPassword.Focus();
        }
        private void btnSavePassword_Click(object sender, RoutedEventArgs e)
        {
            var existUser = entities.Users.FirstOrDefault(u => u.Id_User == user.Id_User);
            existUser.password_User = HashFunction.EncryptPassword(tbPassword.Text);
            entities.SaveChanges();
            MessageBox.Show("Сохранено успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            tbPassword.Clear();
            tbPassword.Text = "***********";
            tbPassword.IsEnabled = false;
            btnNewPassword.IsEnabled = true;
        }
    }
}
