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

        private IDictionary<string, List<CompareTypeResult>> _listTypeMap;

        public OriginalTypesRewriter(IDictionary<string, List<CompareTypeResult>> typeMap)
        {
            _listTypeMap = typeMap;
        }

        //public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        //{
        //    if (node.Expression.Kind() == SyntaxKind.SimpleMemberAccessExpression)
        //    {
        //        CompareMethodResult comparation = GetChangedMethodInfo(node.Expression as MemberAccessExpressionSyntax);
        //        if (comparation != null)
        //        {
        //   //         ReplacedMethods.Add(comparation);
        //            var newnode = node.WithExpression(SyntaxFactory.ParseExpression($"{comparation.NewNamespace}.{comparation.NewType}.{comparation.NewMethodName}"));
        //            Logger.Debug($"{node.ToFullString()} --> {newnode.ToFullString()}");
        //            return newnode;
        //        }
        //    }
        //    return base.VisitInvocationExpression(node);
        //}

        //public override SyntaxNode VisitTypeCref(TypeCrefSyntax node)
        //{
        //    return base.VisitTypeCref(node);
        //}

        //public override SyntaxNode VisitRefType(RefTypeSyntax node)
        //{
        //    return base.VisitRefType(node);
        //}

        //public override SyntaxNode VisitArrayType(ArrayTypeSyntax node)
        //{
        //    return base.VisitArrayType(node);
        //}

        //public override SyntaxNode VisitRefTypeExpression(RefTypeExpressionSyntax node)
        //{
        //    return base.VisitRefTypeExpression(node);
        //}

        //public override SyntaxNode VisitTypeParameterList(TypeParameterListSyntax node)
        //{
        //    return base.VisitTypeParameterList(node);
        //}

        //public override SyntaxNode VisitDeclarationExpression(DeclarationExpressionSyntax node)
        //{
        //    return base.VisitDeclarationExpression(node);
        //}

        //public override SyntaxNode Visit(SyntaxNode node)
        //{
        //    if (node != null && node.ToString().Contains("FTPSClient"))
        //    {
        //        var c = node;
        //    }
        //    return base.Visit(node);
        //}

        public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
        {
            if (node != null && node.ToString().Contains("FTPSClient"))
            {
                var newnode = node.WithIdentifier(SyntaxFactory.Identifier(" FtpsClient "));
                return newnode;
            }
            return base.VisitIdentifierName(node);
        }

        public override SyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
        {
            if (node != null && node.ToString().Contains("FTPSClient"))
            {
                var newnode = 
                    node
                    .WithLeft(SyntaxFactory.ParseName("Kyklos.Kernel.Ftp.Ftps"))
                    .WithRight(SyntaxFactory.IdentifierName("FtpsClient"));

                return newnode;
            }
            return base.VisitQualifiedName(node);
        }

        private CompareTypeResult GetChangedMethodInfo(MemberAccessExpressionSyntax memberAccess)
        {
            string typeFullName = memberAccess.Expression.ToString();
            string methodName = memberAccess.Name.ToString();

            List<CompareTypeResult> list;
            if (_listTypeMap.TryGetValue(methodName, out list))
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
