﻿using Stashbox.Attributes;
using Stashbox.Configuration;
using Stashbox.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Stashbox.MetaInfo
{
    /// <summary>
    /// Represents a store which contains metadata about the services.
    /// </summary>
    public class MetaInfoCache
    {
        private readonly ContainerConfiguration containerConfiguration;

        /// <summary>
        /// The type of the actual service implementation.
        /// </summary>
        public Type TypeTo { get; }

        /// <summary>
        /// Stores the reflected constructor informations.
        /// </summary>
        public ConstructorInformation[] Constructors { get; private set; }

        /// <summary>
        /// Stores the reflected injection method informations.
        /// </summary>
        public MethodInformation[] InjectionMethods { get; private set; }

        /// <summary>
        /// Stores the reflected injection memeber informations.
        /// </summary>
        public MemberInformation[] InjectionMembers { get; private set; }

        /// <summary>
        /// Constructs the <see cref="MetaInfoCache"/>
        /// </summary>
        /// <param name="containerConfiguraton">The container configuration.</param>
        /// <param name="typeTo">The type of the actual service implementation.</param>
        public MetaInfoCache(ContainerConfiguration containerConfiguraton, Type typeTo)
        {
            this.TypeTo = typeTo;
            this.containerConfiguration = containerConfiguraton;

            var typeInfo = typeTo.GetTypeInfo();
            this.AddConstructors(typeInfo.DeclaredConstructors);
            this.AddMethods(typeInfo.DeclaredMethods);
            this.InjectionMembers = this.FillMembers(typeInfo).ToArray();
        }

        private void AddConstructors(IEnumerable<ConstructorInfo> infos)
        {
            this.Constructors = infos.Where(info => !info.IsStatic).Select(info => new ConstructorInformation
            {
                Constructor = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private void AddMethods(IEnumerable<MethodInfo> infos)
        {
            this.InjectionMethods = infos.Where(methodInfo => methodInfo.GetCustomAttribute<InjectionMethodAttribute>() != null).Select(info => new MethodInformation
            {
                Method = info,
                Parameters = this.FillParameters(info.GetParameters()).ToArray()
            }).ToArray();
        }

        private IEnumerable<TypeInformation> FillParameters(IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(parameterInfo => new TypeInformation
            {
                Type = parameterInfo.ParameterType,
                DependencyName = parameterInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                 parameterInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                ParentType = this.TypeTo,
                CustomAttributes = parameterInfo.GetCustomAttributes().ToArray(),
                ParameterName = parameterInfo.Name,
                HasDefaultValue = parameterInfo.HasDefaultValue,
                DefaultValue = parameterInfo.DefaultValue
            });
        }

        private IEnumerable<MemberInformation> FillMembers(TypeInfo typeInfo)
        {
            return this.SelectMembers(typeInfo.DeclaredProperties)
                   .OfType<PropertyInfo>()
                   .Select(propertyInfo => new MemberInformation
                   {
                       TypeInformation = new TypeInformation
                       {
                           Type = propertyInfo.PropertyType,
                           DependencyName = propertyInfo.GetCustomAttribute<DependencyAttribute>() != null ?
                                        propertyInfo.GetCustomAttribute<DependencyAttribute>().Name : null,
                           ParentType = this.TypeTo,
                           CustomAttributes = propertyInfo.GetCustomAttributes().ToArray(),
                           ParameterName = propertyInfo.Name,
                           IsMember = true
                       },
                       MemberInfo = propertyInfo
                   })
                   .Concat(this.SelectMembers(typeInfo.DeclaredFields)
                           .OfType<FieldInfo>()
                           .Where(fieldInfo => fieldInfo.GetCustomAttribute<DependencyAttribute>() != null)
                           .Select(fieldInfo => new MemberInformation
                           {
                               TypeInformation = new TypeInformation
                               {
                                   Type = fieldInfo.FieldType,
                                   DependencyName = fieldInfo.GetCustomAttribute<DependencyAttribute>() != null
                                       ? fieldInfo.GetCustomAttribute<DependencyAttribute>().Name
                                       : null,
                                   ParentType = this.TypeTo,
                                   CustomAttributes = fieldInfo.GetCustomAttributes().ToArray(),
                                   ParameterName = fieldInfo.Name,
                                   IsMember = true
                               },
                               MemberInfo = fieldInfo
                           }));
        }

        private IEnumerable<MemberInfo> SelectMembers(IEnumerable<MemberInfo> members)
        {
            if (this.containerConfiguration.MemberInjectionWithoutAnnotationEnabled)
                return members;
            else
                return members.Where(memberInfo => memberInfo.GetCustomAttribute<DependencyAttribute>() != null);
        }
    }
}
