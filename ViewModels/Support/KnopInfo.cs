namespace TegoareWeb.Data
{
    public class KnopInfo
    {
        // tekst op knop
        public string Label { get; set; } = "Terug naar lijst";

        // naar welke controller verwijst de knop
        public string Controller { get; set; }

        // naar welke actie verwijst de knop
        public string Action { get; set; } = "Index";

        // wat is de css-opmaak van de knop
        public string CSS { get; set; } = "btn-dark";

        // naar FontAwesome icoon moet er links van de tekst getoond worden
        public string FA { get; set; } = "fa-angle-double-left";
    }
}
