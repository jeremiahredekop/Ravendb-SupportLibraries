using System;
using System.Reflection;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb
{
    public class DocumentPlaceholder<T> : IDocumentPlaceholder where T : IDocument
    {
        [JsonIgnore]
        public readonly DocumentPointer<T> DocTypePointer = new DocumentPointer<T>();

        #region IDocumentPlaceholder Members

        public string DocKey
        {
            get { return DocTypePointer.Key; }
            set { DocTypePointer.Key = value; }
        }

        [JsonIgnore]
        public int DocId
        {
            get { return DocTypePointer.Id; }
            set { DocTypePointer.Id = value; }
        }

        public string Name { get; set; }

        #endregion

        public static DocumentPlaceholder<T>
            CreatePlaceholderAndUpdateReverseCollection(int targetId, IDocumentPlaceholderCollection reverseCollection,
                                                        int currentDocId)
        {
            UpdatePointerCollectionReverse(reverseCollection, currentDocId);
            return CreatePlaceholder(targetId);
        }

        private static void UpdatePointerCollectionReverse(IDocumentPlaceholderCollection reverseCollection,
                                                           int currentDocId)
        {
            reverseCollection.AddIfNew(currentDocId);
        }

        public static DocumentPlaceholder<T> CreatePlaceholder(int id)
        {
            return GetPointer(id);
        }


        public static DocumentPlaceholder<T> CreatePlaceholderAndReverse(int targetId, IDocumentPlaceholder reversePlaceholder,
                                                                         int currentDocId)
        {
            UpdatePeerPointerReverse(reversePlaceholder, currentDocId);
            return CreatePlaceholder(targetId);
        }

        private static void UpdatePeerPointerReverse(IDocumentPlaceholder reversePlaceholder,
                                                     int currentDocId)
        {
            if (reversePlaceholder == null) throw new ArgumentNullException("reversePlaceholder");
            reversePlaceholder.DocId = currentDocId;
        }

        internal static DocumentPlaceholder<T> GetPointer(int id)
        {
            var documentPlaceholder = new DocumentPlaceholder<T> { DocId = id };
            return documentPlaceholder;
        }

        private static string GetNameFromDocumentUsingReflection(IDocument t)
        {
            PropertyInfo info = t.GetType().GetProperty("Name",
                                                        BindingFlags.Instance | BindingFlags.GetProperty |
                                                        BindingFlags.Public);

            if (info == null) return string.Empty;

            return info.GetValue(t, null) as string;
        }
    }
}