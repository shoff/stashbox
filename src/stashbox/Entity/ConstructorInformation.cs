﻿using System.Collections.Generic;
using System.Reflection;

namespace Stashbox.Entity
{
    public class ConstructorInformation
    {
        public ConstructorInfo Constructor { get; set; }

        public bool HasInjectionAttribute { get; set; }

        public HashSet<TypeInformation> Parameters { get; set; }
    }
}