using Machine.Specifications.ReSharperProvider.Presentation;

namespace Machine.Specifications.ReSharperProvider.Explorers.ElementHandlers
{
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.UnitTestFramework;
    using Machine.Specifications.ReSharperProvider.Factories;
    using System.Collections.Generic;

    class BehaviorElementHandler : IElementHandler
    {
        readonly BehaviorFactory _factory;
        readonly BehaviorSpecificationFactory _behaviorSpecifications;

        public BehaviorElementHandler(ElementFactories factories)
        {
            this._factory = factories.Behaviors;
            this._behaviorSpecifications = factories.BehaviorSpecifications;
        }

        public bool Accepts(ITreeNode element)
        {
            var declaration = element as IDeclaration;
            if (declaration == null)
            {
                return false;
            }

            return declaration.DeclaredElement.IsBehavior();
        }

        public IEnumerable<UnitTestElementDisposition> AcceptElement(string assemblyPath, IFile file, ITreeNode element)
        {
            IDeclaration declaration = (IDeclaration)element;
            BehaviorElement behavior = this._factory.CreateBehavior(declaration.DeclaredElement);

            if (behavior == null)
            {
                yield break;
            }

            var projectFile = file.GetSourceFile().ToProjectFile();
            var behaviorTextRange = declaration.GetNameDocumentRange().TextRange;
            var behaviorContainingRange = declaration.GetDocumentRange().TextRange;
            yield return new UnitTestElementDisposition(behavior,
                                                        projectFile,
                                                        behaviorTextRange,
                                                        behaviorContainingRange);

            var behaviorContainer = declaration.DeclaredElement.GetFirstGenericArgument();
            if (!behaviorContainer.IsBehaviorContainer())
            {
                yield break;
            }

            foreach (var field in behaviorContainer.Fields)
            {
                if (!field.IsSpecification())
                {
                    continue;
                }

                BehaviorSpecificationElement behaviorSpecification = this._behaviorSpecifications.CreateBehaviorSpecification(behavior, field);

                yield return new UnitTestElementDisposition(behaviorSpecification,
                                                            projectFile,
                                                            behaviorTextRange,
                                                            behaviorContainingRange);
            }
        }
    }
}