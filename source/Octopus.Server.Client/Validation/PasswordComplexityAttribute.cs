using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Octopus.Client.Validation
{
    /// <summary>
    /// A custom validation rule that ensures passwords meet complexity requirements.
    /// </summary>
    public class PasswordComplexityAttribute : ValidationAttribute
    {
        public static string DefaultMessage = "The password was too weak. Please try including a mix of numbers, uppercase and lowercase letters, and special characters.";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordComplexityAttribute" /> class.
        /// </summary>
        public PasswordComplexityAttribute() : base(DefaultMessage)
        {
        }

        public override bool IsValid(object value)
        {
            var text = (value ?? string.Empty).ToString();
            if (string.IsNullOrWhiteSpace(text))
                return true;

            return IsPasswordStrongEnough(text);
        }

        public static bool IsPasswordStrongEnough(string password)
        {
            var score = CalculateStrength(password);
            return score >= 4;
        }

        static int CalculateStrength(string password)
        {
            // This algorithm is also implemented on the client; see Octopus.js::Octopus.Utilities.passwordStrength()
            var score = 0;

            if (password.Length < 7)
            {
                return 0;
            }
            if (password.Length >= 8)
            {
                score = score + 1;
            }
            if (password.Length >= 12)
            {
                score = score + 1;
            }
            if (password.Length >= 16)
            {
                score = score + 1;
            }
            if (Regex.IsMatch(password, @"\d"))
            {
                score = score + 1;
            }
            if (Regex.IsMatch(password, @"\s"))
            {
                score = score + 1;
            }
            if (Regex.IsMatch(password, @"[a-z]"))
            {
                score = score + 1;
            }
            if (Regex.IsMatch(password, @"[A-Z]"))
            {
                score = score + 1;
            }
            if (Regex.IsMatch(password, @"[!@#\$%\^&\*\?_~\-\(\);\.\+:]+"))
            {
                score++;
            }

            return score;
        }
    }
}