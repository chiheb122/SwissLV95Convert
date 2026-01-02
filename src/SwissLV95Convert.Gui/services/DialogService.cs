using System.Threading.Tasks;
using Avalonia.Controls;
using SwissLV95Convert.Gui.views;

namespace SwissLV95Convert.Gui.Services;

public static class DialogService
{
    public static Task ShowInfoAsync(Window owner, string title, string message)
    {
        var dialog = new MessageDialog(title, message);
        return dialog.ShowDialog(owner);
    }
}
