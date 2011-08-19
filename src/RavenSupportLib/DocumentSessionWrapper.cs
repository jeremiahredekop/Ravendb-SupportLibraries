using System;
using System.Transactions;
using Raven.Client;
using Raven.Client.Document;

namespace GeniusCode.RavenDb
{
    public sealed class DocumentSessionWrapper : IDisposable
    {
        #region Fields

        private readonly IDocumentStore _Store;
        private readonly bool _disposeStore;

        #endregion

        #region Constructors

        public DocumentSessionWrapper(IDocumentStore store)
        {
            _Store = store;
        }

        public DocumentSessionWrapper(string url)
        {
            _Store = new DocumentStore {Url = url};
            _Store.Initialize();
            _disposeStore = true;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_disposeStore)
                _Store.Dispose();
        }

        #endregion

        public void DoOnSessionWithSave(Action<IDocumentSession> sessionAction)
        {
            bool save = true;
            DoOnSession(sessionAction, save);
        }

        public void DoOnSession(Action<IDocumentSession> sessionAction, bool save = false)
        {
            Action<IDocumentSession, IDocumentStore> action = (ses, sto) => sessionAction(ses);
            DoOnStore(action, save);
        }

        public void DoOnStoreWithSave(Action<IDocumentSession, IDocumentStore> sessionAction)
        {
            DoOnStore(sessionAction, true);
        }

        public void DoOnRepositoryWithSave(Action<IRavenRepository> repAction)
        {
            bool save = true;
            DoOnRepository(repAction, save);
        }

        public void DoOnRepository(Action<IRavenRepository> repAction, bool save = false)
        {
            Action<IDocumentSession, IDocumentStore> action = (ses, sto) =>
                                                                  {
                                                                      IRavenRepository rep = new RavenRepository(ses,
                                                                                                                 true);
                                                                      repAction(rep);
                                                                  };

            DoOnStore(action, save);
        }

        public void DoOnStore(Action<IDocumentSession, IDocumentStore> sessionAction, bool save = false)
        {
            if (save)
                Perform_DoInTransaction(sessionAction);
            else
                Perform_Do(sessionAction);
        }

        #region helpers

        private void Perform_Do(Action<IDocumentSession, IDocumentStore> sessionAction)
        {
            using (IDocumentSession session = _Store.OpenSession())
            {
                sessionAction(session, _Store);
            }
        }

        private void Perform_DoInTransaction(Action<IDocumentSession, IDocumentStore> sessionAction)
        {
            using (var t = new TransactionScope())
            {
                using (IDocumentSession session = _Store.OpenSession())
                {
                    sessionAction(session, _Store);
                    session.SaveChanges();
                }
                t.Complete();
            }
        }

        #endregion
    }
}