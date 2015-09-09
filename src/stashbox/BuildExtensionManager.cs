﻿using Ronin.Common;
using Stashbox.Entity;
using Stashbox.Infrastructure;
using Stashbox.Infrastructure.ContainerExtension;
using System.Collections.Generic;

namespace Stashbox
{
    internal class BuildExtensionManager : IContainerExtensionManager
    {
        private readonly HashSet<IPreBuildExtension> prebuildExtensions;
        private readonly HashSet<IPostBuildExtension> postbuildExtensions;
        private readonly HashSet<IRegistrationExtension> registrationExtensions;
        private readonly DisposableReaderWriterLock readerWriterLock;

        public BuildExtensionManager()
        {
            this.prebuildExtensions = new HashSet<IPreBuildExtension>();
            this.postbuildExtensions = new HashSet<IPostBuildExtension>();
            this.registrationExtensions = new HashSet<IRegistrationExtension>();
            this.readerWriterLock = new DisposableReaderWriterLock();
        }

        public void AddExtension(IContainerExtension containerExtension)
        {
            using (this.readerWriterLock.AquireWriteLock())
            {
                var preBuildExtension = containerExtension as IPreBuildExtension;
                if (preBuildExtension != null)
                    this.prebuildExtensions.Add(preBuildExtension);

                var postBuildExtension = containerExtension as IPostBuildExtension;
                if (postBuildExtension != null)
                    this.postbuildExtensions.Add(postBuildExtension);

                var registrationExtension = containerExtension as IRegistrationExtension;
                if (registrationExtension != null)
                    this.registrationExtensions.Add(registrationExtension);
            }
        }

        public object ExecutePostBuildExtensions(object instance, IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                var result = instance;
                foreach (var extension in this.postbuildExtensions)
                {
                    result = extension.PostBuild(instance, builderContext, resolutionInfo);
                }

                return result;
            }
        }

        public void ExecutePreBuildExtensions(IBuilderContext builderContext, ResolutionInfo resolutionInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                foreach (var extension in this.prebuildExtensions)
                {
                    extension.PreBuild(builderContext, resolutionInfo);
                }
            }
        }

        public void ExecuteOnRegistrationExtensions(IBuilderContext builderContext, RegistrationInfo registrationInfo)
        {
            using (this.readerWriterLock.AquireReadLock())
            {
                foreach (var extension in this.registrationExtensions)
                {
                    extension.OnRegistration(builderContext, registrationInfo);
                }
            }
        }
    }
}
