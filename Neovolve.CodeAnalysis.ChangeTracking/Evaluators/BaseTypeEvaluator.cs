namespace Neovolve.CodeAnalysis.ChangeTracking.Evaluators
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;
    
    public class BaseTypeEvaluator : Evaluator<IBaseTypeDefinition>, IBaseTypeEvaluator
    {
        protected override void FindMatches(IMatchAgent<IBaseTypeDefinition> agent)
        {
            agent.MatchOn(ExactSignature);
            agent.MatchOn(DifferentGenericTypes);
            agent.MatchOn(MovedType);
            agent.MatchOn(RenamedType);
            agent.MatchOn(ChangedTypeDefinition);
        }

        private static bool ExactSignature(IBaseTypeDefinition oldType, IBaseTypeDefinition newType)
        {
            return IsSameType(oldType, newType);
        }

        private static bool ChangedTypeDefinition(IBaseTypeDefinition oldType, IBaseTypeDefinition newType)
        {
            return IsSameType(oldType, newType, false);
        }

        private static bool DifferentGenericTypes(IBaseTypeDefinition oldType, IBaseTypeDefinition newType)
        {
            return IsSameType(oldType, newType, true, true, false);
        }

        private static bool IsSameType(IBaseTypeDefinition oldType, IBaseTypeDefinition newType,
            bool evaluateTypeDefinition = true, bool evaluateNamespace = true, bool evaluateGenericTypes = true,
            bool evaluateName = true)
        {
            oldType = oldType ?? throw new ArgumentNullException(nameof(oldType));
            newType = newType ?? throw new ArgumentNullException(nameof(newType));

            if (evaluateTypeDefinition && oldType.GetType() != newType.GetType())
            {
                // The types are different (for example one is a class and the other is an interface)
                return false;
            }

            if (evaluateNamespace && oldType.Namespace != newType.Namespace)
            {
                // Early exit if the namespace is different
                // No point running recursion to check if parent types match if the namespace is different
                return false;
            }

            if (evaluateName && oldType.RawName != newType.RawName)
            {
                // The names of the types are different
                return false;
            }

            if (evaluateGenericTypes 
                && oldType is IGenericTypeElement oldTypeDefinition
                && newType is IGenericTypeElement newTypeDefinition)
            {
                // Types are the same if they have the same name with the same number of generic type parameters
                // Check the number of generic type parameters first because if the number is different then it doesn't matter about the name
                // If it is a generic type then we need to parse the type parameters out to validate the name
                if (oldTypeDefinition.GenericTypeParameters.Count != newTypeDefinition.GenericTypeParameters.Count)
                {
                    return false;
                }
            }

            if (oldType.DeclaringType != null
                && newType.DeclaringType == null)
            {
                // The old type has a parent type but the new one doesn't, no match
                return false;
            }

            if (oldType.DeclaringType == null
                && newType.DeclaringType != null)
            {
                // The new type has a parent type but the old one doesn't, no match
                return false;
            }

            // Check the parent types
            if (oldType.DeclaringType != null
                && newType.DeclaringType != null)
            {
                if (IsSameType(oldType.DeclaringType, newType.DeclaringType, evaluateTypeDefinition, evaluateNamespace,
                    evaluateGenericTypes, evaluateName) == false)
                {
                    // The parent types don't match
                    return false;
                }
            }

            // At this point we either don't have parent types or the parent types match

            return true;
        }

        private static bool RenamedType(IBaseTypeDefinition oldBaseType, IBaseTypeDefinition newBaseType)
        {
            if (IsSameType(oldBaseType, newBaseType, evaluateNamespace: false) == false)
            {
                return false;
            }

            // Check that types have the same child members with the same signatures
            if (oldBaseType is IEnumDefinition oldEnum)
            {
                return IsRenamedEnum(oldEnum, (IEnumDefinition)newBaseType);
            }
            
            if (oldBaseType is ITypeDefinition oldType)
            {
                return IsRenamedType(oldType, (ITypeDefinition)newBaseType);
            }

            return false;
        }

        private static bool IsRenamedType(ITypeDefinition oldType, ITypeDefinition newType)
        {
            if (oldType.Methods.Count != newType.Methods.Count)
            {
                return false;
            }

            foreach (var oldMethod in oldType.Methods)
            {
                var newMethod = newType.Methods.FirstOrDefault(x => IsMatchingMethod(oldMethod, x));

                if (newMethod == null)
                {
                    return false;
                }
            }

            foreach (var oldProperty in oldType.Properties)
            {
                var newProperty = newType.Properties.FirstOrDefault(x => IsMatchingProperty(oldProperty, x));

                if (newProperty == null)
                {
                    return false;
                }
            }

            if (oldType is IClassDefinition oldClass)
            {
                var newClass = (IClassDefinition)newType;

                if (ConstructorsMatch(oldClass.Constructors, newClass.Constructors) == false)
                {
                    return false;
                }

                if (FieldsMatch(oldClass.Fields, newClass.Fields) == false)
                {
                    return false;
                }
            }

            if (oldType is IStructDefinition oldStruct)
            {
                var newStruct = (IStructDefinition)newType;

                if (ConstructorsMatch(oldStruct.Constructors, newStruct.Constructors) == false)
                {
                    return false;
                }

                if (FieldsMatch(oldStruct.Fields, newStruct.Fields) == false)
                {
                    return false;
                }
            }

            if (oldType.ChildTypes.Count != newType.ChildTypes.Count)
            {
                return false;
            }

            return false;
        }

        private static bool ConstructorsMatch(IReadOnlyCollection<IConstructorDefinition> oldConstructors, IReadOnlyCollection<IConstructorDefinition> newConstructors)
        {
            if (oldConstructors.Count != newConstructors.Count)
            {
                return false;
            }

            foreach (var oldConstructor in oldConstructors)
            {
                var newConstructor = newConstructors.FirstOrDefault(x => IsMatchingParameterSet(oldConstructor.Parameters, x.Parameters));

                if (newConstructor == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool FieldsMatch(IReadOnlyCollection<IFieldDefinition> oldFields, IReadOnlyCollection<IFieldDefinition> newFields)
        {
            if (oldFields.Count != newFields.Count)
            {
                return false;
            }

            foreach (var oldField in oldFields)
            {
                var newField = newFields.FirstOrDefault(x => x.Name == oldField.Name);

                if (newField == null)
                {
                    return false;
                }

                if (newField.ReturnType != oldField.ReturnType)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsMatchingProperty(IPropertyDefinition oldProperty, IPropertyDefinition newProperty)
        {
            if (oldProperty.Name != newProperty.Name)
            {
                return false;
            }

            if (oldProperty.ReturnType != newProperty.ReturnType)
            {
                return false;
            }

            if ((oldProperty.GetAccessor != null) != (newProperty.GetAccessor != null))
            {
                return false;
            }

            if ((oldProperty.SetAccessor != null) != (newProperty.SetAccessor != null))
            {
                return false;
            }

            if ((oldProperty.InitAccessor != null) != (newProperty.InitAccessor != null))
            {
                return false;
            }

            return true;
        }

        private static bool IsMatchingMethod(IMethodDefinition oldMethod, IMethodDefinition newMethod)
        {
            if (newMethod.Name != oldMethod.Name)
            {
                return false;
            }

            if (newMethod.ReturnType != oldMethod.ReturnType)
            {
                return false;
            }

            return IsMatchingParameterSet(oldMethod.Parameters, newMethod.Parameters);
        }

        private static bool IsMatchingParameterSet(IReadOnlyCollection<IParameterDefinition> oldParameterSet, IReadOnlyCollection<IParameterDefinition> newParameterSet)
        {
            if (newParameterSet.Count != oldParameterSet.Count)
            {
                return false;
            }

            var oldParameters = oldParameterSet.ToList();
            var newParameters = newParameterSet.ToList();

            for (var index = 0; index < newParameters.Count; index++)
            {
                var oldParameter = oldParameters[index];
                var newParameter = newParameters[index];

                if (oldParameter.Name != newParameter.Name)
                {
                    return false;
                }

                if (oldParameter.Type != newParameter.Name)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsRenamedEnum(IEnumDefinition oldEnum, IEnumDefinition newEnum)
        {
            if (oldEnum.Members.Count != newEnum.Members.Count)
            {
                return false;
            }

            foreach (var oldMember in oldEnum.Members)
            {
                var newMember = newEnum.Members.FirstOrDefault(x => x.Name == oldMember.Name);

                if (newMember == null)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool MovedType(IBaseTypeDefinition oldType, IBaseTypeDefinition newType)
        {
            return IsSameType(oldType, newType, true, false);
        }
    }
}