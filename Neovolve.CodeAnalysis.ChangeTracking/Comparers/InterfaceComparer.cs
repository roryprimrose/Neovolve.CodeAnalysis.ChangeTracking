namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class InterfaceComparer : TypeComparer<IInterfaceDefinition>, IInterfaceComparer
    {
        public InterfaceComparer(IAccessModifiersComparer accessModifiersComparer,
            IGenericTypeElementComparer genericTypeElementComparer,
            IPropertyMatchProcessor propertyProcessor, IMethodMatchProcessor methodProcessor,
            IAttributeMatchProcessor attributeProcessor)
            : base(accessModifiersComparer, genericTypeElementComparer, propertyProcessor,
                methodProcessor, attributeProcessor)
        {
        }
    }
}