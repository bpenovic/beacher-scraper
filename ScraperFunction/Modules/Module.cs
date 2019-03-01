using System;
using Microsoft.Extensions.DependencyInjection;

namespace ScrapeFunction.Modules
{
    /// <summary>
    /// This represents the entity containing a list of dependencies.
    /// </summary>
    public class Module: IModule
    {
        public virtual void Load(IServiceCollection services)
        {
            return;
        }
    }
}
