using System.Collections.Generic;
using System.Linq;
using GeniusCode.RavenDb.Loops;
using Raven.Abstractions.Commands;
using Raven.Client;

namespace GeniusCode.RavenDb.Migrations
{
    public class MigrationHost
    {
        private readonly IDocumentStore _store;

        public MigrationHost(IDocumentStore store)
        {
            _store = store;
            Actions = new List<IMigrationAction>();
        }

        public List<IMigrationAction> Actions { get; private set; }

        public void PerformActions()
        {
            var query = from a in Actions
                        from c in GetCommandsForMigrationAction(a)
                        select c;

            var cmds = query.ToArray();

            if (cmds.Any())
                _store.DatabaseCommands.Batch(cmds);
        }

        private IEnumerable<ICommandData> GetCommandsForMigrationAction(IMigrationAction action)
        {
            var loop = new AyendesJSonQueryResultsLoop(_store,
                                                       action.IndexName,
                                                       action.QueryContents);

            var commands = loop.GetCommandToModifyJSONItems(action.GetReplacement);
            return commands;
        }
    }
}