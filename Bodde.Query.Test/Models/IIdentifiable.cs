namespace Bodde.Query.Test.Models;

public interface IIdentifiable<T>
{
    public T Id { get; set; }
}