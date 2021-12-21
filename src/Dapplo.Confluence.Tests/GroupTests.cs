﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Dapplo.Confluence.Tests;

/// <summary>
///     Tests for group related functionality
/// </summary>
[CollectionDefinition("Dapplo.Confluence")]
public class GroupTests : ConfluenceIntegrationTests
{
    public GroupTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    /// <summary>
    ///     Test if the list of Groups is returned correctly
    /// </summary>
    [Fact]
    public async Task TestGetGroups()
    {
        var groups = await ConfluenceTestClient.Group.GetGroupsAsync();
        Assert.NotEmpty(groups);
    }

    /// <summary>
    ///     Test if one of the groups the current user belongs to, also has the current user as a member
    /// </summary>
    [Fact]
    public async Task TestGetGroupMembersAsync()
    {
        var currentUser = await ConfluenceTestClient.User.GetCurrentUserAsync();
        var groupsForUser = await ConfluenceTestClient.User.GetGroupMembershipsAsync(currentUser);

        var usersInGroup = await ConfluenceTestClient.Group.GetGroupMembersAsync(groupsForUser.First().Name);

        Assert.Contains(currentUser.AccountId, usersInGroup.Select(u => u.AccountId));
    }
}