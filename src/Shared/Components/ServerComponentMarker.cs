// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Microsoft.AspNetCore.Components.RenderTree;

namespace Microsoft.AspNetCore.Components;

#nullable enable // This is shared-source with Mvc.ViewFeatures which doesn't enable nullability by default

// Represents the serialized invocation to a component.
// We serialize this marker into a comment in the generated
// HTML.
internal struct ServerComponentMarker
{
    public const string ServerMarkerType = "server";

    private ServerComponentMarker(string? type, string? descriptor, int? sequence, string? key, string? prerenderId) : this()
    {
        Type = type;
        PrerenderId = prerenderId;
        Descriptor = descriptor;
        Sequence = sequence;
        Key = key;
    }

    // The order in which this component was rendered/produced
    // on the server. It matches the number on the descriptor
    // and is used to prevent an infinite amount of components
    // from being rendered from the client-side.
    public int? Sequence { get; set; }

    // The marker type. Right now "server" is the only valid value.
    // The value will be null for end markers.
    public string? Type { get; set; }

    // A string to allow the clients to differentiate between prerendered
    // and non prerendered components and to uniquely identify start and end
    // markers in prererendered components.
    // The value will be null if this marker represents a non-prerendered component.
    public string? PrerenderId { get; set; }

    // A data-protected payload that allows the server to validate the legitimacy
    // of the invocation.
    // The value will be null for end markers.
    public string? Descriptor { get; set; }

    // An additional string that the browser can use when comparing markers to determine
    // whether they represent different component instances.
    public string? Key { get; set; }

    // Creates a marker for a prerendered component.
    public static ServerComponentMarker Prerendered(int sequence, string descriptor, string? key) =>
        new ServerComponentMarker(ServerMarkerType, descriptor, sequence, key, Guid.NewGuid().ToString("N"));

    // Creates a marker for a non prerendered component
    public static ServerComponentMarker NonPrerendered(int sequence, string descriptor, string? key) =>
        new ServerComponentMarker(ServerMarkerType, descriptor, sequence, key, null);

    // Creates the end marker for a prerendered component.
    public ServerComponentMarker GetEndRecord()
    {
        if (PrerenderId == null)
        {
            throw new InvalidOperationException("Can't get an end record for non-prerendered components.");
        }

        return new ServerComponentMarker(null, null, null, null, PrerenderId);
    }
}
