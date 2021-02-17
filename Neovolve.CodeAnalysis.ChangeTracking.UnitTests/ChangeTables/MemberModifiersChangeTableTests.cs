﻿namespace Neovolve.CodeAnalysis.ChangeTracking.UnitTests.ChangeTables
{
    using System;
    using FluentAssertions;
    using Neovolve.CodeAnalysis.ChangeTracking.ChangeTables;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Xunit;

    public class MemberModifiersChangeTableTests
    {
        [Theory]
        [ClassData(typeof(EnumCombinationsDataSet<MemberModifiers>))]
        public void CalculateChangeHandlesAllPossibleValues(MemberModifiers oldValue, MemberModifiers newValue)
        {
            var sut = new MemberModifiersChangeTable();

            Action action = () => sut.CalculateChange(oldValue, newValue);

            action.Should().NotThrow();
        }

        // @formatter:off — disable formatter after this line
        [Theory]
        [InlineData(MemberModifiers.None, MemberModifiers.None, SemVerChangeType.None)]
        [InlineData(MemberModifiers.None, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.New, SemVerChangeType.None)]
        [InlineData(MemberModifiers.None, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.None, MemberModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(MemberModifiers.None, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.None, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.None, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.None, MemberModifiers.SealedOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.AbstractOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.NewAbstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Abstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.None, SemVerChangeType.None)]
        [InlineData(MemberModifiers.New, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.New, SemVerChangeType.None)]
        [InlineData(MemberModifiers.New, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.New, MemberModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(MemberModifiers.New, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.New, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.New, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.New, MemberModifiers.SealedOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Override, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.Override, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Override, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.Virtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Override, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Override, MemberModifiers.NewVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Override, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.None, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.New, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.Sealed, MemberModifiers.SealedOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Static, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.Override, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.Static, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Static, MemberModifiers.Virtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.NewStatic, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Static, MemberModifiers.NewVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Static, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.Override, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.Virtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.NewVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.Virtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.AbstractOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.NewAbstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.AbstractOverride, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.AbstractOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.NewAbstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstract, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.Abstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstract, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.NewAbstractVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.Override, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.Static, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.Virtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.NewStatic, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.NewVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewStatic, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.None, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.New, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.Override, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.Sealed, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.Virtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.NewVirtual, SemVerChangeType.None)]
        [InlineData(MemberModifiers.NewVirtual, MemberModifiers.SealedOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.None, SemVerChangeType.None)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.Abstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.New, SemVerChangeType.None)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.Override, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.Sealed, SemVerChangeType.None)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.Static, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.Virtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.AbstractOverride, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.NewAbstract, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.NewAbstractVirtual, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.NewStatic, SemVerChangeType.Breaking)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.NewVirtual, SemVerChangeType.Feature)]
        [InlineData(MemberModifiers.SealedOverride, MemberModifiers.SealedOverride, SemVerChangeType.None)]
        // @formatter:on — enable formatter after this line
        public void CalculateChangeReturnsExpectedValue(
            MemberModifiers oldModifiers,
            MemberModifiers newModifiers,
            SemVerChangeType expected)
        {
            var sut = new MemberModifiersChangeTable();

            var actual = sut.CalculateChange(oldModifiers, newModifiers);

            actual.Should().Be(expected);
        }
    }
}