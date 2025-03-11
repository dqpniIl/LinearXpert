using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LinearXpert
{
    internal class InputPattern
    {
        public void TBLogin_TextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^a-zA-Z0-9@*._-]");
            if (regex.IsMatch(e.Text))
                e.Handled = true;
        }
        public void TBProfile_TextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^а-яА-Я]");
            if (regex.IsMatch(e.Text))
                e.Handled = true;
        }
    }
}
