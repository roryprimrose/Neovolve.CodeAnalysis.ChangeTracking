namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using ModelBuilder;
    using ModelBuilder.ValueGenerators;

    public class SemVerChangeTypeValueGenerator : ValueGeneratorBase
    {
        protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
        {
            var values = Enum.GetValues(typeof(SemVerChangeType));

            // Skip the first entry which should be None
            var valueIndex = Generator.NextValue(1, values.Length - 1);

            return values.GetValue(valueIndex);
        }

        protected override bool IsMatch(IBuildChain buildChain, Type type, string referenceName)
        {
            return type == typeof(SemVerChangeType);
        }

        public override int Priority { get; } = 100;
    }
}