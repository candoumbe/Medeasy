﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using MedEasy.DTO;
using Xunit;
using System.Threading.Tasks;

namespace MedEasy.Validators.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class PatientInfoValidatorTests
    {
        public IValidate<PatientInfo> Validator { get; set; }
        public PatientInfoValidatorTests()
        {
            Validator = new PatientInfoValidator();
        }

        [Theory]
        [MemberData(nameof(ValidateTestCases))]
        public async Task ValidateTest(PatientInfo info,
            Expression<Func<IEnumerable<ErrorInfo>, bool>> errorMatcher,
            string because = "")
            => (await Task.WhenAll(Validator.Validate(info))).Should().Match(errorMatcher, because);

        public static IEnumerable<object[]> ValidateTestCases
        {
            get
            {
                yield return new object[]
                {
                    null,
                    ((Expression<Func<IEnumerable<ErrorInfo>, bool>>)
                        (errors => errors.Once(errorItem => "".Equals(errorItem.Key) && errorItem.Severity == ErrorLevel.Error))),
                    $"because {nameof(PatientInfo)} is null"
                };

                yield return new object[]
                {
                    new PatientInfo(),
                    ((Expression<Func<IEnumerable<ErrorInfo>, bool>>)
                        (errors => errors.Once(errorItem => string.Empty.Equals(errorItem.Key) && errorItem.Severity == ErrorLevel.Error))),
                    $"because {nameof(PatientInfo)}'s is null"
                };

                yield return new object[]
                {
                    new PatientInfo() { Firstname = "Bruce" },
                    ((Expression<Func<IEnumerable<ErrorInfo>, bool>>)
                        (errors => errors.Once(errorItem => nameof(PatientInfo.Lastname).Equals(errorItem.Key) && errorItem.Severity == ErrorLevel.Error))),
                    $"because {nameof(PatientInfo.Firstname)} is set and {nameof(PatientInfo.Lastname)} is not"
                };

                yield return new object[]
                {
                    new PatientInfo() { Lastname = "Wayne" },
                    ((Expression<Func<IEnumerable<ErrorInfo>, bool>>)
                        (errors => errors.Once(errorItem => nameof(PatientInfo.Firstname).Equals(errorItem.Key) && errorItem.Severity == ErrorLevel.Warning))),
                    $"because {nameof(PatientInfo.Lastname)} is set and {nameof(PatientInfo.Firstname)} is not"
                };
            }
        }
    }
}