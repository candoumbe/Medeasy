﻿using MedEasy.DTO;
using MedEasy.Handlers.Core.Queries;
using MedEasy.Queries;
using System;

namespace MedEasy.Handlers.Core.Appointment.Queries
{

    public interface IHandleGetAppointmentInfoByIdQuery : IHandleQueryAsync<Guid, Guid, AppointmentInfo, IWantOneResource<Guid, Guid, AppointmentInfo>>
    {
    }
}