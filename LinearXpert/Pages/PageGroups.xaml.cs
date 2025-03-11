using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageGroups.xaml
    /// </summary>
    public partial class PageGroups : Page
    {
        private Groups group = new Groups();
        Entities entities = new Entities();
        Groups no = Entities.GetContext().Groups.First();
        public PageGroups()
        {
            InitializeComponent();
            dGridGroupsOfTeacher.ItemsSource = Entities.GetContext().Groups.ToList();
            dGridGroups.ItemsSource = Entities.GetContext().Groups.ToList();
            UpdateDataGrid();
            DataContext = group;
            comboUser.Items.Add(GlobalUser.global_user);
            comboUser.SelectedItem = GlobalUser.global_user;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddGroup(null));
        }
        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageAddGroup((sender as Button).DataContext as Groups));
        }
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            if (dGridGroupsOfTeacher.SelectedItem == null && dGridGroups.SelectedItem == null)
                MessageBox.Show("Выберите сначало группу для удаления!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                if (MessageBox.Show("Вы действительно хотите удалить эту группу?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        Groups remove_group = null;
                        if (dGridGroupsOfTeacher.SelectedItem != null)
                            remove_group = dGridGroupsOfTeacher.SelectedItem as Groups;
                        else if (dGridGroups.SelectedItem != null)
                            remove_group = dGridGroups.SelectedItem as Groups;
                        if (remove_group != null)
                        {
                            var groupOfUsers = entities.Profiles.Where(u => u.id_Group == remove_group.Id_Group).ToList();
                            foreach (var teacher in groupOfUsers)
                            {
                                teacher.id_Group = 1;
                            }
                            var groupsOfTeachersRemove = entities.GroupsOfTeacher.Where(g => g.id_Group == remove_group.Id_Group).ToList();
                            foreach (var groupOfTeacher in groupsOfTeachersRemove)
                                entities.GroupsOfTeacher.Remove(groupOfTeacher);
                            entities.Groups.Remove(remove_group);
                            entities.SaveChanges();
                            UpdateDataGrid();
                            MessageBox.Show("Данные удалены!", "Удаление", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Удаляйте только по одной группе! / {ex.Message}", "Удаление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Navigation.MainFrame.Navigate(new PageUsers());
        }
        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            var minusGroup = dGridGroupsOfTeacher.SelectedItem as Groups;
            entities.GroupsOfTeacher.Remove(entities.GroupsOfTeacher.Where(gt => gt.id_UserTeacher == GlobalUser.global_user.Id_User && gt.id_Group == minusGroup.Id_Group).FirstOrDefault());
            entities.SaveChanges();
            UpdateDataGrid();
        }
        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            var plusQuestion = dGridGroups.SelectedItem as Groups;
            GroupsOfTeacher groupOfTeacher = new GroupsOfTeacher()
            {
                id_UserTeacher = GlobalUser.global_user.Id_User,
                id_Group = plusQuestion.Id_Group
            };
            entities.GroupsOfTeacher.Add(groupOfTeacher);
            entities.SaveChanges();
            UpdateDataGrid();
        }
        private void tbSearchGroupsOfTeacher_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearchGroupsOfTeacher.Text.ToLower();
            var selectedUserTeacher = GlobalUser.global_user;
            var groupsOfTeacherList = Entities.GetContext().GroupsOfTeacher.Where(g => g.id_UserTeacher == selectedUserTeacher.Id_User).Select(g => g.Groups).ToList();
            var filteredGroups = groupsOfTeacherList.Where(q => q.name_Group.ToLower().Contains(searchText)).ToList();
            dGridGroupsOfTeacher.ItemsSource = filteredGroups;
        }
        private void tbSearchGroups_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = tbSearchGroups.Text.ToLower();
            var selectedUserTeacher = GlobalUser.global_user;
            var groupsOfTeacherList = Entities.GetContext().GroupsOfTeacher.Where(g => g.id_UserTeacher == selectedUserTeacher.Id_User).Select(g => g.Groups).ToList();
            groupsOfTeacherList.Add(no);
            var allGroupsList = Entities.GetContext().Groups.ToList();
            var filteredAllGroups = allGroupsList.Where(g => g.name_Group.ToLower().Contains(searchText)).ToList();
            var filteredGroups = filteredAllGroups.Except(groupsOfTeacherList).ToList();
            dGridGroups.ItemsSource = filteredGroups;
        }
        private void UpdateDataGrid()
        {
            var groupsOfTeacherList = Entities.GetContext().GroupsOfTeacher.ToList();
            var selected_teacher = GlobalUser.global_user;
            groupsOfTeacherList = groupsOfTeacherList.Where(g => g.id_UserTeacher == selected_teacher.Id_User).ToList();
            List<Groups> listGroupsOfTeacher = new List<Groups>();
            List<Groups> listGroups = new List<Groups>();
            foreach (var group in groupsOfTeacherList)
                foreach (var g in entities.Groups)
                    if (group.id_Group == g.Id_Group)
                        listGroupsOfTeacher.Add(g);
            bool check = false;
            foreach (var zapis in entities.Groups)
            {
                foreach (var zapis2 in listGroupsOfTeacher)
                    if (zapis2 == zapis)
                        check = true;
                if (!check)
                    listGroups.Add(zapis);
                check = false;
            }
            dGridGroupsOfTeacher.ItemsSource = listGroupsOfTeacher;
            dGridGroups.ItemsSource = listGroups.Skip(1).ToList();
        }
    }
}
