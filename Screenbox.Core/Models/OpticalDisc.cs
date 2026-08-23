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

    /// <summary>Gets the playable titles on the disc, ordered by title number.</summary>
    public IReadOnlyList<DiscTitle> Titles { get; }

    public OpticalDisc(DiscType type, StorageFolder root, IReadOnlyList<DiscTitle> titles)
    {
        Type = type;
        Root = root;
        Titles = titles;
    }
}
