namespace UIComponents.Services;

public class ThemeService
{
    public string CurrentTheme { get; private set; } = "fantasy";
    public event Action? OnThemeChanged;

    public void SetTheme(string theme)
    {
        if (CurrentTheme == theme) return;
        CurrentTheme = theme;
        OnThemeChanged?.Invoke();
    }
}
