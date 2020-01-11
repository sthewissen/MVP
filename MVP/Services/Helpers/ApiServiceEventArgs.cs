using System;

namespace MVP.Services.Helpers
{
    /// <summary>
    /// Event Args for API Service Events
    /// </summary>
    public class ApiServiceEventArgs : EventArgs
    {
        /// <summary>
        /// Indicates the server returned a 400. The query did not contain the expected information, such as correctly formatted data for that request (i.e. PostContribution).
        /// </summary>
        public bool IsBadRequest { get; set; }

        /// <summary>
        /// Indicates that a login is needed to get a new access and refresh token for the user.
        /// </summary>
        public bool IsTokenRefreshNeeded { get; set; }

        /// <summary>
        /// Indicates a 500 error was returned from the server.
        /// </summary>
        public bool IsServerError { get; set; }

        /// <summary>
        /// Message to be shown to the user if the error requires user clarification (i.e. BadRequest is a good example when a user should be explained why it failed).
        /// </summary>
        public string Message { get; set; }
    }
}
