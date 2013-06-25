using System;
using Raven.Client;

namespace SessionTracker.Web
{
    public static class DocumentSessionExtensions {

        public static string GetStringId<T>(this IDocumentSession session, ValueType id) {

            return session.Advanced.DocumentStore.Conventions
                .DefaultFindFullDocumentKeyFromNonStringIdentifier(id, typeof(T), false);
        }

        public static string GetStringId<T>(this IAsyncDocumentSession session, ValueType id) {

            return session.Advanced.DocumentStore.Conventions
                .DefaultFindFullDocumentKeyFromNonStringIdentifier(id, typeof(T), false);
        }
    }
}