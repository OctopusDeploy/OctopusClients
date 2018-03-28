namespace Octopus.Client.Model
{
    public class LoginState
    {
        /// <summary>
        /// Whether the client says it's using a secure connection. We need this because SSL offloading
        /// can obscure this and the server cannot tell whether the client initiated the call using a secure
        /// connection.
        /// </summary>
        public bool UsingSecureConnection { get; set; }
    }
}