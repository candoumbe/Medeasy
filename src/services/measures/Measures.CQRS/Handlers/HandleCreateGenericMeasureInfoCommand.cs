﻿using AutoMapper.QueryableExtensions;

using Measures.CQRS.Commands;
using Measures.DTO;
using Measures.Objects;

using MedEasy.Abstractions;
using MedEasy.CQRS.Core.Commands.Results;
using MedEasy.DAL.Interfaces;

using MediatR;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

using Optional;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Measures.CQRS.Handlers
{
    public class HandleCreateGenericMeasureInfoCommand : IRequestHandler<CreateGenericMeasureInfoCommand, Option<GenericMeasureInfo, CreateCommandResult>>
    {
        private readonly ILogger<HandleCreateGenericMeasureInfoCommand> _logger;
        private readonly IUnitOfWorkFactory _uowFactory;
        private readonly IExpressionBuilder _expressionBuilder;
        
        public HandleCreateGenericMeasureInfoCommand(ILogger<HandleCreateGenericMeasureInfoCommand> logger, IUnitOfWorkFactory uowFactory, IExpressionBuilder expressionBuilder)
        {
            _logger = logger;
            _uowFactory = uowFactory;
            _expressionBuilder = expressionBuilder;
        }

        public async Task<Option<GenericMeasureInfo, CreateCommandResult>> Handle(CreateGenericMeasureInfoCommand request, CancellationToken cancellationToken)
        {
            using IUnitOfWork uow = _uowFactory.NewUnitOfWork();
            CreateGenericMeasureInfo cmdData = request.Data;
            Option<MeasureForm> maybeForm = await uow.Repository<MeasureForm>()
                                                     .SingleOrDefaultAsync(form => form.Id == cmdData.FormId, cancellationToken)
                                                     .ConfigureAwait(false);
            Option<GenericMeasureInfo, CreateCommandResult> maybeGenericMeasureInfo = default;

            await maybeForm.Match(
                some: async form =>
                {
                    Option<Patient> mayBePatient = await uow.Repository<Patient>()
                                                            .SingleOrDefaultAsync(x => x.Id == cmdData.PatientId, cancellationToken)
                                                            .ConfigureAwait(false);

                    maybeGenericMeasureInfo = await mayBePatient.Match(
                        some: async patient =>
                        {
                            Guid measureId = Guid.NewGuid();
                            patient.AddMeasure(form.Id, measureId, cmdData.DateOfMeasure, cmdData.Values);

                            await uow.SaveChangesAsync(cancellationToken)
                                     .ConfigureAwait(false);

                            GenericMeasure measure = patient.Measures.Single(x => x.Id == measureId);

                            IDictionary<string, object> values = new Dictionary<string, object>();

                            foreach (JsonProperty element in measure.Data.RootElement.EnumerateObject())
                            {
                                values.Add(element.Name, element.Value.ToString());
                            }

                            return Option.Some<GenericMeasureInfo, CreateCommandResult>(
                                new GenericMeasureInfo
                                {
                                    FormId = form.Id,
                                    Id = measure.Id,
                                    CreatedDate = measure.CreatedDate,
                                    DateOfMeasure = measure.DateOfMeasure,
                                    PatientId = measure.PatientId,
                                    UpdatedDate = measure.UpdatedDate,
                                    Data = values
                                });
                        },
                        none: () => Task.FromResult(Option.None<GenericMeasureInfo, CreateCommandResult>(CreateCommandResult.Failed_NotFound))
                        ).ConfigureAwait(false);
                },
                none: () => Task.FromResult(Option.None<GenericMeasureInfo, CreateCommandResult>(CreateCommandResult.Failed_NotFound))
                )
                .ConfigureAwait(false);

            return maybeGenericMeasureInfo;
        }
    }
}
