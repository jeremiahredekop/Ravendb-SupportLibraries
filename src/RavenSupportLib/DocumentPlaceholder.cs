using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb
{

    public interface IDocumentPlaceholder
    {
        string DocKey { get; }
        [JsonIgnore]
        int DocId { get; set; }

        string Name { get; set; }
    }

    public class DocumentPlaceholder<T> : IDocumentPlaceholder where T : IDocument
    {

        [JsonIgnore]
        public readonly DocumentPointer<T> DocTypePointer = new DocumentPointer<T>();

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


        public static DocumentPlaceholder<T> CreateFrom(T t, Func<T, IDocumentPlaceholderCollection> reverseFunc, IDocument currentDoc)
        {
            var documentPlaceholder = GetPointer(t);



            if (reverseFunc != null && currentDoc != null)
            {
                var collection = reverseFunc(t);
                collection.AddIfNew(currentDoc);
            }

            return documentPlaceholder;
        }



        public static DocumentPlaceholder<T> CreateFrom(T t, Func<T, IDocumentPlaceholder> reverseFunc, IDocument currentDoc)
        {
            var documentPlaceholder = GetPointer(t);


            UpdatePeerPointerReverse(t, reverseFunc, currentDoc);

            return documentPlaceholder;
        }

        private static void UpdatePeerPointerReverse(T t, Func<T, IDocumentPlaceholder> reverseFunc, IDocument currentDoc)
        {
            if (reverseFunc == null || currentDoc == null) return;

            var placeholder = reverseFunc(t);
            placeholder.DocId = currentDoc.Id;
            placeholder.Name = GetNameFromDocumentUsingReflection(currentDoc);
        }

        internal static DocumentPlaceholder<T> GetPointer(T t)
        {
            var documentPlaceholder = new DocumentPlaceholder<T> { DocId = t.Id, Name = GetNameFromDocumentUsingReflection(t) };

            return documentPlaceholder;
        }

        private static string GetNameFromDocumentUsingReflection(IDocument t)
        {
            var info = t.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public);

            if (info == null) return string.Empty;

            return info.GetValue(t, null) as string;
        }
    }

    public class DocumentPlaceholderCollection<T> : IDocumentPlaceholderCollection, IEnumerable<DocumentPlaceholder<T>> where T : class, IDocument
    {
        [JsonIgnore]
        private Dictionary<int, DocumentPlaceholder<T>> _placeholdersDictionary = new Dictionary<int, DocumentPlaceholder<T>>();


        private List<DocumentPlaceholder<T>> _items = new List<DocumentPlaceholder<T>>();
        public DocumentPlaceholder<T>[] Items
        {
            get { return _items.ToArray(); }
            set
            {
                _items = value.ToList();
                _placeholdersDictionary = _items.ToDictionary(a => a.DocId);
            }
        }

        public void AddIfNew(T item)
        {
            if (!_placeholdersDictionary.ContainsKey(item.Id))
            {
                var placeholder = DocumentPlaceholder<T>.GetPointer(item);
                _placeholdersDictionary.Add(item.Id, placeholder);
            }

        }

        #region Implementation of IEnumerable

        IEnumerator<DocumentPlaceholder<T>> IEnumerable<DocumentPlaceholder<T>>.GetEnumerator()
        {
            return _placeholdersDictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _placeholdersDictionary.Values.GetEnumerator();
        }

        #endregion

        #region Implementation of IDocumentPlaceholderCollection

        void IDocumentPlaceholderCollection.AddIfNew(object item)
        {
            var castedItem = (T)item;
            AddIfNew(castedItem as T);
        }

        #endregion
    }

    public interface IDocumentPlaceholderCollection
    {
        void AddIfNew(object item);
    }
}