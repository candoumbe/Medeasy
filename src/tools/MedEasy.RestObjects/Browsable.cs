﻿#if NETSTANDARD1_1
using Newtonsoft.Json;
#endif
using System.Collections.Generic;
using System.Linq;
using Forms;

namespace MedEasy.RestObjects
{
    /// <summary>
    /// A wrapper of a <typeparamref name="T"/> resource with its current <see cref="Links"/>
    /// </summary>
    /// <typeparam name="T">Type of the resource that will be wrapped</typeparam>
#if NETSTANDARD1_1
    [JsonObject]
#endif
    public class Browsable<T>
    {
        private IEnumerable<Link> _links;

        /// <summary>
        /// Location of the resource. Can be cached for further operations
        /// </summary>
#if NETSTANDARD1_1
        [JsonProperty]
#endif
        public IEnumerable<Link> Links
        {
            get => _links ?? Enumerable.Empty<Link>();

            set => _links = value ?? Enumerable.Empty<Link>();
        }

        /// <summary>
        /// The resource that can be later retrieve using the <see cref="Links"/> property
        /// </summary>
#if NETSTANDARD1_1
        [JsonProperty]
#endif
        public T Resource { get; set; }

        /// <summary>
        /// Builds a new <see cref="Browsable{T}"/> instance.
        /// </summary>
        public Browsable() => _links = Enumerable.Empty<Link>();
    }
}
