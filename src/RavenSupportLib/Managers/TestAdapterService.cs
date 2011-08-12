using System;
using System.Data;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public sealed class TestAdapterService : IAdapterService
    {
        readonly Func<IDataAccessAdapter> _AdapterFunc;
        readonly IsolationLevel _IsolationLevel;

        public TestAdapterService(Func<IDataAccessAdapter> adapterFunc)
            : this(IsolationLevel.ReadUncommitted, adapterFunc)
        {
        }

        private TestAdapterService(IsolationLevel isolationLevelToUse, Func<IDataAccessAdapter> adapterFunc)
        {
            _AdapterFunc = adapterFunc;
            _IsolationLevel = isolationLevelToUse;

            ResetActionsOnceCalled = true;
        }

        public IDataAccessAdapter CreateAdapter()
        {
            return _AdapterFunc();
        }

        public void StartTransaction(IDataAccessAdapter adapter, string transactionName)
        {
            if (!adapter.IsTransactionInProgress)
                adapter.StartTransaction(_IsolationLevel, transactionName);
        }

        public void FinishTransaction(IDataAccessAdapter adapter)
        {
            if (adapter.IsTransactionInProgress)
                adapter.Rollback();
        }

        public bool ResetActionsOnceCalled { get; set; }

        public Action<IDataAccessAdapter> BeforeAction { get; set; }
        public Action<IDataAccessAdapter> AfterAction { get; set; }

        Action<IDataAccessAdapter> IAdapterService.GetBeforeAction()
        {
            var output = BeforeAction;

            if (ResetActionsOnceCalled)
                BeforeAction = null;

            return output;
        }

        Action<IDataAccessAdapter> IAdapterService.GetAfterAction()
        {
            var output = AfterAction;

            if (ResetActionsOnceCalled)
                AfterAction = null;

            return output;
        }


        public bool Calc_HasActions()
        {
            return BeforeAction != null || AfterAction != null;
        }
    }
}
