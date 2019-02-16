﻿using System;

namespace Measures.CQRS.Events.BloodPressures
{
    /// <summary>
    /// Notifies that a <see cref="BloodPressureInfo"/> was updated.
    /// </summary>
    public class BloodPressureUpdated: MeasureDeleted<Guid, Guid>
    {
        /// <summary>
        /// Builds a new <see cref="BloodPressureUpdated"/> instance
        /// </summary>
        /// <param name="measureId">Unique identifier of the updated <see cref="Objects.BloodPressure"/></param>
        public BloodPressureUpdated(Guid measureId) : base(Guid.NewGuid(), measureId)
        {
        }
    }
}
