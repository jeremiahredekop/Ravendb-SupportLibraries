using System;
using Raven.Client;

namespace GeniusCode.RavenDb
{
    public interface IDocumentPointer
    {
        int Id { get; set; }
        string Key { get; set; }
        Type DocumentType { get; }
        object GetRelatedDocument(IDocumentSession session);
    }
}