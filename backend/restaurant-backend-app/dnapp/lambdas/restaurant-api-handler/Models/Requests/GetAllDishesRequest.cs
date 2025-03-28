using Function.Models.Dishes;

namespace Function.Models.Requests;

public class GetAllDishesRequest
{
    public FilterEnum? DishTypeEnum { get; set; }
    
    public SortEnum? SortBy { get; set; }
}