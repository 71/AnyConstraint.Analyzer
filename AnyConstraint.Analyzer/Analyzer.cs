using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AnyConstraint.Analyzer
{
    /// <summary>
    ///   <see cref="DiagnosticAnalyzer"/> that ensures the "CS0702" diagnostic is
    ///   suppressed.
    /// </summary>
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class AnyConstraintAnalyzer : DiagnosticAnalyzer
    {
        /// <summary>
        ///   <see cref="DiagnosticDescriptor"/> of the <see cref="Diagnostic"/> generated
        ///   if the created hook does not work.
        /// </summary>
        public static readonly DiagnosticDescriptor Error
            = new DiagnosticDescriptor("AC", "Error during initialization.", "Could not install hook on constraint checking: {0}", "Hooking", DiagnosticSeverity.Warning, true);

        /// <inheritdoc />
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
            = ImmutableArray.Create(Error);

        /// <inheritdoc />
        public override void Initialize(AnalysisContext context)
        {
            try
            {
                Hook.EnsureAllConstraintsAreValid();
            }
            catch (Exception e)
            {
                bool hasReported = false;

                context.RegisterSyntaxTreeAction(ctx =>
                {
                    if (hasReported)
                        return;

                    ctx.ReportDiagnostic(Diagnostic.Create(Error, Location.None, e.Message));

                    hasReported = true;
                });
            }
        }
    }
}
