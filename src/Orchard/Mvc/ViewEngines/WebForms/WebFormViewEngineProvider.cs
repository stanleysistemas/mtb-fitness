﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Orchard.DisplayManagement.Descriptors.ShapeTemplateStrategy;
using Orchard.Logging;

namespace Orchard.Mvc.ViewEngines.WebForms {
    public class WebFormViewEngineProvider : IViewEngineProvider, IShapeTemplateViewEngine {
        public WebFormViewEngineProvider() {
            Logger = NullLogger.Instance;
        }

        static readonly string[] DisabledFormats = new[] { "~/Disabled" };

        public ILogger Logger { get; set; }

        public IViewEngine CreateThemeViewEngine(CreateThemeViewEngineParams parameters) {
            // Area: if "area" in RouteData. Url hit for module...
            // Area-Layout Paths - no-op because LayoutViewEngine uses multi-pass instead of layout paths
            // Area-View Paths - no-op because LayoutViewEngine relies entirely on Partial view resolution
            // Area-Partial Paths - enable theming views associated with a module based on the route

            // Layout Paths - no-op because LayoutViewEngine uses multi-pass instead of layout paths
            // View Paths - no-op because LayoutViewEngine relies entirely on Partial view resolution
            // Partial Paths - 
            //   {area}/{controller}/

            // for "routed" request views...
            // enable /Views/{area}/{controller}/{viewName}

            // enable /Views/{partialName}
            // enable /Views/"DisplayTemplates/"+{templateName}
            // enable /Views/"EditorTemplates/+{templateName}
            var partialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{0}.ascx",
                parameters.VirtualPath + "/Views/{0}.aspx",
            };

            //Logger.Debug("PartialViewLocationFormats (theme): \r\n\t-{0}", string.Join("\r\n\t-", partialViewLocationFormats));

            var areaPartialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{2}/{1}/{0}.ascx",
                parameters.VirtualPath + "/Views/{2}/{1}/{0}.aspx",
            };

            //Logger.Debug("AreaPartialViewLocationFormats (theme): \r\n\t-{0}", string.Join("\r\n\t-", areaPartialViewLocationFormats));

            var viewEngine = new WebFormViewEngine {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = DisabledFormats,
                PartialViewLocationFormats = partialViewLocationFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = DisabledFormats,
                AreaPartialViewLocationFormats = areaPartialViewLocationFormats,
                ViewLocationCache = new ThemeViewLocationCache(parameters.VirtualPath),
            };

            return viewEngine;
        }

        public IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters) {
            var areaFormats = new[] {
                                        "~/Core/{2}/Views/{1}/{0}.ascx",
                                        "~/Core/{2}/Views/{1}/{0}.aspx",
                                        "~/Modules/{2}/Views/{1}/{0}.ascx",
                                        "~/Modules/{2}/Views/{1}/{0}.aspx",
                                        "~/Themes/{2}/Views/{1}/{0}.ascx",
                                        "~/Themes/{2}/Views/{1}/{0}.aspx",
                                    };

            //Logger.Debug("AreaFormats (module): \r\n\t-{0}", string.Join("\r\n\t-", areaFormats));

            var universalFormats = parameters.VirtualPaths
                .SelectMany(x => new[] {
                                           x + "/Views/{0}.ascx",
                                           x + "/Views/{0}.aspx",
                                       })
                .ToArray();

            //Logger.Debug("UniversalFormats (module): \r\n\t-{0}", string.Join("\r\n\t-", universalFormats));

            var viewEngine = new WebFormViewEngine {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = universalFormats,
                PartialViewLocationFormats = universalFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = areaFormats,
                AreaPartialViewLocationFormats = areaFormats,
            };

            return viewEngine;
        }


        public IViewEngine CreateBareViewEngine() {
            return new WebFormViewEngine {
                MasterLocationFormats = DisabledFormats,
                ViewLocationFormats = DisabledFormats,
                PartialViewLocationFormats = DisabledFormats,
                AreaMasterLocationFormats = DisabledFormats,
                AreaViewLocationFormats = DisabledFormats,
                AreaPartialViewLocationFormats = DisabledFormats,
            };
        }

        public IEnumerable<string> DetectTemplateFileNames(IEnumerable<string> fileNames) {
            return fileNames.Where(fileName => fileName.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase) ||
                                               fileName.EndsWith(".ascx", StringComparison.OrdinalIgnoreCase));
        }
    }
}
