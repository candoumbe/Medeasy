﻿using System.Collections.Generic;
using Xunit;
using System;
using FluentAssertions;
using Xunit.Abstractions;
using MedEasy.Tools.Extensions;

namespace MedEasy.Tools.Tests
{

    public class StringExtensionsTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public StringExtensionsTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public void Dispose()
        {
            _outputHelper = null;
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("bruce", "Bruce")]
        [InlineData("bruce wayne", "Bruce Wayne")]
        [InlineData("cyrille-alexandre", "Cyrille-Alexandre")]
        public void ToTitleCase(string input, string expectedString)
            => input?.ToTitleCase()?.Should().Be(expectedString);


        [Theory]
        [InlineData("bruce", "Bruce", true, true)]
        [InlineData("bruce", "Bruce", false, false)]
        [InlineData("bruce", "br*ce", true, true)]
        [InlineData("bruce", "br?ce", true, true)]
        [InlineData("bruce", "?r?ce", true, true)]
        [InlineData("Bruce", "?r?ce", false, true)]
        [InlineData("Bruce", "Carl", false, false)]
        [InlineData("Bruce", "Carl", true, false)]
        [InlineData("Bruce", "B*e", false, true)]
        [InlineData("Bruce", "B?e", false, false)]
        [InlineData("Bruce", "B?e", true, false)]
        [InlineData("Bruce", "*,*", true, false)]
        [InlineData("Bruce", "*,*", false, false)]
        [InlineData("Bruce,Dick", "*,*", true, true)]
        [InlineData("Bruce,Dick", "*,*", false, true)]
        public void Like(string input, string pattern, bool ignoreCase, bool expectedResult)
        {
            _outputHelper.WriteLine($"input : '{input}'");
            _outputHelper.WriteLine($"pattern : '{pattern}'");
            _outputHelper.WriteLine($"Ignore case : '{ignoreCase}'");


            // Act
            input?.Like(pattern, ignoreCase).Should().Be(expectedResult);
        }

        [Fact]
        public void ToLowerKebabCase_Throws_ArgumentNullException()
        {
            Action act = () => StringExtensions.ToLowerKebabCase(null);


            act.ShouldThrow<ArgumentNullException>().Which
                .ParamName.ShouldBeEquivalentTo("input");
        }




        [Theory]
        [InlineData("firstname", "firstname")]
        [InlineData("firstName", "first-name")]
        [InlineData("FirstName", "first-name")]
        public void ToLowerKebabCase(string input, string expectedOutput)
        {
            _outputHelper.WriteLine($"input : '{input}'");
            input.ToLowerKebabCase().Should().Be(expectedOutput);
        }


        [Fact]
        public void Decode()
        {
            Guid guid = Guid.NewGuid();
            guid.Encode().Decode().Should().Be(guid);
        }
    }



}