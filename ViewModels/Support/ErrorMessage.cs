namespace TegoareWeb.Data
{
    // ErrorMessage is eigenlijk een slechte naam,
    // hoeft niet gebruikt te worden om foutboodschappen te tonen
    public class ErrorMessage
    {
        // bericht die moet getoond worden
        public string Message { get; set; }

        // true = succes
        // false = fout
        public bool Value { get; set; }
    }
}
