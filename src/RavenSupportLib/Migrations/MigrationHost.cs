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
            Actions = new List<MigrationAction>();
        }

        public List<MigrationAction> Actions { get; private set; }

        public void PerformActions()
        {
            IEnumerable<ICommandData> query = from a in Actions
                                              from c in GetCommandsForMigrationAction(a)
                                              select c;

            ICommandData[] cmds = query.ToArray();

            if (cmds.Any())
                _store.DatabaseCommands.Batch(cmds);
        }

        private IEnumerable<ICommandData> GetCommandsForMigrationAction(MigrationAction action)
        {
            var loop = new AyendesJSonQueryResultsLoop(_store,
                                                       action.IndexName,
                                                       action.QueryContents);
            List<ICommandData> commands = loop.GetCommandToModifyJSONItems(action.GetReplacement);

            return commands;
        }
    }
}