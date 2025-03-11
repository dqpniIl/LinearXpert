using System;
using System.Linq;
using System.Timers;
using System.Windows;

namespace LinearXpert
{
    /// <summary>
    /// Логика взаимодействия для WindowValidationCaptcha.xaml
    /// </summary>
    public partial class WindowValidationCaptcha : Window
    {
        private string captchaText;
        private Timer timer;
        public WindowValidationCaptcha()
        {
            InitializeComponent();
            GenerateAndSetCaptchaText();
        }
        private void GenerateAndSetCaptchaText()
        {
            captchaText = GenerateRandomCaptchaText();
            LabelCaptcha.Content = captchaText;
        }
        private string GenerateRandomCaptchaText()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 4)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private void BtnCheckClick(object sender, RoutedEventArgs e)
        {
            string userInput = captchaInputTextBox.Text;
            if (userInput == captchaText)
            {
                LabelMSG2.Content = "";
                Close();
            }
            else
            {
                LabelMSG2.Content = "Неверный ввод captcha, \nпопробуйте еще раз через 10 секунд!";
                captchaInputTextBox.Text = string.Empty;
                BtnCheck.IsEnabled = false;
                timer = new Timer();
                timer.Interval = 10000;
                timer.Elapsed += (s, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        BtnCheck.IsEnabled = true;
                        LabelMSG2.Content = "";
                    });
                    timer.Stop();
                };
                timer.Start();
                GenerateAndSetCaptchaText();
            }
        }
    }
}
