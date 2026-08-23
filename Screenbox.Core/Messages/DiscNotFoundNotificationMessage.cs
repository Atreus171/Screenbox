namespace Screenbox.Core.Messages;

/// <summary>
/// Sent by a view model when disc playback was requested but no playable optical
/// disc was detected. <see cref="ViewModels.NotificationViewModel"/> handles this
/// message and displays a localized error notification.
/// </summary>
public class DiscNotFoundNotificationMessage
{
}
