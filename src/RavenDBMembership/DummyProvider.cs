using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Raven.Client;
using Raven.Client.Document;

namespace RavenDBMembership
{
    public class DummyProvider : IServiceLocator
    {

        public static IDocumentStore MyDocument;

        static DummyProvider()
        {
            MyDocument = doit();
        }

        private static IDocumentStore doit()
        {
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8080"
            }.Initialize();

            return documentStore;
        }

        private static IEnumerable<IDocumentStore> doits()
        {
            var item = doit();
            return new IDocumentStore[] { item };
        }

        public static ServiceLocatorProvider DelegateItem
        {
            get
            {
                return () => new DummyProvider();
            }
        }


        public IEnumerable<TService> GetAllInstances<TService>()
        {
            if (typeof(TService) == typeof(IDocumentStore))
            {
                var item = (TService)doit();
                return new TService[] { item };
            }
            return null;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return doits();
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService)doit();
        }

        public TService GetInstance<TService>()
        {
            return (TService)doit();
        }

        public object GetInstance(Type serviceType, string key)
        {
            return doit();
        }

        public object GetInstance(Type serviceType)
        {
            return doit();
        }

        public object GetService(Type serviceType)
        {
            return doit();
        }
    }
}
