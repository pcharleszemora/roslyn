﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.FindSymbols
{
    /// <summary>
    /// Represents a group of <see cref="ISymbol"/>s that should be treated as a single entity for
    /// the purposes of presentation in a Find UI.  For example, when a symbol is defined in a file
    /// that is linked into multiple project contexts, there will be several unique symbols created
    /// that we search for.  Placing these in a group allows the final consumer to know that these 
    /// symbols can be merged together.
    /// </summary>
    internal class SymbolGroup : IEquatable<SymbolGroup>
    {
        /// <summary>
        /// The main symbol of the group (normally the symbol that was searched for).
        /// </summary>
        public ISymbol PrimarySymbol { get; }

        /// <summary>
        /// All the symbols in the group.  Will include <see cref="PrimarySymbol"/>.
        /// </summary>
        public ImmutableHashSet<ISymbol> Symbols { get; }

        private int _hashCode;

        public SymbolGroup(ISymbol primarySymbol, ImmutableArray<ISymbol> symbols)
        {
            Contract.ThrowIfTrue(symbols.IsDefaultOrEmpty);
            Contract.ThrowIfFalse(symbols.Contains(primarySymbol));

            // We should only get an actual group of symbols if these were from source.
            // Metadata symbols never form a group.
            Contract.ThrowIfTrue(symbols.Length >= 2 && symbols.Any(s => s.Locations.Any(loc => loc.IsInMetadata)));

            PrimarySymbol = primarySymbol;
            Symbols = ImmutableHashSet.CreateRange(
                MetadataUnifyingEquivalenceComparer.Instance, symbols);
        }

        public override bool Equals(object? obj)
            => obj is SymbolGroup group && Equals(group);

        public bool Equals(SymbolGroup? group)
            => this == group || (group != null && Symbols.SetEquals(group.Symbols));

        public override int GetHashCode()
        {
            if (_hashCode == 0)
            {
                foreach (var symbol in Symbols)
                    _hashCode += MetadataUnifyingEquivalenceComparer.Instance.GetHashCode(symbol);
            }

            return _hashCode;
        }
    }

    /// <summary>
    /// Reports the progress of the FindReferences operation.  Note: these methods may be called on
    /// any thread.
    /// </summary>
    internal interface IStreamingFindReferencesProgress
    {
        IStreamingProgressTracker ProgressTracker { get; }

        ValueTask OnStartedAsync();
        ValueTask OnCompletedAsync();

        ValueTask OnFindInDocumentStartedAsync(Document document);
        ValueTask OnFindInDocumentCompletedAsync(Document document);

        ValueTask OnDefinitionFoundAsync(SymbolGroup group);
        ValueTask OnReferenceFoundAsync(SymbolGroup group, ISymbol symbol, ReferenceLocation location);
    }

    internal interface IStreamingFindLiteralReferencesProgress
    {
        IStreamingProgressTracker ProgressTracker { get; }

        ValueTask OnReferenceFoundAsync(Document document, TextSpan span);
    }
}
