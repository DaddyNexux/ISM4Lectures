namespace FuckingLectures.Models.Entities
{
    public class Permission : BaseEntity
    {
        #region Functional
        public string Subject { get; set; }
        public string Action { get; set; }
        public string FullName { get; set; }
        #endregion
    }
}
