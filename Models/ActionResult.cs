namespace OneSCodeChanger.Models
{
    public enum ActionResultType
    {
        Error,
        Success
    }
    public class ActionResult
    {
        public string Answer { get; private set; }
        public ActionResultType Type { get; private set; }
        public ActionResult(string text)
        {
            Answer = text;
            Type = text.Contains("Error") ? ActionResultType.Error : ActionResultType.Success;
        }
        public static implicit operator ActionResult(string answer) => new ActionResult(answer);
    }
}
