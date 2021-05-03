﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics.Analyzers.NamingStyles;
using Microsoft.CodeAnalysis.Simplification;

namespace Microsoft.VisualStudio.LanguageServices.CSharp.Options
{
    public partial class AutomationObject
    {
        public string Style_NamingPreferences
        {
            get
            {
                return _workspace.Options.GetOption(NamingStyleOptions.NamingPreferences, LanguageNames.CSharp).CreateXElement().ToString();
            }

            set
            {
                try
                {
                    _workspace.TryApplyChanges(_workspace.CurrentSolution.WithOptions(_workspace.Options
                        .WithChangedOption(NamingStyleOptions.NamingPreferences, LanguageNames.CSharp, NamingStylePreferences.FromXElement(XElement.Parse(value)))));
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
