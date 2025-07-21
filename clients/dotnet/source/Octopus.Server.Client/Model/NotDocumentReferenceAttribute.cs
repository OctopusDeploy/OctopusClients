using System;

namespace Octopus.Client.Model
{
    /// <summary>
    /// By default, writeable properties with names ending in 'Id' are considered to be document references and are verified.
    /// This attribute allows this behaviour to be disabled. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class NotDocumentReferenceAttribute : Attribute
    {
         
    }
}