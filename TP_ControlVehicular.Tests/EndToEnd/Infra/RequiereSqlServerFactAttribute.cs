using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace TP_ControlVehicular.Tests.EndToEnd.Infra
{
    [XunitTestCaseDiscoverer("TP_ControlVehicular.Tests.EndToEnd.Infra.RequiereSqlServerFactDiscoverer", "TP_ControlVehicular.Tests")]
    public class RequiereSqlServerFactAttribute : FactAttribute
    {
    }

    public class RequiereSqlServerFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public RequiereSqlServerFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var conn = SqlConnectionResolver.ResolveConnectionString();
            var reachable = SqlServerReachability.TryConnect(conn, out var ex);
            if (reachable)
            {
                return new[] { new XunitTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod) };
            }
            else
            {
                var reason = ex != null ? $"SQL Server no disponible: {ex.Message}" : "SQL Server no disponible";
                return new[] { new XunitSkippedDataRowTestCase(_diagnosticMessageSink, discoveryOptions.MethodDisplayOrDefault(), testMethod, reason) };
            }
        }
    }

    public class XunitSkippedDataRowTestCase : XunitTestCase
    {
        [Obsolete("Called by the de-serializer", true)]
        public XunitSkippedDataRowTestCase() { }

        public XunitSkippedDataRowTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay defaultMethodDisplay, ITestMethod testMethod, string skipReason)
            : base(diagnosticMessageSink, defaultMethodDisplay, testMethod, new object[0])
        {
            SkipReason = skipReason;
        }
    }
}