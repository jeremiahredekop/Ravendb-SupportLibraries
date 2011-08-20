using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace GeniusCode.RavenDb.Referential
{
    
    public class DocumentPlaceholderCollection<T, TData> : IDocumentPlaceholderCollection
        where T : IDocument
        where TData : IDocumentPointerData
    {
        
        
        [JsonIgnore]
        private Dictionary<int, DocumentPlaceholder<T, TData>> _placeholdersDictionary =
            new Dictionary<int, DocumentPlaceholder<T, TData>>();

        public DocumentPlaceholder<T, TData>[] Items
        {
            get { return _placeholdersDictionary.Values.ToArray(); }
            set
            {
                value = value ?? new DocumentPlaceholder<T, TData>[] { };
                _placeholdersDictionary = value.ToDictionary(a => a.DocId);
            }
        }

        public void AddIfNew(int targetId, TData targetData)
        {
            if (_placeholdersDictionary.ContainsKey(targetId)) return;

            DocumentPlaceholder<T, TData> placeholder = DocumentPlaceholder<T, TData>.GetPointer(targetId, targetData);
            _placeholdersDictionary.Add(targetId, placeholder);
        }

        #region Implementation of IDocumentPlaceholderCollection

        void IDocumentPlaceholderCollection.AddIfNew(int targetId, IDocumentPointerData targetData)
        {
            AddIfNew(targetId, (TData)targetData);
        }

        #endregion
    }
}