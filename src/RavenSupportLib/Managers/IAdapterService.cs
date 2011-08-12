using System;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public interface IAdapterService
    {
        IDataAccessAdapter CreateAdapter();
        void StartTransaction(IDataAccessAdapter adapter, string transactionName);
        /// <summary>
        /// This might not be a Commit (if in test mode, for example)
        /// </summary>
        /// <param name="adapter"></param>
        void FinishTransaction(IDataAccessAdapter adapter);


        /// <summary>
        /// Please do not call the Harness on this action.  Intended to use directly against the database.
        /// If calls are made back to the harness, then db locks will occur
        /// </summary>
        Action<IDataAccessAdapter> GetBeforeAction();
        /// <summary>
        /// Please do not call the Harness on this action.  Intended to use directly against the database.
        /// If calls are made back to the harness, then db locks will occur
        /// </summary>
        Action<IDataAccessAdapter> GetAfterAction();
        
        
        bool Calc_HasActions();
    }
}
