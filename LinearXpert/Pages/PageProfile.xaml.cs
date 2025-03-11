using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageProfile.xaml
    /// </summary>
    public partial class PageProfile : Page
    {
        Entities entities = new Entities();
        public PageProfile()
        {
            InitializeComponent();
            foreach (var group in entities.Groups)
            {
                if (group.Id_Group == 1)
                    continue;
                comboGroups.Items.Add(group);
            }
            InputPattern inputPattern = new InputPattern();
            tbSurname.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbName.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbPatronymic.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbLogin.PreviewTextInput += inputPattern.TBLogin_TextInput;
            var profile = entities.Profiles.Where(p => p.Id_Profile == GlobalUser.User.Id_User).FirstOrDefault();
            if (profile != null)
            {
                tbSurname.Text = profile.surname_Profile;
                tbName.Text = profile.name_Profile;
                tbPatronymic.Text = profile.patronymic_Profile;
                if (profile.id_Group == 1)
                {
                    lblGroup.Visibility = Visibility.Collapsed;
                    comboGroups.Visibility = Visibility.Collapsed;
                }
                else
                    comboGroups.SelectedItem = profile.Groups;
            }
            var existUser = entities.Users.Where(u => u.Id_User == GlobalUser.User.Id_User).FirstOrDefault();
            tbLogin.Text = existUser.login_User;
        }
        private void btnSaveProfile_Click(object sender, RoutedEventArgs e)
        {
            var profile = entities.Profiles.FirstOrDefault(p => p.Id_Profile == GlobalUser.User.Id_User);
            profile.surname_Profile = tbSurname.Text;
            profile.name_Profile = tbName.Text;
            profile.patronymic_Profile = tbPatronymic.Text;
            if (profile.id_Group != 1)
                profile.id_Group = (comboGroups.SelectedItem as Groups).Id_Group;
            entities.SaveChanges();
            MessageBox.Show("Сохранено успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void btnSaveUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = entities.Users.FirstOrDefault(u => u.Id_User == GlobalUser.User.Id_User);
                if (tbPassword.Text != "***********")
                    user.password_User = HashFunction.EncryptPassword(tbPassword.Text);
                if (tbPassword.Text == "")
                {
                    MessageBox.Show("Введите пароль!", "Ошибка сохранения профиля", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                user.login_User = tbLogin.Text;
                entities.SaveChanges();
                MessageBox.Show("Сохранено успешно!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                GlobalUser.User = user;
                tbPassword.Clear();
                tbPassword.Text = "***********";
                tbPassword.IsEnabled = false;
                btnNewPassword.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnNewPassword_Click(object sender, RoutedEventArgs e)
        {
            btnNewPassword.IsEnabled = false;
            tbPassword.IsEnabled = true;
            tbPassword.Clear();
            tbPassword.Focus();
        }
    }
}
