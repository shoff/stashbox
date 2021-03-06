﻿using System;
using Stashbox.Entity;
using System.Linq.Expressions;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents an object builder.
    /// </summary>
    public interface IObjectBuilder
    {
        /// <summary>
        /// Creates the expression for creating an instance of a registered service.
        /// </summary>
        /// <param name="serviceRegistration">The service registration.</param>
        /// <param name="resolutionInfo">The info about the actual resolution.</param>
        /// <param name="resolveType">The requested type.</param>
        /// <returns>The created object.</returns>
        Expression GetExpression(IServiceRegistration serviceRegistration, ResolutionInfo resolutionInfo, Type resolveType);

        /// <summary>
        /// Indicates that the object builder is handling the disposal of the produced instance or not.
        /// </summary>
        bool HandlesObjectDisposal { get; }

        /// <summary>
        /// Produces an <see cref="IObjectBuilder"/>.
        /// </summary>
        /// <returns>The <see cref="IObjectBuilder"/> instance.</returns>
        IObjectBuilder Produce();
    }
}
