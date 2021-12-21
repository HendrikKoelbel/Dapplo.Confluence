﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Dapplo.Confluence.Query;

/// <summary>
///     Factory method for CQL where clauses
/// </summary>
public static class Where
{
	/// <summary>
	///     Create a clause for the created field
	/// </summary>
	public static IDatetimeClause Created => new DatetimeClause(Fields.Created);

	/// <summary>
	///     Create a clause for the lastmodified field
	/// </summary>
	public static IDatetimeClause LastModified => new DatetimeClause(Fields.LastModified);


	/// <summary>
	///     Create a clause for the Space field
	/// </summary>
	public static ISpaceClause Space => new SpaceClause();

	/// <summary>
	///     Create a clause for the type field
	/// </summary>
	public static ITypeClause Type => new TypeClause();

	/// <summary>
	///     Create a clause for the Title field
	/// </summary>
	public static ITitleClause Title => new TitleClause(Fields.Title);

	/// <summary>
	///     Create a clause for the creator
	/// </summary>
	public static IUserClause Creator => new UserClause(Fields.Creator);

	/// <summary>
	///     Create a clause for the contributor
	/// </summary>
	public static IUserClause Contributor => new UserClause(Fields.Contributor);

	/// <summary>
	///     Create a clause for the mention
	/// </summary>
	public static IUserClause Mention => new UserClause(Fields.Mention);

	/// <summary>
	///     Create a clause for the watcher
	/// </summary>
	public static IUserClause Watcher => new UserClause(Fields.Watcher);

	/// <summary>
	///     Create a clause for the favourite
	/// </summary>
	public static IUserClause Favourite => new UserClause(Fields.Favourite);

	/// <summary>
	/// Create an AND clause of the specified clauses
	/// </summary>
	/// <param name="clauses">two or more IFinalClause</param>
	/// <returns>IFinalClause which ands the passed clauses</returns>
	public static IFinalClause And(params IFinalClause[] clauses)
	{
		return new Clause("(" + string.Join(" and ", clauses.ToList()) + ")");
	}

	/// <summary>
	/// Create an OR clause of the specified clauses
	/// </summary>
	/// <param name="clauses">two or more IFinalClause</param>
	/// <returns>IFinalClause which ors the passed clauses</returns>
	public static IFinalClause Or(params IFinalClause[] clauses)
	{
		return new Clause("(" + string.Join(" or ", clauses.ToList()) + ")");
	}

	/// <summary>
	///     Create a clause for the Text field
	/// </summary>
	public static ITextClause Text => new TextClause(Fields.Text);

	/// <summary>
	///     Create a clause for the Id field
	/// </summary>
	public static IContentClause Id => new ContentClause(Fields.Id);

	/// <summary>
	///     Create a clause for the Ancestor field
	/// </summary>
	public static IContentClause Ancestor => new ContentClause(Fields.Ancestor);

	/// <summary>
	///     Create a clause for the Content field
	/// </summary>
	public static IContentClause Content => new ContentClause(Fields.Content);

	/// <summary>
	///     Create a clause for the Parent field
	/// </summary>
	public static IContentClause Parent => new ContentClause(Fields.Parent);

	/// <summary>
	///     Create a clause for the Label field
	/// </summary>
	public static ISimpleValueClause Label => new SimpleValueClause(Fields.Label);


	/// <summary>
	///     Create a clause for the Container field
	/// </summary>
	public static ISimpleValueClause Container => new SimpleValueClause(Fields.Container);

	/// <summary>
	///     Create a clause for the Macro field
	/// </summary>
	public static ISimpleValueClause Macro => new SimpleValueClause(Fields.Macro);
}