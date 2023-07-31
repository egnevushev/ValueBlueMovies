using MongoDB.Bson;

namespace WebApi.Validation;

public static class ObjectIdValidator
{
    public static bool IsValidObjectId(string id) => ObjectId.TryParse(id, out _);
}