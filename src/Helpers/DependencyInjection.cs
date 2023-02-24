using System;
using Microsoft.Extensions.DependencyInjection;
using PunchedCards.BitVectors;
using PunchedCards.Helpers;

namespace PunchedCards.Helpers
{
    internal static class DependencyInjection
    {
        internal static IServiceProvider ServiceProvider { get; } = new ServiceCollection()
                    .AddSingleton<IBitVectorFactory, BitVectorFactory>()
                    .BuildServiceProvider();
    }
}