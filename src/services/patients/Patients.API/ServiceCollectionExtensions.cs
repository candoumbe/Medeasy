﻿using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MedEasy.Abstractions;
using MedEasy.Core.Filters;
using MedEasy.DAL.EFStore;
using MedEasy.DAL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Patients.Context;
using Patients.Mapping;
using Patients.Validators.Features.Patients.DTO;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Newtonsoft.Json.DateFormatHandling;
using static Newtonsoft.Json.DateTimeZoneHandling;

namespace Patients.API
{
    /// <summary>
    /// Provide extension method used to configure services collection
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure service for MVC
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="env"></param>
        public static IServiceCollection AddCustomMvc(this IServiceCollection services, IConfiguration configuration, IHostEnvironment env)
        {
            services
                .AddControllers(config =>
                {
                    config.Filters.Add<FormatFilterAttribute>();
                    config.Filters.Add<ValidateModelActionFilter>();
                    config.Filters.Add<AddCountHeadersFilterAttribute>();

                    config.Filters.Add<HandleErrorAttribute>();

                    AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .Build();

                    config.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddFluentValidation(options =>
                {
                    options.LocalizationEnabled = true;
                    options.RegisterValidatorsFromAssemblyContaining<CreatePatientInfoValidator>();
                })
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatHandling = IsoDateFormat;
                    options.SerializerSettings.DateTimeZoneHandling = Utc;
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.Indented;
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .AddXmlSerializerFormatters();

                services.AddCors(options =>
                {
                    options.AddPolicy("AllowAnyOrigin", builder =>
                        builder.AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowAnyOrigin()
                    );
                });
                services.AddOptions();
                services.Configure<PatientsApiOptions>((options) =>
                {
                    options.DefaultPageSize = configuration.GetValue($"APIOptions:{nameof(PatientsApiOptions.DefaultPageSize)}", 30);
                    options.MaxPageSize = configuration.GetValue($"APIOptions:{nameof(PatientsApiOptions.DefaultPageSize)}", 100);
                });

                services.Configure<ApiBehaviorOptions>(options =>
                {
                    options.InvalidModelStateResponseFactory = (context) =>
                    {
                        IDictionary<string, IEnumerable<string>> errors = context.ModelState
                            .Where(element => !string.IsNullOrWhiteSpace(element.Key))
                            .ToDictionary(item => item.Key, item => item.Value.Errors.Select(x => x.ErrorMessage).Distinct());
                        ValidationProblemDetails validationProblem = new ValidationProblemDetails
                        {
                            Title = "Validation failed",
                            Detail = $"{errors.Count} validation errors",
                            Status = context.HttpContext.Request.Method == HttpMethods.Get || context.HttpContext.Request.Method == HttpMethods.Head
                                ? Status400BadRequest
                            : Status422UnprocessableEntity
                    };
                    foreach ((string key, IEnumerable<string> details) in errors)
                    {
                        validationProblem.Errors.Add(key, details.ToArray());
                    }

                    return new BadRequestObjectResult(validationProblem);
                };
            });
            services.AddRouting(options =>
            {
                options.AppendTrailingSlash = false;
                options.LowercaseUrls = true;
            });

            return services;
        }

        /// <summary>
        /// Adds required dependencies to access APÏ datastores
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDataStores(this IServiceCollection services)
        {
            static DbContextOptionsBuilder<PatientsContext> BuildDbContextOptions(IServiceProvider serviceProvider)
            {
                IHostEnvironment hostingEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
                DbContextOptionsBuilder<PatientsContext> builder = new DbContextOptionsBuilder<PatientsContext>();
                if (hostingEnvironment.IsEnvironment("IntegrationTest"))
                {
                    builder.UseInMemoryDatabase($"{Guid.NewGuid()}");
                }
                else
                {
                    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
                    builder.UseNpgsql(
                        configuration.GetConnectionString("patients-db"),
                        options => options.EnableRetryOnFailure(5)
                            .MigrationsAssembly(typeof(PatientsContext).Assembly.FullName)
                    );
                }
                builder.UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
                builder.ConfigureWarnings(options =>
                {
                    options.Default(WarningBehavior.Log);
                });
                return builder;
            }

            services.AddTransient(serviceProvider =>
            {
                DbContextOptionsBuilder<PatientsContext> optionsBuilder = BuildDbContextOptions(serviceProvider);

                return new PatientsContext(optionsBuilder.Options);
            });

            services.AddSingleton<IUnitOfWorkFactory, EFUnitOfWorkFactory<PatientsContext>>(serviceProvider =>
            {
                IHostEnvironment hostingEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();
                DbContextOptionsBuilder<PatientsContext> builder = BuildDbContextOptions(serviceProvider);

                return new EFUnitOfWorkFactory<PatientsContext>(builder.Options, options => new PatientsContext(options));
            });

            return services;
        }

        /// <summary>
        /// Configure dependency injections
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton(AutoMapperConfig.Build().CreateMapper());
            services.AddSingleton(serviceProvider => serviceProvider.GetRequiredService<IMapper>().ConfigurationProvider.ExpressionBuilder);

            services.AddHttpContextAccessor();
            services.AddScoped(serviceProvider =>
            {
                HttpContext http = serviceProvider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                return http.Features.Get<IApiVersioningFeature>()?.RequestedApiVersion;
            });

            services.AddSingleton<IDateTimeService, DateTimeService>();

            return services;
        }

        /// <summary>
        /// Adds custom authentication
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddCustomAuthenticationAndAuthorization(this IServiceCollection services, IConfiguration configuration) =>
            services
                    .AddAuthorization()
                    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            RequireExpirationTime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = configuration[$"Authentication:{nameof(JwtOptions)}:{nameof(JwtOptions.Issuer)}"],
                            ValidAudience = configuration[$"Authentication:{nameof(JwtOptions)}:{nameof(JwtOptions.Audience)}"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration[$"Authentication:{nameof(JwtOptions)}:{nameof(JwtOptions.Key)}"]))
                        };
                        options.Validate();
                    });

        /// <summary>
        /// Adds Swagger middlewares
        /// </summary>
        /// <param name="services"></param>
        /// <param name="environment"></param>
        /// <param name="configuration"></param>
        public static IServiceCollection AddCustomizedSwagger(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
        {
            (string applicationName, string applicationBasePath) = (System.Reflection.Assembly.GetEntryAssembly().GetName().Name, AppDomain.CurrentDomain.BaseDirectory);

            services.AddSwaggerGen(config =>
            {
                string contactUrl = configuration.GetValue("Swagger:Contact:Url", string.Empty);
                config.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = environment.ApplicationName,
                    Description = "REST API for Patients management",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Email = configuration.GetValue("Swagger:Contact:Email", string.Empty),
                        Name = configuration.GetValue("Swagger:Contact:Name", string.Empty),
                        Url = string.IsNullOrWhiteSpace(contactUrl)
                            ? null
                            : new Uri(contactUrl)
                    }
                });

                config.IgnoreObsoleteActions();
                config.IgnoreObsoleteProperties();
                string documentationPath = Path.Combine(applicationBasePath, $"{applicationName}.xml");
                if (File.Exists(documentationPath))
                {
                    config.IncludeXmlComments(documentationPath);
                }

                OpenApiSecurityScheme bearerSecurityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Token to access the API",
                    Type = SecuritySchemeType.Http
                };
                config.AddSecurityDefinition("Bearer", bearerSecurityScheme);
                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [bearerSecurityScheme] = new List<string>()
                });
            });

            return services;
        }
    }
}
