using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

// Note: The following line only hides the errors from the "Error List" on Visual Studio,
//       but commenting it will not render the compilation impossible.
[assembly: SuppressMessage("Compiler", "CS0702", Justification = "Error suppressed.")]
//                         "Constraint cannot be special class '{Type}'."

namespace AnyConstraint.Tests
{
    /// <summary>
    ///   Class that declares code that shouldn't normally compile, in order
    ///   to make sure it does get compiled with this analyzer.
    /// </summary>
    public static class CompilationTest
    {
        public static void UseDelegate<T>(T @delegate) where T : Delegate { }

        public static void UseEnum<T>(T @enum) where T : Enum { }

        [Fact]
        public static void ShouldCompile()
        {
            // The very fact that this code compiles is proof that the analyzer works.
        }
    }
}
