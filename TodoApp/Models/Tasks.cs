using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Models
{
    public class Tasks
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskId { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public Users User { get; set; }
        [Required]
        public string TaskName { get; set; }
        public string Description { get; set; }
        public Options Status { get; set; }
        public DateTime TaskCreated { get; set; } = DateTime.Now;
    }
    public enum Options
    {
        Completed,
        NotCompleted
    }
}
