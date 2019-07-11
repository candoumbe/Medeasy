﻿using AutoMapper;
using MedEasy.Mapping;
using MedEasy.Objects;
using MedEasy.RestObjects;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Patients.DTO;
using Patients.Objects;
using System;

namespace Patients.Mapping
{
    /// <summary>
    /// Contains mappings configuration
    /// </summary>
    public static class AutoMapperConfig
    {
        /// <summary>
        /// Creates a new <see cref="MapperConfiguration"/>
        /// </summary>
        /// <returns></returns>
        public static MapperConfiguration Build() => new MapperConfiguration(cfg =>
        {
            cfg.CreateCoreMapping();

            cfg.CreateMap<Patient, PatientInfo>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(entity => entity.Id))
                .ForMember(dto => dto.MainDoctorId, opt => opt.Ignore())
                .IncludeBase<IEntity<Guid>, Resource<Guid>>()
                .ReverseMap()
                .ForMember(entity => entity.CreatedBy, opt => opt.Ignore())
                .ForMember(entity => entity.UpdatedBy, opt => opt.Ignore())
                .ForMember(entity => entity.UpdatedBy, opt => opt.Ignore())
                ;

            cfg.CreateMap(typeof(JsonPatchDocument<>), typeof(JsonPatchDocument<>));
            cfg.CreateMap(typeof(Operation<>), typeof(Operation<>));
        });
    }
}
