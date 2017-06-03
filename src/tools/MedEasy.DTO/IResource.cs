﻿using MedEasy.RestObjects;
using System;

namespace MedEasy.DTO
{
    /// <summary>
    /// Describes the properties a browsable resource must implements
    /// </summary>
    /// <typeparam name="T">Type of the resource identifier</typeparam>
    public interface IResource<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Id of the resource
        /// </summary>
        T Id { get; }

        /// <summary>
        /// Metadata associated with the current resource.
        /// </summary>
        Link Meta { get; set; }

        /// <summary>
        /// Last time the resource was updated
        /// </summary>
        DateTimeOffset? UpdatedDate { get; }
    }
}