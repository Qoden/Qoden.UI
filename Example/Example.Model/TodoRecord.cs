using System;
using System.ComponentModel.DataAnnotations;

namespace Example.Model
{
    public class TodoRecord
    {
        [Key]
        public Guid TodoId { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public bool Completed { get; set; }
    }
}
