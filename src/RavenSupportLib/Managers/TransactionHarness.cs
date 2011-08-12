using System;
using GeniusCode.Framework.Support.Delegates;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{

    public abstract class TransactionHarness<TSession, TMetaData>
        where TSession : class
        where TMetaData : class, ILinqMetaData, new()
    {
        public TransactionHarness(IAdapterService adapterService, Func<TSession> sessionInfoFunc, EntityManagerSource<TSession, TMetaData> source)
        {
            _AdapterService = adapterService;
            _SessionInfoFunc = sessionInfoFunc;
            Source = source;
        }

        readonly IAdapterService _AdapterService;
        readonly Func<TSession> _SessionInfoFunc;
        protected readonly EntityManagerSource<TSession, TMetaData> Source;

        #region Virtual Members

        protected virtual bool HandleException(string message, Exception ex)
        {
            return false;
        }

        #endregion



        #region Shortcuts - Read / Write

        protected T PerformReadUsingManager<T, TManager>(string message, Func<TManager, T> action)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            T output = InvokeOnManager<T, TManager>(message, action);
            return output;
        }

        protected DeferredFunc<TManager> PerformReadUsingManager<TManager>(string message)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            // Cache Manager Type, Return type will be deferred until called
            Func<Func<TManager, object>, object> inputCache = (returnDelegate) =>
            {
                object result = null;

                var msg = "PerformReadUsingManager {0}: {1}".FormatString(typeof(TManager).Name, message);

                this.InvokeOnScope(msg, (scope) =>
                {
                    var manager = Source.GetDataManager<TManager>(scope);
                    result = returnDelegate(manager);
                });

                return result;
            };

            return new DeferredFunc<TManager>(inputCache);
        }


        protected void PerformWriteUsingManager<TManager>(string transactionName, Action<TManager> action)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            InvokeWithTransactionOnManager<TManager>(
                transactionName,
                action);
        }

        protected T PerformWriteUsingManager<T, TManager>(string transactionName, Func<TManager, T> action)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            return InvokeWithTransactionOnManager<T, TManager>(
                transactionName,
                action);
        }

        protected DeferredFunc<TManager> PerformWriteUsingManager<TManager>(string transactionName)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            // Cache Manager Type, Return type will be deferred until called
            Func<Func<TManager, object>, object> inputCache = (returnDelegate) =>
            {
                object result = null;

                var msg = "PerformWriteUsingManager {0}: {1}".FormatString(typeof(TManager).Name, transactionName);

                this.InvokeWithTransactionOnScope(msg, (scope) =>
                {
                    var manager = Source.GetDataManager<TManager>(scope);
                    result = returnDelegate(manager);
                });

                return result;
            };

            return new DeferredFunc<TManager>(inputCache);
        }

        #endregion



        #region Helpers

        private DataScope<TSession, TMetaData> BuildDataScope(IDataAccessAdapter adapter)
        {
            var session = this._SessionInfoFunc();
            var scope = Source.GetDataScope(adapter, session);
            return scope;
        }

        #region Workhorse methods

        /// <summary>
        /// Invoke on Scope with Transaction.
        /// </summary>
        protected void InvokeWithTransactionOnScope(string transactionName, Action<DataScope<TSession, TMetaData>> toPerform)
        {
            using (IDataAccessAdapter adapter = _AdapterService.CreateAdapter())
            {
                try
                {
                    _AdapterService.StartTransaction(adapter, transactionName);

                    var scope = BuildDataScope(adapter);

                    PerformActionsOnScope(_AdapterService, toPerform, scope);

                    _AdapterService.FinishTransaction(adapter);
                }
                catch (Exception ex)
                {
                    adapter.Rollback();
                    if (!HandleException(transactionName, ex))
                        throw ex;
                }
                finally
                {
                    adapter.Dispose();
                }
            }
        }

        /// <summary>
        /// Invoke on Scope. Transaction will not be used, unless IAdapterService has Before or After actions to perform.
        /// </summary>
        protected void InvokeOnScope(string message, Action<DataScope<TSession, TMetaData>> toPerform)
        {
            var requiresTransaction = _AdapterService.Calc_HasActions();

            if (requiresTransaction)
                InvokeWithTransactionOnScope("auto", toPerform);
            else
                InvokeOnScopeWithoutTransaction(message, toPerform);
        }

        private void InvokeOnScopeWithoutTransaction(string message, Action<DataScope<TSession, TMetaData>> toPerform)
        {
            using (IDataAccessAdapter adapter = _AdapterService.CreateAdapter())
            {
                try
                {
                    var scope = BuildDataScope(adapter);

                    PerformActionsOnScope(_AdapterService, toPerform, scope);
                }
                catch (Exception ex)
                {
                    if (!HandleException(message, ex))
                        throw ex;
                }
                finally
                {
                    adapter.Dispose();
                }
            }
        }

        private static void PerformActionsOnScope(IAdapterService adapterService, Action<DataScope<TSession, TMetaData>> toPerform, DataScope<TSession, TMetaData> scope)
        {
            var before = adapterService.GetBeforeAction();
            if (before != null)
                before(scope.Adapter);

            toPerform(scope);

            var after = adapterService.GetAfterAction();
            if (after != null)
                after(scope.Adapter);
        }

        #endregion

        #region Invoke on Managers

        private void InvokeOnManager<TManager>(string message, Action<TManager> toTry)
        where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            InvokeOnScope(message, scope =>
            {
                TManager manager = GetManager<TManager>(scope);
                toTry(manager);
            });
        }

        private T InvokeOnManager<T, TManager>(string message, Func<TManager, T> toTry)
        where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            T output = default(T);
            Action<TManager> wrapper = m => output = toTry(m);
            InvokeOnManager<TManager>(message, wrapper);
            return output;
        }

        private void InvokeWithTransactionOnManager<TManager>(string transactionName, Action<TManager> actionAgainstManager)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            InvokeWithTransactionOnScope(transactionName, (scope) =>
            {
                TManager managerToUse = GetManager<TManager>(scope);
                actionAgainstManager(managerToUse);
            });
        }

        private T InvokeWithTransactionOnManager<T, TManager>(string transactionName, Func<TManager, T> toTry)
        where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            T output = default(T);
            Action<TManager> wrapper = m => output = toTry(m);
            InvokeWithTransactionOnManager<TManager>(transactionName, wrapper);
            return output;
        }

        #endregion



        #endregion



        #region Get Manager Shortcuts

        protected TManager GetManager<TManager>(DataScope<TSession, TMetaData> scope)
            where TManager : class, IDataManager<TSession, TMetaData>, new()
        {
            return Source.GetDataManager<TManager>(scope);
        }

        protected IEntityManager<TSession, TMetaData> GetManagerForEntity<TEntity>(DataScope<TSession, TMetaData> scope)
            where TEntity : EntityBase2, IEntity2, new()
        {
            return Source.GetManagerForEntity<TEntity>(scope);
        }
        #endregion


    }
}
