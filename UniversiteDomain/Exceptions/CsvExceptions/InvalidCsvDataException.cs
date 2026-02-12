namespace UniversiteDomain.Exceptions.CsvExceptions;

[Serializable]
public class InvalidCsvDataException : Exception
{

    public InvalidCsvDataException() : base() { }

    public InvalidCsvDataException(string message) : base(message) { }

    public InvalidCsvDataException(string message, Exception inner) : base(message, inner) { }
}
