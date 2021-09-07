using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CourseManager.Models
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TermName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Term() { }
    }
}
