using System;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb.Referential
{
    public class DocumentPlaceholder<T, TData> : IDocumentPlaceholder
        where T : IDocument
        where TData : IDocumentPointerData
    {
        [JsonIgnore]
        private readonly DocumentPointer<T> _docTypePointer = new DocumentPointer<T>();

        IDocumentPointerData IDocumentPlaceholder.Data
        {
            get { return Data; }
            set { Data = (TData)value; }
        }

        public string DocKey
        {
            get { return _docTypePointer.Key; }
            set { _docTypePointer.Key = value; }
        }

        [JsonIgnore]
        public int DocId
        {
            get { return _docTypePointer.Id; }
            set { _docTypePointer.Id = value; }
        }


        public TData Data { get; set; }

        public static DocumentPlaceholder<T, TData>
            CreatePlaceholderAndUpdateReverseCollection(int targetId, TData targetData, IDocumentPlaceholderCollection reverseCollection,
                                                        int currentDocId, IDocumentPointerData currentData)
        {
            UpdatePointerCollectionReverse(reverseCollection, currentDocId, currentData);
            return CreatePlaceholder(targetId, targetData);
        }

        private static void UpdatePointerCollectionReverse(IDocumentPlaceholderCollection reverseCollection,
                                                           int currentDocId, IDocumentPointerData currentData)
        {
            reverseCollection.AddIfNew(currentDocId, currentData);
        }

        public static DocumentPlaceholder<T, TData> CreatePlaceholder(int targetId, TData targetData)
        {
            return GetPointer(targetId, targetData);
        }


        public static DocumentPlaceholder<T, TData> CreatePlaceholderAndReverse(int targetId, TData targetData,
                                                                         IDocumentPlaceholder targetPlaceholder,
                                                                         int currentDocId, IDocumentPointerData currentData)
        {
            UpdatePeerPointerReverse(targetPlaceholder, currentDocId, currentData);
            return CreatePlaceholder(targetId, targetData);
        }

        private static void UpdatePeerPointerReverse(IDocumentPlaceholder targetPlaceholder,
                                                     int currentDocId, IDocumentPointerData currentData)
        {
            if (targetPlaceholder == null) throw new ArgumentNullException("targetPlaceholder");
            targetPlaceholder.DocId = currentDocId;

            if (currentData != null)
                targetPlaceholder.Data = currentData;
        }

        internal static DocumentPlaceholder<T, TData> GetPointer(int targetId, TData targetData)
        {
            var documentPlaceholder = new DocumentPlaceholder<T, TData> { DocId = targetId, Data = targetData };
            return documentPlaceholder;
        }
    }
}