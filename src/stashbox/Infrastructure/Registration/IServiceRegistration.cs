﻿using System.Linq.Expressions;
using Stashbox.Entity;
using System;
using Stashbox.MetaInfo;
using Stashbox.Registration;

namespace Stashbox.Infrastructure.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public interface IServiceRegistration
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get; }

        /// <summary>
        /// The implementation type.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// The meta info.
        /// </summary>
        MetaInformation MetaInformation { get; }

        /// <summary>
        /// The registration context.
        /// </summary>
        RegistrationContextData RegistrationContext { get; }

        /// <summary>
        /// The registration number.
        /// </summary>
        int RegistrationNumber { get; }

        /// <summary>
        /// True if the registration contains any condition, otherwise false.
        /// </summary>
        bool HasCondition { get; }

        /// <summary>
        /// True if the registration is a decorator.
        /// </summary>
        bool IsDecorator { get; }

        /// <summary>
        /// True if the registration contains a disposable service which should be tracked.
        /// </summary>
        bool ShouldHandleDisposal { get; }

        /// <summary>
        /// Creates an expression for creating the resolved instance.
        /// </summary>
        /// <param name="resolutionInfo">The info about the current resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The expression.</returns>
        Expression GetExpression(ResolutionInfo resolutionInfo, Type resolveType);

        /// <summary>
        /// Checks whether the registration can be used for a current resolution.
        /// </summary>
        /// <param name="typeInfo">The type information.</param>
        /// <returns>True if the registration can be used for the current resolution, otherwise false.</returns>
        bool IsUsableForCurrentContext(TypeInformation typeInfo);

        /// <summary>
        /// Validates that the given type's generic argument fullfills the generic constraint or not 
        /// </summary>
        /// <param name="type">The type information.</param>
        /// <returns>True if the argument is valid.</returns>
        bool ValidateGenericContraints(Type type);
    }
}
