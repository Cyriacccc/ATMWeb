namespace ATMWeb.Exceptions;

public class DomainValidationException(string message) : Exception(message);
