using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProjectAdj
{
    public class OriginalTypesRewriter : CSharpSyntaxRewriter
    {
        private ILog Logger = LogManager.GetLogger(typeof(ProjectMapper));

        private IDictionary<string, List<CompareTypeResult>> _typesMap;
        private string _docName;

        public OriginalTypesRewriter(string docName,  IDictionary<string, List<CompareTypeResult>> typesMap)
        {
            _docName = docName;
            _typesMap = typesMap;
        }

        private CompareTypeResult GetIdentifierCompareTypeResult(string identifier)
        {
            List<CompareTypeResult> list;
            if (_typesMap.TryGetValue(identifier, out list))
            {
                if (list.Count > 1)
                {
                    // Which one is the correct type?
                    Logger.Warn($"Found more refactored types for original type {identifier} in doc {_docName}: {string.Join(" - ", list.Select(x => x.NewFullTypeName))}. Using the first one!");
                }
                return list.First();
            }
            return null;
        }

        private CompareTypeResult GetQualifiedIdentifierCompareTypeResult(string left, string right)
        {
            List<CompareTypeResult> list;
            if (_typesMap.TryGetValue(right, out list))
            {
                var exactMatches = 
                    list
                    .Where(x => x.OriginalNamespace == left)
                    .ToList();

                if (exactMatches.Count == 0)
                {
                    return null;
                }
                if (exactMatches.Count > 1)
                {
                    // Which one is the correct type? Get the first one but it should be manually checked!
                    Logger.Warn($"Found more refactored types for original type {left}.{right} in doc {_docName}: {string.Join(" - ", exactMatches.Select(x => x.NewFullTypeName))}. Using the first one!");
                }
                return list.First();
            }
            return null;
        }

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node != null)
            {
                CompareTypeResult compareTypeResult = GetIdentifierCompareTypeResult(node.ToString());
                if (compareTypeResult != null)
                {
                    var newnode = node.WithIdentifier(SyntaxFactory.Identifier($" {compareTypeResult.NewType} "));
                    return newnode;
                }
                return base.VisitIdentifierName(node);
            }
            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
        {
            if (node != null)
            {
                var left = node.Left.ToString();
                var right = node.Right.ToString();

                CompareTypeResult compareTypeResult = GetQualifiedIdentifierCompareTypeResult(left, right);

                if (compareTypeResult != null)
                {
                    var newnode =
                        node
                        .WithLeft(SyntaxFactory.ParseName(compareTypeResult.NewNamespace))
                        .WithRight(SyntaxFactory.IdentifierName($"{compareTypeResult.NewType} "));

                    return newnode;
                }

                return base.VisitQualifiedName(node);                
            }
            return base.VisitQualifiedName(node);
        }

        private CompareTypeResult GetChangedMethodInfo(MemberAccessExpressionSyntax memberAccess)
        {
            string typeFullName = memberAccess.Expression.ToString();
            string methodName = memberAccess.Name.ToString();

            List<CompareTypeResult> list;
            if (_typesMap.TryGetValue(methodName, out list))
            {
                return list.FirstOrDefault(m => $"{m.OriginalNamespace}.{m.OriginalType}".EndsWith(typeFullName));
            }
            else
            {
                return null;
            }
        }
    }
 }
