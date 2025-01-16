using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace sortifyApp;

public partial class MainWindow : Window
{
    private Queue<string> images = new Queue<string>();
    private string currentImage;
    private string[] imageTypes = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp", ".heic" };
    private TextBlock folderText;
    private TextBlock countText;
    private string savedPicFolder = Path.Combine(Directory.GetCurrentDirectory())  + "\\Saved Photos";

    public MainWindow()
    {
        InitializeComponent();
        
        if (!Directory.Exists(savedPicFolder))
            Directory.CreateDirectory(savedPicFolder);
        
        folderText = this.FindControl<TextBlock>("directoryText");
        countText = this.FindControl<TextBlock>("imageCountText");
    }

    private async void OnBrowseClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog();
        string folder = await dialog.ShowAsync(this);

        if (folder != null)
        {
            string[] allFiles = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            images.Clear();

            foreach (string file in allFiles)
            {
                string extension = Path.GetExtension(file).ToLower();
                foreach (string type in imageTypes)
                {
                    if (extension == type)
                    {
                        images.Enqueue(file);
                        break;
                    }
                }
            }

            folderText.Text = Path.GetFileName(folder);
            countText.Text = "Total: " + images.Count;
            LoadNextPicture();
        }
    }

    private void LoadNextPicture()
    {
        if (images.Count > 0)
        {
            currentImage = images.Dequeue();

            try
            {
                imageDisplay.Source = new Bitmap(currentImage);
                keepButton.IsEnabled = true;
                deleteButton.IsEnabled = true;
                countText.Text = "Images remaining: " + images.Count;
            }
            catch
            {
                LoadNextPicture();
            }
        }
        else
        {
            imageDisplay.Source = null;
            keepButton.IsEnabled = false;
            deleteButton.IsEnabled = false;
            countText.Text = "Done!";
        }
    }

    private void OnKeepClick(object sender, RoutedEventArgs e)
    {
        if (currentImage != null)
        {
            try
            {
                imageDisplay.Source = null;
                File.Move(currentImage, Path.Combine(savedPicFolder, Path.GetFileName(currentImage)));
            }
            catch
            {
            }
        }
    }

    private void OnDeleteClick(object sender, RoutedEventArgs e)
    {
        if (currentImage != null)
        {
            try
            {
                imageDisplay.Source = null;
                File.Delete(currentImage);
            }
            catch
            {
            }

            LoadNextPicture();
        }
    }
}