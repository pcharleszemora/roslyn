﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This is consumed as 'generated' code in a source package and therefore requires an explicit nullable enable
#nullable enable

namespace Microsoft.CommonLanguageServerProtocol.Framework;

#if BINARY_COMPAT // TODO - Remove with https://github.com/dotnet/roslyn/issues/72251
public interface IInitializeManager<TRequest, TResponse>
#else
internal interface IInitializeManager<TRequest, TResponse>
#endif
{
    /// <summary>
    /// Gets a response to be used for "initialize", completing the negoticaitons between client and server.
    /// </summary>
    /// <returns>An InitializeResult.</returns>
    TResponse GetInitializeResult();

    /// <summary>
    /// Store the InitializeParams for later retrieval.
    /// </summary>
    /// <param name="request">The InitializeParams to be stored.</param>
    void SetInitializeParams(TRequest request);

    /// <summary>
    /// Gets the InitializeParams to, for example, examine the ClientCapabilities.
    /// </summary>
    /// <returns>The InitializeParams object sent with "initialize".</returns>
    TRequest GetInitializeParams();
}
