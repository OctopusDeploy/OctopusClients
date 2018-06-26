using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Octopus.Client.Extensibility;
using Octopus.Client.Extensibility.Attributes;

namespace Octopus.Client.Model
{
    /// <summary>
    /// Artifacts are files like documents and test results that may be stored
    /// alongside a release.
    /// </summary>
    public class ArtifactResource : Resource, IHaveSpaceResource
    {
        /// <summary>
        /// Gets or sets the filename of the Artifact to create. An example might be
        /// "Performance Test Results.csv".
        /// </summary>
        /// <remarks>The filename should not include path information.</remarks>
        [WriteableOnCreate]
        [Required(ErrorMessage = "Please provide a filename (without path information) for the artifact.")]
        [JsonProperty(Order = 2)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets a short summary of the source of this attachment. This will typically be the name of a step/machine,
        /// or
        /// "Uploaded by [username]" if the attachment was uploaded by a person.
        /// </summary>
        [JsonProperty(Order = 3)]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the documents with which this artifact is associated.
        /// </summary>
        [WriteableOnCreate]
        [JsonProperty(Order = 4)]
        public ReferenceCollection RelatedDocumentIds { get; set; }

        /// <summary>
        /// Gets or sets the time at which the artifact was created.
        /// </summary>
        [JsonProperty(Order = 5)]
        public DateTimeOffset Created { get; set; }

        /// <summary>
        /// Gets the correlationId of the log block in which the artifact was captured
        /// </summary>
        [JsonProperty(Order = 6)]
        public string LogCorrelationId { get; set; }

        public string SpaceId { get; set; }
    }
}