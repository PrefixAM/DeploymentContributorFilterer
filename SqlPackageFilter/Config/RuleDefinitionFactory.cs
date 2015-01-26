﻿using System;
using System.Collections.Generic;
using AgileSqlClub.SqlPackageFilter.Filter;
using AgileSqlClub.SqlPackageFilter.Rules;

namespace AgileSqlClub.SqlPackageFilter.Config
{
    public class RuleDefinitionFactory
    {
        private readonly IDisplayMessageHandler _messageHandler;

        public RuleDefinitionFactory(IDisplayMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public List<FilterRule> BuildRules(Dictionary<string, string> contextArgs, DeploymentFilter deploymentFilter)
        {
            var defintions = BuildRuleDefinitions(contextArgs, deploymentFilter);

            var rules = new List<FilterRule>();

            foreach (var ruleDefinition in defintions)
            {
                switch (ruleDefinition.FilterType)
                {
                    case FilterType.Schema:
                        rules.Add(new SchemaFilterRule(ruleDefinition.Operation, ruleDefinition.Match, ruleDefinition.MatchType ));
                        break;
                    case FilterType.Name:
                        rules.Add(new NamedObjectFilterRule(ruleDefinition.Operation, ruleDefinition.Match, ruleDefinition.MatchType));
                        break;
                    case FilterType.Type:
                        rules.Add(new ObjectTypeFilterRule(ruleDefinition.Operation, ruleDefinition.Match, ruleDefinition.MatchType));
                        break;

                }
            }

            return rules;

        }

        public IEnumerable<RuleDefinition> BuildRuleDefinitions(Dictionary<string, string> contextArgs, DeploymentFilter deploymentFilter)
        {
            var rules = new List<RuleDefinition>();

            foreach (var arg in contextArgs)
            {
                try
                {
                    var definitionType = FilterDefinitionTypeParser.GetDefinitionType(arg.Key);

                    if (definitionType == FilterDefinitionType.CommandLine)
                        rules.Add(new CommandLineFilterParser().GetDefinitions(arg.Value));

                    if (definitionType == FilterDefinitionType.XmlFile)
                        rules.AddRange(new XmlFilterParser(new FileGateway()).GetDefinitions(arg.Value));
                }
                catch (Exception )
                {
                    deploymentFilter.ShowMessage(string.Format("Error decoding command line arge: {0}", arg.Value));
                }
            }

            return rules;
        } 
    }
}