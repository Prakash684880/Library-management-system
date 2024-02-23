

namespace project.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Borrow
    {
        [Key]
        public int ID { get; set; }
        public string Book_Name { get; set; }
        public Nullable<int> Book_ID { get; set; }
        public Nullable<int> Customer_ID { get; set; }
        public Nullable<System.DateTime> Borrow_Date { get; set; }
        public Nullable<System.DateTime> Return_Date { get; set; }
    
        public virtual Book Book { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
