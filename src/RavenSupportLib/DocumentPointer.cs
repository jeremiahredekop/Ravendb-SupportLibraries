using System;
using System.Text.RegularExpressions;
using Raven.Client;
using System.Linq;
using Raven.Client.Indexes;

namespace RavenSupportLib
{

    public sealed class DocumentPointer<T> : IDocumentPointer
    {

        public DocumentPointer()
            : this((typeof(T).Name.ToLower() + "s"))
        {
        }

        public DocumentPointer(string keyRootName)
        {
            _KeyRootName = keyRootName;
        }

        private readonly string _KeyRootName;
        public static int KeyToId(string key, string keyPath )
        {
            if (String.IsNullOrEmpty(key))
                return 0;

            key = key.Replace("//", "/");

            string stringValue = Regex.Match(key, String.Format("({0}/)([0-9]*)", keyPath)).Groups[2].Value;

            var value = Int32.Parse(stringValue);
            return value;
        }

        public static string IdToKey(int id,string keyPath)
        {
            if (id == 0)
                return String.Empty;

            if (keyPath.EndsWith("/"))
                keyPath = keyPath.Remove(keyPath.Length -1);

            return String.Format("{1}/{0}", id, keyPath);
        }


        private int _Id;
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;

                var newKey = IdToKey(_Id, _KeyRootName);

                if (newKey != _Key)
                    _Key = newKey;
            }
        }
        private string _Key;
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
                var newId = KeyToId(_Key, _KeyRootName);

                if (_Id != newId)
                    _Id = newId;
            }
        }
        public Type DocumentType
        {
            get
            {
                return typeof(T);
            }
        }

        public T GetRelatedDocument(IDocumentSession session)
        {
            return session.Load<T>(Key);
        }



        object IDocumentPointer.GetRelatedDocument(IDocumentSession session)
        {
            return this.GetRelatedDocument(session);
        }
    }
}
