using System;
using SD.LLBLGen.Pro.ORMSupportClasses;
using System.Data;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public sealed class StandardAdapterService : IAdapterService
    {
        readonly Func<IDataAccessAdapter> _AdapterFunc;
        readonly IsolationLevel _IsolationLevel;

        public StandardAdapterService(IsolationLevel isolationLevelToUse, Func<IDataAccessAdapter> adapterFunc)
        {
            _AdapterFunc = adapterFunc;
            _IsolationLevel = isolationLevelToUse;
        }

        public IDataAccessAdapter CreateAdapter()
        {
            return _AdapterFunc();
        }

        public void StartTransaction(IDataAccessAdapter adapter, string transactionName)
        {
            adapter.StartTransaction(_IsolationLevel, transactionName);
        }

        public void FinishTransaction(IDataAccessAdapter adapter)
        {
            adapter.Commit();
        }


        Action<IDataAccessAdapter> IAdapterService.GetBeforeAction()
        {
            return null;
        }

        Action<IDataAccessAdapter> IAdapterService.GetAfterAction()
        {
            return null;
        }


        public bool Calc_HasActions()
        {
            return false;
        }
    }
}
