using System;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model
{
    /// <summary>
    /// A command resource used for logging in.
    /// </summary>
    public class LoginCommand
    {
        /// <summary>
        /// The username to log in with.
        /// </summary>
        [Required(ErrorMessage = "Please provide a username.")]
        public string Username { get; set; }

        /// <summary>
        /// The password to log in with.
        /// </summary>
        [Required(ErrorMessage = "Please provide a password.")]
        public string Password { get; set; }

        /// <summary>
        /// Whether the cookie should be persistent.
        /// </summary>
        public bool RememberMe { get; set; }

        /// <summary>
        /// Provides hints to the server about the context of the client. This will be set automatically by the framework, or you can set it yourself to override the automatic behaviour.
        /// </summary>
        public LoginState State { get; set; }
    }
}