﻿
using AutoMapper.QueryableExtensions;
using FluentAssertions;
using Measures.DTO;
using Measures.Mapping;
using MedEasy.CQRS.Core.Handlers;
using MedEasy.CQRS.Core.Queries;
using MedEasy.DAL.Interfaces;
using MedEasy.DAL.Repositories;
using MedEasy.Data;
using MedEasy.DTO.Search;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using static MedEasy.Data.DataFilterOperator;
using static Moq.MockBehavior;

namespace Measures.CQRS.UnitTests.Handlers
{
    public class HandleSearchQueryTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;
        private Mock<IUnitOfWorkFactory> _uowFactoryMock;
        private HandleSearchQuery _iHandleSearchQuery;
        private Mock<IExpressionBuilder> _expressionBuilderMock;
        private Mock<ILogger<HandleSearchQuery>> _loggerMock;

        public HandleSearchQueryTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;

            _uowFactoryMock = new Mock<IUnitOfWorkFactory>(Strict);
            _uowFactoryMock.Setup(mock => mock.New().Dispose());

            _expressionBuilderMock = new Mock<IExpressionBuilder>(Strict);
            _loggerMock = new Mock<ILogger<HandleSearchQuery>>(Strict);
            _loggerMock.Setup(mock => mock.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));


            _iHandleSearchQuery = new HandleSearchQuery(_uowFactoryMock.Object, _expressionBuilderMock.Object, _loggerMock.Object);
        }

        public void Dispose()
        {
            _outputHelper = null;
            _uowFactoryMock = null;
            _expressionBuilderMock = null;

            _iHandleSearchQuery = null;
        }


        public static IEnumerable<object[]> SearchPatientCases
        {
            get
            {
                yield return new object[]
                {
                    Enumerable.Empty<Objects.Patient>(),
                    new SearchQueryInfo<PatientInfo>
                    {
                        Filter = new DataFilter(field : nameof(PatientInfo.Firstname), @operator : EqualTo, value : "Bruce"),
                        Page = 1,
                        PageSize = 3
                    },
                    ((Expression<Func<Page<PatientInfo>, bool>>)(x => x != null &&
                        !x.Entries.Any() &&
                        x.Count == 0 &&
                        x.Size == 3))
                };


                {
                    Guid patientId = Guid.NewGuid();
                    yield return new object[]
                   {
                        new []
                        {
                            new Objects.Patient {Id = 1, Firstname = "bruce", Lastname = "wayne" },
                            new Objects.Patient {Id = 2, Firstname = "dick", Lastname = "grayson" },
                            new Objects.Patient {Id = 3, Firstname = "damian", Lastname = "wayne", UUID = patientId },

                        },
                        new SearchQueryInfo<PatientInfo>
                        {
                            Filter = new DataFilter(field : nameof(PatientInfo.Lastname), @operator : Contains, value : "y"),
                            Page = 3,
                            PageSize = 1
                        },
                        ((Expression<Func<Page<PatientInfo>, bool>>)(x => x != null &&
                            x.Entries.Count() == 1 &&
                            x.Entries.ElementAt(0).Id == patientId &&
                            x.Count == 3 &&
                            x.Size == 1))
                       };
                }
            }
        }


        [Theory]
        [MemberData(nameof(SearchPatientCases))]
        public async Task SearchPatientInfos(IEnumerable<Objects.Patient> patients, SearchQueryInfo<PatientInfo> search, Expression<Func<Page<PatientInfo>, bool>> resultExpectation)
        {
            _outputHelper.WriteLine($"search : {search}");

            // Arrange
            _expressionBuilderMock.Setup(mock => mock.GetMapExpression(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<IDictionary<string, object>>(), It.IsAny<MemberInfo[]>()))
               .Returns((Type sourceType, Type destinationType, IDictionary<string, object> parameters, MemberInfo[] membersToExpand) => AutoMapperConfig.Build().ExpressionBuilder.GetMapExpression(sourceType, destinationType, parameters, membersToExpand));

            _uowFactoryMock.Setup(mock => mock.New().Repository<Objects.Patient>().WhereAsync(It.IsAny<Expression<Func<Objects.Patient, PatientInfo>>>(),
                It.IsAny<Expression<Func<PatientInfo, bool>>>(), It.IsAny<IEnumerable<OrderClause<PatientInfo>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns((Expression<Func<Objects.Patient, PatientInfo>> selector, Expression<Func<PatientInfo, bool>> filter, IEnumerable<OrderClause<PatientInfo>> sorts, int pageSize, int page, CancellationToken cancellationToken)
                    => 
                    {

                        IEnumerable<PatientInfo> results = patients.Select(selector.Compile())
                            .Where(filter.Compile())
                            .Skip(pageSize * (page - 1))
                            .Take(pageSize);

                        int total = patients.Select(selector.Compile())
                            .Count(filter.Compile());

                        return new ValueTask<Page<PatientInfo>>(new Page<PatientInfo>(results, total, pageSize));
                    });

            // Act
            SearchQuery<PatientInfo> searchQuery = new SearchQuery<PatientInfo>(search);
            Page<PatientInfo> pageOfResult = await _iHandleSearchQuery.Search<Objects.Patient, PatientInfo>(searchQuery);

            // Assert
            pageOfResult.Should()
                .Match(resultExpectation);
        }
    }
}
