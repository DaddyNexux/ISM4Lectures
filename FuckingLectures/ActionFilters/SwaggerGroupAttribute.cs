namespace FuckingLectures.ActionFilter
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class SwaggerGroupAttribute : Attribute
    {
        public IReadOnlyList<string> GroupNames { get; }

        public SwaggerGroupAttribute(params string[] groupNames)
        {
            GroupNames = groupNames.Select(g => g.ToLower()).ToList();
        }
    }
}
