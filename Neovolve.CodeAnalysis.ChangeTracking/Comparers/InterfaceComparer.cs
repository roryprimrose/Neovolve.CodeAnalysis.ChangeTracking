namespace Neovolve.CodeAnalysis.ChangeTracking.Comparers
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    using Neovolve.CodeAnalysis.ChangeTracking.Processors;

    public class InterfaceComparer : TypeComparer<IInterfaceDefinition>, IInterfaceComparer
    {
        public InterfaceComparer(IAccessModifiersComparer accessModifiersComparer, IFieldMatchProcessor fieldProcessor, IPropertyMatchProcessor propertyProcessor, IAttributeMatchProcessor attributeProcessor) : base(accessModifiersComparer, fieldProcessor, propertyProcessor, attributeProcessor)
        {
        }
    }
}