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
            CreatePlaceholderAndUpdateReverseCollection(T t, IDocumentPlaceholderCollection reverseCollection,
                                                                                         IDocument currentDoc)
        {
            UpdatePointerCollectionReverse(reverseCollection, currentDoc);
            return CreatePlaceholder(t);
        }

        private static void UpdatePointerCollectionReverse(IDocumentPlaceholderCollection reverseCollection,
                                                           IDocument currentDoc)
        {
            reverseCollection.AddIfNew(currentDoc);
        }

        public static DocumentPlaceholder<T> CreatePlaceholder(T t)
        {
            return GetPointer(t);
        }


        public static DocumentPlaceholder<T> CreatePlaceholderAndReverse(T t, IDocumentPlaceholder reversePlaceholder,
                                                                         IDocument currentDoc)
        {
            UpdatePeerPointerReverse(reversePlaceholder, currentDoc);
            return CreatePlaceholder(t);
        }

        private static void UpdatePeerPointerReverse(IDocumentPlaceholder reversePlaceholder,
                                                     IDocument currentDoc)
        {
            if (reversePlaceholder == null) throw new ArgumentNullException("reversePlaceholder");
            if (currentDoc == null) throw new ArgumentNullException("currentDoc");


            reversePlaceholder.DocId = currentDoc.Id;
            reversePlaceholder.Name = GetNameFromDocumentUsingReflection(currentDoc);
        }

        internal static DocumentPlaceholder<T> GetPointer(T t)
        {
            var documentPlaceholder = new DocumentPlaceholder<T> { DocId = t.Id, Name = GetNameFromDocumentUsingReflection(t) };
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