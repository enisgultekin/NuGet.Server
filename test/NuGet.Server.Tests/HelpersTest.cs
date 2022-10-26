// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using Moq;
using NuGet.Server.Core.Infrastructure;
using Xunit;

namespace NuGet.Server.Tests
{
    public class HelpersTest
    {
        [Fact]
        public void GetRepositoryUrlCreatesProperUrlWithRootWebApp()
        {
            // Arrange
            Mock<ISettingsProvider> settingsProviderMock = new Mock<ISettingsProvider>();
            var url = new Uri("http://example.com/default.aspx");
            var applicationPath = "/";

            // Act
            var repositoryUrl = Helpers.GetRepositoryUrl(settingsProviderMock.Object, url, applicationPath);

            // Assert
            Assert.Equal("http://example.com/nuget", repositoryUrl);
        }

        [Fact]
        public void GetRepositoryUrlCreatesProperUrlWithVirtualApp()
        {
            // Arrange
            Mock<ISettingsProvider> settingsProviderMock = new Mock<ISettingsProvider>();
            var url = new Uri("http://example.com/Foo/default.aspx");
            var applicationPath = "/Foo";

            // Act
            var repositoryUrl = Helpers.GetRepositoryUrl(settingsProviderMock.Object, url, applicationPath);

            // Assert
            Assert.Equal("http://example.com/Foo/nuget", repositoryUrl);
        }

        [Fact]
        public void GetRepositoryUrlWithNonStandardPortCreatesProperUrlWithRootWebApp()
        {
            // Arrange
            Mock<ISettingsProvider> settingsProviderMock = new Mock<ISettingsProvider>();
            var url = new Uri("http://example.com:1337/default.aspx");
            var applicationPath = "/";

            // Act
            var repositoryUrl = Helpers.GetRepositoryUrl(settingsProviderMock.Object, url, applicationPath);

            // Assert
            Assert.Equal("http://example.com:1337/nuget", repositoryUrl);
        }

        [Fact]
        public void GetRepositoryUrlWithNonStandardPortCreatesProperUrlWithVirtualApp()
        {
            // Arrange
            Mock<ISettingsProvider> settingsProviderMock = new Mock<ISettingsProvider>();
            var url = new Uri("http://example.com:1337/Foo/default.aspx");
            var applicationPath = "/Foo";

            // Act
            var repositoryUrl = Helpers.GetRepositoryUrl(settingsProviderMock.Object, url, applicationPath);

            // Assert
            Assert.Equal("http://example.com:1337/Foo/nuget", repositoryUrl);
        }
    }
}