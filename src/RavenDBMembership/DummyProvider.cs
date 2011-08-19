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
            MyDocument = Doit();
        }

        private static IDocumentStore Doit()
        {
            var documentStore = new DocumentStore
            {
                Url = "http://localhost:8080"
            }.Initialize();

            return documentStore;
        }

        private static IEnumerable<IDocumentStore> Doits()
        {
            var item = Doit();
            return new[] { item };
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
                var item = (TService)Doit();
                return new TService[] { item };
            }
            return null;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return Doits();
        }

        public TService GetInstance<TService>(string key)
        {
            return (TService)Doit();
        }

        public TService GetInstance<TService>()
        {
            return (TService)Doit();
        }

        public object GetInstance(Type serviceType, string key)
        {
            return Doit();
        }

        public object GetInstance(Type serviceType)
        {
            return Doit();
        }

        public object GetService(Type serviceType)
        {
            return Doit();
        }
    }
}
