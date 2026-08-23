using System.Collections.Generic;
using Windows.Storage;

namespace Screenbox.Core.Models;

/// <summary>
/// Represents a single playable title on an optical disc.
/// </summary>
public sealed class DiscTitle
{
    /// <summary>Gets the display number of the title.</summary>
    public int Number { get; }

    /// <summary>Gets the media files that make up the title, ordered by playback position.</summary>
    public IReadOnlyList<StorageFile> Files { get; }

    public DiscTitle(int number, IReadOnlyList<StorageFile> files)
    {
        Number = number;
        Files = files;
    }
}
