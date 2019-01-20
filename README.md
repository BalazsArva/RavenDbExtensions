# RavenDb Extensions

Extensions for RavenDb 4.1. **Not meant for production use**.

I develop these extensions - for now, at least - mainly to play around with the LINQ expressions API and generate some useful code while at it.

## The ConditionalPatch extension library

## The 'What'

RavenDB provides a convenient way for doing partial updates on documents in the form of [patch operations](https://ravendb.net/docs/article-page/4.1/Csharp/client-api/operations/patching/single-document). It also provides some useful extensions for patching arrays, etc.

What is not provided is a way to perform those operations only if some condition holds so that the condition is evaluated by the database. This library is developed to do that.

## The 'Why'

Certainly such conditional updates might be done so that we evaluate the condition in the client app, and determine whether or not the operation should be sent to the database. It would work in most cases, but it is troublesome if the system is highly concurrent and the document can change between loading it from the database, evaluating the condition and determining whether or not the document should be updated.

## The 'How'

Once done, this library will provide an extension which will look something like this:

    session.PatchIf(id, ...updates..., doc => doc.LastChangeId < currentChangeIdKnownToTheApp);
    
While the RavenDB Patch API exposes convenience C# methods for patching documents, JavaScript code can also be sent when dealing more advanced scenarios which cannot be completely handled by the out-of-the-box API. This library will translate the above expression to JavaScript code which will look something like this:

    if (this.LastChangeId < args.__param1) {
        this.LastChangeId = args.__param1;
        
        // Other property updates ...
    }
    
It will then use this script to send a custom, parameterited patch request to the database so the condition can be evaluated there instead of in the client app.
