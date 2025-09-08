using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace GemmaDesktop.Views;

public partial class MainWindow : Window
{

    private readonly ContextManager _contextManager;
    private readonly GemmaClient _gemmaClient;

    public MainWindow()
    {
        InitializeComponent();
        _contextManager = new ContextManager();
        _gemmaClient = new GemmaClient();
    }

    private async void OnSendClick(object sender, RoutedEventArgs e)
    {
        var userMessage = InputBox.Text?.Trim();
        if (string.IsNullOrEmpty(userMessage)) return;

        InputBox.Text = "";
        AddMessage("You", userMessage);

        if (userMessage == "/exit")
        {
            _contextManager.Save();
            Close();
            return;
        }


        _contextManager.AddUserMessage(userMessage);

        var request = new GemmaRequest
        {
            model = "gemma3:27b",
            messages = _contextManager.GetContext(),
            temperature = 0.7
        };

        var response = await _gemmaClient.SendAsync(request);

        _contextManager.AddAssistantMessage(response);
        AddMessage("Gemma", response);
    }

    private void OnExitClick(object sender, RoutedEventArgs e)
    {
        _contextManager.Save(); // Persist soulstack
        Close(); // Gracefully close the window
    }


    private void AddMessage(string sender, string text)
    {
        var block = new Border
        {
            Background = sender == "You" ? Brushes.LightBlue : Brushes.LightGray,
            CornerRadius = new CornerRadius(5),
            Padding = new Thickness(10),
            Margin = new Thickness(5),
            Child = new TextBox
            {
                Text = text,
                Foreground = Brushes.Black,
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 500
            }
,
            HorizontalAlignment = sender == "You"
                ? Avalonia.Layout.HorizontalAlignment.Right
                : Avalonia.Layout.HorizontalAlignment.Left
        };

        ChatPanel.Children.Add(block);
    }
}