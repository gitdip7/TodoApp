namespace TodoApp.Data
{
    public class AddTask
    {
        public string name { get; set; }
        public string description { get; set; }
        public Options status { get; set; }
    }
    public enum Options
    {
        Completed,
        NotCompleted
    }
}
