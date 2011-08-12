using System.Collections.Generic;
using Raven.Client.Linq;
using Raven.Abstractions.Commands;
using Raven.Json.Linq;
using Raven.Client;
using System;
using System.Linq;
using Raven.Client.Indexes;

namespace RavenSupportLib
{
    public static class IDocumentSessionExtensions
    {

        public static PutCommandData ToPutCommandData(this RavenJObject itemToSave)
        {
            var output = new PutCommandData
            {
                Document = itemToSave,
                Metadata = itemToSave.Value<RavenJObject>("@metadata"),
                Key = itemToSave.Value<RavenJObject>("@metadata").Value<string>("@id")
            };

            return output;
        }

        public static T GetRelatedDocument<T>(this IRavenRepository session, DocumentPointer<T> pointer)
        {
            return session.GetById<T>(pointer.Id);
        }

        public static T GetRelatedDocument<T>(this IDocumentSession session, DocumentPointer<T> pointer)
        {
            return session.Load<T>(pointer.Id);
        }

        public static List<T> GetAll<T>(this IRavenQueryable<T> input)
        {
            var q = new AyendesObjectLoop<T>(input);
            return q.GetAll();
        }
    }
}