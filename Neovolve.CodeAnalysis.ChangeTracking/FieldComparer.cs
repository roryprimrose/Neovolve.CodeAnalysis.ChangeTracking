namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public class FieldComparer : MemberComparer<IFieldDefinition>, IFieldComparer
    {
        public FieldComparer(IAttributeMatchProcessor attributeProcessor) : base(attributeProcessor)
        {
        }
    }
}