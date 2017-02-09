﻿using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp.Expressions;

namespace Stashbox.BuildUp
{
    internal class WireUpObjectBuilder : IObjectBuilder
    {
        private object instance;
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly object syncObject = new object();
        private readonly IContainerContext containerContext;
        private readonly IMetaInfoProvider metaInfoProvider;
        private readonly InjectionParameter[] injectionParameters;
        private bool instanceBuilt;

        public WireUpObjectBuilder(object instance, IContainerExtensionManager containerExtensionManager, IContainerContext containerContext,
            IMetaInfoProvider metaInfoProvider, InjectionParameter[] injectionParameters = null)
        {
            this.instance = instance;
            this.containerExtensionManager = containerExtensionManager;
            this.containerContext = containerContext;
            this.metaInfoProvider = metaInfoProvider;
            this.injectionParameters = injectionParameters;
        }

        public Expression GetExpression(ResolutionInfo resolutionInfo, TypeInformation resolveType)
        {
            if (this.instanceBuilt) return Expression.Constant(this.instance);
            lock (this.syncObject)
            {
                if (this.instanceBuilt) return Expression.Constant(this.instance);
                this.instanceBuilt = true;

                var expr = ExpressionDelegateFactory.CreateFillExpression(this.containerExtensionManager, this.containerContext,
                    Expression.Constant(this.instance), resolutionInfo, new TypeInformation { Type = this.metaInfoProvider.TypeTo }, this.injectionParameters,
                    this.metaInfoProvider.GetResolutionMembers(resolutionInfo, this.injectionParameters),
                    this.metaInfoProvider.GetResolutionMethods(resolutionInfo, this.injectionParameters));
                var factory = Expression.Lambda<Func<object>>(expr).Compile();
                this.instance = factory();
                return Expression.Constant(this.instance);
            }
        }

        public bool HandlesObjectDisposal => true;

        public void CleanUp()
        {
            if (this.instance == null) return;
            lock (this.syncObject)
            {
                if (this.instance == null) return;
                var disposable = this.instance as IDisposable;
                disposable?.Dispose();
                this.instance = null;
            }
        }
    }
}