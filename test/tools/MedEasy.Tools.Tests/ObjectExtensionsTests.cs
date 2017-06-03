﻿using System.Collections.Generic;
using Xunit;
using System;
using FluentAssertions;
using System.Linq.Expressions;
using Xunit.Abstractions;
using static Newtonsoft.Json.JsonConvert;
using MedEasy.Data;
using static MedEasy.Data.DataFilterOperator;

namespace MedEasy.Tools.Tests
{
    /// <summary>
    /// Extensions methods for <see cref="Object"/> type.
    /// </summary>
    public class ObjectExtensionsTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        /// <summary>
        /// Builds a new <see cref="ObjectExtensionsTests"/> instance.
        /// </summary
        /// <param name="outputHelper"></param>
        public ObjectExtensionsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }


        public void Dispose()
        {
            _outputHelper = null;
        }


        public static IEnumerable<object[]> ToQueryStringCases
        {
            get
            {
                yield return new object[]
                {
                    null,
                    string.Empty
                };

                yield return new object[]
                {
                    new {},
                    string.Empty,
                };
                yield return new object[]
                {
                    new { limit = 1 },
                    "limit=1"
                };

                yield return new object[]
                 {
                    new { limit = 1, offset=3 },
                    "limit=1&offset=3"
                 };

                yield return new object[]
                {
                    new {limit = new [] {0, 1, 2, 3}},
                    "limit[0]=0&limit[1]=1&limit[2]=2&limit[3]=3"
                };

                yield return new object[]
                {
                    new {
                        search = new
                        {
                            filter = new { field = "Firstname", op = "eq", value = "Bruce" }
                        }
                    },
                    "search[filter][field]=Firstname&search[filter][op]=eq&search[filter][value]=Bruce"
                };

                yield return new object[]
                {
                    new {
                        search = new
                        {
                            filter = new { field = "Firstname", op = "eq", value = "Bru&ce" }
                        }
                    },
                    "search[filter][field]=Firstname&search[filter][op]=eq&search[filter][value]=Bru%26ce"
                };
            }
        }

        [Theory]
        [MemberData(nameof(ToQueryStringCases))]
        public void ToQueryString(object input, string expectedString)
        {
            _outputHelper.WriteLine($"input : {input}");

            ObjectExtensions.ToQueryString(input)?.Should().Be(expectedString);
        }


        public static IEnumerable<object> ParseAnonymousObjectCases
        {
            get
            {

                yield return new object[]
                {
                    new {limit = new [] {0, 2, 4}},
                    ((Expression<Func<IDictionary<string, object>, bool>>)(x =>
                        x != null
                        && x.Keys.Count == 1
                        && x.ContainsKey("limit")
                        && x["limit"] is IDictionary<string, object>
                        && ((IDictionary<string, object>) x["limit"]).Keys.Count == 3
                        && ((IDictionary<string, object>) x["limit"]).ContainsKey("0")
                        && Equals(((IDictionary<string, object>) x["limit"])["0"], 0)
                        && ((IDictionary<string, object>) x["limit"]).ContainsKey("1")
                        && Equals(((IDictionary<string, object>) x["limit"])["1"], 2)
                        && ((IDictionary<string, object>) x["limit"]).ContainsKey("2")
                        && Equals(((IDictionary<string, object>) x["limit"])["2"], 4)
                    ))
                };

                yield return new object[]
                {
                    new { propName = "value" },
                    ((Expression<Func<IDictionary<string, object>, bool>>)(x => 
                        x != null 
                        && x.Keys.Count == 1
                        && x.ContainsKey("propName") && Equals(x["propName"], "value")))
                };
                yield return new object[]
                {
                    new {
                        prop1 = "value",
                        prop2 = new
                        {
                            subProp1 = "subPropValue"
                        }
                    },
                    ((Expression<Func<IDictionary<string, object>, bool>>)(x =>
                        x != null
                        && x.Keys.Count == 2
                        && x.ContainsKey("prop1") && Equals(x["prop1"], "value")

                        && x.ContainsKey("prop2")
                        && x["prop2"] is IDictionary<string, object>
                        && ((IDictionary<string, object>) x["prop2"]).Keys.Count == 1
                        && ((IDictionary<string, object>) x["prop2"]).ContainsKey("subProp1")
                        && Equals(((IDictionary<string, object>) x["prop2"])["subProp1"], "subPropValue")))
                };
                yield return new object[]
                {
                    new
                    {
                        Page = 1,
                        PageSize = 30,
                        Filter = new DataFilter
                        {
                            Field = "Firstname",
                            Operator = EqualTo,
                            Value = "Bruce"
                        }
                    },
                    ((Expression<Func<IDictionary<string, object>, bool>>)(x =>
                        x != null
                        && x.Keys.Count == 3
                        && x.ContainsKey("Page") && Equals(x["Page"], 1)

                        && x.ContainsKey("Filter")
                        && x["Filter"] is IDictionary<string, object>
                        && ((IDictionary<string, object>) x["Filter"]).Keys.Count == 3
                        && ((IDictionary<string, object>) x["Filter"]).ContainsKey(nameof(DataFilter.Field))
                        && Equals(((IDictionary<string, object>) x["Filter"])[nameof(DataFilter.Field)], "Firstname")

                        && ((IDictionary<string, object>) x["Filter"]).ContainsKey(nameof(DataFilter.Operator))
                        && Equals(((IDictionary<string, object>) x["Filter"])[nameof(DataFilter.Operator)], EqualTo)

                        && ((IDictionary<string, object>) x["Filter"]).ContainsKey(nameof(DataFilter.Value))
                        && Equals(((IDictionary<string, object>) x["Filter"])[nameof(DataFilter.Value)], "Bruce")
                    ))

                };

                {
                    DateTimeOffset? from = null;
                    DateTimeOffset? to = 1.February(2001);

                    yield return new object[]
                    {
                        new
                        {
                            from,
                            to
                        },
                        ((Expression<Func<IDictionary<string, object>, bool>>)(x =>
                            x != null
                            && x.Keys.Count == 1
                            && x.ContainsKey("to") && Equals(x["to"], to)
                        ))
                    };
                }
            }
        }
        

        [Theory]
        [MemberData(nameof(ParseAnonymousObjectCases))]
        public void ParseAnonymousObject(object input, Expression<Func<IDictionary<string, object>, bool>> resultExpectation)
        {
            _outputHelper.WriteLine($"input : {input}");

            //Act
            IDictionary<string, object> result = input?.ParseAnonymousObject();

            // Assert
            _outputHelper.WriteLine($"output : {SerializeObject(result)}");
            result.Should().Match(resultExpectation);
        }

    } 
}