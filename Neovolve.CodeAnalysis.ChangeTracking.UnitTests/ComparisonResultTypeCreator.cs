namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests
{
    using System;
    using ModelBuilder;
    using ModelBuilder.TypeCreators;

    public class ComparisonResultTypeCreator : TypeCreatorBase
    {
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
            string referenceName)
        {
            var canCreate = base.CanCreate(configuration, buildChain, type, referenceName);

            if (canCreate == false)
            {
                return false;
            }

            return type == typeof(ComparisonResult);
        }

        protected override object CreateInstance(IExecuteStrategy executeStrategy, Type type, string referenceName,
            params object[] args)
        {
            var changeType = (SemVerChangeType) executeStrategy.Create(typeof(SemVerChangeType));
            var match = (MemberMatch) executeStrategy.Create(typeof(MemberMatch));
            var message = "Some kind of change " + Guid.NewGuid();

            return ComparisonResult.MemberChanged(changeType, match, message);
        }

        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            return instance;
        }

        public override bool AutoDetectConstructor { get; } = false;

        public override bool AutoPopulate => false;

        public override int Priority { get; } = 100;
    }
}