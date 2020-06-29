﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    public interface IMemberComparer
    {
        ComparisonResult Compare(MemberMatch match);

        bool IsSupported(OldMemberDefinition member);
    }
}