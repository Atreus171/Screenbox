namespace Screenbox.Core.Enums;

public enum NotificationKind
{
    None,

    MusicLibraryAccessDenied,
    PicturesLibraryAccessDenied,
    VideosLibraryAccessDenied,

    InitializationFailed,
    FileOpenFailed,
    FolderAddFailed,
    DiscNotFound,
    AudioCdNotSupported,
    MediaLoadFailed,
    SubtitleLoadFailed,
    FrameSaveFailed,

    FrameSaved,
    SubtitleAdded,
    PlaylistCreated,
    PlaylistDeleted,
    PlaylistRenamed,
    PlaylistItemsAdded,

    ResumePosition,
}
