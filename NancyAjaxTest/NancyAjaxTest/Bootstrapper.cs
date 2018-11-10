using System;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;

namespace NancyAjaxTest
{
    public class CustomBoostrapper : DefaultNancyBootstrapper
    {
        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);

            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("assets", "Assets")
            );
        }
    }
}