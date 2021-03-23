using System;
using NServiceBus;

namespace Common.Configuration
{
    public static class CommonConfiguration
    {
        public static void ApplyEndpointConfiguration(this EndpointConfiguration endpointConfiguration,
            Action<TransportExtensions<LearningTransport>> messageEndpointMappings = null)
        {
            endpointConfiguration.Conventions()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"))
                .DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Replys"));

            TransportExtensions<LearningTransport> transport = endpointConfiguration.UseTransport<LearningTransport>();
            PersistenceExtensions<LearningPersistence> persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
            persistence.SagaStorageDirectory(@"..\..\..\..\.sagas\");

            endpointConfiguration.AuditProcessedMessagesTo("audit");
            endpointConfiguration.SendFailedMessagesTo("error");

            MetricsOptions metrics = endpointConfiguration.EnableMetrics();
            metrics.SendMetricDataToServiceControl(
                serviceControlMetricsAddress: "Particular.Monitoring",
                interval: TimeSpan.FromSeconds(2));

            endpointConfiguration.SendHeartbeatTo(
                serviceControlQueue: "Particular.ServiceControl",
                frequency: TimeSpan.FromSeconds(15),
                timeToLive: TimeSpan.FromSeconds(30));

            endpointConfiguration.AuditSagaStateChanges(
                serviceControlQueue: "audit");

            messageEndpointMappings?.Invoke(transport);
        }
    }
}