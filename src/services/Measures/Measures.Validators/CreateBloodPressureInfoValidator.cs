﻿using FluentValidation;
using Measures.DTO;
using MedEasy.DAL.Interfaces;
using System;
using static FluentValidation.Severity;

namespace Measures.Validators
{
    /// <summary>
    /// Validator for <see cref="CreateBloodPressureInfo"/> instances.
    /// </summary>
    public class CreateBloodPressureInfoValidator : AbstractValidator<CreateBloodPressureInfo>
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        /// <summary>
        /// Builds a new <see cref="CreateBloodPressureInfoValidator"/> instance. 
        /// </summary>
        /// <param name="uowFactory">Factory to build <see cref="IUnitOfWork"/> instances.</param>
        public CreateBloodPressureInfoValidator(IUnitOfWorkFactory uowFactory)
        {
            _unitOfWorkFactory = uowFactory ?? throw new ArgumentNullException(nameof(uowFactory), $"{nameof(uowFactory)} cannot be null");

            RuleFor(x => x.DiastolicPressure)
                .GreaterThan(0)
                .WithSeverity(Warning);

            RuleFor(x => x.SystolicPressure)
                .GreaterThan(0)
                .WithSeverity(Warning);

            When(x => x.SystolicPressure > 0 && x.DiastolicPressure > 0, () =>
            {
                RuleFor(x => x.DiastolicPressure)
                    .LessThan((info) => info.SystolicPressure)
                    .WithSeverity(Error);
            });

        }
    }
}
