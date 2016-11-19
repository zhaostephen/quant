namespace Screen.Patterns
{
    public class PatternResult
    {
        public string Code { get; set; }
        public PatterEnum Pattern { get; set; }
        public double CriticalPrice { get; set; }
    }

    public enum PatterEnum
    {
        建仓
    }
}