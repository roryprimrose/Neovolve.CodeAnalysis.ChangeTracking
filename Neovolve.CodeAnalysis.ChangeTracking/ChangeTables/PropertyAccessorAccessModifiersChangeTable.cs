﻿namespace Neovolve.CodeAnalysis.ChangeTracking.ChangeTables
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class PropertyAccessorAccessModifiersChangeTable : ChangeTable<PropertyAccessorAccessModifiers>, IPropertyAccessorAccessModifiersChangeTable
    {
        protected override void BuildChanges()
        {
            // @formatter:off — disable formatter after this line
            AddChange(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.None, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.None );
            AddChange(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Internal, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.None );
            AddChange(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Private, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.Protected, PropertyAccessorAccessModifiers.ProtectedInternal, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.None, SemVerChangeType.Feature );
            AddChange(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Internal, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Private, SemVerChangeType.Breaking );
            AddChange(PropertyAccessorAccessModifiers.ProtectedInternal, PropertyAccessorAccessModifiers.Protected, SemVerChangeType.Breaking );
            // @formatter:on — enable formatter after this line
        }
    }
}