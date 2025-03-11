namespace LinearXpert
{
    public partial class Tests
    {
        public override string ToString()
        {
            if (GlobalUser.global_test != null)
                return "Тест: " + GlobalUser.global_test.name_Test;
            else
                return "Тест: (нет данных)";
        }
    }
    public partial class Questions
    {
        public override string ToString()
        {
            if (GlobalUser.global_question != null)
                return "Вопрос: " + GlobalUser.global_question.name_Question;
            else
                return "Вопрос: (нет данных)";
        }
    }
    public partial class Answers
    {
        public override string ToString()
        {
            return name_Answer;
        }
    }
    public partial class Groups
    {
        public override string ToString()
        {
            return name_Group;
        }
    }
    public partial class Roles
    {
        public override string ToString()
        {
            return name_Role;
        }
    }
    public partial class Users
    {
        public override string ToString()
        {
            return Roles.name_Role + " " + Profiles.surname_Profile + " " + Profiles.name_Profile + " " + Profiles.patronymic_Profile;
        }
    }
}
