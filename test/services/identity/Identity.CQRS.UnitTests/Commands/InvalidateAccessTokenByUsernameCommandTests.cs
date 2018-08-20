﻿using FluentAssertions;
using Identity.CQRS.Commands;
using MedEasy.CQRS.Core.Commands;
using MedEasy.CQRS.Core.Commands.Results;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Categories;

namespace Identity.CQRS.UnitTests.Commands
{
    [UnitTest]
    [Feature("Authentication")]
    [Feature("JWT")]
    public class InvalidateAccessTokenByUsernameCommandTests : IDisposable
    {
        private ITestOutputHelper _outputHelper;

        public InvalidateAccessTokenByUsernameCommandTests(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Dispose()
        {
            _outputHelper = null;
        }

        [Fact]
        public void IsCommand() => typeof(InvalidateAccessTokenByUsernameCommand).Should()
            .Implement<ICommand<Guid, string, InvalidateAccessCommandResult>>();


        [Fact]
        public void Ctor_Is_Valid()
        {
            InvalidateAccessTokenByUsernameCommand instance = new InvalidateAccessTokenByUsernameCommand("thejoker");
            // Assert
            instance.Id.Should()
                .NotBeEmpty();

        }

        [Theory]
        [InlineData(null, "username is null")]
        [InlineData("", "username is empty")]
        [InlineData("   ", "username is whitespace")]
        public void Ctor_Throws_ArgumentNullException(string username, string reason)
        {
            // Act
            Action action = () => new InvalidateAccessTokenByUsernameCommand(username);

            // Assert
            action.Should()
                .Throw<ArgumentException>(reason).Which
                .ParamName.Should()
                .NotBeNullOrWhiteSpace();

        }
    }
}
