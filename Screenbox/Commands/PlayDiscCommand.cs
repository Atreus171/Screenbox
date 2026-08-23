using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Screenbox.Core.Enums;
using Screenbox.Core.Messages;
using Screenbox.Core.Models;
using Screenbox.Core.Services;
using Screenbox.Dialogs;

namespace Screenbox.Commands;

internal sealed partial class PlayDiscCommand : IRelayCommand
{
    private readonly AsyncRelayCommand _asyncCommand;

    public PlayDiscCommand()
    {
        _asyncCommand = new AsyncRelayCommand(PlayDiscAsync);
    }

    public bool CanExecute(object? parameter)
    {
        return _asyncCommand.CanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _asyncCommand.Execute(parameter);
    }

    public event EventHandler? CanExecuteChanged;

    public void NotifyCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task PlayDiscAsync()
    {
        IDiscService discService = Ioc.Default.GetRequiredService<IDiscService>();
        IReadOnlyList<OpticalDisc> discs = await discService.FindDiscsAsync();
        if (discs.Count == 0)
        {
            if (await discService.HasAudioCdAsync())
            {
                WeakReferenceMessenger.Default.Send(new AudioCdNotSupportedNotificationMessage());
            }
            else
            {
                WeakReferenceMessenger.Default.Send(new DiscNotFoundNotificationMessage());
            }

            return;
        }

        List<DiscSelectionItem> items = new();
        foreach (OpticalDisc disc in discs)
        {
            string discType = disc.Type == DiscType.Dvd ? "DVD" : "Blu-ray";
            string subtitle = $"{discType} • {disc.Root.Path}";
            foreach (DiscTitle title in disc.Titles)
            {
                items.Add(new DiscSelectionItem(
                    string.Format(Strings.Resources.DiscTitleName, title.Number),
                    subtitle,
                    title.Files));
            }
        }

        if (items.Count == 1)
        {
            WeakReferenceMessenger.Default.Send(new PlayMediaMessage(items[0].Files));
            return;
        }

        DiscSelectionItem? selection = await DiscSelectionDialog.GetSelectionAsync(items);
        if (selection != null)
        {
            WeakReferenceMessenger.Default.Send(new PlayMediaMessage(selection.Files));
        }
    }
}
