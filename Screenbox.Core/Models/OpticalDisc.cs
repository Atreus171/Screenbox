using System.Collections.Generic;
using Screenbox.Core.Enums;
using Windows.Storage;

namespace Screenbox.Core.Models;

/// <summary>
/// Represents an optical disc with playable content detected on a removable storage device.
/// </summary>
public sealed class OpticalDisc
{
    /// <summary>Gets the type of the disc.</summary>
    public DiscType Type { get; }

    /// <summary>Gets the root folder of the disc.</summary>
    public StorageFolder Root { get; }

    /// <summary>Gets the playable track files on the disc, ordered by playback position.</summary>
    public IReadOnlyList<StorageFile> Tracks { get; }

    public OpticalDisc(DiscType type, StorageFolder root, IReadOnlyList<StorageFile> tracks)
    {
        Type = type;
        Root = root;
        Tracks = tracks;
    }
}
