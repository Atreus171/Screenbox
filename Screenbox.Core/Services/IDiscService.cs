using System.Collections.Generic;
using System.Threading.Tasks;
using Screenbox.Core.Models;

namespace Screenbox.Core.Services;

public interface IDiscService
{
    /// <summary>
    /// Scans removable storage devices for optical discs with playable content.
    /// </summary>
    /// <returns>All detected discs. Empty if none found or access is denied.</returns>
    Task<IReadOnlyList<OpticalDisc>> FindDiscsAsync();
}
