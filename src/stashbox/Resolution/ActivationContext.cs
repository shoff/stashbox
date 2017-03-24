﻿using System;
using System.Linq.Expressions;
using Stashbox.Entity;
using Stashbox.Exceptions;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.Resolution;
using System.Linq;

namespace Stashbox.Resolution
{
    internal class ActivationContext : IActivationContext
    {
        private readonly IContainerContext containerContext;
        private readonly IResolverSelector resolverSelector;

        public ActivationContext(IContainerContext containerContext, IResolverSelector resolverSelector)
        {
            this.containerContext = containerContext;
            this.resolverSelector = resolverSelector;
        }

        public object Activate(Type type, IResolutionScope resolutionScope, string name = null, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetDelegateCacheOrDefault(type, name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : this.Activate(ResolutionInfo.New(resolutionScope, nullResultAllowed), type, name);
        }

        public Delegate ActivateFactory(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, string name = null, bool nullResultAllowed = false)
        {
            var cachedFactory = this.containerContext.DelegateRepository.GetFactoryDelegateCacheOrDefault(type, parameterTypes, name);
            return cachedFactory != null ? cachedFactory(resolutionScope) : ActivateFactoryDelegate(type, parameterTypes, resolutionScope, name, nullResultAllowed);
        }

        public object Activate(ResolutionInfo resolutionInfo, Type type, string name = null)
        {
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(type, name);
            if (registration != null)
            {
                var ragistrationFactory = registration.GetExpression(resolutionInfo, type)?.CompileDelegate(Constants.ScopeExpression);
                if (ragistrationFactory == null)
                    if (resolutionInfo.NullResultAllowed)
                        return null;
                    else
                        throw new ResolutionFailedException(type.FullName);

                this.containerContext.DelegateRepository.AddServiceDelegate(type, ragistrationFactory, name);
                return ragistrationFactory(resolutionInfo.ResolutionScope);
            }

            var expr = this.resolverSelector.GetResolverExpression(containerContext, new TypeInformation { Type = type, DependencyName = name }, resolutionInfo);
            if (expr == null)
                if (resolutionInfo.NullResultAllowed)
                    return null;
                else
                    throw new ResolutionFailedException(type.FullName);

            var factory = expr.CompileDelegate(Constants.ScopeExpression);
            this.containerContext.DelegateRepository.AddServiceDelegate(type, factory, name);
            return factory(resolutionInfo.ResolutionScope);
        }

        private Delegate ActivateFactoryDelegate(Type type, Type[] parameterTypes, IResolutionScope resolutionScope, string name, bool nullResultAllowed)
        {
            var resolutionInfo = new ResolutionInfo(resolutionScope, nullResultAllowed)
            {
                ParameterExpressions = parameterTypes.Length == 0 ? null : parameterTypes.Select(Expression.Parameter).ToArray()
            };
            var typeInfo = new TypeInformation { Type = type, DependencyName = name };
            var registration = this.containerContext.RegistrationRepository.GetRegistrationOrDefault(typeInfo);

            var initExpression = registration == null ?
                this.resolverSelector.GetResolverExpression(containerContext, typeInfo, resolutionInfo) :
                registration.GetExpression(resolutionInfo, type);

            if (initExpression == null)
                throw new ResolutionFailedException(typeInfo.Type.FullName);

            var factory = Expression.Lambda(initExpression, resolutionInfo.ParameterExpressions).CompileDelegate(Constants.ScopeExpression);
            this.containerContext.DelegateRepository.AddFactoryDelegate(type, parameterTypes, factory, name);
            return factory(resolutionScope);
        }
    }
}