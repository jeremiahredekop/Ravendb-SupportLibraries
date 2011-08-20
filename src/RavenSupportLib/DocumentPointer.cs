using System;
using System.Text.RegularExpressions;
using Raven.Client;
using Raven.Client.Util;

namespace GeniusCode.RavenDb
{
    public sealed class DocumentPointer<T> : IDocumentPointer
    {
        #region Helpers

        private readonly string _keyRootName;
        private int _id;
        private string _key;

        #endregion

        public DocumentPointer()
            : this((Inflector.Pluralize(typeof(T).Name)))
        {
        }

        public DocumentPointer(string keyRootName)
        {
            _keyRootName = keyRootName;
        }

        #region IDocumentPointer Members

        public int Id
        {
            get { return _id; }
            set { SetKeyFromId(value); }
        }

        public string Key
        {
            get { return _key; }
            set { SetIdFromKey(value); }
        }

        public Type DocumentType
        {
            get { return typeof(T); }
        }


        object IDocumentPointer.GetRelatedDocument(IDocumentSession session)
        {
            return GetRelatedDocument(session);
        }

        #endregion

        private void SetKeyFromId(int value)
        {
            _id = value;
            string newKey = IdToKey(_id, _keyRootName);
            _key = newKey;
        }

        private void SetIdFromKey(string value)
        {
            _key = value;
            int newId = KeyToId(_key, _keyRootName);
            _id = newId;
        }

        public static int KeyToId(string key, string keyPath)
        {
            if (String.IsNullOrEmpty(key))
                return 0;

            key = key.Replace("//", "/");

            string stringValue = Regex.Match(key, String.Format("({0}/)([0-9]*)", keyPath)).Groups[2].Value;

            int value = Int32.Parse(stringValue);
            return value;
        }

        public static string IdToKey(int id, string keyPath)
        {
            if (id == 0)
                return String.Empty;

            if (keyPath.EndsWith("/"))
                keyPath = keyPath.Remove(keyPath.Length - 1);

            return String.Format("{1}/{0}", id, keyPath);
        }

        public T GetRelatedDocument(IDocumentSession session)
        {
            return session.Load<T>(Key);
        }
    }
}