using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Remote.Protocol.Designer;
using SwissLV95Convert.Core.Services;

namespace SwissLV95Convert.Gui;

public partial class MainWindow : Window
{
     private string? _pathA; // File path

    public MainWindow()
    {
        InitializeComponent();
        // Ensure the Start button exists and the handler is hooked (fallback if XAML wiring fails)
        if (StartButton != null)
            StartButton.Click += Start_Click;
        UpdateStartButtonState();
    }


    // Drag & Drop Handlers
    private void DropZone_DragOver(object? sender, DragEventArgs e)
        {
            // Accept only files
            e.DragEffects = e.DataTransfer.Contains(DataFormat.File) ? DragDropEffects.Copy : DragDropEffects.None;
            SetDragOverVisual(sender, e.DragEffects == DragDropEffects.Copy);
            e.Handled = true;
        }

        private void DropA_Drop(object? sender, DragEventArgs e)
            => HandleDrop(e, isA: true);

        private void HandleDrop(DragEventArgs e, bool isA)
        {
            SetDragOverVisual(e.Source, false);
            if (!e.DataTransfer.Contains(DataFormat.File))
                return;

            var files = e.DataTransfer.TryGetFiles()?.ToList();
            if (files is null || files.Count == 0)
                return;

            var localPath = files[0].TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(localPath))
                return;

            SetPath(isA, localPath);
        }
        // Browse Button Handlers
        private async void BrowseA_Click(object? sender, RoutedEventArgs e)
            => await BrowseAsync(isA: true);

        private async Task BrowseAsync(bool isA)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select a .csv file",
                AllowMultiple = false
            });

            var file = files.FirstOrDefault();
            var path = file?.TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(path))
                SetPath(isA, path);
        }

        // Set Path and Update UI
        private void SetPath(bool isA, string path)
        {
            if (isA)
            {
                _pathA = path;
                PathABox?.SetCurrentValue(TextBox.TextProperty, path); // null-safe, use generated member
            }
            // Update Start button state
            UpdateStartButtonState();
        }

        // Clear and Start Button Handlers
        private void Clear_Click(object? sender, RoutedEventArgs e)
        {
            _pathA = null;
            UpdateStartButtonState();
            if (PathABox is not null) PathABox.Text = "";
            if (SummaryText is not null) SummaryText.Text = "";
            if (ResultHint is not null) ResultHint.Text = "No file selected.";
            if (LoadingCard is not null) LoadingCard.IsVisible = false;
        }

        private async void Start_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_pathA) )
            {
                if (ResultHint is not null) ResultHint.Text = "Please select a file to start conversion.";
                return;
            }

            if (!File.Exists(_pathA))
            {
                if (ResultHint is not null) ResultHint.Text = "One of the selected files does not exist.";
                return;
            }


            try
            {
                if (LoadingCard is not null) LoadingCard.IsVisible = true;
                if (ResultHint is not null) ResultHint.Text = "Please wait...";
                // determine options
                var onlyLongLat = OnlyLatLonCheckBox?.IsChecked == true ? "y" : "n";
                // petit délai pour laisser l'UI se mettre à jour avant de lancer le travail
                await Task.Delay(5000);

                // exécuter le travail lourd en arrière-plan
                await Task.Run(() =>
                {
                    var data = CsvService.ReadCsv(_pathA, separator: ';', skipHeader: false).ToList();
                    Console.WriteLine($"Debug: CSV file read successfully.{data.Count} rows found.");
                    var outputDir = Path.GetDirectoryName(_pathA) ?? Environment.CurrentDirectory;
                    Console.WriteLine($"Debug: Output directory determined as {outputDir}.");
                    var outputPath = Path.GetFullPath(
                        Path.Combine(
                            outputDir,
                            $"output_converted_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
                        ));
                    CsvService.ConvertAndAddToCsv(outputPath, data, 8, 9, onlyLongLat);
                    Console.WriteLine($"Debug: Output path set to {outputPath}.");
                });

                if (ResultHint is not null) ResultHint.Text = "Done.";
            }
            finally
            {
                if (LoadingCard is not null) LoadingCard.IsVisible = false;
            }
        }



        // Update Start Button State        
        private void UpdateStartButtonState()
        {
            if (StartButton == null) return;
            StartButton.IsEnabled = !string.IsNullOrWhiteSpace(_pathA);
        }


        // Visual Feedback for Drag & Drop
        private static void SetDragOverVisual(object? sender, bool isOver)
        {
            if (sender is not Avalonia.Controls.Border border)
                return;

            if (isOver)
                border.Classes.Add("dragover");
            else
                border.Classes.Remove("dragover");
        }

        // CheckBoxes Update status
        private void ExclusiveChoiceChanged(object? sender, RoutedEventArgs e)
        {
            if (sender == OnlyLatLonCheckBox && OnlyLatLonCheckBox.IsChecked == true)
            {
                KeepExistingColumnsCheckBox.IsChecked = false;
            }
            else if (sender == KeepExistingColumnsCheckBox && KeepExistingColumnsCheckBox.IsChecked == true)
            {
                OnlyLatLonCheckBox.IsChecked = false;
            }
        }








}