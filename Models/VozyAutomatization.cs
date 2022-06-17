namespace vozy_automatizacion.Models
{
    public class VozyAutomatization
    {
        public VozyAutomatization(string jsonData, string campaign, string category)
        {
            this.jsonData = jsonData;
            this.campaign = campaign;
            this.category = category;
        }

        public Guid id { get; set; }
        public string jsonData { get; set; }
        public DateTime creationDate { get; set; }
        public string campaign { get; set; }
        public string category { get; set; }
        public bool upload { get; set; }
    }
}
