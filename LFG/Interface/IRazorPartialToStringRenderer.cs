using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LFG.Interface
{
  public interface IRazorPartialToStringRenderer
  {
    Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model);
    Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model, ViewDataDictionary viewData);
  }
}