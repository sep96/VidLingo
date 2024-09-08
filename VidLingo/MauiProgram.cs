using CommunityToolkit.Maui;

namespace VidLingo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement(); // Add this line to initialize MediaElement

            return builder.Build();
        }
    }
}
