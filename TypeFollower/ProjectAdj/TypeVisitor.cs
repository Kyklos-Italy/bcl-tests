using System;
using System.Text;
using Microsoft.CodeAnalysis;

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
}
