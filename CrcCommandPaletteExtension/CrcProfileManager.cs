// <copyright file="ProfileManager.cs" company="Neil Enns">
// Copyright (c) Neil Enns. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
namespace CrcCommandPaletteExtension
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using Microsoft.Win32;

    /// <summary>
    /// Maintains a list of CRC profiles.
    /// </summary>
    public static class CrcProfileManager
    {
        private static readonly List<CrcProfile> Profiles = [];

        private static readonly string CrcProfilePath = CrcProfileManager.GetCrcProfilePath();

        /// <summary>
        /// Gets the list of profiles.
        /// </summary>
        public static IReadOnlyList<CrcProfile> CrcProfiles => Profiles;

        public static IEnumerable<CrcProfile> GetAllProfiles()
        {
            return GetMatchingProfiles("");
        }

        /// <summary>
        /// Returns a list of profiles that match the specified query.
        /// </summary>
        /// <param name="query">The text to search for.</param>
        /// <returns>The list of matching profiles. If query is null or empty all profiles are returned. If no profiles match an empty list is returned.</returns>
        public static IEnumerable<CrcProfile> GetMatchingProfiles(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || CrcProfiles.Count == 0)
            {
                // Issue #19: This ensures the profiles get reloaded every time a new query
                // is started, so new profiles get picked up.
                CrcProfileManager.LoadProfiles();
                return CrcProfiles;
            }

            return CrcProfiles.Where(profile =>
                profile.Name != null && profile.Name.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Loads the profiles from disk.
        /// </summary>
        public static void LoadProfiles()
        {
            Profiles.Clear();

            if (string.IsNullOrEmpty(CrcProfileManager.CrcProfilePath) || !Directory.Exists(CrcProfileManager.CrcProfilePath))
            {
                return;
            }

            var jsonFiles = Directory.GetFiles(CrcProfileManager.CrcProfilePath, "*.json");

            foreach (var file in jsonFiles)
            {
                try
                {
                    string jsonContent = File.ReadAllText(file);

                    // Use source-generated metadata to avoid trimming/runtime-analysis
                    // warnings for reflection-based serialization. The context is
                    // generated from CrcJsonContext.
                    CrcProfile? profile = JsonSerializer.Deserialize(jsonContent, CrcJsonContext.Default.CrcProfile);

                    if (profile != null)
                    {
                        profile.FilePath = file;
                        Profiles.Add(profile);
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// Looks up the path to the CRC profiles from the registry.
        /// </summary>
        /// <returns>The path to the profiles, or the empty string if not found.</returns>
        private static string GetCrcProfilePath()
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"Software\CRC");
                if (key is not null)
                {
                    string? installDir = key.GetValue("Install_Dir") as string;
                    if (!string.IsNullOrEmpty(installDir))
                    {
                        return Path.Combine(installDir, "Profiles");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CRC install path from registry: {ex.Message}");
            }

            return string.Empty;
        }
    }
}