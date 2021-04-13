namespace TegoareWeb.Data
{
    public class KnopInfo
    {
        public string Label { get; set; } = "Terug naar lijst";
        public string Controller { get; set; }
        public string Action { get; set; } = "Index";
        public string CSS { get; set; } = "btn-dark";
        public string FA { get; set; } = "fa-angle-double-left";
    }
}
