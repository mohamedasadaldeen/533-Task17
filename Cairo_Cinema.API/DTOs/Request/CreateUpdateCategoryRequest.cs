namespace Cairo_Cinema.API.DTOs.Request
{
    public class CreateUpdateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
