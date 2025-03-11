using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddEntrie.xaml
    /// </summary>
    public partial class PageAddEntrie : Page
    {
        private ReferenceBook entrie = new ReferenceBook();
        byte[] image_bytes;
        public PageAddEntrie(ReferenceBook selectEntrie)
        {
            InitializeComponent();
            if (selectEntrie != null)
            {
                entrie = selectEntrie;
            }
            DataContext = entrie;
            if (GlobalUser.User.id_Role == 3)
            {
                tbTopic.IsReadOnly = true;
                tbDescription.IsReadOnly = true;
                tbLink.IsReadOnly = true;
                btnClearImage.Visibility = Visibility.Collapsed;
                btnImageChoose.Visibility = Visibility.Collapsed;
                btnSave.Visibility = Visibility.Collapsed;
            }
            else
            {
                Style customStyle = (Style)FindResource("tbForEntrie");
                SolidColorBrush blackBrush = new SolidColorBrush(Colors.Black);
                tbTopic.Style = customStyle;
                tbDescription.Style = customStyle;
                tbLink.Style = customStyle;
                tbTopic.BorderThickness = new Thickness(1);
                tbDescription.BorderThickness = new Thickness(1);
                tbLink.BorderThickness = new Thickness(1);
                tbTopic.BorderBrush = blackBrush;
                tbDescription.BorderBrush = blackBrush;
                tbLink.BorderBrush = blackBrush;
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
        private void btnImageChoose_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage bitmap;
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.ShowDialog();
            string dlgSource = dlg.FileName;
            if (dlgSource != string.Empty)
            {
                image_bytes = File.ReadAllBytes(dlg.FileName);
                string selectedImage = dlg.FileName;
                bitmap = new BitmapImage(new Uri(selectedImage));
                imgEntrie.Source = bitmap;
            }
        }
        private void btnGoLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(tbLink.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии браузера: {ex.Message}");
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(entrie.topic_Entrie))
                errors.AppendLine("Название темы не введено");
            if (string.IsNullOrWhiteSpace(entrie.description_Entrie))
                errors.AppendLine("Описание темы не прописано");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (entrie.Id_Entrie == 0)
                Entities.GetContext().ReferenceBook.Add(entrie);
            if(imgEntrie.Source != null)
                entrie.image_Entrie = image_bytes;
            else
                entrie.image_Entrie = null;
            try
            {
                Entities.GetContext().SaveChanges();
                MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void btnClearImage_Click(object sender, RoutedEventArgs e)
        {
            imgEntrie.Source = null;
        }
    }
}