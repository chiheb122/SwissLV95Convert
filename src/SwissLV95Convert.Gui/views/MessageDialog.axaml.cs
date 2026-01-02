using System;
using Avalonia.Controls;

namespace SwissLV95Convert.Gui.views;

public partial class MessageDialog : Window
{
    public MessageDialog()
    {
        InitializeComponent();
    }

    public MessageDialog(string title, string message) : this()
    {
        Title = title;
        TitleText.Text = title;
        MessageText.Text = message;
    }


     private void OkButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Console.WriteLine("Ok button clicked");
        Close(true);
    }
}
