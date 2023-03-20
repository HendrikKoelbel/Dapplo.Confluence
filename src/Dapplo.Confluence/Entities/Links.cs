// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Newtonsoft.Json;

namespace Dapplo.Confluence.Entities;

/// <summary>
///     Links information
///     See: https://docs.atlassian.com/confluence/REST/latest
/// </summary>
[JsonObject]
public class Links
{
    /// <summary>
    ///     The base (hostname) for the server
    /// </summary>
    [JsonProperty("base", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Uri Base { get; set; }

    /// <summary>
    ///     A path to the rest API to where this belongs, content has a collection of "/rest/api/content"
    /// </summary>
    [JsonProperty("collection", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Collection { get; set; }

    /// <summary>
    ///     TODO: What is this?
    /// </summary>
    [JsonProperty("context", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Context { get; set; }

    /// <summary>
    ///     The link, usually for attachments, to download the content
    /// </summary>
    [JsonProperty("download", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Download { get; set; }

    /// <summary>
    ///     A link to the entity itself (so one can find it again)
    /// </summary>
    [JsonProperty("self", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Uri Self { get; set; }

    /// <summary>
    ///     Status of a task
    /// </summary>
    [JsonProperty("status", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Status { get; set; }

    /// <summary>
    ///     A short link to the content, relative to the hostname (and port)
    /// </summary>
    [JsonProperty("tinyui", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string TinyUi { get; set; }

    /// <summary>
    ///     A normal, but well readable, link to the content
    /// </summary>
    [JsonProperty("webui", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string WebUi { get; set; }

    /// <summary>
    ///     A link to the next result when using paging
    /// </summary>
    [JsonProperty("next", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Uri Next { get; set; }

    /// <summary>
    ///     A link to the prev result when using paging
    /// </summary>
    [JsonProperty("prev", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public Uri Prev { get; set; }
}