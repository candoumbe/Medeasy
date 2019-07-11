﻿using AutoMapper.QueryableExtensions;
using MedEasy.DAL.Interfaces;
using MedEasy.DAL.Repositories;
using DataFilters;
using MedEasy.DTO.Search;
using MedEasy.Objects;
using MedEasy.RestObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Patients.API.Controllers
{
    /// <summary>
    /// Base class for all controllers of the application
    /// </summary>
    [ApiController]
    public abstract class AbstractBaseController<TEntity, TResource, TResourceId>
        where TEntity : class, IEntity<Guid>
        where TResource : Resource<TResourceId>
        where TResourceId : IEquatable<TResourceId>
    {
        /// <summary>
        /// The <see cref="ILogger"/> instance used by the controller
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Factory to build <see cref="IUnitOfWork"/> instances.
        /// </summary>
        protected IUnitOfWorkFactory UowFactory { get; }

        /// <summary>
        /// Builds expression to map instance of one type to an other
        /// </summary>
        protected IExpressionBuilder ExpressionBuilder { get; }

        /// <summary>
        /// Helper to build <see cref="Uri"/>
        /// </summary>
        protected IUrlHelper UrlHelper { get; }

        /// <summary>
        /// Name of the resource
        /// </summary>
        protected abstract string ControllerName { get; }

        /// <summary>
        /// Builds a new <see cref="AbstractBaseController{TEntity, TResource, TResourceId}"/> instance.
        /// </summary>
        /// <param name="logger">the logger</param>
        /// <param name="uowFactory">Factory class that builds <see cref="IUnitOfWork"/> instances.</param>
        /// <param name="expressionBuilder"></param>
        /// <param name="urlHelper"></param>
        protected AbstractBaseController(ILogger logger, IUnitOfWorkFactory uowFactory, IExpressionBuilder expressionBuilder, IUrlHelper urlHelper)
        {
            Logger = logger;
            UowFactory = uowFactory;
            ExpressionBuilder = expressionBuilder;
            UrlHelper = urlHelper;
        }

        /// <summary>
        /// Performs the specified search 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<Page<TResource>> Search(SearchQueryInfo<TResource> search, CancellationToken cancellationToken = default)
        {
            using (IUnitOfWork uow = UowFactory.NewUnitOfWork())
            {
                Expression<Func<TEntity, bool>> filter = search.Filter?.ToExpression<TEntity>() ?? (_ => true);
                Expression<Func<TEntity, TResource>> selector = ExpressionBuilder.GetMapExpression<TEntity, TResource>();

                return await uow.Repository<TEntity>()
                    .WhereAsync(
                        selector,
                        filter ,
                        search.Sort,
                        search.Page,
                        search.PageSize,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}
