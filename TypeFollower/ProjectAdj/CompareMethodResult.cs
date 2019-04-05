namespace ProjectAdj
{
    public class CompareMethodResult : CompareTypeResult
    {
        public string OriginalMethodName { get; set; }
        public string NewMethodName { get; set; }

        public override bool IsChanged()
        {
            return (!string.IsNullOrEmpty(NewMethodName) && OriginalMethodName != NewMethodName) || base.IsChanged();
        }
    }
}
