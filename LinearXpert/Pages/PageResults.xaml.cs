using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace LinearXpert.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageResults.xaml
    /// </summary>
    public partial class PageResults : Page
    {
        Entities entities = new Entities();
        List<Groups> listGroups = new List<Groups>();
        List<Results> listResults = new List<Results>();
        public PageResults()
        {
            InitializeComponent();
            if (GlobalUser.User.id_Role == 1)
            {
                dGridResults.ItemsSource = Entities.GetContext().Results.ToList();
                foreach (var group in entities.Groups)
                {
                    if (group.Id_Group == 1)
                        continue;
                    comboSearch.Items.Add(group);
                }
            }
            else if (GlobalUser.User.id_Role == 2)
            {
                var listTeacherOfGroup = entities.GroupsOfTeacher.Where(g => g.id_UserTeacher == GlobalUser.User.Id_User).ToList();
                foreach (var group in entities.Groups)
                    foreach (var g in listTeacherOfGroup)
                        if (group.Id_Group == g.id_Group)
                            listGroups.Add(group);
                dGridResults.ItemsSource = UpdateDataGrid();
                foreach (var group in listGroups)
                {
                    if (group.Id_Group == 1)
                        continue;
                    comboSearch.Items.Add(group);
                }
            }
            else
            {
                btnLoadData.Visibility = Visibility.Collapsed;
                comboSearch.Visibility = Visibility.Collapsed;
                var listResultsForStudent = entities.Results.Where(r => r.Profiles.Id_Profile == GlobalUser.User.Id_User).ToList();
                dGridResults.ItemsSource = listResultsForStudent;
            }
        }
        private List<Results> UpdateDataGrid()
        {
            foreach (var result in entities.Results)
                foreach (var group in listGroups)
                    if (result.Profiles.id_Group == group.Id_Group)
                        listResults.Add(result);
            return listResults;
        }
        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = tbSearch.Text.ToLower();
            var collectionView = CollectionViewSource.GetDefaultView(dGridResults.ItemsSource);
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
                        Results zapis_Result = item as Results;
                        if (zapis_Result != null)
                        {
                            return zapis_Result.Profiles.surname_Profile.ToLower().Contains(searchText)
                            || zapis_Result.Profiles.name_Profile.ToLower().Contains(searchText)
                            || zapis_Result.Tests.name_Test.ToLower().Contains(searchText)
                            || zapis_Result.percent_Result.ToString().ToLower().Contains(searchText)
                            || zapis_Result.date_Result.ToLower().Contains(searchText);
                        }
                        return false;
                    };
                }
            }
        }
        private void comboSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var resultsList = Entities.GetContext().Results.ToList();
            if (comboSearch.SelectedIndex >-1)
            {
                var selected_item = comboSearch.SelectedItem as Groups;
                resultsList = resultsList.Where(r => r.Profiles.Groups.Id_Group == selected_item.Id_Group).OrderBy(r => r.Profiles.surname_Profile).ToList();
            }
            dGridResults.ItemsSource = resultsList;
        }
        private void btnLoadData_Click(object sender, RoutedEventArgs e)
        {
            string reportFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Report");
            Directory.CreateDirectory(reportFolderPath);
            string fileName = $"ResultsReport_{DateTime.Now:yyyyMMddHHmmss}.docx";
            string filePath = System.IO.Path.Combine(reportFolderPath, fileName);
            using (DocX document = DocX.Create(filePath))
            {
                document.InsertParagraph("Отчет о результатах тестов").FontSize(14).Font(new Font("TimesNewRoman")).Bold().Alignment = Alignment.center;
                Xceed.Document.NET.Table table = document.AddTable(1, 9);
                table.Alignment = Alignment.center;
                table.Rows[0].Cells[0].Paragraphs.First().Append("ID результата");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Группа");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Фамилия");
                table.Rows[0].Cells[3].Paragraphs.First().Append("Имя");
                table.Rows[0].Cells[4].Paragraphs.First().Append("Тест");
                table.Rows[0].Cells[5].Paragraphs.First().Append("Оценка");
                table.Rows[0].Cells[6].Paragraphs.First().Append("Количество правильных ответов");
                table.Rows[0].Cells[7].Paragraphs.First().Append("Процент");
                table.Rows[0].Cells[8].Paragraphs.First().Append("Дата");

                var results = dGridResults.Items.OfType<Results>().ToList();
                foreach (var result in results)
                {
                    Row row = table.InsertRow();

                    row.Cells[0].Paragraphs[0].Append(result.Id_Result.ToString());
                    row.Cells[1].Paragraphs[0].Append(result.Profiles.Groups.name_Group);
                    row.Cells[2].Paragraphs[0].Append(result.Profiles.surname_Profile);
                    row.Cells[3].Paragraphs[0].Append(result.Profiles.name_Profile);
                    row.Cells[4].Paragraphs[0].Append(result.Tests.name_Test);
                    row.Cells[5].Paragraphs[0].Append(result.grade_Result.ToString());
                    row.Cells[6].Paragraphs[0].Append(result.countRightQuestions_Result);
                    row.Cells[7].Paragraphs[0].Append(result.percent_Result.ToString());
                    row.Cells[8].Paragraphs[0].Append(result.date_Result);
                }
                document.InsertTable(table);
                try
                {
                    document.Save();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка при сохранении файла: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                System.Diagnostics.Process.Start("explorer.exe", "/select," + filePath);
            }
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            string excelFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Report\\ResultsReport.xlsx");
            FileInfo excelFile = new FileInfo(excelFilePath);
            using (ExcelPackage package = new ExcelPackage(excelFile))
            {
                string sheetName = "Отчёт_" + DateTime.Now.ToString("yyyyMMddHHmmss");
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetName);
                worksheet.Cells["A1"].Value = "Отчет о результатах тестов";
                worksheet.Cells["A1"].Style.Font.Size = 14;
                worksheet.Cells["A1"].Style.Font.Name = "Times New Roman";
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                worksheet.Cells["A3"].Value = "ID результата";
                worksheet.Cells["B3"].Value = "Группа";
                worksheet.Cells["C3"].Value = "Фамилия";
                worksheet.Cells["D3"].Value = "Имя";
                worksheet.Cells["E3"].Value = "Тест";
                worksheet.Cells["F3"].Value = "Оценка";
                worksheet.Cells["G3"].Value = "Количество правильных ответов";
                worksheet.Cells["H3"].Value = "Процент";
                worksheet.Cells["I3"].Value = "Дата";
                int row = 4;

                var results = dGridResults.Items.OfType<Results>().ToList();
                foreach (var result in results)
                {
                    worksheet.Cells["A" + row].Value = result.Id_Result;
                    worksheet.Cells["B" + row].Value = result.Profiles.Groups.name_Group;
                    worksheet.Cells["C" + row].Value = result.Profiles.surname_Profile;
                    worksheet.Cells["D" + row].Value = result.Profiles.name_Profile;
                    worksheet.Cells["E" + row].Value = result.Tests.name_Test;
                    worksheet.Cells["F" + row].Value = result.grade_Result;
                    worksheet.Cells["G" + row].Value = result.countRightQuestions_Result;
                    worksheet.Cells["H" + row].Value = result.percent_Result;
                    worksheet.Cells["I" + row].Value = result.date_Result;
                    row++;
                }
                row++;
                worksheet.Cells["A" + row].Value = "Общее количество результатов";
                worksheet.Cells["A" + row].Style.Font.Size = 12;
                worksheet.Cells["A" + row].Style.Font.Bold = true;
                worksheet.Cells["A" + row].Style.Font.Name = "Times New Roman";
                worksheet.Cells["A" + row].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                row += 2;
                worksheet.Cells["A" + row].Value = "Процент успеваемости";
                worksheet.Cells["B" + row].Value = "Средняя оценка";
                row++;
                double totalPercent = results.Average(r => r.percent_Result);
                double averageGrade = results.Average(r => r.grade_Result);
                worksheet.Cells["A" + row].Value = totalPercent;
                worksheet.Cells["B" + row].Value = averageGrade;
                package.Save();
            }
            MessageBox.Show("Файлы успешно сохранены!", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
