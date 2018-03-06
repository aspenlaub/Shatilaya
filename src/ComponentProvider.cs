﻿using System;
using System.Collections.Generic;
using Aspenlaub.Net.GitHub.CSharp.Shatilaya.Interfaces;

namespace Aspenlaub.Net.GitHub.CSharp.Shatilaya {
    public class ComponentProvider : IComponentProvider {
        private Dictionary<Type, object> DefaultComponents { get; }

        public ComponentProvider() {
            DefaultComponents = new Dictionary<Type, object>();
        }

        private T DefaultComponent<T, T2>() where T : class where T2 : T, new() {
            if (!DefaultComponents.ContainsKey(typeof(T))) {
                DefaultComponents[typeof(T)] = new T2();
            }
            return (T)DefaultComponents[typeof(T)];
        }

        // ReSharper disable once UnusedMember.Local
        private T DefaultComponent<T, T2>(Func<T2> constructor) where T : class where T2 : T {
            if (!DefaultComponents.ContainsKey(typeof(T))) {
                DefaultComponents[typeof(T)] = constructor();
            }
            return (T)DefaultComponents[typeof(T)];
        }

        public ICakeRunner CakeRunner { get { return DefaultComponent<ICakeRunner, CakeRunner>(); } }
        public IFolderDeleter FolderDeleter { get { return DefaultComponent<IFolderDeleter, FolderDeleter>(); } }
        public IGitUtilities GitUtilities { get { return DefaultComponent<IGitUtilities, GitUtilities>(); } }
        public ILatestBuildCakeScriptProvider LatestBuildCakeScriptProvider { get { return DefaultComponent<ILatestBuildCakeScriptProvider, LatestBuildCakeScriptProvider>(); } }
    }
}
