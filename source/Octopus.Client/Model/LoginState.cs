namespace Octopus.Client.Model
{
    public class LoginState
    {
        /// <summary>
        /// Whether the client says it's using a secure connection. We need this because SSL offloading
        /// can obscure this and the server cannot tell whether the client initiated the call using a secure
        /// connection. This will be set automatically by the framework when the login request is made,
        /// otherwise you can set this value yourself to override the automatic behaviour.
        /// </summary>
        public bool UsingSecureConnection { get; set; }
    }
}