﻿using System;
using System.Collections.Generic;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Registration;

namespace Stashbox.Configuration
{
    /// <summary>
    /// Represents a container configurator.
    /// </summary>
    public class ContainerConfigurator : IContainerConfigurator
    {
        /// <summary>
        /// The container configuration.
        /// </summary>
        public ContainerConfiguration ContainerConfiguration { get; }

        internal ContainerConfigurator()
        {
            this.ContainerConfiguration = ContainerConfiguration.DefaultContainerConfiguration();
        }

        /// <inheritdoc />
        public IContainerConfigurator WithDisposableTransientTracking()
        {
            this.ContainerConfiguration.TrackTransientsForDisposalEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithCircularDependencyTracking()
        {
            this.ContainerConfiguration.CircularDependencyTrackingEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithCircularDependencyWithLazy()
        {
            this.ContainerConfiguration.CircularDependenciesWithLazyEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithOptionalAndDefaultValueInjection()
        {
            this.ContainerConfiguration.OptionalAndDefaultValueInjectionEnabled = true;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithUnknownTypeResolution(Action<IFluentServiceRegistrator> configurator = null)
        {
            this.ContainerConfiguration.UnknownTypeResolutionEnabled = true;
            this.ContainerConfiguration.UnknownTypeConfigurator = configurator;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithMemberInjectionWithoutAnnotation(Rules.AutoMemberInjection rule = Rules.AutoMemberInjection.PropertiesWithPublicSetter)
        {
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationEnabled = true;
            this.ContainerConfiguration.MemberInjectionWithoutAnnotationRule = rule;
            return this;
        }

        /// <inheritdoc />
        public IContainerConfigurator WithConstructorSelectionRule(Func<IEnumerable<ConstructorInformation>, IEnumerable<ConstructorInformation>> selectionRule)
        {
            this.ContainerConfiguration.ConstructorSelectionRule = selectionRule;
            return this;
        }
    }
}
