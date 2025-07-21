namespace Octopus.Client.Model
{
    public class IconResource
    {
        /// <summary>
        /// Represents an Icon
        /// </summary>
        /// <param name="id">Id of the font awesome icon</param>
        /// <param name="color">Color of the icon background, as a hex string</param>
        public IconResource(string id, string color)
        {
            Id = id;
            Color = color;
        }

        /// <summary>
        /// Font Awesome Icon Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Icon background colour, as a Hex string
        /// </summary>
        public string Color { get; set; }
    }
}