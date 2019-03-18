using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ProjectAdj
{
    public class StaticMethodInvocationRewriter : CSharpSyntaxRewriter
    {
        ILog Logger = LogManager.GetLogger(typeof(ProjectMapper));
        List<CompareMethodResult> _listMethodMap;

        public StaticMethodInvocationRewriter(List<CompareMethodResult> methodMap)
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
            var changedMethodList = _listMethodMap.Where(m => m.IsChanged());
            //var identifiers = expression.DescendantNodes().OfType<IdentifierNameSyntax>();
            string typeFullName = memberAccess.Expression.ToString();
            string methodName = memberAccess.Name.ToString();
            return changedMethodList.FirstOrDefault(m => methodName == m.OriginalMethodName && $"{m.OriginalNamespace}.{m.OriginalType}".EndsWith(typeFullName));
        }
    }
 }
