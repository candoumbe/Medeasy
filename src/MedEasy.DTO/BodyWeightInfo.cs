﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedEasy.DTO
{
    [JsonObject]
    public class BodyWeightInfo : PhysiologicalMeasurementInfo
    {
        
        /// <summary>
        /// Value of the measure
        /// </summary>
        public float Value { get; set; }

    }
}
