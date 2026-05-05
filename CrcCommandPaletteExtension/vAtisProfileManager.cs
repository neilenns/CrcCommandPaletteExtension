// <copyright file="ProfileViewModel.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace CrcCommandPaletteExtension
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;

    /// <summary>
    /// Maintains a list of vAtis profiles.
    /// </summary>
    public static class vAtisProfileManager
    {
        private static readonly List<vAtisProfile> Profiles = [];

        private static readonly string AppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "current", "vATIS.exe");

        /// <summary>
        /// Gets the list of profiles.
        /// </summary>
        public static IReadOnlyList<vAtisProfile> vAtisProfiles => Profiles;

        public static IEnumerable<vAtisProfile> GetAllProfiles()
        {
            return GetMatchingProfiles("");
        }

        /// <summary>
        /// Returns a list of profiles that match the specified query.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <returns>The list of matching profiles. If query is null or empty all profiles are returned. If no profiles match an empty list is returned.</returns>
        public static IEnumerable<vAtisProfile> GetMatchingProfiles(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || vAtisProfiles.Count == 0)
            {
                // Issue #19: This ensures the profiles get reloaded every time a new query
                // is started, so new profiles get picked up.
                vAtisProfileManager.LoadProfiles();
                return vAtisProfiles;
            }

            return vAtisProfiles.Where(profile =>
                profile.Name != null && profile.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Loads the profiles from disk.
        /// </summary>
        private static void LoadProfiles()
        {
            var folderPath = vAtisProfileManager.GetProfilePath();
            Profiles.Clear();

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                return;
            }

            var jsonFiles = Directory.GetFiles(folderPath, "*.json");

            foreach (var file in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);

                    vAtisProfile? profile = JsonSerializer.Deserialize(jsonContent, vAtisJsonContext.Default.vAtisProfile);

                    if (profile != null)
                    {
                        profile.FilePath = file;
                        Profiles.Add(profile);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error loading profiles: {ex.Message}", typeof(vAtisProfileManager));
                }
            }

            Debug.WriteLine($"Loaded {vAtisProfiles.Count} profiles.", typeof(vAtisProfileManager));
        }

        /// <summary>
        /// Looks up the path to the vAtis profiles from the registry.
        /// </summary>
        /// <returns>The path to the profiles.</returns>
        private static string GetProfilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "org.vatsim.vatis", "Profiles");
        }
    }
}