﻿using LFG.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace LFG.Service
{
  public class RazorPartialToStringRenderer : IRazorPartialToStringRenderer
  {
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpContextAccessor _contextAccessor;
    public RazorPartialToStringRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider,
        IHttpContextAccessor contextAccessor)
    {
      _viewEngine = viewEngine;
      _tempDataProvider = tempDataProvider;
      _serviceProvider = serviceProvider;
      _contextAccessor = contextAccessor;
    }
    public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model)
    {
      var actionContext = GetActionContext();
      var partial = FindView(actionContext, partialName);
      await using var output = new StringWriter();
      var viewContext = new ViewContext(
          actionContext,
          partial,
          new ViewDataDictionary<TModel>(
              metadataProvider: new EmptyModelMetadataProvider(),
              modelState: new ModelStateDictionary())
          {
            Model = model
          },
          new TempDataDictionary(
              actionContext.HttpContext,
              _tempDataProvider),
          output,
          new HtmlHelperOptions()
      );
      await partial.RenderAsync(viewContext);
      return output.ToString();
    }

    public async Task<string> RenderPartialToStringAsync<TModel>(string partialName, TModel model, ViewDataDictionary viewData)
    {
      var actionContext = GetActionContext();
      var partial = FindView(actionContext, partialName);
      await using var output = new StringWriter();
      var viewContext = new ViewContext(
          actionContext,
          partial,
          new ViewDataDictionary<TModel>(viewData)
          {
            Model = model
          },
          new TempDataDictionary(
              actionContext.HttpContext,
              _tempDataProvider),
          output,
          new HtmlHelperOptions()
      );
      await partial.RenderAsync(viewContext);
      return output.ToString();
    }

    private IView FindView(ActionContext actionContext, string partialName)
    {
      var getPartialResult = _viewEngine.GetView(null, partialName, false);
      if (getPartialResult.Success)
      {
        return getPartialResult.View;
      }
      var findPartialResult = _viewEngine.FindView(actionContext, partialName, false);
      if (findPartialResult.Success)
      {
        return findPartialResult.View;
      }
      var searchedLocations = getPartialResult.SearchedLocations.Concat(findPartialResult.SearchedLocations);
      var errorMessage = string.Join(
          Environment.NewLine,
          new[] { $"Unable to find partial '{partialName}'. The following locations were searched:" }.Concat(searchedLocations)); ;
      throw new InvalidOperationException(errorMessage);
    }
    private ActionContext GetActionContext()
    {
      return new ActionContext(_contextAccessor.HttpContext, _contextAccessor.HttpContext.GetRouteData(), new ActionDescriptor());
    }
  }
}
