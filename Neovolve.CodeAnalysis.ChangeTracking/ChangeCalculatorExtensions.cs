﻿namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Neovolve.CodeAnalysis.ChangeTracking.Models;

    public static class ChangeCalculatorExtensions
    {
        public static Task<ChangeCalculatorResult> CalculateChanges(
            this IChangeCalculator calculator,
            IEnumerable<CodeSource> oldCode,
            IEnumerable<CodeSource> newCode,
            CancellationToken cancellationToken)
        {
            return CalculateChanges(calculator, oldCode, newCode, ComparerOptions.Default, cancellationToken);
        }

        public static async Task<ChangeCalculatorResult> CalculateChanges(
            this IChangeCalculator calculator,
            IEnumerable<CodeSource> oldCode,
            IEnumerable<CodeSource> newCode,
            ComparerOptions options,
            CancellationToken cancellationToken)
        {
            calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
            oldCode = oldCode ?? throw new ArgumentNullException(nameof(oldCode));
            newCode = newCode ?? throw new ArgumentNullException(nameof(newCode));

            // Convert all the old and new code into SyntaxNode objects
            var oldTask = ParseCode(oldCode, cancellationToken);
            var newTask = ParseCode(newCode, cancellationToken);

            await Task.WhenAll(oldTask, newTask).ConfigureAwait(false);

            var oldNodes = oldTask.Result;
            var newNodes = newTask.Result;

            return CalculateChanges(calculator, oldNodes, newNodes, options);
        }

        public static ChangeCalculatorResult CalculateChanges(
            this IChangeCalculator calculator,
            IEnumerable<SyntaxNode> oldNodes,
            IEnumerable<SyntaxNode> newNodes)
        {
            var options = ComparerOptions.Default;

            return CalculateChanges(calculator, oldNodes, newNodes, options);
        }

        public static ChangeCalculatorResult CalculateChanges(
            this IChangeCalculator calculator,
            IEnumerable<SyntaxNode> oldNodes,
            IEnumerable<SyntaxNode> newNodes,
            ComparerOptions options)
        {
            calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
            oldNodes = oldNodes ?? throw new ArgumentNullException(nameof(oldNodes));
            newNodes = newNodes ?? throw new ArgumentNullException(nameof(newNodes));
            options = options ?? throw new ArgumentNullException(nameof(options));

            var oldTypes = ResolveDeclaredTypes(oldNodes);
            var newTypes = ResolveDeclaredTypes(newNodes);

            return calculator.CalculateChanges(oldTypes, newTypes, options);
        }

        private static async Task<IEnumerable<SyntaxNode>> ParseCode(
            IEnumerable<CodeSource> sources,
            CancellationToken cancellationToken)
        {
            var syntaxTrees = sources.Select(x => CSharpSyntaxTree.ParseText(x.Contents, null, x.FilePath));
            var tasks = syntaxTrees.Select(x => x.GetRootAsync(cancellationToken)).FastToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.Select(x => x.Result);
        }

        private static IEnumerable<IBaseTypeDefinition> ResolveDeclaredTypes(IEnumerable<SyntaxNode> nodes)
        {
            var resolvedTypes = new List<IBaseTypeDefinition>();

            foreach (var node in nodes)
            {
                var matches = ResolveDeclaredTypes(node);

                if (matches.Count > 0)
                {
                    resolvedTypes.AddRange(matches);
                }
            }

            return resolvedTypes;
        }

        private static IList<IBaseTypeDefinition> ResolveDeclaredTypes(SyntaxNode node)
        {
            List<IBaseTypeDefinition> resolvedTypes = new();

            if (node is ClassDeclarationSyntax classNode)
            {
                var typeDefinition = new ClassDefinition(classNode);

                resolvedTypes.Add(typeDefinition);
            }
            else if (node is StructDeclarationSyntax structNode)
            {
                var typeDefinition = new StructDefinition(structNode);

                resolvedTypes.Add(typeDefinition);
            }
            else if (node is InterfaceDeclarationSyntax interfaceNode)
            {
                var typeDefinition = new InterfaceDefinition(interfaceNode);

                resolvedTypes.Add(typeDefinition);
            }
            else if (node is EnumDeclarationSyntax enumNode)
            {
                var typeDefinition = new EnumDefinition(enumNode);

                resolvedTypes.Add(typeDefinition);
            }
            else
            {
                // Recursively search all the child nodes
                var childNodes = node.ChildNodes();

                foreach (var childNode in childNodes)
                {
                    var matches = ResolveDeclaredTypes(childNode);

                    if (matches.Count > 0)
                    {
                        resolvedTypes.AddRange(matches);
                    }
                }
            }

            return resolvedTypes;
        }
    }
}