﻿using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using System.Linq;
using static Newtonsoft.Json.Required;
using static Newtonsoft.Json.DefaultValueHandling;
using static Newtonsoft.Json.JsonConvert;
using MedEasy.Data.Converters;

namespace MedEasy.Data
{
    /// <summary>
    /// An instance of this class holds combination of <see cref="IDataFilter"/>
    /// </summary>
    [JsonObject]
    [JsonConverter(typeof(DataCompositeFilterConverter))]
    public class DataCompositeFilter : IDataFilter
    {
        /// <summary>
        /// Name of the json property that holds filter's filters collection.
        /// </summary>
        public const string FiltersJsonPropertyName = "filters";

        /// <summary>
        /// Name of the json property that holds the composite filter's logic
        /// </summary>
        public const string LogicJsonPropertyName = "logic";

        public static JSchema Schema => new JSchema
        {
            Type = JSchemaType.Object,
            Properties =
            {
                [FiltersJsonPropertyName] = new JSchema { Type = JSchemaType.Array, MinimumItems = 2 },
                [LogicJsonPropertyName] = new JSchema { Type = JSchemaType.String, Default = "and"}
            },
            Required = { FiltersJsonPropertyName }
        };

        /// <summary>
        /// Collections of filters
        /// </summary>
        [JsonProperty(PropertyName = FiltersJsonPropertyName, Required = Always)]
        public IEnumerable<IDataFilter> Filters { get; set; } = Enumerable.Empty<IDataFilter>();

        /// <summary>
        /// Operator to apply between <see cref="Filters"/>
        /// </summary>
        [JsonProperty(PropertyName = LogicJsonPropertyName, DefaultValueHandling = IgnoreAndPopulate)]
        [JsonConverter(typeof(CamelCaseEnumTypeConverter))]
        public DataFilterLogic Logic { get; set; } = DataFilterLogic.And;

        public virtual string ToJson()
#if DEBUG
        => SerializeObject(this, Formatting.Indented);
#else
            => SerializeObject(this);
#endif

#if DEBUG
        public override string ToString() => ToJson();
#endif



    }

}