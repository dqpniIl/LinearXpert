using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAddGroup.xaml
    /// </summary>
    public partial class PageAddGroup : Page
    {
        private Groups group = new Groups();
        Entities entities = new Entities();
        public PageAddGroup(Groups selectGroup)
        {
            InitializeComponent();
            if (selectGroup != null)
            {
                group = selectGroup;
            }
            DataContext = group;
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.GoBack();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(group.name_Group))
                errors.AppendLine("Название группы не введено");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (group.Id_Group == 0)
            {
                Entities.GetContext().Groups.Add(group);
            }
            else
            {
                try
                {
                    var existingGroup = entities.Groups.FirstOrDefault(g => g.Id_Group == group.Id_Group);
                    if (existingGroup != null)
                    {
                        existingGroup.name_Group = group.name_Group;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            var existGroup = entities.Groups.Where(g => g.name_Group == group.name_Group).FirstOrDefault();
            if (existGroup != null)
            {
                MessageBox.Show("Группа с таким названием уже существуют!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            entities.SaveChanges();
            Entities.GetContext().SaveChanges();
            MessageBox.Show("Данные сохранены!", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
            GlobalUser.global_group = group;
            Navigation.MainFrame.Navigate(new PageGroups());
        }
    }
}
