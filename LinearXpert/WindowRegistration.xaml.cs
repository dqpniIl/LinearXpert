using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LinearXpert
{
    /// <summary>
    /// Логика взаимодействия для WindowRegistration.xaml
    /// </summary>
    public partial class WindowRegistration : Window
    {
        Entities entities = new Entities();
        public WindowRegistration()
        {
            InitializeComponent();
            foreach (var group in entities.Groups)
            {
                if (group.Id_Group == 1 || group.Id_Group == 2)
                    continue;
                comboGroups.Items.Add(group);
            }
            InputPattern inputPattern = new InputPattern();
            tbLogin.PreviewTextInput += inputPattern.TBLogin_TextInput;
            tbName.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbSurname.PreviewTextInput += inputPattern.TBProfile_TextInput;
            tbPatronymic.PreviewTextInput += inputPattern.TBProfile_TextInput;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbLogin.Text != "" && tbPassword.Text != "" && tbPasswordRepeat.Text != "" && tbName.Text != "" && tbSurname.Text != "" && tbPatronymic.Text != "" && comboGroups.SelectedIndex != -1)
                {
                    if (tbPassword.Text != tbPasswordRepeat.Text)
                    {
                        MessageBox.Show("Пароли не совпадают", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var existUser = entities.Users.Where(u => u.login_User == tbLogin.Text).FirstOrDefault();
                    if (existUser != null)
                    {
                        MessageBox.Show("Пользователь с таким логином уже существуют!", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    var newUser = new Users
                    {
                        login_User = tbLogin.Text,
                        password_User = HashFunction.EncryptPassword(tbPassword.Text),
                        id_Role = 3
                    };
                    entities.Users.Add(newUser);
                    entities.SaveChanges();
                    GlobalUser.User = newUser;
                    var user = entities.Users.Where(u => u.login_User == newUser.login_User).FirstOrDefault();
                    var newProfile = new Profiles
                    {
                        Id_Profile = user.Id_User,
                        surname_Profile = tbSurname.Text,
                        name_Profile = tbName.Text,
                        patronymic_Profile = tbPatronymic.Text,
                        id_Group = (comboGroups.SelectedItem as Groups).Id_Group
                    };
                    entities.Profiles.Add(newProfile);
                    try
                    {
                        entities.SaveChanges();
                    }
                    catch
                    {
                        MessageBox.Show("Ошибка создания пользователя", "Регистрация", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Window currentWindow = GetWindow(this);
                    currentWindow.Close();
                }
                else
                    MessageBox.Show("Введите все поля", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var windowAuthorization = new WindowAuthorization();
            windowAuthorization.Show();
            Window currentWindow = GetWindow(this);
            currentWindow.Close();
        }
        private void btnPower_Click(object sender, RoutedEventArgs e)
        {
            Window currentWindow = GetWindow(this);
            currentWindow.Close();
        }
    }
}
