using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Commands;
using Raven.Client;

namespace RavenSupportLib
{
    public class MigrationHost
    {
        private readonly IDocumentStore _Store;
        public MigrationHost(IDocumentStore store)
        {
            _Store = store;
            Actions = new List<MigrationAction>();
        }

        public void PerformActions()
        {
            var query = from a in Actions
                        from c in GetCommandsForMigrationAction(a)
                        select c;

            var cmds = query.ToArray();

            if (cmds.Any())
                _Store.DatabaseCommands.Batch(cmds);

        }

        private List<ICommandData> GetCommandsForMigrationAction(MigrationAction action)
        {
            var loop = new AyendesJSonQueryResultsLoop(_Store,
                   action.IndexName,
                   action.QueryContents);
            var commands = loop.GetCommandToModifyJSONItems(action.GetReplacement);

            return commands;


        }

        public List<MigrationAction> Actions { get; private set; }


    }
}
