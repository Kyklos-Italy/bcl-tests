using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProjectAdj
{
    public class NamedTypeVisitor : SymbolVisitor
    {
        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            Console.WriteLine(symbol);

            foreach (var childSymbol in symbol.GetMembers())
            {
                //We must implement the visitor pattern ourselves and 
                //accept the child symbols in order to visit their children
                childSymbol.Accept(this);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            Console.WriteLine(symbol);

            foreach (var childSymbol in symbol.GetTypeMembers())
            {
                //Once againt we must accept the children to visit 
                //all of their children
                childSymbol.Accept(this);
            }
        }
    }


    public static class CustomSymbolFinder
    {
        public static IList<INamedTypeSymbol> GetAllSymbols(this Compilation compilation)
        {
            var visitor = new FindAllSymbolsVisitor();
            visitor.Visit(compilation.GlobalNamespace);
            return visitor.AllTypeSymbols;
        }

        private class FindAllSymbolsVisitor : SymbolVisitor
        {
            public List<INamedTypeSymbol> AllTypeSymbols { get; } = new List<INamedTypeSymbol>();

            public override void VisitNamespace(INamespaceSymbol symbol)
            {
                Parallel.ForEach(symbol.GetMembers(), s => s.Accept(this));
            }

            public override void VisitNamedType(INamedTypeSymbol symbol)
            {
                AllTypeSymbols.Add(symbol);
                foreach (var childSymbol in symbol.GetTypeMembers())
                {
                    base.Visit(childSymbol);
                }
            }
        }

        public static IList<INamedTypeSymbol> ExcludeSystemTypes(this IEnumerable<INamedTypeSymbol> usedTypes)
        {
            const string system = "System";
            return
                usedTypes
                .Where(t => t.ContainingNamespace.Name != system && !t.ToString().StartsWith(system))
                .ToList();
        }


        public static async Task<IList<INamedTypeSymbol>> GetAllUsedTypesInCompilations(this IEnumerable<Compilation> compilations)
        {
            List<INamedTypeSymbol> usedTypes = new List<INamedTypeSymbol>();

            foreach (var compilation in compilations)
            {
                usedTypes.AddRange(await compilation.GetAllUsedTypesInCompilation().ConfigureAwait(false));
            }

            return
                usedTypes
                .Distinct()
                .ToList();
        }

        public static async Task<IList<INamedTypeSymbol>> GetAllUsedTypesInCompilation(this Compilation compilation)
        {
            List<INamedTypeSymbol> usedTypes = new List<INamedTypeSymbol>();

            foreach (var st in compilation.SyntaxTrees)
            {
                usedTypes.AddRange(await compilation.GetUsedTypesInSyntaxTree(st).ConfigureAwait(false));
            }

            return
                usedTypes
                .Distinct()
                .ToList();
        }

        public static async Task<IList<INamedTypeSymbol>> GetUsedTypesInSyntaxTree(this Compilation compilation, SyntaxTree syntaxTree)
        {
            List<INamedTypeSymbol> namedTypeSymbols = new List<INamedTypeSymbol>();

            var root = await syntaxTree.GetRootAsync();
            var nodes = root.DescendantNodes(n => true);

            var st = root.SyntaxTree;
            var sm = compilation.GetSemanticModel(st);

            if (nodes != null)
            {
                var syntaxNodes = nodes as SyntaxNode[] ?? nodes.ToArray();

                // IdentifierNameSyntax:
                //  - var keyword
                //  - identifiers of any kind (including type names)
                var namedTypes = 
                    syntaxNodes
                    .OfType<IdentifierNameSyntax>()
                    .Select(id => sm.GetSymbolInfo(id).Symbol)
                    .OfType<INamedTypeSymbol>()
                    .Where(x => x.Kind == SymbolKind.NamedType)
                    .ToArray();

                namedTypeSymbols.AddRange(namedTypes);

                // ExpressionSyntax:
                //  - method calls
                //  - property uses
                //  - field uses
                //  - all kinds of composite expressions
                var expressionTypes = 
                    syntaxNodes
                    .OfType<ExpressionSyntax>()
                    .Select(ma => sm.GetTypeInfo(ma).Type)
                    .OfType<INamedTypeSymbol>()
                    .Where(x => x.Kind == SymbolKind.NamedType)
                    .ToArray();

                namedTypeSymbols.AddRange(expressionTypes);
            }

            return namedTypeSymbols.Distinct().ToList();
        }
    }
}
