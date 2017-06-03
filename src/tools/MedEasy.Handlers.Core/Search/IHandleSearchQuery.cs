﻿using MedEasy.DAL.Repositories;
using MedEasy.Queries.Search;
using System.Threading;
using System.Threading.Tasks;

namespace MedEasy.Handlers.Core.Search.Queries
{
    public interface IHandleSearchQuery
    {
        /// <summary>
        /// Performs the search query
        /// </summary>
        /// <remarks>
        /// <typeparamref name="TEntity"/> shouuld be convertible to <typeparamref name="TResult"/.>
        /// </remarks>
        /// <typeparam name="TEntity">Type of the resource to perform query on</typeparam>
        /// <typeparam name="TResult">Type of the result to perform query on</typeparam>
        /// <param name="searchQuery">The search criteria</param>
        /// <returns><see cref="IPagedResult{T}"/> which holds the result of the search.</returns>
        Task<IPagedResult<TResult>> Search<TEntity, TResult>(SearchQuery<TResult> searchQuery, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
    }
}