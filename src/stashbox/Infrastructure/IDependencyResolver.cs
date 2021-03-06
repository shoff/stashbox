﻿using Stashbox.Exceptions;
using System;
using System.Collections.Generic;

namespace Stashbox.Infrastructure
{
    /// <summary>
    /// Represents a dependency resolver.
    /// </summary>
    public interface IDependencyResolver : IDisposable
    {
        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        TKey Resolve<TKey>(bool nullResultAllowed = false);

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, bool nullResultAllowed = false);

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        TKey Resolve<TKey>(object name, bool nullResultAllowed = false);

        /// <summary>
        /// Resolves an instance from the container.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instance.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The resolved object.</returns>
        object Resolve(Type typeFrom, object name, bool nullResultAllowed = false);

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <typeparam name="TKey">The type of the requested instance.</typeparam>
        /// <returns>The resolved object.</returns>
        IEnumerable<TKey> ResolveAll<TKey>();

        /// <summary>
        /// Resolves all registered types of a service.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <returns>The resolved object.</returns>
        IEnumerable<object> ResolveAll(Type typeFrom);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <param name="typeFrom">The type of the requested instances.</param>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <param name="parameterTypes">The parameter type.</param>
        /// <returns>The factory delegate.</returns>
        Delegate ResolveFactory(Type typeFrom, object name = null, bool nullResultAllowed = false, params Type[] parameterTypes);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        Func<TService> ResolveFactory<TService>(object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        Func<T1, TService> ResolveFactory<T1, TService>(object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        Func<T1, T2, TService> ResolveFactory<T1, T2, TService>(object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        Func<T1, T2, T3, TService> ResolveFactory<T1, T2, T3, TService>(object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Returns a factory method which can be used to activate a type.
        /// </summary>
        /// <typeparam name="TService">The service type.</typeparam>
        /// <typeparam name="T1">The first parameter of the factory.</typeparam>
        /// <typeparam name="T2">The second parameter of the factory.</typeparam>
        /// <typeparam name="T3">The third parameter of the factory.</typeparam>
        /// <typeparam name="T4">The fourth parameter of the factory.</typeparam>
        /// <param name="name">The name of the requested registration.</param>
        /// <param name="nullResultAllowed">If true, the container will return with null instead of throwing <see cref="ResolutionFailedException"/>.</param>
        /// <returns>The factory delegate.</returns>
        Func<T1, T2, T3, T4, TService> ResolveFactory<T1, T2, T3, T4, TService>(object name = null, bool nullResultAllowed = false);

        /// <summary>
        /// Begins a new scope.
        /// </summary>
        IDependencyResolver BeginScope();

        /// <summary>
        /// Puts an instance into the scope which will be dropped when the scope is being disposed.
        /// </summary>
        /// <typeparam name="TFrom">The service type.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The scope.</returns>
        IDependencyResolver PutInstanceInScope<TFrom>(TFrom instance, bool withoutDisposalTracking = false);

        /// <summary>
        /// Puts an instance into the scope which will be dropped when the scope is being disposed.
        /// </summary>
        /// <param name="typeFrom">The service type.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="withoutDisposalTracking">If it's set to true the container will exclude the instance from the disposal tracking.</param>
        /// <returns>The scope.</returns>
        IDependencyResolver PutInstanceInScope(Type typeFrom, object instance, bool withoutDisposalTracking = false);

        /// <summary>
        /// Builds up an instance, the container will perform injections and extensions on it.
        /// </summary>
        /// <typeparam name="TTo">The type of the requested instance.</typeparam>
        /// <param name="instance">The instance to build up.</param>
        /// <returns>The built object.</returns>
        TTo BuildUp<TTo>(TTo instance);
    }
}
