namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;

    public class Change
    {
        private Change(ChangeType changeType, MemberDefinition? oldMember, MemberDefinition? newMember, string message)
        {
            ChangeType = changeType;
            OldMember = oldMember;
            NewMember = newMember;
            Message = message;
        }

        public static Change MemberChanged(ChangeType changeType, MemberDefinition oldMember, MemberDefinition newMember,
            string message)
        {
            changeType = changeType == ChangeType.None
                ? throw new ArgumentException("The changeType cannot be None to indicate a change on the member.",
                    nameof(changeType))
                : changeType;
            oldMember = oldMember ?? throw new ArgumentNullException(nameof(oldMember));
            newMember = newMember ?? throw new ArgumentNullException(nameof(newMember));
            message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentException(nameof(message)) : message;

            return new Change(changeType, oldMember, newMember, message);
        }

        public static Change MemberAdded(MemberDefinition newMember)
        {
            newMember = newMember ?? throw new ArgumentNullException(nameof(newMember));

            var message = newMember + " has been added";

            var changeType = ChangeType.None;

            if (newMember.IsPublic)
            {
                changeType = ChangeType.Feature;
            }

            return new Change(changeType, null, newMember, message);
        }

        public static Change MemberRemoved(MemberDefinition oldMember)
        {
            oldMember = oldMember ?? throw new ArgumentNullException(nameof(oldMember));

            var message = oldMember + " has been removed";

            var changeType = ChangeType.None;

            if (oldMember.IsPublic)
            {
                changeType = ChangeType.Breaking;
            }

            return new Change(changeType, oldMember, null, message);
        }

        public ChangeType ChangeType { get; }

        public MemberDefinition? NewMember { get; }

        public MemberDefinition? OldMember { get; }

        public string Message { get; }
    }
}