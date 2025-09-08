using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

namespace GemmaDesktop.Views;

public partial class MainWindow : Window
{

    private readonly ContextManager _contextManager;
    private readonly GemmaClient _gemmaClient;
    private CancellationTokenSource _statusToken;

    public MainWindow()
    {
        InitializeComponent();
        _contextManager = new ContextManager();
        bool isRaspberry = RuntimeInformation.OSArchitecture == Architecture.Arm64;
        _gemmaClient = new GemmaClient(useRover: isRaspberry);

        this.Loaded += async (_, _) => await InitializeAsync();
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

        ShowThinkingStatus();
        var response = await _gemmaClient.SendAsync(request);
        HideThinkingStatus();

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

    private async Task InitializeAsync()
    {
        if (!await _gemmaClient.IsOllamaRunningAsync())
        {
            await ShowErrorAndExit("❌ Ollama is not running on localhost:11434.\nPlease start it with 'ollama server'.");
            return;
        }
        else
        {
            AddMessage("System", "✅ Ollama connection established.");
        }

        if (!await _gemmaClient.WarmUpAsync())
        {
            await ShowErrorAndExit("⚠️ Gemma did not respond to warm-up. Aborting.");
            return;
        }

        AddMessage("System", "Gemma is warmed up and ready.");
    }

    private async Task ShowErrorAndExit(string message)
    {
        AddMessage("System", message);
        await Task.Delay(3000); // Optional pause before exit
        Close();
    }

    private async void ShowThinkingStatus()
    {
        _statusToken?.Cancel();
        _statusToken = new CancellationTokenSource();

        var frames = new[] { "|", "/", "-", "\\" };
        int i = 0;

        while (!_statusToken.Token.IsCancellationRequested)
        {
            StatusLabel.Text = $"Gemma is answering... {frames[i++ % frames.Length]}";
            await Task.Delay(200);
        }
    }

    private void HideThinkingStatus()
    {
        _statusToken?.Cancel();
        StatusLabel.Text = "";
    }

}