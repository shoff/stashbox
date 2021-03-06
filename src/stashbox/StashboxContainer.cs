﻿using Stashbox.BuildUp.Expressions;
using Stashbox.BuildUp.Resolution;
using Stashbox.Configuration;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using Stashbox.Infrastructure.Registration;
using Stashbox.Infrastructure.Resolution;
using Stashbox.Registration;
using Stashbox.Resolution;
using Stashbox.Utils;
using System;
using System.Linq.Expressions;
using Stashbox.BuildUp;

namespace Stashbox
{
    /// <summary>
    /// Represents the stashbox dependency injection container.
    /// </summary>
    public partial class StashboxContainer : ResolutionScopeBase, IStashboxContainer
    {
        private readonly IContainerExtensionManager containerExtensionManager;
        private readonly IResolverSelector resolverSelector;
        private readonly IRegistrationRepository registrationRepository;
        private readonly IExpressionBuilder expressionBuilder;
        private readonly AtomicBool disposed;
        private readonly IActivationContext activationContext;
        private readonly IObjectBuilderSelector objectBuilderSelector;

        /// <summary>
        /// Constructs a <see cref="StashboxContainer"/>
        /// </summary>
        public StashboxContainer(Action<IContainerConfigurator> config = null)
        {
            this.disposed = new AtomicBool();
            this.containerExtensionManager = new BuildExtensionManager();
            this.resolverSelector = new ResolverSelector();

            var configurator = new ContainerConfigurator();
            config?.Invoke(configurator);

            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), configurator, new DecoratorRepository());
            this.activationContext = new Resolution.ActivationContext(this.ContainerContext, this.resolverSelector, this);
            this.expressionBuilder = new ExpressionBuilder(this.ContainerContext, this.containerExtensionManager);
            this.objectBuilderSelector = new ObjectBuilderSelector(this.ContainerContext, this.expressionBuilder);
            this.ServiceRegistrator = new ServiceRegistrator(this.ContainerContext, this.containerExtensionManager, this.objectBuilderSelector);
            this.RegisterResolvers();
        }

        internal StashboxContainer(IStashboxContainer parentContainer, IContainerExtensionManager containerExtensionManager,
            IResolverSelector resolverSelector)
        {
            this.disposed = new AtomicBool();
            this.ParentContainer = parentContainer;
            this.containerExtensionManager = containerExtensionManager;
            this.resolverSelector = resolverSelector;
            this.registrationRepository = new RegistrationRepository();
            this.ContainerContext = new ContainerContext(this.registrationRepository, new DelegateRepository(), this,
                new ResolutionStrategy(this.resolverSelector), parentContainer.ContainerContext.ContainerConfigurator,
                parentContainer.ContainerContext.DecoratorRepository);
            this.activationContext = new Resolution.ActivationContext(this.ContainerContext, this.resolverSelector, this);
            this.containerExtensionManager.ReinitalizeExtensions(this.ContainerContext);
            this.expressionBuilder = new ExpressionBuilder(this.ContainerContext, this.containerExtensionManager);
            this.objectBuilderSelector = new ObjectBuilderSelector(this.ContainerContext, this.expressionBuilder);
            this.ServiceRegistrator = new ServiceRegistrator(this.ContainerContext, this.containerExtensionManager, this.objectBuilderSelector);
        }

        /// <inheritdoc />
        public void RegisterExtension(IContainerExtension containerExtension)
        {
            containerExtension.Initialize(this.ContainerContext);
            this.containerExtensionManager.AddExtension(containerExtension);
        }

        /// <inheritdoc />
        public void RegisterResolver(Resolver resolver) =>
            this.resolverSelector.AddResolver(resolver);

        /// <inheritdoc />
        public bool CanResolve<TFrom>(object name = null) =>
            this.CanResolve(typeof(TFrom), name);

        /// <inheritdoc />
        public bool CanResolve(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name) ||
                this.resolverSelector.CanResolve(this.ContainerContext, new TypeInformation { Type = typeFrom, DependencyName = name }, ResolutionInfo.New(this, this));

        /// <inheritdoc />
        public bool IsRegistered<TFrom>(object name = null) =>
            this.IsRegistered(typeof(TFrom), name);

        /// <inheritdoc />
        public bool IsRegistered(Type typeFrom, object name = null) =>
            this.registrationRepository.ContainsRegistration(typeFrom, name);

        /// <inheritdoc />
        public void Validate()
        {
            foreach (var serviceRegistration in this.registrationRepository.GetAllRegistrations())
                serviceRegistration.GetExpression(ResolutionInfo.New(this, this), serviceRegistration.ServiceType);
        }

        /// <inheritdoc />
        public IStashboxContainer ParentContainer { get; }

        /// <inheritdoc />
        public IContainerContext ContainerContext { get; }

        /// <inheritdoc />
        public IServiceRegistrator ServiceRegistrator { get; }

        /// <inheritdoc />
        public IStashboxContainer CreateChildContainer() =>
             new StashboxContainer(this, this.containerExtensionManager.CreateCopy(), this.resolverSelector);

        /// <inheritdoc />
        public IDependencyResolver BeginScope() => new ResolutionScope(this.activationContext,
            this.ServiceRegistrator, this.expressionBuilder, this);

        /// <inheritdoc />
        public void Configure(Action<IContainerConfigurator> config) =>
            config?.Invoke(this.ContainerContext.ContainerConfigurator);

        private void RegisterResolvers()
        {
            this.resolverSelector.AddResolver(new EnumerableResolver());
            this.resolverSelector.AddResolver(new LazyResolver(this.resolverSelector));
            this.resolverSelector.AddResolver(new FuncResolver());
            this.resolverSelector.AddResolver(new TupleResolver());
            this.resolverSelector.AddResolver(new ScopedInstanceResolver());
            this.resolverSelector.AddResolver(new DefaultValueResolver());
        }

        /// <inheritdoc />
        public TTo BuildUp<TTo>(TTo instance)
        {
            var typeTo = instance.GetType();
            var registration = this.ServiceRegistrator.PrepareContext(typeTo, typeTo);
            var expr = this.expressionBuilder.CreateFillExpression(registration.CreateServiceRegistration(false), Expression.Constant(instance), ResolutionInfo.New(this, this), typeTo);
            var factory = expr.CompileDelegate(Constants.ScopeExpression);
            return (TTo)factory(this);
        }

        /// <summary>
        /// Disposes the container.
        /// </summary>
        /// <param name="disposing">Indicates the container is disposing or not.</param>
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed.CompareExchange(false, true) || !disposing) return;
            base.Dispose(true);
            this.containerExtensionManager.CleanUp();
        }
    }
}
