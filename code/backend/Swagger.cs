namespace backend;

public static class Swagger {
    public static void InitSwagger(IServiceCollection services) {
        services.AddMvc();
        services.AddSwaggerGen();
    }
}
