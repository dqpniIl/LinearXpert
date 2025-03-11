using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace LinearXpert
{
    /// <summary>
    /// Логика взаимодействия для WindowAuthorization.xaml
    /// </summary>
    public partial class WindowAuthorization : Window
    {
        Entities entities = new Entities();
        private int attempt = 0;
        public WindowAuthorization()
        {
            InitializeComponent();
            InputPattern inputPattern = new InputPattern();
            tbLogin.PreviewTextInput += inputPattern.TBLogin_TextInput;
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbLogin.Text != "" && pbPassword.Password != "")
                {
                    var existUser = entities.Users.Where(user => user.login_User == tbLogin.Text).FirstOrDefault();
                    if (existUser != null)
                    {
                        if (existUser.password_User == HashFunction.EncryptPassword(pbPassword.Password))
                        {
                            GlobalUser.User = existUser;
                            var windowMainWindow = new MainWindow();
                            windowMainWindow.Show();
                            Window currentWindow = GetWindow(this);
                            currentWindow.Close();
                        }
                        else
                        {
                            MessageBox.Show("Пароль не подходит к пользователю", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            attempt++;
                            if (attempt == 3)
                            {
                                attempt = 0;
                                var windowCaptcha = new WindowValidationCaptcha();
                                windowCaptcha.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Такого пользователя не существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        attempt++;
                        if (attempt == 3)
                        {
                            attempt = 0;
                            var windowCaptcha = new WindowValidationCaptcha();
                            windowCaptcha.ShowDialog();
                        }
                    }
                }
                else
                    if (tbLogin.Text == "" && pbPassword.Password == "")
                    MessageBox.Show("Введите логин и пароль!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (tbLogin.Text == "")
                    MessageBox.Show("Введите логин!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                    MessageBox.Show("Введите пароль!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            var windowRegistration = new WindowRegistration();
            windowRegistration.Show();
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
