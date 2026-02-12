namespace UniversiteDomain.Exceptions.CsvExceptions;

[Serializable]
public class InvalidCsvFormatException : Exception
{
    public InvalidCsvFormatException() : base() { }
    public InvalidCsvFormatException(string message) : base(message) { }
    public InvalidCsvFormatException(string message, Exception inner) : base(message, inner) { }
}
