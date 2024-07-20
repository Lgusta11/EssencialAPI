namespace WebAPIUdemy.Pagination.Filters;

public class ProductsFilterPrice :  QueryStringParameters
{
    public decimal? Price { get; set; }
    public string? PriceCriterion { get; set; }
}
