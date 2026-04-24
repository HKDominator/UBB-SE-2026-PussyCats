namespace PussyCatsApp.Models
{
    /// <summary>
    /// Severity Status codes for the Information Bar.
    /// </summary>
    public enum InformationBarSeverityStatus
    {
        /// <summary>
        /// Only an informational message should be displayed, without any specific severity.
        /// </summary>
        Informational = 0,

        /// <summary>
        /// Indicates that the operation completed successfully.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Indicates that a warning should be displayed
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Indicates that an error occurred during the operation.
        /// </summary>
        Error = 3
    }
}
