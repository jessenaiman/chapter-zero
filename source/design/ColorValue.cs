using Newtonsoft.Json;

namespace OmegaSpiral.Source.Frontend.Design;

/// <summary>
/// Represents a color value with RGBA components and optional metadata.
/// </summary>
public sealed class ColorValue
{
    /// <summary>
    /// Gets or sets the red component (0-1 range).
    /// </summary>
    [JsonProperty("r")]
    public float R { get; set; }

    /// <summary>
    /// Gets or sets the green component (0-1 range).
    /// </summary>
    [JsonProperty("g")]
    public float G { get; set; }

    /// <summary>
    /// Gets or sets the blue component (0-1 range).
    /// </summary>
    [JsonProperty("b")]
    public float B { get; set; }

    /// <summary>
    /// Gets or sets the alpha component (0-1 range, defaults to 1.0).
    /// </summary>
    [JsonProperty("a")]
    public float A { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the documentation string, if any.
    /// </summary>
    [JsonProperty("_doc")]
    public string? Doc { get; set; }

    /// <summary>
    /// Gets or sets the hex representation of the color, if any.
    /// </summary>
    [JsonProperty("hex")]
    public string? Hex { get; set; }

    /// <summary>
    /// Gets or sets the alpha note, if any.
    /// </summary>
    [JsonProperty("alpha_note")]
    public string? AlphaNote { get; set; }
}
