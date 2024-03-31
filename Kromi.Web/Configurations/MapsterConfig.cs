namespace Kromi.Web.Configurations
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            // Configure Product to ProductDto mapping
            //TypeAdapterConfig<Product, ProductDto>.NewConfig()

            //    .Map(dest => dest.Id, src => src.Id)

            //    .Map(dest => dest.Name, src => src.Name)

            //    .Map(dest => dest.ReleaseDate, src => src.ReleaseDate.ToString("yyyy-MM-dd"))

            //    // direct list mapping
            //    .Map(dest => dest.Tags, src => src.Tags)

            //    // nested objects mapping
            //    .Map(dest => dest.Reviews, src => src.Reviews.Adapt<List<ReviewDto>>())

            //    .Map(dest => dest.Price, src => src.Price);

            //// Configure Review to ReviewDto mapping
            //TypeAdapterConfig<Review, ReviewDto>.NewConfig()

            //    .Map(dest => dest.ReviewerName, src => src.ReviewerName)

            //    .Map(dest => dest.Content, src => src.Content);

            //// Any other mappings
        }
    }
}
