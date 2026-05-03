using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CrcCommandPaletteExtension;

internal sealed class Icons
{
    internal static IconInfo Profile { get; } = IconHelpers.FromRelativePaths("Assets\\profile.light.png", "Assets\\profile.dark.png");
}