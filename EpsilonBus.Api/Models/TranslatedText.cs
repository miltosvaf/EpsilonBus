namespace EpsilonBus.Api.Models
{
    public class TranslatedText
    {
        public int ID { get; set; }
        public string TableName { get; set; }
        public string PrimaryKeyFieldName { get; set; }
        public int PrimaryKeyValue { get; set; }
        public int LanguageID { get; set; }
        public string TranslatedTextValue { get; set; }
        public Language Language { get; set; }
    }
}