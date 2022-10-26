// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information. 

using System;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.OData.Formatter;
using System.Web.Http.OData.Formatter.Deserialization;
using NuGet.Server.V2.OData.Serializers;
using System.Collections.Generic;
using NuGet.Server.Core.Infrastructure;

namespace NuGet.Server.V2.OData
{
    class NuGetODataControllerConfigurationAttribute : Attribute, IControllerConfiguration
    {
        private ISettingsProvider _settingsProvider;
        private static IList<ODataMediaTypeFormatter> _formatters;
        private static object _syncLock = new object();


        private IList<ODataMediaTypeFormatter> GetFormatters()
        {
            if (_formatters == null)
            {
                lock (_syncLock)
                {
                    if (_formatters == null)
                    {
                        
                        string defaultUriScheme = _settingsProvider.GetStringSetting("defaultUriScheme", string.Empty);
                        var serProvider = new CustomSerializerProvider(provider => new NuGetEntityTypeSerializer(provider, defaultUriScheme));
                        var createdFormatters = ODataMediaTypeFormatters.Create(serProvider, new DefaultODataDeserializerProvider());

                        var jsonFormatters = createdFormatters.Where(x => x.SupportedMediaTypes.Any(y => y.MediaType.Contains("json")));
                        createdFormatters.RemoveAll(x => jsonFormatters.Contains(x));
                        var xmlFormatterIndex = createdFormatters.IndexOf(createdFormatters.Last(x => x.SupportedMediaTypes.Any(y => y.MediaType.Contains("xml"))));
                        foreach (var formatter in jsonFormatters)
                        {
                            createdFormatters.Insert(xmlFormatterIndex++, formatter);
                        }

                        _formatters = createdFormatters;
                    }
                }
            }

            return _formatters;
        }


        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            _settingsProvider = controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(ISettingsProvider)) as ISettingsProvider;
                
            controllerSettings.Formatters.Clear();
            controllerSettings.Formatters.InsertRange(0, GetFormatters());
        }
    }
}