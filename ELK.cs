﻿using ScyScaff.Core.Models.Parser;
using ScyScaff.Core.Models.Plugins;
using ScyScaff.Docker.Enums;
using ScyScaff.Docker.Models.Builder;
using ScyScaff.Docker.Models.Plugins;

namespace ScyScaffPlugin.ELK;

public class ELK : IGlobalWorkerTemplatePlugin, IDockerCompatible
{
    public string Name => "elk";
    
    public Dictionary<string, string[]> SupportedFlags { get; } = new();
    
    public IEnumerable<DockerComposeService> GetComposeServices(string projectName, IScaffolderEntity? entity, string serviceName, int serviceIndex)
    {
        List<DockerComposeService> dockerComposeServices = new();
        
        DockerComposeService elasticService = new DockerComposeService
        {
            Type = DockerComposeServiceType.GlobalWorker,
            Image = "elasticsearch:7.9.1",
            ContainerName = "elasticsearch",
            Ports = new Dictionary<int, int?>
            {
                { 9200, 9200 },
                { 9300, 9300 }
            },
            Volumes = new Dictionary<string, string>
            {
                { $"./{projectName}.Global/ELK/elasticsearch.yml", "/usr/share/elasticsearch/config/elasticsearch.yml" },
                { "es_data", "/usr/share/elasticsearch/data/" }
            },
            EnvironmentVariables = new Dictionary<string, string>
            {
                { "discovery.type", "single-node" },
                { "http.host", "0.0.0.0" },
                { "transport.host", "0.0.0.0" },
                { "xpack.security.enabled", "false" },
                { "xpack.monitoring.enabled", "false" },
                { "cluster.name", "elasticsearch" },
                { "bootstrap.memory_lock", "true" }
            }
        };
        
        DockerComposeService logstashService = new DockerComposeService
        {
            Type = DockerComposeServiceType.GlobalWorker,
            Image = "logstash:7.9.1",
            ContainerName = "logstash",
            Ports = new Dictionary<int, int?>
            {
                { 8080, 8080 },
                { 9600, 9600 }
            },
            Volumes = new Dictionary<string, string>
            {
                { $"./{projectName}.Global/ELK/logstash.conf", "/usr/share/logstash/pipeline/logstash.conf" },
                { "ls_data", "/usr/share/logstash/data" }
            }
        };
        
        DockerComposeService kibanaService = new DockerComposeService
        {
            Type = DockerComposeServiceType.GlobalWorker,
            Image = "kibana:7.9.1",
            ContainerName = "kibana",
            Ports = new Dictionary<int, int?>
            {
                { 5601, 5601 }
            },
            Volumes = new Dictionary<string, string>
            {
                { $"./{projectName}.Global/ELK/kibana.yml", "/usr/share/kibana/config/kibana.yml" },
                { "kb_data", "/usr/share/kibana/data" }
            }
        };
        
        dockerComposeServices.Add(elasticService);
        dockerComposeServices.Add(logstashService);
        dockerComposeServices.Add(kibanaService);
        
        return dockerComposeServices;
    }
}
