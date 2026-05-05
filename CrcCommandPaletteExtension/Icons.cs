using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CrcCommandPaletteExtension;

internal sealed class Icons
{
    internal static IconInfo CrcProfile { get; } = IconHelpers.FromRelativePaths("Assets\\crcprofile.light.png", "Assets\\crcprofile.dark.png");
    internal static IconInfo vAtisProfile { get; } = IconHelpers.FromRelativePaths("Assets\\vatisprofile.light.png", "Assets\\vatisprofile.dark.png");
}