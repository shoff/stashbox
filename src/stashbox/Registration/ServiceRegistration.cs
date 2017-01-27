﻿using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.MetaInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Stashbox.Registration
{
    /// <summary>
    /// Represents a service registration.
    /// </summary>
    public class ServiceRegistration : IServiceRegistration
    {
        private readonly IContainerContext containerContext;
        private readonly ILifetime lifetimeManager;
        private readonly IObjectBuilder objectBuilder;
        private readonly HashSet<Type> attributeConditions;
        private readonly Type targetTypeCondition;
        private readonly Func<TypeInformation, bool> resolutionCondition;
        private readonly MetaInfoCache metaInfoCache;

        /// <summary>
        /// Constructs a <see cref="ServiceRegistration"/>
        /// </summary>
        /// <param name="registrationName">The registration name.</param>
        /// <param name="containerContext">The container context.</param>
        /// <param name="lifetimeManager">The lifetime manager.</param>
        /// <param name="objectBuilder">THe object builder.</param>
        /// <param name="metaInfoCache">THe meta info cache.</param>
        /// <param name="attributeConditions">The attribute conditions.</param>
        /// <param name="targetTypeCondition">The target type condition.</param>
        /// <param name="resolutionCondition">The resolution condition.</param>
        public ServiceRegistration(string registrationName, IContainerContext containerContext, ILifetime lifetimeManager,
            IObjectBuilder objectBuilder, MetaInfoCache metaInfoCache, HashSet<Type> attributeConditions = null, Type targetTypeCondition = null,
            Func<TypeInformation, bool> resolutionCondition = null)
        {
            this.RegistrationName = registrationName;
            this.containerContext = containerContext;
            this.lifetimeManager = lifetimeManager;
            this.objectBuilder = objectBuilder;
            this.attributeConditions = attributeConditions;
            this.targetTypeCondition = targetTypeCondition;
            this.resolutionCondition = resolutionCondition;
            this.metaInfoCache = metaInfoCache;
            this.RegistrationNumber = this.containerContext.ReserveRegistrationNumber();
        }

        /// <inheritdoc />
        public int RegistrationNumber { get; }

        /// <inheritdoc />
        public string RegistrationName { get; }

        /// <inheritdoc />
        public object GetInstance(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            return this.lifetimeManager.GetInstance(this.containerContext, this.objectBuilder, resolutionInfo, resolveType);
        }

        /// <inheritdoc />
        public bool IsUsableForCurrentContext(TypeInformation typeInfo)
        {
            return (this.targetTypeCondition == null && this.resolutionCondition == null && (this.attributeConditions == null || !this.attributeConditions.Any()) && !this.metaInfoCache.HasGenericTypeConstraints) ||
                   (this.targetTypeCondition != null && typeInfo.ParentType != null && this.targetTypeCondition == typeInfo.ParentType) ||
                   (this.attributeConditions != null && typeInfo.CustomAttributes != null &&
                    this.attributeConditions.Intersect(typeInfo.CustomAttributes.Select(attribute => attribute.GetType())).Any()) ||
                   (this.resolutionCondition != null && this.resolutionCondition(typeInfo)) ||
                   (typeInfo.Type.IsConstructedGenericType && this.metaInfoCache.ValidateGenericContraints(typeInfo));
        }

        /// <inheritdoc />
        public bool HasCondition => this.targetTypeCondition != null || this.resolutionCondition != null ||
            (this.attributeConditions != null && this.attributeConditions.Any()) ||
            this.metaInfoCache.HasGenericTypeConstraints;

        /// <inheritdoc />
        public void ServiceUpdated(RegistrationInfo registrationInfo)
        {
            this.objectBuilder.ServiceUpdated(registrationInfo);
        }

        /// <inheritdoc />
        public void CleanUp()
        {
            this.objectBuilder.CleanUp();
            this.lifetimeManager.CleanUp();
        }

        /// <inheritdoc />
        public Expression GetExpression(ResolutionInfo resolutionInfo, Expression resolutionInfoExpression, TypeInformation resolveType)
        {
            return this.lifetimeManager.GetExpression(this.containerContext, this.objectBuilder, resolutionInfo, resolutionInfoExpression, resolveType);
        }
    }
}
