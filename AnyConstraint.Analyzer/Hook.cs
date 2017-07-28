using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ryder.Lightweight;

namespace AnyConstraint.Analyzer
{
    /// <summary>
    ///   Allows managing the hook placed on <c>Microsoft.CodeAnalysis.CSharp.Binder.IsValidConstraintType</c>. 
    /// </summary>
    internal static class Hook
    {
        private static RefCountedDisposable Redirection;

        /// <summary>
        ///   Returns <see langword="true"/>, no matter the input.
        /// </summary>
        private static bool IsValidConstraintType(TypeConstraintSyntax syntax, ITypeSymbol type, object diagnostics) => true;

        /// <summary>
        ///   Ensures the hook is in place, and returns a disposable that allows disabling it.
        /// </summary>
        public static IDisposable EnsureAllConstraintsAreValid()
        {
            if (Redirection != null)
                return Redirection.Clone();

            MethodInfo original = typeof(CSharpCompilation)
                .GetTypeInfo().Assembly
                .GetType("Microsoft.CodeAnalysis.CSharp.Binder")
                .GetTypeInfo().DeclaredMethods.First(x => x.Name == nameof(IsValidConstraintType));

            MethodInfo replacement = typeof(Hook)
                .GetTypeInfo().DeclaredMethods.First(x => x.Name == nameof(IsValidConstraintType));

            // Ensure the methods are jitted
            try
            {
                original.Invoke(null, new object[] { null, null, null });
                replacement.Invoke(null, new object[] { null, null, null });
            }
            catch
            {
                // Threw on purpose
            }

            return Redirection = new RefCountedDisposable(new Redirection(original, replacement, true));
        }

        /// <summary>
        ///   Represents a reference-counted <see cref="IDisposable"/> that calls
        ///   <see cref="IDisposable.Dispose"/> on its <see cref="Inner"/> disposable
        ///   when its <see cref="ReferenceCount"/> reaches 0.
        /// </summary>
        private sealed class RefCountedDisposable : IDisposable
        {
            private IDisposable Inner { get; }
            private int ReferenceCount { get; set; }

            public RefCountedDisposable(IDisposable inner)
            {
                Inner = inner;
                ReferenceCount = 1;
            }

            /// <summary>
            ///   Returns a clone of this <see cref="RefCountedDisposable"/>
            ///   with its reference count increased.
            /// </summary>
            public IDisposable Clone()
            {
                ReferenceCount++;
                return this;
            }

            /// <summary>
            ///   Decrements the reference count, and disposes the inner <see cref="IDisposable"/>
            ///   if needed.
            /// </summary>
            public void Dispose()
            {
                if (--ReferenceCount == 0)
                    Inner.Dispose();
            }
        }
    }
}
