﻿using System;
using Microsoft.Extensions.DependencyInjection;
using ScrapeFunction.Modules;
using ScrapeFunction.Containers;

namespace ScrapeFunction.Containers
{
    // <summary>
    /// This represents the builder entity for IoC container.
    /// </summary>
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerBuilder"/> class.
        /// </summary>
        public ContainerBuilder()
        {
            this._services = new ServiceCollection();
        }

        public IContainerBuilder RegisterModule(IModule module = null)
        {
            if (module == null)
            {
                module = new Module();
            }

            module.Load(this._services);

            return this;
        }

        /// <inheritdoc />
        public IServiceProvider Build()
        {
            var provider = this._services.BuildServiceProvider();

            return provider;
        }
    }
}
