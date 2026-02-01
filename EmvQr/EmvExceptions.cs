namespace EmvQr
{
    /// <summary>
    /// Base exception class for all EMV QR code related exceptions
    /// </summary>
    public class EmvQrException : Exception
    {
        public EmvQrException() : base() { }
        public EmvQrException(string message) : base(message) { }
        public EmvQrException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when validation of an EMV QR code fails
    /// </summary>
    public class EmvValidationException : EmvQrException
    {
        public EmvValidationResult ValidationResult { get; }

        public EmvValidationException(EmvValidationResult validationResult)
            : base($"EMV QR code validation failed with {validationResult.Errors.Count} error(s)")
        {
            ValidationResult = validationResult;
        }

        public EmvValidationException(string message, EmvValidationResult validationResult)
            : base(message)
        {
            ValidationResult = validationResult;
        }
    }

    /// <summary>
    /// Exception thrown when parsing of an EMV QR code fails
    /// </summary>
    public class EmvParserException : EmvQrException
    {
        public EmvParserException() : base() { }
        public EmvParserException(string message) : base(message) { }
        public EmvParserException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when there's an error in building an EMV QR code
    /// </summary>
    public class EmvBuilderException : EmvQrException
    {
        public EmvBuilderException() : base() { }
        public EmvBuilderException(string message) : base(message) { }
        public EmvBuilderException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Exception thrown when there's an invalid tag
    /// </summary>
    public class InvalidTagException : EmvQrException
    {
        public string Tag { get; }

        public InvalidTagException(string tag)
            : base($"Invalid tag: {tag}")
        {
            Tag = tag;
        }

        public InvalidTagException(string tag, string message)
            : base($"Invalid tag: {tag} - {message}")
        {
            Tag = tag;
        }
    }

    /// <summary>
    /// Exception thrown when there's an invalid tag value
    /// </summary>
    public class InvalidTagValueException : EmvQrException
    {
        public string Tag { get; }
        public string Value { get; }

        public InvalidTagValueException(string tag, string value)
            : base($"Invalid value '{value}' for tag: {tag}")
        {
            Tag = tag;
            Value = value;
        }

        public InvalidTagValueException(string tag, string value, string message)
            : base($"Invalid value '{value}' for tag: {tag} - {message}")
        {
            Tag = tag;
            Value = value;
        }
    }
}
