//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LinearXpert
{
    using System;
    using System.Collections.Generic;
    
    public partial class GroupsOfTeacher
    {
        public int Id_TeacherForGroup { get; set; }
        public int id_UserTeacher { get; set; }
        public int id_Group { get; set; }
    
        public virtual Groups Groups { get; set; }
        public virtual Users Users { get; set; }
    }
}
