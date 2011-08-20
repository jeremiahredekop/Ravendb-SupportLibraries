using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb
{
    [Serializable]
    public class DocumentPlaceholderCollection<T> : IDocumentPlaceholderCollection
        where T : class, IDocument
    {

        [JsonIgnore]
        private Dictionary<int, DocumentPlaceholder<T>> _placeholdersDictionary =
            new Dictionary<int, DocumentPlaceholder<T>>();

        public DocumentPlaceholder<T>[] Items
        {
            get
            {
                return _placeholdersDictionary.Values.ToArray();
            }
            set
            {
                value = value ?? new DocumentPlaceholder<T>[] { };
                _placeholdersDictionary = value.ToDictionary(a => a.DocId);
            }
        }

        public void AddIfNew(T item)
        {
            if (_placeholdersDictionary.ContainsKey(item.Id)) return;

            var placeholder = DocumentPlaceholder<T>.GetPointer(item);
            _placeholdersDictionary.Add(item.Id, placeholder);
        }

        #region Implementation of IDocumentPlaceholderCollection

        void IDocumentPlaceholderCollection.AddIfNew(object item)
        {
            var castedItem = (T)item;
            AddIfNew(castedItem);
        }

        #endregion
    }
}