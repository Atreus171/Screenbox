using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Screenbox.Helpers;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Screenbox.Dialogs;

public sealed partial class DiscSelectionDialog : ContentDialog
{
    public DiscSelectionDialog(IReadOnlyList<DiscSelectionItem> items)
    {
        this.DefaultStyleKey = typeof(ContentDialog);
        this.InitializeComponent();
        FlowDirection = GlobalizationHelper.GetFlowDirection();
        RequestedTheme = ((FrameworkElement)Window.Current.Content).RequestedTheme;
        ItemsList.ItemsSource = items;
        if (items.Count > 0)
        {
            ItemsList.SelectedIndex = 0;
        }
    }

    public static async Task<DiscSelectionItem?> GetSelectionAsync(IReadOnlyList<DiscSelectionItem> items)
    {
        DiscSelectionDialog dialog = new(items);
        ContentDialogResult result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary
            ? dialog.ItemsList.SelectedItem as DiscSelectionItem
            : null;
    }

    private void ItemsList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        IsPrimaryButtonEnabled = ItemsList.SelectedItem != null;
    }
}

public sealed class DiscSelectionItem
{
    public string Name { get; }

    public string Subtitle { get; }

    public IReadOnlyList<StorageFile> Files { get; }

    public DiscSelectionItem(string name, string subtitle, IReadOnlyList<StorageFile> files)
    {
        Name = name;
        Subtitle = subtitle;
        Files = files;
    }
}
