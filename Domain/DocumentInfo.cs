using System.Text.Json.Serialization;

namespace Singer.Domain;

public class DocumentInfo
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public bool IsSigned { get; set; }
    public DateTime Date { get; set; }
    public DateTime ExpirationDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public StatusDocument StatusDocument { get; set; }
}
