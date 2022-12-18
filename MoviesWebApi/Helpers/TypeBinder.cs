using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace MoviesWebApi.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var propertyName = bindingContext.ModelName;
            var valueProvider = bindingContext.ValueProvider.GetValue(propertyName);
            if(valueProvider == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }
            try
            {
                var value = JsonConvert.DeserializeObject<T>(valueProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(value);
            }catch(Exception ex)
            {
                bindingContext.ModelState.TryAddModelError(propertyName, "Invalid value for List<int>");
            }
            return Task.CompletedTask;
        }
    }
}
