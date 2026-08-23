using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Screenbox.Core.Enums;
using Screenbox.Core.Models;
using Windows.Storage;

namespace Screenbox.Core.Services;

public sealed class DiscService : IDiscService
{
    private const string VideoTsFolderName = "VIDEO_TS";
    private const string BdmvFolderName = "BDMV";
    private const string StreamFolderName = "STREAM";

    public async Task<IReadOnlyList<OpticalDisc>> FindDiscsAsync()
    {
        List<OpticalDisc> discs = [];
        try
        {
            foreach (StorageFolder device in await KnownFolders.RemovableDevices.GetFoldersAsync())
            {
                OpticalDisc? disc = await TryGetDiscAsync(device);
                if (disc is not null)
                {
                    discs.Add(disc);
                }
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or COMException)
        {
            // Access to removable devices may be denied or a device may be unavailable.
        }

        return discs;
    }

    public async Task<bool> HasAudioCdAsync()
    {
        try
        {
            foreach (StorageFolder device in await KnownFolders.RemovableDevices.GetFoldersAsync())
            {
                try
                {
                    IReadOnlyList<StorageFile> files = await device.GetFilesAsync();
                    if (files.Any(f => f.FileType.Equals(".cda", StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException or COMException)
                {
                }
            }
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or COMException)
        {
        }

        return false;
    }

    private static async Task<OpticalDisc?> TryGetDiscAsync(StorageFolder device)
    {
        StorageFolder? videoTs = await TryGetFolderAsync(device, VideoTsFolderName);
        if (videoTs is not null)
        {
            OpticalDisc? disc = await TryCreateDvdDiscAsync(device, videoTs);
            if (disc is not null) return disc;
        }

        StorageFolder? bdmv = await TryGetFolderAsync(device, BdmvFolderName);
        if (bdmv is null) return null;

        StorageFolder? stream = await TryGetFolderAsync(bdmv, StreamFolderName);
        if (stream is null) return null;

        return await TryCreateBluRayDiscAsync(device, stream);
    }

    private static async Task<OpticalDisc?> TryCreateDvdDiscAsync(StorageFolder device, StorageFolder videoTs)
    {
        IReadOnlyList<StorageFile> files = await videoTs.GetFilesAsync();
        List<StorageFile> vobs = files
            .Where(f => f.FileType.Equals(".vob", StringComparison.OrdinalIgnoreCase))
            .ToList();

        Dictionary<int, List<(int Part, StorageFile File)>> titleSets = new();
        foreach (StorageFile file in vobs)
        {
            string[] segments = Path.GetFileNameWithoutExtension(file.Name).Split('_');
            if (segments.Length != 3 ||
                !segments[0].Equals("VTS", StringComparison.OrdinalIgnoreCase) ||
                !int.TryParse(segments[1], out int titleSet) ||
                !int.TryParse(segments[2], out int part))
            {
                continue;
            }

            if (!titleSets.TryGetValue(titleSet, out List<(int Part, StorageFile File)> parts))
            {
                parts = [];
                titleSets[titleSet] = parts;
            }

            parts.Add((part, file));
        }

        List<DiscTitle> titles = [];
        foreach (KeyValuePair<int, List<(int Part, StorageFile File)>> kv in titleSets.OrderBy(kv => kv.Key))
        {
            // Part 0 holds menu content; movie content starts at part 1.
            List<StorageFile> parts = kv.Value
                .Where(p => p.Part > 0)
                .OrderBy(p => p.Part)
                .Select(p => p.File)
                .ToList();

            if (parts.Count > 0)
            {
                titles.Add(new DiscTitle(kv.Key, parts));
            }
        }

        if (titles.Count == 0 && vobs.Count > 0)
        {
            titles.Add(new DiscTitle(1, vobs.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToList()));
        }

        return titles.Count > 0 ? new OpticalDisc(DiscType.Dvd, device, titles) : null;
    }

    private static async Task<OpticalDisc?> TryCreateBluRayDiscAsync(StorageFolder device, StorageFolder stream)
    {
        IReadOnlyList<StorageFile> files = await stream.GetFilesAsync();
        List<StorageFile> m2tsFiles = files
            .Where(f => f.FileType.Equals(".m2ts", StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (m2tsFiles.Count == 0) return null;

        List<DiscTitle> titles = m2tsFiles
            .Select((file, index) => new DiscTitle(index + 1, [file]))
            .ToList();

        return new OpticalDisc(DiscType.BluRay, device, titles);
    }

    private static async Task<StorageFolder?> TryGetFolderAsync(StorageFolder parent, string name)
    {
        try
        {
            return await parent.GetFolderAsync(name);
        }
        catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException or UnauthorizedAccessException or COMException)
        {
            return null;
        }
    }
}
