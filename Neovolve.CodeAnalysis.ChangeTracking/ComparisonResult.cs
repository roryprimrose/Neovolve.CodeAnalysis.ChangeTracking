namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class ComparisonResult
    {
        private ComparisonResult(SemVerChangeType changeType, MemberDefinition? oldMember, MemberDefinition? newMember, string message)
        {
            ChangeType = changeType;
            OldMember = oldMember;
            NewMember = newMember;
            Message = message;
        }

        public static ComparisonResult MemberChanged(SemVerChangeType changeType, MemberDefinition oldMember, MemberDefinition newMember,
            string message)
        {
            changeType = changeType == SemVerChangeType.None
                ? throw new ArgumentException("The changeType cannot be None to indicate a change on the member.",
                    nameof(changeType))
                : changeType;
            oldMember = oldMember ?? throw new ArgumentNullException(nameof(oldMember));
            newMember = newMember ?? throw new ArgumentNullException(nameof(newMember));
            message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException(nameof(message)) : message;

            return new ComparisonResult(changeType, oldMember, newMember, message);
        }

        public static ComparisonResult MemberAdded(MemberDefinition newMember)
        {
            newMember = newMember ?? throw new ArgumentNullException(nameof(newMember));

            var message = newMember + " has been added";

            var changeType = SemVerChangeType.None;

            if (newMember.IsPublic)
            {
                changeType = SemVerChangeType.Feature;
            }

            return new ComparisonResult(changeType, null, newMember, message);
        }

        public static ComparisonResult MemberRemoved(MemberDefinition oldMember)
        {
            oldMember = oldMember ?? throw new ArgumentNullException(nameof(oldMember));

            var message = oldMember + " has been removed";

            var changeType = SemVerChangeType.None;

            if (oldMember.IsPublic)
            {
                changeType = SemVerChangeType.Breaking;
            }

            return new ComparisonResult(changeType, oldMember, null, message);
        }

        public SemVerChangeType ChangeType { get; }

        public MemberDefinition? NewMember { get; }

        public MemberDefinition? OldMember { get; }

        public string Message { get; }
    }
}