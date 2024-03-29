﻿using Microsoft.Extensions.DependencyInjection;
using BlazorForms.FlowRules;
using BlazorForms.Flows;
using BlazorForms.Flows.Definitions;
using BlazorForms.Forms;
using BlazorForms.Shared;
using BlazorForms.Shared.Reflection;
using BlazorForms.Platform;
using BlazorForms.Rendering.State;
using BlazorForms.Rendering.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using BlazorForms.Platform.Shared.ApplicationParts;

namespace BlazorForms
{
    public static class BlazorFormsServerServiceCollectionExtensions
    {
        public static IServiceCollection AddGenericType([NotNullAttribute] this IServiceCollection serviceCollection, Type type)
        {
            var types = new List<Type>();
            types.Add(type);
            KnownTypesBinder.InitializeConfiguration(types.Distinct());
            return serviceCollection;
        }

        public static IServiceCollection AddGenericTypes([NotNullAttribute] this IServiceCollection serviceCollection, 
            IEnumerable<Type> generics, IEnumerable<Type> arguments)
        {
            var types = new List<Type>();

            foreach (var g in generics)
            {
                foreach (var a in arguments)
                {
                    var type = g.MakeGenericType(a);
                    types.Add(type);
                }
            }

            KnownTypesBinder.InitializeConfiguration(types.Distinct());
            return serviceCollection;
        }

        public static IServiceCollection AddBlazorFormsServerModelAssemblyTypes([NotNullAttribute] this IServiceCollection serviceCollection, Type anyAssemblyType)
        {
            var assembly = anyAssemblyType.GetTypeInfo().Assembly;
            PlatformAssemblyRegistrator.InitializeConfiguration(new Assembly[] { assembly });

            var types = assembly.GetTypes().ToList();

            types.AddRange(new Type[] {
                typeof(System.Dynamic.ExpandoObject),
                typeof(Dictionary<string, string>),
                typeof(Dictionary<string, DynamicRecordset>)
            });

            types.AddRange(typeof(FlowContext).Assembly.GetTypes());
            types.AddRange(typeof(FlowParamsGeneric).Assembly.GetTypes());
            types.AddRange(typeof(DynamicRecordset).Assembly.GetTypes());
            types.AddRange(typeof(FormDetails).Assembly.GetTypes());
            types.AddRange(typeof(RuleEngineExecutionResult).Assembly.GetTypes());

            KnownTypesBinder.InitializeConfiguration(types.Distinct());

            return serviceCollection;
        }
    }
}
