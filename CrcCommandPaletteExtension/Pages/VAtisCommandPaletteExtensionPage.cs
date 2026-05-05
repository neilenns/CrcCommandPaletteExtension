// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.IO;
using System.Linq;
using static Microsoft.CommandPalette.Extensions.Toolkit.ShellHelpers;

namespace CrcCommandPaletteExtension;

internal sealed partial class VAtisCommandPaletteExtensionpage : DynamicListPage
{
    public VAtisCommandPaletteExtensionpage()
    {
        Icon = Icons.CrcProfile;
        Title = "vATIS profile launcher";
        Name = "Launch profile";
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
        RaiseItemsChanged();
    }

    public override IListItem[] GetItems()
    {
        return [
            .. vAtisProfileManager.GetMatchingProfiles(SearchText).Select(profile =>
            {
                var id = profile.Id;

                AnonymousCommand launchCommand = new (() =>
                {
                    var appPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "current", "vATIS.exe");

                    var arguments = $"--profile {id}";

                    ShellHelpers.OpenInShell(
                        path: appPath,
                        arguments: arguments,
                        runAs: ShellRunAsType.None,
                        runWithHiddenWindow: false);
                }) { Result = CommandResult.Dismiss() };

                return new ListItem(launchCommand)
                {
                    Icon = Icons.vAtisProfile,
                    Title = profile.Name
                };
            })
        ];
    }
}
