namespace InsureYou_AI.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<Article> Articles { get; set; }
    }
}
