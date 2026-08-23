namespace Screenbox.Core.Messages;

/// <summary>
/// Sent by a view model when an audio CD was detected but its playback is not
/// supported. <see cref="ViewModels.NotificationViewModel"/> handles this message
/// and displays a localized error notification.
/// </summary>
public class AudioCdNotSupportedNotificationMessage
{
}
