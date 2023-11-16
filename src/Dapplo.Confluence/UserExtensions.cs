// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dapplo.Confluence;

/// <summary>
///     Marker interface for the user domain
/// </summary>
public interface IUserDomain : IConfluenceDomain
{
}

/// <summary>
///     All user related functionality
/// </summary>
public static class UserDomain
{
#pragma warning disable IDE0090 // Use 'new(...)'
    private static readonly LogSource Log = new LogSource();
#pragma warning restore IDE0090 // Use 'new(...)'

    /// <summary>
    ///     Get Anonymous user information, introduced with 6.6
    ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getAnonymous
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>User</returns>
    public static async Task<User> GetAnonymousUserAsync(this IUserDomain confluenceClient, CancellationToken cancellationToken = default)
    {
        var myselfUri = confluenceClient.ConfluenceApiUri.AppendSegments("user", "anonymous");
        confluenceClient.Behaviour.MakeCurrent();
        var response = await myselfUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors();
    }

    /// <summary>
    ///     Get current user information, introduced with 6.6
    ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getCurrent
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>User</returns>
    public static async Task<User> GetCurrentUserAsync(this IUserDomain confluenceClient, CancellationToken cancellationToken = default)
    {
        confluenceClient.Behaviour.MakeCurrent();
        var myselfUri = confluenceClient.ConfluenceApiUri.AppendSegments("user", "current");

        var response = await myselfUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors();
    }

    /// <summary>
    ///     Get user information, introduced with 6.6
    ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getUser
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="username">string with username</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>User</returns>
    public static async Task<User> GetUserAsync(this IUserDomain confluenceClient, string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));

        var userUri = confluenceClient.ConfluenceApiUri
            .AppendSegments("user")
            .ExtendQuery("username", username);
        confluenceClient.Behaviour.MakeCurrent();
        var response = await userUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors();
    }

    /// <summary>
    ///     Get user information, introduced with 6.6
    ///     See: https://docs.atlassian.com/confluence/REST/latest/#user-getUser
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="userIdentifier">IUserIdentifier</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>User</returns>
    public static async Task<User> GetUserAsync(this IUserDomain confluenceClient, IUserIdentifier userIdentifier, CancellationToken cancellationToken = default)
    {
        if (userIdentifier == null) throw new ArgumentNullException(nameof(userIdentifier));

        var userUri = confluenceClient.ConfluenceApiUri
            .AppendSegments("user");
        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        if (isCloudServer)
        {
            if (string.IsNullOrEmpty(userIdentifier.AccountId)) throw new ArgumentNullException(nameof(userIdentifier));
            userUri = userUri.ExtendQuery("accountId", userIdentifier.AccountId);
        }
        else
        {
            if (string.IsNullOrEmpty(userIdentifier.Username)) throw new ArgumentNullException(nameof(userIdentifier));
            userUri = userUri.ExtendQuery("username", userIdentifier.Username);
        }
                
        confluenceClient.Behaviour.MakeCurrent();
        var response = await userUri.GetAsAsync<HttpResponse<User, Error>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors();
    }

    /// <summary>
    ///  Get groups for the specified user, introduced with 6.6
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-memberof-get
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="userIdentifier">IUserIdentifier</param>
    /// <param name="pagingInformation">PagingInformation</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>List with Groups</returns>
    public static async Task<IList<Group>> GetGroupMembershipsAsync(this IUserDomain confluenceClient, IUserIdentifier userIdentifier, PagingInformation pagingInformation = null, CancellationToken cancellationToken = default)
    {
        if (userIdentifier == null) throw new ArgumentNullException(nameof(userIdentifier));

        pagingInformation ??= new PagingInformation
        {
            Limit = 200
        };

        Uri groupUri;

        if (pagingInformation.Links != null)
        {
            groupUri = pagingInformation.GetUriFromLinks();
        }
        else
        {
            groupUri = confluenceClient.ConfluenceApiUri
                .AppendSegments("user", "memberof");

            if (pagingInformation.Limit != null)
            {
                groupUri = groupUri.ExtendQuery("limit", pagingInformation.Limit);
            }

            bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
            if (isCloudServer)
            {
                if (string.IsNullOrEmpty(userIdentifier.AccountId)) throw new ArgumentNullException(nameof(userIdentifier));
                groupUri = groupUri.ExtendQuery("accountId", userIdentifier.AccountId);
            }
            else
            {
                if (string.IsNullOrEmpty(userIdentifier.Username)) throw new ArgumentNullException(nameof(userIdentifier));
                groupUri = groupUri.ExtendQuery("username", userIdentifier.Username);
            }
        }

        confluenceClient.Behaviour.MakeCurrent();
        var response = await groupUri.GetAsAsync<HttpResponse<Result<Group>, Error>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors()?.Results;
    }

    /// <summary>
    /// Helper method to generate the URL needed to contact Confluence.
    /// The display way might be nicer.
    /// </summary>
    /// <param name="confluenceClient">IUserDomain</param>
    /// <param name="isCloudServer">boll with some information</param>
    /// <param name="userIdentifier">IUserDomain</param>
    /// <param name="segments">params with strings</param>
    /// <returns>Uri</returns>
    private static Uri CreateUserWatchUri(this IUserDomain confluenceClient, bool isCloudServer, IUserIdentifier userIdentifier, params object[] segments)
    {
        var userWatchContentUri = confluenceClient.ConfluenceApiUri
            .AppendSegments("user", "watch")
            .AppendSegments(segments);

        // If there is no specified accountId, the current user is used
        if (userIdentifier == null) return userWatchContentUri;
            
        if (isCloudServer)
        {
            // Check the account id value.
            if (string.IsNullOrEmpty(userIdentifier.AccountId)) throw new ArgumentNullException(nameof(userIdentifier), "It seems that there is no account ID supplied.");
            userWatchContentUri = userWatchContentUri.ExtendQuery("accountId", userIdentifier.AccountId);
        }
        else
        {
            // Check the username value.
            if (string.IsNullOrEmpty(userIdentifier.Username)) throw new ArgumentNullException(nameof(userIdentifier), "It seems that there is no username supplied.");

            userWatchContentUri = userWatchContentUri.ExtendQuery("username", userIdentifier.Username);
        }

        return userWatchContentUri;
    }

    /// <summary>
    ///  Add the user to the list of users watching the specified content
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-post
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="contentId">long with the ID for the content</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task AddContentWatcher(this IUserDomain confluenceClient, long contentId, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);

        var userWatchContentUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "content", contentId);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchContentUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);
            
        // Expect a 204, which is NoContent, when we are in the cloud
        if (isCloudServer)
        {
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }
        else
        {
            response.HandleStatusCode(HttpStatusCode.OK);
        }
    }

    /// <summary>
    ///  Remove the user from the list of users watching the specified content
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-delete
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="contentId">long with the ID for the content</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task RemoveContentWatcher(this IUserDomain confluenceClient, long contentId, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);

        var userWatchContentUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "content", contentId);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchContentUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken).ConfigureAwait(false);

        // Expect a 204, which is NoContent
        response.HandleStatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    ///  Check if the user is watching the specified content
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="contentId">long with the ID for the content</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>bool</returns>
    public static async Task<bool> IsContentWatcher(this IUserDomain confluenceClient, long contentId, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (contentId == 0) throw new ArgumentNullException(nameof(contentId));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);

        var userWatchContentUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "content", contentId);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchContentUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors().IsWatching;
    }

    /// <summary>
    ///  Add the user to the list of users watching the specified label
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-label-labelName-post
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="label">string with the label</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task AddLabelWatcher(this IUserDomain confluenceClient, string label, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        if (!isCloudServer)
        {
            Log.Warn().WriteLine("Confluence server doesn't support label watch functionality.");
        }
        var userWatchLabelUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "label", label);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchLabelUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);

        // Expect a 204, which is NoContent
        response.HandleStatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    ///  Remove the user from the list of users watching the specified label
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-label-labelName-delete
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="label">string with the label</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task RemoveLabelWatcher(this IUserDomain confluenceClient, string label, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        if (!isCloudServer)
        {
            Log.Warn().WriteLine("Confluence server doesn't support label watch functionality.");
        }
        var userWatchLabelUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "label", label);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchLabelUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken: cancellationToken).ConfigureAwait(false);

        // Expect a 204, which is NoContent, when we are in the cloud
        response.HandleStatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    ///  Check if the user is watching the specified label
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="label">string with the label</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>bool</returns>
    public static async Task<bool> IsLabelWatcher(this IUserDomain confluenceClient, string label, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(label)) throw new ArgumentNullException(nameof(label));
            
        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        if (!isCloudServer)
        {
            Log.Warn().WriteLine("Confluence server doesn't support label watch functionality.");
        }
        var userWatchLabelUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "label", label);

        confluenceClient.Behaviour.MakeCurrent();
            
        var response = await userWatchLabelUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors()?.IsWatching ?? false;
    }

    /// <summary>
    ///  Add the user to the list of users watching the specified space
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-space-spaceKey-get
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="spaceKey">string with the space key</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task AddSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        var userWatchSpaceUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "space", spaceKey);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchSpaceUri.PostAsync<HttpResponseWithError<Error>>(null, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (isCloudServer)
        {
            // Expect a 204, which is NoContent for the Cloud Server
            response.HandleStatusCode(HttpStatusCode.NoContent);
        }
        else
        {
            // Expect a 200 for Confluence server
            response.HandleStatusCode(HttpStatusCode.OK);
        }
    }

    /// <summary>
    ///  Remove the user from the list of users watching the specified space
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-space-spaceKey-delete
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="spaceKey">string with the space key</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    public static async Task RemoveSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        var userWatchSpaceUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "space", spaceKey);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchSpaceUri.DeleteAsync<HttpResponseWithError<Error>>(cancellationToken).ConfigureAwait(false);

        // Expect a 204, which is NoContent
        response.HandleStatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    ///  Check if the user is watching the specified space
    ///     See: https://developer.atlassian.com/cloud/confluence/rest/#api-api-user-watch-content-contentId-get
    /// </summary>
    /// <param name="confluenceClient">IUserDomain to bind the extension method to</param>
    /// <param name="spaceKey">string with the space key</param>
    /// <param name="userIdentifier">IUserIdentifier for the user (account id), null for the current user</param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns>bool</returns>
    public static async Task<bool> IsSpaceWatcher(this IUserDomain confluenceClient, string spaceKey, IUserIdentifier userIdentifier = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(spaceKey)) throw new ArgumentNullException(nameof(spaceKey));

        bool isCloudServer = await confluenceClient.IsCloudServer(cancellationToken);
        var userWatchSpaceUri = CreateUserWatchUri(confluenceClient, isCloudServer, userIdentifier, "space", spaceKey);

        confluenceClient.Behaviour.MakeCurrent();
        var response = await userWatchSpaceUri.GetAsAsync<HttpResponse<UserWatch>>(cancellationToken).ConfigureAwait(false);
        return response.HandleErrors().IsWatching;
    }
}