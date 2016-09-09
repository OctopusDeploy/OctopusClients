using System;

namespace Octopus.Client
{
    /// <summary>
    /// In the same way web browsers can follow links like "/foo" by knowing the URI of the current page, this class allows
    /// application
    /// links to be resolved into fully-qualified URI's.
    /// </summary>
    public interface ILinkResolver
    {
        /// <summary>
        /// Resolves the specified link into a fully qualified URI.
        /// </summary>
        /// <param name="link">The application relative link (should begin with a <c>/</c>).</param>
        /// <returns>
        /// The fully resolved URI.
        /// </returns>
        Uri Resolve(string link);
    }
}