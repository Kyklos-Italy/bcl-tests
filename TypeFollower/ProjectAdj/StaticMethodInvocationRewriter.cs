using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProjectAdj
{
    public class StaticMethodInvocationRewriter : CSharpSyntaxRewriter
    {
        private static ILog Logger = LogManager.GetLogger(typeof(StaticMethodInvocationRewriter));
        private IDictionary<string, List<CompareMethodResult>> _listMethodMap;

        public StaticMethodInvocationRewriter(IDictionary<string, List<CompareMethodResult>> methodMap)
        {
            _listMethodMap = methodMap;
        }

        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
            {
                CompareMethodResult comparation = GetChangedMethodInfo(node.Expression as MemberAccessExpressionSyntax);
                if (comparation != null)
                {
                    var newnode = node.WithExpression(SyntaxFactory.ParseExpression($"{comparation.NewNamespace}.{comparation.NewType}.{comparation.NewMethodName}"));
                    Logger.Debug($"{node.ToFullString()} --> {newnode.ToFullString()}");
                    return newnode;
                }
            }
            return base.VisitInvocationExpression(node);
        }

        private CompareMethodResult GetChangedMethodInfo(MemberAccessExpressionSyntax memberAccess)
        {
            string typeFullName = memberAccess.Expression.ToString();
            string methodName = memberAccess.Name.ToString();

            List<CompareMethodResult> list;
            if (_listMethodMap.TryGetValue(methodName, out list))
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
