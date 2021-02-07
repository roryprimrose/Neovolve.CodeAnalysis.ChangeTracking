﻿namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class MemberModifiersChangeTable : ChangeTable<MemberModifiers>, IMemberModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(MemberModifiers.None, MemberModifiers.None, SemVerChangeType.None);
            AddChange(MemberModifiers.None, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.New, SemVerChangeType.None);
            AddChange(MemberModifiers.None, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.None, MemberModifiers.Sealed, SemVerChangeType.None);
            AddChange(MemberModifiers.None, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.None, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.None, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.None, MemberModifiers.SealedOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.Abstract, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Abstract, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Abstract, MemberModifiers.AbstractOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.Abstract, MemberModifiers.NewAbstract, SemVerChangeType.None);
            AddChange(MemberModifiers.Abstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.Abstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Abstract, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Abstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.None, SemVerChangeType.None);
            AddChange(MemberModifiers.New, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.New, SemVerChangeType.None);
            AddChange(MemberModifiers.New, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.New, MemberModifiers.Sealed, SemVerChangeType.None);
            AddChange(MemberModifiers.New, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.New, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.New, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.New, MemberModifiers.SealedOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.Override, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.Virtual, SemVerChangeType.None);
            AddChange(MemberModifiers.Override, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Override, MemberModifiers.NewVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.Override, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.None, SemVerChangeType.None);
            AddChange(MemberModifiers.Sealed, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.New, SemVerChangeType.None);
            AddChange(MemberModifiers.Sealed, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Sealed, MemberModifiers.Sealed, SemVerChangeType.None);
            AddChange(MemberModifiers.Sealed, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Sealed, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Sealed, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.Sealed, MemberModifiers.SealedOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.Static, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.Static, SemVerChangeType.None);
            AddChange(MemberModifiers.Static, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.NewStatic, SemVerChangeType.None);
            AddChange(MemberModifiers.Static, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Static, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.Override, SemVerChangeType.None);
            AddChange(MemberModifiers.Virtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.Virtual, SemVerChangeType.None);
            AddChange(MemberModifiers.Virtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.Virtual, MemberModifiers.NewVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.Virtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.Abstract, SemVerChangeType.None);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.AbstractOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.NewAbstract, SemVerChangeType.None);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.AbstractOverride, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.Abstract, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.AbstractOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.NewAbstract, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.Abstract, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstract, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.NewAbstractVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.Override, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.Static, SemVerChangeType.None);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.Virtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.NewStatic, SemVerChangeType.None);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.NewVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewStatic, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.None, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.New, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.Override, SemVerChangeType.None);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.Virtual, SemVerChangeType.None);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.NewVirtual, SemVerChangeType.None);
            AddChange(MemberModifiers.NewVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.None, SemVerChangeType.None);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.Abstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.New, SemVerChangeType.None);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.Override, SemVerChangeType.Feature);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.Sealed, SemVerChangeType.None);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.Static, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.Virtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.NewAbstract, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.NewVirtual, SemVerChangeType.Feature);
            AddChange(MemberModifiers.SealedOverride, MemberModifiers.SealedOverride, SemVerChangeType.None);
            // @formatter:on — enable formatter after this line
        }
    }
}