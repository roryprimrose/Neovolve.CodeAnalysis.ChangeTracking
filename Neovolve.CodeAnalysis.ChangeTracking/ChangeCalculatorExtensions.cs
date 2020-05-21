namespace Neovolve.CodeAnalysis.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;

    public static class ChangeCalculatorExtensions
    {
        public static Task<ChangeCalculatorResult> CalculateChange(this IChangeCalculator calculator,
            IEnumerable<string> oldCode,
            IEnumerable<string> newCode)
        {
            return CalculateChange(calculator, oldCode, newCode, default);
        }

        public static async Task<ChangeCalculatorResult> CalculateChange(this IChangeCalculator calculator,
            IEnumerable<string> oldCode,
            IEnumerable<string> newCode, CancellationToken cancellationToken)
        {
            Ensure.Any.IsNotNull(calculator, nameof(calculator));
            Ensure.Any.IsNotNull(oldCode, nameof(oldCode));
            Ensure.Any.IsNotNull(newCode, nameof(newCode));

            // Convert all the old and new code into SyntaxNode objects
            var oldTask = ParseCode(oldCode, cancellationToken);
            var newTask = ParseCode(newCode, cancellationToken);

            await Task.WhenAll(oldTask, newTask).ConfigureAwait(false);

            var oldNodes = oldTask.Result;
            var newNodes = newTask.Result;

            return calculator.CalculateChanges(oldNodes, newNodes);
        }

        private static async Task<IEnumerable<SyntaxNode>> ParseCode(IEnumerable<string> code,
            CancellationToken cancellationToken)
        {
            var syntaxTrees = code.Select(x => CSharpSyntaxTree.ParseText(x));
            var tasks = syntaxTrees.Select(x => x.GetRootAsync(cancellationToken)).ToList();

            await Task.WhenAll(tasks).ConfigureAwait(false);

            return tasks.Select(x => x.Result);
        }
    }
}