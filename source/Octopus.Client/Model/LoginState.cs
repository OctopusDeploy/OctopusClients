namespace Octopus.Client.Model
{
    public class LoginState
    {
        /// <summary>
        /// The Url, relative to the portal site, to redirect to post successful login.
        /// </summary>
        public string RedirectAfterLoginTo { get; set; }

        /// <summary>
        /// Whether the client says it's using a secure connection. We need this because SSL offloading
        /// can obscure this and the server cannot tell whether the client initiated the call using a secure
        /// connection.
        /// </summary>
        public bool UsingSecureConnection { get; set; }
    }
}