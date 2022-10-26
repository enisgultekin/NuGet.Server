// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Configuration;
using NuGet.Server.Core.Infrastructure;

namespace NuGet.Server
{
    public static class Helpers
    {
        public static string GetRepositoryUrl(ISettingsProvider settingsProvider,Uri currentUrl, string applicationPath)
        {
            string defaultUriScheme = settingsProvider.GetStringSetting("defaultUriScheme", string.Empty);
            return GetBaseUrl(currentUrl, applicationPath, defaultUriScheme) + "nuget";
        }

        public static string GetPushUrl(ISettingsProvider settingsProvider,Uri currentUrl, string applicationPath)
        {
            string defaultUriScheme = settingsProvider.GetStringSetting("defaultUriScheme", string.Empty);
            return GetBaseUrl(currentUrl, applicationPath, defaultUriScheme) + "nuget";
        }

        public static string GetBaseUrl(Uri currentUrl, string applicationPath, string defaultUriScheme)
        {
            var uriBuilder = new UriBuilder(currentUrl);

            string uriScheme = uriBuilder.Scheme;
            if (!string.IsNullOrEmpty(defaultUriScheme))
                uriScheme = defaultUriScheme;
            
            var repositoryUrl = uriScheme + "://" + uriBuilder.Host;
            if (uriBuilder.Port != 80 && uriBuilder.Port != 443)
            {
                repositoryUrl += ":" + uriBuilder.Port;
            }

            repositoryUrl += applicationPath;

            // ApplicationPath for Virtual Apps don't end with /
            return EnsureTrailingSlash(repositoryUrl);
        }

        internal static string EnsureTrailingSlash(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return path;
            }

            if (!path.EndsWith("/"))
            {
                return path + "/";
            }

            return path;
        }
    }
}