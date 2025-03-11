using LinearXpert.Pages;
using System.Windows;
using System.Windows.Input;

namespace LinearXpert
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Navigation.MainFrame = MainFrame;
            if (GlobalUser.User.id_Role == 2)
            {
                btnUsers.Visibility = Visibility.Collapsed;
            }
            else if (GlobalUser.User.id_Role == 3)
            {
                btnTestManager.Visibility = Visibility.Collapsed;
                btnUsers.Visibility = Visibility.Collapsed;
            }
            MainFrame.Navigate(new PageReferenceBook());
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void btnReferenceBook_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageReferenceBook());
        }
        private void btnCalculator_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageCalculator());
        }
        private void btnTestManager_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageTests());
        }
        private void btnTestsToPass_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageTestsToPass());
        }
        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageResults());
        }
        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageProfile());
        }
        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PageUsers());
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            var windowAuthorization = new WindowAuthorization();
            windowAuthorization.Show();
            Window currentWindow = GetWindow(this);
            currentWindow.Close();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window currentWindow = GetWindow(this);
            currentWindow.Close();
        }
    }
}
