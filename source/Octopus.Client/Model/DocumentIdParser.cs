using System;

namespace Octopus.Client.Model
{
    public static class DocumentIdParser
    {
        public static void Split(string documentId, out string groupPrefix, out string identitySuffix)
        {
            if (documentId == null) throw new ArgumentNullException("documentId");
            var dash = documentId.LastIndexOf('-');
            if (dash < 0)
                throw new DocumentIdFormatException($"The document ID {documentId} doesn't have a dash!");
            groupPrefix = documentId.Substring(0, dash);
            identitySuffix = documentId.Substring(dash + 1);
        }
    }
}