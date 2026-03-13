using System.Data;
using System.Runtime.CompilerServices;

namespace TicketSystem.Common.Entity.Result;

public class Result
{
    public static Success Success => default;
    public static Created Created => default;
    public static Deleted Deleted => default;
    public static Updated Updated => default;

}

public class Result <TValue>
{
   private TValue? _value  ; 
   private readonly List<Error> _errors;
   public bool IsSuccess {get;} =false;
    
    private Result(Error error)
    {
        _errors = [error];
        IsSuccess =false;
    }
    private Result(List<Error> errors)
    {
        _errors = errors;
        IsSuccess =false;
    }
    private Result(TValue value)
    {
        if(value is null)
        {
            IsSuccess = false;
            throw new NoNullAllowedException();
        }
        _value = value;
        IsSuccess =true;
    }
    public static implicit operator Result<TValue>(TValue value)=> new(value);
    public static implicit operator Result<TValue>(Error error)=> new(error);
    public static implicit operator Result<TValue>(List<Error> errors)=> new(errors);
        


}
public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Deleted;
public readonly record struct Updated;