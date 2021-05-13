﻿
using MedEasy.Ids;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;

namespace Agenda.Ids
{
    public record AppointmentId(Guid Value) : StronglyTypedId<Guid>(Value)
    {
        public static AppointmentId New() => new(Guid.NewGuid());

        public static AppointmentId Empty => new(Guid.Empty);

#pragma warning disable S1185 // Overriding members should do more than simply call the same member in the base class
        public override string ToString() => base.ToString();
#pragma warning restore S1185 // Overriding members should do more than simply call the same member in the base class


        public class EfValueConverter : ValueConverter<AppointmentId, Guid>
        {
            public EfValueConverter(ConverterMappingHints mappingHints = null) : base(
                id => id.Value,
                value => new AppointmentId(value),
                mappingHints
            )
            { }
        }
    }

}
