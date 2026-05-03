// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CrcCommandPaletteExtension;

public partial class CrcCommandPaletteExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public CrcCommandPaletteExtensionCommandsProvider()
    {
        DisplayName = "CRC";
        Icon = Icons.Profile;
        _commands = [
            new CommandItem(new CrcCommandPaletteExtensionPage()) { Title = DisplayName },
        ];
    }

    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }

}
