// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Confluence.Entities;

/// <summary>
/// Enum to choose from where to source the next page
/// </summary>
public enum PageSource
{
    /// <summary>
    /// If Links is set, source from the initial page
    /// </summary>
    Initial = 0,
    /// <summary>
    /// If Links is set, source from the next page
    /// </summary>
    Next = 1,
    /// <summary>
    /// If Links is set, source from the previous page
    /// </summary>
    Prev = 2
}

/// <summary>
/// Contains some basic paging information
/// </summary>
public class PagingInformation
{
    /// <summary>
    ///     Page size to limit the result set
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    ///     UNUSED - kept only for backward compatibility - should be removed
    /// </summary>
    public int? Start { get; set; }

    /// <summary>
    ///     The links to be used for paging
    /// </summary>
    public Links Links { get; set; }

    /// <summary>
    ///     The source of page to be used from the links
    /// </summary>
    public PageSource PageSource { get; set; } = PageSource.Initial;

    /// <summary>
    ///     Constructor
    /// </summary>
    public PagingInformation(int? limit = null, int? start = null, Links links = null, PageSource pageSource = PageSource.Initial)
    {
        Limit = limit;
        Start = start;
        Links = links;
        PageSource = pageSource;
    }

    /// <summary>
    ///     Returns a Uri based on the Paging Information
    /// </summary>
    public Uri GetUriFromLinks()
    {
        if (PageSource == PageSource.Initial)
        {
            if (Limit == null)
            {
                return Links.Self;
            }

            return Links.Self.ExtendQuery("limit", Limit);
        }

        if (PageSource == PageSource.Next)
        {
            if (Links?.Next != null)
            {
                return new Uri(string.Concat(Links.Base.ToString(), Links.Next.ToString()));
            }

            throw new ArgumentException("Request for next page when there is no next page link.", nameof(PageSource));
        }

        if (PageSource == PageSource.Prev)
        {
            if (Links?.Prev != null)
            {
                return new Uri(string.Concat(Links.Base.ToString(), Links.Prev.ToString()));
            }

            throw new ArgumentException("Request for previous page when there is no prev page link.", nameof(PageSource));
        }

        throw new ArgumentException("Invalid page source.", nameof(PageSource));
    }
}