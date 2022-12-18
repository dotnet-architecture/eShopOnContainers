namespace Coupon.API.Dtos
{
    public interface IMapper<TResult, TEntity>
    {
        TResult Translate(TEntity entity);
    }
}
