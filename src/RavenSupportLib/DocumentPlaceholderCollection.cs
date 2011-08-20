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
            get { return _placeholdersDictionary.Values.ToArray(); }
            set
            {
                value = value ?? new DocumentPlaceholder<T>[] { };
                _placeholdersDictionary = value.ToDictionary(a => a.DocId);
            }
        }

        public void AddIfNew(int id)
        {
            if (_placeholdersDictionary.ContainsKey(id)) return;

            DocumentPlaceholder<T> placeholder = DocumentPlaceholder<T>.GetPointer(id);
            _placeholdersDictionary.Add(id, placeholder);
        }

        #region Implementation of IDocumentPlaceholderCollection

        void IDocumentPlaceholderCollection.AddIfNew(int id)
        {
            AddIfNew(id);
        }

        #endregion
    }
}