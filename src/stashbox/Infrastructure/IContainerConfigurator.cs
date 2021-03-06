﻿using System;
using System.Collections.Generic;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a container configurator.
    /// </summary>
    public interface IContainerConfigurator
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        ContainerConfiguration ContainerConfiguration { get; }

        /// <summary>
        /// Enables the tracking of disposable transient objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithDisposableTransientTracking();

        /// <summary>
        /// Enables the circular dependency tracking.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithCircularDependencyTracking();

        /// <summary>
        /// Allows circular dependencies through Lazy objects.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithCircularDependencyWithLazy();

        /// <summary>
        /// Enables the optional and default value injection.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithOptionalAndDefaultValueInjection();

        /// <summary>
        /// Enables the unknown type resolution.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithUnknownTypeResolution(Action<IFluentServiceRegistrator> configurator = null);

        /// <summary>
        /// Enables the member injection without annotation.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter);

        /// <summary>
        /// Sets the constructor selection rule.
        /// </summary>
        /// <returns>The container configurator.</returns>
        IContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> selectionRule);
    }
}
