namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    using System.Collections.Generic;

    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;

    using Machine.Specifications.ReSharperProvider.Factories;

    class ContextElementHandler : IElementHandler
    {
        readonly ContextFactory _factory;

        public ContextElementHandler(ElementFactories factories)
        {
            this._factory = factories.Contexts;
        }

        public bool Accepts(ITreeNode element)
        {
            var declaration = element as IDeclaration;
            if (declaration == null)
            {
                return false;
            }

            return declaration.DeclaredElement.IsContext();
        }

        public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
        {
            var declaration = (IDeclaration)element;
            var context = this._factory.CreateContext(assemblyPath, declaration);

            if (context == null)
            {
                yield break;
            }

            yield return new UnitTestElementDisposition(context,
                                                        file.GetSourceFile().ToProjectFile(),
                                                        declaration.GetNameDocumentRange().TextRange,
                                                        declaration.GetDocumentRange().TextRange);
        }
    }
}